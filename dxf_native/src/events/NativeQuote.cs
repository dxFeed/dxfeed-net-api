#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;
using System;
using System.Globalization;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Quote event is a snapshot of the best bid and ask prices,
    /// and other fields that change with each quote.
    /// It represents the most recent information that is available about the best quote on
    /// the market at any given moment of time.
    /// </summary>
    public class NativeQuote : MarketEventImpl, IDxQuote
    {
        internal unsafe NativeQuote(DxQuote* q, string symbol) : base(symbol)
        {
            DxQuote quote = *q;
            BidTime = TimeConverter.ToUtcDateTime(quote.bid_time);
            BidExchangeCode = quote.bid_exchange_code;
            BidPrice = quote.bid_price;
            BidSize = quote.bid_size;
            AskTime = TimeConverter.ToUtcDateTime(quote.ask_time);
            AskExchangeCode = quote.ask_exchange_code;
            AskPrice = quote.ask_price;
            AskSize = quote.ask_size;
        }

        internal NativeQuote(IDxQuote quote) : base(quote.EventSymbol)
        {
            BidTime = quote.BidTime;
            BidExchangeCode = quote.BidExchangeCode;
            BidPrice = quote.BidPrice;
            BidSize = quote.BidSize;
            AskTime = quote.AskTime;
            AskExchangeCode = quote.AskExchangeCode;
            AskPrice = quote.AskPrice;
            AskSize = quote.AskSize;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Quote: {{{8}, " +
                "AskExchangeCode: '{0}', Ask: {2}@{1}, AskTime: {3:o}, " +
                "BidExchangeCode: '{4}', Bid: {6}@{5}, BidTime: {7:o} }}",
                AskExchangeCode, AskPrice, AskSize, AskTime, BidExchangeCode, BidPrice,
                BidSize, BidTime, EventSymbol);
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeQuote(this);
        }
        #endregion

        #region Implementation of IDxQuote

        /// <summary>
        /// Returns date time of the last bid change.
        /// </summary>
        public DateTime BidTime
        {
            get; private set;
        }

        /// <summary>
        /// Returns bid exchange code.
        /// </summary>
        public char BidExchangeCode
        {
            get; private set;
        }

        /// <summary>
        /// Returns bid price.
        /// </summary>
        public double BidPrice
        {
            get; private set;
        }

        /// <summary>
        /// Returns bid size.
        /// </summary>
        public long BidSize
        {
            get; private set;
        }

        /// <summary>
        /// Returns date time of the last ask change.
        /// </summary>
        public DateTime AskTime
        {
            get; private set;
        }

        /// <summary>
        /// Returns ask exchange code.
        /// </summary>
        public char AskExchangeCode
        {
            get; private set;
        }

        /// <summary>
        /// Returns ask price.
        /// </summary>
        public double AskPrice
        {
            get; private set;
        }

        /// <summary>
        /// Returns ask size.
        /// </summary>
        public long AskSize
        {
            get; private set;
        }

        #endregion
    }
}