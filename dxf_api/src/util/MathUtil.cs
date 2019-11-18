#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.util
{
    /// <summary>
    /// A collection of non-trivial mathematical utility methods.
    /// </summary>
    class MathUtil
    {
        private MathUtil() { } // to prevent accidental initialization

        private static readonly int MAX_DECIMAL_DIGITS = 14;
        private static readonly long[] POW10 = new long[MAX_DECIMAL_DIGITS + 1];

        static MathUtil()
        {
            POW10[0] = 1;
            for (int i = 1; i < POW10.Length; i++)
                POW10[i] = POW10[i - 1] * 10;
        }

        /// <summary>
        /// Rounds a specified double number to a decimal number with at most
        /// 14 significant digits and at most 14 digits after decimal point.
        /// When x is integer, NaN or infinity, then x is returned
        /// (this method does not round big integers to powers of 10).
        ///
        /// <p>For example, suppose you have 1 dollar and 10 cents and you pay 20 cent.
        /// You should keep 90 cents.However, the following expression is <b>false</b> in Java:
        /// <pre>1.1 - 0.2 == 0.9</pre>
        /// because both 1.1 and 0.2 do not have precise representations in `double`.
        /// To make this comparison work, you have to use `roundDecimal` method:
        /// <pre>roundDecimal(1.1 - 0.2) == 0.9</pre>
        ///
        /// <p>As a general rule, you should use `roundDecimal` after any operation
        /// (addition, subtraction, multiplication, division) on two decimal numbers if you
        /// know that the result is a decimal with at most 14 significant digits and at most
        /// 14 digits after decimal point.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double RoundDecimal(double x)
        {
            if (double.IsNaN(x) || x == Math.Floor(x))
                return x; // integer, NaN, or +/- inf
            double signum = Math.Sign(x);
            double abs = Math.Abs(x);
            int pow = Math.Min(MAX_DECIMAL_DIGITS, MAX_DECIMAL_DIGITS - 1 - (int)(Math.Floor(Math.Log10(abs))));
            for (int i = pow; i >= 0; i--)
            {
                long mantissa = (long)(POW10[i] * abs + 0.5);
                if (mantissa < POW10[MAX_DECIMAL_DIGITS])
                    return signum * mantissa / POW10[i];
            }
            // Mantissa >= 10^14 with fractions -- just round
            return Math.Round(x);
        }

        /// <summary>
        /// Returns quotient according to number theory - i.e. when remainder is zero or positive.
        /// </summary>
        /// <param name="a">dividend</param>
        /// <param name="b">divisor</param>
        /// <returns>quotient according to number theory</returns>
        public static int Div(int a, int b)
        {
            return a >= 0 ? a / b : b >= 0 ? (a + 1) / b - 1 : (a + 1) / b + 1;
        }

        /// <summary>
        /// Returns quotient according to number theory - i.e. when remainder is zero or positive.
        /// </summary>
        /// <param name="a">dividend</param>
        /// <param name="b">divisor</param>
        /// <returns>quotient according to number theory</returns>
        public static long Div(long a, long b)
        {
            return a >= 0 ? a / b : b >= 0 ? (a + 1) / b - 1 : (a + 1) / b + 1;
        }

        /// <summary>
        /// Returns remainder according to number theory - i.e. when remainder is zero or positive.
        /// </summary>
        /// <param name="a">dividend</param>
        /// <param name="b">divisor</param>
        /// <returns>remainder according to number theory</returns>
        public static int Rem(int a, int b)
        {
            int r = a % b;
            return r >= 0 ? r : b >= 0 ? r + b : r - b;
        }

        /// <summary>
        /// Returns remainder according to number theory - i.e. when remainder is zero or positive.
        /// </summary>
        /// <param name="a">dividend</param>
        /// <param name="b">divisor</param>
        /// <returns>remainder according to number theory</returns>
        public static long Rem(long a, long b)
        {
            long r = a % b;
            return r >= 0 ? r : b >= 0 ? r + b : r - b;
        }
    }
}
