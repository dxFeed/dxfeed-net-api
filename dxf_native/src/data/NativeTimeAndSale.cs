using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native.data {
	public struct NativeTimeAndSale : IDxTimeAndSale {
		private readonly DxTimeAndSale ts;
		private readonly DxString saleCond;

		internal unsafe NativeTimeAndSale(DxTimeAndSale* ts) {
			this.ts = *ts;
			saleCond = DxMarshal.ReadDxString(this.ts.exchange_sale_conditions);
		}

		#region Implementation of IDxTimeAndSale

		public long EventId {
			get { return ts.event_id; }
		}

		public long Time {
			get { return ts.time; }
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