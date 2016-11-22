/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Summary information snapshot about the trading session including session highs, lows, etc.
    /// It represents the most recent information that is available about the trading session 
    /// in the market at any given moment of time. 
    /// </summary>
    public interface IDxSummary : IDxMarketEvent
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
        /// Returns identifier of the previous day that this summary represents.
        /// Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        int PrevDayId { get; }
        /// <summary>
        /// Returns the last (close) price for the previous day.
        /// </summary>
        double PrevDayClosePrice { get; }
        /// <summary>
        /// Returns open interest of the symbol as the number of open contracts.
        /// </summary>
        long OpenInterest { get; }
        /// <summary>
        /// Returns implementation-specific flags.
        /// 
        /// Flags field contains both event flags (left-shifted) and business flags (not shifted):
        ///   31..28   27   26   25   24   23...4    3    2    1    0
        /// +--------+----+----+----+----+--------+----+----+----+----+
        /// |  0..0  | SE | SB | RE | TX |  0..0  |  Close  |PrevClose|
        /// +--------+----+----+----+----+--------+----+----+----+----+
        ///  \------- event flags ------/ \----- business flags -----/
        /// </summary>
        long Flags { get; }
        /// <summary>
        /// Returns exchange code
        /// </summary>
        char ExchangeCode { get; }
        /// <summary>
        /// Returns the price type of the last (close) price for the day.
        /// </summary>
        PriceType DayClosePriceType { get; }
        /// <summary>
        /// Returns the price type of the last (close) price for the previous day.
        /// </summary>
        PriceType PrevDayClosePriceType { get; }
    }
}
