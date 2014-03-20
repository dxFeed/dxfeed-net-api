using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events {
	public struct NativeTrade : IDxTrade {
		private readonly DxTrade trade;

		internal unsafe NativeTrade(DxTrade* trade) {
			this.trade = *trade;
		}

		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "Trade {{Time: {0}, ExchangeCode: '{1}', Price: {2}, Size: {3}, DayVolume: {4}}}",
				Time, ExchangeCode, Price, Size, DayVolume);
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