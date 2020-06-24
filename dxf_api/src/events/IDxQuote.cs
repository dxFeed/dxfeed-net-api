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
    /// Quote event is a snapshot of the best bid and ask prices,
    /// and other fields that change with each quote.
    /// It represents the most recent information that is available about the best quote on
    /// the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("Quote")]
    public interface IDxQuote : IDxMarketEvent, IDxLastingEvent<string>
    {
        /// <summary>
        /// Returns time of the last bid or ask change.
        /// This method is the same as max(getBidTime(), getAskTime())
        /// Note, that unlike bid/ask times, that are transmitted over network in a second-precision, this
        /// time is transmitted up to a millisecond and even nano-second precision (see getTimeNanoPart())
        /// if it is enabled on server side.
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        /// Returns sequence number of this quote to distinguish quotes that have the same
        /// time. This sequence number does not have to be unique and
        /// does not need to be sequential.
        /// </summary>
        int Sequence { get; }
        /// <summary>
        /// Returns microseconds and nanoseconds part of time of the last bid or ask change.
        /// </summary>
        int TimeNanoPart { get; }
        /// <summary>
        /// Returns time of the last bid change.
        /// This time is always transmitted with seconds precision, so the result of this method
        /// is usually a multiple of 1000.
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
        /// This time is always transmitted with seconds precision, so the result of this method
        /// is usually a multiple of 1000.
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
        /// <summary>
        /// Returns scope
        /// </summary>
        Scope Scope { get; }
    }
}