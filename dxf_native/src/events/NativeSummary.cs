#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.Globalization;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    ///     Summary information snapshot about the trading session including session highs, lows, etc.
    ///     It represents the most recent information that is available about the trading session
    ///     in the market at any given moment of time.
    /// </summary>
    public class NativeSummary : MarketEventImpl, IDxSummary
    {
        internal unsafe NativeSummary(DxSummary* s, string symbol) : base(symbol)
        {
            var summary = *s;

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

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="summary">The original Summary event</param>
        public NativeSummary(IDxSummary summary) : base(summary.EventSymbol)
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

        /// <summary>
        ///     Constructs the copy and replaces the symbol
        /// </summary>
        /// <param name="summary">The original Summary event</param>
        /// <param name="symbol">The new symbol</param>
        public NativeSummary(IDxSummary summary, string symbol) : base(symbol)
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

        /// <summary>
        ///     Default constructor
        /// </summary>
        public NativeSummary()
        {
        }

        /// <summary>
        ///     Returns a normalized event. Normalization occurs for events in which Scope = Composite, and the symbol ends
        ///     with &amp; and the exchange code (that is, satisfies the regex: "&amp;[A-Z]")
        /// </summary>
        /// <returns>Normalized event or current event if normalization has not been performed.</returns>
        public IDxSummary Normalized()
        {
            if (Scope == Scope.Regional) return this;

            var exchangeCodeSeparatorPos = EventSymbol.LastIndexOf('&');

            if (exchangeCodeSeparatorPos < 0 || exchangeCodeSeparatorPos != EventSymbol.Length - 2) return this;

            var exchangeCode = EventSymbol[exchangeCodeSeparatorPos + 1];

            if (exchangeCode < 'A' || exchangeCode > 'Z') return this;

            return new NativeSummary(this, EventSymbol.Substring(0, exchangeCodeSeparatorPos))
            {
                Scope = Scope.Regional,
                ExchangeCode = exchangeCode
            };
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeSummary(this);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Summary: {{{0}, " +
                "DayId: {1}, DayOpenPrice: {2}, DayHighPrice: {3}, DayLowPrice: {4}, DayClosePrice: {5}, DayClosePriceType: {6}, " +
                "PrevDayId: {7}, PrevDayClosePrice: {8}, PrevDayVolume: {9}, PrevDayClosePriceType {10}, " +
                "OpenInterest: {11}, ExchangeCode: {12}, " +
                "RawFlags: {13:x8}, " +
                "Scope: {14}" +
                "}}",
                EventSymbol,
                DayId, DayOpenPrice, DayHighPrice, DayLowPrice, DayClosePrice, DayClosePriceType,
                PrevDayId, PrevDayClosePrice, PrevDayVolume, PrevDayClosePriceType,
                OpenInterest, ExchangeCode,
                RawFlags, Scope
            );
        }

        #region Implementation of IDxSummary

        /// <summary>
        ///     Returns identifier of the day that this summary represents.
        ///     Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        public int DayId { get; set; }

        /// <summary>
        ///     Returns the first (open) price for the day.
        /// </summary>
        public double DayOpenPrice { get; set; }

        /// <summary>
        ///     Returns the maximal (high) price for the day.
        /// </summary>
        public double DayHighPrice { get; set; }

        /// <summary>
        ///     Returns the minimal (low) price for the day.
        /// </summary>
        public double DayLowPrice { get; set; }

        /// <summary>
        ///     Returns the last (close) price for the day.
        /// </summary>
        public double DayClosePrice { get; set; }

        /// <summary>
        ///     Returns the price type of the last (close) price for the day.
        /// </summary>
        public PriceType DayClosePriceType { get; set; }

        /// <summary>
        ///     Returns identifier of the previous day that this summary represents.
        ///     Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        public int PrevDayId { get; set; }

        /// <summary>
        ///     Returns the last (close) price for the previous day.
        /// </summary>
        public double PrevDayClosePrice { get; set; }

        /// <summary>
        ///     Returns the price type of the last (close) price for the previous day.
        /// </summary>
        public PriceType PrevDayClosePriceType { get; set; }

        /// <summary>
        ///     Returns total volume traded for the previous day.
        /// </summary>
        public double PrevDayVolume { get; set; }

        /// <summary>
        ///     Returns open interest of the symbol as the number of open contracts.
        /// </summary>
        public double OpenInterest { get; set; }

        /// <summary>
        ///     Returns exchange code
        /// </summary>
        public char ExchangeCode { get; set; }

        /// <summary>
        ///     Returns implementation-specific raw bit flags value
        /// </summary>
        public int RawFlags { get; set; }

        /// <summary>
        ///     Returns whether summary was a composite or regional (other constants are not used here).
        /// </summary>
        public Scope Scope { get; set; }

        #endregion
    }
}