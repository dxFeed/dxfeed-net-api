/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Series event is a snapshot of computed values that are available for all
    /// option series for a given underlying symbol based on the option prices on
    /// the market. It represents the most recent information that is available
    /// about the corresponding values on the market at any given moment of time.
    /// </summary>
    public interface IDxSeries : IDxMarketEvent
    {
        /// <summary>
        /// Returns day id of expiration.
        /// Example: DayUtil.getDayIdByYearMonthDay(20090117). Most significant
        /// 32 bits of Index contain day id of expiration, so changing Index also
        /// changes day id of expiration.
        /// </summary>
        int Expiration { get; }
        /// <summary>
        /// 
        /// </summary>
        int Sequence { get; }
        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        double Volatility { get; }
        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        double PutCallRatio { get; }
        /// <summary>
        /// Returns implied forward price for this option series.
        /// </summary>
        double ForwardPrice { get; }
        /// <summary>
        /// Returns implied simple dividend return of the corresponding option series.
        /// See the model section for an explanation this simple dividend return \( Q(\tau) \).
        /// </summary>
        double Dividend { get; }
        /// <summary>
        /// Returns implied simple interest return of the corresponding option series.
        /// See the model section for an explanation this simple interest return \( R(\tau) \).
        /// </summary>
        double Interest { get; }
    }
}
