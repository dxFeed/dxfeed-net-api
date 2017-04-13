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

        /// <summary>
        ///     Creates Order events via all parameters.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="count"></param>
        /// <param name="eventFlags"></param>
        /// <param name="exchangeCode"></param>
        /// <param name="index"></param>
        /// <param name="level"></param>
        /// <param name="side"></param>
        /// <param name="price"></param>
        /// <param name="scope"></param>
        /// <param name="sequence"></param>
        /// <param name="size"></param>
        /// <param name="source"></param>
        /// <param name="time"></param>
        /// <param name="timeSequence"></param>
        /// <param name="marketMaker"></param>
        internal unsafe PlayedOrder(string symbol, int count, EventFlag eventFlags, 
            char exchangeCode, long index, int level, Side side, double price, Scope scope, 
            int sequence, long size, IndexedEventSource source, long time, long timeSequence, 
            string marketMaker)
        {
            EventSymbol = symbol;
            Count = count;
            EventFlags = eventFlags;
            ExchangeCode = exchangeCode;
            Index = index;
            Level = level;
            Side = side;
            Price = price;
            Scope = Scope.ValueOf(scope.Code);
            Sequence = sequence;
            Size = size;
            Time = Tools.UnixTimeToDate(time);
            Source = OrderSource.ValueOf(source.Name);
            TimeSequence = timeSequence;
            fixed (char* pMarketMaker = marketMaker.ToCharArray())
            {
                MarketMaker = new DxString(pMarketMaker);
            }

            marketMakerCharArray = marketMaker.ToCharArray();
            IntPtr marketMakerCharsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(marketMakerCharArray, 0);
            Params = new EventParams(EventFlags, (ulong)Index, 0);
            Data = new DxTestOrder(count, eventFlags, exchangeCode, index, level, side, price, scope, sequence, size, source, time, timeSequence, marketMakerCharsPtr);
        }

        /// <summary>
        ///     Creates Order event from another object.
        /// </summary>
        /// <param name="order">Other Order object.</param>
        internal PlayedOrder(IDxOrder order)
        {
            EventSymbol = order.EventSymbol;
            Count = order.Count;
            EventFlags = order.EventFlags;
            ExchangeCode = order.ExchangeCode;
            Index = order.Index;
            Level = order.Level;
            Side = order.Side;
            Price = order.Price;
            Scope = Scope.ValueOf(order.Scope.Code);
            Sequence = order.Sequence;
            Size = order.Size;
            Time = order.Time;
            Source = OrderSource.ValueOf(order.Source.Name);
            TimeSequence = order.TimeSequence;
            MarketMaker = (DxString)order.MarketMaker.Clone();

            marketMakerCharArray = MarketMaker.ToString().ToCharArray();
            IntPtr marketMakerCharsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(marketMakerCharArray, 0);
            Params = new EventParams(EventFlags, (ulong)Index, 0);
            Data = new DxTestOrder(Count, EventFlags, ExchangeCode, Index, Level, Side, Price, Scope, Sequence, Size, Source, Tools.DateToUnixTime(Time), TimeSequence, marketMakerCharsPtr);
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

        public int Level
        {
            get; private set;
        }

        public DxString MarketMaker
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

        public long TimeSequence
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
