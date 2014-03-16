using System;
using System.Runtime.InteropServices;

namespace com.dxfeed.native.api {

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DxOrder {
		internal long index;
		internal int side;
		internal int level;
		internal long time;
		internal char exchange_code;
		internal IntPtr market_maker; //string
		internal double price;
		internal long size;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DxTrade {
		internal long time;
		internal char exchange_code;
		internal double price;
		internal long size;
		internal double day_volume;
	}


	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DxQuote {
		internal long bid_time;
		internal char bid_exchange_code;
		internal double bid_price;
		internal long bid_size;
		internal long ask_time;
		internal char ask_exchange_code;
		internal double ask_price;
		internal long ask_size;

		public override string ToString() {
			return string.Format("BidTime: {0}, BidExchangeCode: {1}, BidPrice: {2}, BidSize: {3}, AskTime: {4}, AskExchangeCode: {5}, AskPrice: {6}, AskSize: {7}", bid_time, bid_exchange_code, bid_price, bid_size, ask_time, ask_exchange_code, ask_price, ask_size);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct DxFundamental {
		internal double day_high_price;
		internal double day_low_price;
		internal double day_open_price;
		internal double prev_day_close_price;
		internal long open_interest;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class DxProfile {
		internal string description;
	}

	internal class DxMarketMaker {
		char mm_exchange;
		int mm_id;
		double mmbid_price;
		int mmbid_size;
		double mmask_price;
		int mmask_size;
	}

	internal class DxTimeAndSale {
		long event_id;
		long time;
		char exchange_code;
		double price;
		long size;
		double bid_price;
		double ask_price;
		string exchange_sale_conditions;
		bool is_trade;
		int type;
	}
}