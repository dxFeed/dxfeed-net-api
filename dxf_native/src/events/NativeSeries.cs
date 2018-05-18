#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.data;
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

            EventFlags = s.event_flags;
            Index = s.index;
            Expiration = s.expiration;
            Volatility = s.volatility;
            PutCallRatio = s.put_call_ratio;
            ForwardPrice = s.forward_price;
            Dividend = s.dividend;
            Interest = s.interest;
        }

        internal NativeSeries(IDxSeries s) : base(s.EventSymbol)
        {
            EventFlags = s.EventFlags;
            Index = s.Index;
            Expiration = s.Expiration;
            Volatility = s.Volatility;
            PutCallRatio = s.PutCallRatio;
            ForwardPrice = s.ForwardPrice;
            Dividend = s.Dividend;
            Interest = s.Interest;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Series: {{{0}, "                        +
                "EventFlags: 0x{1:x2}, Index: {2:x16}, " +
                "Expiration: {3}, "                      +
                "Volatility: {4}, PutCallRatio: {5}, "   +
                "ForwardPrice: {6}, "                    +
                "Dividend: {7}, Interest: {8}"           +
                 "}}",
                EventSymbol,
                EventFlags, Index,
                Expiration,
                Volatility, PutCallRatio,
                ForwardPrice,
                Dividend, Interest
            );
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeSeries(this);
        }
        #endregion

        #region Implementation of IDxSeries

        /// <summary>
        ///     Returns source of this event.
        /// </summary>
        /// <returns>Source of this event.</returns>
        public IndexedEventSource Source { get { return IndexedEventSource.DEFAULT; }  }
        /// <summary>
        ///    Gets or sets transactional event flags.
        ///    See "Event Flags" section from <see cref="IndexedEvent"/>.
        /// </summary>
        public EventFlag EventFlags { get; set; }
        /// <summary>
        ///     Gets unique per-symbol index of this event.
        /// </summary>
        public long Index { get; private set; }
        /// <summary>
        /// Returns day id of expiration.
        /// Example: DayUtil.getDayIdByYearMonthDay(20090117). Most significant
        /// 32 bits of Index contain day id of expiration, so changing Index also
        /// changes day id of expiration.
        /// </summary>
        public int Expiration { get; private set; }
        /// <summary>
        /// Returns sequence of this series
        /// </summary>
        public int Sequence { get; private set; }
        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double Volatility { get; private set; }
        /// <summary>
        /// Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double PutCallRatio { get; private set; }
        /// <summary>
        /// Returns implied forward price for this option series.
        /// </summary>
        public double ForwardPrice { get; private set; }
        /// <summary>
        /// Returns implied simple dividend return of the corresponding option series.
        /// See the model section for an explanation this simple dividend return \( Q(\tau) \).
        /// </summary>
        public double Dividend { get; private set; }
        /// <summary>
        /// Returns implied simple interest return of the corresponding option series.
        /// See the model section for an explanation this simple interest return \( R(\tau) \).
        /// </summary>
        public double Interest { get; private set; }

        #endregion
    }
}
