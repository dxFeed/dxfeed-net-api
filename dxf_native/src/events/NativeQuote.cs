#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

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
    ///     Quote event is a snapshot of the best bid and ask prices,
    ///     and other fields that change with each quote.
    ///     It represents the most recent information that is available about the best quote on
    ///     the market at any given moment of time.
    /// </summary>
    public class NativeQuote : MarketEventImpl, IDxQuote
    {
        internal unsafe NativeQuote(DxQuote* q, string symbol) : base(symbol)
        {
            var quote = *q;

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

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="quote">The original Quote event</param>
        public NativeQuote(IDxQuote quote) : base(quote.EventSymbol)
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

        /// <summary>
        ///     Default constructor
        /// </summary>
        public NativeQuote()
        {
        }

        /// <summary>
        ///     Constructs the copy and replaces the symbol
        /// </summary>
        /// <param name="quote">The original Quote event</param>
        /// <param name="symbol">The new symbol</param>
        public NativeQuote(IDxQuote quote, string symbol) : base(symbol)
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

        /// <summary>
        ///     Returns a normalized event. Normalization occurs for events in which Scope = Composite, and the symbol ends
        ///     with &amp; and the exchange code (that is, satisfies the regex: "&amp;[A-Z]")
        /// </summary>
        /// <returns>Normalized event or current event if normalization has not been performed.</returns>
        public IDxQuote Normalized()
        {
            if (Scope == Scope.Regional) return this;

            var exchangeCodeSeparatorPos = EventSymbol.LastIndexOf('&');

            if (exchangeCodeSeparatorPos < 0 || exchangeCodeSeparatorPos != EventSymbol.Length - 2) return this;

            var exchangeCode = EventSymbol[exchangeCodeSeparatorPos + 1];

            if (exchangeCode < 'A' || exchangeCode > 'Z') return this;

            return new NativeQuote(this, EventSymbol.Substring(0, exchangeCodeSeparatorPos))
            {
                Scope = Scope.Regional,
                AskExchangeCode = exchangeCode,
                BidExchangeCode = exchangeCode
            };
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeQuote(this);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Quote: {{{0}, " +
                "AskExchangeCode: '{1}', Ask: {2}@{3}, AskTime: {4:o}, " +
                "BidExchangeCode: '{5}', Bid: {6}@{7}, BidTime: {8:o}, Scope: {9}" +
                "}}",
                EventSymbol,
                AskExchangeCode, AskSize, AskPrice, AskTime,
                BidExchangeCode, BidSize, BidPrice, BidTime, Scope);
        }

        #region Implementation of IDxQuote

        /// <summary>
        ///     Returns time of the last bid or ask change.
        ///     This method is the same as max(getBidTime(), getAskTime())
        ///     Note, that unlike bid/ask times, that are transmitted over network in a second-precision, this
        ///     time is transmitted up to a millisecond and even nano-second precision (see getTimeNanoPart())
        ///     if it is enabled on server side.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///     Returns sequence number of this quote to distinguish quotes that have the same
        ///     time. This sequence number does not have to be unique and
        ///     does not need to be sequential.
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        ///     Returns microseconds and nanoseconds part of time of the last bid or ask change.
        /// </summary>
        public int TimeNanoPart { get; set; }

        /// <summary>
        ///     Returns time of the last bid change.
        ///     This time is always transmitted with seconds precision, so the result of this method
        ///     is usually a multiple of 1000.
        /// </summary>
        public DateTime BidTime { get; set; }

        /// <summary>
        ///     Returns bid exchange code.
        /// </summary>
        public char BidExchangeCode { get; set; }

        /// <summary>
        ///     Returns bid price.
        /// </summary>
        public double BidPrice { get; set; }

        /// <summary>
        ///     Returns bid size.
        /// </summary>
        public double BidSize { get; set; }

        /// <summary>
        ///     Returns date time of the last ask change.
        ///     This time is always transmitted with seconds precision, so the result of this method
        ///     is usually a multiple of 1000.
        /// </summary>
        public DateTime AskTime { get; set; }

        /// <summary>
        ///     Returns ask exchange code.
        /// </summary>
        public char AskExchangeCode { get; set; }

        /// <summary>
        ///     Returns ask price.
        /// </summary>
        public double AskPrice { get; set; }

        /// <summary>
        ///     Returns ask size.
        /// </summary>
        public double AskSize { get; set; }

        /// <summary>
        ///     Returns whether quote is composite or regional (other constants are not used here)
        /// </summary>
        public Scope Scope { get; set; }

        #endregion
    }
}