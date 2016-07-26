using System;

namespace com.dxfeed.api {
    public class Tools {

        public static Int64 DateToUnixTime(DateTime dateTime) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = (DateTime)dateTime - origin;
            return Convert.ToInt64(Math.Floor(diff.TotalMilliseconds));
        }

        public static DateTime UnixTimeToDate(double unixTimeStamp) {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static bool IsEquals(double double1, double double2) {
            //TODO: diff
            double difference = 0.000001d;
            return Math.Abs(double1 - double2) <= difference;
        }

    }
}
