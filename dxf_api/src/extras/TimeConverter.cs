#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.extras
{
    public class TimeConverter
    {
        private static readonly DateTime offset = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToUtcDateTime(long utcMillis)
        {
            return offset.AddMilliseconds(utcMillis);
        }

        public static long ToUnixTime(DateTime time)
        {
            return (time - offset).Milliseconds;
        }
    }
}
