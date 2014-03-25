using System;
using System.Collections;
using System.Collections.Generic;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native.events {
	public struct NativeEventBuffer<T> : IDxEventBuf<T> {
		private readonly EventType type;
		private readonly IntPtr head;
		private readonly int size;
		private readonly Func<IntPtr, int, T> readEvent;
		private readonly DxString symbol;


		internal unsafe NativeEventBuffer(EventType type, IntPtr symbol, IntPtr head, int size, Func<IntPtr, int, T> readEvent) {
			this.type = type;
			this.head = head;
			this.size = size;
			this.readEvent = readEvent;
			this.symbol = new DxString((char*)symbol.ToPointer());
		}

		#region Implementation of IEnumerable
		public struct Enumerator : IEnumerator<T> {
			private readonly IntPtr head;
			private readonly int size;
			private readonly Func<IntPtr, int, T> readEvent;
			private T current;
			private int nextRead;

			internal Enumerator(NativeEventBuffer<T> buf) {
				head = buf.head;
				size = buf.size;
				readEvent = buf.readEvent;
				nextRead = 0;
				current = default(T);
			}

			#region Implementation of IDisposable

			public void Dispose() {
			}

			#endregion

			#region Implementation of IEnumerator

			public bool MoveNext() {
				if (nextRead == size) {
					current = default(T);
					return false;
				}
				current = readEvent(head, nextRead);
				nextRead++;
				return true;
			}

			public void Reset() {
				nextRead = 0;
				current = default(T);
			}

			public T Current {
				get {
					if (nextRead == size + 1)
						throw new InvalidOperationException("Out of bound read");
					return current;
				}
			}

			object IEnumerator.Current {
				get { return Current; }
			}

			#endregion
		}

		public Enumerator GetEnumerator() {
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return new Enumerator(this);
		}

		#endregion

		#region Implementation of IDxEventBuf<out T>

		public EventType EventType {
			get { return type; }
		}

		public DxString Symbol {
			get { return symbol; }
		}

		public int Size {
			get { return size; }
		}

		#endregion
	}

	public class NativeBufferFactory {
		private static readonly Func<IntPtr, int, NativeQuote> QUOTE_READER = DxMarshal.ReadQuote;
		private static readonly Func<IntPtr, int, NativeTrade> TRADE_READER = DxMarshal.ReadTrade;
		private static readonly Func<IntPtr, int, NativeOrder> ORDER_READER = DxMarshal.ReadOrder;
		private static readonly Func<IntPtr, int, NativeProfile> PROFILE_READER = DxMarshal.ReadProfile;
		private static readonly Func<IntPtr, int, NativeTimeAndSale> TS_READER = DxMarshal.ReadTimeAndSale;
		private static readonly Func<IntPtr, int, NativeSummary> SUMMARY_READER = DxMarshal.ReadSummary;
		
		
		public static NativeEventBuffer<NativeQuote> CreateQuoteBuf(IntPtr symbol, IntPtr head, int size) {
			return new NativeEventBuffer<NativeQuote>(EventType.Quote, symbol, head, size, QUOTE_READER);
		}

		public static NativeEventBuffer<NativeTrade> CreateTradeBuf(IntPtr symbol, IntPtr head, int size) {
			return new NativeEventBuffer<NativeTrade>(EventType.Trade, symbol, head, size, TRADE_READER);
		}

		public static NativeEventBuffer<NativeOrder> CreateOrderBuf(IntPtr symbol, IntPtr head, int size) {
			return new NativeEventBuffer<NativeOrder>(EventType.Order, symbol, head, size, ORDER_READER);
		}

		public static NativeEventBuffer<NativeProfile> CreateProfileBuf(IntPtr symbol, IntPtr head, int size) {
			return new NativeEventBuffer<NativeProfile>(EventType.Profile, symbol, head, size, PROFILE_READER);
		}

		public static NativeEventBuffer<NativeTimeAndSale> CreateTimeAndSaleBuf(IntPtr symbol, IntPtr head, int size) {
			return new NativeEventBuffer<NativeTimeAndSale>(EventType.TimeAndSale, symbol, head, size, TS_READER);
		}

		public static NativeEventBuffer<NativeSummary> CreateSummaryBuf(IntPtr symbol, IntPtr head, int size) {
			return new NativeEventBuffer<NativeSummary>(EventType.Summary, symbol, head, size, SUMMARY_READER);
		}
	}
}