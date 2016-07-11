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

namespace com.dxfeed.tests.tools
{
    /// <summary>
    /// Event listener class for tests.
    /// Allow to get any parameters from received events and transfer to test method.
    /// </summary>
    public class TestListener : IDxFeedListener, IDxCandleListener {

        public class ReceivedEvent<TE> {
            public string Symbol { get; private set; }
            public EventParams EventParams { get; private set; }
            public TE Event { get; private set; }
            public ReceivedEvent(DxString symbol, EventParams eventParams, TE eventObj) {
                this.Symbol = symbol.ToString();
                this.EventParams = new EventParams(eventParams.Flags, eventParams.TimeIntField, eventParams.SnapshotKey);
                this.Event = eventObj;
            }
        }

        List<ReceivedEvent<IDxQuote>> quotes = new List<ReceivedEvent<IDxQuote>>();
        List<ReceivedEvent<IDxTrade>> trades = new List<ReceivedEvent<IDxTrade>>();
        List<ReceivedEvent<IDxOrder>> orders = new List<ReceivedEvent<IDxOrder>>();
        List<ReceivedEvent<IDxProfile>> profiles = new List<ReceivedEvent<IDxProfile>>();
        List<ReceivedEvent<IDxSummary>> summaries = new List<ReceivedEvent<IDxSummary>>();
        List<ReceivedEvent<IDxTimeAndSale>> timesAndSales = new List<ReceivedEvent<IDxTimeAndSale>>();
        List<ReceivedEvent<IDxCandle>> candles = new List<ReceivedEvent<IDxCandle>>();

        ReaderWriterLock rwl = new ReaderWriterLock();

        int lockTimeout = 1000;
        int eventsTimeout = 120000;
        int eventsSleepTime = 100;
        Func<bool> IsConnected = null;

        public TestListener(int eventsTimeout, int eventsSleepTime, Func<bool> IsConnected) {
            this.eventsTimeout = eventsTimeout;
            this.eventsSleepTime = eventsSleepTime;
            this.IsConnected = IsConnected;
        }

        private List<ReceivedEvent<TE>> GetList<TE>() {
            if (typeof(TE) == typeof(IDxQuote))
                return quotes as List<ReceivedEvent<TE>>;
            else if (typeof(TE) == typeof(IDxTrade))
                return trades as List<ReceivedEvent<TE>>;
            else if (typeof(TE) == typeof(IDxOrder))
                return orders as List<ReceivedEvent<TE>>;
            else if (typeof(TE) == typeof(IDxProfile))
                return profiles as List<ReceivedEvent<TE>>;
            else if (typeof(TE) == typeof(IDxSummary))
                return summaries as List<ReceivedEvent<TE>>;
            else if (typeof(TE) == typeof(IDxTimeAndSale))
                return timesAndSales as List<ReceivedEvent<TE>>;
            else if (typeof(TE) == typeof(IDxCandle))
                return candles as List<ReceivedEvent<TE>>;
            else
                return null;
        }

        public ReceivedEvent<TE> GetLastEvent<TE>() {
            rwl.AcquireReaderLock(lockTimeout);
            try {
                List<ReceivedEvent<TE>> list = GetList<TE>();
                if (list.Count == 0)
                    return null;
                return list[list.Count - 1];
            } finally {
                rwl.ReleaseReaderLock();
            }
        }

        private void AddEvent<TE>(ReceivedEvent<TE> newEvent) {
            rwl.AcquireWriterLock(lockTimeout);
            try {
                List<ReceivedEvent<TE>> list = GetList<TE>();
                list.Add(newEvent);
            } finally {
                rwl.ReleaseWriterLock();
            }
        }

        public void ClearEvents<TE>() {
            rwl.AcquireWriterLock(lockTimeout);
            try {
                List<ReceivedEvent<TE>> list = GetList<TE>();
                list.Clear();
            } finally {
                rwl.ReleaseWriterLock();
            }
        }

