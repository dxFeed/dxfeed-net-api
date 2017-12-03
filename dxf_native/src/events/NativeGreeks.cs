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
    /// Greeks event is a snapshot of the option price, Black-Scholes volatility and greeks.
    /// It represents the most recent information that is available about the corresponding
    /// values on the market at any given moment of time.
    /// </summary>
    public class NativeGreeks : MarketEventImpl, IDxGreeks
    {

        private static readonly int maxSequence = (1 << 22) - 1;

        internal unsafe NativeGreeks(DxGreeks* g, string symbol) : base(symbol)
        {
            DxGreeks greeks = *g;

            Delta = greeks.delta;
            Gamma = greeks.gamma;
            GreeksPrice = greeks.greeks_price;
            Rho = greeks.rho;
            Sequence = greeks.sequence;
            Theta = greeks.theta;
            Time = TimeConverter.ToUtcDateTime(greeks.time);
            Vega = greeks.vega;
            Volatility = greeks.volatility;
            Index = greeks.index;
        }

        internal NativeGreeks(IDxGreeks greeks) : base(greeks.EventSymbol)
        {

            Delta = greeks.Delta;
            Gamma = greeks.Gamma;
            GreeksPrice = greeks.GreeksPrice;
            Rho = greeks.Rho;
            Sequence = greeks.Sequence;
            Theta = greeks.Theta;
            Time = greeks.Time;
            Vega = greeks.Vega;
            Volatility = greeks.Volatility;
            Index = greeks.Index;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Greeks: {{{0}, " +
                "Time: {1:o}, Sequence: {2}, GreekPrice: {3}, Volatility: {4}, " +
                "Delta: {5}, Gamma: {6}, Theta: {7}, Rho: {8}, Vega: {9}, Index: {10}}}",
                EventSymbol, Time, Sequence, GreeksPrice, Volatility, Delta, Gamma, Theta, Rho, Vega, Index);
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeGreeks(this);
        }
        #endregion

        #region Implementation of IDxGreeks

        /// <summary>
        /// Return option delta.
        /// Delta is the first derivative of an option price by an underlying price.
        /// </summary>
        public double Delta
        {
            get; private set;
        }

        /// <summary>
        /// Returns option gamma.
        /// Gamma is the second derivative of an option price by an underlying price.
        /// </summary>
        public double Gamma
        {
            get; private set;
        }

        /// <summary>
        /// Returns option market price.
        /// </summary>
        public double GreeksPrice
        {
            get; private set;
        }

        /// <summary>
        /// Maximum allowed sequence value. Constant field value.
        /// </summary>
        public int MaxSequence
        {
            get { return maxSequence; }
        }

        /// <summary>
        /// Returns option rho.
        /// Rho is the first derivative of an option price by percentage interest rate.
        /// </summary>
        public double Rho
        {
            get; private set;
        }

        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same time.
        /// This sequence number does not have to be unique and does not need to be sequential.
        /// Sequence can range from 0 to MaxSequence.
        /// </summary>
        public int Sequence
        {
            get; private set;
        }

        /// <summary>
        /// Returns option theta.
        /// Theta is the first derivative of an option price by a number of days to expiration.
        /// </summary>
        public double Theta
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
        /// Returns option vega.
        /// Vega is the first derivative of an option price by percentage volatility.
        /// </summary>
        public double Vega
        {
            get; private set;
        }

        /// <summary>
        /// Returns Black-Scholes implied volatility of the option.
        /// </summary>
        public double Volatility
        {
            get; private set;
        }

        /// <summary>
        /// Returns unique per-symbol index of this event.
        /// The index is composed of Time and Sequence.
        /// </summary>
        public long Index
        {
            get; private set;
        }

        #endregion
    }
}
