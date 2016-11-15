/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System.Globalization;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Summary information snapshot about the trading session including session highs, lows, etc.
    /// It represents the most recent information that is available about the trading session 
    /// in the market at any given moment of time. 
    /// </summary>
    public class NativeSummary : MarketEvent, IDxSummary
    {
        private readonly DxSummary summary;

        internal unsafe NativeSummary(DxSummary* summary, string symbol) : base(symbol)
        {
            this.summary = *summary;
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Summary: {{{10}, DayId: {0}, DayOpenPrice: {1}, DayHighPrice: {2}, DayLowPrice: {3}, " +
                "DayClosePrice: {4}, PrevDayId: {5}, PrevDayClosePrice: {6}, OpenInterest: {7}, " +
                "Flags: {8}, ExchangeCode: {9}, DayClosePriceType: {10}, PrevDayClosePriceType {11} }}",
                DayId, DayOpenPrice, DayHighPrice, DayLowPrice,
                DayClosePrice, PrevDayId, PrevDayClosePrice, OpenInterest,
                Flags, ExchangeCode, EventSymbol, DayClosePriceType, PrevDayClosePriceType);
        }

        #region Implementation of IDxSummary

        /// <summary>
        /// Returns identifier of the day that this summary represents.
        /// Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        public int DayId
        {
            get { return summary.day_id; }
        }

        /// <summary>
        /// Returns the first (open) price for the day.
        /// </summary>
        public double DayOpenPrice
        {
            get { return summary.day_open_price; }
        }

        /// <summary>
        /// Returns the maximal (high) price for the day.
        /// </summary>
        public double DayHighPrice
        {
            get { return summary.day_high_price; }
        }

        /// <summary>
        /// Returns the minimal (low) price for the day.
        /// </summary>
        public double DayLowPrice
        {
            get { return summary.day_low_price; }
        }

        /// <summary>
        /// Returns the last (close) price for the day.
        /// </summary>
        public double DayClosePrice
        {
            get { return summary.day_close_price; }
        }

        /// <summary>
        /// Returns identifier of the previous day that this summary represents.
        /// Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        public int PrevDayId
        {
            get { return summary.prev_day_id; }
        }

        /// <summary>
        /// Returns the last (close) price for the previous day.
        /// </summary>
        public double PrevDayClosePrice
        {
            get { return summary.prev_day_close_price; }
        }

        /// <summary>
        /// Returns open interest of the symbol as the number of open contracts.
        /// </summary>
        public long OpenInterest
        {
            get { return summary.open_interest; }
        }

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
        public long Flags
        {
            get { return summary.flags; }
        }

        /// <summary>
        /// Returns exchange code
        /// </summary>
        public char ExchangeCode
        {
            get { return summary.exchange_code; }
        }

        /// <summary>
        /// Returns the price type of the last (close) price for the day.
        /// </summary>
        public PriceType DayClosePriceType
        {
            get { return summary.day_close_price_type; }
        }

        /// <summary>
        /// Returns the price type of the last (close) price for the previous day.
        /// </summary>
        public PriceType PrevDayClosePriceType
        {
            get { return summary.prev_day_close_price_type; }
        }

        #endregion
    }
}