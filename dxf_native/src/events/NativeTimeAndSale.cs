#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

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
    ///     Time and Sale represents a trade (or other market event with price, e.g. market open/close
    ///     price, etc).
    ///     Time and Sales are intended to provide information about trades in a continuous time slice
    ///     (unlike Trade events which are supposed to provide snapshot about the current last trade).
    /// </summary>
    public class NativeTimeAndSale : MarketEventImpl, IDxTimeAndSale
    {
        internal unsafe NativeTimeAndSale(DxTimeAndSale* timeAndSale, string symbol) : base(symbol)
        {
            var ts = *timeAndSale;

            EventFlags = ts.event_flags;
            Index = ts.index;
            Time = TimeConverter.ToUtcDateTime(ts.time);
            ExchangeCode = ts.exchange_code;
            Price = ts.price;
            Size = ts.size;
            BidPrice = ts.bid_price;
            AskPrice = ts.ask_price;
            ExchangeSaleConditions = new string((char*)ts.exchange_sale_conditions.ToPointer());
            Buyer = new string((char*)ts.buyer.ToPointer());
            Seller = new string((char*)ts.seller.ToPointer());
            AggressorSide = ts.side;
            Type = ts.type;
            IsValidTick = ts.is_valid_tick;
            IsETHTrade = ts.is_eth_trade;
            TradeThroughExempt = ts.trade_through_exempt;
            IsSpreadLeg = ts.is_spread_leg;
            RawFlags = ts.raw_flags;
            Scope = ts.scope;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="ts">The original TimeAndSale event</param>
        public NativeTimeAndSale(IDxTimeAndSale ts) : base(ts.EventSymbol)
        {
            EventFlags = ts.EventFlags;
            Index = ts.Index;
            Time = ts.Time;
            ExchangeCode = ts.ExchangeCode;
            Price = ts.Price;
            Size = ts.Size;
            BidPrice = ts.BidPrice;
            AskPrice = ts.AskPrice;
            ExchangeSaleConditions = ts.ExchangeSaleConditions;
            Buyer = ts.Buyer;
            Seller = ts.Seller;
            AggressorSide = ts.AggressorSide;
            Type = ts.Type;
            IsValidTick = ts.IsValidTick;
            IsETHTrade = ts.IsETHTrade;
            TradeThroughExempt = ts.TradeThroughExempt;
            IsSpreadLeg = ts.IsSpreadLeg;
            RawFlags = ts.RawFlags;
            Scope = ts.Scope;
        }

        /// <summary>
        ///     Constructs the copy and replaces the symbol
        /// </summary>
        /// <param name="ts">The original TimeAndSale event</param>
        /// <param name="symbol">The new symbol</param>
        public NativeTimeAndSale(IDxTimeAndSale ts, string symbol) : base(symbol)
        {
            EventFlags = ts.EventFlags;
            Index = ts.Index;
            Time = ts.Time;
            ExchangeCode = ts.ExchangeCode;
            Price = ts.Price;
            Size = ts.Size;
            BidPrice = ts.BidPrice;
            AskPrice = ts.AskPrice;
            ExchangeSaleConditions = ts.ExchangeSaleConditions;
            Buyer = ts.Buyer;
            Seller = ts.Seller;
            AggressorSide = ts.AggressorSide;
            Type = ts.Type;
            IsValidTick = ts.IsValidTick;
            IsETHTrade = ts.IsETHTrade;
            TradeThroughExempt = ts.TradeThroughExempt;
            IsSpreadLeg = ts.IsSpreadLeg;
            RawFlags = ts.RawFlags;
            Scope = ts.Scope;
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        public NativeTimeAndSale()
        {
        }

        /// <summary>
        ///     Returns a normalized event. Normalization occurs for events in which Scope = Composite, and the symbol ends
        ///     with &amp; and the exchange code (that is, satisfies the regex: "&amp;[A-Z]")
        /// </summary>
        /// <returns>Normalized event or current event if normalization has not been performed.</returns>
        public IDxTimeAndSale Normalized()
        {
            if (Scope == Scope.Regional) return this;

            var exchangeCodeSeparatorPos = EventSymbol.LastIndexOf('&');

            if (exchangeCodeSeparatorPos < 0 || exchangeCodeSeparatorPos != EventSymbol.Length - 2) return this;

            var exchangeCode = EventSymbol[exchangeCodeSeparatorPos + 1];

            if (exchangeCode < 'A' || exchangeCode > 'Z') return this;

            return new NativeTimeAndSale(this, EventSymbol.Substring(0, exchangeCodeSeparatorPos))
            {
                Scope = Scope.Regional,
                ExchangeCode = exchangeCode
            };
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeTimeAndSale(this);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "TimeAndSale: {{{0}, " +
                "EventFlags: 0x{1:x2}, Index: {2:x16}, " +
                "Time: {3:o}, " +
                "ExchangeCode: {4}, ExchangeSaleConditions: '{5}', " +
                "Price: {6}, Size: {7}, " +
                "BidPrice: {8}, AskPrice: {9}, " +
                "Buyer: '{10}', Seller: '{11}', " +
                "AggressorSide: {12}, " +
                "Type: {13}, " +
                "IsValidTick: {14}, IsETHTrade: {15}, IsSpreadLeg: {16}, TTE: '{17}', " +
                "RawFlags: {18:x8}, " +
                "Scope: {19}" +
                "}}",
                EventSymbol,
                (int)EventFlags, Index,
                Time,
                ExchangeCode, ExchangeSaleConditions,
                Price, Size,
                BidPrice, AskPrice,
                Buyer, Seller,
                AggressorSide,
                Type,
                IsValidTick, IsETHTrade, IsSpreadLeg, TradeThroughExempt,
                RawFlags, Scope
            );
        }

        #region Implementation of IDxTimeAndSale

        /// <summary>
        ///     Returns source of this event.
        /// </summary>
        /// <returns>Source of this event.</returns>
        public IndexedEventSource Source => IndexedEventSource.DEFAULT;

        /// <summary>
        ///     Gets or sets transactional event flags.
        ///     See "Event Flags" section from <see cref="IDxIndexedEvent" />.
        /// </summary>
        public EventFlag EventFlags { get; set; }

        /// <summary>
        ///     Gets unique per-symbol index of this event.
        /// </summary>
        public long Index { get; set; }

        /// <summary>
        ///     Returns timestamp of this event.
        ///     The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        public long TimeStamp => TimeConverter.ToUnixTime(Time);

        /// <summary>
        ///     Returns UTC date and time of this event.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///     Returns exchange code of this time and sale event.
        /// </summary>
        public char ExchangeCode { get; set; }

        /// <summary>
        ///     Returns price of this time and sale event.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Returns size of this time and sale event.
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        ///     Returns the current bid price on the market when this time and sale event had occurred.
        /// </summary>
        public double BidPrice { get; set; }

        /// <summary>
        ///     Returns the current ask price on the market when this time and sale event had occurred.
        /// </summary>
        public double AskPrice { get; set; }

        /// <summary>
        ///     Returns sale conditions provided for this event by data feed.
        /// </summary>
        public string ExchangeSaleConditions { get; set; }

        /// <summary>
        ///     Returns implementation-specific raw bit flags value
        /// </summary>
        public int RawFlags { get; set; }

        /// <summary>
        ///     MMID of buyer (available not for all markets).
        /// </summary>
        public string Buyer { get; set; }

        /// <summary>
        ///     MMID of seller (available not for all markets).
        /// </summary>
        public string Seller { get; set; }

        /// <summary>
        ///     Returns aggressor side of this time and sale event.
        /// </summary>
        public Side AggressorSide { get; set; }

        /// <summary>
        ///     Returns type of this time and sale event.
        /// </summary>
        public TimeAndSaleType Type { get; set; }

        /// <summary>
        ///     Returns whether this is a cancellation of a previous event.
        ///     It is false for newly created time and sale event.
        /// </summary>
        public bool IsCancel => Type == TimeAndSaleType.Cancel;

        /// <summary>
        ///     Returns whether this is a correction of a previous event.
        ///     It is false for newly created time and sale event.
        /// </summary>
        public bool IsCorrection => Type == TimeAndSaleType.Correction;

        /// <summary>
        ///     Returns whether this is a new event (not cancellation or correction).
        ///     It is true for newly created time and sale event.
        /// </summary>
        public bool IsNew => Type == TimeAndSaleType.New;

        /// <summary>
        ///     Returns whether this event represents a valid intraday tick.
        ///     Note, that a correction for a previously distributed valid tick represents a new valid tick itself,
        ///     but a cancellation of a previous valid tick does not.
        /// </summary>
        public bool IsValidTick { get; set; }

        /// <summary>
        ///     Returns whether this event represents an extended trading hours sale.
        /// </summary>
        public bool IsETHTrade { get; set; }

        /// <summary>
        ///     Returns TradeThroughExempt flag of this time and sale event.
        /// </summary>
        public char TradeThroughExempt { get; set; }

        /// <summary>
        ///     Returns whether this event represents a spread leg.
        /// </summary>
        public bool IsSpreadLeg { get; set; }

        /// <summary>
        ///     Returns whether time&amp;sale was a composite or regional (other constants are not used here).
        /// </summary>
        public Scope Scope { get; set; }

        #endregion
    }
}