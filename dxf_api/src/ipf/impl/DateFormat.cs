using System;

namespace com.dxfeed.ipf {
    class DateFormat {
        private string format = string.Empty;
        private TimeZoneInfo timeZone;

        public DateFormat(string format) {
            this.format = format;
        }

        public void SetTimeZone(TimeZoneInfo timeZone) {
            this.timeZone = timeZone;
        }

        public string Format(DateTime dateTime) {
            return dateTime.ToString(format);
        }

        /// <summary>
        /// Parses text of the given string to produce a date time object.
        /// </summary>
        /// <param name="s">String that should be parsed.</param>
        /// <returns>Parsed value.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public DateTime Parse(string s) {
            return DateTime.ParseExact(s, format, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
