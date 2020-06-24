#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using System;
using System.Runtime.InteropServices;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Order event implementation for test event player.
    /// </summary>
    internal class PlayedOrder : IPlayedEvent<DxTestOrder>, IDxOrder
    {

        internal unsafe PlayedOrder(string symbol, EventFlag event_flags, long index,
            long time, int time_nanos, int sequence,
            double price, int size, int count,
            Scope scope, Side side, char exchange_code,
            IndexedEventSource source, string mm)
        {
            this.EventSymbol = symbol;
            this.EventFlags = event_flags;
            this.Index = index;
            this.Time = Tools.UnixTimeToDate(time);
            this.TimeNanoPart = time_nanos;
            this.Sequence = sequence;
            this.Price = price;
            this.Size = size;
            this.Count = count;
            this.Scope = scope;
            this.Side = side;
            this.ExchangeCode = exchange_code;
            this.Source = source;
            fixed (char* pMarketMaker = mm.ToCharArray())
            {
                this.MarketMaker = new string(pMarketMaker);
            }

            marketMakerCharArray = mm.ToCharArray();
            IntPtr marketMakerCharsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(marketMakerCharArray, 0);
            Params = new EventParams(EventFlags, (ulong)Index, 0);
            Data = new DxTestOrder(event_flags, index, time, time_nanos, sequence, price, size, count, scope, side, exchange_code, source, marketMakerCharsPtr);
        }

        /// <summary>
        ///     Creates Order event from another object.
        /// </summary>
        /// <param name="order">Other Order object.</param>
        internal PlayedOrder(IDxOrder order)
        {
            this.EventSymbol = order.EventSymbol;
            this.EventFlags = order.EventFlags;
            this.Index = order.Index;
            this.Time = order.Time;
            this.TimeNanoPart = order.TimeNanoPart;
            this.Sequence = order.Sequence;
            this.Price = order.Price;
            this.Size = order.Size;
            this.Count = order.Count;
            this.Scope = order.Scope;
            this.Side = order.Side;
            this.ExchangeCode = order.ExchangeCode;
            this.Source = order.Source;
            this.MarketMaker = order.MarketMaker;

            marketMakerCharArray = MarketMaker.ToString().ToCharArray();
            IntPtr marketMakerCharsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(marketMakerCharArray, 0);
            Params = new EventParams(EventFlags, (ulong)Index, 0);
            Data = new DxTestOrder(EventFlags, Index, Tools.DateToUnixTime(Time), TimeNanoPart, Sequence, Price, (int)Size, Count, Scope, Side, ExchangeCode, Source, marketMakerCharsPtr);
        }

        public int Count
        {
            get; private set;
        }

        public DxTestOrder Data
        {
            get; private set;
        }

        public EventFlag EventFlags
        {
            get; set;
        }

        public string EventSymbol
        {
            get; private set;
        }

        public char ExchangeCode
        {
            get; private set;
        }

        public long Index
        {
            get; private set;
        }

        public string MarketMaker
        {
            get; private set;
        }

        public EventParams Params
        {
            get; private set;
        }

        public double Price
        {
            get; private set;
        }

        public Scope Scope
        {
            get; private set;
        }

        public int Sequence
        {
            get; private set;
        }

        public Side Side
        {
            get; private set;
        }

        public long Size
        {
            get; private set;
        }

        public IndexedEventSource Source
        {
            get; private set;
        }

        public DateTime Time
        {
            get; private set;
        }

        public int TimeNanoPart
        {
            get; private set;
        }

        object IDxEventType.EventSymbol
        {
            get
            {
                return EventSymbol as object;
            }
        }

        object IPlayedEvent.Data
        {
            get
            {
                return Data as object;
            }
        }

        public object Clone()
        {
            return new PlayedOrder(this);
        }

        #region Private fields

        private char[] marketMakerCharArray;

        #endregion

    }
}
