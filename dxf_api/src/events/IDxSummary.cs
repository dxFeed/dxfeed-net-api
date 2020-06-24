#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Summary information snapshot about the trading session including session highs, lows, etc.
    /// It represents the most recent information that is available about the trading session
    /// in the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("Summary")]
    public interface IDxSummary : IDxMarketEvent, IDxLastingEvent<string>
    {
        /// <summary>
        /// Returns identifier of the day that this summary represents.
        /// Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        int DayId { get; }
        /// <summary>
        /// Returns the first (open) price for the day.
        /// </summary>
        double DayOpenPrice { get; }
        /// <summary>
        /// Returns the maximal (high) price for the day.
        /// </summary>
        double DayHighPrice { get; }
        /// <summary>
        /// Returns the minimal (low) price for the day.
        /// </summary>
        double DayLowPrice { get; }
        /// <summary>
        /// Returns the last (close) price for the day.
        /// </summary>
        double DayClosePrice { get; }
        /// <summary>
        /// Returns the price type of the last (close) price for the day.
        /// </summary>
        PriceType DayClosePriceType { get; }
        /// <summary>
        /// Returns identifier of the previous day that this summary represents.
        /// Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        int PrevDayId { get; }
        /// <summary>
        /// Returns the last (close) price for the previous day.
        /// </summary>
        double PrevDayClosePrice { get; }
        /// <summary>
        /// Returns the price type of the last (close) price for the previous day.
        /// </summary>
        PriceType PrevDayClosePriceType { get; }
        /// <summary>
        /// Returns total volume traded for the previous day.
        /// </summary>
        double PrevDayVolume { get; }
        /// <summary>
        /// Returns open interest of the symbol as the number of open contracts.
        /// </summary>
        long OpenInterest { get; }
        /// <summary>
        /// Returns exchange code
        /// </summary>
        char ExchangeCode { get; }
        /// <summary>
        /// Returns implementation-specific raw bit flags value
        /// </summary>
        int RawFlags { get; }
        
        /// <summary>
        /// Returns whether summary was a composite or regional (other constants are not used here).
        /// </summary>
        Scope Scope { get; }
    }
}
