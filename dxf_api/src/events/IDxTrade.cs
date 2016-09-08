using System;

namespace com.dxfeed.api.events {
	public interface IDxTrade : IDxMarketEvent {
		DateTime Time { get; }
		char ExchangeCode { get; }
		double Price { get; }
		long Size { get; }
		long Tick { get; }
		double Change { get; }
		double DayVolume { get; }
	}
}