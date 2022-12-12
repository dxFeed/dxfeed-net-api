#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Globalization;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    ///     Theo price is a snapshot of the theoretical option price computation that
    ///     is periodically performed by dxPrice model-free computation. It represents
    ///     the most recent information that is available about the corresponding values
    ///     at any given moment of time. The values include first and second order
    ///     derivative of the price curve by price, so that the real-time theoretical
    ///     option price can be estimated on real-time changes of the underlying price
    ///     in the vicinity.
    /// </summary>
    public class NativeTheoPrice : MarketEventImpl, IDxTheoPrice
    {
        internal unsafe NativeTheoPrice(DxTheoPrice* theoPrice, string symbol) : base(symbol)
        {
            var tp = *theoPrice;

            Time = TimeConverter.ToUtcDateTime(tp.time);
            Price = tp.price;
            UnderlyingPrice = tp.underlying_price;
            Delta = tp.delta;
            Gamma = tp.gamma;
            Dividend = tp.dividend;
            Interest = tp.interest;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="tp">The original TheoPrice event</param>
        public NativeTheoPrice(IDxTheoPrice tp) : base(tp.EventSymbol)
        {
            Time = tp.Time;
            Price = tp.Price;
            UnderlyingPrice = tp.UnderlyingPrice;
            Delta = tp.Delta;
            Gamma = tp.Gamma;
            Dividend = tp.Dividend;
            Interest = tp.Interest;
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        public NativeTheoPrice()
        {
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeTheoPrice(this);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "TheoPrice: {{{0}, " +
                "Time: {1:o}, " +
                "Price: {2}, UnderlyingPrice: {3}, " +
                "Delta: {4}, Gamma: {5}, " +
                "Dividend: {6}, Interest: {7}" +
                "}}",
                EventSymbol,
                Time,
                Price, UnderlyingPrice,
                Delta, Gamma,
                Dividend, Interest
            );
        }

        #region Implementation of IDxTheoPrice

        /// <summary>
        ///     Returns date time of the last theo price computation.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///     Returns theoretical option price.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Returns underlying price at the time of theo price computation.
        /// </summary>
        public double UnderlyingPrice { get; set; }

        /// <summary>
        ///     Returns delta of the theoretical price.
        ///     Delta is the first derivative of the theoretical price by the underlying price.
        /// </summary>
        public double Delta { get; set; }

        /// <summary>
        ///     Returns gamma of the theoretical price.
        ///     Gamma is the second derivative of the theoretical price by the underlying price.
        /// </summary>
        public double Gamma { get; set; }

        /// <summary>
        ///     Returns implied simple dividend return of the corresponding option series.
        ///     See the model section for an explanation this simple dividend return \(Q(\tau)\).
        /// </summary>
        public double Dividend { get; set; }

        /// <summary>
        ///     Returns implied simple interest return of the corresponding option series.
        ///     See the model section for an explanation this simple interest return \(R(\tau)\).
        /// </summary>
        public double Interest { get; set; }

        #endregion
    }
}