#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Runtime.InteropServices;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native.api
{
    public static class Constants
    {
        /// The length of record suffix including the terminating null character
        public const int RecordSuffixSize = 17; // 16 + 1
    } 
    
    /// <summary>
    /// Trade event is a snapshot of the price and size of the last trade during regular trading hours and an overall day
    /// volume and day turnover. It represents the most recent information that is available about the regular last trade on
    /// the market at any given moment of time.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTrade
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
        /// Possible values: Scope.Composite (Trade\TradeETH events) , Scope.Regional (Trade&amp;\TradeETH&amp; events)
        /// </summary>
        internal Scope scope;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxQuote
    {
        internal long time;
        internal int sequence;
        internal int time_nanos;
        internal long bid_time;
        internal char bid_exchange_code;
        internal double bid_price;
        internal double bid_size;
        internal long ask_time;
        internal char ask_exchange_code;
        internal double ask_price;
        internal double ask_size;
        internal Scope scope;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxSummary
    {
        internal int day_id;
        internal double day_open_price;
        internal double day_high_price;
        internal double day_low_price;
        internal double day_close_price;
        internal int prev_day_id;
        internal double prev_day_close_price;
        internal double prev_day_volume;
        internal double open_interest;
        internal int raw_flags;
        internal char exchange_code;
        internal PriceType day_close_price_type;
        internal PriceType prev_day_close_price_type;
        internal Scope scope;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxProfile
    {
        internal double beta;
        internal double eps;
        internal double div_freq;
        internal double exd_div_amount;
        internal int exd_div_date;
        internal double high_52_week_price;
        internal double low_52_week_price;
        internal double shares;
        internal double free_float;
        internal double high_limit_price;
        internal double low_limit_price;
        internal long halt_start_time;
        internal long halt_end_time;
        internal int raw_flags;
        internal IntPtr description; // String
        internal IntPtr status_reason; // String
        internal TradingStatus trading_status;
        internal ShortSaleRestriction ssr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct DxOrder
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
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTimeAndSale
    {
        internal EventFlag event_flags;
        internal long index;
        internal long time;
        internal char exchange_code;
        internal double price;
        internal double size;
        internal double bid_price;
        internal double ask_price;
        internal IntPtr exchange_sale_conditions; // String
        internal int raw_flags;
        internal IntPtr buyer; // String
        internal IntPtr seller; // String
        internal Side side;
        internal TimeAndSaleType type;
        internal bool is_valid_tick;
        internal bool is_eth_trade;
        internal char trade_through_exempt;
        internal bool is_spread_leg;
        internal Scope scope;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxCandle
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
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxGreeks
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
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTheoPrice
    {
        internal long time;
        internal double price;
        internal double underlying_price;
        internal double delta;
        internal double gamma;
        internal double dividend;
        internal double interest;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxUnderlying
    {
        internal double volatility;
        internal double front_volatility;
        internal double back_volatility;
        internal double call_volume;
        internal double put_volume;
        internal double option_volume;
        internal double put_call_ratio;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxSeries
    {
        internal EventFlag event_flags;
        internal long index;
        internal long time;
        internal int sequence;
        internal int expiration;
        internal double volatility;
        internal double call_volume;
        internal double put_volume;
        internal double option_volume;
        internal double put_call_ratio;
        internal double forward_price;
        internal double dividend;
        internal double interest;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct DxConfiguration
    {
        internal int version;
        internal IntPtr string_object; //string
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxEventParams
    {
        internal EventFlag flags;
        internal ulong time_int_field;
        internal ulong snapshot_key;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxSnapshotData
    {
        internal EventType event_type;
        internal IntPtr symbol;

        internal int records_count;
        internal IntPtr records;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxPriceLevelBook
    {
        internal IntPtr symbol;
        internal uint bids_count;
        internal IntPtr bids;
        internal uint asks_count;
        internal IntPtr asks;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxPriceLevel
    {
        internal double price;
        internal double size;
        internal long time;
    }
}