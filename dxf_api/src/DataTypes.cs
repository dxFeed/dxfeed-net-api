using System;

namespace com.dxfeed.api {

	public enum Side : int {
		Sell = 0,
		Buy = 1
	}

	public interface IDxOrder {
		long Index { get; }
		Side Side { get; }
		int Level { get; }
		DateTime Time { get; }
		char ExchangeCode { get; }
		DxString MarketMaker { get; }
		double Price { get; }
		long Size { get; }
	}

	public interface IDxTrade {
		DateTime Time { get; }
		char ExchangeCode { get; }
		double Price { get; }
		long Size { get; }
		double DayVolume { get; }
	}

	public interface IDxQuote {
		DateTime BidTime { get; }
		char BidExchangeCode { get; }
		double BidPrice { get; }
		long BidSize { get; }
		DateTime AskTime { get; }
		char AskExchangeCode { get; }
		double AskPrice { get; }
		long AskSize { get; }
	}

	public interface IDxFundamental {
		double DayHighPrice { get; }
		double DayLowPrice { get; }
		double DayOpenPrice { get; }
		double PrevDayClosePrice { get; }
		long OpenInterest { get; }
	}

	public interface IDxProfile {
		string Description { get; }
	}

	public interface IDxMarketMaker {
		char Exchange { get; }
		int Id { get; }
		double BidPrice { get; }
		int BidSize { get; }
		double AskPrice { get; }
		int AskSize { get; }
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
		int Type { get; }
	}


}