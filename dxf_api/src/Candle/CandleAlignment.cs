using System;
using System.Collections.Generic;

namespace com.dxfeed.api.candle {

    class CandleAlignment : ICandleSymbolAttribute {

        /// <summary>
        /// The attribute key that is used to store the value of {@code CandleAlignment} in
        /// a symbol string using methods of {@link MarketEventSymbols} class.
        /// The value of this constant is "a".
        /// The value that this key shall be set to is equal to
        /// the corresponding {@link #toString() CandleAlignment.ToString()}
        /// </summary>
        private static readonly string ATTRIBUTE_KEY = "a";
        private enum CandleAlignmentType { Midnight = 0, Session = 1 };
        private static Dictionary<string, CandleAlignment> objCash = new Dictionary<string, CandleAlignment>();

        /// <summary>
        /// Align candles on midnight.
        /// </summary>
        public static readonly CandleAlignment MIDNIGHT = new CandleAlignment(CandleAlignmentType.Midnight, "m");

        /// <summary>
        /// Align candles on trading sessions.
        /// </summary>
        public static readonly CandleAlignment SESSION = new CandleAlignment(CandleAlignmentType.Session, "s");

        /// <summary>
        /// Default alignment is {@link #MIDNIGHT}.
        /// </summary>
        public static readonly CandleAlignment DEFAULT = MIDNIGHT;

        private readonly CandleAlignmentType type;
        private readonly string value;

        private CandleAlignment(CandleAlignmentType type, string value) {
            this.type = type;
            this.value = value;
            objCash.Add(value, this);
        }

        /// <summary>
        /// Returns string representation of this candle alignment.
        /// The string representation of candle alignment "m" for {@link #MIDNIGHT}
        ///  and "s" for {@link #SESSION}.
        /// </summary>
        /// <returns>string representation of this candle alignment.</returns>
        public override string ToString() {
            return value;
        }

        /// <summary>
        /// Returns candle event symbol string with this candle alignment set.
        /// </summary>
        /// <param name="symbol">original candle event symbol</param>
        /// <returns>candle event symbol string with this candle alignment set.</returns>
        public String ChangeAttributeForSymbol(string symbol) {
            return this == DEFAULT ?
                MarketEventSymbols.RemoveAttributeStringByKey(symbol, ATTRIBUTE_KEY) :
                MarketEventSymbols.ChangeAttributeStringByKey(symbol, ATTRIBUTE_KEY, ToString());
        }

        /// <summary>
        /// Internal method that initializes attribute in the candle symbol.
        /// </summary>
        /// <param name="candleSymbol">candle symbol.</param>
        /// <exception cref="InvalidOperationException">Object is already initialized</exception>
        public void CheckInAttributeImpl(CandleSymbol candleSymbol) {
            if (candleSymbol.alignment != null)
                throw new InvalidOperationException("Already initialized");
            candleSymbol.alignment = this;
        }

        /// <summary>
        /// Get id of attribute value
        /// </summary>
        /// <returns>id of attribute value</returns>
        public int GetId() {
            return (int)type;
        }


        /// <summary>
        /// Parses string representation of candle alignment into object.
        /// Any string that was returned by {@link #toString()} can be parsed
        /// and case is ignored for parsing.
        /// 
        /// Excepions:
        ///    ArgumentNullException
        /// </summary>
        /// <param name="s">string representation of candle alignment.</param>
        /// <returns>candle alignment</returns>
        public static CandleAlignment parse(string s) {
            // fast path to reverse toString result
            if (objCash.ContainsKey(s))
                return objCash[s];
            // slow path for different case
            foreach (CandleAlignment align in objCash.Values) {
                if (align.ToString().Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    return align;
            }
            throw new ArgumentNullException("Unknown candle alignment: " + s);
        }

        /// <summary>
        /// Returns candle alignment of the given candle symbol string.
        /// The result is {@link #DEFAULT} if the symbol does not have candle alignment attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string</param>
        /// <returns>candle alignment of the given candle symbol string.</returns>
        public static CandleAlignment getAttributeForSymbol(string symbol) {
            string s = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);
            return s == null ? DEFAULT : parse(s);
        }

        /// <summary>
        /// Returns candle symbol string with the normalized representation of the candle alignment attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string.</param>
        /// <returns>candle symbol string with the normalized representation of the the candle alignment attribute.</returns>
        public static string normalizeAttributeForSymbol(string symbol) {
            string a = MarketEventSymbols.GetAttributeStringByKey(symbol, ATTRIBUTE_KEY);
            if (a == null)
                return symbol;
            try {
                CandleAlignment other = parse(a);
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
