#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Action enum for the Full Order Book (FOB) Orders. Action describes business meaning of the NativeOrder event:
    /// whether order was added or replaced, partially or fully executed, etc.
    /// </summary>
    public enum OrderAction
    {
        /// <summary>
        /// Default enum value for orders that do not support "Full Order Book" and for backward compatibility -
        /// action must be derived from other <see cref="com.dxfeed.api.events.IDxOrder">Order</see>|<see cref="com.dxfeed.api.events.IDxSpreadOrder">SpreadOrder</see> fields
        ///
        /// All Full Order Book related fields for this action will be empty.
        ///
        /// Integer value = 0
        /// </summary>
        Undefined = 0,
        
        /// <summary>
        /// New Order is added to Order Book.
        ///
        /// Full Order Book fields:
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.OrderId">IDxOrderBase.OrderId</see> - always present
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.AuxOrderId">IDxOrderBase.AuxOrderId</see> - ID of the order replaced by this new order - if available.
        /// - Trade fields will be empty
        ///
        /// Integer value = 1
        /// </summary>
        New = 1,
        
        /// <summary>
        /// Order is modified and price-time-priority is not maintained (i.e. order has re-entered Order Book).
        /// Order <see cref="com.dxfeed.api.events.IDxOrderBase.EventSymbol">Symbol</see> and <see cref="com.dxfeed.api.events.IDxOrderBase.Side">Side</see> will remain the same.
        ///
        /// Full Order Book fields:
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.OrderId">IDxOrderBase.OrderId</see> - always present
        /// - Trade fields will be empty
        ///
        /// Integer value = 2
        /// </summary>
        Replace = 2,
        
        /// <summary>
        /// Order is modified without changing its price-time-priority (usually due to partial cancel by user).
        /// Order's <see cref="com.dxfeed.api.events.IDxOrderBase.Size">Size</see> will contain new updated size.
        ///
        /// Full Order Book fields:
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.OrderId">IDxOrderBase.OrderId</see> - always present
        /// - Trade fields will be empty
        ///
        /// Integer value = 3
        /// </summary>
        Modify = 3,
        
        /// <summary>
        /// Order is fully canceled and removed from Order Book.
        /// Order's <see cref="com.dxfeed.api.events.IDxOrderBase.Size">Size</see> will be equal to 0.
        ///
        /// Full Order Book fields:
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.OrderId">IDxOrderBase.OrderId</see> - always present
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.AuxOrderId">IDxOrderBase.AuxOrderId</see> - ID of the new order replacing this order - if available.
        /// - Trade fields will be empty
        ///
        /// Integer value = 4
        /// </summary>
        Delete = 4,

        /// <summary>
        /// Size is changed (usually reduced) due to partial order execution.
        /// Order's <see cref="com.dxfeed.api.events.IDxOrderBase.Size">Size</see> will be updated to show current outstanding size.
        ///
        /// Full Order Book fields:
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.OrderId">IDxOrderBase.OrderId</see> - always present
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.AuxOrderId">IDxOrderBase.AuxOrderId</see> - aggressor order ID, if available
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.TradeId">IDxOrderBase.TradeId</see> - if available
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.TradeSize">IDxOrderBase.TradeSize</see> and <see cref="com.dxfeed.api.events.IDxOrderBase.TradePrice">IDxOrderBase.TradePrice</see>  - contain size and price of this execution
        ///
        /// Integer value = 5
        /// </summary>
        Partial = 5,
        
        /// <summary>
        /// Order is fully executed and removed from Order Book.
        /// Order's <see cref="com.dxfeed.api.events.IDxOrderBase.Size">Size</see> will be equals to 0.
        ///
        /// Full Order Book fields:
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.OrderId">IDxOrderBase.OrderId</see> - always present
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.AuxOrderId">IDxOrderBase.AuxOrderId</see> - aggressor order ID, if available
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.TradeId">IDxOrderBase.TradeId</see> - if available
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.TradeSize">IDxOrderBase.TradeSize</see> and <see cref="com.dxfeed.api.events.IDxOrderBase.TradePrice">IDxOrderBase.TradePrice</see> - contain size and price of this execution - always present
        ///
        /// Integer value = 6
        /// </summary>
        Execute = 6,
        
        /// <summary>
        /// Non-Book Trade - this Trade not refers to any entry in Order Book.
        /// Order's <see cref="com.dxfeed.api.events.IDxOrderBase.Size">Size</see> and <see cref="com.dxfeed.api.events.IDxOrderBase.Price">Price</see> will be equals to 0.
        ///
        /// Full Order Book fields:
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.OrderId">IDxOrderBase.OrderId</see> - always empty
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.TradeId">IDxOrderBase.TradeId</see>- if available
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.TradeSize">IDxOrderBase.TradeSize</see> and <see cref="com.dxfeed.api.events.IDxOrderBase.TradePrice">IDxOrderBase.TradePrice</see> - contain size and price of this trade - always present
        ///
        /// Integer value = 7
        /// </summary>
        Trade = 7,
        
        /// <summary>
        /// Prior Trade/Order Execution bust.
        /// Order's <see cref="com.dxfeed.api.events.IDxOrderBase.Size">Size</see> and <see cref="com.dxfeed.api.events.IDxOrderBase.Price">Price</see> will be equals to 0.
        ///
        /// Full Order Book fields:
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.OrderId">IDxOrderBase.OrderId</see> - always empty
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.TradeId">IDxOrderBase.TradeId</see> - always present
        /// - <see cref="com.dxfeed.api.events.IDxOrderBase.TradeSize">IDxOrderBase.TradeSize</see> and <see cref="com.dxfeed.api.events.IDxOrderBase.TradePrice">IDxOrderBase.TradePrice</see> - always empty
        ///
        /// Integer value = 8
        /// </summary>
        Bust = 8
    }
}