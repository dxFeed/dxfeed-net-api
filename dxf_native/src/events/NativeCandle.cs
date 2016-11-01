/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Candle event with open, high, low, close prices and other information
    /// for a specific period.Candles are build with a specified CandlePeriod using
    /// a specified CandlePrice type with a data taken from the specified CandleExchange 
    /// from the specified CandleSession with further details of aggregation provided by 
    /// CandleAlignment.
    /// </summary>
    public struct NativeCandle : IDxCandle
    {
        private readonly DxCandle candle;
        private string symbolString;

        internal unsafe NativeCandle(DxCandle* c, string symbol)
        {
            candle = *c;
            symbolString = symbol;
            EventSymbol = CandleSymbol.ValueOf(symbolString);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Candle: {{{11}, DateTime: {0:o}, " +
            "Sequence: {1}, Count: {2:0.00}, Open: {3:0.000000}, High: {4:0.000000}, " +
            "Low: {5:0.000000}, Close: {6:0.000000}, Volume: {7:0.0}, VWAP: {8:0.0}, " +
            "BidVolume: {9:0.0}, AskVolume: {10:0.0} }}",
                DateTime, Sequence, Count, Open, High, Low, Close, Volume, VWAP, BidVolume, AskVolume, symbolString);
        }
        #region Implementation of IDxCandle

        /// <summary>
        /// Returns timestamp of the candle in milliseconds.
        /// </summary>
        public long Time
        {
            get
            {
                return candle.time;
            }
        }

        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same
        /// Time. This sequence number does not have to be unique and does not need to be 
        /// sequential. Sequence can range from 0 to MAX_SEQUENCE.
        /// </summary>
        public int Sequence
        {
            get
            {
                return candle.sequence;
            }
        }

        /// <summary>
        /// Returns total number of original trade (or quote) events in this candle.
        /// </summary>
        public double Count
        {
            get
            {
                return candle.count;
            }
        }

        /// <summary>
        /// Returns the first (open) price of this candle.
        /// </summary>
        public double Open
        {
            get
            {
                return candle.open;
            }
        }

        /// <summary>
        /// Returns the maximal (high) price of this candle.
        /// </summary>
        public double High
        {
            get
            {
                return candle.high;
            }
        }

        /// <summary>
        /// Returns the minimal (low) price of this candle.
        /// </summary>
        public double Low
        {
            get
            {
                return candle.low;
            }
        }

        /// <summary>
        /// Returns the last (close) price of this candle.
        /// </summary>
        public double Close
        {
            get
            {
                return candle.close;
            }
        }

        /// <summary>
        /// Returns total volume in this candle.
        /// </summary>
        public double Volume
        {
            get
            {
                return candle.volume;
            }
        }

        /// <summary>
        /// Returns volume-weighted average price (VWAP) in this candle.
        /// </summary>
        public double VWAP
        {
            get
            {
                return candle.vwap;
            }
        }

        /// <summary>
        /// Returns bid volume in this candle.
        /// </summary>
        public double BidVolume
        {
            get
            {
                return candle.bid_volume;
            }
        }

        /// <summary>
        /// Returns ask volume in this candle.
        /// </summary>
        public double AskVolume
        {
            get
            {
                return candle.ask_volume;
            }
        }

        /// <summary>
        /// Returns datetime of the candle.
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                dateTime = dateTime.AddMilliseconds(candle.time);
                return dateTime;
            }
        }

        /// <summary>
        /// Returns unique per-symbol index of this candle event.
        /// Candle index is composed of Time and Sequence.
        /// Changing either time or sequence changes event index.
        /// </summary>
        public long Index
        {
            get
            {
                return candle.index;
            }
        }

        /// <summary>
        /// Returns candle event symbol.
        /// </summary>
        public CandleSymbol EventSymbol
        {
            get; private set;
        }

        #endregion
    }
}
