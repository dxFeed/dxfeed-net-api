using System;

namespace com.dxfeed.api.candle {

    /// <summary>
    ///  Helper class to compose and parse symbols for market events.
    ///
    /// <h3>Regional symbols</h3>
    ///
    /// Regional symbol subscription receives events only from a designated exchange, marketplace, or venue
    /// instead of receiving composite events from all venues (by default). Regional symbol is composed from a
    /// <i>base symbol</i>, ampersand character ('&amp;'), and an exchange code character. For example,
    /// <ul>
    /// <li>"SPY" is the symbol for composite events for SPDR S&amp;P 500 ETF from all exchanges,
    /// <li>"SPY&amp;N" is the symbol for event for SPDR S&amp;P 500 ETF that originate only from NYSE marketplace.
    /// </ul>
    ///
    /// <h3>Symbol attributes</h3>
    ///
    /// Market event symbols can have a number of attributes attached to then in curly braces
    /// with {@code <key>=<value>} paris separated by commas. For example,
    /// <ul>
    /// <li>"SPY{price=bid}" is the market symbol "SPY" with an attribute key "price" set to value "bid".
    /// <li>"SPY(=5m,tho=true}" is the market symbol "SPY" with two attributes. One has an empty key and
    /// value "5m", while the other has key "tho" and value "true".
    /// </ul>
    /// The methods in this class always maintain attribute keys in alphabetic order.
    /// </summary>
    private class MarketEventSymbols {

        private static readonly char EXCHANGE_SEPARATOR = '&';
        private static readonly char ATTRIBUTES_OPEN = '{';
        private static readonly char ATTRIBUTES_CLOSE = '}';
        private static readonly char ATTRIBUTES_SEPARATOR = ',';
        private static readonly char ATTRIBUTE_VALUE = '=';

        private MarketEventSymbols() {}

        /// <summary>
        /// Returns {@code true} is the specified symbol has the exchange code specification.
        /// The result is {@code false} if symbol is {@code null}.
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <returns>{@code true} is the specified symbol has the exchange code specification.</returns>
        public static bool hasExchangeCode(string symbol) {
            return symbol != null && hasExchangeCodeInternal(symbol, getLengthWithoutAttributesInternal(symbol));
        }

        /// <summary>
        /// Returns exchange code of the specified symbol or {@code '\0'} if none is defined.
        /// The result is {@code '\0'} if symbol is {@code null}.
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <returns>exchange code of the specified symbol or {@code '\0'} if none is defined.</returns>
        public static char getExchangeCode(string symbol) {
            return hasExchangeCode(symbol) ? symbol[getLengthWithoutAttributesInternal(symbol) - 1] : '0';
        }

        /// <summary>
        /// Changes exchange code of the specified symbol or removes it
        /// if new exchange code is {@code '\0'}.
        /// The result is {@code null} if old symbol is {@code null}.
        /// </summary>
        /// <param name="symbol">old symbol.</param>
        /// <param name="exchangeCode">new exchange code.</param>
        /// <returns>new symbol with the changed exchange code.</returns>
        public static string changeExchangeCode(string symbol, char exchangeCode) {
            if (symbol == null)
                return exchangeCode == 0 ? null : "" + EXCHANGE_SEPARATOR + exchangeCode;
            int i = getLengthWithoutAttributesInternal(symbol);
            string result = exchangeCode == 0 ?
                getBaseSymbolInternal(symbol, i) :
                getBaseSymbolInternal(symbol, i) + EXCHANGE_SEPARATOR + exchangeCode;
            return i == symbol.Length ? result : result + symbol.Substring(i);
        }

        /// <summary>
        /// Returns base symbol without exchange code and attributes.
        /// The result is {@code null} if symbol is {@code null}.
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <returns>base symbol without exchange code and attributes.</returns>
        public static string getBaseSymbol(string symbol) {
            if (symbol == null)
                return null;
            return getBaseSymbolInternal(symbol, getLengthWithoutAttributesInternal(symbol));
        }

