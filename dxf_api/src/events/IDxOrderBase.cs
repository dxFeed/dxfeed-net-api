#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.data;
using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    ///   Interface for common fields of Order and SpreadOrder events.
    ///   Order events represent a snapshot for a full available market depth for a symbol.
    ///   The collection of order events of a symbol represents the most recent information that is
    ///   available about orders on the market at any given moment of time.
    /// </summary>
    public interface IDxOrderBase : IDxMarketEvent, IndexedEvent<string>
    {
        /// <summary>
        ///  Returns date time of this order.
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        ///  Returns microseconds and nanoseconds time part of the last trade.
        /// </summary>
        int TimeNanoPart { get; }
        /// <summary>
        ///   Returns sequence number of this order to distinguish orders that have the same Time.
        ///   This sequence number does not have to be unique and does not need to be sequential.
        /// </summary>
        int Sequence { get; }
        /// <summary>
        ///   Returns price of this order.
        /// </summary>
        double Price { get; }
        /// <summary>
        ///   Returns size of this order.
        /// </summary>
        long Size { get; }
        /// <summary>
        ///   Returns number of individual orders in this aggregate order.
        /// </summary>
        int Count { get; }
        /// <summary>
        ///   Returns scope of this order.
        /// </summary>
        Scope Scope { get; }
        /// <summary>
        ///   Returns side of this order.
        /// </summary>
        Side Side { get; }
        /// <summary>
        ///   Returns exchange code of this order.
        /// </summary>
        char ExchangeCode { get; }
    }
}
