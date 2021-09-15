#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.data;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;
using System;
using System.Globalization;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Candle event with open, high, low, close prices and other information
    /// for a specific period.Candles are build with a specified CandlePeriod using
    /// a specified CandlePrice type with a data taken from the specified CandleExchange
    /// from the specified CandleSession with further details of aggregation provided by
    /// CandleAlignment.
    /// </summary>
    public class NativeCandle : IDxCandle
    {
        internal unsafe NativeCandle(DxCandle* c, string symbol)
        {
            DxCandle candle = *c;

            EventSymbol = CandleSymbol.ValueOf(symbol);

            EventFlags = candle.event_flags;
            Index = candle.index;
            Time = TimeConverter.ToUtcDateTime(candle.time);
            Sequence = candle.sequence;
            Count = candle.count;
            Open = candle.open;
            High = candle.high;
            Low = candle.low;
            Close = candle.close;
            Volume = candle.volume;
            VWAP = candle.vwap;
            BidVolume = candle.bid_volume;
            AskVolume = candle.ask_volume;
            OpenInterest = candle.open_interest;
            ImpVolatility = candle.imp_volatility;
        }

        internal NativeCandle(IDxCandle candle)
        {
            EventSymbol = CandleSymbol.ValueOf(candle.EventSymbol.ToString());

            EventFlags = candle.EventFlags;
            Index = candle.Index;
            Time = TimeConverter.ToUtcDateTime(TimeStamp);
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
            OpenInterest = candle.OpenInterest;
            ImpVolatility = candle.ImpVolatility;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Candle: {{{0}, " +
                "EventFlags: 0x{1:x2}, Index: {2:x16}, " +
                "Time: {3:o}, Sequence: {4}, Count: {5}, " +
                "Open: {6}, High: {7}, Low: {8}, Close: {9}, " +
                "Volume: {10}, VWAP: {11},  " +
                "BidVolume: {12}, AskVolume: {13}, " +
                "OpenInterest: {14}, ImpVolatility: {15} " +
                "}}",
                EventSymbol.ToString(),
                (int)EventFlags, Index,
                Time, Sequence, Count,
                Open, High, Low, Close,
                Volume, VWAP,
                BidVolume, AskVolume,
                OpenInterest, ImpVolatility
            );
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public object Clone()
        {
            return new NativeCandle(this);
        }

        #endregion

        #region Implementation of IDxCandle

        /// <summary>
        ///     Returns event symbol that identifies this event type.
        /// </summary>
        public CandleSymbol EventSymbol { get; private set; }

        /// <summary>
        ///     Returns source of this event.
        /// </summary>
        /// <returns>Source of this event.</returns>
        public IndexedEventSource Source
        {
            get { return IndexedEventSource.DEFAULT; }
        }

        /// <summary>
        ///    Gets or sets transactional event flags.
        ///    See "Event Flags" section from <see cref="IDxIndexedEvent"/>.
        /// </summary>
        public EventFlag EventFlags { get; set; }

        /// <summary>
        ///     Gets unique per-symbol index of this event.
        /// </summary>
        public long Index { get; private set; }

        /// <summary>
        /// Returns timestamp of this event.
        /// The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        public long TimeStamp
        {
            get { return TimeConverter.ToUnixTime(Time); }
        }

        /// <summary>
        /// Returns UTC date and time of this event.
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same
        /// Time. This sequence number does not have to be unique and does not need to be
        /// sequential.
        /// </summary>
        public int Sequence { get; private set; }

        /// <summary>
        /// Returns total number of original trade (or quote) events in this candle.
        /// </summary>
        public double Count { get; private set; }

        /// <summary>
        /// Returns the first (open) price of this candle.
        /// </summary>
        public double Open { get; private set; }

        /// <summary>
        /// Returns the maximal (high) price of this candle.
        /// </summary>
        public double High { get; private set; }

        /// <summary>
        /// Returns the minimal (low) price of this candle.
        /// </summary>
        public double Low { get; private set; }

        /// <summary>
        /// Returns the last (close) price of this candle.
        /// </summary>
        public double Close { get; private set; }

        /// <summary>
        /// Returns total volume in this candle.
        /// </summary>
        public double Volume { get; private set; }

        /// <summary>
        /// Returns volume-weighted average price (VWAP) in this candle.
        /// </summary>
        public double VWAP { get; private set; }

        /// <summary>
        /// Returns bid volume in this candle.
        /// </summary>
        public double BidVolume { get; private set; }

        /// <summary>
        /// Returns ask volume in this candle.
        /// </summary>
        public double AskVolume { get; private set; }

        /// <summary>
        /// Returns open interest.
        /// </summary>
        public double OpenInterest { get; private set; }

        /// <summary>
        /// Returns implied volatility.
        /// </summary>
        public double ImpVolatility { get; private set; }

        object IDxEventType.EventSymbol
        {
            get { return EventSymbol; }
        }

        #endregion
    }
}