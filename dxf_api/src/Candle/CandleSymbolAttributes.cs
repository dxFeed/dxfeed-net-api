#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.candle
{
    /// <summary>
    /// Class stores all candle symbol attributes
    /// </summary>
    public static class CandleSymbolAttributes
    {
        /// <summary>
        /// Exchange attribute of {@link CandleSymbol} defines exchange identifier where data is
        /// taken from to build the candles.
        /// </summary>
        public static class Exchange
        {
            /// <summary>
            /// Composite exchange where data is taken from all exchanges.
            /// </summary>
            public static readonly ICandleSymbolAttribute COMPOSITE = CandleExchange.COMPOSITE;

            /// <summary>
            /// Default exchange is {@link #COMPOSITE}.
            /// </summary>
            public static readonly ICandleSymbolAttribute DEFAULT = CandleExchange.DEFAULT;

            /// <summary>
            /// Creates a new candle exchange object
            /// </summary>
            /// <param name="code">exchange code</param>
            /// <returns>new candle exchange</returns>
            public static ICandleSymbolAttribute NewExchange(char code)
            {
                return CandleExchange.ValueOf(code);
            }
        }

        /// <summary>
        /// Period attribute of {@link CandleSymbol} defines aggregation period of the candles.
        /// </summary>
        public static class Period
        {
            /// <summary>
            /// Tick aggregation where each candle represents an individual tick.
            /// </summary>
            public static readonly ICandleSymbolAttribute TICK = CandlePeriod.TICK;

            /// <summary>
            /// Day aggregation where each candle represents a day.
            /// </summary>
            public static readonly ICandleSymbolAttribute DAY = CandlePeriod.DAY;

            /// <summary>
            /// Default period is {@link #TICK}.
            /// </summary>
            public static readonly ICandleSymbolAttribute DEFAULT = CandlePeriod.DEFAULT;

            /// <summary>
            /// Creates a new candle period with certain value and type
            /// </summary>
            /// <param name="period">value of candle period</param>
            /// <param name="type">type of candle period</param>
            /// <returns>new candle period</returns>
            public static ICandleSymbolAttribute NewPeriod(double period, CandleType type)
            {
                return CandlePeriod.ValueOf(period, type);
            }
        }

        /// <summary>
        /// Price type attribute of {@link CandleSymbol} defines price that is used to build the candles.
        /// </summary>
        public static class Price
        {
            /// <summary>
            /// Last trading price.
            /// </summary>
            public static readonly ICandleSymbolAttribute LAST = CandlePrice.LAST;

            /// <summary>
            /// Quote bid price.
            /// </summary>
            public static readonly ICandleSymbolAttribute BID = CandlePrice.BID;

            /// <summary>
            /// Quote ask price.
            /// </summary>
            public static readonly ICandleSymbolAttribute ASK = CandlePrice.ASK;

            /// <summary>
            /// Market price defined as average between quote bid and ask prices.
            /// </summary>
            public static readonly ICandleSymbolAttribute MARK = CandlePrice.MARK;

            /// <summary>
            /// Official settlement price that is defined by exchange or last trading price otherwise.
            /// It updates based on all {@link PriceType PriceType} values:
            /// {@link PriceType#INDICATIVE}, {@link PriceType#PRELIMINARY}, and {@link PriceType#FINAL}.
            /// </summary>
            public static readonly ICandleSymbolAttribute SETTLEMENT = CandlePrice.SETTLEMENT;

            /// <summary>
            /// Default price type is {@link #LAST}.
            /// </summary>
            public static readonly ICandleSymbolAttribute DEFAULT = CandlePrice.DEFAULT;

            /// <summary>
            /// Parses string representation of candle price type into object.
            /// Any string that was returned by {@link #toString()} can be parsed
            /// and case is ignored for parsing.
            /// </summary>
            /// <param name="s">string representation of candle price type.</param>
            /// <returns>candle price type.</returns>
            /// <exception cref="InvalidOperationException">if the string representation is invalid.</exception>
            public static ICandleSymbolAttribute Parse(string s)
            {
                return CandlePrice.Parse(s);
            }
        }

        /// <summary>
        /// Session attribute of {@link CandleSymbol} defines trading that is used to build the candles.
        /// </summary>
        public static class Session
        {
            /// <summary>
            /// All trading sessions are used to build candles.
            /// </summary>
            public static readonly ICandleSymbolAttribute ANY = CandleSession.ANY;

            /// <summary>
            /// Only regular trading session data is used to build candles.
            /// </summary>
            public static readonly ICandleSymbolAttribute REGULAR = CandleSession.REGULAR;

            /// <summary>
            /// Default trading session is {@link #ANY}.
            /// </summary>
            public static readonly ICandleSymbolAttribute DEFAULT = CandleSession.DEFAULT;

            /// <summary>
            /// Parses string representation of candle session attribute into object.
            /// Any string that was returned by {@link #toString()} can be parsed
            /// and case is ignored for parsing.
            /// </summary>
            /// <param name="s">string representation of candle candle session attribute.</param>
            /// <returns>candle session attribute.</returns>
            /// <exception cref="InvalidOperationException">if the string representation is invalid.</exception>
            public static ICandleSymbolAttribute Parse(string s)
            {
                return CandleSession.Parse(s);
            }
        }

        /// <summary>
        /// Candle alignment attribute of {@link CandleSymbol} defines how candle are aligned with respect to time.
        /// </summary>
        public static class Alignment
        {
            /// <summary>
            /// Align candles on midnight.
            /// </summary>
            public static readonly ICandleSymbolAttribute MIDNIGHT = CandleAlignment.MIDNIGHT;

            /// <summary>
            /// Align candles on trading sessions.
            /// </summary>
            public static readonly ICandleSymbolAttribute SESSION = CandleAlignment.SESSION;

            /// <summary>
            /// Default alignment is {@link #MIDNIGHT}.
            /// </summary>
            public static readonly ICandleSymbolAttribute DEFAULT = CandleAlignment.DEFAULT;

            /// <summary>
            /// Parses string representation of candle alignment into object.
            /// Any string that was returned by {@link #toString()} can be parsed
            /// and case is ignored for parsing.
            ///
            /// </summary>
            /// <param name="s">string representation of candle alignment.</param>
            /// <returns>candle alignment</returns>
            /// <exception cref="ArgumentNullException">Canlde alignment in string is unknown</exception>
            public static ICandleSymbolAttribute Parse(string s)
            {
                return CandleAlignment.Parse(s);
            }
        }
    }
}
