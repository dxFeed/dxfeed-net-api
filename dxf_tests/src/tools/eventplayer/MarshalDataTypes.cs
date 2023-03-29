#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;
using com.dxfeed.api.events;
using System;
using System.Runtime.InteropServices;
using com.dxfeed.native.api;

namespace com.dxfeed.tests.tools.eventplayer
{
    //Note: copy code from CDataTypes.cs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct DxTestEventParams
    {
        internal EventFlag flags;
        internal ulong time_int_field;
        internal ulong snapshot_key;

        internal DxTestEventParams(EventFlag flags, ulong time_int_field, ulong snapshot_key)
        {
            this.flags = flags;
            this.time_int_field = time_int_field;
            this.snapshot_key = snapshot_key;
        }
    }

    //Note: copy code from CDataTypes.cs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct DxTestOrder
    {
        /// <summary>
        /// Source of this order
        /// </summary>
        internal fixed char source[Constants.RecordSuffixSize];

        /// <summary>
        /// Transactional event flags.
        /// </summary>
        internal EventFlag event_flags;

        /// <summary>
        /// Unique per-symbol index of this order.
        /// </summary>
        internal long index;

        /// <summary>
        /// Time of this order. Time is measured in milliseconds between the current time and midnight,
        /// January 1, 1970 UTC.
        /// </summary>
        internal long time;

        /// <summary>
        /// Sequence number of this order to distinguish orders that have the same time.
        /// </summary>
        internal int sequence;

        /// <summary>
        /// Microseconds and nanoseconds part of time of this order.
        /// </summary>
        internal int time_nanos;

        /// <summary>
        /// Order action if available, otherwise - OrderAction.Undefined.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        internal OrderAction action;

        /// <summary>
        /// Time of the last NativeOrder.Action if available, otherwise - 0.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        internal long action_time;

        /// <summary>
        /// Contains order ID if available, otherwise - 0. Some actions OrderAction.Trade, OrderAction.Bust have no
        /// order since they are not related to any order in Order book.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        internal long order_id;

        /// <summary>
        /// Contains auxiliary order ID if available, otherwise - 0:
        /// - in OrderAction.New - ID of the order replaced by this new order
        /// - in OrderAction.Delete - ID of the order that replaces this deleted order
        /// - in OrderAction.Partial - ID of the aggressor order
        /// - in OrderAction.Execute - ID of the aggressor order
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        internal long aux_order_id;

        /// <summary>
        /// Price of this order.
        /// </summary>
        internal double price;

        /// <summary>
        /// Size of this order
        /// </summary>
        internal double size;

        /// <summary>
        /// Executed size of this order
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        internal double executed_size;

        /// <summary>
        /// Number of individual orders in this aggregate order.
        /// </summary>
        internal double count;

        /// <summary>
        /// Contains trade (order execution) ID for events containing trade-related action if available, otherwise - 0.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        internal long trade_id;

        /// <summary>
        /// Contains trade price for events containing trade-related action.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        internal double trade_price;

        /// <summary>
        /// Contains trade size for events containing trade-related action.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        internal double trade_size;

        /// <summary>
        /// Exchange code of this order
        /// </summary>
        internal char exchange_code;

        /// <summary>
        /// Side of this order
        /// </summary>
        internal Side side;

        /// <summary>
        /// Scope of this order
        /// </summary>
        internal Scope scope;

        /// <summary>
        /// Market maker of this order or spread symbol of this spread order
        ///
        /// String
        /// </summary>
        internal IntPtr mm_or_ss;

        internal DxTestOrder(IndexedEventSource source, EventFlag event_flags, long index,
            long time, int sequence, int time_nanos, OrderAction action, long action_time, long order_id, long aux_order_id,
            double price, double size, double executed_size, double count, long trade_id, double trade_price, double trade_size, char exchange_code,
            Side side, Scope scope, IntPtr mm_or_ss)
        {
            fixed (char* pSource = this.source)
            {
                var length = Math.Min(4, source.Name.Length);
                Marshal.Copy(source.Name.ToCharArray(), 0, (IntPtr)pSource, length);
                pSource[length] = (char)0;
            }

            this.event_flags = event_flags;
            this.index = index;
            this.time = time;
            this.sequence = sequence;
            this.time_nanos = time_nanos;
            this.action = action;
            this.action_time = action_time;
            this.order_id = order_id;
            this.aux_order_id = aux_order_id;
            this.price = price;
            this.size = size;
            this.executed_size = executed_size;
            this.count = count;
            this.trade_id = trade_id;
            this.trade_price = trade_price;
            this.trade_size = trade_size;
            this.exchange_code = exchange_code;
            this.side = side;
            this.scope = scope;
            this.mm_or_ss = mm_or_ss;
        }
    }

