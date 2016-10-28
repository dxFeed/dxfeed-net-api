/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;
using com.dxfeed.api.events;
using com.dxfeed.native.api;
using com.dxfeed.api.extras;

namespace com.dxfeed.native.events
{
    public struct NativeProfile : IDxProfile
    {
        private readonly DxProfile profile;
        private readonly string description;
        private readonly string statusReason;
        private readonly string symbol;

        internal unsafe NativeProfile(DxProfile* profile, string symbol)
        {
            this.profile = *profile;
            description = new string((char*)this.profile.description.ToPointer());
            statusReason = new string((char*)this.profile.status_reason.ToPointer());
            this.symbol = symbol;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Profile: {{{15}, Beta: {0}, Eps: {1}, DivFreq: {2}, " +
                "ExdDivAmount: {3}, ExdDivDate: {4}, 52HighPrice: {5}, 52LowPrice: {6}, Shares: {7}, " +
                "Description: '{8}', Flags: {9}, StatusReason: '{10}', HaltStartTime: {11}, HaltEndTime: {12}, " +
                "HighLimitPrice: {13}, LowLimitPrice: {14}}}",
                Beta, Eps, DivFreq, ExdDivAmount, ExdDivDate, _52HighPrice, _52LowPrice, Shares, Description,
                Flags, StatusReason, HaltStartTime, HaltEndTime, HighLimitPrice, LowLimitPrice, Symbol);
        }

        #region Implementation of IDxProfile

        public double Beta
        {
            get { return profile.beta; }
        }

        public double Eps
        {
            get { return profile.eps; }
        }

        public long DivFreq
        {
            get { return profile.div_freq; }
        }

        public double ExdDivAmount
        {
            get { return profile.exd_div_amount; }
        }

        public int ExdDivDate
        {
            get { return profile.exd_div_date; }
        }

        public double _52HighPrice
        {
            get { return profile._52_high_price; }
        }

        public double _52LowPrice
        {
            get { return profile._52_low_price; }
        }

        public double Shares
        {
            get { return profile.shares; }
        }

        public string Description
        {
            get { return description; }
        }

        public long Flags
        {
            get { return profile.flags; }
        }

        public string StatusReason
        {
            get { return statusReason; }
        }

        public DateTime HaltStartTime
        {
            get { return TimeConverter.ToUtcDateTime(profile.halt_start_time); }
        }

        public DateTime HaltEndTime
        {
            get { return TimeConverter.ToUtcDateTime(profile.halt_end_time); }
        }

        public double HighLimitPrice
        {
            get { return profile.high_limit_price; }
        }

        public double LowLimitPrice
        {
            get { return profile.low_limit_price; }
        }

        public string Symbol
        {
            get { return symbol; }
        }

        #endregion
    }
}