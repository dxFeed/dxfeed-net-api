/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Base class for common fields of Order and SpreadOrder events.
    /// Order events represent a snapshot for a full available market depth for a symbol.
    /// The collection of order events of a symbol represents the most recent information that is
    /// available about orders on the market at any given moment of time.
    /// </summary>
    public class NativeOrderBase : MarketEvent, IDxOrderBase
    {
        private readonly DxOrder order;
        private readonly OrderSource source;

        internal unsafe NativeOrderBase(DxOrder* order, string symbol) : base(symbol)
        {
            this.order = *order;

            fixed (char* charPtr = this.order.source)
            {
                source = OrderSource.ValueOf(new string(charPtr));
            }
        }

        internal unsafe NativeOrderBase(DxSpreadOrder* order, string symbol) : base(symbol)
        {
            this.order.count = order->count;
            this.order.event_flags = order->event_flags;
            this.order.exchange_code = order->exchange_code;
            this.order.index = order->index;
            this.order.level = order->level;
            this.order.side = order->side;
            this.order.price = order->price;
            this.order.scope = order->scope;
            this.order.sequence = order->sequence;
            this.order.size = order->size;
            source = OrderSource.ValueOf(new string(order->source));
            this.order.time = order->time;
            this.order.time_sequence = order->time_sequence;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{1}, {7}@{6}, " +
                "Index: {0:x4}, Level: {2}, Time: {3:o}, Sequence: {4}, ExchangeCode: '{5}', " +
                "Source: '{8}', EventFlags: 0x{9:X}, Scope: {10}, Count: {11}",
                Index, Side, Level, Time, Sequence, ExchangeCode, Price, Size, Source, EventFlags, 
                Scope, Count);
        }

        #region Implementation of IDxOrderBase

        /// <summary>
        /// Returns number of individual orders in this aggregate order.
        /// </summary>
        public long Count
        {
            get { return order.count; }
        }

        /// <summary>
        /// Returns event flags.
        /// </summary>
        public int EventFlags
        {
            get { return order.event_flags; }
        }

        /// <summary>
        /// Returns exchange code of this order.
        /// </summary>
        public char ExchangeCode
        {
            get { return order.exchange_code; }
        }

        /// <summary>
        /// Returns unique per-symbol index of this order. Index is non-negative.
        /// </summary>
        public long Index
        {
            get { return order.index; }
        }

        /// <summary>
        /// Returns detail level of this order.
        /// Deprecated use Scope instead.
        /// </summary>
        public int Level
        {
            get { return order.level; }
        }

        /// <summary>
        /// Returns side of this order.
        /// </summary>
        public Side Side
        {
            get { return order.side; }
        }

        /// <summary>
        /// Returns price of this order.
        /// </summary>
        public double Price
        {
            get { return order.price; }
        }

        /// <summary>
        /// Returns scope of this order.
        /// </summary>
        public Scope Scope
        {
            get { return Scope.ValueOf(order.scope); }
        }

        /// <summary>
        /// Returns sequence number of this order to distinguish orders that have the same Time.
        /// This sequence number does not have to be unique and does not need to be sequential.
        /// Sequence can range from 0 to MAX_SEQUENCE.
        /// </summary>
        public int Sequence
        {
            get { return order.sequence; }
        }

        /// <summary>
        /// Returns size of this order.
        /// </summary>
        public long Size
        {
            get { return order.size; }
        }

        /// <summary>
        /// Returns source of this event.
        /// </summary>
        public OrderSource Source
        {
            get { return source; }
        }

        /// <summary>
        /// Returns time of this order.
        /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
        /// </summary>
        public DateTime Time
        {
            get { return TimeConverter.ToUtcDateTime(order.time); }
        }

        /// <summary>
        /// Returns time and sequence of this order packaged into single long value.
        /// This method is intended for efficient order time priority comparison.
        /// </summary>
        public long TimeSequence
        {
            get { return order.time_sequence; }
        }

        #endregion
    }
}