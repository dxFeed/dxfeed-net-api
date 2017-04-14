using com.dxfeed.api;
using com.dxfeed.api.events;
using System;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Trade event implementation for test event player.
    /// </summary>
    internal class PlayedTrade : IPlayedEvent<DxTestTrade>, IDxTrade
    {
        /// <summary>
        /// Creates Trade events via all parameters.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="time"></param>
        /// <param name="exchangeCode"></param>
        /// <param name="price"></param>
        /// <param name="size"></param>
        /// <param name="tick"></param>
        /// <param name="change"></param>
        /// <param name="dayVolume"></param>
        internal PlayedTrade(string symbol, long time, char exchangeCode, double price, long size, 
            long tick, double change, double dayVolume)
        {
            EventSymbol = symbol;
            Time = Tools.UnixTimeToDate(time);
            ExchangeCode = exchangeCode;
            Price = price;
            Size = size;
            Tick = tick;
            Change = change;
            DayVolume = dayVolume;

            Params = new EventParams(0, 0, 0);
            Data = new DxTestTrade(time, exchangeCode, price, size, tick, change, dayVolume);
        }

        /// <summary>
        ///     Creates Trade event from another object.
        /// </summary>
        /// <param name="trade">Other Trade object.</param>
        internal PlayedTrade(IDxTrade trade)
        {
            EventSymbol = trade.EventSymbol;
            Time = trade.Time;
            ExchangeCode = trade.ExchangeCode;
            Price = trade.Price;
            Size = trade.Size;
            Tick = trade.Tick;
            Change = trade.Change;
            DayVolume = trade.DayVolume;

            Params = new EventParams(0, 0, 0);
            Data = new DxTestTrade(Tools.DateToUnixTime(Time), ExchangeCode, Price, Size, Tick, Change, DayVolume);
        }

        public double Change
        {
            get; private set;
        }

        public DxTestTrade Data
        {
            get; private set;
        }

        public double DayVolume
        {
            get; private set;
        }

        public string EventSymbol
        {
            get; private set;
        }

        public char ExchangeCode
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

        public long Size
        {
            get; private set;
        }

        public long Tick
        {
            get; private set;
        }

        public DateTime Time
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
            return new PlayedTrade(this) as object;
        }
    }
}
