using System;

namespace com.dxfeed.api.extras {
	public class TimeConverter {
		 private static readonly DateTime offset = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime ToUtcDateTime(long utcMillis) {
			return offset.AddMilliseconds(utcMillis);
		}
	}
}