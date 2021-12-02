#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.data;
using com.dxfeed.native.api;
using com.dxfeed.native.events;
using com.dxfeed.api.events.market;

namespace com.dxfeed.native
{
    /// <summary>
    /// Class provides native event subscription
    ///
    /// Not thread safe.
    /// </summary>
    public class NativeSubscription : IDxSubscription
    {
        private IntPtr subscriptionPtr;

        private readonly IDxEventListener eventListener;

        //to prevent callback from being garbage collected
        private readonly C.dxf_event_listener_v2_t callback;
        private readonly EventType eventType;
        private NativeConnection connection;
        private bool disposed;

        /// <summary>
        /// Creates the new event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">native connection pointer</param>
        /// <param name="eventType">type of event to create</param>
        /// <param name="eventSubscriptionFlags">event subscription flags</param>
        /// <param name="listener">event listener</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException">Connection or listener is invalid.</exception>
        public NativeSubscription(NativeConnection connection, EventType eventType,
            EventSubscriptionFlag eventSubscriptionFlags, IDxEventListener listener)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            this.connection = connection;
            this.eventType = eventType;
            eventListener = listener;

            C.CheckOk(eventSubscriptionFlags == EventSubscriptionFlag.Default
                ? C.Instance.dxf_create_subscription(connection.Handle, eventType, out subscriptionPtr)
                : C.Instance.dxf_create_subscription_with_flags(connection.Handle, eventType, eventSubscriptionFlags,
                    out subscriptionPtr));

            try
            {
                C.CheckOk(C.Instance.dxf_attach_event_listener_v2(subscriptionPtr, callback = OnEvent, IntPtr.Zero));
            }
            catch (DxException)
            {
                C.Instance.dxf_close_subscription(subscriptionPtr);
                throw;
            }
        }

        /// <summary>
        /// Creates the new event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">native connection pointer</param>
        /// <param name="eventType">type of event to create</param>
        /// <param name="listener">event listener</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException">Connection or listener is invalid.</exception>
        public NativeSubscription(NativeConnection connection, EventType eventType, IDxEventListener listener) :
            this(connection, eventType, EventSubscriptionFlag.Default, listener)
        {
        }

        /// <summary>
        /// Creates the new time event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="eventType">Type of event to create.</param>
        /// <param name="time">Unix time stamp (the number of milliseconds from 1.1.1970)</param>
        /// <param name="eventSubscriptionFlags">event subscription flags</param>
        /// <param name="listener">Event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException">Connection or listener is invalid.</exception>
        public NativeSubscription(NativeConnection connection, EventType eventType, long time,
            EventSubscriptionFlag eventSubscriptionFlags, IDxEventListener listener)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            this.connection = connection;
            this.eventType = eventType;
            eventListener = listener;

            if (eventSubscriptionFlags == EventSubscriptionFlag.Default)
            {
                C.CheckOk(C.Instance.dxf_create_subscription_timed(connection.Handle, eventType, time,
                    out subscriptionPtr));
            }
            else
            {
                C.CheckOk(C.Instance.dxf_create_subscription_timed_with_flags(connection.Handle, eventType, time,
                    eventSubscriptionFlags,
                    out subscriptionPtr));
            }

