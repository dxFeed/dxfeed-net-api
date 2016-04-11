using System;
using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events {
	public struct NativeOrder : IDxOrder {
		private readonly DxOrder order;
		private readonly DxString marketMaker;
		private readonly string source;

		internal unsafe NativeOrder(DxOrder* order) {
			this.order = *order;
			marketMaker = DxMarshal.ReadDxString(this.order.market_maker);

			fixed (char* charPtr = this.order.source) {
				source = new string(charPtr);
			}
		}

		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "Order: {{{1} {7}@{6}, Index: {0:x4}, Level: {2}, Time: {3:o}, ExchangeCode: '{4}', MarketMaker: '{5}', Source: '{8}'}}",
				Index, Side, Level, Time, ExchangeCode, MarketMaker, Price, Size, Source);
		}

		#region Implementation of IDxOrder

		public long Index {
			get { return order.index; }
		}

		public Side Side {
			get { return order.side; }
		}

		public int Level {
			get { return order.level; }
		}

		public DateTime Time {
			get { return TimeConverter.ToUtcDateTime(order.time); }
		}

		public char ExchangeCode {
			get { return order.exchange_code; }
		}

		public DxString MarketMaker {
			get { return marketMaker; }
		}

		public double Price {
			get { return order.price; }
		}

		public long Size {
			get { return order.size; }
		}

		public string Source
		{
			get { return source; }
		}

		#endregion
	}
}