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
    /// Base class for common fields of Order and SpreadOrder events.
    /// Order events represent a snapshot for a full available market depth for a symbol.
    /// The collection of order events of a symbol represents the most recent information that is
    /// available about orders on the market at any given moment of time.
    /// </summary>
    public class NativeOrderBase : MarketEventImpl, IDxOrderBase
    {
        internal unsafe NativeOrderBase(DxOrder* o, string symbol) : base(symbol)
        {
            DxOrder order = *o;

            Count = order.count;
            EventFlags = order.event_flags;
            ExchangeCode = order.exchange_code;
            Index = order.index;
            Level = order.level;
            Side = order.side;
            Price = order.price;
            Scope = Scope.ValueOf(order.scope);
            Sequence = order.sequence;
            Size = order.size;
            Time = TimeConverter.ToUtcDateTime(order.time);
            //TODO: check 
            Source = OrderSource.ValueOf(new string(order.source));
            TimeSequence = order.time_sequence;
        }

        internal unsafe NativeOrderBase(DxSpreadOrder* o, string symbol) : base(symbol)
        {
            DxSpreadOrder order = *o;

            Count = order.count;
            EventFlags = order.event_flags;
            ExchangeCode = order.exchange_code;
            Index = order.index;
            Level = order.level;
            Side = order.side;
            Price = order.price;
            Scope = Scope.ValueOf(order.scope);
            Sequence = order.sequence;
            Size = order.size;
            Time = TimeConverter.ToUtcDateTime(order.time);
            Source = OrderSource.ValueOf(new string(order.source));
            TimeSequence = order.time_sequence;
        }

        internal NativeOrderBase(IDxOrderBase order) : base(order.EventSymbol)
        {
            Count = order.Count;
            EventFlags = order.EventFlags;
            ExchangeCode = order.ExchangeCode;
            Index = order.Index;
            Level = order.Level;
            Side = order.Side;
            Price = order.Price;
            Scope = Scope.ValueOf(order.Scope.Code);
            Sequence = order.Sequence;
            Size = order.Size;
            Time = order.Time;
            Source = OrderSource.ValueOf(order.Source.Name);
            TimeSequence = order.TimeSequence;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{1}, {7}@{6}, " +
                "Index: {0:x4}, Level: {2}, Time: {3:o}, Sequence: {4}, ExchangeCode: '{5}', " +
                "Source: '{8}', EventFlags: 0x{9:X}, Scope: {10}, Count: {11}",
                Index, Side, Level, Time, Sequence, ExchangeCode, Price, Size, Source, EventFlags, 
                Scope, Count);
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeOrderBase(this);
        }
        #endregion

        #region Implementation of IDxOrderBase

        /// <summary>
        /// Returns number of individual orders in this aggregate order.
        /// </summary>
        public long Count
        {
            get; private set;
        }

        /// <summary>
        /// Returns event flags.
        /// </summary>
        public EventFlag EventFlags
        {
            get; set;
        }

        /// <summary>
        /// Returns exchange code of this order.
        /// </summary>
        public char ExchangeCode
        {
            get; private set;
        }

        /// <summary>
        /// Returns unique per-symbol index of this order. Index is non-negative.
        /// </summary>
        public long Index
        {
            get; private set;
        }

        /// <summary>
        /// Returns detail level of this order.
        /// Deprecated use Scope instead.
        /// </summary>
        public int Level
        {
            get; private set;
        }

        /// <summary>
        /// Returns side of this order.
        /// </summary>
        public Side Side
        {
            get; private set;
        }

        /// <summary>
        /// Returns price of this order.
        /// </summary>
        public double Price
        {
            get; private set;
        }

        /// <summary>
        /// Returns scope of this order.
        /// </summary>
        public Scope Scope
        {
            get; private set;
        }

        /// <summary>
        /// Returns sequence number of this order to distinguish orders that have the same Time.
        /// This sequence number does not have to be unique and does not need to be sequential.
        /// </summary>
        public int Sequence
        {
            get; private set;
        }

        /// <summary>
        /// Returns size of this order.
        /// </summary>
        public long Size
        {
            get; private set;
        }

        /// <summary>
        /// Returns source of this event.
        /// </summary>
        public OrderSource Source
        {
            get; private set;
        }

        /// <summary>
        /// Returns date time of this order.
        /// </summary>
        public DateTime Time
        {
            get; private set;
        }

        /// <summary>
        /// Returns time and sequence of this order packaged into single long value.
        /// This method is intended for efficient order time priority comparison.
        /// </summary>
        public long TimeSequence
        {
            get; private set;
        }

        #endregion
    }
}