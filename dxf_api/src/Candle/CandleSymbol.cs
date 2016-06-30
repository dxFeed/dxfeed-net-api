using System;
using System.Runtime.Serialization;

namespace com.dxfeed.api.candle {
    /// <summary>
    /// Symbol that should be used with {@link DXFeedSubscription} class
    /// to subscribe for {@link Candle} events. {@code DXFeedSubscription} also accepts a string
    /// representation of the candle symbol for subscription.
    ///
    /// <h3>String representation</h3>
    ///
    /// The string representation of the candle symbol consist of a {@link #getBaseSymbol() baseSymbol} followed by
    /// an optional '&' with an {@link #getExchange() exchange} code letter and followed by
    /// a list of comma-separated key=value pairs in curly braces:
    ///
    /// <p>{@code <baseSymbol> [ '&' <exchange> ] '{' <key1>=<value1> [ ',' <key2>=<value2> [ ',' ... ]] '}'}
    ///
    /// <p>Properties of the candle symbol correspond to the keys in the string representation in the following way:
    ///
    /// <ul>
    /// <li>Empty key corresponds to {@link #getPeriod() period} &mdash; aggregation period of this symbol.
    ///     The period value is composed of an optional
    ///     {@link CandlePeriod#getValue() value} which defaults to 1 when not specified, followed by
    ///     a {@link CandlePeriod#getType() type} string which is defined by one of the
    ///     {@link CandleType} enum values and can be abbreviated to first letters. For example, a daily candle of "IBM" base
    ///     symbol can be specified as "IBM{=d}" and 15 minute candle on it as "IBM{=15m}". The shortest
    ///     possible abbreviation for {@link CandleType#MONTH CandleType.MONTH} is "mo", so the monthly
    ///     candle can be specified as "IBM{=mo}". When period is not specified, then the
    ///     {@link CandlePeriod#TICK TICK} aggregation period is assumed as default. Note, that tick aggregation may
    ///     not be available on the demo system which is limited to a subset of symbols and aggregation periods.
    /// <li>"price" key corresponds to {@link #getPrice() price} &mdash; price type attribute of this symbol.
    ///     The {@link CandlePrice} enum defines possible values with {@link CandlePrice#LAST LAST} being default.
    ///     For legacy backwards-compatibility purposes, most of the price values cannot be abbreviated, so a one-minute candle
    ///     of "EUR/USD" bid price shall be specified with "EUR/USD{=m,price=bid}" candle symbol string. However,
    ///     the {@link CandlePrice#SETTLEMENT SETTLEMENT} can be abbreviated to "s", so a daily candle on
    ///     "/ES" futures settlement prices can be specified with "/ES{=d,price=s}" string.
    /// <li>"tho" key with a value of "true" corresponds to {@link #getSession() session} set to {@link CandleSession#REGULAR}
    ///     which limits the candle to trading hours only, so a 133 tick candles on "GOOG" base symbol collected over
    ///     trading hours only can be specified with "GOOG{=133t,tho=true}" string. Note, that the default daily candles for
    ///     US equities are special for historical reasons and correspond to the way US equity exchange report their
    ///     daily summary data. The volume the US equity default daily candle corresponds to the total daily traded volume,
    ///     while open, high, low, and close correspond to the regular trading hours only.
    /// <li>"a" key corresponds to {@link #getAlignment() alignment} &mdash; alignment attribute of this symbol.
    ///     The {@link CandleAlignment} enum defines possible values with {@link CandleAlignment#MIDNIGHT MIDNIGHT} being default.
    ///     The alignment values can be abbreviated to the first letter. So, a 1 hour candle on a symbol "AAPL" that starts
    ///     at the regular trading session at 9:30 am ET can be specified with "AAPL{=h,a=s,tho=true}". Contrast that
    ///     to the "AAPL{=h,tho=true}" candle that is aligned at midnight and thus starts at 9:00 am.
    /// </ul>
    ///
    /// Keys in the candle symbol are case-sensitive, while values are not. The {@link #valueOf(String)} method parses
    /// any valid string representation into a candle symbol object.
    /// The result of the candle symbol
    /// {@link #toString()} method is always normalized: keys are ordered lexicographically, values are in lower-case
    /// and are abbreviated to their shortest possible form.
    /// </summary>
    [Serializable()]
    public class CandleSymbol {

        private readonly string symbol;

        [NonSerialized()]
        private string baseSymbol;

        [NonSerialized()]
        private CandleExchange exchange;
        [NonSerialized()]
        private CandlePrice price;
        [NonSerialized()]
        private CandleSession session;
        [NonSerialized()]
        private CandlePeriod period;
        [NonSerialized()]
        private CandleAlignment alignment;

        private CandleSymbol(string symbol) {
            this.symbol = symbol.Normalize();
            initTransientFields();
        }

        private CandleSymbol(string symbol, CandleSymbolAttribute attribute) {
            this.symbol = changeAttribute(symbol, attribute).Normalize();
            attribute.checkInAttributeImpl(this);
            initTransientFields();
        }

