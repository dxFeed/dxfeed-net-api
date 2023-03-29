#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

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
    public interface IDxOrderBase : IDxMarketEvent, IDxIndexedEvent<string>
    {
        /// <summary>
        ///  Returns date time of this order.
        /// </summary>
        DateTime Time { get; }
        
        /// <summary>
        ///   Returns sequence number of this order to distinguish orders that have the same Time.
        ///   This sequence number does not have to be unique and does not need to be sequential.
        /// </summary>
        int Sequence { get; }

        /// <summary>
        ///  Returns microseconds and nanoseconds time part of the last trade.
        /// </summary>
        int TimeNanoPart { get; }
        
        /// <summary>
        /// Returns order action if available, otherwise - <c>OrderAction.Undefined</c>.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        /// <seealso cref="com.dxfeed.api.data.OrderAction"/>
        OrderAction Action { get; }
        
        /// <summary>
        /// Returns time of the last <see cref="com.dxfeed.api.data.OrderAction">OrderAction</see> if available, otherwise - 0.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        /// <seealso cref="com.dxfeed.api.data.OrderAction"/>
        DateTime ActionTime { get; }
        
        /// <summary>
        /// Returns order ID if available, otherwise - 0. Some actions <c>OrderAction.Trade</c>, <c>OrderAction.Bust</c> have no
        /// order since they are not related to any order in Order book.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        /// <seealso cref="com.dxfeed.api.data.OrderAction"/>
        long OrderId { get; }
        
        /// <summary>
        /// Returns auxiliary order ID if available, otherwise - 0:
        /// - in <c>OrderAction.New</c> - ID of the order replaced by this new order
        /// - in <c>OrderAction.Delete</c> - ID of the order that replaces this deleted order
        /// - in <c>OrderAction.Partial</c> - ID of the aggressor order
        /// - in <c>OrderAction.Execute</c> - ID of the aggressor order
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        /// <seealso cref="com.dxfeed.api.data.OrderAction"/>
        long AuxOrderId { get; }
        
        /// <summary>
        ///   Returns price of this order.
        /// </summary>
        double Price { get; }

        /// <summary>
        ///   Returns size of this order.
        /// </summary>
        double Size { get; }

        /// <summary>
        /// Returns <c>true</c> if this order has some size (neither 0 nor NaN)
        /// </summary>
        /// <returns><c>true</c> if this order has some size (neither 0 nor NaN)</returns>
        bool HasSize();

        /// <summary>
        ///   Returns executed size of this order.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        double ExecutedSize { get; }

        /// <summary>
        ///   Returns number of individual orders in this aggregate order.
        /// </summary>
        double Count { get; }
        
        /// <summary>
        /// Returns trade (order execution) ID for events containing trade-related action if available, otherwise - 0.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        /// <seealso cref="com.dxfeed.api.data.OrderAction"/>
        long TradeId { get; }
        
        /// <summary>
        /// Returns trade price for events containing trade-related action.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        /// <seealso cref="com.dxfeed.api.data.OrderAction"/>
        double TradePrice { get; }
        
        /// <summary>
        /// Returns trade size for events containing trade-related action.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        /// <seealso cref="com.dxfeed.api.data.OrderAction"/>
        double TradeSize { get; }
        
        /// <summary>
        ///   Returns exchange code of this order.
        /// </summary>
        char ExchangeCode { get; }

        /// <summary>
        ///   Returns side of this order.
        /// </summary>
        Side Side { get; }

        /// <summary>
        ///   Returns scope of this order.
        /// </summary>
        Scope Scope { get; }
    }
}
