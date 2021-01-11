#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;

namespace com.dxfeed.ipf.option
{
    /// <summary>
    /// Set of option series for a single product or underlying symbol.
    /// </summary>
    /// <typeparam name="T">The type of option instrument instances.</typeparam>
    public sealed class OptionChain : ICloneable
    {
        private readonly SortedDictionary<OptionSeries, OptionSeries> seriesMap = new SortedDictionary<OptionSeries, OptionSeries>();

        internal OptionChain(string symbol)
        {
            Symbol = symbol;
        }

        /// <summary>
        /// Symbol (product or underlying) of this option chain.
        /// </summary>
        /// <value>Gets symbol (product or underlying) of this option chain.</value>
        public string Symbol
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a shall copy of this option chain.
        /// All series are copied(cloned) themselves, but option instrument instances are shared with original.
        /// </summary>
        /// <returns>Returns a shall copy of this option chain.</returns>
        public object Clone()
        {
            OptionChain clone = new OptionChain(Symbol);
            foreach (var series in seriesMap.Values)
            {
                OptionSeries seriesClone = (OptionSeries)series.Clone();
                clone.seriesMap.Add(seriesClone, seriesClone);
            }
            return clone;
        }

        /// <summary>
        /// Returns a sorted set of option series of this option chain.
        /// </summary>
        /// <returns>sorted set of option series of this option chain.</returns>
        public SortedDictionary<OptionSeries, OptionSeries>.KeyCollection GetSeries()
        {
            return seriesMap.Keys;
        }

        internal void AddOption(OptionSeries series, bool isCall, double strike, InstrumentProfile option)
        {
            OptionSeries os;
            if (!seriesMap.TryGetValue(series, out os))
            {
                os = new OptionSeries(series);
                seriesMap.Add(os, os);
            }
            os.AddOption(isCall, strike, option);
        }
    }
}