        private CandleSymbol(string symbol, CandleSymbolAttribute attribute, params CandleSymbolAttribute[] attributes) {
            this.symbol = normalize(changeAttributes(symbol, attribute, attributes));
            attribute.checkInAttributeImpl(this);
            foreach (CandleSymbolAttribute a in attributes)
                a.checkInAttributeImpl(this);
            initTransientFields();
        }

        /// <summary>
        /// Returns base market symbol without attributes.
        /// </summary>
        /// <returns>Returns base market symbol without attributes.</returns>
        public string GetBaseSymbol() {
            return baseSymbol;
        }

        /// <summary>
        /// Returns exchange attribute of this symbol.
        /// </summary>
        /// <returns>Returns exchange attribute of this symbol.</returns>
        public CandleExchange GetExchange() {
            return exchange;
        }

        /// <summary>
        /// Returns price type attribute of this symbol.
        /// </summary>
        /// <returns>Returns price type attribute of this symbol.</returns>
        public CandlePrice GetPrice() {
            return price;
        }

        /// <summary>
        /// Returns session attribute of this symbol.
        /// </summary>
        /// <returns>Returns session attribute of this symbol.</returns>
        public CandleSession GetSession() {
            return session;
        }

        /// <summary>
        /// Returns aggregation period of this symbol.
        /// </summary>
        /// <returns>Returns aggregation period of this symbol.</returns>
        public CandlePeriod GetPeriod() {
            return period;
        }

        /// <summary>
        /// Returns alignment attribute of this symbol.
        /// </summary>
        /// <returns>Returns alignment attribute of this symbol.</returns>
        public CandleAlignment GetAlignment() {
            return alignment;
        }

        /// <summary>
        /// Returns string representation of this symbol.
        /// The string representation can be transformed back into symbol object
        /// using {@link #valueOf(String) valueOf(String)} method.
        /// </summary>
        /// <returns>string representation of this symbol.</returns>
        public override string ToString() {
            return symbol;
        }

        /// <summary>
        /// Indicates whether this symbol is the same as another one.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>{@code true} if this symbol is the same as another one.</returns>
        public override bool Equals(object obj) {
            return this == obj || obj.GetType() == typeof(CandleSymbol) && symbol.Equals(((CandleSymbol)obj).symbol);
        }

        /// <summary>
        /// Returns hash code of this symbol.
        /// </summary>
        /// <returns>hash code of this symbol.</returns>
        public override int GetHashCode() {
            return symbol.GetHashCode();
        }

        /// <summary>
        /// Converts the given string symbol into the candle symbol object.
        /// </summary>
        /// <param name="symbol">the string symbol.</param>
        /// <returns>the candle symbol object.</returns>
        public static CandleSymbol ValueOf(string symbol) {
            return new CandleSymbol(symbol);
        }

        /// <summary>
        /// Converts the given string symbol into the candle symbol object with the specified attribute set.
        /// </summary>
        /// <param name="symbol">the string symbol.</param>
        /// <param name="attribute">the attribute to set.</param>
        /// <returns>the candle symbol object.</returns>
        public static CandleSymbol ValueOf(string symbol, CandleSymbolAttribute attribute) {
            return new CandleSymbol(symbol, attribute);
        }

        /// <summary>
        /// Converts the given string symbol into the candle symbol object with the specified attributes set.
        /// </summary>
        /// <param name="symbol">the string symbol.</param>
        /// <param name="attribute">the attribute to set.</param>
        /// <param name="attributes">more attributes to set.</param>
        /// <returns>the candle symbol object.</returns>
        public static CandleSymbol ValueOf(string symbol, CandleSymbolAttribute attribute, 
                                           params CandleSymbolAttribute[] attributes) {
            return new CandleSymbol(symbol, attribute, attributes);
        }

        //----------------------- private implementation details -----------------------

        private static String changeAttributes(String symbol, CandleSymbolAttribute attribute, 
                                               params CandleSymbolAttribute[] attributes) {
            symbol = changeAttribute(symbol, attribute);
            foreach (CandleSymbolAttribute a in attributes)
                symbol = changeAttribute(symbol, a);
            return symbol;
        }

        private static string changeAttribute(string symbol, CandleSymbolAttribute attribute) {
            return attribute.changeAttributeForSymbol(symbol);
        }

        private static string normalize(string symbol) {
            symbol = CandlePrice.normalizeAttributeForSymbol(symbol);
            symbol = CandleSession.normalizeAttributeForSymbol(symbol);
            symbol = CandlePeriod.normalizeAttributeForSymbol(symbol);
            symbol = CandleAlignment.normalizeAttributeForSymbol(symbol);
            return symbol;
        }

        private void initTransientFields() {
            baseSymbol = MarketEventSymbols.getBaseSymbol(symbol);
            if (exchange == null)
                exchange = CandleExchange.getAttributeForSymbol(symbol);
            if (price == null)
                price = CandlePrice.getAttributeForSymbol(symbol);
            if (session == null)
                session = CandleSession.getAttributeForSymbol(symbol);
            if (period == null)
                period = CandlePeriod.getAttributeForSymbol(symbol);
            if (alignment == null)
                alignment = CandleAlignment.getAttributeForSymbol(symbol);
        }

        [OnDeserialized()]
        private void readObject(StreamingContext context) {
            initTransientFields();
        }
    }
}
