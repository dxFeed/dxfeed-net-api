using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using System;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Candle event implementation for test event player.
    /// </summary>
    internal class PlayedCandle : IPlayedEvent<DxTestCandle>, IDxCandle
    {
        /// <summary>
        /// Creates Candle events via all parameters.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="time"></param>
        /// <param name="sequence"></param>
        /// <param name="count"></param>
        /// <param name="open"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="close"></param>
        /// <param name="volume"></param>
        /// <param name="vwap"></param>
        /// <param name="bidVolume"></param>
        /// <param name="askVolume"></param>
        /// <param name="index"></param>
        /// <param name="openInterest"></param>
        /// <param name="impVolatility"></param>
        /// <param name="eventFlags"></param>
        internal PlayedCandle(string symbol, long time, int sequence, double count, double open, 
            double high, double low, double close, double volume, double vwap, double bidVolume,
            double askVolume, long index, long openInterest, double impVolatility,
            EventFlag eventFlags)
        {
            EventSymbol = CandleSymbol.ValueOf(symbol);
            EventFlags = eventFlags;

            TimeStamp = time;
            Time = Tools.UnixTimeToDate(time);
            Sequence = sequence;
            Count = count;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            VWAP = vwap;
            BidVolume = bidVolume;
            AskVolume = askVolume;
            DateTime = Tools.UnixTimeToDate(time);
            Index = index;
            ImpVolatility = impVolatility;
            OpenInterest = openInterest;

            Params = new EventParams(EventFlags, ((ulong)time << 32) + (uint)sequence, 0);
            Data = new DxTestCandle(time, sequence, count, open, high, low, close, volume, vwap, bidVolume, askVolume, index, openInterest, impVolatility, eventFlags);
        }

        /// <summary>
        ///     Creates Candle event from another object.
        /// </summary>
        /// <param name="candle">Other Candle object.</param>
        internal PlayedCandle(IDxCandle candle)
        {
            EventSymbol = CandleSymbol.ValueOf(candle.EventSymbol.ToString());
            EventFlags = candle.EventFlags;

            TimeStamp = candle.TimeStamp;
            Time = Tools.UnixTimeToDate(TimeStamp);
            Sequence = candle.Sequence;
            Count = candle.Count;
            Open = candle.Open;
            High = candle.High;
            Low = candle.Low;
            Close = candle.Close;
            Volume = candle.Volume;
            VWAP = candle.VWAP;
            BidVolume = candle.BidVolume;
            AskVolume = candle.AskVolume;
            DateTime = Tools.UnixTimeToDate(TimeStamp);
            Index = candle.Index;
            ImpVolatility = candle.ImpVolatility;
            OpenInterest = candle.OpenInterest;

            Params = new EventParams(EventFlags, ((ulong)TimeStamp << 32) + (uint)Sequence, 0);
            Data = new DxTestCandle(TimeStamp, Sequence, Count, Open, High, Low, Close, Volume, VWAP, BidVolume, AskVolume, Index, OpenInterest, ImpVolatility, EventFlags);
        }

        public double AskVolume
        {
            get; private set;
        }

        public double BidVolume
        {
            get; private set;
        }

        public double Close
        {
            get; private set;
        }

        public double Count
        {
            get; private set;
        }

        public DxTestCandle Data
        {
            get; private set;
        }

        public DateTime DateTime
        {
            get; private set;
        }

        public EventFlag EventFlags
        {
            get; set;
        }

        public CandleSymbol EventSymbol
        {
            get; private set;
        }

        public double High
        {
            get; private set;
        }

        public double ImpVolatility
        {
            get; private set;
        }

        public long Index
        {
            get; private set;
        }

        public double Low
        {
            get; private set;
        }

        public double Open
        {
            get; private set;
        }

        public long OpenInterest
        {
            get; private set;
        }

        public EventParams Params
        {
            get; private set;
        }

        public int Sequence
        {
            get; private set;
        }

        public IndexedEventSource Source
        {
            get
            {
                return IndexedEventSource.DEFAULT;
            }
        }

        public DateTime Time
        {
            get; private set;
        }

        public long TimeStamp
        {
            get; private set;
        }

        public double Volume
        {
            get; private set;
        }

        public double VWAP
        {
            get; private set;
        }

        object IPlayedEvent.Data
        {
            get
            {
                return Data as object;
            }
        }

        object IDxEventType.EventSymbol
        {
            get
            {
                return EventSymbol as object;
            }
        }

        public object Clone()
        {
            return new PlayedCandle(this);
        }
    }
}
