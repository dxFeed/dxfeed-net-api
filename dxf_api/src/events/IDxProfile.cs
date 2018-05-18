/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.api.data;
using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Profile information snapshot that contains security instrument description.
    /// It represents the most recent information that is available about the traded security
    /// on the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("Profile")]
    public interface IDxProfile : IDxMarketEvent, IDxLastingEvent<string>
    {
        /// <summary>
        /// Returns Beta of the security instrument.
        /// </summary>
        double Beta { get; }
        /// <summary>
        /// Returns Earnings per Share of the security instrument.
        /// </summary>
        double EPS { get; }
        /// <summary>
        /// Returns Dividend Payment Frequency of the security instrument.
        /// </summary>
        long DivFreq { get; }
        /// <summary>
        /// Returns Latest paid dividends for the security instrument.
        /// </summary>
        double ExdDivAmount { get; }
        /// <summary>
        /// Returns Latest paid dividends day (day id) for the security instrument.
        /// </summary>
        int ExdDivDate { get; }
        /// <summary>
        /// Returns 52 Weeks high price of the security instrument.
        /// </summary>
        double _52HighPrice { get; }
        /// <summary>
        /// Returns 52 Weeks low price of the security instrument.
        /// </summary>
        double _52LowPrice { get; }
        /// <summary>
        /// Returns shares availiable of the security instrument.
        /// </summary>
        double Shares { get; }
        /// <summary>
        /// Returns free float of the security instrument.
        /// </summary>
        double FreeFloat { get; }
        /// <summary>
        /// Returns description of the security instrument.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Returns short sale restriction of the security instrument.
        /// </summary>
        ShortSaleRestriction ShortSaleRestriction { get; }
        /// <summary>
        /// Returns trading status of the security instrument.
        /// </summary>
        TradingStatus TradingStatus { get; }
        /// <summary>
        /// Returns description of the reason that trading was halted.
        /// </summary>
        string StatusReason { get; }
        /// <summary>
        /// Returns starting time of the trading halt interval.
        /// </summary>
        DateTime HaltStartTime { get; }
        /// <summary>
        /// Returns ending time of the trading halt interval.
        /// </summary>
        DateTime HaltEndTime { get; }
        /// <summary>
        /// Returns the maximal (high) allowed price.
        /// </summary>
        double HighLimitPrice { get; }
        /// <summary>
        /// Returns the minimal (low) allowed price.
        /// </summary>
        double LowLimitPrice { get; }
        /// <summary>
        /// Returns implementation-specific raw bit flags value
        /// </summary>
        long RawFlags { get; }
    }
}