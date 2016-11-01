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
    /// Time and Sale represents a trade (or other market event with price, e.g. market open/close 
    /// price, etc).
    /// Time and Sales are intended to provide information about trades in a continuous time slice
    /// (unlike Trade events which are supposed to provide snapshot about the current last trade).
    /// </summary>
    public class NativeTimeAndSale : MarketEvent, IDxTimeAndSale
    {
        private readonly DxTimeAndSale ts;
        private readonly DxString saleCond;

        internal unsafe NativeTimeAndSale(DxTimeAndSale* ts, string symbol) : base(symbol)
        {
            this.ts = *ts;
            saleCond = DxMarshal.ReadDxString(this.ts.exchange_sale_conditions);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "TimeAndSale: {{{10}, " +
                "EventId: {0:x4}, Time: {1:o}, ExchangeCode: '{2}', Ask: {3}, Bid: {4}, " +
                "ExchangeSaleConditions: '{5}', IsTrade: {6}, Price: {7}, Size: {8}, Type: {9}}}",
                EventId, Time, ExchangeCode, AskPrice, BidPrice, ExchangeSaleConditions, IsTrade,
                Price, Size, Type, EventSymbol);
        }

        #region Implementation of IDxTimeAndSale

        /// <summary>
        /// Returns aggressor side of this time and sale event.
        /// </summary>
        public Side AgressorSide
        {
            get { return ts.side; }
        }

        /// <summary>
        /// Returns the current ask price on the market when this time and sale event had occurred.
        /// </summary>
        public double AskPrice
        {
            get { return ts.ask_price; }
        }

        /// <summary>
        /// Returns the current bid price on the market when this time and sale event had occurred.
        /// </summary>
        public double BidPrice
        {
            get { return ts.bid_price; }
        }

        /// <summary>
        /// Returns event flags of this time and sale event.
        /// </summary>
        public int EventFlags
        {
            get { return ts.event_flags; }
        }

        /// <summary>
        /// Returns unique per-symbol index of this time and sale event.
        /// Deprecated Use Index
        /// </summary>
        public long EventId
        {
            get { return ts.event_id; }
        }

        /// <summary>
        /// Returns exchange code of this time and sale event.
        /// </summary>
        public char ExchangeCode
        {
            get { return ts.exchange_code; }
        }

        /// <summary>
        /// Returns sale conditions provided for this event by data feed.
        /// </summary>
        public DxString ExchangeSaleConditions
        {
            get { return saleCond; }
        }

        /// <summary>
        /// Returns unique per-symbol index of this time and sale event.
        /// Time and sale index is composed of Time and Sequence.
        /// </summary>
        public long Index
        {
            get { return ts.index; }
        }

        /// <summary>
        /// Returns price of this time and sale event.
        /// </summary>
        public double Price
        {
            get { return ts.price; }
        }

        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same
        /// Time. This sequence number does not have to be unique and does not need to be 
        /// sequential.Sequence can range from 0 to MAX_SEQUENCE.
        /// </summary>
        public int Sequence
        {
            get { return ts.sequence; }
        }

        /// <summary>
        /// Returns size of this time and sale event.
        /// </summary>
        public long Size
        {
            get { return ts.size; }
        }

        /// <summary>
        /// Returns timestamp of the original event.
        /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
        /// </summary>
        public DateTime Time
        {
            get { return TimeConverter.ToUtcDateTime(ts.time); }
        }

        /// <summary>
        /// Returns type of this time and sale event.
        /// </summary>
        public TimeAndSaleType Type
        {
            get { return ts.type; }
        }

        /// <summary>
        /// Returns whether this is a cancellation of a previous event.
        /// It is false for newly created time and sale event.
        /// </summary>
        public bool IsCancel
        {
            get { return ts.is_cancel; }
        }

        /// <summary>
        /// Returns whether this is a correction of a previous event.
        /// It is false for newly created time and sale event.
        /// </summary>
        public bool IsCorrection
        {
            get { return ts.is_correction; }
        }

        /// <summary>
        /// Returns whether this event represents an extended trading hours sale.
        /// </summary>
        public bool IsTrade
        {
            get { return ts.is_trade; }
        }

        /// <summary>
        /// Returns whether this is a new event (not cancellation or correction).
        /// It is true for newly created time and sale event.
        /// </summary>
        public bool IsNew
        {
            get { return ts.is_new; }
        }

        /// <summary>
        /// Returns whether this event represents a spread leg.
        /// </summary>
        public bool IsSpreadLeg
        {
            get { return ts.is_spread_leg; }
        }

        /// <summary>
        /// Returns whether this event represents a valid intraday tick.
        /// Note, that a correction for a previously distributed valid tick represents a new valid tick itself,
        /// but a cancellation of a previous valid tick does not.
        /// </summary>
        public bool IsValidTick
        {
            get { return ts.is_valid_tick; }
        }

        #endregion
    }
}