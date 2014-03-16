using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events {
	public struct NativeFundamental : IDxFundamental {
		private readonly DxFundamental fundamental;

		internal unsafe NativeFundamental(DxFundamental* fundamental) {
			this.fundamental = *fundamental;
		}

		#region Implementation of IDxFundamental

		public double DayHighPrice {
			get { return fundamental.day_high_price; }
		}

		public double DayLowPrice {
			get { return fundamental.day_low_price; }
		}

		public double DayOpenPrice {
			get { return fundamental.day_open_price; }
		}

		public double PrevDayClosePrice {
			get { return fundamental.prev_day_close_price; }
		}

		public long OpenInterest {
			get { return fundamental.open_interest; }
		}

		#endregion
	}
}