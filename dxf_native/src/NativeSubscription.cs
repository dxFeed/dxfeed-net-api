using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.native.api;
using com.dxfeed.native.events;

namespace com.dxfeed.native {
	public class NativeSubscription : IDxSubscription {
		private readonly IntPtr connectionPtr;
		private IntPtr subscriptionPtr;
		private readonly IDxFeedListener listener;
        private readonly IDxCandleListener candleListener;
		//to prevent callback from being garbage collected
		private readonly C.dxf_event_listener_t callback;

		public NativeSubscription(NativeConnection connection, EventType eventType, IDxFeedListener listener) {
			if (listener == null)
				throw new ArgumentNullException("listener");

			connectionPtr = connection.Handler;
			this.listener = listener;

			C.CheckOk(C.Instance.dxf_create_subscription(connectionPtr, eventType, out subscriptionPtr));
			try {
				C.CheckOk(C.Instance.dxf_attach_event_listener(subscriptionPtr, callback = OnEvent, IntPtr.Zero));
			} catch (DxException) {
				C.Instance.dxf_close_subscription(subscriptionPtr);
				throw;
			}
		}

        public NativeSubscription(NativeConnection connection, CandleSymbol symbol, DateTime? time, IDxCandleListener listener) {
            if (listener == null)
                throw new ArgumentNullException("listener");

            connectionPtr = connection.Handler;
            this.candleListener = listener;

            IntPtr candleAttributesPtr = null;
            C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.GetBaseSymbol()));

            if (time == null) {
                C.CheckOk(C.Instance.dxf_create_subscription(connectionPtr, EventType.Candle, out subscriptionPtr));
            } else {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = (DateTime)time - origin;
                Int64 unix_timestamp = Convert.ToInt64(Math.Floor(diff.TotalMilliseconds));
                C.CheckOk(C.Instance.dxf_create_subscription_timed(connectionPtr, EventType.Candle, unix_timestamp, out subscriptionPtr));
            }

            try {
                C.CheckOk(C.Instance.dxf_attach_event_listener(subscriptionPtr, callback = OnEvent, IntPtr.Zero));
            } catch (DxException) {
                C.Instance.dxf_close_subscription(subscriptionPtr);
                throw;
            }
        }

		private void OnEvent(EventType eventType, IntPtr symbol, IntPtr data, EventFlag flags, int dataCount, IntPtr userData) {
			switch (eventType) {
				case EventType.Order:
					var orderBuf = NativeBufferFactory.CreateOrderBuf(symbol, data, flags, dataCount);
					listener.OnOrder<NativeEventBuffer<NativeOrder>, NativeOrder>(orderBuf);
					break;
				case EventType.Profile:
					var profileBuf = NativeBufferFactory.CreateProfileBuf(symbol, data, flags, dataCount);
					listener.OnProfile<NativeEventBuffer<NativeProfile>, NativeProfile>(profileBuf);
					break;
				case EventType.Quote:
					var quoteBuf = NativeBufferFactory.CreateQuoteBuf(symbol, data, flags, dataCount);
					listener.OnQuote<NativeEventBuffer<NativeQuote>, NativeQuote>(quoteBuf);
					break;
				case EventType.TimeAndSale:
					var tsBuf = NativeBufferFactory.CreateTimeAndSaleBuf(symbol, data, flags, dataCount);
					listener.OnTimeAndSale<NativeEventBuffer<NativeTimeAndSale>, NativeTimeAndSale>(tsBuf);
					break;
                case EventType.Trade:
                    var tBuf = NativeBufferFactory.CreateTradeBuf(symbol, data, flags, dataCount);
                    listener.OnTrade<NativeEventBuffer<NativeTrade>, NativeTrade>(tBuf);
                    break;
                case EventType.Summary:
                    var sBuf = NativeBufferFactory.CreateSummaryBuf(symbol, data, flags, dataCount);
                    listener.OnFundamental<NativeEventBuffer<NativeSummary>, NativeSummary>(sBuf);
                    break;
                case EventType.Candle:
                    var cBuf = NativeBufferFactory.CreateCandleBuf(symbol, data, flags, dataCount);
                    candleListener.OnCandle<NativeEventBuffer<NativeCandle>, NativeCandle>(cBuf);
                    break;
			}
		}

		public void AddSymbol(string symbol) {
			C.CheckOk(C.Instance.dxf_add_symbol(subscriptionPtr, symbol));
		}

		#region Implementation of IDisposable

		public void Dispose() {
			if (subscriptionPtr == IntPtr.Zero) return;
			
			C.CheckOk(C.Instance.dxf_close_subscription(subscriptionPtr));
			subscriptionPtr = IntPtr.Zero;
		}

		#endregion

		#region Implementation of IDxSubscription

		public void AddSymbols(params string[] symbols) {
			C.CheckOk(C.Instance.dxf_add_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		public void RemoveSymbols(params string[] symbols) {
			C.CheckOk(C.Instance.dxf_remove_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		public void SetSymbols(params string[] symbols) {
			C.CheckOk(C.Instance.dxf_set_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		public void Clear() {
			C.CheckOk(C.Instance.dxf_clear_symbols(subscriptionPtr));
		}

		public unsafe IList<string> GetSymbols() {
			IntPtr head;
			int len;
			C.CheckOk(C.Instance.dxf_get_symbols(subscriptionPtr, out head, out len));
			
			var result = new string[len];
			for(var i = 0; i < len; i++) {
				var ptr = *(IntPtr*)IntPtr.Add(head, IntPtr.Size*i);
				result[i] = Marshal.PtrToStringUni(ptr);
			}

			return result;
		}

		public void AddSource(params string[] sources) {
			Encoding ascii = Encoding.ASCII;
			for (int i = 0; i < sources.Length; i++) {
				byte[] source = ascii.GetBytes(sources[i]);
				C.CheckOk(C.Instance.dxf_add_order_source(subscriptionPtr, source));
			}
		}

		public void SetSource(params string[] sources) {
			if (sources.Length == 0)
				return;
			Encoding ascii = Encoding.ASCII;
			byte[] source = ascii.GetBytes(sources[0]);
			C.CheckOk(C.Instance.dxf_set_order_source(subscriptionPtr, source));
			for (int i = 1; i < sources.Length; i++) {
				source = ascii.GetBytes(sources[i]);
				C.CheckOk(C.Instance.dxf_add_order_source(subscriptionPtr, source));
			}
		}

		#endregion
	}
}