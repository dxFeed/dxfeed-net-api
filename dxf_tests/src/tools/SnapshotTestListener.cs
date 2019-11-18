﻿#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.data;

namespace com.dxfeed.tests.tools
{
    /// <summary>
    /// Snapshots listener class for tests.
    /// Allow to get any parameters from received snapshots and transfer to test method.
    ///
    /// WARNING: this handler do not differ order snapshot by source,
    /// i.e. "Order#NTV AAPL" and "Order#DEX AAPL" is the same snapshots
    /// </summary>
    public class SnapshotTestListener :
        IDxOrderSnapshotListener,
        IDxCandleSnapshotListener,
        IDxTimeAndSaleSnapshotListener,
        IDxSpreadOrderSnapshotListener,
        IDxGreeksSnapshotListener,
        IDxSeriesSnapshotListener
    {
        public class ReceivedSnapshot<TE>
        {
            List<TE> events;

            public ReceivedSnapshot(string symbol, IList<TE> events)
            {
                Symbol = symbol.ToString();
                this.events = new List<TE>(events);
            }

            public string Symbol { get; private set; }
            public IList<TE> Events
            {
                get
                {
                    return this.events;
                }
            }
        }

        Dictionary<string, ReceivedSnapshot<IDxOrder>> orders = new Dictionary<string, ReceivedSnapshot<IDxOrder>>();
        Dictionary<string, ReceivedSnapshot<IDxCandle>> candles = new Dictionary<string, ReceivedSnapshot<IDxCandle>>();
        Dictionary<string, ReceivedSnapshot<IDxTimeAndSale>> timeAndSales = new Dictionary<string, ReceivedSnapshot<IDxTimeAndSale>>();
        Dictionary<string, ReceivedSnapshot<IDxSpreadOrder>> spreadOrders = new Dictionary<string, ReceivedSnapshot<IDxSpreadOrder>>();
        Dictionary<string, ReceivedSnapshot<IDxGreeks>> greeks = new Dictionary<string, ReceivedSnapshot<IDxGreeks>>();
        Dictionary<string, ReceivedSnapshot<IDxSeries>> series = new Dictionary<string, ReceivedSnapshot<IDxSeries>>();

        ReaderWriterLock rwl = new ReaderWriterLock();

        int lockTimeout = 1000;
        int eventsTimeout = 120000;
        int eventsSleepTime = 100;
        Func<bool> IsConnected = null;
        public const string COMPOSITE_BID = "COMPOSITE_BID";
        public const string COMPOSITE_ASK = "COMPOSITE_ASK";

        public SnapshotTestListener(int eventsTimeout, int eventsSleepTime, Func<bool> IsConnected)
        {
            this.eventsTimeout = eventsTimeout;
            this.eventsSleepTime = eventsSleepTime;
            this.IsConnected = IsConnected;
        }

        private Dictionary<string, ReceivedSnapshot<TE>> GetDictionary<TE>()
        {
            if (typeof(TE) == typeof(IDxOrder))
                return orders as Dictionary<string, ReceivedSnapshot<TE>>;
            else if (typeof(TE) == typeof(IDxCandle))
                return candles as Dictionary<string, ReceivedSnapshot<TE>>;
            else if (typeof(TE) == typeof(IDxTimeAndSale))
                return timeAndSales as Dictionary<string, ReceivedSnapshot<TE>>;
            else if (typeof(TE) == typeof(IDxSpreadOrder))
                return spreadOrders as Dictionary<string, ReceivedSnapshot<TE>>;
            else if (typeof(TE) == typeof(IDxGreeks))
                return greeks as Dictionary<string, ReceivedSnapshot<TE>>;
            else if (typeof(TE) == typeof(IDxSeries))
                return series as Dictionary<string, ReceivedSnapshot<TE>>;
            else
                return null;
        }

        private void AddSnapshot<TE>(ReceivedSnapshot<TE> snapshot)
        {
            rwl.AcquireWriterLock(lockTimeout);
            try
            {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                dict[snapshot.Symbol] = snapshot;
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        public bool HaveSnapshotEvents<TE>(string symbol, string source)
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                if (dict == null)
                    return false;
                bool isCompareSource = typeof(TE) == typeof(IDxOrder) && source != null && !source.Equals(string.Empty);
                foreach (ReceivedSnapshot<TE> snapshot in dict.Values)
                {
                    if (!snapshot.Symbol.Equals(symbol))
                        continue;
                    if (!isCompareSource)
                        return snapshot.Events.Count > 0;
                    foreach (IDxOrder order in snapshot.Events)
                    {
                        if (order.Source.Equals(source))
                            return true;
                    }
                }
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
            return false;
        }

        public void WaitSnapshot<TE>(string symbol, string source)
        {
            DateTime time = DateTime.Now;
            while (true)
            {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed!");
                if (HaveSnapshotEvents<TE>(symbol, source))
                    break;
                Thread.Sleep(eventsSleepTime);
            }
        }

        public void WaitSnapshot<TE>(string symbol)
        {
            WaitSnapshot<TE>(symbol, string.Empty);
        }

        public void ClearEvents<TE>()
        {
            rwl.AcquireWriterLock(lockTimeout);
            try
            {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                dict.Clear();
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Returns number of snapshot objects
        /// </summary>
        /// <typeparam name="TE">event type</typeparam>
        /// <returns>number of snapshot objects</returns>
        public int GetSnapshotsCount<TE>()
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                return dict.Count;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        public int GetSnapshotsCount<TE>(params string[] symbols)
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                Dictionary<string, ReceivedSnapshot<TE>> dict = GetDictionary<TE>();
                List<string> symbolList = new List<string>(symbols);
                int count = 0;
                foreach (ReceivedSnapshot<TE> s in dict.Values)
                    if (symbolList.Contains(s.Symbol))
                        count++;
                return count;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        #region IDxOrderSnapshotListener implementation

        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            List<IDxOrder> list = new List<IDxOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(new ReceivedSnapshot<IDxOrder>(buf.Symbol, list));
        }

        #endregion //IDxOrderSnapshotListener implementation end

        #region IDxCandleSnapshotListener implementation

        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            List<IDxCandle> list = new List<IDxCandle>();
            foreach (var c in buf)
                list.Add(c);
            AddSnapshot(new ReceivedSnapshot<IDxCandle>(buf.Symbol, list));
        }

        #endregion //IDxCandleSnapshotListener implementation end

        #region IDxTimeAndSaleSnapshotListener implementation

        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            List<IDxTimeAndSale> list = new List<IDxTimeAndSale>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(new ReceivedSnapshot<IDxTimeAndSale>(buf.Symbol, list));
        }

        #endregion //IDxTimeAndSaleSnapshotListener implementation end

        #region IDxSpreadOrderSnapshotListener implementation

        public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            List<IDxSpreadOrder> list = new List<IDxSpreadOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(new ReceivedSnapshot<IDxSpreadOrder>(buf.Symbol, list));
        }

        #endregion //IDxSpreadOrderSnapshotListener implementation end

        #region IDxGreeksSnapshotListener implementation

        public void OnGreeksSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            List<IDxGreeks> list = new List<IDxGreeks>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(new ReceivedSnapshot<IDxGreeks>(buf.Symbol, list));
        }

        #endregion //IDxGreeksSnapshotListener implementation end

        #region IDxSeriesSnapshotListener implementation

        public void OnSeriesSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            List<IDxSeries> list = new List<IDxSeries>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(new ReceivedSnapshot<IDxSeries>(buf.Symbol, list));
        }

        #endregion //IDxSeriesSnapshotListener implementation end

    }
}
