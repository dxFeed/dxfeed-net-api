using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events {
	public struct NativeSummary : IDxSummary {
		private readonly DxSummary summary;

		internal unsafe NativeSummary(DxSummary* summary) {
			this.summary = *summary;
		}

		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "Summary: {{DayHighPrice: {0}, DayLowPrice: {1}, DayOpenPrice: {2}, PrevDayClosePrice: {3}, OpenInterest: {4} }}",
				DayHighPrice, DayLowPrice, DayOpenPrice, PrevDayClosePrice, OpenInterest);
		}

		#region Implementation of IDxFundamental

		public double DayHighPrice {
			get { return summary.day_high_price; }
		}

		public double DayLowPrice {
			get { return summary.day_low_price; }
		}

		public double DayOpenPrice {
			get { return summary.day_open_price; }
		}

		public double PrevDayClosePrice {
			get { return summary.prev_day_close_price; }
		}

		public long OpenInterest {
			get { return summary.open_interest; }
		}

		#endregion
	}
}