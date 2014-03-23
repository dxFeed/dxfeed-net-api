using System;
using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events {
	public struct NativeQuote : IDxQuote {
		private DxQuote quote;

		internal unsafe NativeQuote(DxQuote* quote) {
			this.quote = *quote;
		}

		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "Quote: {{AskExchangeCode: '{0}', Ask: {2}@{1}, AskTime: {3:o}, BidExchangeCode: '{4}', Bid: {6}@{5}, BidTime: {7:o} }}",
				AskExchangeCode, AskPrice, AskSize, AskTime, BidExchangeCode, BidPrice, BidSize, BidTime);
		}

		#region Implementation of IDxQuote

		public DateTime BidTime {
			get { return TimeConverter.ToUtcDateTime(quote.bid_time); }
		}

		public char BidExchangeCode {
			get { return quote.bid_exchange_code; }
		}

		public double BidPrice {
			get { return quote.bid_price; }
		}

		public long BidSize {
			get { return quote.bid_size; }
		}

		public DateTime AskTime {
			get { return TimeConverter.ToUtcDateTime(quote.ask_time); }
		}

		public char AskExchangeCode {
			get { return quote.ask_exchange_code; }
		}

		public double AskPrice {
			get { return quote.ask_price; }
		}

		public long AskSize {
			get { return quote.ask_size; }
		}

		#endregion
	}
}