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
        internal int count;
        internal EventFlag event_flags;
        internal char exchange_code;
        internal long index;
        internal int level;
        internal Side side;
        internal double price;
        internal int scope;
        internal int sequence;
        internal long size;
        internal fixed char source[5]; //string
        internal long time;
        internal long time_sequence;
        internal IntPtr market_maker; //string

        internal DxTestOrder(int count, EventFlag event_flags, char exchange_code, long index, int level, Side side, double price, Scope scope, int sequence, long size, IndexedEventSource source, long time, long time_sequence, IntPtr market_maker)
        {
            this.count = count;
            this.event_flags = event_flags;
            this.exchange_code = exchange_code;
            this.index = index;
            this.level = level;
            this.side = side;
            this.price = price;
            this.scope = scope.Code;
            this.sequence = sequence;
            this.size = size;
            fixed (char* pSource = this.source)
            {
                //TODO: magic numbers
                var length = Math.Min(4, source.Name.Length);
                Marshal.Copy(source.Name.ToCharArray(), 0, (IntPtr)pSource, length);
                pSource[length] = (char)0;
            }
            this.time = time;
            this.time_sequence = time_sequence;
            this.market_maker = market_maker;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DxTestTrade
    {
        internal long time;
        internal char exchange_code;
        internal double price;
        internal long size;
        internal long tick;
        internal double change;
        internal double day_volume;

        internal DxTestTrade(long time, char exchange_code, double price, long size, long tick, double change, double day_volume)
        {
            this.time = time;
            this.exchange_code = exchange_code;
            this.price = price;
            this.size = size;
            this.tick = tick;
            this.change = change;
            this.day_volume = day_volume;
        }
    }
}
