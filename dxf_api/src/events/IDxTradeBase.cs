/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Interface for common fields of IDxTrade and IDxTradeETH events. Trade events represent 
    /// the most recent information that is available about the last trade on the market at any 
    /// given moment of time.
    ///
    /// IDxTrade event represents last trade information for regular trading hours(RTH) with an 
    /// official volumefor the whole trading day.
    ///
    /// IDxTradeETH event is defined only for symbols (typically stocks and ETFs) with a designated
    /// extended trading hours (ETH, pre market and post market trading sessions). It represents
    /// last trade price during ETH and accumulated volume during ETH.
    /// </summary>
    public interface IDxTradeBase : IDxMarketEvent, LastingEvent<string>
    {
        /// <summary>
        /// Get date time of the last trade.
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        /// Returns exchange code of the last trade.
        /// </summary>
        char ExchangeCode { get; }
        /// <summary>
        /// Returns price of the last trade.
        /// </summary>
        double Price { get; }
        /// <summary>
        /// Returns size of the last trade.
        /// </summary>
        long Size { get; }
        /// <summary>
        /// Returns tick of the last trade.
        /// </summary>
        long Tick { get; }
        /// <summary>
        /// Returns change value of the last trade.
        /// </summary>
        double Change { get; }
        /// <summary>
        /// Returns total volume traded for a day.
        /// </summary>
        double DayVolume { get; }
    }
}