using System;
using System.Runtime.InteropServices;
using com.dxfeed.native.api;

namespace com.dxfeed.native.data {
	public unsafe class DxMarshal {
		private static readonly int QUOTE_SIZE;
		private static readonly int ORDER_SIZE;
		private static readonly int TRADE_SIZE;

		static DxMarshal() {
			QUOTE_SIZE = sizeof (DxQuote);
			ORDER_SIZE = sizeof (DxOrder);
			TRADE_SIZE = sizeof (DxTrade);
		}

		public static string ReadString(IntPtr ptr) {
			return Marshal.PtrToStringUni(ptr);
		}

		public static NativeQuote ReadQuote(IntPtr head, int offset) {
			var dxQuote = *(DxQuote*) IntPtr.Add(head, offset*QUOTE_SIZE);
			return new NativeQuote(dxQuote);
		}

		public static NativeOrder ReadOrder(IntPtr head, int offset, StringCache cache) {
			var dxOrder = *(DxOrder*) IntPtr.Add(head, offset*ORDER_SIZE);
			return new NativeOrder(dxOrder);
		}

		public static NativeTrade ReadTrade(IntPtr head, int offset) {
			var dxTrade = *(DxTrade*) IntPtr.Add(head, offset*TRADE_SIZE);
			return new NativeTrade(dxTrade);
		}

	}
}