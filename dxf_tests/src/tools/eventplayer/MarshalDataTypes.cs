using com.dxfeed.api.data;
using com.dxfeed.api.events;
using System;
using System.Runtime.InteropServices;

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

        internal DxTestOrder(EventFlag event_flags, long index,
            long time, int time_nanos, int sequence,
            double price, int size, int count,
            Scope scope, Side side, char exchange_code,
            IndexedEventSource source, IntPtr mm_or_ss)
        {
            this.event_flags = event_flags;
            this.index = index;
            this.time = time;
            this.time_nanos = time_nanos;
            this.sequence = sequence;
            this.price = price;
            this.size = size;
            this.count = count;
            this.scope = scope;
            this.side = side;
            this.exchange_code = exchange_code;
            fixed (char* pSource = this.source)
            {
                var length = Math.Min(4, source.Name.Length);
                Marshal.Copy(source.Name.ToCharArray(), 0, (IntPtr)pSource, length);
                pSource[length] = (char)0;
            }
            this.mm_or_ss = mm_or_ss;
        }
    }

    //Note: copy code from CDataTypes.cs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTestTrade
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


        internal DxTestTrade(long time, int sequence, int time_nanos,
                                char exchange_code,
                                double price, int size,
                                int tick, double change,
                                int raw_flags,
                                double day_volume, double day_turnover,
                                Direction direction, bool is_eth)
        {
            this.time = time;
            this.sequence = sequence;
            this.time_nanos = time_nanos;
            this.exchange_code = exchange_code;
            this.price = price;
            this.size = size;
            this.tick = tick;
            this.change = change;
            this.raw_flags = raw_flags;
            this.day_volume = day_volume;
            this.day_turnover = day_turnover;
            this.direction = direction;
            this.is_eth = is_eth;
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
        internal int open_interest;
        internal double imp_volatility;

        internal DxTestCandle(EventFlag event_flags, long index, long time, int sequence,
                                double count,
                                double open, double high, double low, double close, double volume,
                                double vwap, double bid_volume, double ask_volume,
                                int open_interest, double imp_volatility)
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
