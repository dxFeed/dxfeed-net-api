/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.api.events;
using com.dxfeed.api.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.dxfeed.api
{
    public class DXFeedSubscription<E> : IDisposable
    {
        private bool isClosedNotSync = false;
        private List<DXFeedEventListener<E>> eventListeners = new List<DXFeedEventListener<E>>();
        private object eventListenerLocker = new object();
        private object isClosedLocker = new object();
        private IDxSubscription subscriptionInstance;

        private class DXFeedEventHandler : IDxFeedListener, IDxCandleListener
        {
            private IList<DXFeedEventListener<E>> eventListeners = null;
            private object eventListenerLocker = null;
            private Type subscriptionType;

            public DXFeedEventHandler(IList<DXFeedEventListener<E>> eventListeners, object eventListenerLocker)
            {
                this.eventListeners = eventListeners;
                this.eventListenerLocker = eventListenerLocker;
                subscriptionType = typeof(E);
            }

            private void CallListeners(IList<E> events)
            {
                lock (eventListenerLocker)
                {
                    foreach (DXFeedEventListener<E> listener in eventListeners)
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
        }

        /// <summary>
        /// Creates detached subscription for a single event type.
        /// </summary>
        /// <param name="connection">The native connection to server.</param>
        /// <exception cref="ArgumentNullException">If connection is null.</exception>
        /// <exception cref="ArgumentException">If type E is not event class.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public DXFeedSubscription(IDxConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            subscriptionInstance = connection.CreateSubscription(EventTypeUtil.GetEventsType(typeof(E)), new DXFeedEventHandler(eventListeners, eventListenerLocker));
        }

        /// <summary>
        /// Creates detached subscription for the given list of event types.
        /// </summary>
        /// <param name="connection">The native connection to server.</param>
        /// <param name="eventTypes">The list of event types.</param>
        /// <exception cref="ArgumentNullException">If connection or eventTypes is null.</exception>
        /// <exception cref="ArgumentException">If eventTypes are empty or any type of eventTypes is not event class.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public DXFeedSubscription(IDxConnection connection, params Type[] eventTypes)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            subscriptionInstance = connection.CreateSubscription(EventTypeUtil.GetEventsType(eventTypes), new DXFeedEventHandler(eventListeners, eventListenerLocker));
        }

        public void Dispose()
        {
            Close();
        }

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
        /// Closes this subscription and makes it <i>permanently detached</i>. 
        /// </summary>
        public void Close()
        {
            if (IsClosed)
                return;
            lock (isClosedLocker)
            {
                isClosedNotSync = true;
                eventListeners.Clear();
                subscriptionInstance.Dispose();
            }
        }

        /// <summary>
        /// Returns a set of subscribed symbols.
        /// </summary>
        /// <returns></returns>
        public HashSet<string> GetSymbols()
        {
            return new HashSet<string>(subscriptionInstance.GetSymbols());
        }

        /// <summary>
        /// Changes the set of subscribed symbols so that it contains just the symbols from the specified collection.
        /// To conveniently set subscription for just one or few symbols you can use
        /// SetSymbols(params string[] symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">The collection of symbols.</param>
        public void SetSymbols(ICollection<string> symbols)
        {
            subscriptionInstance.SetSymbols(symbols.ToArray());
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
        public void SetSymbols(params string[] symbols)
        {
            subscriptionInstance.SetSymbols(symbols);
        }

        /// <summary>
        /// Adds the specified collection of symbols to the set of subscribed symbols.
        /// To conveniently add one or few symbols you can use
        /// AddSymbols(params string[] symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">Symbols the collection of symbols.</param>
        public void AddSymbols(ICollection<string> symbols)
        {
            if (symbols.Count == 0)
                return;
            subscriptionInstance.AddSymbols(symbols.ToArray());
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
        public void AddSymbols(params string[] symbols)
        {
            if (symbols.Length == 0)
                return; // no symbols -- nothing to do
            subscriptionInstance.AddSymbols(symbols);
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
        public void AddSymbols(string symbol)
        {
            subscriptionInstance.AddSymbol(symbol);
        }

        /// <summary>
        /// Adds listener for events.
        /// Newly added listeners start receiving only new events.
        /// </summary>
        /// <param name="listener">The event listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void AddEventListener(DXFeedEventListener<E> listener)
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
        public void RemoveEventListener(DXFeedEventListener<E> listener)
        {
            if (listener == null)
                throw new ArgumentNullException();
            lock (eventListenerLocker)
            {
                eventListeners.Remove(listener);
            }
        }
    }
}
