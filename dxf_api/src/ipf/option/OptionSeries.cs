using System;
using System.Collections.Generic;

namespace com.dxfeed.ipf.option {
    class OptionSeries<T> : ICloneable {

        private readonly SortedDictionary<double, T> calls = new SortedDictionary<double, T>();
        private readonly SortedDictionary<double, T> puts = new SortedDictionary<double, T>();

        private List<double> strikes;

        internal OptionSeries() {
            AdditionalUnderlyings = "";
            Mmy = "";
            OptionType = "";
            ExpirationStyle = "";
            SettlementStyle = "";
            Cfi = "";
        }

        internal OptionSeries(OptionSeries<T> series) {
            Expiration = series.Expiration;
            LastTrade = series.LastTrade;
            Multiplier = series.Multiplier;
            Spc = series.Spc;
            AdditionalUnderlyings = series.AdditionalUnderlyings;
            Mmy = series.Mmy;
            OptionType = series.OptionType;
            ExpirationStyle = series.ExpirationStyle;
            SettlementStyle = series.SettlementStyle;
            Cfi = series.Cfi;
        }

        public object Clone() {
            OptionSeries<T> clone = new OptionSeries<T>(this);
            foreach (var call in calls) {
                clone.calls.Add(call.Key, call.Value);
            }
            foreach (var put in puts) {
                clone.puts.Add(put.Key, put.Value);
            }
            return clone;
        }

        /// <summary>
        /// Day id of expiration.
        /// Example: {@link DayUtil#getDayIdByYearMonthDay DayUtil.getDayIdByYearMonthDay}(20090117).
        /// </summary>
        /// <value>Gets day id of expiration.</value>
        public int Expiration {
            get;
            internal set;
        }

        /// <summary>
        /// Day id of last trading day.
        /// Example: {@link DayUtil#getDayIdByYearMonthDay DayUtil.getDayIdByYearMonthDay}(20090116).
        /// </summary>
        /// <value>Gets day id of last trading day.</value>
        public int LastTrade {
            get;
            internal set;
        }

        /// <summary>
        /// Market value multiplier.
        /// Example: 100, 33.2.
        /// </summary>
        /// <value>Gets market value multiplier.</value>
        public double Multiplier {
            get;
            internal set;
        }

        /// <summary>
        /// Shares per contract for options.
        /// Example: 1, 100.
        /// </summary>
        /// <value>Gets shares per contract for options.</value>
        public double Spc {
            get;
            internal set;
        }

        /// <summary>
        /// Additional underlyings for options, including additional cash.
        /// It shall use following format:
        /// <pre>
        ///     &lt;VALUE> ::= &lt;empty> | &lt;LIST>
        ///     &lt;LIST> ::= &lt;AU> | &lt;AU> &lt;semicolon> &lt;space> &lt;LIST>
        ///     &lt;AU> ::= &lt;UNDERLYING> &lt;space> &lt;SPC> </pre>
        /// the list shall be sorted by &lt;UNDERLYING>.
        /// Example: "SE 50", "FIS 53; US$ 45.46".
        /// </summary>
        /// <value>Gets additional underlyings for options, including additional cash.</value>
        public string AdditionalUnderlyings {
            get;
            internal set;
        }

        /// <summary>
        /// Maturity month-year as provided for corresponding FIX tag (200).
        /// It can use several different formats depending on data source:
        /// <ul>
        /// <li>YYYYMM – if only year and month are specified
        /// <li>YYYYMMDD – if full date is specified
        /// <li>YYYYMMwN – if week number(within a month) is specified
        /// </ul>
        /// </summary>
        /// <value>Gets maturity month-year as provided for corresponding FIX tag(200).</value>
        public string Mmy {
            get;
            internal set;
        }

        /// <summary>
        /// Type of option.
        /// It shall use one of following values:
        /// <ul>
        /// <li>STAN = Standard Options
        /// <li>LEAP = Long-term Equity AnticiPation Securities
        /// <li>SDO = Special Dated Options
        /// <li>BINY = Binary Options
        /// <li>FLEX = FLexible EXchange Options
        /// <li>VSO = Variable Start Options
        /// <li>RNGE = Range
        /// </ul>
        /// </summary>
        /// <value>Gets type of option.</value>
        public string OptionType {
            get;
            internal set;
        }

        /// <summary>
        /// Expiration cycle style, such as "Weeklys", "Quarterlys".
        /// </summary>
        /// <value>gets expiration cycle style.</value>
        public string ExpirationStyle {
            get;
            internal set;
        }

        /// <summary>
        /// Settlement price determination style, such as "Open", "Close".
        /// </summary>
        /// <value>Gets settlement price determination style.</value>
        public string SettlementStyle {
            get;
            internal set;
        }

        /// <summary>
        /// Classification of Financial Instruments code of this option series.
        /// It shall use six-letter CFI code from ISO 10962 standard.
        /// It is allowed to use 'X' extensively and to omit trailing letters(assumed to be 'X').
        /// See<a href="http://en.wikipedia.org/wiki/ISO_10962"> ISO 10962 on Wikipedia</a>.
        /// It starts with "OX" as both {@link #getCalls() calls} and {@link #getPuts()} puts} are stored in a series.
        /// </summary>
        /// <value>Gets CFI code.</value>
        public string Cfi {
            get;
            internal set;
        }

        /// <summary>
        /// Java's Map like put method.
        /// Associates the specified value with the specified key in this map (optional operation).
        /// If the map previously contained a mapping for the key, the old value is replaced by the specified value.
        /// </summary>
        /// <param name="map">map</param>
        /// <param name="key">key with which the specified value is to be associated</param>
        /// <param name="value">key with which the specified value is to be associated</param>
        /// <returns>the previous value associated with key, or default(T) if there was no mapping for key</returns>
        private T Put(SortedDictionary<double, T> map, double key, T value) {
            if (!map.ContainsKey(key)) {
                map.Add(key, value);
                return default(T);
            }
            T oldValue = map[key];
            map[key] = value;
            return oldValue;
        }

        internal void AddOption(bool isCall, double strike, T option) {
            SortedDictionary<double, T> map = isCall ? calls : puts;
            if (Put(map, strike, option) == null) {
                strikes = null;
            }
        }
    }
}
