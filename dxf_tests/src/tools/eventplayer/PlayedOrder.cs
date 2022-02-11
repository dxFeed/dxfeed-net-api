#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Runtime.InteropServices;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Order event implementation for test event player.
    /// </summary>
    internal class PlayedOrder : IPlayedEvent<DxTestOrder>, IDxOrder
    {
        #region Private fields

        private readonly char[] marketMakerCharArray;

        #endregion

        internal unsafe PlayedOrder(string symbol, EventFlag event_flags, long index,
            long time, int time_nanos, int sequence,
            double price, int size, int count,
            Scope scope, Side side, char exchange_code,
            IndexedEventSource source, string mm)
        {
            EventSymbol = symbol;
            EventFlags = event_flags;
            Index = index;
            Time = Tools.UnixTimeToDate(time);
            TimeNanoPart = time_nanos;
            Sequence = sequence;
            Price = price;
            Size = size;
            Count = count;
            Scope = scope;
            Side = side;
            ExchangeCode = exchange_code;
            Source = source;
            fixed (char* pMarketMaker = mm.ToCharArray())
            {
                MarketMaker = new string(pMarketMaker);
            }

            marketMakerCharArray = mm.ToCharArray();
            var marketMakerCharsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(marketMakerCharArray, 0);
            Params = new EventParams(EventFlags, (ulong) Index, 0);
            Data = new DxTestOrder(source, event_flags, index, time, sequence, time_nanos, OrderAction.Undefined, 0, 0,
                0, price, size, 0, count, 0, 0, 0, exchange_code, side, scope, marketMakerCharsPtr);
        }

        /// <summary>
        ///     Creates Order event from another object.
        /// </summary>
        /// <param name="order">Other Order object.</param>
        internal PlayedOrder(IDxOrder order)
        {
            EventSymbol = order.EventSymbol;
            EventFlags = order.EventFlags;
            Index = order.Index;
            Time = order.Time;
            TimeNanoPart = order.TimeNanoPart;
            Sequence = order.Sequence;
            Action = order.Action;
            ActionTime = order.ActionTime;
            OrderId = order.OrderId;
            AuxOrderId = order.AuxOrderId;
            Price = order.Price;
            Size = order.Size;
            Count = order.Count;
            TradeId = order.TradeId;
            TradePrice = order.TradePrice;
            TradeSize = order.TradeSize;
            Scope = order.Scope;
            Side = order.Side;
            ExchangeCode = order.ExchangeCode;
            Source = order.Source;
            MarketMaker = order.MarketMaker;

            marketMakerCharArray = MarketMaker.ToCharArray();
            var marketMakerCharsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(marketMakerCharArray, 0);
            Params = new EventParams(EventFlags, (ulong) Index, 0);
            Data = new DxTestOrder(Source, EventFlags, Index, Tools.DateToUnixTime(Time), Sequence, TimeNanoPart,
                Action, Tools.DateToUnixTime(ActionTime), OrderId, AuxOrderId, Price, Size, ExecutedSize, Count,
                TradeId, TradePrice, TradeSize, ExchangeCode, Side, Scope, marketMakerCharsPtr);
        }

        public double Count { get; }

        public long TradeId { get; }
        public double TradePrice { get; }
        public double TradeSize { get; }

        public EventFlag EventFlags { get; set; }

        public string EventSymbol { get; }

        public char ExchangeCode { get; }

        public long Index { get; }

        public string MarketMaker { get; }

        public long AuxOrderId { get; }

        public double Price { get; }

        public Scope Scope { get; }

        public int Sequence { get; }

        public Side Side { get; }

        public bool HasSize()
        {
            return Size != 0 && !double.IsNaN(Size);
        }

        public double ExecutedSize { get; private set; }

        public double Size { get; }

        public IndexedEventSource Source { get; }

        public DateTime Time { get; }

        public int TimeNanoPart { get; }

        public OrderAction Action { get; }
        public DateTime ActionTime { get; }
        public long OrderId { get; }

        object IDxEventType.EventSymbol => EventSymbol;

        public object Clone()
        {
            return new PlayedOrder(this);
        }

        public DxTestOrder Data { get; }

        public EventParams Params { get; }

        object IPlayedEvent.Data => Data;
    }
}