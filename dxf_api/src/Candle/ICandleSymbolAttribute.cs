namespace com.dxfeed.api.candle
{
    /// <summary>
    /// Attribute of the {@link CandleSymbol}
    /// </summary>
    public interface ICandleSymbolAttribute
    {
        /// <summary>
        /// Returns candle event symbol string with this attribute set.
        /// </summary>
        /// <param name="symbol">original candle event symbol.</param>
        /// <returns>candle event symbol string with this attribute set.</returns>
        string ChangeAttributeForSymbol(string symbol);

        /// <summary>
        /// Internal method that initializes attribute in the candle symbol.
        /// </summary>
        /// <param name="candleSymbol">candleSymbol candle symbol.</param>
        void CheckInAttributeImpl(CandleSymbol candleSymbol);
    }
}