    //Note: copy code from CDataTypes.cs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTestTrade
    {
        /// <summary>
        /// Time of the last trade.
        /// </summary>
        internal long time;

        /// <summary>
        /// Sequence number of the last trade to distinguish trades that have the same time.
        /// </summary>
        internal int sequence;

        /// <summary>
        /// Microseconds and nanoseconds part of time of the last trade
        /// </summary>
        internal int time_nanos;

        /// <summary>
        /// Exchange code of the last trade
        /// </summary>
        internal char exchange_code;

        /// <summary>
        /// Price of the last trade
        /// </summary>
        internal double price;

        /// <summary>
        /// Size of the last trade as integer number (rounded toward zero)
        /// </summary>
        internal double size;

        /// <summary>
        /// Trend indicator – in which direction price is moving. The values are: Up (Tick = 1), Down (Tick = 2),
        /// and Undefined (Tick = 0).
        /// Should be used if direction is Undefined (Direction.Undefined = 0).
        ///
        /// This field is absent in NativeTradeETH
        /// </summary>
        internal int tick;

        /// <summary>
        /// Change of the last trade.
        /// Value equals price minus NativeSummary.PrevDayClosePrice
        /// </summary>
        internal double change;

        /// <summary>
        /// Identifier of the day that this `trade` or `trade_eth` represents. Identifier of the day is the number of
        /// days passed since January 1, 1970.
        /// </summary>
        internal int day_id;

        /// <summary>
        /// Total volume traded for a day
        /// </summary>
        internal double day_volume;

        /// <summary>
        /// Total turnover traded for a day
        /// </summary>
        internal double day_turnover;

        /// <summary>
        /// This field contains several individual flags encoded as an integer number the following way:
        ///
        /// |31...4|  3 |  2 |  1 |  0 |
        /// |------|----|----|----|----|
        /// |      |  Direction |||ETH |
        ///
        /// 1. Tick Direction (Direction)
        /// 2. ETH (extendedTradingHours) - flag that determines current trading session: extended or regular
        ///    (0 - regular trading hours, 1 - extended trading hours).
        /// </summary>
        internal int raw_flags;

        /// <summary>
        /// Tick direction of the last trade
        /// </summary>
        internal Direction direction;

        /// <summary>
        /// Last trade was in extended trading hours
        /// </summary>
        internal bool is_eth;

        /// <summary>
        /// Last trade scope
        ///
        /// Possible values: Scope.Composite (Trade\TradeETH events) , Scope.Regional (Trade&\TradeETH& events)
        /// </summary>
        internal Scope scope;


        internal DxTestTrade(long time, int sequence, int time_nanos, char exchange_code, double price, double size,
            int tick, double change, int day_id, double day_volume, double day_turnover, int raw_flags,
            Direction direction, bool is_eth, Scope scope)
        {
            this.time = time;
            this.sequence = sequence;
            this.time_nanos = time_nanos;
            this.exchange_code = exchange_code;
            this.price = price;
            this.size = size;
            this.tick = tick;
            this.change = change;
            this.day_id = day_id;
            this.day_volume = day_volume;
            this.day_turnover = day_turnover;
            this.raw_flags = raw_flags;
            this.direction = direction;
            this.is_eth = is_eth;
            this.scope = scope;
        }
    }

    //Note: copy code from CDataTypes.cs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTestCandle
    {
        internal EventFlag event_flags;
        internal long index;
        internal long time;
        internal int sequence;
        internal double count;
        internal double open;
        internal double high;
        internal double low;
        internal double close;
        internal double volume;
        internal double vwap;
        internal double bid_volume;
        internal double ask_volume;
        internal double open_interest;
        internal double imp_volatility;

        internal DxTestCandle(EventFlag event_flags, long index, long time, int sequence,
            double count,
            double open, double high, double low, double close, double volume,
            double vwap, double bid_volume, double ask_volume,
            double open_interest, double imp_volatility)
        {
            this.event_flags = event_flags;
            this.index = index;
            this.time = time;
            this.sequence = sequence;
            this.count = count;
            this.open = open;
            this.high = high;
            this.low = low;
            this.close = close;
            this.volume = volume;
            this.vwap = vwap;
            this.bid_volume = bid_volume;
            this.ask_volume = ask_volume;
            this.open_interest = open_interest;
            this.imp_volatility = imp_volatility;
        }
    }

    //Note: copy code from CDataTypes.cs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTestGreeks
    {
        internal EventFlag event_flags;
        internal long index;
        internal long time;
        internal double price;
        internal double volatility;
        internal double delta;
        internal double gamma;
        internal double theta;
        internal double rho;
        internal double vega;

        internal DxTestGreeks(EventFlag event_flags, long index, long time,
            double price, double volatility,
            double delta, double gamma, double theta, double rho, double vega)
        {
            this.event_flags = event_flags;
            this.index = index;
            this.time = time;
            this.price = price;
            this.volatility = volatility;
            this.delta = delta;
            this.gamma = gamma;
            this.theta = theta;
            this.rho = rho;
            this.vega = vega;
        }
    }

    //Note: copy code from CDataTypes.cs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTestSnapshotData
    {
        internal EventType event_type;
        internal IntPtr symbol;

        internal int records_count;
        internal IntPtr records;
    }
}