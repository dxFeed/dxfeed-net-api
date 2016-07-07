using System;
using System.Collections.Generic;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.native.api;
using com.dxfeed.native.events;

namespace com.dxfeed.native {

    /// <summary>
    /// Class provides native snapshot subscription
    /// </summary>
    public class NativeSnapshotSubscription : IDxSubscription {
        private readonly IntPtr connectionPtr;
        private IntPtr snapshotPtr = NativeSnapshotSubscription.InvalidSnapshot;
        private readonly IDxSnapshotListener listener;
        //to prevent callback from being garbage collected
        private C.dxf_snapshot_listener_t callback;
        private Int64 time = 0;
        private string symbol = string.Empty;
        private string source = string.Empty;

        /// <summary>
        /// Invalid snapshot
        /// </summary>
        public static IntPtr InvalidSnapshot = IntPtr.Zero;

        /// <summary>
        /// Creates new native snapshot subscription
        /// </summary>
        /// <param name="connection">native connection pointer</param>
        /// <param name="time">date time in the past</param>
        /// <param name="listener">snapshot events listener</param>
        /// <exception cref="ArgumentNullException">listener is invalid</exception>
        public NativeSnapshotSubscription(NativeConnection connection, Int64 time, IDxSnapshotListener listener) {
            if (listener == null)
                throw new ArgumentNullException("listener");

            this.connectionPtr = connection.Handler;
            this.listener = listener;
            this.time = time;
        }

        private void OnEvent(DxSnapshotData snapshotData, IntPtr userData) {
            switch (snapshotData.event_type) {
                case EventType.Order:
                    var orderBuf = NativeBufferFactory.CreateOrderBuf(snapshotData.symbol, snapshotData.records, snapshotData.records_count, null);
                    listener.OnOrderSnapshot<NativeEventBuffer<NativeOrder>, NativeOrder>(orderBuf);
                    break;
                case EventType.Candle:
                    var candleBuf = NativeBufferFactory.CreateCandleBuf(snapshotData.symbol, snapshotData.records, snapshotData.records_count, null);
                    listener.OnCandleSnapshot<NativeEventBuffer<NativeCandle>, NativeCandle>(candleBuf);
                    break;
            }
        }

        private void CreateOrderSnapshot() {
            if (symbol == string.Empty)
                throw new ArgumentException("Symbol is empty");

            byte[] sourceBytes = null;
            if (source != string.Empty) {
                Encoding ascii = Encoding.ASCII;
                sourceBytes = ascii.GetBytes(source);
            }
            C.CheckOk(C.Instance.dxf_create_order_snapshot(connectionPtr, symbol, sourceBytes, time, out snapshotPtr));
            try {
                C.CheckOk(C.Instance.dxf_attach_snapshot_listener(snapshotPtr, callback = OnEvent, IntPtr.Zero));
            } catch (DxException) {
                Dispose();
                throw;
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Dispose native snapshot subscription
        /// </summary>
        /// <exception cref="NativeDxSubscription"></exception>
        public void Dispose() {
            if (snapshotPtr == InvalidSnapshot) return;

            C.CheckOk(C.Instance.dxf_close_snapshot(snapshotPtr));
            snapshotPtr = InvalidSnapshot;
        }

        #endregion

        #region Implementation of IDxSubscription

        /// <summary>
        /// Add symbol to subscription
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <exception cref="DxException"></exception>
        public void AddSymbol(string symbol) {
            SetSymbols(symbol);
        }

        /// <summary>
        /// Add candle symbol to subscription
        /// </summary>
        /// <param name="symbol">candle symbol</param>
        /// <exception cref="DxException"></exception>
        public void AddSymbol(CandleSymbol symbol) {
            if (snapshotPtr != InvalidSnapshot)
                Dispose();

            IntPtr candleAttributesPtr = IntPtr.Zero;
            try { 
                C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.BaseSymbol,
                symbol.ExchangeCode, symbol.PeriodValue, symbol.PeriodId, symbol.PriceId,
                symbol.SessionId, symbol.AlignmentId, out candleAttributesPtr));
            } catch (DxException) {
                throw;
            }

            try {
                C.CheckOk(C.Instance.dxf_create_candle_snapshot(connectionPtr, candleAttributesPtr, time, out snapshotPtr));
            } finally {
                C.CheckOk(C.Instance.dxf_delete_candle_symbol_attributes(candleAttributesPtr));
            }

            try {
                C.CheckOk(C.Instance.dxf_attach_snapshot_listener(snapshotPtr, callback = OnEvent, IntPtr.Zero));
            } catch (DxException) {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Add multiply symbols to subscription.
        /// It's not applicable to Candle symbols.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="DxException"></exception>
        public void AddSymbols(params string[] symbols) {
            SetSymbols(symbols);
        }

        /// <summary>
        /// Remove multiply symbols from subscription.
        /// It's not applicable to Candle symbols.
        /// On snapshots call Dispose.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="DxException"></exception>
        public void RemoveSymbols(params string[] symbols) {
            Dispose();
        }

        /// <summary>
        /// Set multiply symbols to subscription.
        /// It's not applicable to Candle symbols.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="DxException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void SetSymbols(params string[] symbols) {
            if (symbols.Length != 1)
                throw new ArgumentException("Error: it is allowed to use only one symbol.");

            this.symbol = symbols[0];
            if (this.symbol.Length == 0)
                throw new ArgumentException("Error: invalid parameter");

            if (snapshotPtr != InvalidSnapshot)
                Dispose();

            CreateOrderSnapshot();
        }

        /// <summary>
        /// Clear all symbols from subscription.
        /// It's not applicable to Candle symbols.
        /// On snapshots call Dispose.
        /// </summary>
        /// <exception cref="DxException"></exception>
        public void Clear() {
            Dispose();
        }

        /// <summary>
        /// Get all symbols list from subscription.
        /// </summary>
        /// <returns>list of subscribed symbols</returns>
        /// <exception cref="DxException"></exception>
        public IList<string> GetSymbols() {
            List<string> list =new List<string>();
            list.Add(symbol);
            return list;
        }

        /// <summary>
        /// Add order source to subscription.
        /// </summary>
        /// <param name="sources">list of souces</param>
        /// <exception cref="DxException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddSource(params string[] sources) {
            SetSource(sources);
        }

        /// <summary>
        /// Remove existing sources and set new
        /// </summary>
        /// <param name="sources">list of sources</param>
        /// <exception cref="DxException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void SetSource(params string[] sources) {
            if (sources.Length > 1)
                throw new ArgumentException("Error: it is allowed to use up to one source.");
            this.source = sources[0];
            if (snapshotPtr != InvalidSnapshot)
                Dispose();
            CreateOrderSnapshot();
        }

        #endregion
    }
}
