#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Order event is a snapshot for a full available market depth for a symbol.
    /// The collection of order events of a symbol represents the most recent information
    /// that is available about orders on the market at any given moment of time.
    /// Order events give information on several levels of details, called scopes - see Scope.
    /// Scope of an order is available via Scope property.
    /// </summary>
    [EventTypeAttribute("Order")]
    public interface IDxOrder : IDxOrderBase
    {
        /// <summary>
        /// Returns market maker or other aggregate identifier of this order.
        /// This value is defined for Scope.AGGREGATE and Scope.ORDER orders.
        /// </summary>
        string MarketMaker { get; }
    }
}