#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.util
{
    /// <summary>
    ///     A collection of static utility methods for manipulation of int day id, that is the number
    ///     of days since Unix epoch of January 1, 1970.
    /// </summary>
    public static class DayUtil
    {
        private static readonly int[] DAY_OF_YEAR = { 0, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };

        /// <summary>
        ///     Returns day identifier for specified year, month and day in Gregorian calendar.
        ///     The day identifier is defined as the number of days since Unix epoch of January 1, 1970.
        ///     Month must be between 1 and 12 inclusive.Year and day might take arbitrary values assuming
        ///     proleptic Gregorian calendar.The value returned by this method for an arbitrary day value always
        ///     satisfies the following equality:
        ///     {@code GetDayIdByYearMonthDay(year, month, day) == GetDayIdByYearMonthDay(year, month, 0) + day @endcode}
        ///     @throws ArgumentOutOfRangeException when month is less than 1 or more than 12.
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <param name="day">day</param>
        /// <returns>Returns day identifier for specified year, month and day in Gregorian calendar.</returns>
        public static int GetDayIdByYearMonthDay(int year, int month, int day)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException("month", "invalid month " + month);
            var dayOfYear = DAY_OF_YEAR[month] + day - 1;
            if (month > 2 && year % 4 == 0 && (year % 100 != 0 || year % 400 == 0))
                dayOfYear++;
            return year * 365 + MathUtil.Div(year - 1, 4) - MathUtil.Div(year - 1, 100) + MathUtil.Div(year - 1, 400) +
                dayOfYear - 719527;
        }

        /// <summary>
        ///     Returns day identifier for specified yyyymmdd integer in Gregorian calendar.
        ///     The day identifier is defined as the number of days since Unix epoch of January 1, 1970.
        ///     The yyyymmdd integer is equal to `yearSign * (abs(year) * 10000 + month * 100 + day)`, where year,
        ///     month, and day are in Gregorian calendar, month is between 1 and 12 inclusive, and day is counted from 1.
        ///     @throws ArgumentOutOfRangeException when month is less than 1 or more than 12.
        ///     @see #GetDayIdByYearMonthDay(int year, int month, int day)
        /// </summary>
        /// <param name="yyyymmdd"></param>
        /// <returns></returns>
        public static int GetDayIdByYearMonthDay(int yyyymmdd)
        {
            if (yyyymmdd >= 0)
                return GetDayIdByYearMonthDay(yyyymmdd / 10000, yyyymmdd / 100 % 100, yyyymmdd % 100);
            return GetDayIdByYearMonthDay(-(-yyyymmdd / 10000), -yyyymmdd / 100 % 100, -yyyymmdd % 100);
        }

        /// <summary>
        ///     Returns yyyymmdd integer in Gregorian calendar for a specified day identifier.
        ///     The day identifier is defined as the number of days since Unix epoch of January 1, 1970.
        ///     The result is equal to `yearSign * (abs(year) * 10000 + month * 100 + day)`, where year,
        ///     month, and day are in Gregorian calendar, month is between 1 and 12 inclusive, and day is counted from 1.
        /// </summary>
        /// <param name="dayId"></param>
        /// <returns></returns>
        public static int GetYearMonthDayByDayId(int dayId)
        {
            var j = dayId + 2472632; // this shifts the epoch back to astronomical year -4800
            var g = MathUtil.Div(j, 146097);
            var dg = j - g * 146097;
            var c = (dg / 36524 + 1) * 3 / 4;
            var dc = dg - c * 36524;
            var b = dc / 1461;
            var db = dc - b * 1461;
            var a = (db / 365 + 1) * 3 / 4;
            var da = db - a * 365;
            var y = g * 400 + c * 100 + b * 4 +
                    a; // this is the integer number of full years elapsed since March 1, 4801 BC at 00:00 UTC
            var m = (da * 5 + 308) / 153 -
                    2; // this is the integer number of full months elapsed since the last March 1 at 00:00 UTC
            var d = da - (m + 4) * 153 / 5 +
                    122; // this is the number of days elapsed since day 1 of the month at 00:00 UTC
            var yyyy = y - 4800 + (m + 2) / 12;
            var mm = (m + 2) % 12 + 1;
            var dd = d + 1;
            var yyyymmdd = Math.Abs(yyyy) * 10000 + mm * 100 + dd;
            return yyyy >= 0 ? yyyymmdd : -yyyymmdd;
        }
    }
}