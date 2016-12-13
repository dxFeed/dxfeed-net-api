/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Greeks event is a snapshot of the option price, Black-Scholes volatility and greeks.
    /// It represents the most recent information that is available about the corresponding
    /// values on the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("Greeks")]
    public interface IDxGreeks : IDxMarketEvent
    {
        /// <summary>
        /// Maximum allowed sequence value. Constant field value.
        /// </summary>
        int MaxSequence { get; }
        /// <summary>
        /// Returns date time of this event.
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        /// Returns sequence number of this event to distinguish events that have the same time.
        /// This sequence number does not have to be unique and does not need to be sequential.
        /// Sequence can range from 0 to MaxSequence.
        /// </summary>
        int Sequence { get; }
        /// <summary>
        /// Returns option market price.
        /// </summary>
        double GreeksPrice { get; }
        /// <summary>
        /// Returns Black-Scholes implied volatility of the option.
        /// </summary>
        double Volatility { get; }
        /// <summary>
        /// Return option delta.
        /// Delta is the first derivative of an option price by an underlying price.
        /// </summary>
        double Delta { get; }
        /// <summary>
        /// Returns option gamma.
        /// Gamma is the second derivative of an option price by an underlying price.
        /// </summary>
        double Gamma { get; }
        /// <summary>
        /// Returns option theta.
        /// Theta is the first derivative of an option price by a number of days to expiration.
        /// </summary>
        double Theta { get; }
        /// <summary>
        /// Returns option rho.
        /// Rho is the first derivative of an option price by percentage interest rate.
        /// </summary>
        double Rho { get; }
        /// <summary>
        /// Returns option vega.
        /// Vega is the first derivative of an option price by percentage volatility.
        /// </summary>
        double Vega { get; }
        /// <summary>
        /// Returns unique per-symbol index of this event.
        /// The index is composed of Time and Sequence.
        /// </summary>
        long Index { get; }
    }
}
