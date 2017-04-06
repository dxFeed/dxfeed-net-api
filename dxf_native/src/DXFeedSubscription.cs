#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.events.market;
using com.dxfeed.api.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.dxfeed.api
{
    //TODO: comments
    public class DXFeedSubscription<E> : IDXFeedSubscription<E>
    {
        private bool isClosedNotSync = false;
        private List<IDXFeedEventListener<E>> eventListeners = new List<IDXFeedEventListener<E>>();
        private object eventListenerLocker = new object();
        private object isClosedLocker = new object();
        private object symbolsLocker = new object();
        private IDxSubscription subscriptionInstance;
        private IDXFeed attachedFeed = null;

        private class DXFeedEventHandler :
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
            IDxTradeEthListener,
            IDxUnderlyingListener,
            IDxConfigurationListener,
            IDxOrderSnapshotListener,
            IDxCandleSnapshotListener,
            IDxTimeAndSaleSnapshotListener,
            IDxSpreadOrderSnapshotListener,
            IDxGreeksSnapshotListener,
            IDxSeriesSnapshotListener
        {
            private IList<IDXFeedEventListener<E>> eventListeners = null;
            private object eventListenerLocker = null;
            private Type subscriptionType;

            public DXFeedEventHandler(IList<IDXFeedEventListener<E>> eventListeners, object eventListenerLocker)
            {
                this.eventListeners = eventListeners;
                this.eventListenerLocker = eventListenerLocker;
                subscriptionType = typeof(E);
            }

            private void CallListeners(IList<E> events)
            {
                lock (eventListenerLocker)
                {
                    foreach (IDXFeedEventListener<E> listener in eventListeners)
                        listener.EventsReceived(events);
                }
            }

            #region Implementation of IDxFeedListener

            public void OnQuote<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxQuote
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxQuote)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            public void OnTrade<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxTrade
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxTrade)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            public void OnOrder<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxOrder
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxOrder)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            public void OnProfile<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxProfile
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxProfile)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            public void OnFundamental<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxSummary
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxSummary)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            public void OnTimeAndSale<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxTimeAndSale
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxTimeAndSale)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxCandleListener

            public void OnCandle<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxCandle
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxCandle)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxGreeks

            public void OnGreeks<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxGreeks
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxGreeks)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxSeries

            public void OnSeries<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxSeries
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxSeries)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxSpreadOrder

            public void OnSpreadOrder<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxSpreadOrder
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxSpreadOrder)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxTheoPrice

            public void OnTheoPrice<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxTheoPrice
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxTheoPrice)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxTradeEth

            public void OnTradeEth<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxTradeEth
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxTradeEth)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxUnderlying

            public void OnUnderlying<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxUnderlying
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxUnderlying)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxConfiguration

            public void OnConfiguration<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxConfiguration
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxConfiguration)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxOrder snapshot

            public void OnOrderSnapshot<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxOrder
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxOrder)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxCandle snapshot

            public void OnCandleSnapshot<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxCandle
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxCandle)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxTimeAndSale snapshot

            public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxTimeAndSale
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxTimeAndSale)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxSpreadOrder snapshot

            public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxSpreadOrder
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxSpreadOrder)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxGreeks snapshot

            public void OnGreeksSnapshot<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxGreeks
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxGreeks)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion

            #region Implementation of IDxSeries snapshot

            public void OnSeriesSnapshot<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxSeries
            {
                if (!subscriptionType.IsAssignableFrom(typeof(IDxSeries)))
                    return;
                List<E> events = new List<E>();
                foreach (var item in buf)
                    events.Add((E)(object)item);
                CallListeners(events);
            }

            #endregion
        }

        /// <summary>
        /// Creates detached subscription for a single event type.
        /// </summary>
        /// <param name="endpoint">The <see cref="DXEndpoint"/> instance.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="endpoint"/> is null.</exception>
        /// <exception cref="ArgumentException">If type E is not event class.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public DXFeedSubscription(DXEndpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");
            subscriptionInstance = endpoint.Connection.CreateSubscription(
                EventTypeUtil.GetEventsType(typeof(E)),
                new DXFeedEventHandler(eventListeners, eventListenerLocker));
        }

        /// <summary>
        /// Creates detached subscription for the given list of event types.
        /// </summary>
        /// <param name="endpoint">The <see cref="DXEndpoint"/> instance.</param>
        /// <param name="eventTypes">The list of event types.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="endpoint"/> or <paramref name="eventTypes"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="eventTypes"/> are empty or any type of 
        ///     <paramref name="eventTypes"/> is not event class.
        /// </exception>
        /// <exception cref="DxException">Internal error.</exception>
        public DXFeedSubscription(DXEndpoint endpoint, params Type[] eventTypes)
        {
            if (endpoint == null)
                throw new ArgumentNullException("connection");
            subscriptionInstance = endpoint.Connection.CreateSubscription(
                EventTypeUtil.GetEventsType(eventTypes),
                new DXFeedEventHandler(eventListeners, eventListenerLocker));
        }

        /// <summary>
        /// Creates detached snapshot subscription for a single event type.
        /// </summary>
        /// <param name="endpoint">The <see cref="DXEndpoint"/> instance.</param>
        /// <param name="time">Unix time in the past - number of milliseconds from 1.1.1970.</param>
        /// <param name="source">The source of the event.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="endpoint"/> is null.</exception>
        /// <exception cref="ArgumentException">If type E is not event class.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        internal DXFeedSubscription(DXEndpoint endpoint, long time, IndexedEventSource source)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");
            subscriptionInstance = endpoint.Connection.CreateSnapshotSubscription(
                EventTypeUtil.GetEventsType(typeof(E)), time,
                new DXFeedEventHandler(eventListeners, eventListenerLocker));
            if (source != IndexedEventSource.DEFAULT)
                subscriptionInstance.SetSource(source.Name);
        }

        /// <summary>
        ///     Attaches subscription to the specified feed.
        /// </summary>
        /// <param name="feed">Feed to attach to.</param>
        public void Attach(IDXFeed feed)
        {
            if (attachedFeed != null)
                return;
            feed.AttachSubscription(this);
            attachedFeed = feed;
        }

        /// <summary>
        ///     Detaches subscription from the specified feed.
        /// </summary>
        /// <param name="feed">Feed to detach from.</param>
        public void Detach(IDXFeed feed)
        {
            feed.DetachSubscription(this);
            attachedFeed = null;
        }

        /// <summary>
        ///     Returns <c>true</c> if this subscription is closed.
        ///     <seealso cref="Close()"/>
        /// </summary>
        public bool IsClosed
        {
            get
            {
                bool value;
                lock (isClosedLocker)
                {
                    value = isClosedNotSync;
                }
                return value;

            }
        }

        /// <summary>
        ///     <para>
        ///         Closes this subscription and makes it permanently detached. 
        ///         This method notifies attached <see cref="IDXFeed"/> by invoking 
        ///         <see cref="Detach(IDXFeed)"/> and <see cref="IDXFeed.DetachSubscription{E}(IDXFeedSubscription{E})"/> 
        ///         methods while holding the lock for this subscription. This method clears lists 
        ///         of all installed event listeners and subscription change listeners and makes 
        ///         sure that no more listeners can be added.
        ///     </para>
        ///     <para>
        ///         This method ensures that subscription can be safely garbage-collected when all 
        ///         outside references to it are lost.
        ///     </para>
        /// </summary>
        public void Close()
        {
            if (IsClosed)
                return;

            if (attachedFeed != null)
                Detach(attachedFeed);

            lock (isClosedLocker)
            {
                isClosedNotSync = true;
                eventListeners.Clear();
                subscriptionInstance.Dispose();
            }
        }

        /// <summary>
        ///     Clears the set of subscribed symbols.
        /// </summary>
        public void Clear()
        {
            subscriptionInstance.Clear();
        }

        /// <summary>
        ///     Returns a set of subscribed symbols. The resulting set cannot be modified. The 
        ///     contents of the resulting set are undefined if the set of symbols is changed after 
        ///     invocation of this method, but the resulting set is safe for concurrent reads from 
        ///     any threads. The resulting set maybe either a snapshot of the set of the subscribed 
        ///     symbols at the time of invocation or a weakly consistent view of the set. 
        /// </summary>
        /// <returns>Set of subscribed symbols.</returns>
        public ISet<object> GetSymbols()
        {
            HashSet<object> symbolsSet = new HashSet<object>();
            lock (symbolsLocker)
            {
                subscriptionInstance.GetSymbols().All(s => symbolsSet.Add(s));
            }
            return symbolsSet;
        }

        /// <summary>
        ///     Changes the set of subscribed symbols so that it contains just the symbols from 
        ///     the specified collection.
        ///     To conveniently set subscription for just one or few symbols you can use
        ///     <see cref="SetSymbols(object[])"/> method.
        ///     All registered event listeners will receive update on the last events for all
        ///     newly added symbols.
        /// </summary>
        /// <param name="symbols">The collection of symbols.</param>
        public void SetSymbols(ICollection<object> symbols)
        {
            lock (symbolsLocker)
            {
                subscriptionInstance.SetSymbols(SymbolsToStringList(symbols).ToArray());
            }
        }

        /// <summary>
        /// Changes the set of subscribed symbols so that it contains just the symbols from the specified array.
        /// This is a convenience method to set subscription to one or few symbols at a time.
        /// When setting subscription to multiple symbols at once it is preferable to use
        /// SetSymbols(ICollection<string> symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">The array of symbols.</param>
        public void SetSymbols(params object[] symbols)
        {
            lock (symbolsLocker)
            {
                subscriptionInstance.SetSymbols(SymbolsToStringList(symbols).ToArray());
            }
        }

        /// <summary>
        /// Adds the specified collection of symbols to the set of subscribed symbols.
        /// To conveniently add one or few symbols you can use
        /// AddSymbols(params string[] symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">Symbols the collection of symbols.</param>
        public void AddSymbols(ICollection<object> symbols)
        {
            if (symbols.Count == 0)
                return;
            lock (symbolsLocker)
            {
                subscriptionInstance.AddSymbols(SymbolsToStringList(symbols).ToArray());
            }
        }

        /// <summary>
        /// Adds the specified array of symbols to the set of subscribed symbols.
        /// This is a convenience method to subscribe to one or few symbols at a time.
        /// When subscribing to multiple symbols at once it is preferable to use
        /// AddSymbols(ICollection<string> symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">The array of symbols.</param>
        public void AddSymbols(params object[] symbols)
        {
            if (symbols.Length == 0)
                return;
            lock (symbolsLocker)
            {
                subscriptionInstance.AddSymbols(SymbolsToStringList(symbols).ToArray());
            }
        }

        /// <summary>
        /// Adds the specified symbol to the set of subscribed symbols.
        /// This is a convenience method to subscribe to one symbol at a time that
        /// has a return fast-path for a case when the symbol is already in the set.
        /// When subscribing to multiple symbols at once it is preferable to use
        /// AddSymbols(ICollection<string> symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void AddSymbols(object symbol)
        {
            lock (symbolsLocker)
            {
                subscriptionInstance.AddSymbol(SymbolToString(symbol));
            }
        }

        public void RemoveSymbols(ICollection<object> symbols)
        {
            if (symbols.Count == 0)
                return;
            lock (symbolsLocker)
            {
                subscriptionInstance.RemoveSymbols(SymbolsToStringList(symbols).ToArray());
            }
        }

        public void RemoveSymbols(params object[] symbols)
        {
            if (symbols.Length == 0)
                return;
            lock (symbolsLocker)
            {
                subscriptionInstance.RemoveSymbols(SymbolsToStringList(symbols).ToArray());
            }
        }

        /// <summary>
        /// Adds listener for events.
        /// Newly added listeners start receiving only new events.
        /// </summary>
        /// <param name="listener">The event listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void AddEventListener(IDXFeedEventListener<E> listener)
        {
            if (listener == null)
                throw new ArgumentNullException();
            if (IsClosed)
                return;
            lock (eventListenerLocker)
            {
                if (!eventListeners.Contains(listener))
                    eventListeners.Add(listener);
            }
        }

        /// <summary>
        /// Removes listener for events.
        /// </summary>
        /// <param name="listener">Listener the event listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void RemoveEventListener(IDXFeedEventListener<E> listener)
        {
            if (listener == null)
                throw new ArgumentNullException();
            lock (eventListenerLocker)
            {
                eventListeners.Remove(listener);
            }
        }

        private ICollection<string> SymbolsToStringList(ICollection<object> symbols)
        {
            List<string> stringList = new List<string>();
            foreach (var obj in symbols)
                stringList.Add(SymbolToString(obj));
            return stringList;
        }

        private string SymbolToString(object obj)
        {
            MarketEventSymbols.ValidateSymbol(obj);
            return obj is CandleSymbol ? (obj as CandleSymbol).ToString() : obj as string;
        }

    }
}
