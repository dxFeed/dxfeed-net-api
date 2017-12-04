#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.candle;
using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Candle event with open, high, low, close prices and other information
    /// for a specific period.Candles are build with a specified CandlePeriod using
    /// a specified CandlePrice type with a data taken from the specified CandleExchange 
    /// from the specified CandleSession with further details of aggregation provided by 
    /// CandleAlignment.
    /// </summary>
    [EventTypeAttribute("Candle")]
    public interface IDxCandle : TimeSeriesEvent<CandleSymbol>, LastingEvent<CandleSymbol>
    {
        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same
        /// Time. This sequence number does not have to be unique and does not need to be 
        /// sequential.
        /// </summary>
        int Sequence { get; }
        /// <summary>
        /// Returns total number of original trade (or quote) events in this candle.
        /// </summary>
        double Count { get; }
        /// <summary>
        /// Returns the first (open) price of this candle.
        /// </summary>
        double Open { get; }
        /// <summary>
        /// Returns the maximal (high) price of this candle.
        /// </summary>
        double High { get; }
        /// <summary>
        /// Returns the minimal (low) price of this candle.
        /// </summary>
        double Low { get; }
        /// <summary>
        /// Returns the last (close) price of this candle.
        /// </summary>
        double Close { get; }
        /// <summary>
        /// Returns total volume in this candle.
        /// </summary>
        double Volume { get; }
        /// <summary>
        /// Returns volume-weighted average price (VWAP) in this candle.
        /// </summary>
        double VWAP { get; }
        /// <summary>
        /// Returns bid volume in this candle.
        /// </summary>
        double BidVolume { get; }
        /// <summary>
        /// Returns ask volume in this candle.
        /// </summary>
        double AskVolume { get; }
        /// <summary>
        /// Returns date time of the candle.
        /// </summary>
        [Obsolete("DateTime is deprecated, please use Time instead.")]
        DateTime DateTime { get; }
        /// <summary>
        /// Returns open interest.
        /// </summary>
        long OpenInterest { get; }
        /// <summary>
        /// Returns implied volatility.
        /// </summary>
        double ImpVolatility { get; }
    }
}
