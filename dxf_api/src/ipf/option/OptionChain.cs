using System;
using System.Collections.Generic;

namespace com.dxfeed.ipf.option {
    /// <summary>
    /// Set of option series for a single product or underlying symbol.
    /// </summary>
    /// <typeparam name="T">The type of option instrument instances.</typeparam>
    class OptionChain<T> : ICloneable {

        private readonly SortedDictionary<OptionSeries<T>, OptionSeries<T>> seriesMap = new SortedDictionary<OptionSeries<T>, OptionSeries<T>>();

        internal OptionChain(string symbol) {
            Symbol = symbol;
        }

        /// <summary>
        /// Symbol (product or underlying) of this option chain.
        /// </summary>
        /// <value>Gets symbol (product or underlying) of this option chain.</value>
        public string Symbol {
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
            OptionChain<T> clone = new OptionChain<T>(Symbol);
            foreach (var series in seriesMap.Values) {
                OptionSeries<T> seriesClone = (OptionSeries<T>)series.Clone();
                clone.seriesMap.Add(seriesClone, seriesClone);
            }
            return clone;
        }

        /// <summary>
        /// Returns a sorted set of option series of this option chain.
        /// </summary>
        /// <returns>sorted set of option series of this option chain.</returns>
        public SortedDictionary<OptionSeries<T>, OptionSeries<T>>.KeyCollection GetSeries() {
            return seriesMap.Keys;
        }

        internal void AddOption(OptionSeries<T> series, bool isCall, double strike, T option) {
            OptionSeries<T> os;
            if (!seriesMap.TryGetValue(series, out os)) {
                os = new OptionSeries<T>(series);
                seriesMap.Add(os, os);
            }
            os.AddOption(isCall, strike, option);
        }
    }
}
