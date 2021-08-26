#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using com.dxfeed.api;
using com.dxfeed.api.events;
using NUnit.Framework;

namespace com.dxfeed.tests.tools
{
    /// <summary>
    ///     Event listener class for tests.
    ///     Allow to get any parameters from received events and transfer to test method.
    /// </summary>
    public class TestListener :
        IDxFeedListener,
        IDxCandleListener,
        IDxTradeETHListener,
        IDxSpreadOrderListener,
        IDxGreeksListener,
        IDxTheoPriceListener,
        IDxUnderlyingListener,
        IDxSeriesListener,
        IDxConfigurationListener
    {
        private readonly List<ReceivedEvent<IDxCandle>> candles = new List<ReceivedEvent<IDxCandle>>();

        private readonly List<ReceivedEvent<IDxConfiguration>> configurations =
            new List<ReceivedEvent<IDxConfiguration>>();

        private readonly int eventsSleepTime = 100;
        private readonly int eventsTimeout = 120000;
        private readonly List<ReceivedEvent<IDxGreeks>> greeks = new List<ReceivedEvent<IDxGreeks>>();
        private readonly Func<bool> IsConnected;

        private readonly int lockTimeout = 1000;
        private readonly List<ReceivedEvent<IDxOrder>> orders = new List<ReceivedEvent<IDxOrder>>();
        private readonly List<ReceivedEvent<IDxProfile>> profiles = new List<ReceivedEvent<IDxProfile>>();

        private readonly List<ReceivedEvent<IDxQuote>> quotes = new List<ReceivedEvent<IDxQuote>>();

        private readonly ReaderWriterLock rwl = new ReaderWriterLock();
        private readonly List<ReceivedEvent<IDxSeries>> series = new List<ReceivedEvent<IDxSeries>>();
        private readonly List<ReceivedEvent<IDxSpreadOrder>> spreadOrders = new List<ReceivedEvent<IDxSpreadOrder>>();
        private readonly List<ReceivedEvent<IDxSummary>> summaries = new List<ReceivedEvent<IDxSummary>>();
        private readonly List<ReceivedEvent<IDxTheoPrice>> theoPrice = new List<ReceivedEvent<IDxTheoPrice>>();
        private readonly List<ReceivedEvent<IDxTimeAndSale>> timesAndSales = new List<ReceivedEvent<IDxTimeAndSale>>();
        private readonly List<ReceivedEvent<IDxTrade>> trades = new List<ReceivedEvent<IDxTrade>>();
        private readonly List<ReceivedEvent<IDxTradeETH>> tradesETH = new List<ReceivedEvent<IDxTradeETH>>();
        private readonly List<ReceivedEvent<IDxUnderlying>> underlying = new List<ReceivedEvent<IDxUnderlying>>();

        public TestListener()
        {
        }

        public TestListener(int eventsTimeout, int eventsSleepTime, Func<bool> IsConnected)
        {
            this.eventsTimeout = eventsTimeout;
            this.eventsSleepTime = eventsSleepTime;
            this.IsConnected = IsConnected;
        }

        #region Implementation of IDxCandleListener

        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            foreach (var c in buf)
                AddEvent(new ReceivedEvent<IDxCandle>(buf.Symbol, buf.EventParams, c));
        }

        #endregion

        #region Implementation of IDxConfigurationListener

        public void OnConfiguration<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxConfiguration
        {
            foreach (var c in buf)
                AddEvent(new ReceivedEvent<IDxConfiguration>(buf.Symbol, buf.EventParams, c));
        }

        #endregion

        #region Implementation of IDxGreeksListener

