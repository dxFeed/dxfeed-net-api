#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Text;

namespace com.dxfeed.util
{
    /// <summary>
    /// Value class for period of time with support for ISO8601 duration format.
    /// </summary>
    public class TimePeriod
    {
        /// <summary>
        /// Time-period of zero.
        /// </summary>
        public static readonly TimePeriod ZERO = new TimePeriod(0);

        /// <summary>
        /// Returns TimePeriod with value milliseconds.
        /// </summary>
        /// <param name="value">Value in milliseconds.</param>
        /// <returns>TimePeriod with value milliseconds.</returns>
        public static TimePeriod ValueOf(long value)
        {
            return value == 0 ? ZERO : new TimePeriod(value);
        }

        /// <summary>
        /// Returns TimePeriod represented with a given string.
        ///
        /// Allowable format is ISO8601 duration, but there are some simplifications and modifications available:
        /// Letters are case insensitive.
        /// Letters "P" and "T" can be omitted.
        /// Letter "S" can be also omitted. In this case last number will be supposed to be seconds.
        /// Number of seconds can be fractional. So it is possible to define duration accurate within milliseconds.
        /// Every part can be omitted. It is supposed that it's value is zero then.
        /// </summary>
        /// <param name="value">String representation.</param>
        /// <returns>TimePeriod represented with a given string.</returns>
        /// <exception cref="FormatException">if cannot parse value.</exception>
        public static TimePeriod ValueOf(string value)
        {
            return ValueOf(Parse(value));
        }

        // value in milliseconds
        private long value;

        protected TimePeriod(long value)
        {
            this.value = value;
        }

        protected static long Parse(string value)
        {
            try
            {
                bool metAnyPart = false;
                value = value.ToUpper() + '#';
                long res = 0;
                int i = 0;
                if (value[i] == 'P')
                {
                    i++;
                }
                int j = i;
                while (Char.IsDigit(value[j]))
                {
                    j++;
                }
                if (value[j] == 'D')
                {
                    res += Int32.Parse(value.Substring(i, j - i));
                    metAnyPart = true;
                    j++;
                    i = j;
                    while (Char.IsDigit(value[j]))
                    {
                        j++;
                    }
                }
                res *= 24;
                if (value[j] == 'T')
                {
                    if (i != j)
                    {
                        throw new FormatException("Wrong time period format.");
                    }
                    j++;
                    i = j;
                    while (Char.IsDigit(value[j]))
                    {
                        j++;
                    }
                }
                if (value[j] == 'H')
                {
                    res += Int32.Parse(value.Substring(i, j - i));
                    metAnyPart = true;
                    j++;
                    i = j;
                    while (Char.IsDigit(value[j]))
                    {
                        j++;
                    }
                }
                res *= 60;
                if (value[j] == 'M')
                {
                    res += Int32.Parse(value.Substring(i, j - i));
                    metAnyPart = true;
                    j++;
                    i = j;
                    while (Char.IsDigit(value[j]))
                    {
                        j++;
                    }
                }
                res *= 60 * 1000;
                if (value[j] == '.')
                {
                    j++;
                    while (Char.IsDigit(value[j]))
                    {
                        j++;
                    }
                }
                if (i != j)
                {
                    res += (long)Math.Round(Double.Parse(value.Substring(i, j - i)) * 1000);
                    metAnyPart = true;
                }
                bool good = ((value[j] == 'S') && (j == value.Length - 2) && (i != j)) ||
                    ((value[j] == '#') && (j == value.Length - 1));
                good &= metAnyPart;
                if (!good)
                {
                    throw new FormatException("Wrong time period format.");
                }
                return res;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FormatException("Wrong time period format:" + e);
            }
        }

        public override string ToString()
        {
            long v = value;
            long millis = v % 1000;
            v = v / 1000;
            long secs = v % 60;
            v = v / 60;
            long mins = v % 60;
            v = v / 60;
            long hours = v % 24;
            v = v / 24;
            long days = v;
            StringBuilder result = new StringBuilder();
            result.Append('P');
            if (days > 0)
            {
                result.Append(days).Append("D");
            }
            result.Append('T');
            if (hours > 0)
            {
                result.Append(hours).Append("H");
            }
            if (mins > 0)
            {
                result.Append(mins).Append("M");
            }
            if (millis > 0)
            {
                result.Append((secs * 1000 + millis) / 1000d);
            }
            else
            {
                result.Append(secs);
            }
            result.Append("S");
            return result.ToString();
        }

        /// <summary>
        /// Returns value in milliseconds.
        /// </summary>
        /// <returns>Value in milliseconds.</returns>
        public long GetTime()
        {
            return value;
        }

        /// <summary>
        /// Returns value in nanoseconds.
        /// </summary>
        /// <returns>Value in nanoseconds.</returns>
        public long GetNanos()
        {
            return 1000000 * value;
        }

        public override bool Equals(object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;
            TimePeriod that = (TimePeriod)o;
            return value == that.value;
        }

        public override int GetHashCode()
        {
            return (int)value;
        }
    }
}
