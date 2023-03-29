#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events.market;
using System;

namespace com.dxfeed.api.candle
{
    /// <summary>
    /// Exchange attribute of {@link CandleSymbol} defines exchange identifier where data is
    /// taken from to build the candles.
    ///
    /// <h3>Implementation details</h3>
    ///
    /// This attribute is encoded in a symbol string with
    /// {@link com.dxfeed.api.events.market.MarketEventSymbols#GetExchangeCode(string)
    /// MarketEventSymbols.GetExchangeCode} and
    /// {@link com.dxfeed.api.events.market.MarketEventSymbols#ChangeExchangeCode(string, char)
    /// MarketEventSymbols.ChangeExchangeCode} methods.
    /// </summary>
    class CandleExchange : ICandleSymbolAttribute
    {
        /// <summary>
        /// Composite exchange where data is taken from all exchanges.
        /// </summary>
        public static readonly CandleExchange COMPOSITE = new CandleExchange('\0');

        /// <summary>
        /// Default exchange is {@link #COMPOSITE}.
        /// </summary>
        public static readonly CandleExchange DEFAULT = COMPOSITE;

        private readonly char exchangeCode;

        CandleExchange(char exchangeCode)
        {
            this.exchangeCode = exchangeCode;
        }

        /// <summary>
        /// Returns exchange code. It is `'\0'` for {@link #COMPOSITE} exchange.
        /// </summary>
        /// <returns>exchange code.</returns>
        public char GetExchangeCode()
        {
            return exchangeCode;
        }

        /// <summary>
        /// Returns string representation of this exchange.
        /// It is the string `"COMPOSITE"` for {@link #COMPOSITE} exchange or
        /// exchange character otherwise.
        /// </summary>
        /// <returns>string representation of this exchange.</returns>
        public override string ToString()
        {
            return exchangeCode == '\0' ? "COMPOSITE" : "" + exchangeCode;
        }

        /// <summary>
        /// Indicates whether this exchange attribute is the same as another one.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>`true` if this exchange attribute is the same as another one.</returns>
        public override bool Equals(object o)
        {
            return this == o || o.GetType() == typeof(CandleExchange) && exchangeCode == ((CandleExchange)o).exchangeCode;
        }

        /// <summary>
        /// Returns hash code of this exchange attribute.
        /// </summary>
        /// <returns>hash code of this exchange attribute.</returns>
        public override int GetHashCode()
        {
            return (int)exchangeCode;
        }

        /// <summary>
        /// Returns candle event symbol string with this exchange set.
        /// </summary>
        /// <param name="symbol">original candle event symbol.</param>
        /// <returns>candle event symbol string with this exchange set.</returns>
        public string ChangeAttributeForSymbol(string symbol)
        {
            return MarketEventSymbols.ChangeExchangeCode(symbol, exchangeCode);
        }

        /// <summary>
        /// Internal method that initializes attribute in the candle symbol.
        /// </summary>
        /// <param name="candleSymbol">candle symbol.</param>
        /// <exception cref="InvalidOperationException">if used outside of internal initialization logic.</exception>
        public void CheckInAttributeImpl(CandleSymbol candleSymbol)
        {
            if (candleSymbol.exchange != null)
                throw new InvalidOperationException("Already initialized");
            candleSymbol.exchange = this;
        }

        /// <summary>
        /// Returns exchange attribute object that corresponds to the specified exchange code character.
        /// </summary>
        /// <param name="exchangeCode">exchange code character.</param>
        /// <returns>exchange attribute object.</returns>
        public static CandleExchange ValueOf(char exchangeCode)
        {
            return exchangeCode == '\0' ? COMPOSITE : new CandleExchange(exchangeCode);
        }

        /// <summary>
        /// Returns exchange attribute object of the given candle symbol string.
        /// The result is {@link #DEFAULT} if the symbol does not have exchange attribute.
        /// </summary>
        /// <param name="symbol">candle symbol string.</param>
        /// <returns>exchange attribute object of the given candle symbol string.</returns>
        public static CandleExchange GetAttributeForSymbol(string symbol)
        {
            return ValueOf(MarketEventSymbols.GetExchangeCode(symbol));
        }
    }
}
