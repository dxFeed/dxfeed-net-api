/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

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
    public class NativeUnderlying : MarketEvent, IDxUnderlying
    {
        private readonly DxUnderlying u;

        internal unsafe NativeUnderlying(DxUnderlying* u, string symbol) : base(symbol)
        {
            this.u = *u;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Underlying: {{{0}, " +
                "Volatility: {1}, FrontVolatility: {2}, BackVolatility: {3}, PutCallRatio: {4}}}",
                EventSymbol, Volatility, FrontVolatility, BackVolatility, PutCallRatio);
        }

        #region Implementation of IDxUnderlying

        /// <summary>
        /// Returns back month implied volatility for this underlying based on VIX methodology.
        /// </summary>
        public double BackVolatility
        {
            get { return u.back_volatility; }
        }

        /// <summary>
        /// Returns front month implied volatility for this underlying based on VIX methodology.
        /// </summary>
        public double FrontVolatility
        {
            get { return u.front_volatility; }
        }

        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double PutCallRatio
        {
            get { return u.put_call_ratio; }
        }

        /// <summary>
        /// Returns 30-day implied volatility for this underlying based on VIX methodology.
        /// </summary>
        public double Volatility
        {
            get { return u.volatility; }
        }

        #endregion
    }
}