        public void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            foreach (var g in buf)
                AddEvent(new ReceivedEvent<IDxGreeks>(buf.Symbol, buf.EventParams, g));
        }

        #endregion

        #region Implementation of IDxSeriesListener

        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            foreach (var s in buf)
                AddEvent(new ReceivedEvent<IDxSeries>(buf.Symbol, buf.EventParams, s));
        }

        #endregion

        #region Implementation of IDxSpreadOrderListener

        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            foreach (var o in buf)
                AddEvent(new ReceivedEvent<IDxSpreadOrder>(buf.Symbol, buf.EventParams, o));
        }

        #endregion

        #region Implementation of IDxTheoPriceListener

        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
            foreach (var tp in buf)
                AddEvent(new ReceivedEvent<IDxTheoPrice>(buf.Symbol, buf.EventParams, tp));
        }

        #endregion

        #region Implementation of IDxTradeETHListener

        public void OnTradeETH<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeETH
        {
            foreach (var te in buf)
                AddEvent(new ReceivedEvent<IDxTradeETH>(buf.Symbol, buf.EventParams, te));
        }

        #endregion

        #region Implementation of IDxUnderlyingListener

        public void OnUnderlying<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxUnderlying
        {
            foreach (var u in buf)
                AddEvent(new ReceivedEvent<IDxUnderlying>(buf.Symbol, buf.EventParams, u));
        }

        #endregion

        private List<ReceivedEvent<TE>> GetList<TE>()
        {
            var type = typeof(TE);
            if (typeof(IDxQuote).IsAssignableFrom(type))
                return quotes as List<ReceivedEvent<TE>>;
            if (typeof(IDxTrade).IsAssignableFrom(type))
                return trades as List<ReceivedEvent<TE>>;
            if (typeof(IDxOrder).IsAssignableFrom(type))
                return orders as List<ReceivedEvent<TE>>;
            if (typeof(IDxProfile).IsAssignableFrom(type))
                return profiles as List<ReceivedEvent<TE>>;
            if (typeof(IDxSummary).IsAssignableFrom(type))
                return summaries as List<ReceivedEvent<TE>>;
            if (typeof(IDxTimeAndSale).IsAssignableFrom(type))
                return timesAndSales as List<ReceivedEvent<TE>>;
            if (typeof(IDxCandle).IsAssignableFrom(type))
                return candles as List<ReceivedEvent<TE>>;
            if (typeof(IDxTradeETH).IsAssignableFrom(type))
                return tradesETH as List<ReceivedEvent<TE>>;
            if (typeof(IDxSpreadOrder).IsAssignableFrom(type))
                return spreadOrders as List<ReceivedEvent<TE>>;
            if (typeof(IDxGreeks).IsAssignableFrom(type))
                return greeks as List<ReceivedEvent<TE>>;
            if (typeof(IDxTheoPrice).IsAssignableFrom(type))
                return theoPrice as List<ReceivedEvent<TE>>;
            if (typeof(IDxUnderlying).IsAssignableFrom(type))
                return underlying as List<ReceivedEvent<TE>>;
            if (typeof(IDxSeries).IsAssignableFrom(type))
                return series as List<ReceivedEvent<TE>>;
            if (typeof(IDxConfiguration).IsAssignableFrom(type))
                return configurations as List<ReceivedEvent<TE>>;
            throw new ArgumentException(string.Format("Unknown event type: {0}", type));
        }

        public ReceivedEvent<TE> GetLastEvent<TE>()
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                var list = GetList<TE>();
                if (list.Count == 0)
                    return null;
                return list[list.Count - 1];
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        private void AddEvent<TE>(ReceivedEvent<TE> newEvent)
        {
            rwl.AcquireWriterLock(lockTimeout);
            try
            {
                var list = GetList<TE>();
                list.Add(newEvent);
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        public void ClearEvents<TE>()
        {
            rwl.AcquireWriterLock(lockTimeout);
            try
            {
                var list = GetList<TE>();
                list.Clear();
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        public int GetEventCount<TE>()
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                var list = GetList<TE>();
                return list.Count;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        public int GetEventCount<TE>(params string[] symbols)
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                var list = GetList<TE>();
                var symbolList = new List<string>(symbols);
                var count = 0;
                foreach (var ev in list)
                    if (symbolList.Contains(ev.Symbol))
                        count++;
                return count;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        public void WaitEvents<TE>()
        {
            var time = DateTime.Now;
            while (true)
            {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed!");
                if (GetEventCount<TE>() > 0)
                    break;
                Thread.Sleep(eventsSleepTime);
            }
        }

        public void WaitEvents<TE>(params string[] symbols)
        {
            var symbolList = new List<string>(symbols);
            var time = DateTime.Now;
            var lastIndex = 0;
            while (true)
            {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed! Not received events for next symbols: " +
                                string.Join(", ", symbolList.ToArray()));

                var list = GetList<TE>();
                rwl.AcquireReaderLock(lockTimeout);
                var size = GetEventCount<TE>();
                for (var i = lastIndex; i < size; i++)
                    if (symbolList.Contains(list[i].Symbol))
                        symbolList.Remove(list[i].Symbol);
                lastIndex = size;
                rwl.ReleaseReaderLock();

                if (symbolList.Count == 0)
                    break;

                Thread.Sleep(eventsSleepTime);
            }
        }

        public void WaitOrders(params string[] sources)
        {
            var sourceList = new List<string>(sources);
            var time = DateTime.Now;
            var lastIndex = 0;
            while (true)
            {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed! Not received events for next symbols: " +
                                string.Join(", ", sourceList.ToArray()));

                var list = GetList<IDxOrder>();
                rwl.AcquireReaderLock(lockTimeout);
                var size = GetEventCount<IDxOrder>();
                for (var i = lastIndex; i < size; i++)
                {
                    var order = list[i].Event;
                    if (sourceList.Contains(order.Source.Name))
                        sourceList.Remove(order.Source.Name);
                }

                lastIndex = size;
                rwl.ReleaseReaderLock();

                if (sourceList.Count == 0)
                    break;

                Thread.Sleep(eventsSleepTime);
            }
        }

        public int GetOrderCount(params string[] sources)
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                var list = GetList<IDxOrder>();
                var sourceList = new List<string>(sources);
                var count = 0;
                foreach (var ev in list)
                    if (sourceList.Contains(ev.Event.Source.Name))
                        count++;
                return count;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        public class ReceivedEvent<TE>
        {
            public ReceivedEvent(string symbol, EventParams eventParams, TE eventObj)
            {
                Symbol = symbol;
                EventParams = new EventParams(eventParams.Flags, eventParams.TimeIntField, eventParams.SnapshotKey);
                Event = eventObj;
            }

            public string Symbol { get; }
            public EventParams EventParams { get; }
            public TE Event { get; }
        }

        #region Implementation of IDxFeedListener

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
            foreach (var q in buf)
                AddEvent(new ReceivedEvent<IDxQuote>(buf.Symbol, buf.EventParams, q));
        }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {
            foreach (var t in buf)
                AddEvent(new ReceivedEvent<IDxTrade>(buf.Symbol, buf.EventParams, t));
        }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            foreach (var o in buf)
                AddEvent(new ReceivedEvent<IDxOrder>(buf.Symbol, buf.EventParams, o));
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {
            foreach (var p in buf)
                AddEvent(new ReceivedEvent<IDxProfile>(buf.Symbol, buf.EventParams, p));
        }

        public void OnSummary<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {
            foreach (var f in buf)
                AddEvent(new ReceivedEvent<IDxSummary>(buf.Symbol, buf.EventParams, f));
        }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            foreach (var ts in buf)
                AddEvent(new ReceivedEvent<IDxTimeAndSale>(buf.Symbol, buf.EventParams, ts));
        }

        #endregion
    }
}