#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System;
using System.Runtime.InteropServices;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native.api
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTrade
    {
        internal long time;
        internal int sequence;
        internal int time_nanos;
        internal char exchange_code;
        internal double price;
        internal int size;
        /* This field is absent in TradeETH */
        internal int tick;
        /* This field is absent in TradeETH */
        internal double change;
        internal int raw_flags;
        internal double day_volume;
        internal double day_turnover;
        internal Direction direction;
        internal bool is_eth;
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
        internal int bid_size;
        internal long ask_time;
        internal char ask_exchange_code;
        internal double ask_price;
        internal int ask_size;
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
        internal int open_interest;
        internal int raw_flags;
        internal char exchange_code;
        internal PriceType day_close_price_type;
        internal PriceType prev_day_close_price_type;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxProfile
    {
        internal double beta;
        internal double eps;
        internal int div_freq;
        internal double exd_div_amount;
        internal int exd_div_date;
        internal double _52_high_price;
        internal double _52_low_price;
        internal double shares;
        internal double free_float;
        internal double high_limit_price;
        internal double low_limit_price;
        internal long halt_start_time;
        internal long halt_end_time;
        internal int raw_flags;
        internal IntPtr description;    // String
        internal IntPtr status_reason; // String
        internal TradingStatus trading_status;
        internal ShortSaleRestriction ssr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct DxOrder
    {
        internal EventFlag event_flags;
        internal long index;
        internal long time;
        internal int time_nanos;
        internal int sequence;
        internal double price;
        internal int size;
        internal int count;
        internal Scope scope;
        internal Side side;
        internal char exchange_code;
        internal fixed char source[5];
        internal IntPtr mm_or_ss; // String
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTimeAndSale
    {
        internal EventFlag event_flags;
        internal long index;
        internal long time;
        internal char exchange_code;
        internal double price;
        internal int size;
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
        internal int open_interest;
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
        internal double put_call_ratio;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxSeries
    {
        internal EventFlag event_flags;
        internal long index;
        internal int expiration;
        internal double volatility;
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
    internal struct DxPriceLevel {
        internal double price;
        internal long size;
        internal long time;
    }
}
