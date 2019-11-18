#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.events.market;
using System;
using System.Globalization;

namespace com.dxfeed.api.candle
{
    /// <summary>
    /// Period attribute of {@link CandleSymbol} defines aggregation period of the candles.
    /// Aggregation period is defined as pair of a {@link #GetValue()} and {@link #GetCandleType() type}.
    ///
    /// <h3>Implementation details</h3>
    ///
    /// This attribute is encoded in a symbol string with
    /// {@link com.dxfeed.api.events.market.MarketEventSymbols#GetAttributeStringByKey
    /// MarketEventSymbols.GetAttributeStringByKey},
    /// {@link com.dxfeed.api.events.market.MarketEventSymbols#ChangeAttributeStringByKey
    /// MarketEventSymbols.ChangeAttributeStringByKey}, and
    /// {@link com.dxfeed.api.events.market.MarketEventSymbols#RemoveAttributeStringByKey
    /// MarketEventSymbols.RemoveAttributeStringByKey} methods.
    /// The key to use with these methods is available via
    /// {@link #ATTRIBUTE_KEY} constant.
    /// The value that this key shall be set to is equal to
    /// the corresponding {@link #ToString() CandlePeriod.ToString()}
    /// </summary>
    public class CandlePeriod : ICandleSymbolAttribute
    {
        /// <summary>
        /// The number represents default period value.
        /// </summary>
        private static readonly int PERIOD_VALUE_DEFAULT = 1;

        /// <summary>
        /// Tick aggregation where each candle represents an individual tick.
        /// </summary>
        public static readonly CandlePeriod TICK = new CandlePeriod(PERIOD_VALUE_DEFAULT, CandleType.TICK);

        /// <summary>
        /// Day aggregation where each candle represents a day.
        /// </summary>
        public static readonly CandlePeriod DAY = new CandlePeriod(PERIOD_VALUE_DEFAULT, CandleType.DAY);

        /// <summary>
        /// Default period is {@link #TICK}.
        /// </summary>
        public static readonly CandlePeriod DEFAULT = TICK;

        /// <summary>
        /// The attribute key that is used to store the value of `CandlePeriod` in
        /// a symbol string using methods of {@link com.dxfeed.api.events.market.MarketEventSymbols MarketEventSymbols}
        /// class.
        /// The value of this constant is an empty string, because this is the
        /// main attribute that every {@link CandleSymbol} must have.
        /// The value that this key shall be set to is equal to
        /// the corresponding {@link #ToString() CandlePeriod.ToString()}
        /// </summary>
        public static readonly string ATTRIBUTE_KEY = ""; // empty string as attribute key is allowed!

        private readonly double value;
        private readonly CandleType type;

        private string stringBuf;

        /// <summary>
        /// Create a new candle period attribute
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        CandlePeriod(double value, CandleType type)
        {
            this.value = value;
            this.type = type;
            if (value == PERIOD_VALUE_DEFAULT)
                stringBuf = type.ToString();
            else
                stringBuf = value == (long)value ? (long)value + "" + type : value.ToString(new CultureInfo("en-US")) + "" + type;
        }

        /// <summary>
        /// Returns aggregation period in milliseconds as closely as possible.
        /// Certain aggregation types like {@link CandleType#SECOND SECOND} and
        /// {@link CandleType#DAY DAY} span a specific number of milliseconds.
        /// {@link CandleType#MONTH}, {@link CandleType#OPTEXP} and {@link CandleType#YEAR}
        /// are approximate. Candle period of
        /// {@link CandleType#TICK}, {@link CandleType#VOLUME}, {@link CandleType#PRICE},
        /// {@link CandleType#PRICE_MOMENTUM} and {@link CandleType#PRICE_RENKO}
        /// is not defined and this method returns `0`.
        /// The result of this method is equal to
        /// `(long)(this.GetCandleType().getPeriodIntervalMillis() /// this.GetValue())`
        /// @see CandleType#GetPeriodIntervalMillis()
        /// </summary>
        /// <returns>aggregation period in milliseconds.</returns>
        public long GetPeriodIntervalMillis()
        {
            return (long)(type.GetPeriodIntervalMillis() * value);
        }

        /// <summary>
        /// Returns candle event symbol string with this aggregation period set.
        /// </summary>
        /// <param name="symbol">original candle event symbol.</param>
        /// <returns>candle event symbol string with this aggregation period set.</returns>
        public string ChangeAttributeForSymbol(string symbol)
        {
            return this == DEFAULT ?
                MarketEventSymbols.RemoveAttributeStringByKey(symbol, ATTRIBUTE_KEY) :
                MarketEventSymbols.ChangeAttributeStringByKey(symbol, ATTRIBUTE_KEY, ToString());
        }

        /// <summary>
        /// Internal method that initializes attribute in the candle symbol.
        /// </summary>
        /// <param name="candleSymbol">candle symbol.</param>
        /// <exception cref="InvalidOperationException">if used outside of internal initialization logic.</exception>
        public void CheckInAttributeImpl(CandleSymbol candleSymbol)
        {
            if (candleSymbol.period != null)
                throw new InvalidOperationException("Already initialized");
            candleSymbol.period = this;
        }

