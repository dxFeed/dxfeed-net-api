using System;
using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events {
	public struct NativeTimeAndSale : IDxTimeAndSale {
		private readonly DxTimeAndSale ts;
		private readonly DxString saleCond;

		internal unsafe NativeTimeAndSale(DxTimeAndSale* ts) {
			this.ts = *ts;
			saleCond = DxMarshal.ReadDxString(this.ts.exchange_sale_conditions);
		}

		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "TimeAndSale: {{EventId: {0:x4}, Time: {1:o}, ExchangeCode: '{2}', Ask: {3}, Bid: {4}, ExchangeSaleConditions: '{5}', IsTrade: {6}, Price: {7}, Size: {8}, Type: {9}}}",
				EventId, Time, ExchangeCode, AskPrice, BidPrice, ExchangeSaleConditions, IsTrade, Price, Size, Type);
		}

		#region Implementation of IDxTimeAndSale

		public long EventId {
			get { return ts.event_id; }
		}

		public DateTime Time {
			get { return TimeConverter.ToUtcDateTime(ts.time); }
		}

		public char ExchangeCode {
			get { return ts.exchange_code; }
		}

		public double Price {
			get { return ts.price; }
		}

		public long Size {
			get { return ts.size; }
		}

		public double BidPrice {
			get { return ts.bid_price; }
		}

		public double AskPrice {
			get { return ts.ask_price; }
		}

		public DxString ExchangeSaleConditions {
			get { return saleCond; }
		}

		public bool IsTrade {
			get { return ts.is_trade; }
		}

		public int Type {
			get { return ts.type; }
		}

		#endregion
	}
}