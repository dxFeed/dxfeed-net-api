#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Series event is a snapshot of computed values that are available for all
    /// option series for a given underlying symbol based on the option prices on
    /// the market. It represents the most recent information that is available
    /// about the corresponding values on the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("Series")]
    public interface IDxSeries : IDxMarketEvent, IDxIndexedEvent<string>
    {
        /// <summary>
        ///  Returns date time of this order.
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        /// Returns sequence of this series
        /// </summary>
        int Sequence { get; }
        /// <summary>
        /// Returns day id of expiration.
        /// Example: DayUtil.GetDayIdByYearMonthDay(20090117). Most significant
        /// 32 bits of Index contain day id of expiration, so changing Index also
        /// changes day id of expiration.
        /// </summary>
        int Expiration { get; }
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
