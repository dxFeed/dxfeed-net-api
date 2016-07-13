using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.data;
using com.dxfeed.native;

namespace com.dxfeed.tests.tools {

    /// <summary>
    /// Snapshots listener class for tests.
    /// Allow to get any parameters from received snapshots and transfer to test method.
    /// 
    /// WARNING: this handler do not differ order snapshot by source, 
    /// i.e. "Order#NTV AAPL" and "Order#DEX AAPL" is the same snapshots
    /// </summary>
    public class SnapshotTestListener : IDxSnapshotListener {

        public class ReceivedSnapshot<TE> {
            List<TE> events;

            public ReceivedSnapshot(DxString symbol, IList<TE> events) {
                this.Symbol = symbol.ToString();
                this.events = new List<TE>(events);
            }

            public string Symbol { get; private set; }
            public IList<TE> Events {
                get {
                    return this.events;
                }
            }
        }

        Dictionary<string, ReceivedSnapshot<IDxOrder>> orders = new Dictionary<string, ReceivedSnapshot<IDxOrder>>();
        Dictionary<string, ReceivedSnapshot<IDxCandle>> candles = new Dictionary<string, ReceivedSnapshot<IDxCandle>>();

        ReaderWriterLock rwl = new ReaderWriterLock();

        int lockTimeout = 1000;
        int eventsTimeout = 120000;
        int eventsSleepTime = 100;
        Func<bool> IsConnected = null;

        public SnapshotTestListener(int eventsTimeout, int eventsSleepTime, Func<bool> IsConnected) {
            this.eventsTimeout = eventsTimeout;
            this.eventsSleepTime = eventsSleepTime;
            this.IsConnected = IsConnected;
        }

        private Dictionary<string, ReceivedSnapshot<TE>> GetDictionary<TE>() {
            if (typeof(TE) == typeof(IDxOrder))
                return orders as Dictionary<string, ReceivedSnapshot<TE>>;
            else if (typeof(TE) == typeof(IDxCandle))
                return candles as Dictionary<string, ReceivedSnapshot<TE>>;
            else
                return null;
        }

        private void AddSnapshot<TE>(ReceivedSnapshot<TE> snapshot) {
            rwl.AcquireWriterLock(lockTimeout);
            try {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                dict[snapshot.Symbol] = snapshot;
            } finally {
                rwl.ReleaseWriterLock();
            }
        }

        public bool HaveSnapshotEvents<TE>(string symbol, string source) {
            rwl.AcquireReaderLock(lockTimeout);
            try {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                if (dict == null)
                    return false;
                bool isCompareSource = typeof(TE) == typeof(IDxOrder) && source != null && !source.Equals(string.Empty);
                foreach (ReceivedSnapshot<TE> snapshot in dict.Values) {
                    if (!snapshot.Symbol.Equals(symbol))
                        continue;
                    if (!isCompareSource)
                        return snapshot.Events.Count > 0;
                    foreach (IDxOrder order in snapshot.Events) {
                        if (order.Source.Equals(source))
                            return true;
                    }
                }
            } finally {
                rwl.ReleaseReaderLock();
            }
            return false;
        }

        public void WaitSnapshot<TE>(string symbol, string source) {
            DateTime time = DateTime.Now;
            while(true) {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed!");
                if (HaveSnapshotEvents<TE>(symbol, source))
                    break;
                Thread.Sleep(eventsSleepTime);
            }
        }

        public void WaitSnapshot<TE>(string symbol) {
            WaitSnapshot<TE>(symbol, string.Empty);
        }

        public void ClearEvents<TE>() {
            rwl.AcquireWriterLock(lockTimeout);
            try {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                dict.Clear();
            } finally {
                rwl.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Returns number of snapshot objects
        /// </summary>
        /// <typeparam name="TE">event type</typeparam>
        /// <returns>number of snapshot objects</returns>
        public int GetSnapshotsCount<TE>() {
            rwl.AcquireReaderLock(lockTimeout);
            try {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                return dict.Count;
            } finally {
                rwl.ReleaseReaderLock();
            }
        }

        public int GetSnapshotsCount<TE>(params string[] symbols) {
            rwl.AcquireReaderLock(lockTimeout);
            try {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                List<string> symbolList = new List<string>(symbols);
                int count = 0;
                foreach (ReceivedSnapshot<TE> s in dict.Values)
                    if (symbolList.Contains(s.Symbol))
                        count++;
                return count;
            } finally {
                rwl.ReleaseReaderLock();
            }
        }

        #region IDxSnapshotListener implementation

        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            List<IDxOrder> list = new List<IDxOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot<IDxOrder>(new ReceivedSnapshot<IDxOrder>(buf.Symbol, list));
        }

        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            List<IDxCandle> list = new List<IDxCandle>();
            foreach (var c in buf)
                list.Add(c);
            AddSnapshot<IDxCandle>(new ReceivedSnapshot<IDxCandle>(buf.Symbol, list));
        }

        #endregion //IDxSnapshotListener implementation end
    }
}
