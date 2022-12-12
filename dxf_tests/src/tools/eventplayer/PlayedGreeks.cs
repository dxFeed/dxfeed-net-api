#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using System;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Greeks event implementation for test event player.
    /// </summary>
    internal class PlayedGreeks : IPlayedEvent<DxTestGreeks>, IDxGreeks
    {
        internal PlayedGreeks(string symbol, EventFlag event_flags, long index, long time,
                                double price, double volatility,
                                double delta, double gamma, double theta, double rho, double vega)
        {
            EventSymbol = symbol;
            EventFlags = event_flags;

            Delta = delta;
            Gamma = gamma;
            Price = price;
            Rho = rho;
            Theta = theta;
            Time = Tools.UnixTimeToDate(time);
            Vega = vega;
            Volatility = volatility;
            Index = index;

            Params = new EventParams(EventFlags, ((ulong)time << 32), 0);
            Data = new DxTestGreeks(event_flags, index, time, price, volatility, delta, gamma, theta, rho, vega);
        }

        /// <summary>
        ///     Creates Greeks event from another object.
        /// </summary>
        /// <param name="greeks">Other Greeks object.</param>
        internal PlayedGreeks(IDxGreeks greeks)
        {
            EventSymbol = greeks.EventSymbol;
            EventFlags = greeks.EventFlags;

            Delta = greeks.Delta;
            Gamma = greeks.Gamma;
            Price = greeks.Price;
            Rho = greeks.Rho;
            Theta = greeks.Theta;
            Time = greeks.Time;
            Vega = greeks.Vega;
            Volatility = greeks.Volatility;
            Index = greeks.Index;

            Params = new EventParams(EventFlags, ((ulong)TimeStamp << 32), 0);
            Data = new DxTestGreeks(EventFlags, Index, Tools.DateToUnixTime(Time), Price, Volatility, Delta, Gamma, Theta, Rho, Vega);
        }

        public DxTestGreeks Data
        {
            get; private set;
        }

        public double Delta
        {
            get; private set;
        }

        public EventFlag EventFlags
        {
            get; set;
        }

        public string EventSymbol
        {
            get; private set;
        }

        public double Gamma
        {
            get; private set;
        }

        public double Price
        {
            get; private set;
        }

        public long Index
        {
            get; private set;
        }

        public EventParams Params
        {
            get; private set;
        }

        public double Rho
        {
            get; private set;
        }

        public int Sequence
        {
            get; private set;
        }

        public IndexedEventSource Source
        {
            get
            {
                return IndexedEventSource.DEFAULT;
            }
        }

        public double Theta
        {
            get; private set;
        }

        public DateTime Time
        {
            get; private set;
        }

        public long TimeStamp
        {
            get { return Tools.DateToUnixTime(Time); }
        }

        public double Vega
        {
            get; private set;
        }

        public double Volatility
        {
            get; private set;
        }

        object IPlayedEvent.Data
        {
            get
            {
                return Data as object;
            }
        }

        object IDxEventType.EventSymbol
        {
            get
            {
                return EventSymbol as object;
            }
        }

        public object Clone()
        {
            return new PlayedGreeks(this);
        }
    }
}
