/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
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
    [EventTypeAttribute("TheoPrice")]
    public interface IDxTheoPrice : IDxMarketEvent, LastingEvent<string>
    {
        /// <summary>
        /// Returns date time of the last theo price computation.
        /// </summary>
        DateTime TheoTime { get; }
        /// <summary>
        /// Returns theoretical option price.
        /// </summary>
        double TheoPrice { get; }
        /// <summary>
        /// Returns underlying price at the time of theo price computation.
        /// </summary>
        double TheoUnderlyingPrice { get; }
        /// <summary>
        /// Returns delta of the theoretical price.
        /// Delta is the first derivative of the theoretical price by the underlying price.
        /// </summary>
        double TheoDelta { get; }
        /// <summary>
        /// Returns gamma of the theoretical price.
        /// Gamma is the second derivative of the theoretical price by the underlying price.
        /// </summary>
        double TheoGamma { get; }
        /// <summary>
        /// Returns implied simple dividend return of the corresponding option series.
        /// See the model section for an explanation this simple dividend return \(Q(\tau)\).
        /// </summary>
        double TheoDividend { get; }
        /// <summary>
        /// Returns implied simple interest return of the corresponding option series.
        /// See the model section for an explanation this simple interest return \(R(\tau)\).
        /// </summary>
        double TheoInterest { get; }
    }
}
