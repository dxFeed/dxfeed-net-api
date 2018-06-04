#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
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

            EventFlags = order.event_flags;
            Index = order.index;
            Time = TimeConverter.ToUtcDateTime(order.time);
            TimeNanoPart = order.time_nanos;
            Sequence = order.sequence;
            Price = order.price;
            Size = order.size;
            Count = order.count;
            Scope = order.scope;
            Side = order.side;
            ExchangeCode = order.exchange_code;
            Source = OrderSource.ValueOf(new string(order.source));
        }

        internal NativeOrderBase(IDxOrderBase order) : base(order.EventSymbol)
        {
            EventFlags = order.EventFlags;
            Index = order.Index;
            Time = order.Time;
            TimeNanoPart = order.TimeNanoPart;
            Sequence = order.Sequence;
            Price = order.Price;
            Size = order.Size;
            Count = order.Count;
            Scope = order.Scope;
            Side = order.Side;
            ExchangeCode = order.ExchangeCode;
            Source = order.Source;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Source: {0}, "                                   +
                "EventFlags: 0x{1:x2}, Index: {2:x16}, "          +
                "Time: {3:o}, TimeNanoPart: {4}, Sequence: {5}, " +
                "Price: {6}, Size: {7}, Count: {8}, "             +
                "Scope: {9}, Side: {10}, "                        +
                "ExchangeCode: {11}",
                Source,
                (int) EventFlags, Index,
                Time, TimeNanoPart, Sequence,
                Price, Size, Count,
                Scope, Side,
                ExchangeCode
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
        ///     Returns source of this event.
        /// </summary>
        public IndexedEventSource Source { get; internal set; }

        /// <summary>
        ///    Gets or sets transactional event flags.
        ///    See "Event Flags" section from <see cref="IDxIndexedEvent"/>.
        /// </summary>
        public EventFlag EventFlags { get; set; }

        /// <summary>
        ///     Gets unique per-symbol index of this event.
        /// </summary>
        public long Index { get; internal set; }

        /// <summary>
        ///  Returns date time of this order.
        /// </summary>
        public DateTime Time { get; internal set; }
        /// <summary>
        ///  Returns microseconds and nanoseconds time part of the last trade.
        /// </summary>
        public int TimeNanoPart { get; internal set; }
        /// <summary>
        ///   Returns sequence number of this order to distinguish orders that have the same Time.
        ///   This sequence number does not have to be unique and does not need to be sequential.
        /// </summary>
        public int Sequence { get; internal set; }
        /// <summary>
        ///   Returns price of this order.
        /// </summary>
        public double Price { get; internal set; }
        /// <summary>
        ///   Returns size of this order.
        /// </summary>
        public long Size { get; internal set; }
        /// <summary>
        ///   Returns number of individual orders in this aggregate order.
        /// </summary>
        public int Count { get; internal set; }
        /// <summary>
        ///   Returns scope of this order.
        /// </summary>
        public Scope Scope { get; internal set; }
        /// <summary>
        ///   Returns side of this order.
        /// </summary>
        public Side Side { get; internal set; }
        /// <summary>
        ///   Returns exchange code of this order.
        /// </summary>
        public char ExchangeCode { get; internal set; }

        #endregion
    }
}
