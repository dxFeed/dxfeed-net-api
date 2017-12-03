#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

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
            Eps = profile.eps;
            DivFreq = profile.div_freq;
            ExdDivAmount = profile.exd_div_amount;
            ExdDivDate = profile.exd_div_date;
            _52HighPrice = profile._52_high_price;
            _52LowPrice = profile._52_low_price;
            Shares = profile.shares;
            Description = new string((char*)profile.description.ToPointer());
            Flags = profile.flags;
            StatusReason = new string((char*)profile.status_reason.ToPointer());
            HaltStartTime = TimeConverter.ToUtcDateTime(profile.halt_start_time);
            HaltEndTime = TimeConverter.ToUtcDateTime(profile.halt_end_time);
            HighLimitPrice = profile.high_limit_price;
            LowLimitPrice = profile.low_limit_price;
        }

        internal NativeProfile(IDxProfile profile) : base(profile.EventSymbol)
        {
            Beta = profile.Beta;
            Eps = profile.Eps;
            DivFreq = profile.DivFreq;
            ExdDivAmount = profile.ExdDivAmount;
            ExdDivDate = profile.ExdDivDate;
            _52HighPrice = profile._52HighPrice;
            _52LowPrice = profile._52LowPrice;
            Shares = profile.Shares;
            Description = string.Copy(profile.Description);
            Flags = profile.Flags;
            StatusReason = string.Copy(profile.StatusReason);
            HaltStartTime = profile.HaltStartTime;
            HaltEndTime = profile.HaltEndTime;
            HighLimitPrice = profile.HighLimitPrice;
            LowLimitPrice = profile.LowLimitPrice;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Profile: {{{15}, Beta: {0}, Eps: {1}, DivFreq: {2}, " +
                "ExdDivAmount: {3}, ExdDivDate: {4}, 52HighPrice: {5}, 52LowPrice: {6}, Shares: {7}, " +
                "Description: '{8}', Flags: {9}, StatusReason: '{10}', HaltStartTime: {11}, HaltEndTime: {12}, " +
                "HighLimitPrice: {13}, LowLimitPrice: {14}}}",
                Beta, Eps, DivFreq, ExdDivAmount, ExdDivDate, _52HighPrice, _52LowPrice, Shares, Description,
                Flags, StatusReason, HaltStartTime, HaltEndTime, HighLimitPrice, LowLimitPrice, EventSymbol);
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeProfile(this);
        }
        #endregion

        #region Implementation of IDxProfile

        public double Beta
        {
            get; private set;
        }

        public double Eps
        {
            get; private set;
        }

        public long DivFreq
        {
            get; private set;
        }

        public double ExdDivAmount
        {
            get; private set;
        }

        public int ExdDivDate
        {
            get; private set;
        }

        public double _52HighPrice
        {
            get; private set;
        }

        public double _52LowPrice
        {
            get; private set;
        }

        public double Shares
        {
            get; private set;
        }

        public string Description
        {
            get; private set;
        }

        public long Flags
        {
            get; private set;
        }

        public string StatusReason
        {
            get; private set;
        }

        public DateTime HaltStartTime
        {
            get; private set;
        }

        public DateTime HaltEndTime
        {
            get; private set;
        }

        public double HighLimitPrice
        {
            get; private set;
        }

        public double LowLimitPrice
        {
            get; private set;
        }

        #endregion
    }
}
