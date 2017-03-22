#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.events;
using com.dxfeed.native.api;
using System;
using System.Globalization;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Series event is a snapshot of computed values that are available for all
    /// option series for a given underlying symbol based on the option prices on
    /// the market. It represents the most recent information that is available
    /// about the corresponding values on the market at any given moment of time.
    /// </summary>
    public class NativeSeries : MarketEventImpl, IDxSeries
    {
        internal unsafe NativeSeries(DxSeries* series, string symbol) : base(symbol)
        {
            DxSeries s = *series;

            Dividend = s.dividend;
            Expiration = s.expiration;
            ForwardPrice = s.forward_price;
            Interest = s.interest;
            PutCallRatio = s.put_call_ratio;
            Sequence = s.sequence;
            Volatility = s.volatility;
            Index = s.index;
            EventFlags = s.event_flags;
        }

        internal NativeSeries(IDxSeries s) : base(s.EventSymbol)
        {
            Dividend = s.Dividend;
            Expiration = s.Expiration;
            ForwardPrice = s.ForwardPrice;
            Interest = s.Interest;
            PutCallRatio = s.PutCallRatio;
            Sequence = s.Sequence;
            Volatility = s.Volatility;
            Index = s.Index;
            EventFlags = s.EventFlags;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Series: {{{0}, " +
                "Expiration: {1}, Sequence: {2}, Volatility: {3}, " +
                "PutCallRatio: {4}, ForwardPrice: {5}, Dividend: {6}, Interest: {7}, Index: {8}}}",
                EventSymbol, Expiration, Sequence, Volatility, PutCallRatio,
                ForwardPrice, Dividend, Interest, Index);
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeSeries(this);
        }
        #endregion

        #region Implementation of IDxSeries

        /// <summary>
        /// Returns implied simple dividend return of the corresponding option series.
        /// See the model section for an explanation this simple dividend return \( Q(\tau) \).
        /// </summary>
        public double Dividend
        {
            get; private set;
        }

        /// <summary>
        /// Returns day id of expiration.
        /// Example: DayUtil.GetDayIdByYearMonthDay(20090117). Most significant
        /// 32 bits of Index contain day id of expiration, so changing Index also
        /// changes day id of expiration.
        /// </summary>
        public int Expiration
        {
            get; private set;
        }

        /// <summary>
        /// Returns implied forward price for this option series.
        /// </summary>
        public double ForwardPrice
        {
            get; private set;
        }

        /// <summary>
        /// Returns implied simple interest return of the corresponding option series.
        /// See the model section for an explanation this simple interest return \( R(\tau) \).
        /// </summary>
        public double Interest
        {
            get; private set;
        }

        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double PutCallRatio
        {
            get; private set;
        }

        /// <summary>
        /// Returns sequence of this series
        /// </summary>
        public int Sequence
        {
            get; private set;
        }

        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double Volatility
        {
            get; private set;
        }

        /// <summary>
        /// Returns unique per-symbol index of this series.
        /// Most significant 32 bits of index contain Expiration value and Sequence.
        /// </summary>
        public long Index
        {
            get; private set;
        }

        /// <summary>
        /// Gets transactional event flags.
        /// See "Event Flags" section from <see cref="IndexedEvent"/>.
        /// </summary>
        public EventFlag EventFlags
        {
            get; set;
        }

        #endregion
    }
}
