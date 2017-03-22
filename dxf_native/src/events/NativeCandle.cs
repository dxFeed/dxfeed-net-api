#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.candle;
using com.dxfeed.api.events;
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

            TimeStamp = candle.time;
            Time = TimeConverter.ToUtcDateTime(TimeStamp);
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
            DateTime = TimeConverter.ToUtcDateTime(TimeStamp);
            Index = candle.index;
            ImpVolatility = candle.imp_volatility;
            OpenInterest = candle.open_interest;
        }

        internal NativeCandle(IDxCandle candle)
        {
            EventSymbol = CandleSymbol.ValueOf(candle.EventSymbol.ToString());
            EventFlags = candle.EventFlags;

            TimeStamp = candle.TimeStamp;
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
            DateTime = TimeConverter.ToUtcDateTime(TimeStamp);
            Index = candle.Index;
            ImpVolatility = candle.ImpVolatility;
            OpenInterest = candle.OpenInterest;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Candle: {{{11}, Time: {0:o}, " +
            "Sequence: {1}, Count: {2:0.00}, Open: {3:0.000000}, High: {4:0.000000}, " +
            "Low: {5:0.000000}, Close: {6:0.000000}, Volume: {7:0.0}, VWAP: {8:0.0}, " +
            "BidVolume: {9:0.0}, AskVolume: {10:0.0}, OpenInterest: {12}, " +
            "ImpVolatility: {13:0.0} }}",
                Time, Sequence, Count, Open, High, Low, Close, Volume, VWAP, BidVolume, 
                AskVolume, EventSymbol.ToString(), OpenInterest, ImpVolatility);
        }

        #region Implementation of ICloneable
        public object Clone()
        {
            return new NativeCandle(this);
        }
        #endregion

        #region Implementation of IDxCandle

        /// <summary>
        /// Returns timestamp of this event.
        /// The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        public long TimeStamp
        {
            get; private set;
        }

        /// <summary>
        /// Returns UTC date and time of this event.
        /// </summary>
        public DateTime Time
        {
            get; private set;
        }

        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same
        /// Time. This sequence number does not have to be unique and does not need to be 
        /// sequential.
        /// </summary>
        public int Sequence
        {
            get; private set;
        }

        /// <summary>
        /// Returns total number of original trade (or quote) events in this candle.
        /// </summary>
        public double Count
        {
            get; private set;
        }

        /// <summary>
        /// Returns the first (open) price of this candle.
        /// </summary>
        public double Open
        {
            get; private set;
        }

        /// <summary>
        /// Returns the maximal (high) price of this candle.
        /// </summary>
        public double High
        {
            get; private set;
        }

        /// <summary>
        /// Returns the minimal (low) price of this candle.
        /// </summary>
        public double Low
        {
            get; private set;
        }

        /// <summary>
        /// Returns the last (close) price of this candle.
        /// </summary>
        public double Close
        {
            get; private set;
        }

        /// <summary>
        /// Returns total volume in this candle.
        /// </summary>
        public double Volume
        {
            get; private set;
        }

        /// <summary>
        /// Returns volume-weighted average price (VWAP) in this candle.
        /// </summary>
        public double VWAP
        {
            get; private set;
        }

        /// <summary>
        /// Returns bid volume in this candle.
        /// </summary>
        public double BidVolume
        {
            get; private set;
        }

        /// <summary>
        /// Returns ask volume in this candle.
        /// </summary>
        public double AskVolume
        {
            get; private set;
        }

        /// <summary>
        /// Returns date time of the candle.
        /// </summary>
        [Obsolete("DateTime is deprecated, please use Time instead.")]
        public DateTime DateTime
        {
            get; private set;
        }

        /// <summary>
        /// Returns unique per-symbol index of this candle event.
        /// Candle index is composed of Time and Sequence.
        /// </summary>
        public long Index
        {
            get; private set;
        }

        /// <summary>
        /// Returns candle event symbol.
        /// </summary>
        public CandleSymbol EventSymbol
        {
            get; private set;
        }

        /// <summary>
        /// Returns implied volatility.
        /// </summary>
        public double ImpVolatility
        {
            get; private set;
        }

        /// <summary>
        /// Returns open interest.
        /// </summary>
        public long OpenInterest
        {
            get; private set;
        }

        /// <summary>
        ///     Gets transactional event flags.
        ///     See "Event Flags" section from <see cref="IndexedEvent"/>.
        /// </summary>
        public EventFlag EventFlags
        {
            get; set;
        }

        #endregion
    }
}
