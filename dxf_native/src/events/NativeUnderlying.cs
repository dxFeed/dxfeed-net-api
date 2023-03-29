#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.Globalization;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Underlying event is a snapshot of computed values that are available for an
    /// option underlying symbol based on the option prices on the market. It
    /// represents the most recent information that is available about the corresponding
    /// values on the market at any given moment of time.
    /// </summary>
    public class NativeUnderlying : MarketEventImpl, IDxUnderlying
    {
        internal unsafe NativeUnderlying(DxUnderlying* underlying, string symbol) : base(symbol)
        {
            var u = *underlying;

            Volatility = u.volatility;
            FrontVolatility = u.front_volatility;
            BackVolatility = u.back_volatility;
            CallVolume = u.call_volume;
            PutVolume = u.put_volume;
            OptionVolume = u.option_volume;
            PutCallRatio = u.put_call_ratio;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="u">The original Underlying event</param>
        public NativeUnderlying(IDxUnderlying u) : base(u.EventSymbol)
        {
            Volatility = u.Volatility;
            FrontVolatility = u.FrontVolatility;
            BackVolatility = u.BackVolatility;
            CallVolume = u.CallVolume;
            PutVolume = u.PutVolume;
            OptionVolume = u.OptionVolume;
            PutCallRatio = u.PutCallRatio;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public NativeUnderlying()
        {
            
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Underlying: {{{0}, Volatility: {1}, FrontVolatility: {2}, BackVolatility: {3}, CallVolume: {4}, " +
                "PutVolume: {5}, OptionVolume: {6}, PutCallRatio: {7}}}",
                EventSymbol, Volatility, FrontVolatility, BackVolatility, CallVolume, PutVolume, OptionVolume, 
                PutCallRatio
            );
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeUnderlying(this);
        }

        #endregion

        #region Implementation of IDxUnderlying

        /// <summary>
        /// Returns 30-day implied volatility for this underlying based on VIX methodology.
        /// </summary>
        public double Volatility { get; set; }
        /// <summary>
        /// Returns front month implied volatility for this underlying based on VIX methodology.
        /// </summary>
        public double FrontVolatility { get; set; }
        /// <summary>
        /// Returns back month implied volatility for this underlying based on VIX methodology.
        /// </summary>
        public double BackVolatility { get; set; }
        /// <summary>
        /// Returns call options traded volume for a day
        /// </summary>
        public double CallVolume { get; set; }
        /// <summary>
        /// Returns put options traded volume for a day
        /// </summary>
        public double PutVolume { get; set; }
        /// <summary>
        /// Returns options traded volume for a day
        /// </summary>
        public double OptionVolume { get; set; }
        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double PutCallRatio { get; set; }

        #endregion
    }
}
