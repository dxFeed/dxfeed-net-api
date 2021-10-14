#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.Collections.Generic;
using System.Text;
using com.dxfeed.api.candle;

namespace com.dxfeed.api.util
{
    /// <summary>
    ///     A collection of static utility methods for string manipulation
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        ///     Parses the comma list of candle symbols
        /// </summary>
        /// <param name="symbols">The comma separated list of candle symbols</param>
        /// <returns>The list of candle symbols</returns>
        public static List<CandleSymbol> ParseCandleSymbols(string symbols)
        {
            var result = new List<CandleSymbol>();

            if (string.IsNullOrEmpty(symbols)) return result;

            var symbolParams = false;
            var sb = new StringBuilder();
            foreach (var t in symbols)
                switch (t)
                {
                    case '{':
                        sb.Append(t);
                        symbolParams = true;
                        break;
                    case '}':
                        sb.Append(t);
                        symbolParams = false;
                        break;
                    case ',':
                        if (symbolParams)
                        {
                            sb.Append(t);
                        }
                        else
                        {
                            result.Add(CandleSymbol.ValueOf(sb.ToString()));
                            sb.Clear();
                        }

                        break;
                    default:
                        sb.Append(t);
                        break;
                }

            if (sb.Length > 0) result.Add(CandleSymbol.ValueOf(sb.ToString()));

            return result;
        }
    }
}