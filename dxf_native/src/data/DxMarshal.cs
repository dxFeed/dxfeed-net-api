using System;
using System.Runtime.InteropServices;
using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native.data {
	public unsafe class DxMarshal {
		private static readonly int QUOTE_SIZE;
		private static readonly int ORDER_SIZE;
		private static readonly int TRADE_SIZE;
		private static readonly int FUNDAMENTAL_SIZE;
		private static readonly int PROFILE_SIZE;
		private static readonly int MM_SIZE;
		private static readonly int TS_SIZE;

		static DxMarshal() {
			QUOTE_SIZE = sizeof (DxQuote);
			ORDER_SIZE = sizeof (DxOrder);
			TRADE_SIZE = sizeof (DxTrade);
			FUNDAMENTAL_SIZE = sizeof (DxFundamental);
			PROFILE_SIZE = sizeof (DxProfile);
			MM_SIZE = sizeof (DxMarketMaker);
			TS_SIZE = sizeof (DxTimeAndSale);
		}

		public static string ReadString(IntPtr ptr) {
			return Marshal.PtrToStringUni(ptr);
		}

		public static NativeQuote ReadQuote(IntPtr head, int offset) {
			return new NativeQuote((DxQuote*) IntPtr.Add(head, offset*QUOTE_SIZE));
		}

		public static NativeOrder ReadOrder(IntPtr head, int offset) {
			return new NativeOrder((DxOrder*) IntPtr.Add(head, offset*ORDER_SIZE));
		}

		public static NativeTrade ReadTrade(IntPtr head, int offset) {
			return new NativeTrade((DxTrade*) IntPtr.Add(head, offset*TRADE_SIZE));
		}

		public static NativeFundamental ReadFundamental(IntPtr head, int offset) {
			return new NativeFundamental((DxFundamental*)IntPtr.Add(head, offset * FUNDAMENTAL_SIZE));
		}

		public static NativeProfile ReadProfile(IntPtr head, int offset) {
			return new NativeProfile((DxProfile*) IntPtr.Add(head, offset*PROFILE_SIZE));
		}

		public static NativeMarketMaker ReadMarketMaker(IntPtr head, int offset) {
			return new NativeMarketMaker((DxMarketMaker*) IntPtr.Add(head, offset*MM_SIZE));
		}

		public static NativeTimeAndSale ReadTimeAndSale(IntPtr head, int offset) {
			return new NativeTimeAndSale((DxTimeAndSale*) IntPtr.Add(head, offset*TS_SIZE));
		}

		public static DxString ReadDxString(IntPtr ptr) {
			return new DxString((char *)ptr.ToPointer());
		}

	}
}