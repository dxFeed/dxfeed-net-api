#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

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
    public struct NativeEventBuffer<T> : IDxEventBuf<T>
    {
        private readonly EventType type;
        private readonly IntPtr head;
        private readonly int size;
        private readonly Func<IntPtr, int, string, T> readEvent;
        private readonly string symbol;
        private readonly EventParams eventParams;

        internal unsafe NativeEventBuffer(EventType type, IntPtr symbol, IntPtr head, int size, EventParams eventParams, Func<IntPtr, int, string, T> readEvent)
        {
            this.type = type;
            this.head = head;
            this.size = size;
            this.readEvent = readEvent;
            this.symbol = new string((char*)symbol.ToPointer());
            this.eventParams = eventParams;
        }

        #region Implementation of IEnumerable

        public struct Enumerator : IEnumerator<T>
        {
            private readonly IntPtr head;
            private readonly int size;
            private readonly Func<IntPtr, int, string, T> readEvent;
            private T current;
            private int nextRead;
            private string symbol;

            internal Enumerator(NativeEventBuffer<T> buf)
            {
                head = buf.head;
                size = buf.size;
                readEvent = buf.readEvent;
                nextRead = 0;
                current = default(T);
                symbol = buf.Symbol;
            }

            #region Implementation of IDisposable

            public void Dispose()
            {
            }

            #endregion

            #region Implementation of IEnumerator

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

            public void Reset()
            {
                nextRead = 0;
                current = default(T);
            }

            public T Current
            {
                get
                {
                    if (nextRead == size + 1)
                        throw new InvalidOperationException("Out of bound read");
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            #endregion
        }

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

        public EventType EventType
        {
            get { return type; }
        }

        public string Symbol
        {
            get { return symbol; }
        }

        public int Size
        {
            get { return size; }
        }

        public EventParams EventParams
        {
            get { return eventParams; }
        }

        #endregion
    }

    public class NativeBufferFactory
    {
        private static readonly Func<IntPtr, int, string, NativeQuote> QUOTE_READER = DxMarshal.ReadQuote;
        private static readonly Func<IntPtr, int, string, NativeTrade> TRADE_READER = DxMarshal.ReadTrade;
        private static readonly Func<IntPtr, int, string, NativeOrder> ORDER_READER = DxMarshal.ReadOrder;
        private static readonly Func<IntPtr, int, string, NativeProfile> PROFILE_READER = DxMarshal.ReadProfile;
        private static readonly Func<IntPtr, int, string, NativeTimeAndSale> TS_READER = DxMarshal.ReadTimeAndSale;
        private static readonly Func<IntPtr, int, string, NativeSummary> SUMMARY_READER = DxMarshal.ReadSummary;
        private static readonly Func<IntPtr, int, string, NativeCandle> CANDLE_READER = DxMarshal.ReadCandle;
        private static readonly Func<IntPtr, int, string, NativeTradeETH> TRADE_ETH_READER = DxMarshal.ReadTradeETH;
        private static readonly Func<IntPtr, int, string, NativeSpreadOrder> SPREAD_ORDER_READER = DxMarshal.ReadSpreadOrder;
        private static readonly Func<IntPtr, int, string, NativeGreeks> GREEKS_READER = DxMarshal.ReadGreeks;
        private static readonly Func<IntPtr, int, string, NativeTheoPrice> THEO_PRICE_READER = DxMarshal.ReadTheoPrice;
        private static readonly Func<IntPtr, int, string, NativeUnderlying> UNDERLYING_READER = DxMarshal.ReadUnderlying;
        private static readonly Func<IntPtr, int, string, NativeSeries> SERIES_READER = DxMarshal.ReadSeries;
        private static readonly Func<IntPtr, int, string, NativeConfiguration> CONFIGURATION_READER = DxMarshal.ReadConfiguration;


        public static NativeEventBuffer<NativeQuote> CreateQuoteBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeQuote>(EventType.Quote, symbol, head, size, eventParams, QUOTE_READER);
        }

        public static NativeEventBuffer<NativeTrade> CreateTradeBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeTrade>(EventType.Trade, symbol, head, size, eventParams, TRADE_READER);
        }

        public static NativeEventBuffer<NativeOrder> CreateOrderBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeOrder>(EventType.Order, symbol, head, size, eventParams, ORDER_READER);
        }

        public static NativeEventBuffer<NativeProfile> CreateProfileBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeProfile>(EventType.Profile, symbol, head, size, eventParams, PROFILE_READER);
        }

        public static NativeEventBuffer<NativeTimeAndSale> CreateTimeAndSaleBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeTimeAndSale>(EventType.TimeAndSale, symbol, head, size, eventParams, TS_READER);
        }

        public static NativeEventBuffer<NativeSummary> CreateSummaryBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeSummary>(EventType.Summary, symbol, head, size, eventParams, SUMMARY_READER);
        }

        public static NativeEventBuffer<NativeCandle> CreateCandleBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeCandle>(EventType.Candle, symbol, head, size, eventParams, CANDLE_READER);
        }

        public static NativeEventBuffer<NativeTradeETH> CreateTradeETHBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeTradeETH>(EventType.TradeETH, symbol, head, size, eventParams, TRADE_ETH_READER);
        }

        public static NativeEventBuffer<NativeSpreadOrder> CreateSpreadOrderBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeSpreadOrder>(EventType.SpreadOrder, symbol, head, size, eventParams, SPREAD_ORDER_READER);
        }

        public static NativeEventBuffer<NativeGreeks> CreateGreeksBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeGreeks>(EventType.Greeks, symbol, head, size, eventParams, GREEKS_READER);
        }

        public static NativeEventBuffer<NativeTheoPrice> CreateTheoPriceBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeTheoPrice>(EventType.TheoPrice, symbol, head, size, eventParams, THEO_PRICE_READER);
        }

        public static NativeEventBuffer<NativeUnderlying> CreateUnderlyingBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeUnderlying>(EventType.Underlying, symbol, head, size, eventParams, UNDERLYING_READER);
        }

        public static NativeEventBuffer<NativeSeries> CreateSeriesBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeSeries>(EventType.Series, symbol, head, size, eventParams, SERIES_READER);
        }

        public static NativeEventBuffer<NativeConfiguration> CreateConfigurationBuf(IntPtr symbol, IntPtr head, int size, EventParams eventParams)
        {
            return new NativeEventBuffer<NativeConfiguration>(EventType.Configuration, symbol, head, size, eventParams, CONFIGURATION_READER);
        }
    }
}
