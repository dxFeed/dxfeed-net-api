#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;
using System;
using System.Globalization;

namespace com.dxfeed.native.events
{
    public class NativeProfile : MarketEventImpl, IDxProfile
    {
        internal unsafe NativeProfile(DxProfile* p, string symbol) : base(symbol)
        {
            DxProfile profile = *p;

            Beta = profile.beta;
            EPS = profile.eps;
            DivFreq = profile.div_freq;
            ExdDivAmount = profile.exd_div_amount;
            ExdDivDate = profile.exd_div_date;
            _52HighPrice = profile._52_high_price;
            _52LowPrice = profile._52_low_price;
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

        internal NativeProfile(IDxProfile profile) : base(profile.EventSymbol)
        {
            Beta = profile.Beta;
            EPS = profile.EPS;
            DivFreq = profile.DivFreq;
            ExdDivAmount = profile.ExdDivAmount;
            ExdDivDate = profile.ExdDivDate;
            _52HighPrice = profile._52HighPrice;
            _52LowPrice = profile._52LowPrice;
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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Profile: {{{0}, "                                   +
                "Beta: {1}, EPS: {2}, "                              +
                "DivFreq: {3}, ExdDivAmount: {4}, ExdDivDate: {5}, " +
                "52HighPrice: {6}, 52LowPrice: {7}, "                +
                "Shares: {8}, FreeFloat: {9},"                       +
                "HighLimitPrice: {10}, LowLimitPrice: {11}, "        +
                "HaltStartTime: {12:o}, HaltEndTime: {13:o}, "       +
                "Description: '{14}', "                              +
                "TradingStatus: {15}, StatusReason: '{16}', "        +
                "SSR: {17}, "                                        +
                "RawFlags: {18:x8}"                                  +
                "}}",
                EventSymbol,
                Beta, EPS,
                DivFreq, ExdDivAmount, ExdDivDate,
                _52HighPrice, _52LowPrice,
                Shares, FreeFloat,
                HighLimitPrice, LowLimitPrice,
                HaltStartTime, HaltEndTime,
                Description,
                TradingStatus, StatusReason,
                ShortSaleRestriction,
                RawFlags
            );
        }

        #region Implementation of ICloneable

        public override object Clone()
        {
            return new NativeProfile(this);
        }

        #endregion

        #region Implementation of IDxProfile

        /// <summary>
        /// Returns Beta of the security instrument.
        /// </summary>
        public double Beta { get; private set; }
        /// <summary>
        /// Returns Earnings per Share of the security instrument.
        /// </summary>
        public double EPS { get; private set; }
        /// <summary>
        /// Returns Dividend Payment Frequency of the security instrument.
        /// </summary>
        public long DivFreq { get; private set; }
        /// <summary>
        /// Returns Latest paid dividends for the security instrument.
        /// </summary>
        public double ExdDivAmount { get; private set; }
        /// <summary>
        /// Returns Latest paid dividends day (day id) for the security instrument.
        /// </summary>
        public int ExdDivDate { get; private set; }
        /// <summary>
        /// Returns 52 Weeks high price of the security instrument.
        /// </summary>
        public double _52HighPrice { get; private set; }
        /// <summary>
        /// Returns 52 Weeks low price of the security instrument.
        /// </summary>
        public double _52LowPrice { get; private set; }
        /// <summary>
        /// Returns shares availiable of the security instrument.
        /// </summary>
        public double Shares { get; private set; }
        /// <summary>
        /// Returns free float of the security instrument.
        /// </summary>
        public double FreeFloat { get; private set; }
        /// <summary>
        /// Returns description of the security instrument.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Returns short sale restriction of the security instrument.
        /// </summary>
        public ShortSaleRestriction ShortSaleRestriction { get; private set; }
        /// <summary>
        /// Returns trading status of the security instrument.
        /// </summary>
        public TradingStatus TradingStatus { get; private set; }
        /// <summary>
        /// Returns description of the reason that trading was halted.
        /// </summary>
        public string StatusReason { get; private set; }
        /// <summary>
        /// Returns starting time of the trading halt interval.
        /// </summary>
        public DateTime HaltStartTime { get; private set; }
        /// <summary>
        /// Returns ending time of the trading halt interval.
        /// </summary>
        public DateTime HaltEndTime { get; private set; }
        /// <summary>
        /// Returns the maximal (high) allowed price.
        /// </summary>
        public double HighLimitPrice { get; private set; }
        /// <summary>
        /// Returns the minimal (low) allowed price.
        /// </summary>
        public double LowLimitPrice { get; private set; }
        /// <summary>
        /// Returns implementation-specific raw bit flags value
        /// </summary>
        public long RawFlags { get; private set; }

        #endregion
    }
}