        /// <summary>
        /// Changes base symbol while leaving exchange code and attributes intact.
        /// The result is {@code null} if old symbol is {@code null}.
        /// </summary>
        /// <param name="symbol">old symbol</param>
        /// <param name="baseSymbol">new base symbol.</param>
        /// <returns>new symbol with new base symbol and old symbol's exchange code and attributes.</returns>
        public static string changeBaseSymbol(string symbol, string baseSymbol) {
            if (symbol == null)
                return baseSymbol;
            int i = getLengthWithoutAttributesInternal(symbol);
            return hasExchangeCodeInternal(symbol, i) ?
                baseSymbol + EXCHANGE_SEPARATOR + symbol[i - 1] + symbol.Substring(i) :
                i == symbol.Length ? baseSymbol : baseSymbol + symbol.Substring(i);
        }

        /// <summary>
        /// Returns true if the specified symbol has any attributes.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool hasAttributes(string symbol) {
            return symbol != null && getLengthWithoutAttributesInternal(symbol) < symbol.Length;
        }

        /// <summary>
        /// Returns value of the attribute with the specified key.
        /// The result is {@code null} if attribute with the specified key is not found.
        /// The result is {@code null} if symbol is {@code null}.
        /// 
        /// Exceptions:
        ///     ArgumentNullException
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <param name="key">attribute key</param>
        /// <returns>value of the attribute with the specified key</returns>
        public static string getAttributeStringByKey(string symbol, string key) {
            if (key == null)
                throw new ArgumentNullException();
            if (symbol == null)
                return null;
            return getAttributeInternal(symbol, getLengthWithoutAttributesInternal(symbol), key);
        }

        /// <summary>
        /// Changes value of one attribute value while leaving exchange code and other attributes intact.
        /// The {@code null} symbol is interpreted as empty one by this method..
        /// 
        /// Exceptions:
        ///     ArgumentNullException
        /// </summary>
        /// <param name="symbol">old symbol</param>
        /// <param name="key">attribute key</param>
        /// <param name="value">attribute value</param>
        /// <returns>new symbol with key attribute with the specified value and everything else from the old symbol.</returns>
        public static string changeAttributeStringByKey(string symbol, string key, string value) {
            if (key == null)
                throw new ArgumentNullException();
            if (symbol == null)
                return value == null ? null : ATTRIBUTES_OPEN + key + ATTRIBUTE_VALUE + value + ATTRIBUTES_CLOSE;
            int i = getLengthWithoutAttributesInternal(symbol);
            if (i == symbol.Length)
                return value == null ? symbol : symbol + ATTRIBUTES_OPEN + key + ATTRIBUTE_VALUE + value + ATTRIBUTES_CLOSE;
            return value == null ? removeAttributeInternal(symbol, i, key) : addAttributeInternal(symbol, i, key, value);
        }

        /// <summary>
        /// Removes one attribute with the specified key while leaving exchange code and other attributes intact.
        /// The result is {@code null} if symbol is {@code null}.
        /// 
        /// Exception:
        ///     ArgumentNullException
        /// </summary>
        /// <param name="symbol">old symbol</param>
        /// <param name="key">attribute key</param>
        /// <returns>new symbol without the specified key and everything else from the old symbol.</returns>
        public static string removeAttributeStringByKey(string symbol, string key) {
            if (key == null)
                throw new ArgumentNullException();
            if (symbol == null)
                return null;
            return removeAttributeInternal(symbol, getLengthWithoutAttributesInternal(symbol), key);
        }

        private static bool hasExchangeCodeInternal(string symbol, int length) {
            return length >= 2 && symbol[length - 2] == EXCHANGE_SEPARATOR;
        }

        private static string getBaseSymbolInternal(string symbol, int length) {
            return hasExchangeCodeInternal(symbol, length) ? symbol.Substring(0, length - 2) : symbol.Substring(0, length);
        }

