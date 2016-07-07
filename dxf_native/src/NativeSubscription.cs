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

	/// <summary>
	/// Class provides native event subscription
	/// </summary>
	public class NativeSubscription : IDxSubscription {
		private readonly IntPtr connectionPtr;
		private IntPtr subscriptionPtr;
		private readonly IDxFeedListener eventListener;
		private readonly IDxCandleListener candleListener;
		//to prevent callback from being garbage collected
		private readonly C.dxf_event_listener_v2_t callback;
		private readonly EventType eventType;

		/// <summary>
		/// Create event subscription
		/// </summary>
		/// <param name="connection">native connection pointer</param>
		/// <param name="eventType">type of event to create</param>
		/// <param name="listener">event listener</param>
		public NativeSubscription(NativeConnection connection, EventType eventType, IDxFeedListener listener) {
			if (listener == null)
				throw new ArgumentNullException("listener");

			connectionPtr = connection.Handler;
			this.eventType = eventType;
			this.eventListener = listener;

			C.CheckOk(C.Instance.dxf_create_subscription(connectionPtr, eventType, out subscriptionPtr));
			try {
				C.CheckOk(C.Instance.dxf_attach_event_listener_v2(subscriptionPtr, callback = OnEvent, IntPtr.Zero));
			} catch (DxException) {
				C.Instance.dxf_close_subscription(subscriptionPtr);
				throw;
			}
		}

		/// <summary>
		/// Create Candle event subscription
		/// </summary>
		/// <param name="connection">native connection pointer</param>
		/// <param name="time">date time in the past</param>
		/// <param name="listener">candle event listener</param>
		public NativeSubscription(NativeConnection connection, DateTime? time, IDxCandleListener listener) {
			if (listener == null)
				throw new ArgumentNullException("listener");

			connectionPtr = connection.Handler;
			this.eventType = EventType.Candle;
			this.candleListener = listener;

			if (time == null) {
				C.CheckOk(C.Instance.dxf_create_subscription(connectionPtr, eventType, out subscriptionPtr));
			} else {
				Int64 unixTimestamp = NativeTools.DateToUnixTime((DateTime)time);
				C.CheckOk(C.Instance.dxf_create_subscription_timed(connectionPtr, EventType.Candle, unixTimestamp, out subscriptionPtr));
			}

			try {
				C.CheckOk(C.Instance.dxf_attach_event_listener_v2(subscriptionPtr, callback = OnEvent, IntPtr.Zero));
			} catch (DxException) {
				C.Instance.dxf_close_subscription(subscriptionPtr);
				throw;
			}
		}

		private void OnEvent(EventType eventType, IntPtr symbol, IntPtr data, int dataCount, DxEventParams event_params, IntPtr userData) {
			EventParams nativeEventParams = new EventParams(event_params.flags, event_params.time_int_field, event_params.snapshot_key);
			switch (eventType) {
				case EventType.Order:
					var orderBuf = NativeBufferFactory.CreateOrderBuf(symbol, data, dataCount, nativeEventParams);
					eventListener.OnOrder<NativeEventBuffer<NativeOrder>, NativeOrder>(orderBuf);
					break;
				case EventType.Profile:
					var profileBuf = NativeBufferFactory.CreateProfileBuf(symbol, data, dataCount, nativeEventParams);
					eventListener.OnProfile<NativeEventBuffer<NativeProfile>, NativeProfile>(profileBuf);
					break;
				case EventType.Quote:
					var quoteBuf = NativeBufferFactory.CreateQuoteBuf(symbol, data, dataCount, nativeEventParams);
					eventListener.OnQuote<NativeEventBuffer<NativeQuote>, NativeQuote>(quoteBuf);
					break;
				case EventType.TimeAndSale:
					var tsBuf = NativeBufferFactory.CreateTimeAndSaleBuf(symbol, data, dataCount, nativeEventParams);
					eventListener.OnTimeAndSale<NativeEventBuffer<NativeTimeAndSale>, NativeTimeAndSale>(tsBuf);
					break;
				case EventType.Trade:
					var tBuf = NativeBufferFactory.CreateTradeBuf(symbol, data, dataCount, nativeEventParams);
					eventListener.OnTrade<NativeEventBuffer<NativeTrade>, NativeTrade>(tBuf);
					break;
				case EventType.Summary:
					var sBuf = NativeBufferFactory.CreateSummaryBuf(symbol, data, dataCount, nativeEventParams);
					eventListener.OnFundamental<NativeEventBuffer<NativeSummary>, NativeSummary>(sBuf);
					break;
				case EventType.Candle:
					var cBuf = NativeBufferFactory.CreateCandleBuf(symbol, data, dataCount, nativeEventParams);
					candleListener.OnCandle<NativeEventBuffer<NativeCandle>, NativeCandle>(cBuf);
					break;
			}
		}

		#region Implementation of IDisposable

		public void Dispose() {
			if (subscriptionPtr == IntPtr.Zero) return;
			
			C.CheckOk(C.Instance.dxf_close_subscription(subscriptionPtr));
			subscriptionPtr = IntPtr.Zero;
		}

		#endregion

		#region Implementation of IDxSubscription

		/// <summary>
		/// Add symbol to subscription
		/// </summary>
		/// <param name="symbol">symbol</param>
		/// <exception cref="DxException"></exception>
		public void AddSymbol(string symbol) {
			if (eventType == EventType.Candle)
				return;
			C.CheckOk(C.Instance.dxf_add_symbol(subscriptionPtr, symbol));
		}

		/// <summary>
		/// Add candle symbol to subscription
		/// </summary>
		/// <param name="symbol">candle symbol</param>
		/// <exception cref="DxException"></exception>
		public void AddSymbol(CandleSymbol symbol) {
			IntPtr candleAttributesPtr = IntPtr.Zero;
			C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.BaseSymbol,
				symbol.ExchangeCode, symbol.PeriodValue, symbol.PeriodId, symbol.PriceId,
				symbol.SessionId, symbol.AlignmentId, out candleAttributesPtr));
			C.CheckOk(C.Instance.dxf_add_candle_symbol(subscriptionPtr, candleAttributesPtr));
		}

		/// <summary>
		/// Add multiply symbols to subscription.
		/// It's not applicable to Candle symbols.
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="DxException"></exception>
		public void AddSymbols(params string[] symbols) {
			if (eventType == EventType.Candle)
				return;
			C.CheckOk(C.Instance.dxf_add_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		/// <summary>
		/// Remove multiply symbols from subscription.
		/// It's not applicable to Candle symbols.
		/// On snapshots call Dispose.
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="DxException"></exception>
		public void RemoveSymbols(params string[] symbols) {
			if (eventType == EventType.Candle)
				return;
			C.CheckOk(C.Instance.dxf_remove_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		/// <summary>
		/// Set multiply symbols to subscription.
		/// It's not applicable to Candle symbols.
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="DxException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public void SetSymbols(params string[] symbols) {
			if (eventType == EventType.Candle)
				return;
			C.CheckOk(C.Instance.dxf_set_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		/// <summary>
		/// Clear all symbols from subscription.
		/// It's not applicable to Candle symbols.
		/// On snapshots call Dispose.
		/// </summary>
		/// <exception cref="DxException"></exception>
		public void Clear() {
			if (eventType == EventType.Candle)
				return;
			C.CheckOk(C.Instance.dxf_clear_symbols(subscriptionPtr));
		}

		/// <summary>
		/// Get all symbols list from subscription.
		/// </summary>
		/// <returns>list of subscribed symbols</returns>
		/// <exception cref="DxException"></exception>
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

		/// <summary>
		/// Add order source to subscription.
		/// </summary>
		/// <param name="sources">list of souces</param>
		/// <exception cref="DxException"></exception>
		public void AddSource(params string[] sources) {
			Encoding ascii = Encoding.ASCII;
			for (int i = 0; i < sources.Length; i++) {
				byte[] source = ascii.GetBytes(sources[i]);
				C.CheckOk(C.Instance.dxf_add_order_source(subscriptionPtr, source));
			}
		}

		/// <summary>
		/// Remove existing sources and set new
		/// </summary>
		/// <param name="sources">list of sources</param>
		/// <exception cref="DxException"></exception>
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