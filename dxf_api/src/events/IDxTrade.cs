using System;

namespace com.dxfeed.api.events {
	public interface IDxTrade {
		DateTime Time { get; }
		char ExchangeCode { get; }
		double Price { get; }
		long Size { get; }
		double DayVolume { get; }
	}
}