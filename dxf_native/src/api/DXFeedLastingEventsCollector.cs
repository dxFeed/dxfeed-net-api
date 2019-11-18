﻿#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.candle;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.events.market;
using com.dxfeed.api.util;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace com.dxfeed.api
{
    /// <summary>
    ///     Class provides methods for collecting and storing lasting events.
    /// </summary>
    internal class DXFeedLastingEventsCollector :
        IDxCandleListener,
        IDxGreeksListener,
        IDxOrderListener,
        IDxProfileListener,
        IDxQuoteListener,
        IDxSeriesListener,
        IDxSpreadOrderListener,
        IDxFundamentalListener,
        IDxTheoPriceListener,
        IDxTimeAndSaleListener,
        IDxTradeListener,
        IDxTradeETHListener,
        IDxUnderlyingListener,
        IDxConfigurationListener
    {

        private class EventStorage<E> where E : class, IDxEventType
        {
            private E eventData;
            private object eventLock = new object();

            public E Event
            {
                get
                {
                    E result = default(E);
                    lock(eventLock)
                    {
                        result = (E)eventData.Clone();
                    }
                    return result;
                }
                set
                {
                    lock (eventLock)
                    {
                        eventData = (E)value.Clone();
                    }
                }
            }
        }

        private class EventsCollection
        {
            private ConcurrentDictionary<EventType, EventStorage<IDxEventType>> lastEvents = new ConcurrentDictionary<EventType, EventStorage<IDxEventType>>();

            public bool HasEvent<E>() where E : class, IDxEventType
            {
                EventType eventType = EventTypeUtil.GetEventsType(typeof(E));
                return lastEvents.ContainsKey(eventType);
            }

            public E GetEvent<E>() where E : class, IDxEventType
            {
                if (!HasEvent<E>())
                    return null;
                EventType eventType = EventTypeUtil.GetEventsType(typeof(E));
                return (E)lastEvents[eventType].Event;
            }

            public void AddEvent<E>(E eventData) where E : class, IDxEventType
            {
                EventType eventType = EventTypeUtil.GetEventsType(typeof(E));
                lastEvents.GetOrAdd(eventType, new EventStorage<IDxEventType>()).Event = eventData;
            }
        }

        private ConcurrentDictionary<string, EventsCollection> lastSymbols = new ConcurrentDictionary<string, EventsCollection>();

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public DXFeedLastingEventsCollector() { }

        /// <summary>
        ///     Checks collector contains lasting event for specified <paramref name="symbol"/>
        ///     and event of type <c>E</c>.
        /// </summary>
        /// <typeparam name="E">Event type.</typeparam>
        /// <param name="symbol">The market symbol.</param>
        /// <returns>
        ///     <c>True</c> if collector contains lasting event for specified
        ///     <paramref name="symbol"/> and event of type <c>E</c>; otherwise returns
        ///     <c>false</c>.
        /// </returns>
        public bool HasEvent<E>(object symbol) where E : class, IDxEventType
        {
            string key = GetSymbolKey(symbol);
            return lastSymbols.ContainsKey(key) && lastSymbols[key].HasEvent<E>();
        }

        /// <summary>
        ///     Returns lasting event for specified <paramref name="symbol"/> and event of type
        ///     <c>E</c>. If collector haven't such event <c>null</c> will be returned.
        /// </summary>
        /// <typeparam name="E">Event type.</typeparam>
        /// <param name="symbol">The market symbol.</param>
        /// <returns>
        ///     Lasting event for specified <paramref name="symbol"/> and event of type
        ///     <c>E</c> if such contains in collector; otherwise returns <c>null.</c>
        /// </returns>
        public E GetEvent<E>(object symbol) where E : class, IDxEventType
        {
            if (!HasEvent<E>(symbol))
                return null;
            string key = GetSymbolKey(symbol);
            return lastSymbols[key].GetEvent<E>();
        }

        private string GetSymbolKey(object symbolObj)
        {
            MarketEventSymbols.ValidateSymbol(symbolObj);
            return (symbolObj is CandleSymbol) ? (symbolObj as CandleSymbol).ToString() : symbolObj as string;
        }

        protected void AddEvent<E>(object symbol, E eventData) where E : class, IDxEventType
        {
            string key = GetSymbolKey(symbol);
            lastSymbols.GetOrAdd(key, new EventsCollection()).AddEvent(eventData);
        }

        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            foreach (var e in buf)
                AddEvent<IDxCandle>(buf.Symbol.ToString(), e);
        }

        public void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            foreach (var e in buf)
                AddEvent<IDxGreeks>(buf.Symbol.ToString(), e);
        }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            foreach (var e in buf)
                AddEvent<IDxOrder>(buf.Symbol.ToString(), e);
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {
            foreach (var e in buf)
                AddEvent<IDxProfile>(buf.Symbol.ToString(), e);
        }

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
            foreach (var e in buf)
                AddEvent<IDxQuote>(buf.Symbol.ToString(), e);
        }

        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            foreach (var e in buf)
                AddEvent<IDxSeries>(buf.Symbol.ToString(), e);
        }

        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            foreach (var e in buf)
                AddEvent<IDxSpreadOrder>(buf.Symbol.ToString(), e);
        }

        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {
            foreach (var e in buf)
                AddEvent<IDxSummary>(buf.Symbol.ToString(), e);
        }

        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
            foreach (var e in buf)
                AddEvent<IDxTheoPrice>(buf.Symbol.ToString(), e);
        }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            foreach (var e in buf)
                AddEvent<IDxTimeAndSale>(buf.Symbol.ToString(), e);
        }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {
            foreach (var e in buf)
                AddEvent<IDxTrade>(buf.Symbol.ToString(), e);
        }

        public void OnTradeETH<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeETH
        {
            foreach (var e in buf)
                AddEvent<IDxTradeETH>(buf.Symbol.ToString(), e);
        }

        public void OnUnderlying<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxUnderlying
        {
            foreach (var e in buf)
                AddEvent<IDxUnderlying>(buf.Symbol.ToString(), e);
        }

        public void OnConfiguration<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxConfiguration
        {
            foreach (var e in buf)
                AddEvent<IDxConfiguration>(buf.Symbol.ToString(), e);
        }

    }

    /// <summary>
    ///     Class provides methods for collecting and storing lasting events. This collector
    ///     expands with universal event listener.
    /// </summary>
    /// <typeparam name="E">The type of event.</typeparam>
    class LastingEventsCollector<E> : DXFeedLastingEventsCollector, IDXFeedEventListener<E>
        where E : class, IDxEventType
    {
        public void EventsReceived(IList<E> events)
        {
            foreach (var e in events)
                AddEvent(e.EventSymbol, e);
        }
    }

}
