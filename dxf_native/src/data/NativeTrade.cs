using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native.data {
	public struct NativeTrade : IDxTrade {
		private readonly DxTrade trade;

		internal NativeTrade(DxTrade trade) {
			this.trade = trade;
		}

		#region Implementation of IDxTrade

		public long Time {
			get { return trade.time; }
		}

		public char ExchangeCode {
			get { return trade.exchange_code; }
		}

		public double Price {
			get { return trade.price; }
		}

		public long Size {
			get { return trade.size; }
		}

		public double DayVolume {
			get { return trade.day_volume; }
		}

		#endregion
	}
}