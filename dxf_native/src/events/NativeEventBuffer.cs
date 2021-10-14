#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native.events
{
    /// <summary>
    ///     The class that describes a native event buffer
    /// </summary>
    /// <typeparam name="T">The event type</typeparam>
    public struct NativeEventBuffer<T> : IDxEventBuf<T>
    {
        private readonly IntPtr head;
        private readonly Func<IntPtr, int, string, T> readEvent;

        internal unsafe NativeEventBuffer(EventType type, IntPtr symbol, IntPtr head, int size, EventParams eventParams,
            Func<IntPtr, int, string, T> readEvent)
        {
            EventType = type;
            this.head = head;
            Size = size;
            this.readEvent = readEvent;
            Symbol = new string((char*)symbol.ToPointer());
            EventParams = eventParams;
        }

        #region Implementation of IEnumerable

        /// <summary>
        ///     The native event buffer's enumerator
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly IntPtr head;
            private readonly int size;
            private readonly Func<IntPtr, int, string, T> readEvent;
            private T current;
            private int nextRead;
            private readonly string symbol;

            internal Enumerator(NativeEventBuffer<T> buf)
            {
                head = buf.head;
                size = buf.Size;
                readEvent = buf.readEvent;
                nextRead = 0;
                current = default(T);
                symbol = buf.Symbol;
            }

            #region Implementation of IDisposable

            /// <inheritdoc />
            public void Dispose()
            {
            }

            #endregion

            #region Implementation of IEnumerator

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (nextRead == size)
                {
                    current = default(T);
                    return false;
                }

                current = readEvent(head, nextRead, symbol);
                nextRead++;
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                nextRead = 0;
                current = default(T);
            }

            /// <inheritdoc />
            public T Current
            {
                get
                {
                    if (nextRead == size + 1)
                        throw new InvalidOperationException("Out of bound read");
                    return current;
                }
            }

            object IEnumerator.Current => Current;

            #endregion
        }

        /// <summary>
        ///     Returns current buffer's enumerator
        /// </summary>
        /// <returns>The new enumerator of the buffer</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Implementation of IDxEventBuf<out T>

        /// <inheritdoc />
        public EventType EventType { get; }

        /// <inheritdoc />
        public string Symbol { get; }

        /// <inheritdoc />
        public int Size { get; }

        /// <inheritdoc />
        public EventParams EventParams { get; }

        #endregion
    }

    /// <summary>
    ///     The factory that used for native buffer creation
    /// </summary>
    public class NativeBufferFactory
    {
        private static readonly Func<IntPtr, int, string, NativeQuote> QuoteReader = DxMarshal.ReadQuote;
        private static readonly Func<IntPtr, int, string, NativeTrade> TradeReader = DxMarshal.ReadTrade;
        private static readonly Func<IntPtr, int, string, NativeOrder> OrderReader = DxMarshal.ReadOrder;
        private static readonly Func<IntPtr, int, string, NativeProfile> ProfileReader = DxMarshal.ReadProfile;
        private static readonly Func<IntPtr, int, string, NativeTimeAndSale> TsReader = DxMarshal.ReadTimeAndSale;
        private static readonly Func<IntPtr, int, string, NativeSummary> SummaryReader = DxMarshal.ReadSummary;
        private static readonly Func<IntPtr, int, string, NativeCandle> CandleReader = DxMarshal.ReadCandle;
        private static readonly Func<IntPtr, int, string, NativeTradeETH> TradeEthReader = DxMarshal.ReadTradeEth;

        private static readonly Func<IntPtr, int, string, NativeSpreadOrder> SpreadOrderReader =
            DxMarshal.ReadSpreadOrder;

        private static readonly Func<IntPtr, int, string, NativeGreeks> GreeksReader = DxMarshal.ReadGreeks;
        private static readonly Func<IntPtr, int, string, NativeTheoPrice> TheoPriceReader = DxMarshal.ReadTheoPrice;

        private static readonly Func<IntPtr, int, string, NativeUnderlying>
            UnderlyingReader = DxMarshal.ReadUnderlying;

        private static readonly Func<IntPtr, int, string, NativeSeries> SeriesReader = DxMarshal.ReadSeries;

        private static readonly Func<IntPtr, int, string, NativeConfiguration> ConfigurationReader =
            DxMarshal.ReadConfiguration;


        /// <summary>
        ///     Creates the NativeQuote buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeQuote buffer</returns>
        public static NativeEventBuffer<NativeQuote> CreateQuoteBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeQuote>(EventType.Quote, symbol, head, size, eventParams, QuoteReader);
        }

        /// <summary>
        ///     Creates the NativeTrade buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeTrade buffer</returns>
        public static NativeEventBuffer<NativeTrade> CreateTradeBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeTrade>(EventType.Trade, symbol, head, size, eventParams, TradeReader);
        }

        /// <summary>
        ///     Creates the NativeOrder buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeOrder buffer</returns>
        public static NativeEventBuffer<NativeOrder> CreateOrderBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeOrder>(EventType.Order, symbol, head, size, eventParams, OrderReader);
        }

        /// <summary>
        ///     Creates the NativeProfile buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeProfile buffer</returns>
        public static NativeEventBuffer<NativeProfile> CreateProfileBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeProfile>(EventType.Profile, symbol, head, size, eventParams,
                ProfileReader);
        }

        /// <summary>
        ///     Creates the NativeTimeAndSale buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeTimeAndSale buffer</returns>
        public static NativeEventBuffer<NativeTimeAndSale> CreateTimeAndSaleBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeTimeAndSale>(EventType.TimeAndSale, symbol, head, size, eventParams,
                TsReader);
        }

        /// <summary>
        ///     Creates the NativeSummary buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeSummary buffer</returns>
        public static NativeEventBuffer<NativeSummary> CreateSummaryBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeSummary>(EventType.Summary, symbol, head, size, eventParams,
                SummaryReader);
        }

        /// <summary>
        ///     Creates the NativeCandle buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeCandle buffer</returns>
        public static NativeEventBuffer<NativeCandle> CreateCandleBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeCandle>(EventType.Candle, symbol, head, size, eventParams,
                CandleReader);
        }

        /// <summary>
        ///     Creates the NativeTradeETH buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeTradeETH buffer</returns>
        public static NativeEventBuffer<NativeTradeETH> CreateTradeEthBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeTradeETH>(EventType.TradeETH, symbol, head, size, eventParams,
                TradeEthReader);
        }

        /// <summary>
        ///     Creates the NativeSpreadOrder buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeSpreadOrder buffer</returns>
        public static NativeEventBuffer<NativeSpreadOrder> CreateSpreadOrderBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeSpreadOrder>(EventType.SpreadOrder, symbol, head, size, eventParams,
                SpreadOrderReader);
        }

        /// <summary>
        ///     Creates the NativeGreeks buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeGreeks buffer</returns>
        public static NativeEventBuffer<NativeGreeks> CreateGreeksBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeGreeks>(EventType.Greeks, symbol, head, size, eventParams,
                GreeksReader);
        }

        /// <summary>
        ///     Creates the NativeTheoPrice buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeTheoPrice buffer</returns>
        public static NativeEventBuffer<NativeTheoPrice> CreateTheoPriceBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeTheoPrice>(EventType.TheoPrice, symbol, head, size, eventParams,
                TheoPriceReader);
        }

        /// <summary>
        ///     Creates the NativeUnderlying buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeUnderlying buffer</returns>
        public static NativeEventBuffer<NativeUnderlying> CreateUnderlyingBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeUnderlying>(EventType.Underlying, symbol, head, size, eventParams,
                UnderlyingReader);
        }

        /// <summary>
        ///     Creates the NativeSeries buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeSeries buffer</returns>
        public static NativeEventBuffer<NativeSeries> CreateSeriesBuf(IntPtr symbol, IntPtr head, int size,
            EventParams eventParams)
        {
            return new NativeEventBuffer<NativeSeries>(EventType.Series, symbol, head, size, eventParams,
                SeriesReader);
        }

        /// <summary>
        ///     Creates the NativeConfiguration buffer
        /// </summary>
        /// <param name="symbol">The address of an event symbol</param>
        /// <param name="head">The start address of events</param>
        /// <param name="size">The buffer size</param>
        /// <param name="eventParams">The event params</param>
        /// <returns>The new NativeConfiguration buffer</returns>
        public static NativeEventBuffer<NativeConfiguration> CreateConfigurationBuf(IntPtr symbol, IntPtr head,
            int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeConfiguration>(EventType.Configuration, symbol, head, size, eventParams,
                ConfigurationReader);
        }
    }
}