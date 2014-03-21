using System;

namespace com.dxfeed.native.events {
	internal class TimeConverter {
		 private static readonly DateTime offset = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime ToDateTime(long utcMillis) {
			return offset.AddMilliseconds(utcMillis);
		}
	}
}