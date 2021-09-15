#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Globalization;
using com.dxfeed.api.events.market;

namespace com.dxfeed.api.candle
{
    /// <summary>
    /// Candle price level attribute of {@link CandleSymbol} defines how candles shall be aggregated in respect to
    /// price interval. The negative or infinite values of price interval are treated as exceptional.
    /// <ul>
    /// <li>Price interval may be equal to zero. It means every unique price creates a particular candle
    /// to aggregate all events with this price for the chosen {@link CandlePeriod}.</li>
    /// <li>Non-zero price level creates sequence of intervals starting from 0:
    /// ...,[-pl;0),[0;pl),[pl;2*pl),...,[n*pl,n*pl+pl). Events aggregated by chosen {@link CandlePeriod} and price intervals.</li>
    /// </ul>
    ///
    /// <h3>Implementation details</h3>
    /// <p/>
    /// This attribute is encoded in a symbol string with
    /// {@link MarketEventSymbols#getAttributeStringByKey(String, String) MarketEventSymbols.getAttributeStringByKey},
    /// {@link MarketEventSymbols#changeAttributeStringByKey(String, String, String) changeAttributeStringByKey}, and
    /// {@link MarketEventSymbols#removeAttributeStringByKey(String, String) removeAttributeStringByKey} methods.
    /// The key to use with these methods is available via
    /// {@link #ATTRIBUTE_KEY} constant.
    /// The value that this key shall be set to is equal to
    /// the corresponding {@link #toString() CandlePriceLevel.toString()}
    /// </summary>
    public class CandlePriceLevel : ICandleSymbolAttribute
    {
        /// <summary>
        /// Default candle price level (NaN)
        /// </summary>
        public static readonly CandlePriceLevel DEFAULT = new CandlePriceLevel(double.NaN);
        /// <summary>
        /// The price level attribute key
        /// </summary>
        public const string ATTRIBUTE_KEY = "pl";

        private readonly double value;
        private readonly string stringBuf;

        private static bool IsNegativeZero(double x)
        {
            return x.Equals(0.0) && double.IsNegativeInfinity(1.0 / x);
        }

        CandlePriceLevel(double value)
        {
            if (double.IsInfinity(value) || IsNegativeZero(value)) // reject -0.0
                throw new ArgumentException($"Incorrect candle price level: {value}");

            this.value = value;
            stringBuf = value == (long) value ? (long) value + "" : value.ToString(new CultureInfo("en-US"));
        }

        /// <summary>
        /// Returns a value of the candle price level 
        /// </summary>
        /// <returns></returns>
        public double GetValue()
        {
            return value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return stringBuf;
        }

        /// <summary>
        /// Changes attributes for the symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <returns>The new symbol</returns>
        public string ChangeAttributeForSymbol(string symbol)
        {
            return Equals(this, DEFAULT)
                ? MarketEventSymbols.RemoveAttributeStringByKey(symbol, ATTRIBUTE_KEY)
                : MarketEventSymbols.ChangeAttributeStringByKey(symbol, ATTRIBUTE_KEY, stringBuf);
        }

        /// <summary>
        /// Internal method that initializes attribute in the candle symbol.
        /// </summary>
        /// <param name="candleSymbol">The candle symbol.</param>
        /// <exception cref="InvalidOperationException">if used outside of internal initialization logic</exception>
        public void CheckInAttributeImpl(CandleSymbol candleSymbol)
        {
            if (candleSymbol.priceLevel != null)
                throw new InvalidOperationException("Already initialized");
            candleSymbol.priceLevel = this;
        }

        /// <summary>
        /// Parses string representation of candle price level attribute into object.
        /// Any string that was returned by {@link #ToString()} can be parsed
        /// </summary>
        /// <param name="s">The string representation of candle candle price level attribute.</param>
        /// <returns>The candle price level attribute.</returns>
        public static CandlePriceLevel Parse(string s)
        {
            var value = double.Parse(s, new CultureInfo("en-US"));
            return ValueOf(value);
        }

        /// <summary>
        /// Returns price level attribute object by value.
        /// </summary>
        /// <param name="value">The price level value</param>
        /// <returns>The candle price level attribute object.</returns>
        public static CandlePriceLevel ValueOf(double value)
        {
            return double.IsNaN(value) ? DEFAULT : new CandlePriceLevel(value);
        }

        /// <summary>
        /// Returns candle price level attribute of the given candle symbol string.
        /// The result is {@link #DEFAULT} if the symbol does not have candle session attribute.
        /// </summary>
        /// <param name="symbol">The candle symbol string.</param>
        /// <returns>The candle price level attribute of the given candle symbol string.</returns>
        public static CandlePriceLevel GetAttributeForSymbol(string symbol)
        {
            var s = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);

            return s == null ? DEFAULT : Parse(s);
        }

        /// <summary>
        /// Returns full string representation of this candle price level attribute.
        /// It is contains attribute key and its value.
        /// The full string representation of price level = 0.5 is "pl=0.5"
        /// </summary>
        /// <returns>The full string representation of a candle price level attribute</returns>
        public string ToFullString()
        {
            return $"{ATTRIBUTE_KEY}={stringBuf}";
        }

        /// <summary>
        /// Returns candle symbol string with the normalized representation of the candle price level attribute.
        /// </summary>
        /// <param name="symbol">The candle symbol string.</param>
        /// <returns>The candle symbol string with the normalized representation of the the candle price level attribute.</returns>
        public static string NormalizeAttributeForSymbol(string symbol)
        {
            var a = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);

            if (a == null)
                return symbol;
            try
            {
                var other = Parse(a);

                if (other.Equals(DEFAULT))
                    MarketEventSymbols.RemoveAttributeStringByKey(symbol, ATTRIBUTE_KEY);

                return !a.Equals(other.ToString())
                    ? MarketEventSymbols.ChangeAttributeStringByKey(symbol, ATTRIBUTE_KEY, other.ToString())
                    : symbol;
            } catch (ArgumentNullException)
            {
                return symbol;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            if (this == o)
                return true;
            
            if (o == null || GetType() != o.GetType()) 
            {
                return false;
            }
            
            var that = (CandlePriceLevel)o;
            
            return value.CompareTo(that.GetValue()) == 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}