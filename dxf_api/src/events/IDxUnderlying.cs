#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Underlying event is a snapshot of computed values that are available for an
    /// option underlying symbol based on the option prices on the market. It
    /// represents the most recent information that is available about the corresponding
    /// values on the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("Underlying")]
    public interface IDxUnderlying : IDxMarketEvent, IDxLastingEvent<string>
    {
        /// <summary>
        /// Returns 30-day implied volatility for this underlying based on VIX methodology.
        /// </summary>
        double Volatility { get; }
        /// <summary>
        /// Returns front month implied volatility for this underlying based on VIX methodology.
        /// </summary>
        double FrontVolatility { get; }
        /// <summary>
        /// Returns back month implied volatility for this underlying based on VIX methodology.
        /// </summary>
        double BackVolatility { get; }
        /// <summary>
        /// Returns call options traded volume for a day
        /// </summary>
        double CallVolume { get; }
        /// <summary>
        /// Returns put options traded volume for a day
        /// </summary>
        double PutVolume { get; }
        /// <summary>
        /// Returns options traded volume for a day
        /// </summary>
        double OptionVolume { get; }
        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        double PutCallRatio { get; }
    }
}
