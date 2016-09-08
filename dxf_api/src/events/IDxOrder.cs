using System;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events {
	public interface IDxOrder : IDxMarketEvent {
		long Index { get; }
		Side Side { get; }
		int Level { get; }
		DateTime Time { get; }
		char ExchangeCode { get; }
		DxString MarketMaker { get; }
		double Price { get; }
		long Size { get; }
		string Source { get; }
	}
}