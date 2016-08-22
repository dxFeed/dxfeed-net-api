using com.dxfeed.io;
using System;
using System.Net;
using System.Threading;
using com.dxfeed.ipf.impl;
using com.dxfeed.api;
using System.Globalization;
using System.IO;

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
        private int updatePeriod;
        private string address = string.Empty;
        private object stateLocker = new object();
        private object lastModifiedLocker = new object();
        private Thread handlerThread; // != null when state in (CONNECTING, CONNECTED, COMPLETE)
        private DateTime lastModified = DateTime.MinValue;
        private bool supportsLive;
        private bool completed = false;

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
                using (Stream dataStream = webResponse.GetResponseStream()) {
                    URLInputStream.CheckConnectionResponseCode(webResponse);
                    DateTime time = ((HttpWebResponse)webResponse).LastModified;
                    if (time == lastModified)
                        return; // nothing changed
                    supportsLive = Constants.LIVE_PROP_RESPONSE.Equals(webResponse.Headers.Get(Constants.LIVE_PROP_KEY));
                    //TODO: commented log
                    //if (supportsLive)
                        //log.info("Live streaming connection has been open");
                    MakeConnected();
                    try (InputStream decompressedIn = StreamCompression.detectCompressionByHeaderAndDecompress(in)) {
                        int count = process(decompressedIn);
                        // Update timestamp only after first successful processing
                        lastModified = time;
                        //TODO: commented
                        //log.info("Downloaded " + count + " instrument profiles " +
                        //    (lastModified == 0 ? "" : " (last modified on " + TimeFormat.DEFAULT.format(lastModified) + ")"));
                    } finally {
                        // move this generation to old (if anything was received), so that we can drop those
                        // instruments when we receive new update later on
                        if (newGeneration != null) {
                            oldGenerations.add(newGeneration);
                            newGeneration = null;
                        }
                    }
                }
            } finally
                {
                    if (webResponse != null)
                        webResponse.Dispose();
                }
        }

    }
}
