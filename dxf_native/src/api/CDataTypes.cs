using System;
using System.Runtime.InteropServices;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native.api {

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal unsafe struct DxOrder {
		internal long index;
		internal Side side;
		internal int level;
		internal long time;
		internal char exchange_code;
		internal IntPtr market_maker; //string
		internal double price;
		internal long size;
		internal fixed char source[5]; //string
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
	internal struct DxSummary {
		internal double day_high_price;
		internal double day_low_price;
		internal double day_open_price;
		internal double prev_day_close_price;
		internal long open_interest;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DxProfile {
		internal IntPtr description;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DxMarketMaker {
		internal char mm_exchange;
		internal int mm_id;
		internal double mmbid_price;
		internal int mmbid_size;
		internal double mmask_price;
		internal int mmask_size;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DxTimeAndSale {
		internal long event_id;
		internal long time;
		internal char exchange_code;
		internal double price;
		internal long size;
		internal double bid_price;
		internal double ask_price;
		internal IntPtr exchange_sale_conditions;
		internal bool is_trade;
		internal TimeAndSaleType type;
	}
}