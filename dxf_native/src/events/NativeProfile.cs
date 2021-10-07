#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Globalization;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    ///     Profile information snapshot that contains security instrument description.
    ///     It represents the most recent information that is available about the traded security
    ///     on the market at any given moment of time.
    /// </summary>
    public class NativeProfile : MarketEventImpl, IDxProfile
    {
        internal unsafe NativeProfile(DxProfile* p, string symbol) : base(symbol)
        {
            var profile = *p;

            Beta = profile.beta;
            EPS = profile.eps;
            DivFreq = profile.div_freq;
            ExdDivAmount = profile.exd_div_amount;
            ExdDivDate = profile.exd_div_date;
            High52WeekPrice = profile.high_52_week_price;
            Low52WeekPrice = profile.low_52_week_price;
            Shares = profile.shares;
            FreeFloat = profile.free_float;
            HighLimitPrice = profile.high_limit_price;
            LowLimitPrice = profile.low_limit_price;
            HaltStartTime = TimeConverter.ToUtcDateTime(profile.halt_start_time);
            HaltEndTime = TimeConverter.ToUtcDateTime(profile.halt_end_time);
            Description = new string((char*)profile.description.ToPointer());
            StatusReason = new string((char*)profile.status_reason.ToPointer());
            TradingStatus = profile.trading_status;
            ShortSaleRestriction = profile.ssr;
            RawFlags = profile.raw_flags;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="profile">The original Profile event</param>
        public NativeProfile(IDxProfile profile) : base(profile.EventSymbol)
        {
            Beta = profile.Beta;
            EPS = profile.EPS;
            DivFreq = profile.DivFreq;
            ExdDivAmount = profile.ExdDivAmount;
            ExdDivDate = profile.ExdDivDate;
            High52WeekPrice = profile.High52WeekPrice;
            Low52WeekPrice = profile.Low52WeekPrice;
            Shares = profile.Shares;
            FreeFloat = profile.FreeFloat;
            HighLimitPrice = profile.HighLimitPrice;
            LowLimitPrice = profile.LowLimitPrice;
            HaltStartTime = profile.HaltStartTime;
            HaltEndTime = profile.HaltEndTime;
            Description = profile.Description;
            StatusReason = profile.StatusReason;
            TradingStatus = profile.TradingStatus;
            ShortSaleRestriction = profile.ShortSaleRestriction;
            RawFlags = profile.RawFlags;
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        public NativeProfile()
        {
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeProfile(this);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Profile: {{{0}, " +
                "Beta: {1}, EPS: {2}, " +
                "DivFreq: {3}, ExdDivAmount: {4}, ExdDivDate: {5}, " +
                "High52WeekPrice: {6}, Low52WeekPrice: {7}, " +
                "Shares: {8}, FreeFloat: {9}," +
                "HighLimitPrice: {10}, LowLimitPrice: {11}, " +
                "HaltStartTime: {12:o}, HaltEndTime: {13:o}, " +
                "Description: '{14}', " +
                "TradingStatus: {15}, StatusReason: '{16}', " +
                "SSR: {17}, " +
                "RawFlags: {18:x8}" +
                "}}",
                EventSymbol,
                Beta, EPS,
                DivFreq, ExdDivAmount, ExdDivDate,
                High52WeekPrice, Low52WeekPrice,
                Shares, FreeFloat,
                HighLimitPrice, LowLimitPrice,
                HaltStartTime, HaltEndTime,
                Description,
                TradingStatus, StatusReason,
                ShortSaleRestriction,
                RawFlags
            );
        }

        #region Implementation of IDxProfile

        /// <summary>
        ///     Returns Beta of the security instrument.
        /// </summary>
        public double Beta { get; set; }

        /// <summary>
        ///     Returns Earnings per Share of the security instrument.
        /// </summary>
        public double EPS { get; set; }

        /// <summary>
        ///     Returns Dividend Payment Frequency of the security instrument.
        /// </summary>
        public double DivFreq { get; set; }

        /// <summary>
        ///     Returns Latest paid dividends for the security instrument.
        /// </summary>
        public double ExdDivAmount { get; set; }

        /// <summary>
        ///     Returns Latest paid dividends day (day id) for the security instrument.
        /// </summary>
        public int ExdDivDate { get; set; }

        /// <summary>
        ///     Returns 52 Weeks high price of the security instrument.
        /// </summary>
        public double High52WeekPrice { get; set; }

        /// <summary>
        ///     Returns 52 Weeks low price of the security instrument.
        /// </summary>
        public double Low52WeekPrice { get; set; }

        /// <summary>
        ///     Returns shares available of the security instrument.
        /// </summary>
        public double Shares { get; set; }

        /// <summary>
        ///     Returns free float of the security instrument.
        /// </summary>
        public double FreeFloat { get; set; }

        /// <summary>
        ///     Returns description of the security instrument.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Returns short sale restriction of the security instrument.
        /// </summary>
        public ShortSaleRestriction ShortSaleRestriction { get; set; }

        /// <summary>
        ///     Returns trading status of the security instrument.
        /// </summary>
        public TradingStatus TradingStatus { get; set; }

        /// <summary>
        ///     Returns description of the reason that trading was halted.
        /// </summary>
        public string StatusReason { get; set; }

        /// <summary>
        ///     Returns starting time of the trading halt interval.
        /// </summary>
        public DateTime HaltStartTime { get; set; }

        /// <summary>
        ///     Returns ending time of the trading halt interval.
        /// </summary>
        public DateTime HaltEndTime { get; set; }

        /// <summary>
        ///     Returns the maximal (high) allowed price.
        /// </summary>
        public double HighLimitPrice { get; set; }

        /// <summary>
        ///     Returns the minimal (low) allowed price.
        /// </summary>
        public double LowLimitPrice { get; set; }

        /// <summary>
        ///     Returns implementation-specific raw bit flags value
        /// </summary>
        public long RawFlags { get; set; }

        #endregion
    }
}