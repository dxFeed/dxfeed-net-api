#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

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