        /// <summary>
        /// Returns aggregation period value. For example, the value of `5` with
        /// the candle type of {@link CandleType#MINUTE MINUTE} represents 5 minute
        /// aggregation period.
        /// </summary>
        /// <returns>aggregation period value.</returns>
        public double GetValue()
        {
            return value;
        }

        /// <summary>
        /// Returns aggregation period type.
        /// </summary>
        /// <returns>aggregation period type.</returns>
        public CandleType GetCandleType()
        {
            return type;
        }

        /// <summary>
        /// Indicates whether this aggregation period is the same as another one.
        /// The same aggregation period has the same {@link #GetValue() value} and
        /// {@link #GetCandleType() type}.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>`true` if this aggregation period is the same as another one.</returns>
        public override bool Equals(object o)
        {
            if (this == o)
                return true;
            if (!(o.GetType() == typeof(CandlePeriod)))
                return false;
            CandlePeriod that = (CandlePeriod)o;
            return value.CompareTo(that.value) == 0 && type == that.type;
        }

        /// <summary>
        /// Returns hash code of this aggregation period.
        /// </summary>
        /// <returns>hash code of this aggregation period.</returns>
        public override int GetHashCode()
        {
            ulong temp = value != +0.0d ? (ulong)BitConverter.DoubleToInt64Bits(value) : 0UL;
            return 31 * (int)(temp ^ (temp >> 32)) + type.GetHashCode();
        }

        /// <summary>
        /// Returns string representation of this aggregation period.
        /// The string representation is composed of value and type string.
        /// For example, 5 minute aggregation is represented as `"5m"`.
        /// The value of `1` is omitted in the string representation, so
        /// {@link #DAY} (one day) is represented as `"d"`.
        /// This string representation can be converted back into object
        /// with {@link #Parse(string)} method.
        /// </summary>
        /// <returns>string representation of this aggregation period.</returns>
        public override string ToString()
        {
            return stringBuf;
        }

        /// <summary>
        /// Parses string representation of aggregation period into object.
        /// Any string that was returned by {@link #ToString()} can be parsed.
        /// This method is flexible in the way candle types can be specified.
        /// See {@link CandleType#Parse(string)} for details.
        /// </summary>
        /// <param name="s">string representation of aggregation period.</param>
        /// <returns>aggregation period object.</returns>
        /// <exception cref="ArgumentNullException">s is null</exception>
        /// <exception cref="FormatException">s does not represent a number in a valid format.</exception>
        /// <exception cref="OverflowException">s represents a number that is less than System.Double.MinValue or greater
        /// than System.Double.MaxValue.</exception>
        public static CandlePeriod Parse(string s)
        {
            if (s.Equals(CandleType.DAY.ToString()))
                return DAY;
            if (s.Equals(CandleType.TICK.ToString()))
                return TICK;
            int i = 0;
            for (; i < s.Length; i++)
            {
                char c = s[i];
                if ((c < '0' || c > '9') && c != '.' && c != '-')
                    break;
            }
            string value = s.Substring(0, i);
            string type = s.Substring(i);
            return ValueOf(value.Length == 0 ? PERIOD_VALUE_DEFAULT : double.Parse(value, new CultureInfo("en-US")), CandleType.Parse(type));
        }

        /// <summary>
        /// Returns candle period with the given value and type.
        /// </summary>
        /// <param name="value">value candle period value.</param>
        /// <param name="type">candle period type.</param>
        /// <returns>candle period with the given value and type.</returns>
        public static CandlePeriod ValueOf(double value, CandleType type)
        {
            if (value == PERIOD_VALUE_DEFAULT && type == CandleType.DAY)
                return DAY;
            if (value == PERIOD_VALUE_DEFAULT && type == CandleType.TICK)
                return TICK;
            return new CandlePeriod(value, type);
        }

        /// <summary>
        /// Returns candle period of the given candle symbol string.
        /// The result is {@link #DEFAULT} if the symbol does not have candle period attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string.</param>
        /// <returns>candle period of the given candle symbol string.</returns>
        /// <exception cref="ArgumentNullException">if string representation is invalid.</exception>
        public static CandlePeriod GetAttributeForSymbol(string symbol)
        {
            string s = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);
            return s == null ? DEFAULT : Parse(s);
        }


        /// <summary>
        /// Returns candle symbol string with the normalized representation of the candle period attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string.</param>
        /// <returns>candle symbol string with the normalized representation of the the candle period attribute.</returns>
        public static string NormalizeAttributeForSymbol(string symbol)
        {
            string a = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);
            if (a == null)
                return symbol;
            try
            {
                CandlePeriod other = Parse(a);
                if (other.Equals(DEFAULT))
                    MarketEventSymbols.RemoveAttributeStringByKey(symbol, ATTRIBUTE_KEY);
                if (!a.Equals(other.ToString()))
                    return MarketEventSymbols.ChangeAttributeStringByKey(symbol, ATTRIBUTE_KEY, other.ToString());
                return symbol;
            }
            catch (ArgumentNullException)
            {
                return symbol;
            }
        }
    }
}
