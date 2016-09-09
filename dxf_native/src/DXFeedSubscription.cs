using com.dxfeed.api;
using com.dxfeed.api.events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.dxfeed.native {
    public class DXFeedSubscription<E> : IDisposable {

        private bool isClosedNotSync = false;
        private List<DXFeedEventListener<E>> eventListeners = new List<DXFeedEventListener<E>>();
        private object eventListenerLocker = new object();
        private object isClosedLocker = new object();
        private IDxSubscription subscriptionInstance;

        private class DXFeedEventHandler : IDxFeedListener, IDxCandleListener {

            private IList<DXFeedEventListener<E>> eventListeners = null;
            private object eventListenerLocker = null;
            private Type subscriptionType;

            public DXFeedEventHandler(IList<DXFeedEventListener<E>> eventListeners, object eventListenerLocker) {
                this.eventListeners = eventListeners;
                this.eventListenerLocker= eventListenerLocker;
                subscriptionType = typeof(E);
            }

            private void CallListeners(IList<E> events) {
                lock (eventListenerLocker) {
                    foreach (DXFeedEventListener<E> listener in eventListeners)
                        listener.EventsReceived(events);
                }
            }

            #region Implementation of IDxFeedListener

            public void OnQuote<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxQuote {
                Type eventType = typeof(IDxQuote);
                if (eventType != subscriptionType && !eventType.IsSubclassOf(subscriptionType))
                    return;
                List<IDxQuote> events = new List<IDxQuote>();
                foreach (var item in buf)
                    events.Add(item);
                CallListeners((IList<E>)events);
            }

            public void OnTrade<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxTrade {
                Type eventType = typeof(IDxTrade);
                if (eventType != subscriptionType && !eventType.IsSubclassOf(subscriptionType))
                    return;
                List<IDxTrade> events = new List<IDxTrade>();
                foreach (var item in buf)
                    events.Add(item);
                CallListeners((IList<E>)events);
            }

            public void OnOrder<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxOrder {
                Type eventType = typeof(IDxOrder);
                if (eventType != subscriptionType && !eventType.IsSubclassOf(subscriptionType))
                    return;
                List<IDxOrder> events = new List<IDxOrder>();
                foreach (var item in buf)
                    events.Add(item);
                CallListeners((IList<E>)events);
            }

            public void OnProfile<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxProfile {
                Type eventType = typeof(IDxProfile);
                if (eventType != subscriptionType && !eventType.IsSubclassOf(subscriptionType))
                    return;
                List<IDxProfile> events = new List<IDxProfile>();
                foreach (var item in buf)
                    events.Add(item);
                CallListeners((IList<E>)events);
            }

            public void OnFundamental<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxSummary {
                Type eventType = typeof(IDxSummary);
                if (eventType != subscriptionType && !eventType.IsSubclassOf(subscriptionType))
                    return;
                List<IDxSummary> events = new List<IDxSummary>();
                foreach (var item in buf)
                    events.Add(item);
                CallListeners((IList<E>)events);
            }

            public void OnTimeAndSale<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxTimeAndSale {
                Type eventType = typeof(IDxTimeAndSale);
                if (eventType != subscriptionType && !eventType.IsSubclassOf(subscriptionType))
                    return;
                List<IDxTimeAndSale> events = new List<IDxTimeAndSale>();
                foreach (var item in buf)
                    events.Add(item);
                CallListeners((IList<E>)events);
            }

            #endregion

            #region Implementation of IDxCandleListener

            public void OnCandle<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxCandle {
                Type eventType = typeof(IDxCandle);
                if (eventType != subscriptionType && !eventType.IsSubclassOf(subscriptionType))
                    return;
                List<IDxCandle> events = new List<IDxCandle>();
                foreach (var item in buf)
                    events.Add(item);
                CallListeners((IList<E>)events);
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
        public DXFeedSubscription(IDxConnection connection) {
            if (connection == null)
                throw new ArgumentNullException("connection");
            subscriptionInstance = connection.CreateSubscription(GetEventsType(typeof(E)), new DXFeedEventHandler(eventListeners, eventListenerLocker));
        }

        /// <summary>
        /// Creates detached subscription for the given list of event types.
        /// </summary>
        /// <param name="connection">The native connection to server.</param>
        /// <param name="eventTypes">The list of event types.</param>
        /// <exception cref="ArgumentNullException">If connection or eventTypes is null.</exception>
        /// <exception cref="ArgumentException">If eventTypes are empty or any type of eventTypes is not event class.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public DXFeedSubscription(IDxConnection connection, params Type[] eventTypes) {
            if (connection == null)
                throw new ArgumentNullException("connection");
            subscriptionInstance = connection.CreateSubscription(GetEventsType(eventTypes), new DXFeedEventHandler(eventListeners, eventListenerLocker));
        }

        public void Dispose() {
            Close();
        }

        public bool IsClosed {
            get {
                bool value;
                lock(isClosedLocker) {
                    value = isClosedNotSync;
                }
                return value;
                
            }
        }

        /// <summary>
        /// Closes this subscription and makes it <i>permanently detached</i>. 
        /// </summary>
        public void Close() {
            if (IsClosed)
                return;
            lock (isClosedLocker) {
                isClosedNotSync = true;
                eventListeners.Clear();
                subscriptionInstance.Dispose();
            }
        }

        /// <summary>
        /// Returns a set of subscribed symbols.
        /// </summary>
        /// <returns></returns>
        public HashSet<string> GetSymbols() {
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
        public void SetSymbols(ICollection<string> symbols) {
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
        public void SetSymbols(params string[] symbols) {
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
        public void AddSymbols(ICollection<string> symbols) {
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
        public void AddSymbols(params string[] symbols) {
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
        public void AddSymbols(string symbol) {
            subscriptionInstance.AddSymbol(symbol);
        }

        /// <summary>
        /// Adds listener for events.
        /// Newly added listeners start receiving only new events.
        /// </summary>
        /// <param name="listener">The event listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void AddEventListener(DXFeedEventListener<E> listener) {
            if (listener == null)
                throw new ArgumentNullException();
            if (IsClosed)
                return;
            lock (eventListenerLocker) {
                if (!eventListeners.Contains(listener))
                    eventListeners.Add(listener);
            }
        }

        /// <summary>
        /// Removes listener for events.
        /// </summary>
        /// <param name="listener">Listener the event listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        public void RemoveEventListener(DXFeedEventListener<E> listener) {
            if (listener == null)
                throw new ArgumentNullException();
            lock (eventListenerLocker) {
                eventListeners.Remove(listener);
            }
        }

        /// <summary>
        /// Gets enum values of events type by generic class.
        /// </summary>
        /// <param name="types">The list of generic class represents events.</param>
        /// <returns>Enum values of events type by generic class.</returns>
        /// <exception cref="ArgumentException">If types element is not event class.</exception>
        private EventType GetEventsType(params Type[] types) {
            if (types == null)
                throw new ArgumentNullException("types");
            EventType events = EventType.None;
            foreach (Type t in types) {
                if (t == typeof(IDxTrade))
                    events |= EventType.Trade;
                else if (t == typeof(IDxQuote))
                    events |= EventType.Quote;
                else if (t == typeof(IDxSummary))
                    events |= EventType.Summary;
                else if (t == typeof(IDxProfile))
                    events |= EventType.Profile;
                else if (t == typeof(IDxOrder))
                    events |= EventType.Order;
                else if (t == typeof(IDxTimeAndSale))
                    events |= EventType.TimeAndSale;
                else if (t == typeof(IDxCandle))
                    events |= EventType.Candle;
                else throw new ArgumentException("Unknown event type: " + t);
            }
            return events;
        }

    }
}
