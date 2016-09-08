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
		/// Create event subscription.
		/// For candle event use other constructor.
		/// </summary>
		/// <param name="connection">native connection pointer</param>
		/// <param name="eventType">type of event to create</param>
		/// <param name="listener">event listener</param>
		/// <exception cref="ArgumentNullException">If listener is null.</exception>
		/// <exception cref="DxException"></exception>
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
		/// Create Candle event subscription.
		/// For rest events use another constructor.
		/// </summary>
		/// <param name="connection">native connection pointer</param>
		/// <param name="time">date time in the past</param>
		/// <param name="listener">candle event listener</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="DxException"></exception>
		public NativeSubscription(NativeConnection connection, DateTime? time, IDxCandleListener listener) {
			if (listener == null)
				throw new ArgumentNullException("listener");

			connectionPtr = connection.Handler;
			this.eventType = EventType.Candle;
			this.candleListener = listener;

			if (time == null) {
				C.CheckOk(C.Instance.dxf_create_subscription(connectionPtr, eventType, out subscriptionPtr));
			} else {
				long unixTimestamp = Tools.DateToUnixTime((DateTime)time);
				C.CheckOk(C.Instance.dxf_create_subscription_timed(connectionPtr, EventType.Candle, unixTimestamp, out subscriptionPtr));
			}

			try {
				C.CheckOk(C.Instance.dxf_attach_event_listener_v2(subscriptionPtr, callback = OnEvent, IntPtr.Zero));
			} catch (DxException) {
				C.Instance.dxf_close_subscription(subscriptionPtr);
				throw;
			}
		}

		private void OnEvent(EventType eventType, IntPtr symbol, IntPtr data, int dataCount, IntPtr eventParamsPtr, IntPtr userData) {
			object obj = Marshal.PtrToStructure(eventParamsPtr, typeof(DxEventParams));
			DxEventParams event_params = (DxEventParams)obj;
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
		/// It's not applicable to Candle subscription.
		/// </summary>
		/// <param name="symbol">symbol</param>
		/// <exception cref="ArgumentException">Invalid symbol parameter</exception>
		/// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
		/// <exception cref="DxException"></exception>
		public void AddSymbol(string symbol) {
			if (eventType == EventType.Candle)
				return;
			if (symbol == null || symbol.Length == 0)
				throw new ArgumentException("Invalid symbol parameter");
			C.CheckOk(C.Instance.dxf_add_symbol(subscriptionPtr, symbol));
		}

		/// <summary>
		/// Add candle symbol to subscription.
		/// This method applies only to candle subscription. For other events it does not make sense.
		/// </summary>
		/// <param name="symbol">candle symbol</param>
		/// <exception cref="ArgumentException">Invalid symbol parameter</exception>
		/// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
		/// <exception cref="DxException"></exception>
		public void AddSymbol(CandleSymbol symbol) {
			if (eventType != EventType.Candle)
				return;
			if (symbol == null)
				throw new ArgumentException("Invalid symbol parameter");
			IntPtr candleAttributesPtr = IntPtr.Zero;
			C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.BaseSymbol,
				symbol.ExchangeCode, symbol.PeriodValue, symbol.PeriodId, symbol.PriceId,
				symbol.SessionId, symbol.AlignmentId, out candleAttributesPtr));
			try {
				C.CheckOk(C.Instance.dxf_add_candle_symbol(subscriptionPtr, candleAttributesPtr));
			} finally {
				C.CheckOk(C.Instance.dxf_delete_candle_symbol_attributes(candleAttributesPtr));
			}
		}

		/// <summary>
		/// Add multiply symbols to subscription.
		/// It's not applicable to Candle subscription.
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="ArgumentException">Invalid symbol parameter</exception>
		/// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
		/// <exception cref="DxException"></exception>
		public void AddSymbols(params string[] symbols) {
			if (eventType == EventType.Candle)
				return;
			if (symbols == null || symbols.Length == 0)
				throw new ArgumentException("Invalid symbol parameter");
			C.CheckOk(C.Instance.dxf_add_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		/// <summary>
		/// Add multiply candle symbols to subscription.
		/// This method applies only to candle subscription. For other events it does not make sense.
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="ArgumentException">Invalid symbol parameter</exception>
		/// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
		/// <exception cref="DxException"></exception>
		public void AddSymbols(params CandleSymbol[] symbols) {
			if (eventType != EventType.Candle)
				return;
			if (symbols == null || symbols.Length == 0)
				throw new ArgumentException("Invalid symbol parameter");
			foreach (CandleSymbol symbol in symbols) {
				AddSymbol(symbol);
			}
		}

		/// <summary>
		/// Remove multiply symbols from subscription.
		/// It's not applicable to Candle subscription.
		/// 
		/// Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="ArgumentException">Invalid symbol parameter</exception>
		/// <exception cref="DxException"></exception>
		public void RemoveSymbols(params string[] symbols) {
			if (eventType == EventType.Candle)
				return;
			if (symbols == null || symbols.Length == 0)
				throw new ArgumentException("Invalid symbol parameter");
			C.CheckOk(C.Instance.dxf_remove_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		/// <summary>
		/// Remove multiply symbols from subscription.
		/// This method applies only to candle subscription. For other events it does not make sense.
		/// 
		/// Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="ArgumentException">Invalid symbol parameter</exception>
		/// <exception cref="DxException"></exception>
		public void RemoveSymbols(params CandleSymbol[] symbols) {
			if (eventType != EventType.Candle)
				return;
			if (symbols == null || symbols.Length == 0)
				throw new ArgumentException("Invalid symbol parameter");
			foreach (CandleSymbol symbol in symbols) {
				IntPtr candleAttributesPtr = IntPtr.Zero;
				C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.BaseSymbol,
					symbol.ExchangeCode, symbol.PeriodValue, symbol.PeriodId, symbol.PriceId,
					symbol.SessionId, symbol.AlignmentId, out candleAttributesPtr));
				try {
					C.CheckOk(C.Instance.dxf_remove_candle_symbol(subscriptionPtr, candleAttributesPtr));
				} finally {
					C.CheckOk(C.Instance.dxf_delete_candle_symbol_attributes(candleAttributesPtr));
				}
			}
		}

		/// <summary>
		/// Set multiply symbols to subscription.
		/// It's not applicable to Candle subscription.
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="ArgumentException">Invalid symbol parameter</exception>
		/// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
		/// <exception cref="DxException"></exception>
		public void SetSymbols(params string[] symbols) {
			if (eventType == EventType.Candle)
				return;
			if (symbols == null || symbols.Length == 0)
				throw new ArgumentException("Invalid symbol parameter");
			C.CheckOk(C.Instance.dxf_set_symbols(subscriptionPtr, symbols, symbols.Length));
		}

		/// <summary>
		/// Set multiply symbols to subscription.
		/// This method applies only to candle subscription. For other events it does not make sense.
		/// </summary>
		/// <param name="symbols">list of symbols</param>
		/// <exception cref="ArgumentException">Invalid symbol parameter</exception>
		/// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
		/// <exception cref="DxException"></exception>
		public void SetSymbols(params CandleSymbol[] symbols) {
			if (eventType != EventType.Candle)
				return;
			if (symbols == null || symbols.Length == 0)
				throw new ArgumentException("Invalid symbol parameter");
			Clear();
			AddSymbols(symbols);
		}

		/// <summary>
		/// Clear all symbols from subscription.
		/// On snapshots call Dispose.
		/// </summary>
		/// <exception cref="DxException"></exception>
		public void Clear() {
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
		/// <exception cref="ArgumentException">Invalid source parameter</exception>
		/// <exception cref="InvalidOperationException">You try to add more than one source to subscription</exception>
		/// <exception cref="DxException"></exception>
		public void AddSource(params string[] sources) {
			if (eventType == EventType.Candle || sources == null || sources.Length == 0)
				return;
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
		/// <exception cref="ArgumentException">Invalid source parameter</exception>
		/// <exception cref="InvalidOperationException">You try to add more than one source to subscription</exception>
		/// <exception cref="DxException"></exception>
		public void SetSource(params string[] sources) {
			if (eventType == EventType.Candle || sources == null || sources.Length == 0)
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