using System;

namespace com.dxfeed.api {

    /// <summary>
    /// Class provides some helpers for library.
    /// </summary>
    public class Tools {

        /// <summary>
        /// Convert date time value to unix time (the number of milliseconds from 1.1.1970).
        /// </summary>
        /// <param name="dateTime">Date time value to convert.</param>
        /// <returns>Unix time value of date time.</returns>
        /// <exception cref="System.FormatException"></exception>
        public static Int64 DateToUnixTime(DateTime dateTime) {
            try {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = (DateTime)dateTime - origin;
                return Convert.ToInt64(Math.Floor(diff.TotalMilliseconds));
            } catch (Exception exc) {
                throw new FormatException("DateToUnixTime failed: " + exc.ToString());
            }
        }

        /// <summary>
        /// Convert unix time to corresponding date time value.
        /// </summary>
        /// <param name="unixTimeStamp">Unix time (the number of milliseconds from 1.1.1970).</param>
        /// <returns>Date time value of unix time stamp.</returns>
        /// <exception cref="System.FormatException"></exception>
        public static DateTime UnixTimeToDate(double unixTimeStamp) {
            try {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToUniversalTime();
                return dtDateTime;
            } catch (Exception exc) {
                throw new FormatException("UnixTimeToDate failed:" + exc.ToString());
            }
        }

        /// <summary>
        /// Compares two double values with some epsilon.
        /// </summary>
        /// <param name="double1">First double to compare.</param>
        /// <param name="double2">Second double to compare.</param>
        /// <returns>True if doubles are differs not greater than some epsilon, otherwise returns false.</returns>
        public static bool IsEquals(double double1, double double2) {
            //TODO: diff
            double difference = 0.000001d;
            return Math.Abs(double1 - double2) <= difference;
        }

    }
}