        private static bool hasAttributesInternal(string symbol, int length) {
            if (length >= 3 && symbol[length - 1] == ATTRIBUTES_CLOSE) {
                int i = symbol.LastIndexOf(ATTRIBUTES_OPEN, length - 2);
                return i >= 0 && i < length - 2;
            }
            else
                return false;
        }

        private static int getLengthWithoutAttributesInternal(string symbol) {
            int length = symbol.Length;
            return hasAttributesInternal(symbol, length) ? symbol.LastIndexOf(ATTRIBUTES_OPEN) : length;
        }

        private static string getKeyInternal(string symbol, int i) {
            int val = symbol.IndexOf(ATTRIBUTE_VALUE, i);
            return val < 0 ? null : symbol.Substring(i, val);
        }

        private static int getNextKeyInternal(string symbol, int i) {
            int val = symbol.IndexOf(ATTRIBUTE_VALUE, i) + 1;
            int sep = symbol.IndexOf(ATTRIBUTES_SEPARATOR, val);
            return sep < 0 ? symbol.Length : sep + 1;
        }

        private static string getValueInternal(string symbol, int i, int j) {
            return symbol.Substring(symbol.IndexOf(ATTRIBUTE_VALUE, i) + 1, j - 1);
        }

        private static string dropKeyAndValueInternal(string symbol, int length, int i, int j) {
            return j == symbol.Length ? i == length + 1 ? symbol.Substring(0, length) :
                symbol.Substring(0, i - 1) + symbol.Substring(j - 1) :
                symbol.Substring(0, i) + symbol.Substring(j);
        }

        private static string getAttributeInternal(string symbol, int length, string key) {
            if (length == symbol.Length)
                return null;
            int i = length + 1;
            while (i < symbol.Length) {
                string cur = getKeyInternal(symbol, i);
                if (cur == null)
                    break;
                int j = getNextKeyInternal(symbol, i);
                if (key.Equals(cur))
                    return getValueInternal(symbol, i, j);
                i = j;
            }
            return null;
        }

        private static string removeAttributeInternal(string symbol, int length, string key) {
            if (length == symbol.Length)
                return symbol;
            int i = length + 1;
            while (i < symbol.Length) {
                string cur = getKeyInternal(symbol, i);
                if (cur == null)
                    break;
                int j = getNextKeyInternal(symbol, i);
                if (key.Equals(cur))
                    symbol = dropKeyAndValueInternal(symbol, length, i, j);
                else
                    i = j;
            }
            return symbol;
        }

        private static string addAttributeInternal(string symbol, int length, string key, string value) {
            if (length == symbol.Length)
                return symbol + ATTRIBUTES_OPEN + key + ATTRIBUTE_VALUE + value + ATTRIBUTES_CLOSE;
            int i = length + 1;
            bool added = false;
            while (i < symbol.Length) {
                string cur = getKeyInternal(symbol, i);
                if (cur == null)
                    break;
                int j = getNextKeyInternal(symbol, i);
                int cmp = cur.CompareTo(key);
                if (cmp == 0) {
                    if (added) {
                        // drop, since we've already added this key
                        symbol = dropKeyAndValueInternal(symbol, length, i, j);
                    } else {
                        // replace value
                        symbol = symbol.Substring(0, i) + key + ATTRIBUTE_VALUE + value + symbol.Substring(j - 1);
                        added = true;
                        i += key.Length + value.Length + 2;
                    }
                } else if (cmp > 0 && !added) {
                    // insert value here
                    symbol = symbol.Substring(0, i) + key + ATTRIBUTE_VALUE + value + ATTRIBUTES_SEPARATOR + symbol.Substring(i);
                    added = true;
                    i += key.Length + value.Length + 2;
                } else
                    i = j;
            }
            return added ? symbol : symbol.Substring(0, i - 1) + ATTRIBUTES_SEPARATOR + key + ATTRIBUTE_VALUE + value + symbol.Substring(i - 1);
        }

    }
}
