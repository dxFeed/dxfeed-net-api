/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;

namespace com.dxfeed.api.candle
{
    /// <summary>
    /// Type of the candle aggregation period constitutes {@link CandlePeriod} type together
    /// its actual {@link CandlePeriod#getValue() value}.
    /// </summary>
    public class CandleType
    {
        private static Dictionary<string, CandleType> objCash = new Dictionary<string, CandleType>();

        /// <summary>
        /// Certain number of ticks.
        /// </summary>
        public static readonly CandleType TICK = new CandleType(0, "t", 0);

        /// <summary>
        /// Certain number of seconds.
        /// </summary>
        public static readonly CandleType SECOND = new CandleType(1, "s", 1000L);

        /// <summary>
        /// Certain number of minutes.
        /// </summary>
        public static readonly CandleType MINUTE = new CandleType(2, "m", 60 * 1000L);

        /// <summary>
        /// Certain number of hours.
        /// </summary>
        public static readonly CandleType HOUR = new CandleType(3, "h", 60 * 60 * 1000L);

        /// <summary>
        /// Certain number of days.
        /// </summary>
        public static readonly CandleType DAY = new CandleType(4, "d", 24 * 60 * 60 * 1000L);

        /// <summary>
        /// Certain number of weeks.
        /// </summary>
        public static readonly CandleType WEEK = new CandleType(5, "w", 7 * 24 * 60 * 60 * 1000L);

        /// <summary>
        /// Certain number of months.
        /// </summary>
        public static readonly CandleType MONTH = new CandleType(6, "mo", 30 * 24 * 60 * 60 * 1000L);

        /// <summary>
        /// Certain number of option expirations.
        /// </summary>
        public static readonly CandleType OPTEXP = new CandleType(7, "o", 30 * 24 * 60 * 60 * 1000L);

        /// <summary>
        /// Certain number of years.
        /// </summary>
        public static readonly CandleType YEAR = new CandleType(8, "y", 365 * 24 * 60 * 60 * 1000L);

        /// <summary>
        /// Certain volume of trades.
        /// </summary>
        public static readonly CandleType VOLUME = new CandleType(9, "v", 0);

        /// <summary>
        /// Certain price change, calculated according to the following rules:
        /// <ol>
        ///     <li>high(n) - low(n) = price range</li>
        ///     <li>close(n) = high(n) or close(n) = low(n)</li>
        ///     <li>open(n+1) = close(n)</li>
        /// where n is the number of the bar.
        /// </ol>
        /// </summary>
        public static readonly CandleType PRICE = new CandleType(10, "p", 0);

        /// <summary>
        /// Certain price change, calculated according to the following rules:
        /// <ol>
        ///     <li>high(n) - low(n) = price range</li>
        ///     <li>close(n) = high(n) or close(n) = low(n)</li>
        ///     <li>open(n+1) = close(n) + tick size, if close(n) = high(n)</li>
        ///     <li>open(n+1) = close(n) - tick size, if close(n) = low(n)</li>
        /// where n is the number of the bar.
        /// </ol>
        /// </summary>
        public static readonly CandleType PRICE_MOMENTUM = new CandleType(11, "pm", 0);

        /// <summary>
        /// Certain price change, calculated according to the following rules:
        /// <ol>
        ///     <li>high(n+1) - high(n) = price range or low(n) - low(n+1) = price range</li>
        ///     <li>close(n) = high(n) or close(n) = low(n)</li>
        ///     <li>open(n+1) = high(n), if high(n+1) - high(n) = price range</li>
        ///     <li>open(n+1) = low(n), if low(n) - low(n+1) = price range</li>
        /// where n is the number of the bar.
        /// </ol>
        /// </summary>
        public static readonly CandleType PRICE_RENKO = new CandleType(12, "pr", 0);

        private readonly int typeId;
        private readonly string typeStr;
        private readonly long periodIntervalMillis;

        CandleType(int typeId, string typeStr, long periodIntervalMillis)
        {
            this.typeId = typeId;
            this.typeStr = typeStr;
            this.periodIntervalMillis = periodIntervalMillis;

            objCash.Add(typeStr, this);
        }

        /// <summary>
        /// Get id of candle period type
        /// </summary>
        public int Id
        {
            get
            {
                return typeId;
            }
        }

        /// <summary>
        /// Returns candle type period in milliseconds as closely as possible.
        /// Certain types like {@link #SECOND SECOND} and
        /// {@link #DAY DAY} span a specific number of milliseconds.
        /// {@link #MONTH}, {@link #OPTEXP} and {@link #YEAR}
        /// are approximate. Candle type period of
        /// {@link #TICK}, {@link #VOLUME}, {@link #PRICE},
        /// {@link #PRICE_MOMENTUM} and {@link #PRICE_RENKO}
        /// is not defined and this method returns {@code 0}.
        /// </summary>
        /// <returns>aggregation period in milliseconds.</returns>
        public long GetPeriodIntervalMillis()
        {
            return periodIntervalMillis;
        }

        /// <summary>
        /// Returns string representation of this candle type.
        /// The string representation of candle type is the shortest unique prefix of the
        /// lower case string that corresponds to its {@link #name() name}. For example,
        /// {@link #TICK} is represented as {@code "t"}, while {@link #MONTH} is represented as
        /// {@code "mo"} to distinguish it from {@link #MINUTE} that is represented as {@code "m"}.
        /// @return string representation of this candle price type.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return typeStr;
        }

        /// <summary>
        /// Parses string representation of candle type into object.
        /// Any string that that is a prefix of candle type {@link #name()} can be parsed
        /// (including the one that was returned by {@link #toString()})
        /// and case is ignored for parsing.
        /// </summary>
        /// <param name="s">string representation of candle type.</param>
        /// <returns>candle type.</returns>
        /// <exception cref="ArgumentException">if the string representation is invalid.</exception>
        public static CandleType Parse(string s)
        {
            int n = s.Length;
            if (n == 0)
                throw new ArgumentException("Missing candle type");
            // fast path to reverse toString result
            if (objCash.ContainsKey(s))
                return objCash[s];
            // slow path for everything else
            foreach (CandleType type in objCash.Values)
            {
                string name = type.ToString();
                if (name.Length >= n && name.Substring(0, n).Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    return type;
                // Ticks, Minutes, Seconds, etc
                if (s.EndsWith("s") && name.Equals(s.Substring(0, n - 1), StringComparison.InvariantCultureIgnoreCase))
                    return type;
            }
            throw new ArgumentException("Unknown candle type: " + s);
        }
    }
}
