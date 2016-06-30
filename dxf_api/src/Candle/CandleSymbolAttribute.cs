using System;

namespace com.dxfeed.api.candle {

    /// <summary>
    /// Attribute of the {@link CandleSymbol}
    /// </summary>
    public interface CandleSymbolAttribute {

        /// <summary>
        /// Returns candle event symbol string with this attribute set.
        /// </summary>
        /// <param name="symbol">original candle event symbol.</param>
        /// <returns>candle event symbol string with this attribute set.</returns>
        public string changeAttributeForSymbol(String symbol);

        /// <summary>
        /// Internal method that initializes attribute in the candle symbol.
        /// </summary>
        /// <param name="candleSymbol">candleSymbol candle symbol.</param>
        public void checkInAttributeImpl(CandleSymbol candleSymbol);

        /// <summary>
        /// Get id of attribute value
        /// </summary>
        /// <returns>id of attribute value</returns>
        public int GetId();

    }
}
