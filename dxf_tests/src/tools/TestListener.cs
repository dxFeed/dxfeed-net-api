/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

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
    /// Event listener class for tests.
    /// Allow to get any parameters from received events and transfer to test method.
    /// </summary>
    public class TestListener :
        IDxFeedListener,
        IDxCandleListener,
        IDxTradeEthListener,
        IDxSpreadOrderListener,
        IDxGreeksListener,
        IDxTheoPriceListener,
        IDxUnderlyingListener,
        IDxSeriesListener,
        IDxConfigurationListener,
        IDXFeedEventListener<IDxCandle>,
        IDXFeedEventListener<IDxOrder>,
        IDXFeedEventListener<TimeSeriesEvent>,
        IDXFeedEventListener<IDxEventType>
    {
        public class ReceivedEvent<TE>
        {
            public string Symbol { get; private set; }
            public EventParams EventParams { get; private set; }
            public TE Event { get; private set; }
            public ReceivedEvent(DxString symbol, EventParams eventParams, TE eventObj)
            {
                Symbol = symbol.ToString();
                EventParams = new EventParams(eventParams.Flags, eventParams.TimeIntField, eventParams.SnapshotKey);
                Event = eventObj;
            }
            public ReceivedEvent(string symbol, EventParams eventParams, TE eventObj)
            {
                Symbol = symbol;
                EventParams = new EventParams(eventParams.Flags, eventParams.TimeIntField, eventParams.SnapshotKey);
                Event = eventObj;
            }
        }

        List<ReceivedEvent<IDxQuote>> quotes = new List<ReceivedEvent<IDxQuote>>();
        List<ReceivedEvent<IDxTrade>> trades = new List<ReceivedEvent<IDxTrade>>();
        List<ReceivedEvent<IDxOrder>> orders = new List<ReceivedEvent<IDxOrder>>();
        List<ReceivedEvent<IDxProfile>> profiles = new List<ReceivedEvent<IDxProfile>>();
        List<ReceivedEvent<IDxSummary>> summaries = new List<ReceivedEvent<IDxSummary>>();
        List<ReceivedEvent<IDxTimeAndSale>> timesAndSales = new List<ReceivedEvent<IDxTimeAndSale>>();
        List<ReceivedEvent<IDxCandle>> candles = new List<ReceivedEvent<IDxCandle>>();
        List<ReceivedEvent<IDxTradeEth>> tradesEth = new List<ReceivedEvent<IDxTradeEth>>();
        List<ReceivedEvent<IDxSpreadOrder>> spreadOrders = new List<ReceivedEvent<IDxSpreadOrder>>();
        List<ReceivedEvent<IDxGreeks>> greeks = new List<ReceivedEvent<IDxGreeks>>();
        List<ReceivedEvent<IDxTheoPrice>> theoPrice = new List<ReceivedEvent<IDxTheoPrice>>();
        List<ReceivedEvent<IDxUnderlying>> underlying = new List<ReceivedEvent<IDxUnderlying>>();
        List<ReceivedEvent<IDxSeries>> series = new List<ReceivedEvent<IDxSeries>>();
        List<ReceivedEvent<IDxConfiguration>> configurations = new List<ReceivedEvent<IDxConfiguration>>();

        ReaderWriterLock rwl = new ReaderWriterLock();

        int lockTimeout = 1000;
        int eventsTimeout = 120000;
        int eventsSleepTime = 100;
        Func<bool> IsConnected = null;

        public TestListener() { }

        public TestListener(int eventsTimeout, int eventsSleepTime, Func<bool> IsConnected)
        {
            this.eventsTimeout = eventsTimeout;
            this.eventsSleepTime = eventsSleepTime;
            this.IsConnected = IsConnected;
        }

        private List<ReceivedEvent<TE>> GetList<TE>()
        {
            Type type = typeof(TE);
            if (typeof(IDxQuote).IsAssignableFrom(type))
                return quotes as List<ReceivedEvent<TE>>;
            else if (typeof(IDxTrade).IsAssignableFrom(type))
                return trades as List<ReceivedEvent<TE>>;
            else if (typeof(IDxOrder).IsAssignableFrom(type))
                return orders as List<ReceivedEvent<TE>>;
            else if (typeof(IDxProfile).IsAssignableFrom(type))
                return profiles as List<ReceivedEvent<TE>>;
            else if (typeof(IDxSummary).IsAssignableFrom(type))
                return summaries as List<ReceivedEvent<TE>>;
            else if (typeof(IDxTimeAndSale).IsAssignableFrom(type))
                return timesAndSales as List<ReceivedEvent<TE>>;
            else if (typeof(IDxCandle).IsAssignableFrom(type))
                return candles as List<ReceivedEvent<TE>>;
            else if (typeof(IDxTradeEth).IsAssignableFrom(type))
                return tradesEth as List<ReceivedEvent<TE>>;
            else if (typeof(IDxSpreadOrder).IsAssignableFrom(type))
                return spreadOrders as List<ReceivedEvent<TE>>;
            else if (typeof(IDxGreeks).IsAssignableFrom(type))
                return greeks as List<ReceivedEvent<TE>>;
            else if (typeof(IDxTheoPrice).IsAssignableFrom(type))
                return theoPrice as List<ReceivedEvent<TE>>;
            else if (typeof(IDxUnderlying).IsAssignableFrom(type))
                return underlying as List<ReceivedEvent<TE>>;
            else if (typeof(IDxSeries).IsAssignableFrom(type))
                return series as List<ReceivedEvent<TE>>;
            else if (typeof(IDxConfiguration).IsAssignableFrom(type))
                return configurations as List<ReceivedEvent<TE>>;
            else
                throw new ArgumentException(string.Format("Unknown event type: {0}", type));
        }

        public ReceivedEvent<TE> GetLastEvent<TE>()
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                List<ReceivedEvent<TE>> list = GetList<TE>();
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
                List<ReceivedEvent<TE>> list = GetList<TE>();
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
                List<ReceivedEvent<TE>> list = GetList<TE>();
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
                List<ReceivedEvent<TE>> list = GetList<TE>();
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
                List<ReceivedEvent<TE>> list = GetList<TE>();
                List<string> symbolList = new List<string>(symbols);
                int count = 0;
                foreach (ReceivedEvent<TE> ev in list)
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
            DateTime time = DateTime.Now;
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
            List<string> symbolList = new List<string>(symbols);
            DateTime time = DateTime.Now;
            int lastIndex = 0;
            while (true)
            {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed! Not received events for next symbols: " + String.Join(", ", symbolList.ToArray()));

                List<ReceivedEvent<TE>> list = GetList<TE>();
                rwl.AcquireReaderLock(lockTimeout);
                int size = GetEventCount<TE>();
                for (int i = lastIndex; i < size; i++)
                {
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

        public void WaitOrders(params string[] sources)
        {
            List<string> sourceList = new List<string>(sources);
            DateTime time = DateTime.Now;
            int lastIndex = 0;
            while (true)
            {
                if (IsConnected != null)
                    Assert.IsTrue(IsConnected(), "Connection was lost");
                if (DateTime.Now.Subtract(time).TotalMilliseconds >= eventsTimeout)
                    Assert.Fail("Timeout elapsed! Not received events for next symbols: " + String.Join(", ", sourceList.ToArray()));

                List<ReceivedEvent<IDxOrder>> list = GetList<IDxOrder>();
                rwl.AcquireReaderLock(lockTimeout);
                int size = GetEventCount<IDxOrder>();
                for (int i = lastIndex; i < size; i++)
                {
                    IDxOrder order = list[i].Event;
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
                List<ReceivedEvent<IDxOrder>> list = GetList<IDxOrder>();
                List<string> sourceList = new List<string>(sources);
                int count = 0;
                foreach (ReceivedEvent<IDxOrder> ev in list)
                    if (sourceList.Contains(ev.Event.Source.Name))
                        count++;
                return count;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
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

        public void OnFundamental<TB, TE>(TB buf)
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

        #region Implementation of IDxTradeEthListener

        public void OnTradeEth<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeEth
        {
            foreach (var te in buf)
                AddEvent(new ReceivedEvent<IDxTradeEth>(buf.Symbol, buf.EventParams, te));
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

        #region Implementation of IDxCandleListener

        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            foreach (var c in buf)
                AddEvent(new ReceivedEvent<IDxCandle>(buf.Symbol, buf.EventParams, c));
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

        #region Implementation of IDxTheoPriceListener

        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
            foreach (var tp in buf)
                AddEvent(new ReceivedEvent<IDxTheoPrice>(buf.Symbol, buf.EventParams, tp));
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

        #region Implementation of IDxSeriesListener

        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            foreach (var s in buf)
                AddEvent(new ReceivedEvent<IDxSeries>(buf.Symbol, buf.EventParams, s));
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

        #region Implementation of IDXFeedListener

        public void EventsReceived(IList<IDxCandle> events)
        {
            foreach (var c in events)
                AddEvent(new ReceivedEvent<IDxCandle>(c.EventSymbol.ToString(), new EventParams(c.EventFlags, 0, 0), c));
        }

        public void EventsReceived(IList<IDxOrder> events)
        {
            foreach (var o in events)
                AddEvent(new ReceivedEvent<IDxOrder>(o.EventSymbol, new EventParams(o.EventFlags, 0, 0), o));
        }

        public void EventsReceived(IList<TimeSeriesEvent> events)
        {
            List<IDxEventType> timeSeriesEvents = new List<IDxEventType>(events.Count);
            foreach (var e in events)
                timeSeriesEvents.Add(e);
            EventsReceived(timeSeriesEvents);
        }

        public void EventsReceived(IList<IDxEventType> events)
        {
            foreach (var e in events)
            {
                EventParams eventParams = new EventParams(e is IndexedEvent ? (e as IndexedEvent).EventFlags : 0, 0, 0);
                string symbol = e.EventSymbol.ToString();
                if (e is IDxQuote)
                    AddEvent(new ReceivedEvent<IDxQuote>(symbol, eventParams, e as IDxQuote));
                else if (e is IDxTrade)
                    AddEvent(new ReceivedEvent<IDxTrade>(symbol, eventParams, e as IDxTrade));
                else if (e is IDxOrder)
                    AddEvent(new ReceivedEvent<IDxOrder>(symbol, eventParams, e as IDxOrder));
                else if (e is IDxProfile)
                    AddEvent(new ReceivedEvent<IDxProfile>(symbol, eventParams, e as IDxProfile));
                else if (e is IDxSummary)
                    AddEvent(new ReceivedEvent<IDxSummary>(symbol, eventParams, e as IDxSummary));
                else if (e is IDxTimeAndSale)
                    AddEvent(new ReceivedEvent<IDxTimeAndSale>(symbol, eventParams, e as IDxTimeAndSale));
                else if (e is IDxCandle)
                    AddEvent(new ReceivedEvent<IDxCandle>(symbol, eventParams, e as IDxCandle));
                else if (e is IDxTradeEth)
                    AddEvent(new ReceivedEvent<IDxTradeEth>(symbol, eventParams, e as IDxTradeEth));
                else if (e is IDxSpreadOrder)
                    AddEvent(new ReceivedEvent<IDxSpreadOrder>(symbol, eventParams, e as IDxSpreadOrder));
                else if (e is IDxGreeks)
                    AddEvent(new ReceivedEvent<IDxGreeks>(symbol, eventParams, e as IDxGreeks));
                else if (e is IDxTheoPrice)
                    AddEvent(new ReceivedEvent<IDxTheoPrice>(symbol, eventParams, e as IDxTheoPrice));
                else if (e is IDxUnderlying)
                    AddEvent(new ReceivedEvent<IDxUnderlying>(symbol, eventParams, e as IDxUnderlying));
                else if (e is IDxSeries)
                    AddEvent(new ReceivedEvent<IDxSeries>(symbol, eventParams, e as IDxSeries));
                else if (e is IDxConfiguration)
                    AddEvent(new ReceivedEvent<IDxConfiguration>(symbol, eventParams, e as IDxConfiguration));
                else
                    throw new ArgumentException(string.Format("Unknown event type: {0}", e.GetType()));
            }
        }

        #endregion
    }
}
