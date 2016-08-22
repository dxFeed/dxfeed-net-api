using System;
using System.Collections.Generic;

namespace com.dxfeed.ipf.option {
    class OptionChain<T> : ICloneable {

        private readonly SortedDictionary<OptionSeries<T>, OptionSeries<T>> seriesMap = new SortedDictionary<OptionSeries<T>, OptionSeries<T>>();

        internal OptionChain(string symbol) {
            Symbol = symbol;
        }

        /// <summary>
        /// Symbol(product or underlying) of this option chain.
        /// </summary>
        /// <value>Gets symbol (product or underlying) of this option chain.</value>
        public string Symbol {
            get;
            private set;
        }

        public object Clone()
        {
            OptionChain<T> clone = new OptionChain<T>(Symbol);
            foreach (var series in seriesMap.Values) {
                OptionSeries<T> seriesClone = (OptionSeries<T>)series.Clone();
                clone.seriesMap.Add(seriesClone, seriesClone);
            }
            return clone;
        }

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
