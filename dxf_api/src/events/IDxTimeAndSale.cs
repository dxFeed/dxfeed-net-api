using System;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events {
	public enum TimeAndSaleType {
		New = 0, Correction = 1, Cancel = 2 
	}

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
		TimeAndSaleType Type { get; }
	}


}