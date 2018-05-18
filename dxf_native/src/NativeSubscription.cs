/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
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
    /// </summary>
    public class NativeSubscription : IDxSubscription
    {
        private readonly IntPtr connectionPtr;
        private IntPtr subscriptionPtr;
        private readonly IDxEventListener eventListener;
        //to prevent callback from being garbage collected
        private readonly C.dxf_event_listener_v2_t callback;
        private readonly EventType eventType;

        /// <summary>
        /// Create event subscription.
        /// </summary>
        /// <param name="connection">native connection pointer</param>
        /// <param name="eventType">type of event to create</param>
        /// <param name="listener">event listener</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException"></exception>
        public NativeSubscription(NativeConnection connection, EventType eventType, IDxEventListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            connectionPtr = connection.Handler;
            this.eventType = eventType;
            eventListener = listener;

            C.CheckOk(C.Instance.dxf_create_subscription(connectionPtr, eventType, out subscriptionPtr));
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
        /// Create time event subscription.
        /// </summary>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="eventType">Type of event to create.</param>
        /// <param name="time">Unix time stamp (the number of milliseconds from 1.1.1970)</param>
        /// <param name="listener">Event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException"></exception>
        public NativeSubscription(NativeConnection connection, EventType eventType, long time, IDxEventListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            connectionPtr = connection.Handler;
            this.eventType = eventType;
            eventListener = listener;

            C.CheckOk(C.Instance.dxf_create_subscription_timed(connectionPtr, eventType, time, out subscriptionPtr));
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
        /// Create time event subscription.
        /// </summary>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="eventType">Type of event to create.</param>
        /// <param name="time">Time to getting events from.</param>
        /// <param name="listener">Event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException"></exception>
        public NativeSubscription(NativeConnection connection, EventType eventType, DateTime time, IDxEventListener listener) :
            this(connection, eventType, Tools.DateToUnixTime(time), listener)
        {}

        /// <summary>
        /// Create Candle event subscription.
        /// For rest events use another constructor.
        /// </summary>
        /// <param name="connection">Native connection pointer.</param>
        /// <param name="time">Date time in the past.</param>
        /// <param name="listener">Candle event listener.</param>
        /// <exception cref="ArgumentException">One of passed parameters is not valid.</exception>
        /// <exception cref="DxException"></exception>
        public NativeSubscription(NativeConnection connection, DateTime? time, IDxCandleListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            connectionPtr = connection.Handler;
            eventType = EventType.Candle;
            eventListener = listener;

            long unixTimestamp = (time == null ? 0 : Tools.DateToUnixTime((DateTime)time));
            C.CheckOk(C.Instance.dxf_create_subscription_timed(connectionPtr, eventType, unixTimestamp, out subscriptionPtr));

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

        private void OnEvent(EventType eventType, IntPtr symbol, IntPtr data, int dataCount, IntPtr eventParamsPtr, IntPtr userData)
        {
            object obj = Marshal.PtrToStructure(eventParamsPtr, typeof(DxEventParams));
            DxEventParams event_params = (DxEventParams)obj;

            EventParams nativeEventParams = new EventParams(event_params.flags, event_params.time_int_field, event_params.snapshot_key);
            switch (eventType)
            {
                case EventType.Order:
                    var orderBuf = NativeBufferFactory.CreateOrderBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxOrderListener)
                        (eventListener as IDxOrderListener).OnOrder<NativeEventBuffer<NativeOrder>, NativeOrder>(orderBuf);
                    break;
                case EventType.Profile:
                    var profileBuf = NativeBufferFactory.CreateProfileBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxProfileListener)
                        (eventListener as IDxProfileListener).OnProfile<NativeEventBuffer<NativeProfile>, NativeProfile>(profileBuf);
                    break;
                case EventType.Quote:
                    var quoteBuf = NativeBufferFactory.CreateQuoteBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxQuoteListener)
                        (eventListener as IDxQuoteListener).OnQuote<NativeEventBuffer<NativeQuote>, NativeQuote>(quoteBuf);
                    break;
                case EventType.TimeAndSale:
                    var tsBuf = NativeBufferFactory.CreateTimeAndSaleBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxTimeAndSaleListener)
                        (eventListener as IDxTimeAndSaleListener).OnTimeAndSale<NativeEventBuffer<NativeTimeAndSale>, NativeTimeAndSale>(tsBuf);
                    break;
                case EventType.Trade:
                    var tBuf = NativeBufferFactory.CreateTradeBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxTradeListener)
                        (eventListener as IDxTradeListener).OnTrade<NativeEventBuffer<NativeTrade>, NativeTrade>(tBuf);
                    break;
                case EventType.Summary:
                    var sBuf = NativeBufferFactory.CreateSummaryBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxFundamentalListener)
                        (eventListener as IDxFundamentalListener).OnFundamental<NativeEventBuffer<NativeSummary>, NativeSummary>(sBuf);
                    break;
                case EventType.Candle:
                    var cBuf = NativeBufferFactory.CreateCandleBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxCandleListener)
                        (eventListener as IDxCandleListener).OnCandle<NativeEventBuffer<NativeCandle>, NativeCandle>(cBuf);
                    break;
                case EventType.TradeETH:
                    var teBuf = NativeBufferFactory.CreateTradeETHBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxTradeETHListener)
                        (eventListener as IDxTradeETHListener).OnTradeETH<NativeEventBuffer<NativeTradeETH>, NativeTradeETH>(teBuf);
                    break;
                case EventType.SpreadOrder:
                    var spreadOrderBuf = NativeBufferFactory.CreateSpreadOrderBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxSpreadOrderListener)
                        (eventListener as IDxSpreadOrderListener).OnSpreadOrder<NativeEventBuffer<NativeSpreadOrder>, NativeSpreadOrder>(spreadOrderBuf);
                    break;
                case EventType.Greeks:
                    var greeksBuf = NativeBufferFactory.CreateGreeksBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxGreeksListener)
                        (eventListener as IDxGreeksListener).OnGreeks<NativeEventBuffer<NativeGreeks>, NativeGreeks>(greeksBuf);
                    break;
                case EventType.TheoPrice:
                    var theoPriceBuf = NativeBufferFactory.CreateTheoPriceBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxTheoPriceListener)
                        (eventListener as IDxTheoPriceListener).OnTheoPrice<NativeEventBuffer<NativeTheoPrice>, NativeTheoPrice>(theoPriceBuf);
                    break;
                case EventType.Underlying:
                    var underlyingBuf = NativeBufferFactory.CreateUnderlyingBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxUnderlyingListener)
                        (eventListener as IDxUnderlyingListener).OnUnderlying<NativeEventBuffer<NativeUnderlying>, NativeUnderlying>(underlyingBuf);
                    break;
                case EventType.Series:
                    var seriesBuf = NativeBufferFactory.CreateSeriesBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxSeriesListener)
                        (eventListener as IDxSeriesListener).OnSeries<NativeEventBuffer<NativeSeries>, NativeSeries>(seriesBuf);
                    break;
                case EventType.Configuration:
                    var configurationBuf = NativeBufferFactory.CreateConfigurationBuf(symbol, data, dataCount, nativeEventParams);
                    if (eventListener is IDxConfigurationListener)
                        (eventListener as IDxConfigurationListener).OnConfiguration<NativeEventBuffer<NativeConfiguration>, NativeConfiguration>(configurationBuf);
                    break;
            }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (subscriptionPtr == IntPtr.Zero) return;

            C.CheckOk(C.Instance.dxf_close_subscription(subscriptionPtr));
            subscriptionPtr = IntPtr.Zero;
        }

        #endregion

        #region Implementation of IDxSubscription

        /// <summary>
        ///     Add symbol to subscription.
        /// </summary>
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
        ///     Add candle symbol to subscription.
        /// </summary>
        /// <param name="symbol"><see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbol"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void AddSymbol(CandleSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentException("Invalid symbol parameter.");
            IntPtr candleAttributesPtr = IntPtr.Zero;
            C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.BaseSymbol,
                symbol.ExchangeCode, symbol.PeriodValue, symbol.PeriodId, symbol.PriceId,
                symbol.SessionId, symbol.AlignmentId, out candleAttributesPtr));
            try
            {
                C.CheckOk(C.Instance.dxf_add_candle_symbol(subscriptionPtr, candleAttributesPtr));
            }
            finally
            {
                C.CheckOk(C.Instance.dxf_delete_candle_symbol_attributes(candleAttributesPtr));
            }
        }

        /// <summary>
        ///     Add multiply symbols to subscription.
        /// </summary>
        /// <param name="symbols">List of symbols.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbol"/> parameter.</exception>
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
        ///     Add multiply candle symbols to subscription.
        /// </summary>
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
        ///         Remove multiply symbols from subscription.
        ///     </para>
        ///     <para>
        ///         Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
        ///     </para>
        /// </summary>
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
        ///         Remove multiply symbols from subscription.
        ///     </para>
        ///     <para>
        ///         Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
        ///     </para>
        /// </summary>
        /// <param name="symbols">List of <see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void RemoveSymbols(params CandleSymbol[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("Invalid symbol parameter");
            foreach (CandleSymbol symbol in symbols)
            {
                IntPtr candleAttributesPtr = IntPtr.Zero;
                C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.BaseSymbol,
                    symbol.ExchangeCode, symbol.PeriodValue, symbol.PeriodId, symbol.PriceId,
                    symbol.SessionId, symbol.AlignmentId, out candleAttributesPtr));
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
        ///     Set multiply symbols to subscription.
        /// </summary>
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
        ///     Set multiply symbols to subscription.
        /// </summary>
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
        ///         Clear all symbols from subscription.
        ///     </para>
        ///     <para>
        ///         Snapshot will be <see cref="IDisposable.Dispose()"/>.
        ///     </para>
        /// </summary>
        /// <exception cref="DxException">Internal error.</exception>
        public void Clear()
        {
            C.CheckOk(C.Instance.dxf_clear_symbols(subscriptionPtr));
        }

        /// <summary>
        ///     Get all symbols from subscription.
        /// </summary>
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
        ///     Add <see cref="OrderSource"/> to subscription.
        /// </summary>
        /// <param name="sources">List of <see cref="OrderSource"/> names.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="sources"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void AddSource(params string[] sources)
        {
            if (sources == null || sources.Length == 0)
                throw new ArgumentException("Invalid sources parameter");

            Encoding ascii = Encoding.ASCII;
            for (int i = 0; i < sources.Length; i++)
            {
                byte[] source = ascii.GetBytes(sources[i]);
                C.CheckOk(C.Instance.dxf_add_order_source(subscriptionPtr, source));
            }
        }

        /// <summary>
        ///     Remove existing <see cref="OrderSource"/> from subscription and set new.
        /// </summary>
        /// <param name="sources">List of <see cref="OrderSource"/> names.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="sources"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public void SetSource(params string[] sources)
        {
            if (sources == null || sources.Length == 0)
                throw new ArgumentException("Invalid sources parameter");

            Encoding ascii = Encoding.ASCII;
            byte[] source = ascii.GetBytes(sources[0]);
            C.CheckOk(C.Instance.dxf_set_order_source(subscriptionPtr, source));
            for (int i = 1; i < sources.Length; i++)
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
            foreach (var s in symbols)
                if (IsCandleSymbol(s))
                    return true;
            return false;
        }
    }
}
