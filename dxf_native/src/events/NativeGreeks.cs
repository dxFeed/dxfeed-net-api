#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

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
    ///     Greeks event is a snapshot of the option price, Black-Scholes volatility and greeks.
    ///     It represents the most recent information that is available about the corresponding
    ///     values on the market at any given moment of time.
    /// </summary>
    public class NativeGreeks : MarketEventImpl, IDxGreeks
    {
        internal unsafe NativeGreeks(DxGreeks* g, string symbol) : base(symbol)
        {
            var greeks = *g;

            EventFlags = greeks.event_flags;
            Index = greeks.index;
            Time = TimeConverter.ToUtcDateTime(greeks.time);
            Price = greeks.price;
            Volatility = greeks.volatility;
            Delta = greeks.delta;
            Gamma = greeks.gamma;
            Rho = greeks.rho;
            Theta = greeks.theta;
            Vega = greeks.vega;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="greeks">The original Greeks event</param>
        public NativeGreeks(IDxGreeks greeks) : base(greeks.EventSymbol)
        {
            EventFlags = greeks.EventFlags;

            EventFlags = greeks.EventFlags;
            Index = greeks.Index;
            Time = greeks.Time;
            Price = greeks.Price;
            Volatility = greeks.Volatility;
            Delta = greeks.Delta;
            Gamma = greeks.Gamma;
            Rho = greeks.Rho;
            Theta = greeks.Theta;
            Vega = greeks.Vega;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public NativeGreeks()
        {
            
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeGreeks(this);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Greeks: {{{0}, " +
                "EventFlags: 0x{1:x2}, Index: {2:x16}, " +
                "Time: {3:o}, " +
                "Price: {4}, Volatility: {5}, " +
                "Delta: {6}, Gamma: {7}, Rho: {8}, " +
                "Theta: {9}, Vega: {10}" +
                "}}",
                EventSymbol,
                (int)EventFlags, Index,
                Time,
                Price, Volatility,
                Delta, Gamma, Rho,
                Theta, Vega
            );
        }

        #region Implementation of IDxGreeks

        /// <summary>
        ///     Returns source of this event.
        /// </summary>
        /// <returns>Source of this event.</returns>
        public IndexedEventSource Source => IndexedEventSource.DEFAULT;

        /// <summary>
        ///     Gets or sets transactional event flags.
        ///     See "Event Flags" section from <see cref="IDxIndexedEvent" />.
        /// </summary>
        public EventFlag EventFlags { get; set; }

        /// <summary>
        ///     Gets unique per-symbol index of this event.
        /// </summary>
        public long Index { get; set; }

        /// <summary>
        ///     Returns timestamp of this event.
        ///     The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        public long TimeStamp => TimeConverter.ToUnixTime(Time);

        /// <summary>
        ///     Returns UTC date and time of this event.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///     Returns option market price.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Returns Black-Scholes implied volatility of the option.
        /// </summary>
        public double Volatility { get; set; }

        /// <summary>
        ///     Return option delta.
        ///     Delta is the first derivative of an option price by an underlying price.
        /// </summary>
        public double Delta { get; set; }

        /// <summary>
        ///     Returns option gamma.
        ///     Gamma is the second derivative of an option price by an underlying price.
        /// </summary>
        public double Gamma { get; set; }

        /// <summary>
        ///     Returns option theta.
        ///     Theta is the first derivative of an option price by a number of days to expiration.
        /// </summary>
        public double Theta { get; set; }

        /// <summary>
        ///     Returns option rho.
        ///     Rho is the first derivative of an option price by percentage interest rate.
        /// </summary>
        public double Rho { get; set; }

        /// <summary>
        ///     Returns option vega.
        ///     Vega is the first derivative of an option price by percentage volatility.
        /// </summary>
        public double Vega { get; set; }

        #endregion
    }
}