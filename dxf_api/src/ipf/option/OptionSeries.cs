using com.dxfeed.util;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.dxfeed.ipf.option {
    /// <summary>
    /// Series of call and put options with different strike sharing the same attributes of
    /// expiration, last trading day, spc, multiplies, etc.
    /// </summary>
    /// <typeparam name="T">The type of option instrument instances.</typeparam>
    public sealed class OptionSeries<T> : ICloneable, IComparable<OptionSeries<T>> {

        private readonly SortedDictionary<double, T> calls = new SortedDictionary<double, T>();
        private readonly SortedDictionary<double, T> puts = new SortedDictionary<double, T>();

        internal OptionSeries() {
            AdditionalUnderlyings = "";
            Mmy = "";
            OptionType = "";
            ExpirationStyle = "";
            SettlementStyle = "";
            Cfi = "";
            Strikes = null;
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
            Strikes = null;
        }

        /// <summary>
        /// Returns a shall copy of this option series.
        /// Collections of calls and puts are copied, but option instrument instances are shared with original.
        /// </summary>
        /// <returns>a shall copy of this option series.</returns>
        public object Clone() {
            OptionSeries<T> clone = new OptionSeries<T>(this);
            foreach (var call in Calls) {
                clone.Calls.Add(call.Key, call.Value);
            }
            foreach (var put in Puts) {
                clone.Puts.Add(put.Key, put.Value);
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
        /// A sorted map of all calls from strike to a corresponding option instrument.
        /// </summary>
        public SortedDictionary<double, T> Calls {
            get {
                return calls;
            }
        }

        /// <summary>
        /// A sorted map of all puts from strike to a corresponding option instrument.
        /// </summary>
        public SortedDictionary<double, T> Puts {
            get {
                return puts;
            }
        }

        /// <summary>
        /// A list of all strikes in ascending order.
        /// </summary>
        public List<double> Strikes {
            get {
                if (Strikes == null) {
                    SortedSet<double> strikesSet = new SortedSet<double>(calls.Keys);
                    strikesSet.UnionWith(puts.Keys);
                    Strikes = new List<double>(strikesSet);
                }
                return Strikes;
            }
            private set {
                Strikes = value;
            }
        }

        /// <summary>
        /// Returns n strikes that are centered around a specified strike value.
        /// throws IllegalArgumentException when n < 0.
        /// </summary>
        /// <param name="n">the maximal number of strikes to return.</param>
        /// <param name="strike">the center strike.</param>
        /// <returns>n strikes that are centered around a specified strike value.</returns>
        public List<double> GetNStrikesAround(int n, double strike) {
            if (n < 0)
                throw new ArgumentOutOfRangeException("n", "is negative");
            List<double> strikes = Strikes;
            int i = Strikes.BinarySearch(strike);
            if (i < 0)
                i = -i - 1;
            int from = Math.Max(0, i - n / 2);
            int to = Math.Min(strikes.Count, from + n);
            return strikes.GetRange(from, to - from);
        }

        /// <summary>
        /// Compares this option series to another one by its attributes.
        /// Expiration takes precedence in comparison.
        /// </summary>
        /// <param name="other">another option series to compare with.</param>
        /// <returns>result of comparison.</returns>
        public int CompareTo(OptionSeries<T> other) {
            if (Expiration < other.Expiration)
                return -1;
            if (Expiration > other.Expiration)
                return 1;
            if (LastTrade < other.LastTrade)
                return -1;
            if (LastTrade > other.LastTrade)
                return 1; 
            int i = Multiplier.CompareTo(other.Multiplier);
            if (i != 0)
                return i;
            i = Spc.CompareTo(other.Spc);
            if (i != 0)
                return i;
            i = AdditionalUnderlyings.CompareTo(other.AdditionalUnderlyings);
            if (i != 0)
                return i;
            i = Mmy.CompareTo(other.Mmy);
            if (i != 0)
                return i;
            i = OptionType.CompareTo(other.OptionType);
            if (i != 0)
                return i;
            i = ExpirationStyle.CompareTo(other.ExpirationStyle);
            if (i != 0)
                return i;
            i = SettlementStyle.CompareTo(other.SettlementStyle);
            if (i != 0)
                return i;
            return Cfi.CompareTo(other.Cfi);
        }

        /// <summary>
        /// Indicates whether some other object is equal to this option series by its attributes.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>another object to compare with.</returns>
        public bool equals(object o) {
            if (this == o)
                return true;
            if (!(o is OptionSeries<T>))
                return false;
            OptionSeries<T> that = (OptionSeries <T>)o;
            return Expiration == that.Expiration &&
                LastTrade == that.LastTrade &&
                Multiplier.CompareTo(that.Multiplier) == 0 &&
                Spc.CompareTo(that.Spc) == 0 &&
                AdditionalUnderlyings.Equals(that.AdditionalUnderlyings) &&
                ExpirationStyle.Equals(that.ExpirationStyle) &&
                Mmy.Equals(that.Mmy) &&
                OptionType.Equals(that.OptionType) &&
                Cfi.Equals(that.Cfi) &&
                SettlementStyle.Equals(that.SettlementStyle);
        }

        /// <summary>
        /// Returns a hash code value for this option series.
        /// </summary>
        /// <returns>a hash code value.</returns>
        public int hashCode() {
            int result;
            long temp;
            result = Expiration;
            result = 31 * result + LastTrade;
            temp = Multiplier != +0.0d ? BitConverter.DoubleToInt64Bits(Multiplier) : 0L;
            result = 31 * result + (int)((ulong)temp ^ ((ulong)temp >> 32));
            temp = Spc != +0.0d ? BitConverter.DoubleToInt64Bits(Spc) : 0L;
            result = 31 * result + (int)((ulong)temp ^ ((ulong)temp >> 32));
            result = 31 * result + AdditionalUnderlyings.GetHashCode();
            result = 31 * result + Mmy.GetHashCode();
            result = 31 * result + OptionType.GetHashCode();
            result = 31 * result + ExpirationStyle.GetHashCode();
            result = 31 * result + SettlementStyle.GetHashCode();
            result = 31 * result + Cfi.GetHashCode();
            return result;
        }


        internal void AddOption(bool isCall, double strike, T option) {
            SortedDictionary<double, T> map = isCall ? Calls : Puts;
            if (Put(map, strike, option) == null) {
                Strikes = null;
            }
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

        /// <summary>
        /// Returns a string representation of this series.
        /// </summary>
        /// <returns>a string representation of this series.</returns>
        public string toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("expiration=").Append(DayUtil.GetYearMonthDayByDayId(Expiration));
            if (LastTrade != 0)
                sb.Append(", lastTrade=").Append(DayUtil.GetYearMonthDayByDayId(LastTrade));
            if (Multiplier != 0)
                sb.Append(", multiplier=").Append(Multiplier);
            if (Spc != 0)
                sb.Append(", spc=").Append(Spc);
            if (AdditionalUnderlyings.Length > 0)
                sb.Append(", additionalUnderlyings=").Append(AdditionalUnderlyings);
            if (Mmy.Length > 0)
                sb.Append(", mmy=").Append(Mmy);
            if (OptionType.Length > 0)
                sb.Append(", optionType=").Append(OptionType);
            if (ExpirationStyle.Length > 0)
                sb.Append(", expirationStyle=").Append(ExpirationStyle);
            if (SettlementStyle.Length > 0)
                sb.Append(", settlementStyle=").Append(SettlementStyle);
            sb.Append(", cfi=").Append(Cfi);
            return sb.ToString();
        }
    }
}
