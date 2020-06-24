#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native.api;
using System.Globalization;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Summary information snapshot about the trading session including session highs, lows, etc.
    /// It represents the most recent information that is available about the trading session
    /// in the market at any given moment of time.
    /// </summary>
    public class NativeSummary : MarketEventImpl, IDxSummary
    {
        internal unsafe NativeSummary(DxSummary* s, string symbol) : base(symbol)
        {
            DxSummary summary = *s;

            DayId = summary.day_id;
            DayOpenPrice = summary.day_open_price;
            DayHighPrice = summary.day_high_price;
            DayLowPrice = summary.day_low_price;
            DayClosePrice = summary.day_close_price;
            PrevDayId = summary.prev_day_id;
            PrevDayClosePrice = summary.prev_day_close_price;
            PrevDayVolume = summary.prev_day_volume;
            OpenInterest = summary.open_interest;
            RawFlags = summary.raw_flags;
            ExchangeCode = summary.exchange_code;
            DayClosePriceType = summary.day_close_price_type;
            PrevDayClosePriceType = summary.prev_day_close_price_type;
            Scope = summary.scope;
        }

        internal NativeSummary(IDxSummary summary) : base(summary.EventSymbol)
        {
            DayId = summary.DayId;
            DayOpenPrice = summary.DayOpenPrice;
            DayHighPrice = summary.DayHighPrice;
            DayLowPrice = summary.DayLowPrice;
            DayClosePrice = summary.DayClosePrice;
            PrevDayId = summary.PrevDayId;
            PrevDayClosePrice = summary.PrevDayClosePrice;
            PrevDayVolume = summary.PrevDayVolume;
            OpenInterest = summary.OpenInterest;
            RawFlags = summary.RawFlags;
            ExchangeCode = summary.ExchangeCode;
            DayClosePriceType = summary.DayClosePriceType;
            PrevDayClosePriceType = summary.PrevDayClosePriceType;
            Scope = summary.Scope;
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Summary: {{{0}, "                                                                                                 +
                "DayId: {1}, DayOpenPrice: {2}, DayHighPrice: {3}, DayLowPrice: {4}, DayClosePrice: {5}, DayClosePriceType: {6}, " +
                "PrevDayId: {7}, PrevDayClosePrice: {8}, PrevDayValoume: {9}, PrevDayClosePriceType {10}, "                        +
                "OpenInterest: {11}, ExchangeCode: {12}, "                                                                         +
                "RawFlags: {13:x8}, "                                                                                              +
                "Scope: {14}"                                                                                                      +
                "}}",
                EventSymbol,
                DayId, DayOpenPrice, DayHighPrice, DayLowPrice, DayClosePrice, DayClosePriceType,
                PrevDayId, PrevDayClosePrice, PrevDayVolume, PrevDayClosePriceType,
                OpenInterest, ExchangeCode,
                RawFlags, Scope
            );
        }

        #region Implementation of ICloneable

        public override object Clone()
        {
            return new NativeSummary(this);
        }

        #endregion

        #region Implementation of IDxSummary

        /// <summary>
        /// Returns identifier of the day that this summary represents.
        /// Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        public int DayId { get; private set; }
        /// <summary>
        /// Returns the first (open) price for the day.
        /// </summary>
        public double DayOpenPrice { get; private set; }
        /// <summary>
        /// Returns the maximal (high) price for the day.
        /// </summary>
        public double DayHighPrice { get; private set; }
        /// <summary>
        /// Returns the minimal (low) price for the day.
        /// </summary>
        public double DayLowPrice { get; private set; }
        /// <summary>
        /// Returns the last (close) price for the day.
        /// </summary>
        public double DayClosePrice { get; private set; }
        /// <summary>
        /// Returns the price type of the last (close) price for the day.
        /// </summary>
        public PriceType DayClosePriceType { get; private set; }
        /// <summary>
        /// Returns identifier of the previous day that this summary represents.
        /// Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        public int PrevDayId { get; private set; }
        /// <summary>
        /// Returns the last (close) price for the previous day.
        /// </summary>
        public double PrevDayClosePrice { get; private set; }
        /// <summary>
        /// Returns the price type of the last (close) price for the previous day.
        /// </summary>
        public PriceType PrevDayClosePriceType { get; private set; }
        /// <summary>
        /// Returns total volume traded for the previous day.
        /// </summary>
        public double PrevDayVolume { get; private set; }
        /// <summary>
        /// Returns open interest of the symbol as the number of open contracts.
        /// </summary>
        public long OpenInterest { get; private set; }
        /// <summary>
        /// Returns exchange code
        /// </summary>
        public char ExchangeCode { get; private set; }
        /// <summary>
        /// Returns implementation-specific raw bit flags value
        /// </summary>
        public int RawFlags { get; private set; }
        
        /// <summary>
        /// Returns whether summary was a composite or regional (other constants are not used here).
        /// </summary>
        public Scope Scope { get; private set; }

        #endregion
    }
}