/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.api.events.market;
using System;
using System.Collections.Generic;

namespace com.dxfeed.api.candle
{
    /// <summary>
    /// Session attribute of {@link CandleSymbol} defines trading that is used to build the candles.
    ///
    /// <h3>Implementation details</h3>
    ///
    /// This attribute is encoded in a symbol string with
    /// {@link MarketEventSymbols#getAttributeStringByKey(string, string) MarketEventSymbols.getAttributeStringByKey},
    /// {@link MarketEventSymbols#changeAttributeStringByKey(string, string, string) changeAttributeStringByKey}, and
    /// {@link MarketEventSymbols#removeAttributeStringByKey(string, string) removeAttributeStringByKey} methods.
    ///
    /// <p> {@link #ANY} session is a default.
    /// The key to use with these methods is available via
    /// {@link #ATTRIBUTE_KEY} constant.
    /// The value that this key shall be set to is equal to
    /// the corresponding {@link #toString() CandleSession.ToString()}
    /// </summary>
    class CandleSession : ICandleSymbolAttribute
    {
        /// <summary>
        /// The attribute key that is used to store the value of {@code CandleSession} in
        /// a symbol string using methods of {@link MarketEventSymbols} class.
        /// The value of this constant is "tho", which is an abbreviation for "trading hours only".
        /// The value that this key shall be set to is equal to
        /// the corresponding {@link #toString() CandleSession.ToString()}
        /// </summary>
        public static readonly string ATTRIBUTE_KEY = "tho";
        private enum CandleSessionType { Any = 0, Regular = 1 };
        private static Dictionary<string, CandleSession> objCash = new Dictionary<string, CandleSession>();

        /// <summary>
        /// All trading sessions are used to build candles.
        /// </summary>
        public static readonly CandleSession ANY = new CandleSession(CandleSessionType.Any, "false");

        /// <summary>
        /// Only regular trading session data is used to build candles.
        /// </summary>
        public static readonly CandleSession REGULAR = new CandleSession(CandleSessionType.Regular, "true");

        /// <summary>
        /// Default trading session is {@link #ANY}.
        /// </summary>
        public static readonly CandleSession DEFAULT = ANY;

        private readonly CandleSessionType sessionFilter;
        private readonly string value;

        CandleSession(CandleSessionType sessionFilter, string value)
        {
            this.sessionFilter = sessionFilter;
            this.value = value;
            objCash.Add(value, this);
        }

        /// <summary>
        /// Returns candle session struct id
        /// </summary>
        /// <returns>candle session struct id</returns>
        public int GetId()
        {
            return (int)sessionFilter;
        }

        /// <summary>
        /// Returns candle event symbol string with this session attribute set.
        /// </summary>
        /// <param name="symbol">original candle event symbol.</param>
        /// <returns>candle event symbol string with this session attribute set.</returns>
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
            if (candleSymbol.session != null)
                throw new InvalidOperationException("Already initialized");
            candleSymbol.session = this;
        }

        /// <summary>
        /// Returns string representation of this candle session attribute.
        /// The string representation of candle session attribute is a lower case string
        /// that corresponds to its {@link #name() name}. For example,
        /// {@link #ANY} is represented as "any".
        /// </summary>
        /// <returns>string representation of this candle session attribute.</returns>
        public override string ToString()
        {
            return value;
        }

        /// <summary>
        /// Returns full string representation of this candle session attribute. 
        /// It is contains attribute key and its value. 
        /// The full string representation of {@link #ANY} is "tho=any"
        /// </summary>
        /// <returns></returns>
        public string ToFullString()
        {
            return string.Format("{0}={1}", ATTRIBUTE_KEY, value);
        }

        /// <summary>
        /// Parses string representation of candle session attribute into object.
        /// Any string that was returned by {@link #toString()} can be parsed
        /// and case is ignored for parsing.
        /// </summary>
        /// <param name="s">string representation of candle candle session attribute.</param>
        /// <returns>candle session attribute.</returns>
        /// <exception cref="InvalidOperationException">if the string representation is invalid.</exception>
        public static CandleSession Parse(string s)
        {
            int n = s.Length;
            if (n == 0)
                throw new InvalidOperationException("Missing candle session");
            foreach (CandleSession session in objCash.Values)
            {
                string ss = session.ToString();
                if (ss.Length >= n && ss.Substring(0, n).Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    return session;
            }
            throw new InvalidOperationException("Unknown candle session: " + s);
        }

        /// <summary>
        /// Returns candle session attribute of the given candle symbol string.
        /// The result is {@link #DEFAULT} if the symbol does not have candle session attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string.</param>
        /// <returns>candle session attribute of the given candle symbol string.</returns>
        public static CandleSession GetAttributeForSymbol(string symbol)
        {
            string a = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);
            return a != null && bool.Parse(a) ? REGULAR : DEFAULT;
        }

        /// <summary>
        /// Returns candle symbol string with the normalized representation of the candle session attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string.</param>
        /// <returns>candle symbol string with the normalized representation of the the candle session attribute.</returns>
        public static string NormalizeAttributeForSymbol(string symbol)
        {
            string a = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);
            if (a == null)
                return symbol;
            try
            {
                bool b = bool.Parse(a);
                if (!b)
                    MarketEventSymbols.RemoveAttributeStringByKey(symbol, ATTRIBUTE_KEY);
                if (b && !a.Equals(REGULAR.ToString()))
                    return MarketEventSymbols.ChangeAttributeStringByKey(symbol, ATTRIBUTE_KEY, REGULAR.ToString());
                return symbol;
            }
            catch (ArgumentNullException)
            {
                return symbol;
            }
        }
    }
}
