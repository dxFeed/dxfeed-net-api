/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Type of a time and sale event.
    /// </summary>
    public enum TimeAndSaleType
    {
        /// <summary>
        /// Represents new time and sale event.
        /// </summary>
        New = 0,
        /// <summary>
        /// Represents correction time and sale event.
        /// </summary>
        Correction = 1,
        /// <summary>
        /// Represents cancel time and sale event.
        /// </summary>
        Cancel = 2
    }

    /// <summary>
    /// Time and Sale represents a trade (or other market event with price, e.g. market open/close 
    /// price, etc).
    /// Time and Sales are intended to provide information about trades in a continuous time slice
    /// (unlike Trade events which are supposed to provide snapshot about the current last trade).
    /// </summary>
    [EventTypeAttribute("TimeAndSale")]
    public interface IDxTimeAndSale : IDxMarketEvent
    {
        /// <summary>
        /// Returns aggressor side of this time and sale event.
        /// </summary>
        Side AgressorSide { get; }
        /// <summary>
        /// Returns the current ask price on the market when this time and sale event had occurred.
        /// </summary>
        double AskPrice { get; }
        /// <summary>
        /// Returns the current bid price on the market when this time and sale event had occurred.
        /// </summary>
        double BidPrice { get; }
        /// <summary>
        /// Returns unique per-symbol index of this time and sale event.
        /// Deprecated Use Index
        /// </summary>
        long EventId { get; }
        /// <summary>
        /// Returns event flags of this time and sale event.
        /// </summary>
        int EventFlags { get; }
        /// <summary>
        /// Returns exchange code of this time and sale event.
        /// </summary>
        char ExchangeCode { get; }
        /// <summary>
        /// Returns sale conditions provided for this event by data feed.
        /// </summary>
        DxString ExchangeSaleConditions { get; }
        /// <summary>
        /// Returns unique per-symbol index of this time and sale event.
        /// Time and sale index is composed of Time and Sequence.
        /// </summary>
        long Index { get; }
        /// <summary>
        /// Returns price of this time and sale event.
        /// </summary>
        double Price { get; }
        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same
        /// Time. This sequence number does not have to be unique and does not need to be 
        /// sequential.
        /// </summary>
        int Sequence { get; }
        /// <summary>
        /// Returns size of this time and sale event.
        /// </summary>
        long Size { get; }
        /// <summary>
        /// Returns date time of the original event.
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        /// Returns type of this time and sale event.
        /// </summary>
        TimeAndSaleType Type { get; }
        /// <summary>
        /// Returns whether this is a cancellation of a previous event.
        /// It is false for newly created time and sale event.
        /// </summary>
        bool IsCancel { get; }
        /// <summary>
        /// Returns whether this is a correction of a previous event.
        /// It is false for newly created time and sale event.
        /// </summary>
        bool IsCorrection { get; }
        /// <summary>
        /// Returns whether this event represents an extended trading hours sale.
        /// </summary>
        bool IsTrade { get; }
        /// <summary>
        /// Returns whether this is a new event (not cancellation or correction).
        /// It is true for newly created time and sale event.
        /// </summary>
        bool IsNew { get; }
        /// <summary>
        /// Returns whether this event represents a spread leg.
        /// </summary>
        bool IsSpreadLeg { get; }
        /// <summary>
        /// Returns whether this event represents a valid intraday tick.
        /// Note, that a correction for a previously distributed valid tick represents a new valid tick itself,
        /// but a cancellation of a previous valid tick does not.
        /// </summary>
        bool IsValidTick { get; }
    }
}