            try
            {
                C.CheckOk(C.Instance.dxf_attach_event_listener_v2(subscriptionPtr, callback = OnEvent, IntPtr.Zero));
            }
            catch (DxException)
            {
                C.Instance.dxf_close_subscription(subscriptionPtr);
                throw;
            }
        }

        /// <summary>
        /// Creates the new time event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="eventType">Type of event to create.</param>
        /// <param name="time">Unix time stamp (the number of milliseconds from 1.1.1970)</param>
        /// <param name="listener">Event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException">Connection or listener is invalid.</exception>
        public NativeSubscription(NativeConnection connection, EventType eventType, long time,
            IDxEventListener listener) :
            this(connection, eventType, time, EventSubscriptionFlag.Default, listener)
        {
        }

        /// <summary>
        /// Creates the new time event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="eventType">Type of event to create.</param>
        /// <param name="time">Time to getting events from.</param>
        /// <param name="eventSubscriptionFlags">event subscription flags</param>
        /// <param name="listener">Event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException">Connection or listener is invalid.</exception>
        public NativeSubscription(NativeConnection connection, EventType eventType, DateTime time,
            EventSubscriptionFlag eventSubscriptionFlags, IDxEventListener listener) :
            this(connection, eventType, Tools.DateToUnixTime(time), eventSubscriptionFlags, listener)
        {
        }

        /// <summary>
        /// Creates the new time event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="eventType">Type of event to create.</param>
        /// <param name="time">Time to getting events from.</param>
        /// <param name="listener">Event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException">Connection or listener is invalid.</exception>
        public NativeSubscription(NativeConnection connection, EventType eventType, DateTime time,
            IDxEventListener listener) :
            this(connection, eventType, Tools.DateToUnixTime(time), listener)
        {
        }

        /// <summary>
        /// Creates the new Candle event subscription.
        /// For rest events use another constructor.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="time">Date time in the past.</param>
        /// <param name="eventSubscriptionFlags">event subscription flags</param>
        /// <param name="listener">Candle event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException">Connection or listener is invalid.</exception>
        public NativeSubscription(NativeConnection connection, DateTime? time,
            EventSubscriptionFlag eventSubscriptionFlags, IDxCandleListener listener)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            this.connection = connection;
            eventType = EventType.Candle;
            eventListener = listener;
            
            var unixTimestamp = time == null ? 0 : Tools.DateToUnixTime((DateTime)time);

            C.CheckOk(eventSubscriptionFlags == EventSubscriptionFlag.Default
                ? C.Instance.dxf_create_subscription_timed(connection.Handle, eventType, unixTimestamp, out subscriptionPtr)
                : C.Instance.dxf_create_subscription_timed_with_flags(connection.Handle, eventType, unixTimestamp,
                    eventSubscriptionFlags, out subscriptionPtr));

            try
            {
                C.CheckOk(C.Instance.dxf_attach_event_listener_v2(subscriptionPtr, callback = OnEvent, IntPtr.Zero));
            }
            catch (DxException)
            {
                C.Instance.dxf_close_subscription(subscriptionPtr);
                throw;
            }
        }

        /// <summary>
        /// Creates the new Candle event subscription.
        /// For rest events use another constructor.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="time">Date time in the past.</param>
        /// <param name="listener">Candle event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException">Connection or listener is invalid.</exception>
        public NativeSubscription(NativeConnection connection, DateTime? time, IDxCandleListener listener) :
            this(connection, time, EventSubscriptionFlag.Default, listener)
        {
        }

        private void OnEvent(EventType onEventEventType, IntPtr symbol, IntPtr data, int dataCount,
            IntPtr eventParamsPtr, IntPtr userData)
        {
            var obj = Marshal.PtrToStructure(eventParamsPtr, typeof(DxEventParams));
            var eventParams = (DxEventParams)obj;
            var nativeEventParams =
                new EventParams(eventParams.flags, eventParams.time_int_field, eventParams.snapshot_key);

            switch (onEventEventType)
            {
                case EventType.Order:
                    var orderBuf = NativeBufferFactory.CreateOrderBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxOrderListener)?.OnOrder<NativeEventBuffer<NativeOrder>, NativeOrder>(orderBuf);
                    break;
                case EventType.Profile:
                    var profileBuf = NativeBufferFactory.CreateProfileBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxProfileListener)?.OnProfile<NativeEventBuffer<NativeProfile>, NativeProfile>(
                        profileBuf);
                    break;
                case EventType.Quote:
                    var quoteBuf = NativeBufferFactory.CreateQuoteBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxQuoteListener)?.OnQuote<NativeEventBuffer<NativeQuote>, NativeQuote>(quoteBuf);
                    break;
                case EventType.TimeAndSale:
                    var tsBuf = NativeBufferFactory.CreateTimeAndSaleBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxTimeAndSaleListener)
                        ?.OnTimeAndSale<NativeEventBuffer<NativeTimeAndSale>, NativeTimeAndSale>(tsBuf);
                    break;
                case EventType.Trade:
                    var tBuf = NativeBufferFactory.CreateTradeBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxTradeListener)?.OnTrade<NativeEventBuffer<NativeTrade>, NativeTrade>(tBuf);
                    break;
                case EventType.Summary:
                    var sBuf = NativeBufferFactory.CreateSummaryBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxSummaryListener)
                        ?.OnSummary<NativeEventBuffer<NativeSummary>, NativeSummary>(sBuf);
                    break;
                case EventType.Candle:
                    var cBuf = NativeBufferFactory.CreateCandleBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxCandleListener)?.OnCandle<NativeEventBuffer<NativeCandle>, NativeCandle>(cBuf);
                    break;
                case EventType.TradeETH:
                    var teBuf = NativeBufferFactory.CreateTradeEthBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxTradeETHListener)
                        ?.OnTradeETH<NativeEventBuffer<NativeTradeETH>, NativeTradeETH>(teBuf);
                    break;
                case EventType.SpreadOrder:
                    var spreadOrderBuf =
                        NativeBufferFactory.CreateSpreadOrderBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxSpreadOrderListener)
                        ?.OnSpreadOrder<NativeEventBuffer<NativeSpreadOrder>, NativeSpreadOrder>(spreadOrderBuf);
                    break;
                case EventType.Greeks:
                    var greeksBuf = NativeBufferFactory.CreateGreeksBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxGreeksListener)?.OnGreeks<NativeEventBuffer<NativeGreeks>, NativeGreeks>(
                        greeksBuf);
                    break;
                case EventType.TheoPrice:
                    var theoPriceBuf =
                        NativeBufferFactory.CreateTheoPriceBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxTheoPriceListener)
                        ?.OnTheoPrice<NativeEventBuffer<NativeTheoPrice>, NativeTheoPrice>(theoPriceBuf);
                    break;
                case EventType.Underlying:
                    var underlyingBuf =
                        NativeBufferFactory.CreateUnderlyingBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxUnderlyingListener)
                        ?.OnUnderlying<NativeEventBuffer<NativeUnderlying>, NativeUnderlying>(underlyingBuf);
                    break;
                case EventType.Series:
                    var seriesBuf = NativeBufferFactory.CreateSeriesBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxSeriesListener)?.OnSeries<NativeEventBuffer<NativeSeries>, NativeSeries>(
                        seriesBuf);
                    break;
                case EventType.Configuration:
                    var configurationBuf =
                        NativeBufferFactory.CreateConfigurationBuf(symbol, data, dataCount, nativeEventParams);
                    (eventListener as IDxConfigurationListener)
                        ?.OnConfiguration<NativeEventBuffer<NativeConfiguration>,
                            NativeConfiguration>(configurationBuf);
                    break;
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        ///     This code added to correctly implement the disposable pattern.
        /// </summary>
        /// <param name="disposing">The disposing flag</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
            }

            var needToClose = false;

            if (connection != null)
            {
                needToClose = connection.RemoveSubscription(this);
                connection = null;
            }

            if (subscriptionPtr == IntPtr.Zero) return;

            if (needToClose)
            {
                if (callback != null)
                {
                    C.CheckOk(C.Instance.dxf_detach_event_listener_v2(subscriptionPtr, callback));                    
                }
                
                C.CheckOk(C.Instance.dxf_close_subscription(subscriptionPtr));
            }

            subscriptionPtr = IntPtr.Zero;

            disposed = true;
        }

        /// <summary>
        /// Disposes the subscription
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Implementation of IDxSubscription

        /// <summary>
        ///     <para>
        ///         Adds a symbol to the subscription.
        ///     </para>
        ///     <para>
        ///         A wildcard symbol "*" will replace all symbols: there will be an unsubscription from messages on all current symbols
        ///         and a subscription to "*". The subscription type will be changed to STREAM
        ///         If there is already a subscription to "*", then nothing will happen
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbol">Symbol.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbol"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void AddSymbol(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Invalid symbol parameter.");
            if (eventType.HasFlag(EventType.Candle) && IsCandleSymbol(symbol))
            {
                AddSymbol(CandleSymbol.ValueOf(symbol));
            }

            C.CheckOk(C.Instance.dxf_add_symbol(subscriptionPtr, symbol));
        }

        /// <summary>
        ///     Adds a candle symbol to the subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbol"><see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbol"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void AddSymbol(CandleSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentException("Invalid symbol parameter.");

            C.CheckOk(C.Instance.dxf_add_symbol(subscriptionPtr, symbol.ToString()));
        }

        /// <summary>
        ///     <para>
        ///         Adds symbols to the subscription.
        ///     </para>
        ///     <para>
        ///         First met the "*" symbol (wildcard) will overwrite all other symbols: there will be an unsubscription from messages
        ///         on all current symbols and a subscription to "*". The subscription type will be changed to STREAM
        ///         If there is already a subscription to "*", then nothing will happen.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbols">List of symbols.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void AddSymbols(params string[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("Invalid symbols parameter.");
            if (eventType.HasFlag(EventType.Candle) && HasCandleSymbol(symbols))
            {
                foreach (var s in symbols)
                {
                    if (IsCandleSymbol(s))
                        AddSymbol(CandleSymbol.ValueOf(s));
                    else
                        AddSymbol(s);
                }
            }
            else
            {
                C.CheckOk(C.Instance.dxf_add_symbols(subscriptionPtr, symbols, symbols.Length));
            }
        }

        /// <summary>
        ///     Adds candle symbols to the subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbols">List of <see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void AddSymbols(params CandleSymbol[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("Invalid symbol parameter");
            foreach (CandleSymbol symbol in symbols)
            {
                AddSymbol(symbol);
            }
        }

        /// <summary>
        ///     <para>
        ///         Removes symbols from the subscription.
        ///     </para>
        ///     <para>
        ///         First met the "*" symbol (wildcard) will remove all symbols: there will be an unsubscription from messages on all
        ///         current symbols.
        ///         If there is already a subscription to "*" and the symbols to remove are not contain a "*", then nothing will happen.
        ///     </para>
        ///     <para>
        ///         Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbols">List of symbols.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void RemoveSymbols(params string[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("Invalid symbol parameter");
            if (eventType.HasFlag(EventType.Candle) && HasCandleSymbol(symbols))
            {
                foreach (var s in symbols)
                {
                    if (IsCandleSymbol(s))
                        RemoveSymbols(CandleSymbol.ValueOf(s));
                    else
                        C.CheckOk(C.Instance.dxf_remove_symbol(subscriptionPtr, s));
                }
            }
            else
            {
                C.CheckOk(C.Instance.dxf_remove_symbols(subscriptionPtr, symbols, symbols.Length));
            }
        }

        /// <summary>
        ///     <para>
        ///         Removes symbols from the subscription.
        ///     </para>
        ///     <para>
        ///         Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbols">List of <see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void RemoveSymbols(params CandleSymbol[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("Invalid symbol parameter");

            foreach (var symbol in symbols)
            {
                IntPtr candleAttributesPtr;

                C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.BaseSymbol,
                    symbol.ExchangeCode, symbol.PeriodValue, symbol.PeriodId, symbol.PriceId,
                    symbol.SessionId, symbol.AlignmentId, symbol.PriceLevel, out candleAttributesPtr));
                try
                {
                    C.CheckOk(C.Instance.dxf_remove_candle_symbol(subscriptionPtr, candleAttributesPtr));
                }
                finally
                {
                    C.CheckOk(C.Instance.dxf_delete_candle_symbol_attributes(candleAttributesPtr));
                }
            }
        }

        /// <summary>
        ///     Sets symbols of the subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbols">List of symbols.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void SetSymbols(params string[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("Invalid symbol parameter");
            if (eventType.HasFlag(EventType.Candle) && HasCandleSymbol(symbols))
            {
                Clear();
                foreach (var s in symbols)
                {
                    if (IsCandleSymbol(s))
                        AddSymbol(CandleSymbol.ValueOf(s));
                    else
                        AddSymbol(s);
                }
            }
            else
            {
                C.CheckOk(C.Instance.dxf_set_symbols(subscriptionPtr, symbols, symbols.Length));
            }
        }

        /// <summary>
        ///     Sets symbols of the subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbols">List of <see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void SetSymbols(params CandleSymbol[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("Invalid symbol parameter");

            Clear();
            AddSymbols(symbols);
        }

        /// <summary>
        ///     <para>
        ///         Removes all symbols from the subscription.
        ///     </para>
        ///     <para>
        ///         Snapshot will be <see cref="IDisposable.Dispose()"/>.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <exception cref="DxException">Internal error.</exception>
        public void Clear()
        {
            C.CheckOk(C.Instance.dxf_clear_symbols(subscriptionPtr));
        }

        /// <summary>
        ///     Returns all symbols of the subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <returns>List of subscribed symbols.</returns>
        /// <exception cref="DxException">Internal error.</exception>
        public unsafe IList<string> GetSymbols()
        {
            IntPtr head;
            int len;

            C.CheckOk(C.Instance.dxf_get_symbols(subscriptionPtr, out head, out len));

            var result = new string[len];
            for (var i = 0; i < len; i++)
            {
                var ptr = *(IntPtr*)IntPtr.Add(head, IntPtr.Size * i);
                result[i] = Marshal.PtrToStringUni(ptr);
            }

            return result;
        }

        /// <summary>
        ///     Adds a <see cref="OrderSource"/> to the subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="sources">List of <see cref="OrderSource"/> names.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="sources"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void AddSource(params string[] sources)
        {
            if (sources == null || sources.Length == 0)
                throw new ArgumentException("Invalid sources parameter");

            var ascii = Encoding.ASCII;

            foreach (var source in sources)
            {
                var sourceBytes = ascii.GetBytes(source);
                C.CheckOk(C.Instance.dxf_add_order_source(subscriptionPtr, sourceBytes));
            }
        }

        /// <summary>
        ///     Removes the existing <see cref="OrderSource"/> from the subscription and sets the new one.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="sources">List of <see cref="OrderSource"/> names.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="sources"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void SetSource(params string[] sources)
        {
            if (sources == null || sources.Length == 0)
                throw new ArgumentException("Invalid sources parameter");

            var ascii = Encoding.ASCII;
            var source = ascii.GetBytes(sources[0]);

            C.CheckOk(C.Instance.dxf_set_order_source(subscriptionPtr, source));

            for (var i = 1; i < sources.Length; i++)
            {
                source = ascii.GetBytes(sources[i]);
                C.CheckOk(C.Instance.dxf_add_order_source(subscriptionPtr, source));
            }
        }

        #endregion

        internal static bool IsCandleSymbol(string symbol)
        {
            return (MarketEventSymbols.HasExchangeCode(symbol) || MarketEventSymbols.HasAttributes(symbol));
        }

        internal static bool HasCandleSymbol(params string[] symbols)
        {
            return symbols.Any(IsCandleSymbol);
        }

        /// <inheritdoc />
        ~NativeSubscription()
        {
            Dispose(false);
        }
    }
}