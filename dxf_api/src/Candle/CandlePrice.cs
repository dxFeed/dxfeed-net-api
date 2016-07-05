﻿using System;
using System.Collections.Generic;

namespace com.dxfeed.api.candle {
    /// <summary>
    /// Price type attribute of {@link CandleSymbol} defines price that is used to build the candles.
    ///
    /// <h3>Implementation details</h3>
    ///
    /// This attribute is encoded in a symbol string with
    /// {@link MarketEventSymbols#getAttributeStringByKey(string, string) MarketEventSymbols.getAttributeStringByKey},
    /// {@link MarketEventSymbols#changeAttributeStringByKey(string, string, string) changeAttributeStringByKey}, and
    /// {@link MarketEventSymbols#removeAttributeStringByKey(string, string) removeAttributeStringByKey} methods.
    /// The key to use with these methods is available via
    /// {@link #ATTRIBUTE_KEY} constant.
    /// The value that this key shall be set to is equal to
    /// the corresponding {@link #toString() CandlePrice.ToString()}
    /// </summary>
    class CandlePrice : ICandleSymbolAttribute {

        /// <summary>
        /// The attribute key that is used to store the value of {@code CandlePrice} in
        /// a symbol string using methods of {@link MarketEventSymbols} class.
        /// The value of this constant is "price".
        /// The value that this key shall be set to is equal to
        /// the corresponding {@link #toString() CandlePrice.ToString()}
        /// </summary>
        public static readonly string ATTRIBUTE_KEY = "price";
        public enum CandlePriceType { Last = 0, Bid = 1, Ask = 2, Mark = 3, Settlement = 4 };
        private static Dictionary<string, CandlePrice> objCash = new Dictionary<string, CandlePrice>();

        /// <summary>
        /// Last trading price.
        /// </summary>
        public static readonly CandlePrice LAST = new CandlePrice(CandlePriceType.Last, "last");

        /// <summary>
        /// Quote bid price.
        /// </summary>
        public static readonly CandlePrice BID = new CandlePrice(CandlePriceType.Bid, "bid");

        /// <summary>
        /// Quote ask price.
        /// </summary>
        public static readonly CandlePrice ASK = new CandlePrice(CandlePriceType.Ask, "ask");

        /// <summary>
        /// Market price defined as average between quote bid and ask prices.
        /// </summary>
        public static readonly CandlePrice MARK = new CandlePrice(CandlePriceType.Mark, "mark");

        /// <summary>
        /// Official settlement price that is defined by exchange or last trading price otherwise.
        /// It updates based on all {@link PriceType PriceType} values:
        /// {@link PriceType#INDICATIVE}, {@link PriceType#PRELIMINARY}, and {@link PriceType#FINAL}.
        /// </summary>
        public static readonly CandlePrice SETTLEMENT = new CandlePrice(CandlePriceType.Settlement, "s");

        /// <summary>
        /// Default price type is {@link #LAST}.
        /// </summary>
        public static readonly CandlePrice DEFAULT = LAST;

        private readonly string value;
        private readonly CandlePriceType priceType;

        CandlePrice(CandlePriceType priceType, string value) {
            this.value = value;
            this.priceType = priceType;
            CandlePrice.objCash.Add(value, this);
        }

        /// <summary>
        /// Get id of price type attribute
        /// </summary>
        /// <returns>id of price type attribute</returns>
        public int GetId() {
            return (int)priceType;
        }

        /// <summary>
        /// Returns candle event symbol string with this candle price type set.
        /// </summary>
        /// <param name="symbol">original candle event symbol.</param>
        /// <returns>candle event symbol string with this candle price type set.</returns>
        public string ChangeAttributeForSymbol(string symbol) {
            return this == DEFAULT ?
                MarketEventSymbols.RemoveAttributeStringByKey(symbol, ATTRIBUTE_KEY) :
                MarketEventSymbols.ChangeAttributeStringByKey(symbol, ATTRIBUTE_KEY, ToString());
        }

        /// <summary>
        /// Internal method that initializes attribute in the candle symbol.
        /// </summary>
        /// <param name="candleSymbol">candle symbol.</param>
        /// <exception cref="InvalidOperationException">if used outside of internal initialization logic.</exception>
        public void CheckInAttributeImpl(CandleSymbol candleSymbol) {
            if (candleSymbol.price != null)
                throw new InvalidOperationException("Already initialized");
            candleSymbol.price = this;
        }

        /// <summary>
        /// Returns string representation of this candle price type.
        /// The string representation of candle price type is a lower case string
        /// that corresponds to its {@link #name() name}. For example,
        /// {@link #LAST} is represented as "last".
        /// </summary>
        /// <returns>string representation of this candle price type.</returns>
        public override string ToString() {
            return value;
        }

        /// <summary>
        /// Parses string representation of candle price type into object.
        /// Any string that was returned by {@link #toString()} can be parsed
        /// and case is ignored for parsing.
        /// </summary>
        /// <param name="s">string representation of candle price type.</param>
        /// <returns>candle price type.</returns>
        /// <exception cref="InvalidOperationException">if the string representation is invalid.</exception>
        public static CandlePrice Parse(string s) {
            int n = s.Length;
            if (n == 0)
                throw new InvalidOperationException("Missing candle price");
            // fast path to reverse toString result
            if (CandlePrice.objCash.ContainsKey(s))
                return CandlePrice.objCash[s];
            // slow path for everything else
            foreach (CandlePrice price in CandlePrice.objCash.Values) {
                string ps = price.ToString();
                if (ps.Length >= n && ps.Substring(0, n).Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    return price;
            }
            throw new InvalidOperationException("Unknown candle price: " + s);
        }

        /// <summary>
        /// Returns candle price type of the given candle symbol string.
        /// The result is {@link #DEFAULT} if the symbol does not have candle price attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string.</param>
        /// <returns>candle price of the given candle symbol string.</returns>
        public static CandlePrice GetAttributeForSymbol(string symbol) {
            string s = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);
            return s == null ? DEFAULT : Parse(s);
        }

        /// <summary>
        /// Returns candle symbol string with the normalized representation of the candle price type attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string.</param>
        /// <returns>candle symbol string with the normalized representation of the the candle price type attribute.</returns>
        public static string NormalizeAttributeForSymbol(string symbol) {
            string a = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);
            if (a == null)
                return symbol;
            try {
                CandlePrice other = Parse(a);
                if (other == DEFAULT)
                    MarketEventSymbols.RemoveAttributeStringByKey(symbol, ATTRIBUTE_KEY);
                if (!a.Equals(other.ToString()))
                    return MarketEventSymbols.ChangeAttributeStringByKey(symbol, ATTRIBUTE_KEY, other.ToString());
                return symbol;
            } catch (ArgumentNullException) {
                return symbol;
            }
        }

    }
}
