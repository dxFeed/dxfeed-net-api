/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Quote event is a snapshot of the best bid and ask prices,
    /// and other fields that change with each quote.
    /// It represents the most recent information that is available about the best quote on
    /// the market at any given moment of time.
    /// </summary>
    public interface IDxQuote : IDxMarketEvent
    {
        /// <summary>
        /// Returns date time of the last bid change.
        /// </summary>
        DateTime BidTime { get; }
        /// <summary>
        /// Returns bid exchange code.
        /// </summary>
        char BidExchangeCode { get; }
        /// <summary>
        /// Returns bid price.
        /// </summary>
        double BidPrice { get; }
        /// <summary>
        /// Returns bid size.
        /// </summary>
        long BidSize { get; }
        /// <summary>
        /// Returns date time of the last ask change.
        /// </summary>
        DateTime AskTime { get; }
        /// <summary>
        /// Returns ask exchange code.
        /// </summary>
        char AskExchangeCode { get; }
        /// <summary>
        /// Returns ask price.
        /// </summary>
        double AskPrice { get; }
        /// <summary>
        /// Returns ask size.
        /// </summary>
        long AskSize { get; }
    }
}