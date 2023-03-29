#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.ipf
{
    class DateFormat
    {
        private string format = string.Empty;
        private TimeZoneInfo timeZone;

        public DateFormat(string format)
        {
            this.format = format;
        }

        public void SetTimeZone(TimeZoneInfo timeZone)
        {
            this.timeZone = timeZone;
        }

        public string Format(DateTime dateTime)
        {
            return dateTime.ToString(format);
        }

        /// <summary>
        /// Parses text of the given string to produce a date time object.
        /// </summary>
        /// <param name="s">String that should be parsed.</param>
        /// <returns>Parsed value.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        public DateTime Parse(string s)
        {
            return DateTime.ParseExact(s, format, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
