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
    ///   Time and Sale represents a trade (or other market event with price, e.g. market open/close
    ///   price, etc).
    ///   Time and Sales are intended to provide information about trades in a continuous time slice
    ///   (unlike Trade events which are supposed to provide snapshot about the current last trade).
    /// </summary>
    public class NativeTimeAndSale : MarketEventImpl, IDxTimeAndSale
    {
        internal unsafe NativeTimeAndSale(DxTimeAndSale* timeAndSale, string symbol) : base(symbol)
        {
            DxTimeAndSale ts = *timeAndSale;

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
            AgressorSide = ts.side;
            Type = ts.type;
            IsValidTick = ts.is_valid_tick;
            IsETHTrade = ts.is_eth_trade;
            TradeThroughExempt = ts.trade_through_exempt;
            IsSpreadLeg = ts.is_spread_leg;
            RawFlags = ts.raw_flags;
            Scope = ts.scope;
        }

        internal NativeTimeAndSale(IDxTimeAndSale ts) : base(ts.EventSymbol)
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
            AgressorSide = ts.AgressorSide;
            Type = ts.Type;
            IsValidTick = ts.IsValidTick;
            IsETHTrade = ts.IsETHTrade;
            TradeThroughExempt = ts.TradeThroughExempt;
            IsSpreadLeg = ts.IsSpreadLeg;
            RawFlags = ts.RawFlags;
            Scope = ts.Scope;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "TimeAndSale: {{{0}, "                                                  +
                "EventFlags: 0x{1:x2}, Index: {2:x16}, "                                +
                "Time: {3:o}, "                                                         +
                "ExchangeCode: {4}, ExchangeSaleConditions: '{5}', "                    +
                "Price: {6}, Size: {7}, "                                               +
                "BidPrice: {8}, AskPrice: {9}, "                                        +
                "Buyer: '{10}', Seller: '{11}', "                                       +
                "AggressorSide: {12}, "                                                 +
                "Type: {13}, "                                                          +
                "IsValidTick: {14}, IsETHTrade: {15}, IsSpreadLeg: {16}, TTE: '{17}', " +
                "RawFlags: {18:x8}, "                                                   +
                "Scope: {19}"                                                           +
                "}}",
                EventSymbol,
                (int) EventFlags, Index,
                Time,
                ExchangeCode, ExchangeSaleConditions,
                Price, Size,
                BidPrice, AskPrice,
                Buyer, Seller,
                AgressorSide,
                Type,
                IsValidTick, IsETHTrade, IsSpreadLeg, TradeThroughExempt,
                RawFlags, Scope
            );
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeTimeAndSale(this);
        }
        #endregion

        #region Implementation of IDxTimeAndSale

        /// <summary>
        ///     Returns source of this event.
        /// </summary>
        /// <returns>Source of this event.</returns>
        public IndexedEventSource Source { get { return IndexedEventSource.DEFAULT; }  }
        /// <summary>
        ///    Gets or sets transactional event flags.
        ///    See "Event Flags" section from <see cref="IDxIndexedEvent"/>.
        /// </summary>
        public EventFlag EventFlags { get; set; }
        /// <summary>
        ///     Gets unique per-symbol index of this event.
        /// </summary>
        public long Index { get; private set; }
        /// <summary>
        /// Returns timestamp of this event.
        /// The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        public long TimeStamp { get { return TimeConverter.ToUnixTime(Time); } }
        /// <summary>
        /// Returns UTC date and time of this event.
        /// </summary>
        public DateTime Time { get; private set; }
        /// <summary>
        ///   Returns exchange code of this time and sale event.
        /// </summary>
        public char ExchangeCode { get; private set; }
        /// <summary>
        ///   Returns price of this time and sale event.
        /// </summary>
        public double Price { get; private set; }
        /// <summary>
        ///   Returns size of this time and sale event.
        /// </summary>
        public long Size { get; private set; }
        /// <summary>
        ///   Returns the current bid price on the market when this time and sale event had occurred.
        /// </summary>
        public double BidPrice { get; private set; }
        /// <summary>
        ///   Returns the current ask price on the market when this time and sale event had occurred.
        /// </summary>
        public double AskPrice { get; private set; }
        /// <summary>
        ///   Returns sale conditions provided for this event by data feed.
        /// </summary>
        public string ExchangeSaleConditions { get; private set; }
        /// <summary>
        /// Returns implementation-specific raw bit flags value
        /// </summary>
        public int RawFlags { get; private set; }
        /// <summary>
        ///   MMID of buyer (availible not for all markets).
        /// </summary>
        public string Buyer { get; private set; }
        /// <summary>
        ///   MMID of seller (availible not for all markets).
        /// </summary>
        public string Seller { get; private set; }
        /// <summary>
        ///   Returns aggressor side of this time and sale event.
        /// </summary>
        public Side AgressorSide { get; private set; }
        /// <summary>
        ///   Returns type of this time and sale event.
        /// </summary>
        public TimeAndSaleType Type { get; private set; }
        /// <summary>
        ///   Returns whether this is a cancellation of a previous event.
        ///   It is false for newly created time and sale event.
        /// </summary>
        public bool IsCancel { get { return Type == TimeAndSaleType.Cancel; } }
        /// <summary>
        ///   Returns whether this is a correction of a previous event.
        ///   It is false for newly created time and sale event.
        /// </summary>
        public bool IsCorrection { get { return Type == TimeAndSaleType.Correction; } }
        /// <summary>
        ///   Returns whether this is a new event (not cancellation or correction).
        ///   It is true for newly created time and sale event.
        /// </summary>
        public bool IsNew { get { return Type == TimeAndSaleType.New; } }
        /// <summary>
        ///   Returns whether this event represents a valid intraday tick.
        ///   Note, that a correction for a previously distributed valid tick represents a new valid tick itself,
        ///   but a cancellation of a previous valid tick does not.
        /// </summary>
        public bool IsValidTick { get; private set; }
        /// <summary>
        ///   Returns whether this event represents an extended trading hours sale.
        /// </summary>
        public bool IsETHTrade { get; private set; }
        /// <summary>
        ///   Returns TradeThroughExempt flag of this time and sale event.
        /// </summary>
        public char TradeThroughExempt { get; private set; }
        /// <summary>
        ///   Returns whether this event represents a spread leg.
        /// </summary>
        public bool IsSpreadLeg { get; private set; }

        /// <summary>
        /// Returns whether time&sale was a composite or regional (other constants are not used here).
        /// </summary>
        public Scope Scope { get; private set; }
        
        #endregion
    }
}