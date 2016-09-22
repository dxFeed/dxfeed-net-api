/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

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
        private readonly Func<IntPtr, int, T> readEvent;
        private readonly DxString symbol;
        private readonly EventParams eventParams;

        internal unsafe NativeEventBuffer(EventType type, IntPtr symbol, IntPtr head, int size, EventParams eventParams, Func<IntPtr, int, T> readEvent)
        {
            this.type = type;
            this.head = head;
            this.size = size;
            this.readEvent = readEvent;
            this.symbol = new DxString((char*)symbol.ToPointer());
            this.eventParams = eventParams;
        }

        #region Implementation of IEnumerable
        public struct Enumerator : IEnumerator<T>
        {
            private readonly IntPtr head;
            private readonly int size;
            private readonly Func<IntPtr, int, T> readEvent;
            private T current;
            private int nextRead;

            internal Enumerator(NativeEventBuffer<T> buf)
            {
                head = buf.head;
                size = buf.size;
                readEvent = buf.readEvent;
                nextRead = 0;
                current = default(T);
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
                current = readEvent(head, nextRead);
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

        public DxString Symbol
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
        private static readonly Func<IntPtr, int, NativeQuote> QUOTE_READER = DxMarshal.ReadQuote;
        private static readonly Func<IntPtr, int, NativeTrade> TRADE_READER = DxMarshal.ReadTrade;
        private static readonly Func<IntPtr, int, NativeOrder> ORDER_READER = DxMarshal.ReadOrder;
        private static readonly Func<IntPtr, int, NativeProfile> PROFILE_READER = DxMarshal.ReadProfile;
        private static readonly Func<IntPtr, int, NativeTimeAndSale> TS_READER = DxMarshal.ReadTimeAndSale;
        private static readonly Func<IntPtr, int, NativeSummary> SUMMARY_READER = DxMarshal.ReadSummary;
        private static readonly Func<IntPtr, int, NativeCandle> CANDLE_READER = DxMarshal.ReadCandle;


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
    }
}
