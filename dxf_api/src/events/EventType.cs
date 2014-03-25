using System;

namespace com.dxfeed.api.events {
	[Flags]
	public enum EventType : int {
		None = 0,
		Trade = 1,
		Quote = 2,
		Summary = 4,
		Profile = 8,
		Order = 16,
		TimeAndSale = 32
	}
}