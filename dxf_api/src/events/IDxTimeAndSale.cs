using System;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events {
	public interface IDxTimeAndSale {
		long EventId { get; }
		DateTime Time { get; }
		char ExchangeCode { get; }
		double Price { get; }
		long Size { get; }
		double BidPrice { get; }
		double AskPrice { get; }
		DxString ExchangeSaleConditions { get; }
		bool IsTrade { get; }
		int Type { get; }
	}


}