        public int GetEventCount<TE>() {
            rwl.AcquireReaderLock(lockTimeout);
            try {
                List<ReceivedEvent<TE>> list = GetList<TE>();
                return list.Count;
            } finally {
                rwl.ReleaseReaderLock();
            }
        }

        public int GetEventCount<TE>(params string[] symbols) {
            rwl.AcquireReaderLock(lockTimeout);
            try {
                List<ReceivedEvent<TE>> list = GetList<TE>();
                List<string> symbolList = new List<string>(symbols);
                int count = 0;
                foreach (ReceivedEvent<TE> ev in list)
                    if (symbolList.Contains(ev.Symbol))
                        count++;
                return count;
            } finally {
                rwl.ReleaseReaderLock();
            }
        }

        public void WaitEvents<TE>() {
            DateTime time = DateTime.Now;
            while(true) {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed!");
                if (GetEventCount<TE>() > 0)
                    break;
                Thread.Sleep(eventsSleepTime);
            }
        }

        public void WaitEvents<TE>(params string[] symbols) {
            List<string> symbolList = new List<string>(symbols);
            DateTime time = DateTime.Now;
            int lastIndex = 0;
            while(true) {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed! Not received events for next symbols: " + String.Join(", ", symbolList.ToArray()));

                List<ReceivedEvent<TE>> list = GetList<TE>();
                rwl.AcquireReaderLock(lockTimeout);
                int size = GetEventCount<TE>();
                for (int i = lastIndex; i < size; i++) {
                    if (symbolList.Contains(list[i].Symbol))
                        symbolList.Remove(list[i].Symbol);
                }
                lastIndex = size;
                rwl.ReleaseReaderLock();

                if (symbolList.Count == 0)
                    break;

                Thread.Sleep(eventsSleepTime);
            }
        }

        public void WaitOrders(params string[] sources) {
            List<string> sourceList = new List<string>(sources);
            DateTime time = DateTime.Now;
            int lastIndex = 0;
            while(true) {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed! Not received events for next symbols: " + String.Join(", ", sourceList.ToArray()));

                List<ReceivedEvent<IDxOrder>> list = GetList<IDxOrder>();
                rwl.AcquireReaderLock(lockTimeout);
                int size = GetEventCount<IDxOrder>();
                for (int i = lastIndex; i < size; i++) {
                    IDxOrder order = list[i].Event;
                    if (sourceList.Contains(order.Source))
                        sourceList.Remove(order.Source);
                }
                lastIndex = size;
                rwl.ReleaseReaderLock();

                if (sourceList.Count == 0)
                    break;

                Thread.Sleep(eventsSleepTime);
            }
        }

        #region Implementation of IDxFeedListener

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote {
                foreach (var q in buf)
                    AddEvent<IDxQuote>(new ReceivedEvent<IDxQuote>(buf.Symbol, buf.EventParams, q));
        }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade {
                foreach (var t in buf)
                    AddEvent<IDxTrade>(new ReceivedEvent<IDxTrade>(buf.Symbol, buf.EventParams, t));
        }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
                foreach (var o in buf)
                    AddEvent<IDxOrder>(new ReceivedEvent<IDxOrder>(buf.Symbol, buf.EventParams, o));
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile {
                foreach (var p in buf)
                    AddEvent<IDxProfile>(new ReceivedEvent<IDxProfile>(buf.Symbol, buf.EventParams, p));
        }

        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary {
                foreach (var f in buf)
                    AddEvent<IDxSummary>(new ReceivedEvent<IDxSummary>(buf.Symbol, buf.EventParams, f));
        }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale {
                foreach (var ts in buf)
                    AddEvent<IDxTimeAndSale>(new ReceivedEvent<IDxTimeAndSale>(buf.Symbol, buf.EventParams, ts));
        }

        #endregion

        #region Implementation of IDxCandleListener

        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle {
                foreach (var c in buf)
                    AddEvent<IDxCandle>(new ReceivedEvent<IDxCandle>(buf.Symbol, buf.EventParams, c));
            }

        #endregion
    }
}
