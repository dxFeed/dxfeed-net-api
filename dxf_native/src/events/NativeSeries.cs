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
    /// Series event is a snapshot of computed values that are available for all
    /// option series for a given underlying symbol based on the option prices on
    /// the market. It represents the most recent information that is available
    /// about the corresponding values on the market at any given moment of time.
    /// </summary>
    public class NativeSeries : MarketEvent, IDxSeries
    {
        private readonly DxSeries s;

        internal unsafe NativeSeries(DxSeries* s, string symbol) : base(symbol)
        {
            this.s = *s;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Series: {{{0}, " +
                "Expiration: {1}, Sequence: {2}, Volatility: {3}, " +
                "PutCallRatio: {4}, ForwardPrice: {5}, Dividend: {6}, Interest: {7}, Index: {8}}}",
                EventSymbol, Expiration, Sequence, Volatility, PutCallRatio,
                ForwardPrice, Dividend, Interest, Index);
        }

        #region Implementation of IDxSeries

        /// <summary>
        /// Returns implied simple dividend return of the corresponding option series.
        /// See the model section for an explanation this simple dividend return \( Q(\tau) \).
        /// </summary>
        public double Dividend
        {
            get { return s.dividend; }
        }

        /// <summary>
        /// Returns day id of expiration.
        /// Example: DayUtil.getDayIdByYearMonthDay(20090117). Most significant
        /// 32 bits of Index contain day id of expiration, so changing Index also
        /// changes day id of expiration.
        /// </summary>
        public int Expiration
        {
            get { return s.expiration; }
        }

        /// <summary>
        /// Returns implied forward price for this option series.
        /// </summary>
        public double ForwardPrice
        {
            get { return s.forward_price; }
        }

        /// <summary>
        /// Returns implied simple interest return of the corresponding option series.
        /// See the model section for an explanation this simple interest return \( R(\tau) \).
        /// </summary>
        public double Interest
        {
            get { return s.interest; }
        }

        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double PutCallRatio
        {
            get { return s.put_call_ratio; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Sequence
        {
            get { return s.sequence; }
        }

        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double Volatility
        {
            get { return s.volatility; }
        }

        /// <summary>
        /// Returns unique per-symbol index of this series.
        /// Most significant 32 bits of index contain Expiration value and Sequence,
        /// so changing Expiration also changes index.
        /// </summary>
        public long Index
        {
            get { return s.index; }
        }

        #endregion
    }
}
