using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native.data {
	public struct NativeOrder : IDxOrder {
		private readonly DxOrder order;
		private readonly DxString marketMaker;

		internal unsafe NativeOrder(DxOrder* order) {
			this.order = *order;
			marketMaker = DxMarshal.ReadDxString(this.order.market_maker);
		}

		#region Implementation of IDxOrder

		public long Index {
			get { return order.index; }
		}

		public int Side {
			get { return order.side; }
		}

		public int Level {
			get { return order.level; }
		}

		public long Time {
			get { return order.time; }
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

		#endregion
	}
}