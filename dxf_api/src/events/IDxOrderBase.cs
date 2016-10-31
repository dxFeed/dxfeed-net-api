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
    /// Interface for common fields of Order and SpreadOrder events.
    /// Order events represent a snapshot for a full available market depth for a symbol.
    /// The collection of order events of a symbol represents the most recent information that is
    /// available about orders on the market at any given moment of time.
    /// </summary>
    public interface IDxOrderBase : IDxMarketEvent
    {
        /// <summary>
        /// Returns number of individual orders in this aggregate order.
        /// </summary>
        long Count { get; }
        /// <summary>
        /// Returns event flags.
        /// </summary>
        int EventFlags { get; }
        /// <summary>
        /// Returns exchange code of this order.
        /// </summary>
        char ExchangeCode { get; }
        /// <summary>
        /// Returns unique per-symbol index of this order. Index is non-negative.
        /// </summary>
        long Index { get; }
        /// <summary>
        /// Returns detail level of this order.
        /// Deprecated use Scope instead.
        /// </summary>
        int Level { get; }
        /// <summary>
        /// Returns side of this order.
        /// </summary>
        Side Side { get; }
        /// <summary>
        /// Returns price of this order.
        /// </summary>
        double Price { get; }
        /// <summary>
        /// Returns scope of this order.
        /// </summary>
        Scope Scope { get; }
        /// <summary>
        /// Returns sequence number of this order to distinguish orders that have the same Time.
        /// This sequence number does not have to be unique and does not need to be sequential.
        /// Sequence can range from 0 to MAX_SEQUENCE.
        /// </summary>
        int Sequence { get; }
        /// <summary>
        /// Returns size of this order.
        /// </summary>
        long Size { get; }
        /// <summary>
        /// Returns source of this event.
        /// </summary>
        string Source { get; }
        /// <summary>
        /// Returns time of this order.
        /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        /// Returns time and sequence of this order packaged into single long value.
        /// This method is intended for efficient order time priority comparison.
        /// </summary>
        long TimeSequence { get; }
    }
}