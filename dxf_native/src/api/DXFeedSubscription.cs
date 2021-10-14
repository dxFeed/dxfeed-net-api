#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

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
    /// <summary>
    ///     Subscription for a set of symbols and event types.
    /// </summary>
    /// <typeparam name="TE">The type of events.</typeparam>
    // ReSharper disable once InconsistentNaming
    public class DXFeedSubscription<TE> : IDXFeedSubscription<TE>
        where TE : IDxEventType
    {
        /// <summary>
        ///     Creates detached subscription for a single event type.
        /// </summary>
        /// <param name="endpoint">The <see cref="DXEndpoint"/> instance.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="endpoint"/> is null.</exception>
        /// <exception cref="ArgumentException">If type E is not event class.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public DXFeedSubscription(DXEndpoint endpoint) : this()
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            endpoint.OnClosing += Endpoint_OnClosing;

            SubscriptionInstance = endpoint.Connection.CreateSubscription(
                EventTypeUtil.GetEventsType(typeof(TE)),
                new DXFeedEventHandler<TE>(EventListeners, EventListenerLocker));
        }

        /// <summary>
        ///     Creates detached subscription for the given list of event types.
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
        public DXFeedSubscription(DXEndpoint endpoint, params Type[] eventTypes) : this(eventTypes)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            foreach (Type t in eventTypes)
                if (!typeof(TE).IsAssignableFrom(t))
                    throw new ArgumentException($"The type {t} is not {typeof(TE)}");

            endpoint.OnClosing += Endpoint_OnClosing;

            SubscriptionInstance = endpoint.Connection.CreateSubscription(
                EventTypeUtil.GetEventsType(eventTypes),
                new DXFeedEventHandler<TE>(EventListeners, EventListenerLocker));
        }

        /// <summary>
        ///     Creates detached snapshot subscription for a single event type.
        /// </summary>
        /// <param name="endpoint">The <see cref="DXEndpoint"/> instance.</param>
        /// <param name="time">Unix time in the past - number of milliseconds from 1.1.1970.</param>
        /// <param name="source">The source of the event.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="endpoint"/> is null.</exception>
        /// <exception cref="ArgumentException">If type E is not event class.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        internal DXFeedSubscription(DXEndpoint endpoint, long time, IndexedEventSource source) : this()
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            endpoint.OnClosing += Endpoint_OnClosing;

            SubscriptionInstance = endpoint.Connection.CreateSnapshotSubscription(
                EventTypeUtil.GetEventsType(typeof(TE)),
                time,
                new DXFeedEventHandler<TE>(EventListeners, EventListenerLocker));
            if (!Equals(source, IndexedEventSource.DEFAULT))
                SubscriptionInstance.SetSource(source.Name);
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
                EventListeners.Clear();
                SubscriptionInstance.Dispose();
            }

            OnSubscriptionClosed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Returns a set of subscribed event types. The resulting set cannot be modified.
        /// </summary>
        public ISet<Type> EventTypes
        {
            get { return new HashSet<Type>(eventTypesSet); }
        }

        /// <summary>
        ///     Returns <c>true</c> if this subscription contains the corresponding event type.
        /// </summary>
        /// <param name="eventType">Type of event.</param>
        /// <returns><c>True</c> if this subscription contains the corresponding event type.</returns>
        public bool ContainsEventType(Type eventType)
        {
            return EventTypes.Contains(eventType);
        }

        /// <summary>
        ///     <para>
        ///         Clears the set of subscribed symbols.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsRemoved"/> events on
        ///         clear symbols from this subscription.
        ///     </para>
        /// </summary>
        public void Clear()
        {
            if (IsClosed)
                return;
            lock (symbolsLocker)
            {
                OnSymbolsRemoved?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(GetSymbols()));
                SubscriptionInstance.Clear();
            }
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
            ISet<object> symbolsSet;
            lock (symbolsLocker)
            {
                symbolsSet = GetSymbolsUnsafe();
            }

            return symbolsSet;
        }

        /// <summary>
        ///     <para>
        ///         Changes the set of subscribed symbols so that it contains just the symbols from
        ///         the specified collection.
        ///         To conveniently set subscription for just one or few symbols you can use
        ///         <see cref="SetSymbols(object[])"/> method.
        ///         All registered event listeners will receive update on the last events for all
        ///         newly added symbols.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsAdded"/> and
        ///         <see cref="OnSymbolsRemoved"/> events on symbols changing for this subscription.
        ///     </para>
        /// </summary>
        /// <param name="symbols">The collection of symbols.</param>
        public void SetSymbols(ICollection<object> symbols)
        {
            if (IsClosed)
                return;
            if (symbols == null || symbols.Count == 0)
                return;
            lock (symbolsLocker)
            {
                OnSymbolsRemoved?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(GetSymbolsUnsafe()));
                SubscriptionInstance.SetSymbols(SymbolsToStringList(symbols).ToArray());
                OnSymbolsAdded?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(symbols));
            }
        }

        /// <summary>
        ///     <para>
        ///         Changes the set of subscribed symbols so that it contains just the symbols from
        ///         the specified array.
        ///         This is a convenience method to set subscription to one or few symbols at a time.
        ///         When setting subscription to multiple symbols at once it is preferable to use
        ///         <see cref="SetSymbols(ICollection{object})"/> method.
        ///         All registered event listeners will receive update on the last events for all
        ///         newly added symbols.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsAdded"/> and
        ///         <see cref="OnSymbolsRemoved"/> events on symbols changing for this subscription.
        ///     </para>
        /// </summary>
        /// <param name="symbols">The array of symbols.</param>
        public void SetSymbols(params object[] symbols)
        {
            if (IsClosed)
                return;
            if (symbols == null || symbols.Length == 0)
                return;
            lock (symbolsLocker)
            {
                OnSymbolsRemoved?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(GetSymbolsUnsafe()));
                SubscriptionInstance.SetSymbols(SymbolsToStringList(symbols).ToArray());
                OnSymbolsAdded?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(symbols));
            }
        }

        /// <summary>
        ///     <para>
        ///         Adds the specified collection of symbols to the set of subscribed symbols.
        ///         To conveniently add one or few symbols you can use
        ///         <see cref="AddSymbols(object[])"/> method.
        ///         All registered event listeners will receive update on the last events for all
        ///         newly added symbols.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsAdded"/> events on
        ///         symbols changing for this subscription.
        ///     </para>
        /// </summary>
        /// <param name="symbols">Symbols the collection of symbols.</param>
        public void AddSymbols(ICollection<object> symbols)
        {
            if (symbols == null)
                return;
            AddSymbols(symbols.ToArray());
        }

        /// <summary>
        ///     <para>
        ///         Adds the specified array of symbols to the set of subscribed symbols.
        ///         This is a convenience method to subscribe to one or few symbols at a time.
        ///         When subscribing to multiple symbols at once it is preferable to use
        ///         <see cref="AddSymbols(ICollection{object})"/> method.
        ///         All registered event listeners will receive update on the last events for all
        ///         newly added symbols.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsAdded"/> events on
        ///         symbols changing for this subscription.
        ///     </para>
        /// </summary>
        /// <param name="symbols">The array of symbols.</param>
        public void AddSymbols(params object[] symbols)
        {
            AddSymbols(true, symbols);
        }

        /// <summary>
        ///     <para>
        ///         Adds the specified symbol to the set of subscribed symbols.
        ///         This is a convenience method to subscribe to one symbol at a time that
        ///         has a return fast-path for a case when the symbol is already in the set.
        ///         When subscribing to multiple symbols at once it is preferable to use
        ///         <see cref="AddSymbols(ICollection{object})"/> method.
        ///         All registered event listeners will receive update on the last events for all
        ///         newly added symbols.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsAdded"/> events on
        ///         symbols changing for this subscription.
        ///     </para>
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void AddSymbols(object symbol)
        {
            if (IsClosed)
                return;
            if (symbol == null)
                return;
            lock (symbolsLocker)
            {
                SubscriptionInstance.AddSymbol(SymbolToString(symbol));
                OnSymbolsAdded?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(symbol));
            }
        }

        /// <summary>
        ///     <para>
        ///         Removes the specified collection of symbols from the set of subscribed symbols.
        ///         To conveniently remove one or few symbols you can use
        ///         <see cref="RemoveSymbols(object[])"/> method.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsRemoved"/> events on
        ///         symbols changing for this subscription.
        ///     </para>
        /// </summary>
        /// <param name="symbols">The collection of symbols.</param>
        public void RemoveSymbols(ICollection<object> symbols)
        {
            if (IsClosed)
                return;
            if (symbols == null || symbols.Count == 0)
                return;
            lock (symbolsLocker)
            {
                SubscriptionInstance.RemoveSymbols(SymbolsToStringList(symbols).ToArray());
                OnSymbolsRemoved?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(symbols));
            }
        }

        /// <summary>
        ///     <para>
        ///         Removes the specified array of symbols from the set of subscribed symbols.
        ///         This is a convenience method to remove one or few symbols at a time.
        ///         When removing multiple symbols at once it is preferable to use
        ///         <see cref="RemoveSymbols(ICollection{object})"/> method.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsRemoved"/> events on
        ///         symbols changing for this subscription.
        ///     </para>
        /// </summary>
        /// <param name="symbols">The array of symbols.</param>
        public void RemoveSymbols(params object[] symbols)
        {
            if (IsClosed)
                return;
            if (symbols == null || symbols.Length == 0)
                return;
            lock (symbolsLocker)
            {
                SubscriptionInstance.RemoveSymbols(SymbolsToStringList(symbols).ToArray());
                OnSymbolsRemoved?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(symbols));
            }
        }

        /// <summary>
        ///     Adds listener for events.
        ///     Newly added listeners start receiving only new events.
        ///     This method does nothing if this subscription is <see cref="IsClosed"/>.
        /// </summary>
        /// <param name="listener">The event listener.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="listener"/> is null.</exception>
        public void AddEventListener(IDXFeedEventListener<TE> listener)
        {
            if (listener == null)
                throw new ArgumentNullException();
            if (IsClosed)
                return;
            lock (EventListenerLocker)
            {
                if (!EventListeners.Contains(listener))
                    EventListeners.Add(listener);
            }
        }

        /// <summary>
        ///     Removes listener for events.
        /// </summary>
        /// <param name="listener">Listener the event listener.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="listener"/> is null.</exception>
        public void RemoveEventListener(IDXFeedEventListener<TE> listener)
        {
            if (listener == null)
                throw new ArgumentNullException();
            lock (EventListenerLocker)
            {
                EventListeners.Remove(listener);
            }
        }

        /// <summary>
        ///     Event calls when any symbols is added to subscription.
        /// </summary>
        public event DXFeedSymbolsUpdateEventHandler OnSymbolsAdded;

        /// <summary>
        /// Event calls when any symbols is removed from subscription.
        /// </summary>
        public event DXFeedSymbolsUpdateEventHandler OnSymbolsRemoved;

        /// <summary>
        /// Event calls when subscription is closing.
        /// </summary>
        public event DXFeedSubscriptionClosedEventHandler OnSubscriptionClosed;

        #region protected fields and methods

        /// <summary>
        /// The current native subscription
        /// </summary>
        protected IDxSubscription SubscriptionInstance;

        /// <summary>
        /// The collection of event listeners
        /// </summary>
        protected readonly List<IDXFeedEventListener<TE>> EventListeners = new List<IDXFeedEventListener<TE>>();

        /// <summary>
        /// The lock object for the event listeners
        /// </summary>
        protected readonly object EventListenerLocker = new object();

        /// <summary>
        /// Default constructor
        /// </summary>
        protected DXFeedSubscription()
        {
            eventTypesSet.Add(typeof(TE));
        }

        /// <summary>
        /// Create subscription by event types
        /// </summary>
        /// <param name="eventTypes">The event types</param>
        protected DXFeedSubscription(params Type[] eventTypes)
        {
            foreach (var eventType in eventTypes)
            {
                eventTypesSet.Add(eventType);
            }
        }

        /// <summary>
        ///     <para>
        ///         Adds the specified array of symbols to the set of subscribed symbols.
        ///         This is a convenience method to subscribe to one or few symbols at a time.
        ///         When subscribing to multiple symbols at once it is preferable to use
        ///         <see cref="AddSymbols(ICollection{object})"/> method.
        ///         All registered event listeners will receive update on the last events for all
        ///         newly added symbols.
        ///     </para>
        ///     <para>
        ///         Implementation notes.
        ///     </para>
        ///     <para>
        ///         This method notifies all subscribed <see cref="OnSymbolsAdded"/> events on
        ///         symbols changing for this subscription. The <c>false</c> value of
        ///         <paramref name="callUpdateEvent"/> disables calling any symbols update events
        ///         for this method.
        ///     </para>
        /// </summary>
        /// <param name="callUpdateEvent">
        ///     The <c>false</c> value disables calling any symbols update events for this method.
        /// </param>
        /// <param name="symbols">The array of symbols.</param>
        protected void AddSymbols(bool callUpdateEvent, params object[] symbols)
        {
            if (IsClosed)
                return;
            if (symbols == null || symbols.Length == 0)
                return;
            lock (symbolsLocker)
            {
                SubscriptionInstance.AddSymbols(SymbolsToStringList(symbols).ToArray());
                if (callUpdateEvent)
                    OnSymbolsAdded?.Invoke(this, new DXFeedSymbolsUpdateEventArgs(symbols));
            }
        }

        /// <summary>
        /// The OnClosing event handler
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The event arguments</param>
        protected void Endpoint_OnClosing(object sender, EventArgs e)
        {
            ((IDXEndpoint)sender).OnClosing -= Endpoint_OnClosing;
            Close();
        }

        #endregion

        #region private fields and methods

        private bool isClosedNotSync;
        private readonly object isClosedLocker = new object();
        private readonly object symbolsLocker = new object();
        private IDXFeed attachedFeed;
        private readonly HashSet<Type> eventTypesSet = new HashSet<Type>();

        private ICollection<string> SymbolsToStringList(ICollection<object> symbols)
        {
            List<string> stringList = new List<string>();
            foreach (var obj in symbols)
                stringList.Add(SymbolToString(obj));
            return stringList;
        }

        private static string SymbolToString(object obj)
        {
            MarketEventSymbols.ValidateSymbol(obj);
            var symbol = obj as CandleSymbol;
            return symbol != null ? symbol.ToString() : obj as string;
        }

        private ISet<object> GetSymbolsUnsafe()
        {
            var symbolsSet = new HashSet<object>();
            
            if (IsClosed) return symbolsSet;
            
            foreach (var symbol in SubscriptionInstance.GetSymbols())
            {
                symbolsSet.Add(symbol);
            }

            return symbolsSet;
        }

        #endregion
    }
}