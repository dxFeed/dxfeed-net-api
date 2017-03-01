/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Greeks event is a snapshot of the option price, Black-Scholes volatility and greeks.
    /// It represents the most recent information that is available about the corresponding
    /// values on the market at any given moment of time.
    /// </summary>
    public class NativeGreeks : MarketEvent, IDxGreeks
    {
        private readonly DxGreeks grks;
        private static readonly int maxSequence = (1 << 22) - 1;

        //TODO: add event flags argument
        internal unsafe NativeGreeks(DxGreeks* grks, string symbol) : base(symbol)
        {
            this.grks = *grks;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Greeks: {{{0}, " +
                "Time: {1:o}, Sequence: {2}, GreekPrice: {3}, Volatility: {4}, " +
                "Delta: {5}, Gamma: {6}, Theta: {7}, Rho: {8}, Vega: {9}, Index: {10}}}",
                EventSymbol, Time, Sequence, GreeksPrice, Volatility, Delta, Gamma, Theta, Rho, Vega, Index);
        }

        #region Implementation of IDxGreeks

        /// <summary>
        /// Return option delta.
        /// Delta is the first derivative of an option price by an underlying price.
        /// </summary>
        public double Delta
        {
            get { return grks.delta; }
        }

        /// <summary>
        /// Returns option gamma.
        /// Gamma is the second derivative of an option price by an underlying price.
        /// </summary>
        public double Gamma
        {
            get { return grks.gamma; }
        }

        /// <summary>
        /// Returns option market price.
        /// </summary>
        public double GreeksPrice
        {
            get { return grks.greeks_price; }
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
            get { return grks.rho; }
        }

        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same time.
        /// This sequence number does not have to be unique and does not need to be sequential.
        /// Sequence can range from 0 to MaxSequence.
        /// </summary>
        public int Sequence
        {
            get { return grks.sequence; }
        }

        /// <summary>
        /// Returns option theta.
        /// Theta is the first derivative of an option price by a number of days to expiration.
        /// </summary>
        public double Theta
        {
            get { return grks.theta; }
        }

        /// <summary>
        /// Returns timestamp of this event.
        /// The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        public long TimeStamp
        {
            get { return grks.time; }
        }

        /// <summary>
        /// Returns UTC date and time of this event.
        /// </summary>
        public DateTime Time
        {
            get { return TimeConverter.ToUtcDateTime(TimeStamp); }
        }

        /// <summary>
        /// Returns option vega.
        /// Vega is the first derivative of an option price by percentage volatility.
        /// </summary>
        public double Vega
        {
            get { return grks.vega; }
        }

        /// <summary>
        /// Returns Black-Scholes implied volatility of the option.
        /// </summary>
        public double Volatility
        {
            get { return grks.volatility; }
        }

        /// <summary>
        /// Returns unique per-symbol index of this event.
        /// The index is composed of Time and Sequence.
        /// </summary>
        public long Index
        {
            get { return grks.index; }
        }

        /// <summary>
        /// Gets transactional event flags.
        /// See "Event Flags" section from <see cref="IndexedEvent"/>.
        /// </summary>
        public int EventFlags
        {
            //TODO: implement
            get; set;
        }

        #endregion
    }
}
