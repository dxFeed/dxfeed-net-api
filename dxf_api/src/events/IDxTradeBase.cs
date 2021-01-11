#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.data;

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
    public interface IDxTradeBase : IDxMarketEvent, IDxLastingEvent<string>
    {
        /// <summary>
        /// Returns time of the last trade. This time has precision up to milliseconds.
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        /// Returns sequence number of the last trade to distinguish trades that have the same
        /// time. This sequence number does not have to be unique and
        /// does not need to be sequential.
        /// </summary>
        int Sequence { get; }
        /// <summary>
        /// Returns microseconds and nanoseconds time part of the last trade.
        /// </summary>
        int TimeNanoPart { get; }
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
        /// Returns price change of the last trade, if available.
        /// </summary>
        double Change { get; }
        /// <summary>
        /// Returns total volume traded for a day.
        /// </summary>
        double DayVolume { get; }
        /// <summary>
        /// Returns total turnover traded for a day.
        /// Day VWAP can be computed with getDayTurnover() / getDayVolume}().
        /// </summary>
        double DayTurnover { get; }
        /// <summary>
        /// Returns tick direction of the last trade.
        /// </summary>
        Direction TickDirection { get; }
        /// <summary>
        /// Returns whether last trade was in extended trading hours.
        /// </summary>
        bool IsExtendedTradingHours { get; }
        /// <summary>
        /// Returns implementation-specific raw bit flags value
        /// </summary>
        int RawFlags { get; }
        /// <summary>
        /// Returns whether last trade was a composite or regional (other constants are not used here).
        /// </summary>
        Scope Scope { get; }
    }
}