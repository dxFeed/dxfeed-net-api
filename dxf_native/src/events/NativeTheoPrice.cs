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
    /// Theo price is a snapshot of the theoretical option price computation that
    /// is periodically performed by dxPrice model-free computation. It represents
    /// the most recent information that is available about the corresponding values
    /// at any given moment of time. The values include first and second order
    /// derivative of the price curve by price, so that the real-time theoretical
    /// option price can be estimated on real-time changes of the underlying price
    /// in the vicinity.
    /// </summary>
    public class NativeTheoPrice : MarketEventImpl, IDxTheoPrice
    {
        internal unsafe NativeTheoPrice(DxTheoPrice* theoPrice, string symbol) : base(symbol)
        {
            DxTheoPrice tp = *theoPrice;

            TheoDelta = tp.theo_delta;
            TheoDividend = tp.theo_dividend;
            TheoGamma = tp.theo_gamma;
            TheoInterest = tp.theo_interest;
            TheoPrice = tp.theo_price;
            TheoTime = TimeConverter.ToUtcDateTime(tp.theo_time);
            TheoUnderlyingPrice = tp.theo_underlying_price;
        }

        internal NativeTheoPrice(IDxTheoPrice tp) : base(tp.EventSymbol)
        {
            TheoDelta = tp.TheoDelta;
            TheoDividend = tp.TheoDividend;
            TheoGamma = tp.TheoGamma;
            TheoInterest = tp.TheoInterest;
            TheoPrice = tp.TheoPrice;
            TheoTime = TheoTime;
            TheoUnderlyingPrice = tp.TheoUnderlyingPrice;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "TheoPrice: {{{0}, " +
                "TheoTime: {1:o}, TheoPrice: {2}, TheoUnderlyingPrice: {3}, " +
                "TheoDelta: {4}, TheoGamma: {5}, TheoDividend: {6}, TheoInterest: {7}}}",
                EventSymbol, TheoTime, TheoPrice, TheoUnderlyingPrice, TheoDelta, TheoGamma, TheoDividend, TheoInterest);
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeTheoPrice(this);
        }
        #endregion

        #region Implementation of IDxTheoPrice

        /// <summary>
        /// Returns delta of the theoretical price.
        /// Delta is the first derivative of the theoretical price by the underlying price.
        /// </summary>
        public double TheoDelta
        {
            get; private set;
        }

        /// <summary>
        /// Returns implied simple dividend return of the corresponding option series.
        /// See the model section for an explanation this simple dividend return \(Q(\tau)\).
        /// </summary>
        public double TheoDividend
        {
            get; private set;
        }

        /// <summary>
        /// Returns gamma of the theoretical price.
        /// Gamma is the second derivative of the theoretical price by the underlying price.
        /// </summary>
        public double TheoGamma
        {
            get; private set;
        }

        /// <summary>
        /// Returns implied simple interest return of the corresponding option series.
        /// See the model section for an explanation this simple interest return \(R(\tau)\).
        /// </summary>
        public double TheoInterest
        {
            get; private set;
        }

        /// <summary>
        /// Returns underlying price at the time of theo price computation.
        /// </summary>
        public double TheoPrice
        {
            get; private set;
        }

        /// <summary>
        /// Returns date time of the last theo price computation.
        /// </summary>
        public DateTime TheoTime
        {
            get; private set;
        }

        /// <summary>
        /// Returns underlying price at the time of theo price computation.
        /// </summary>
        public double TheoUnderlyingPrice
        {
            get; private set;
        }

        #endregion
    }
}
