using com.dxfeed.io;
using System;
using System.Net;
using System.Threading;
using com.dxfeed.ipf.impl;
using com.dxfeed.api;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace com.dxfeed.ipf.live {

    //TODO: comment
    public class InstrumentProfileConnection {

        private static readonly string IF_MODIFIED_SINCE = "If-Modified-Since";
        private static readonly string HTTP_DATE_FORMAT = "ddd, dd MMM yyyy HH:mm:ss zzz";

        /// <summary>
        /// Instrument profile connection state.
        /// </summary>
        public enum State {
            /// <summary>
            /// Instrument profile connection is not started yet.
            /// InstrumentProfileConnection#start() was not invoked yet.
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

        private State state = State.NotConnected;
        private int updatePeriod = 60000;
        private string address = string.Empty;
        private object stateLocker = new object();
        private object lastModifiedLocker = new object();
        private object listenersLocker = new object();
        private Thread handlerThread; // != null when state in (CONNECTING, CONNECTED, COMPLETE)
        private DateTime lastModified = DateTime.MinValue;
        private bool supportsLive;
        private bool completed = false;
        private List<InstrumentProfileUpdateListener> listeners = new List<InstrumentProfileUpdateListener>();
        private List<InstrumentProfile> instrumentProfiles = new List<InstrumentProfile>();
        private InstrumentProfileUpdater updater = new InstrumentProfileUpdater();

        /// <summary>
        /// Creates instrument profile connection with a specified address.
        /// Address may be just "<host>:<port>" of server, URL, or a file path.
        /// The "[update=<period>]" clause can be optionally added at the end of the address to
        /// specify an #UpdatePeriod via an address string.
        /// Default update period is 1 minute.
        /// Connection needs to be started to begin an actual operation.
        /// </summary>
        /// <param name="address">Address of server</param>
        public InstrumentProfileConnection(string address) {
            this.address = address;
        }

        /// <summary>
        /// Gets or sets update period in milliseconds.
        /// It is period of an update check when the instrument profiles source does not support live updates
        /// and/or when connection is dropped.
        /// Default update period is 1 minute.
        /// </summary>
        public int UpdatePeriod {
            get {
                return Thread.VolatileRead(ref updatePeriod);
            }
            set {
                Interlocked.Exchange(ref updatePeriod, value);
            }
        }

        /// <summary>
        /// Returns state of this instrument profile connections.
        /// </summary>
        public State CurrentState {
            get {
                State currentState = State.Closed;
                lock (stateLocker) {
                    currentState = state;
                }
                return currentState;
            }
        }
        
        /// <summary>
        /// Get or set modification time of instrument profiles or DateTime.MinValue if it is unknown.
        /// </summary>
        public DateTime LastModified {
            get {
                DateTime value;
                lock (lastModifiedLocker) {
                    value = lastModified;
                }
                return value;
            }
            private set {
                lock (lastModifiedLocker) {
                    lastModified = value;
                }
            }
        }

        /// <summary>
        /// Starts this instrument profile connection. This connection's state immediately changes to
        /// State#Connecting and the actual connection establishment proceeds in the background.
        /// </summary>
        public void Start() {
            lock(stateLocker) {
                if (state != State.NotConnected)
                    throw new InvalidOperationException("Invalid state " + state);
                handlerThread = new Thread(Handler);
                handlerThread.Name = ToString();
                handlerThread.Start();
                state = State.Connecting;
            }
        }

        /// <summary>
        /// Adds listener that is notified about any updates in the set of instrument profiles.
        /// If a set of instrument profiles is not empty, then this listener is immediately
        /// notified right from inside this add method.
        /// </summary>
        /// <param name="listener">Profile update listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void AddUpdateListener(InstrumentProfileUpdateListener listener) {
            if (listener == null)
                throw new ArgumentNullException("null listener");
            lock(listenersLocker) {
                if (!listeners.Contains(listener))
                    listeners.Add(listener);
                listener.InstrumentProfilesUpdated(updater.InstrumentProfiles);
            }
        }

        /// <summary>
        /// Removes listener that is notified about any updates in the set of instrument profiles.
        /// </summary>
        /// <param name="listener">Profile update listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void RemoveUpdateListener(InstrumentProfileUpdateListener listener) {
            if (listener == null)
                throw new ArgumentNullException("null listener");
            lock (listenersLocker) {
                listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString() {
            // Note: it is also used as a thread name.
            return "IPC:" + address;
        }

        private void Handler() {
            while (CurrentState != State.Closed) {
                try {
                    Download();
                } catch (Exception) {
                    //TODO: error handling
                }
                // wait before retrying
                Thread.Sleep(UpdatePeriod);
            }
        }

        private void MakeConnected() {
            lock (stateLocker) {
                if (state == State.Connecting)
                    state = State.Connected;
            }
        }

        private void MakeComplete() {
            lock (stateLocker) {
                if (state == State.Connected) {
                    completed = true;
                    state = State.Completed;
                }
            }
        }

        private void Download() {
            WebResponse webResponse = null;
            try { 
                WebRequest webRequest = URLInputStream.OpenConnection(address);
                webRequest.Headers.Add(Constants.LIVE_PROP_KEY, Constants.LIVE_PROP_REQUEST_YES);
                if (LastModified != DateTime.MinValue && !supportsLive &&
                    webRequest.GetType() == typeof(HttpWebRequest)) {

                    // Use If-Modified-Since
                    webRequest.Headers.Add(IF_MODIFIED_SINCE, lastModified.ToString(HTTP_DATE_FORMAT, new CultureInfo("en-US")));
                    webResponse = webRequest.GetResponse();
                    if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.NotModified)
                        return; // not modified
                } else {
                    webResponse = webRequest.GetResponse();
                }
                using (Stream inputStream = webResponse.GetResponseStream()) {
                    URLInputStream.CheckConnectionResponseCode(webResponse);
                    DateTime time = ((HttpWebResponse)webResponse).LastModified;
                    if (time == LastModified)
                        return; // nothing changed
                    supportsLive = Constants.LIVE_PROP_RESPONSE.Equals(webResponse.Headers.Get(Constants.LIVE_PROP_KEY));
                    MakeConnected();
                    using (Stream decompressedIn = StreamCompression.DetectCompressionByHeaderAndDecompress(inputStream)) {
                        int count = process(decompressedIn);
                        // Update timestamp only after first successful processing
                        LastModified = time;
                    } 
                }
            } finally {
                if (webResponse != null)
                    webResponse.Dispose();
            }
        }


        private int process(Stream inputStream) {
            int count = 0;
            InstrumentProfileParser parser = new InstrumentProfileParser(inputStream);
            parser.OnFlush += Flush;
            parser.OnComplete += Complete;
            InstrumentProfile ip;
            while ((ip = parser.Next()) != null) {
                count++;
                instrumentProfiles.Add(ip);
            }

            //Flush(this, new EventArgs());
            // EOF of live connection is _NOT_ a signal that snapshot was complete (it sends an explicit complete)
            // for non-live data sources, though, EOF is a completion signal
            if (!supportsLive)
                Complete(this, new EventArgs());
            return count;
        }

        private void Flush(object sender, EventArgs e) {
            if (instrumentProfiles.Count == 0)
                return;
            ICollection<InstrumentProfile> updateList = updater.Update(instrumentProfiles);
            CallListeners(updateList);
            instrumentProfiles.Clear();
        }

        private void Complete(object sender, EventArgs e) {
            Flush(this, e);
            MakeComplete();
        }

        private void CallListeners(ICollection<InstrumentProfile> instrumentProfiles) {
            lock(listenersLocker) {
                foreach (InstrumentProfileUpdateListener listener in listeners) {
                    listener.InstrumentProfilesUpdated(instrumentProfiles);
                }
            }
        }

    }
}
