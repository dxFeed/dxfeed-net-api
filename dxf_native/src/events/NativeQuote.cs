#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

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

            Time = TimeConverter.ToUtcDateTime(quote.time);
            Sequence = quote.sequence;
            TimeNanoPart = quote.time_nanos;
            BidTime = TimeConverter.ToUtcDateTime(quote.bid_time);
            BidExchangeCode = quote.bid_exchange_code;
            BidPrice = quote.bid_price;
            BidSize = quote.bid_size;
            AskTime = TimeConverter.ToUtcDateTime(quote.ask_time);
            AskExchangeCode = quote.ask_exchange_code;
            AskPrice = quote.ask_price;
            AskSize = quote.ask_size;
            Scope = quote.scope;
        }

        internal NativeQuote(IDxQuote quote) : base(quote.EventSymbol)
        {
            Time = quote.Time;
            Sequence = quote.Sequence;
            TimeNanoPart = quote.TimeNanoPart;
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
            return string.Format(CultureInfo.InvariantCulture,
                "Quote: {{{0}, " +
                "AskExchangeCode: '{1}', Ask: {2}@{3}, AskTime: {4:o}, " +
                "BidExchangeCode: '{5}', Bid: {6}@{7}, BidTime: {8:o}, Scope: {9}"   +
                "}}",
                EventSymbol,
                AskExchangeCode, AskSize, AskPrice, AskTime,
                BidExchangeCode, BidSize, BidPrice, BidTime, Scope);
        }

        #region Implementation of ICloneable

        public override object Clone()
        {
            return new NativeQuote(this);
        }

        #endregion

        #region Implementation of IDxQuote

        /// <summary>
        /// Returns time of the last bid or ask change.
        /// This method is the same as max(getBidTime(), getAskTime())
        /// Note, that unlike bid/ask times, that are transmitted over network in a second-precision, this
        /// time is transmitted up to a millisecond and even nano-second precision (see getTimeNanoPart())
        /// if it is enabled on server side.
        /// </summary>
        public DateTime Time { get; private set; }
        /// <summary>
        /// Returns sequence number of this quote to distinguish quotes that have the same
        /// time. This sequence number does not have to be unique and
        /// does not need to be sequential.
        /// </summary>
        public int Sequence { get; private set; }
        /// <summary>
        /// Returns microseconds and nanoseconds part of time of the last bid or ask change.
        /// </summary>
        public int TimeNanoPart { get; private set; }
        /// <summary>
        /// Returns time of the last bid change.
        /// This time is always transmitted with seconds precision, so the result of this method
        /// is usually a multiple of 1000.
        /// </summary>
        public DateTime BidTime { get; private set; }
        /// <summary>
        /// Returns bid exchange code.
        /// </summary>
        public char BidExchangeCode { get; private set; }
        /// <summary>
        /// Returns bid price.
        /// </summary>
        public double BidPrice { get; private set; }
        /// <summary>
        /// Returns bid size.
        /// </summary>
        public long BidSize { get; private set; }
        /// <summary>
        /// Returns date time of the last ask change.
        /// This time is always transmitted with seconds precision, so the result of this method
        /// is usually a multiple of 1000.
        /// </summary>
        public DateTime AskTime { get; private set; }
        /// <summary>
        /// Returns ask exchange code.
        /// </summary>
        public char AskExchangeCode { get; private set; }
        /// <summary>
        /// Returns ask price.
        /// </summary>
        public double AskPrice { get; private set; }
        /// <summary>
        /// Returns ask size.
        /// </summary>
        public long AskSize { get; private set; }
        /// <summary>
        /// Returns whether quote is composite or regional (other constants are not used here)
        /// </summary>
        public Scope Scope { get; internal set; }

        #endregion
    }
}