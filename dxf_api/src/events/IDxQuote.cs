using System;

namespace com.dxfeed.api.events {
	[EventTypeAttribute("Quote")]
	public interface IDxQuote : IDxMarketEvent {
		DateTime BidTime { get; }
		char BidExchangeCode { get; }
		double BidPrice { get; }
		long BidSize { get; }
		DateTime AskTime { get; }
		char AskExchangeCode { get; }
		double AskPrice { get; }
		long AskSize { get; }
	}
}