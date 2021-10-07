#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

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
    ///     Series event is a snapshot of computed values that are available for all
    ///     option series for a given underlying symbol based on the option prices on
    ///     the market. It represents the most recent information that is available
    ///     about the corresponding values on the market at any given moment of time.
    /// </summary>
    public class NativeSeries : MarketEventImpl, IDxSeries
    {
        internal unsafe NativeSeries(DxSeries* series, string symbol) : base(symbol)
        {
            var s = *series;

            EventFlags = s.event_flags;
            Index = s.index;
            Time = TimeConverter.ToUtcDateTime(s.time);
            Sequence = s.sequence;
            Expiration = s.expiration;
            Volatility = s.volatility;
            CallVolume = s.call_volume;
            PutVolume = s.put_volume;
            OptionVolume = s.option_volume;
            PutCallRatio = s.put_call_ratio;
            ForwardPrice = s.forward_price;
            Dividend = s.dividend;
            Interest = s.interest;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="s">The original Series event</param>
        public NativeSeries(IDxSeries s) : base(s.EventSymbol)
        {
            EventFlags = s.EventFlags;
            Index = s.Index;
            Time = s.Time;
            Sequence = s.Sequence;
            Expiration = s.Expiration;
            Volatility = s.Volatility;
            CallVolume = s.CallVolume;
            PutVolume = s.PutVolume;
            OptionVolume = s.OptionVolume;
            PutCallRatio = s.PutCallRatio;
            ForwardPrice = s.ForwardPrice;
            Dividend = s.Dividend;
            Interest = s.Interest;
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        public NativeSeries()
        {
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeSeries(this);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Series: {{{0}, EventFlags: 0x{1:x2}, Index: {2:x16}, Time: {3:o}, Sequence: {4}, Expiration: {5}, " +
                "Volatility: {6}, CallVolume: {7}, PutVolume: {8}, OptionVolume: {9}, PutCallRatio: {10}, " +
                "ForwardPrice: {11}, Dividend: {12}, Interest: {13}}}",
                EventSymbol, (int)EventFlags, Index, Time, Sequence, Expiration, Volatility, CallVolume, PutVolume,
                OptionVolume, PutCallRatio, ForwardPrice, Dividend, Interest
            );
        }

        #region Implementation of IDxSeries

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
        ///     Returns date time of this order.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///     Returns sequence of this series
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        ///     Returns day id of expiration.
        ///     Example: DayUtil.GetDayIdByYearMonthDay(20090117). Most significant
        ///     32 bits of Index contain day id of expiration, so changing Index also
        ///     changes day id of expiration.
        /// </summary>
        public int Expiration { get; set; }

        /// <summary>
        ///     Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double Volatility { get; set; }

        /// <summary>
        ///     Returns call options traded volume for a day
        /// </summary>
        public double CallVolume { get; set; }

        /// <summary>
        ///     Returns put options traded volume for a day
        /// </summary>
        public double PutVolume { get; set; }

        /// <summary>
        ///     Returns options traded volume for a day
        /// </summary>
        public double OptionVolume { get; set; }

        /// <summary>
        ///     Returns ratio of put traded volume to call traded volume for a day.
        /// </summary>
        public double PutCallRatio { get; set; }

        /// <summary>
        ///     Returns implied forward price for this option series.
        /// </summary>
        public double ForwardPrice { get; set; }

        /// <summary>
        ///     Returns implied simple dividend return of the corresponding option series.
        ///     See the model section for an explanation this simple dividend return \( Q(\tau) \).
        /// </summary>
        public double Dividend { get; set; }

        /// <summary>
        ///     Returns implied simple interest return of the corresponding option series.
        ///     See the model section for an explanation this simple interest return \( R(\tau) \).
        /// </summary>
        public double Interest { get; set; }

        #endregion
    }
}