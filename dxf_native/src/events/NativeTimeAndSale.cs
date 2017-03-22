#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System;
using System.Globalization;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Time and Sale represents a trade (or other market event with price, e.g. market open/close 
    /// price, etc).
    /// Time and Sales are intended to provide information about trades in a continuous time slice
    /// (unlike Trade events which are supposed to provide snapshot about the current last trade).
    /// </summary>
    public class NativeTimeAndSale : MarketEventImpl, IDxTimeAndSale
    {
        internal unsafe NativeTimeAndSale(DxTimeAndSale* timeAndSale, string symbol) : base(symbol)
        {
            DxTimeAndSale ts = *timeAndSale;

            AgressorSide = ts.side;
            AskPrice = ts.ask_price;
            BidPrice = ts.bid_price;
            EventFlags = ts.event_flags;
            EventId = ts.event_id;
            ExchangeCode = ts.exchange_code;
            ExchangeSaleConditions = DxMarshal.ReadDxString(ts.exchange_sale_conditions);
            Index = ts.index;
            Price = ts.price;
            Sequence = ts.sequence;
            Size = ts.size;
            TimeStamp = ts.time;
            Time = TimeConverter.ToUtcDateTime(TimeStamp);
            Type = ts.type;
            IsCancel = ts.is_cancel;
            IsCorrection = ts.is_correction;
            IsTrade = ts.is_trade;
            IsNew = ts.is_new;
            IsSpreadLeg = ts.is_spread_leg;
            IsValidTick = ts.is_valid_tick;
        }

        internal NativeTimeAndSale(IDxTimeAndSale ts) : base(ts.EventSymbol)
        {
            AgressorSide = ts.AgressorSide;
            AskPrice = ts.AskPrice;
            BidPrice = ts.BidPrice;
            EventFlags = ts.EventFlags;
            EventId = ts.EventId;
            ExchangeCode = ts.ExchangeCode;
            //TODO: check
            ExchangeSaleConditions = (DxString)ts.ExchangeSaleConditions.Clone();
            Index = ts.Index;
            Price = ts.Price;
            Sequence = ts.Sequence;
            Size = ts.Size;
            TimeStamp = ts.TimeStamp;
            Time = ts.Time;
            Type = ts.Type;
            IsCancel = ts.IsCancel;
            IsCorrection = ts.IsCorrection;
            IsTrade = ts.IsTrade;
            IsNew = ts.IsNew;
            IsSpreadLeg = ts.IsSpreadLeg;
            IsValidTick = ts.IsValidTick;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "TimeAndSale: {{{10}, " +
                "EventId: {0:x4}, Time: {1:o}, ExchangeCode: '{2}', Ask: {3}, Bid: {4}, " +
                "ExchangeSaleConditions: '{5}', IsTrade: {6}, Price: {7}, Size: {8}, Type: {9}}}",
                EventId, Time, ExchangeCode, AskPrice, BidPrice, ExchangeSaleConditions, IsTrade,
                Price, Size, Type, EventSymbol);
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeTimeAndSale(this);
        }
        #endregion

        #region Implementation of IDxTimeAndSale

        /// <summary>
        /// Returns aggressor side of this time and sale event.
        /// </summary>
        public Side AgressorSide
        {
            get; private set;
        }

        /// <summary>
        /// Returns the current ask price on the market when this time and sale event had occurred.
        /// </summary>
        public double AskPrice
        {
            get; private set;
        }

        /// <summary>
        /// Returns the current bid price on the market when this time and sale event had occurred.
        /// </summary>
        public double BidPrice
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets event flags of this time and sale event.
        /// </summary>
        public EventFlag EventFlags
        {
            get; set;
        }

        /// <summary>
        /// Returns unique per-symbol index of this time and sale event.
        /// Deprecated Use Index
        /// </summary>
        public long EventId
        {
            get; private set;
        }

        /// <summary>
        /// Returns exchange code of this time and sale event.
        /// </summary>
        public char ExchangeCode
        {
            get; private set;
        }

        /// <summary>
        /// Returns sale conditions provided for this event by data feed.
        /// </summary>
        public DxString ExchangeSaleConditions
        {
            get; private set;
        }

        /// <summary>
        /// Returns unique per-symbol index of this time and sale event.
        /// Time and sale index is composed of Time and Sequence.
        /// </summary>
        public long Index
        {
            get; private set;
        }

        /// <summary>
        /// Returns price of this time and sale event.
        /// </summary>
        public double Price
        {
            get; private set;
        }

        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same
        /// Time. This sequence number does not have to be unique and does not need to be 
        /// sequential.
        /// </summary>
        public int Sequence
        {
            get; private set;
        }

        /// <summary>
        /// Returns size of this time and sale event.
        /// </summary>
        public long Size
        {
            get; private set;
        }

        /// <summary>
        /// Returns timestamp of this event.
        /// The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        public long TimeStamp
        {
            get; private set;
        }

        /// <summary>
        /// Returns UTC date and time of this event.
        /// </summary>
        public DateTime Time
        {
            get; private set;
        }

        /// <summary>
        /// Returns type of this time and sale event.
        /// </summary>
        public TimeAndSaleType Type
        {
            get; private set;
        }

        /// <summary>
        /// Returns whether this is a cancellation of a previous event.
        /// It is false for newly created time and sale event.
        /// </summary>
        public bool IsCancel
        {
            get; private set;
        }

        /// <summary>
        /// Returns whether this is a correction of a previous event.
        /// It is false for newly created time and sale event.
        /// </summary>
        public bool IsCorrection
        {
            get; private set;
        }

        /// <summary>
        /// Returns whether this event represents an extended trading hours sale.
        /// </summary>
        public bool IsTrade
        {
            get; private set;
        }

        /// <summary>
        /// Returns whether this is a new event (not cancellation or correction).
        /// It is true for newly created time and sale event.
        /// </summary>
        public bool IsNew
        {
            get; private set;
        }

        /// <summary>
        /// Returns whether this event represents a spread leg.
        /// </summary>
        public bool IsSpreadLeg
        {
            get; private set;
        }

        /// <summary>
        /// Returns whether this event represents a valid intraday tick.
        /// Note, that a correction for a previously distributed valid tick represents a new valid tick itself,
        /// but a cancellation of a previous valid tick does not.
        /// </summary>
        public bool IsValidTick
        {
            get; private set;
        }

        #endregion
    }
}