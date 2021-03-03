#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;
using System;
using System.Globalization;

namespace com.dxfeed.native.events
{
    /// <summary>
    ///   Base class for common fields of Order and SpreadOrder events.
    ///   Order events represent a snapshot for a full available market depth for a symbol.
    ///   The collection of order events of a symbol represents the most recent information that is
    ///   available about orders on the market at any given moment of time.
    /// </summary>
    public class NativeOrderBase : MarketEventImpl, IDxOrderBase
    {
        internal unsafe NativeOrderBase(DxOrder* o, string symbol) : base(symbol)
        {
            DxOrder order = *o;

            Source = OrderSource.ValueOf(new string(order.source));
            EventFlags = order.event_flags;
            Index = order.index;
            Time = TimeConverter.ToUtcDateTime(order.time);
            Sequence = order.sequence;
            TimeNanoPart = order.time_nanos;
            Action = order.action;
            ActionTime = TimeConverter.ToUtcDateTime(order.action_time);
            OrderId = order.order_id;
            AuxOrderId = order.aux_order_id;
            Price = order.price;
            Size = order.size;
            Count = order.count;
            TradeId = order.trade_id;
            TradePrice = order.trade_price;
            TradeSize = order.trade_size;
            ExchangeCode = order.exchange_code;
            Side = order.side;
            Scope = order.scope;
        }

        internal NativeOrderBase(IDxOrderBase order) : base(order.EventSymbol)
        {
            Source = order.Source;
            EventFlags = order.EventFlags;
            Index = order.Index;
            Time = order.Time;
            Sequence = order.Sequence;
            TimeNanoPart = order.TimeNanoPart;
            Action = order.Action;
            ActionTime = order.ActionTime;
            OrderId = order.OrderId;
            AuxOrderId = order.AuxOrderId;
            Price = order.Price;
            Size = order.Size;
            Count = order.Count;
            TradeId = order.TradeId;
            TradePrice = order.TradePrice;
            TradeSize = order.TradeSize;
            ExchangeCode = order.ExchangeCode;
            Side = order.Side;
            Scope = order.Scope;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Source: {0}, EventFlags: 0x{1:x2}, Index: {2:x16}, Time: {3:o}, Sequence: {4}, TimeNanoPart: {5}, " +
                "Action: {6}, ActionTime: {7:o}, OrderId: {8}, AuxOrderId: {9}, Price: {10}, Size: {11}, Count: {12}, " +
                "ExchangeCode: {13}, Side: {14}, Scope: {15}, TradeId: {16}, TradePrice: {17}, TradeSize: {18}",
                Source, (int) EventFlags, Index, Time, Sequence, TimeNanoPart,
                Action, ActionTime, OrderId, AuxOrderId, Price, Size, Count,
                ExchangeCode, Side, Scope, TradeId, TradePrice, TradeSize
            );
        }

        #region Implementation of ICloneable

        public override object Clone()
        {
            return new NativeOrderBase(this);
        }

        #endregion

        #region Implementation of IDxOrderBase

        /// <summary>
        /// Returns source of this event.
        /// </summary>
        public IndexedEventSource Source { get; internal set; }

        /// <summary>
        /// Gets or sets transactional event flags.
        /// See "Event Flags" section from <see cref="IDxIndexedEvent"/>.
        /// </summary>
        public EventFlag EventFlags { get; set; }

        /// <summary>
        /// Returns unique per-symbol index of this event.
        /// </summary>
        public long Index { get; internal set; }

        /// <summary>
        ///  Returns date time of this order.
        /// </summary>
        public DateTime Time { get; internal set; }
        
        /// <summary>
        /// Returns sequence number of this order to distinguish orders that have the same Time.
        /// This sequence number does not have to be unique and does not need to be sequential.
        /// </summary>
        public int Sequence { get; internal set; }

        /// <summary>
        /// Returns microseconds and nanoseconds time part of the last trade.
        /// </summary>
        public int TimeNanoPart { get; internal set; }
        
        /// <summary>
        /// Returns order action if available, otherwise - OrderAction.Undefined.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        public OrderAction Action { get; internal set; }
        
        /// <summary>
        /// Returns time of the last NativeOrder.Action if available, otherwise - 0.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        public DateTime ActionTime { get; internal set; }
        
        /// <summary>
        /// Returns order ID if available, otherwise - 0. Some actions OrderAction.Trade, OrderAction.Bust have no
        /// order since they are not related to any order in Order book.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        public long OrderId { get; internal set; }
        
        /// <summary>
        /// Returns auxiliary order ID if available, otherwise - 0:
        /// - in OrderAction.New - ID of the order replaced by this new order
        /// - in OrderAction.Delete - ID of the order that replaces this deleted order
        /// - in OrderAction.Partial - ID of the aggressor order
        /// - in OrderAction.Execute - ID of the aggressor order
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        public long AuxOrderId { get; internal set; }
        
        /// <summary>
        /// Returns price of this order.
        /// </summary>
        public double Price { get; internal set; }
        
        /// <summary>
        /// Returns size of this order.
        /// </summary>
        public long Size { get; internal set; }
        
        /// <summary>
        /// Returns number of individual orders in this aggregate order.
        /// </summary>
        public int Count { get; internal set; }
        
        /// <summary>
        /// Returns trade (order execution) ID for events containing trade-related action if available, otherwise - 0.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        public long TradeId { get; internal set; }
        
        /// <summary>
        /// Returns trade price for events containing trade-related action.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        public double TradePrice { get; internal set; }
        
        /// <summary>
        /// Returns trade size for events containing trade-related action.
        ///
        /// This field is a part of the FOB ("Full Order Book") support.
        /// </summary>
        public double TradeSize { get; internal set; }
        
        /// <summary>
        /// Returns exchange code of this order.
        /// </summary>
        public char ExchangeCode { get; internal set; }

        /// <summary>
        /// Returns side of this order.
        /// </summary>
        public Side Side { get; internal set; }

        /// <summary>
        /// Returns scope of this order.
        /// </summary>
        public Scope Scope { get; internal set; }

        #endregion
    }
}
