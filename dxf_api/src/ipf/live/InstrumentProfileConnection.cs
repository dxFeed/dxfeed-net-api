/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.io;
using com.dxfeed.ipf.impl;
using com.dxfeed.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace com.dxfeed.ipf.live
{
    /// <summary>
    /// Connects to an instrument profile URL and reads instrument profiles using Simple File Format with support of
    /// streaming live updates.
    /// Please see Instrument Profile Format documentation for complete description.
    ///
    /// The key different between this class and InstrumentProfileReader is that the later just reads
    /// a snapshot of a set of instrument profiles, while this classes allows to track live updates, e.g.
    /// addition and removal of instruments.
    ///
    /// To use this class you need an address of the data source from you data provider. The name of the IPF file can
    /// also serve as an address for debugging purposes.
    ///
    /// The recommended usage of this class to receive a live stream of instrument profile updates is:
    ///
    ///     class UpdateListener : InstrumentProfileUpdateListener {
    ///         public void InstrumentProfilesUpdated(ICollection<InstrumentProfile> instruments) {
    ///             foreach (InstrumentProfile ip in instruments) {
    ///                 // do something with instrument here.
    ///             }
    ///         }
    ///     }
    ///
    ///     class Program {
    ///         static void Main(string[] args) {
    ///             string address = "<host>:<port>";
    ///             InstrumentProfileConnection connection = new InstrumentProfileConnection(path);
    ///             UpdateListener updateListener = new UpdateListener();
    ///             connection.AddUpdateListener(updateListener);
    ///             connection.Start();
    ///         }
    ///     }
    ///
    /// If long-running processing of instrument profile is needed, then it is better to use
    /// InstrumentProfileUpdateListener.InstrumentProfilesUpdated notification
    /// to schedule processing task in a separate thread.
    ///
    //// This class is thread-safe.
    /// </summary>
    public class InstrumentProfileConnection
    {
        public delegate void OnErrorEventHandler(object sender, ErrorEventArgs e);
        public event OnErrorEventHandler OnError;

        /// <summary>
        /// Instrument profile connection state.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Instrument profile connection is not started yet.
            /// InstrumentProfileConnection.Start() was not invoked yet.
            /// </summary>
            NotConnected,

            /// <summary>
            /// Connection is being established.
            /// </summary>
            Connecting,

            /// <summary>
            /// Connection was established.
            /// </summary>
            Connected,

            /// <summary>
            /// Initial instrument profiles snapshot was fully read (this state is set only once).
            /// </summary>
            Completed,

            /// <summary>
            /// Instrument profile connection was closed.
            /// </summary>
            Closed
        }

        private static readonly string UPDATE_PATTERN = "(.*)\\[update=([^\\]]+)\\]";
        private static readonly long DEFAULT_UPDATE_PERIOD = 60000;
        private State state = State.NotConnected;
        private long updatePeriod = DEFAULT_UPDATE_PERIOD;
        private string address = string.Empty;
        private object stateLocker = new object();
        private object lastModifiedLocker = new object();
        private object listenersLocker = new object();
        /// <summary>
        /// Working thread to download instruments.
        /// Thread must != null when state in (CONNECTING, CONNECTED, COMPLETE).
        /// </summary>
        private Thread handlerThread;
        private DateTime lastModified = DateTime.MinValue;
        private bool supportsLive = false;
        private List<InstrumentProfileUpdateListener> listeners = new List<InstrumentProfileUpdateListener>();
        private List<InstrumentProfile> ipBuffer = new List<InstrumentProfile>();
        private InstrumentProfileUpdater updater = new InstrumentProfileUpdater();

        /// <summary>
        /// Creates instrument profile connection with a specified address.
        /// Address may be just "<host>:<port>" of server, URL, or a file path.
        /// The "[update=<period>]" clause can be optionally added at the end of the address to
        /// specify an UpdatePeriod via an address string.
        /// Default update period is 1 minute.
        /// Connection needs to be started to begin an actual operation.
        /// </summary>
        /// <param name="address">Address of server</param>
        public InstrumentProfileConnection(string address)
        {
            this.address = address;
            Regex regex = new Regex(UPDATE_PATTERN, RegexOptions.IgnoreCase);
            Match match = regex.Match(address);
            if (match.Success)
            {
                this.address = match.Groups[1].ToString();
                updatePeriod = TimePeriod.ValueOf(match.Groups[2].ToString()).GetTime();
            }
        }

        /// <summary>
        /// Gets or sets update period in milliseconds.
        /// It is period of an update check when the instrument profiles source does not support live updates
        /// and/or when connection is dropped.
        /// Default update period is 1 minute.
        /// </summary>
        public long UpdatePeriod
        {
            get
            {
                return Thread.VolatileRead(ref updatePeriod);
            }
            set
            {
                Interlocked.Exchange(ref updatePeriod, value);
            }
        }

        /// <summary>
        /// Returns state of this instrument profile connections.
        /// </summary>
        private State CurrentState
        {
            get
            {
                State currentState = State.Closed;
                lock (stateLocker)
                {
                    currentState = state;
                }
                return currentState;
            }
        }

        /// <summary>
        /// Get or set modification time of instrument profiles or DateTime.MinValue if it is unknown.
        /// </summary>
        private DateTime LastModified
        {
            get
            {
                DateTime value;
                lock (lastModifiedLocker)
                {
                    value = lastModified;
                }
                return value;
            }
            set
            {
                lock (lastModifiedLocker)
                {
                    lastModified = value;
                }
            }
        }

        /// <summary>
        /// Starts this instrument profile connection. This connection's state immediately changes to
        /// State.Connecting and the actual connection establishment proceeds in the background.
        /// </summary>
        public void Start()
        {
            lock (stateLocker)
            {
                if (state != State.NotConnected)
                    throw new InvalidOperationException("Invalid state " + state);
                handlerThread = new Thread(Handler);
                handlerThread.Name = ToString();
                handlerThread.Start();
                state = State.Connecting;
            }
        }

        /// <summary>
        /// Closes this instrument profile connection. This connection's state immediately changes to
        /// State.Closed and the background update procedures are terminated.
        /// </summary>
        public void Close()
        {
            lock (stateLocker)
            {
                if (state == State.Closed)
                    return;
                state = State.Closed;
            }
        }

        /// <summary>
        /// Adds listener that is notified about any updates in the set of instrument profiles.
        /// If a set of instrument profiles is not empty, then this listener is immediately
        /// notified right from inside this add method.
        /// </summary>
        /// <param name="listener">Profile update listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void AddUpdateListener(InstrumentProfileUpdateListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("null listener");
            lock (listenersLocker)
            {
                if (!listeners.Contains(listener))
                    listeners.Add(listener);
                CheckAndCallListener(listener, updater.InstrumentProfiles);
            }
        }

        /// <summary>
        /// Removes listener that is notified about any updates in the set of instrument profiles.
        /// </summary>
        /// <param name="listener">Profile update listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void RemoveUpdateListener(InstrumentProfileUpdateListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("null listener");
            lock (listenersLocker)
            {
                listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            // Note: it is also used as a thread name.
            return "IPC:" + address;
        }

        /// <summary>
        /// Method of worker thread. Runs downloading process by specified update period.
        /// </summary>
        private void Handler()
        {
            DateTime downloadStart = DateTime.MinValue;
            while (CurrentState != State.Closed)
            {
                // wait before retrying
                if (DateTime.Now.Subtract(downloadStart).TotalMilliseconds > UpdatePeriod)
                {
                    try
                    {
                        Download();
                    }
                    catch (Exception e)
                    {
                        CallOnError(e);
                    }
                    downloadStart = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Change connection status to connected.
        /// </summary>
        private void MakeConnected()
        {
            lock (stateLocker)
            {
                if (state == State.Connecting)
                    state = State.Connected;
            }
        }

        /// <summary>
        /// Change connection status to completed.
        /// </summary>
        private void MakeComplete()
        {
            lock (stateLocker)
            {
                if (state == State.Connected)
                {
                    state = State.Completed;
                }
            }
        }

        /// <summary>
        /// Opens (or keeps) connection for receiving instrument profiles from server.
        /// </summary>
        private void Download()
        {
            WebRequest webRequest = URLInputStream.OpenConnection(address);
            webRequest.Headers.Add(Constants.LIVE_PROP_KEY, Constants.LIVE_PROP_REQUEST_YES);
            if (LastModified != DateTime.MinValue && !supportsLive &&
                webRequest.GetType() == typeof(HttpWebRequest))
            {

                ((HttpWebRequest)webRequest).IfModifiedSince = lastModified;
            }

            try
            {
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    bool isFileStream = webResponse.GetType() == typeof(FileWebResponse);
                    DateTime time;
                    if (isFileStream)
                    {
                        Uri fileUri = new Uri(address);
                        time = File.GetLastWriteTime(fileUri.AbsolutePath);
                    }
                    else
                    {
                        URLInputStream.CheckConnectionResponseCode(webResponse);
                        time = ((HttpWebResponse)webResponse).LastModified;
                        supportsLive = Constants.LIVE_PROP_RESPONSE.Equals(webResponse.Headers.Get(Constants.LIVE_PROP_KEY));
                    }
                    if (time == LastModified)
                        return; // nothing changed
                    MakeConnected();
                    using (Stream inputStream = webResponse.GetResponseStream())
                    {
                        StreamCompression compress = (isFileStream ? StreamCompression.DetectCompressionByExtension(new Uri(address)) :
                            StreamCompression.DetectCompressionByMimeType(webResponse.ContentType));
                        using (Stream decompressedIn = compress.Decompress(inputStream))
                        {
                            int count = process(decompressedIn);
                            // Update timestamp only after first successful processing
                            LastModified = time;
                        }
                    }
                }
            }
            catch (WebException we)
            {
                using (WebResponse webResponse = we.Response)
                {
                    if (webResponse != null && webResponse.GetType() == typeof(HttpWebResponse) &&
                        ((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.NotModified)
                        return; // not modified
                }
                throw;
            }
        }

        /// <summary>
        /// Instrument profiles parser operations.
        /// </summary>
        /// <param name="inputStream">Decompressed input stream of instrument profiles.</param>
        /// <returns>Number of received instrument profiles.</returns>
        private int process(Stream inputStream)
        {
            int count = 0;
            InstrumentProfileParser parser = new InstrumentProfileParser(inputStream);
            parser.OnFlush += Flush;
            parser.OnComplete += Complete;
            InstrumentProfile ip;
            while ((ip = parser.Next()) != null)
            {
                count++;
                ipBuffer.Add(ip);
                if (CurrentState == State.Closed)
                    return count;
            }

            // EOF of live connection is _NOT_ a signal that snapshot was complete (it sends an explicit complete)
            // for non-live data sources, though, EOF is a completion signal
            if (!supportsLive)
                Complete(this, new EventArgs());
            return count;
        }

        /// <summary>
        /// Sends updates to listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Flush(object sender, EventArgs e)
        {
            if (ipBuffer.Count == 0)
                return;
            ICollection<InstrumentProfile> updateList = updater.Update(ipBuffer);
            CallListeners(updateList);
            ipBuffer.Clear();
        }

        /// <summary>
        /// Set status of completed snapshot and sends updates to listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Complete(object sender, EventArgs e)
        {
            MakeComplete();
            Flush(this, e);
        }

        /// <summary>
        /// Check that instrument profiles update is not empty and call listener.
        /// </summary>
        /// <param name="listener">Listener to call.</param>
        /// <param name="instrumentProfiles">Instrument profiles updates.</param>
        private void CheckAndCallListener(InstrumentProfileUpdateListener listener,
            ICollection<InstrumentProfile> instrumentProfiles)
        {

            if (instrumentProfiles == null || instrumentProfiles.Count == 0)
                return;
            listener.InstrumentProfilesUpdated(instrumentProfiles);
        }

        /// <summary>
        /// Iterate through listeners and call them.
        /// </summary>
        /// <param name="instrumentProfiles"></param>
        private void CallListeners(ICollection<InstrumentProfile> instrumentProfiles)
        {
            lock (listenersLocker)
            {
                foreach (InstrumentProfileUpdateListener listener in listeners)
                {
                    CheckAndCallListener(listener, instrumentProfiles);
                }
            }
        }

        /// <summary>
        /// Call error listener.
        /// </summary>
        /// <param name="e">Exception object.</param>
        private void CallOnError(Exception e)
        {
            lock (listenersLocker)
            {
                OnError?.Invoke(this, new ErrorEventArgs(e));
            }
        }
    }
}
