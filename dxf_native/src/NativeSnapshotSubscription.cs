using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.native.api;
using com.dxfeed.native.events;

namespace com.dxfeed.native
{
    /// <summary>
    /// Class provides native snapshot subscription
    /// </summary>
    public class NativeSnapshotSubscription : IDxSubscription
    {
        private readonly IntPtr connectionPtr;
        private IntPtr snapshotPtr = NativeSnapshotSubscription.InvalidSnapshot;
        private readonly IDxSnapshotListener listener;
        //to prevent callback from being garbage collected
        private C.dxf_snapshot_listener_t callback;
        private Int64 time = 0;
        private string source = string.Empty;
        private EventType eventType = EventType.None;

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
        public NativeSnapshotSubscription(NativeConnection connection, Int64 time, IDxSnapshotListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            this.connectionPtr = connection.Handler;
            this.listener = listener;
            this.time = time;
        }

        private void OnEvent(IntPtr snapshotDataPtr, IntPtr userData)
        {
            DxSnapshotData snapshotData = (DxSnapshotData)Marshal.PtrToStructure(snapshotDataPtr, typeof(DxSnapshotData));
            switch (snapshotData.event_type)
            {
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

        /// <summary>
        /// Get symbol of snapshot
        /// </summary>
        public string Symbol
        {
            get
            {
                IList<string> symbols = GetSymbols();
                return symbols.Count > 0 ? symbols[0] : string.Empty;
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Dispose native snapshot subscription
        /// </summary>
        /// <exception cref="NativeDxSubscription"></exception>
        public void Dispose()
        {
            if (snapshotPtr == InvalidSnapshot) return;

            C.CheckOk(C.Instance.dxf_close_snapshot(snapshotPtr));
            snapshotPtr = InvalidSnapshot;

            eventType = EventType.None;
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
        public void AddSymbol(string symbol)
        {
            if (snapshotPtr != InvalidSnapshot)
                throw new InvalidOperationException("It is allowed to add only one symbol to snapshot subscription");
            if (symbol == null || symbol.Length == 0)
                throw new ArgumentException("Invalid symbol parameter");


            byte[] sourceBytes = null;
            if (source != string.Empty)
            {
                Encoding ascii = Encoding.ASCII;
                sourceBytes = ascii.GetBytes(source);
            }
            C.CheckOk(C.Instance.dxf_create_order_snapshot(connectionPtr, symbol, sourceBytes, time, out snapshotPtr));
            try
            {
                C.CheckOk(C.Instance.dxf_attach_snapshot_listener(snapshotPtr, callback = OnEvent, IntPtr.Zero));
            }
            catch (DxException)
            {
                Dispose();
                throw;
            }

            eventType = EventType.Order;
        }

        /// <summary>
        /// Add candle symbol to subscription.
        /// This method applies only to candle subscription. For other events it does not make sense.
        /// </summary>
        /// <param name="symbol">candle symbol</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        public void AddSymbol(CandleSymbol symbol)
        {
            if (snapshotPtr != InvalidSnapshot)
                throw new InvalidOperationException("It is allowed to add only one symbol to snapshot subscription");
            if (symbol == null)
                throw new ArgumentException("Invalid symbol parameter");

            IntPtr candleAttributesPtr = IntPtr.Zero;
            try
            {
                C.CheckOk(C.Instance.dxf_create_candle_symbol_attributes(symbol.BaseSymbol,
                symbol.ExchangeCode, symbol.PeriodValue, symbol.PeriodId, symbol.PriceId,
                symbol.SessionId, symbol.AlignmentId, out candleAttributesPtr));
            }
            catch (DxException)
            {
                throw;
            }

            try
            {
                C.CheckOk(C.Instance.dxf_create_candle_snapshot(connectionPtr, candleAttributesPtr, time, out snapshotPtr));
            }
            finally
            {
                C.CheckOk(C.Instance.dxf_delete_candle_symbol_attributes(candleAttributesPtr));
            }

            try
            {
                C.CheckOk(C.Instance.dxf_attach_snapshot_listener(snapshotPtr, callback = OnEvent, IntPtr.Zero));
            }
            catch (DxException)
            {
                Dispose();
                throw;
            }

            eventType = EventType.Candle;
        }

        /// <summary>
        /// Add multiply symbols to subscription.
        /// It's not applicable to Candle subscription.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        public void AddSymbols(params string[] symbols)
        {
            if (symbols == null)
                throw new ArgumentException("Invalid symbol parameter");
            if (symbols.Length != 1)
                throw new InvalidOperationException("It is allowed to add only one symbol to snapshot subscription");
            AddSymbol(symbols[0]);
        }

        /// <summary>
        /// Add multiply candle symbols to subscription.
        /// This method applies only to candle subscription. For other events it does not make sense.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        public void AddSymbols(params CandleSymbol[] symbols)
        {
            if (symbols == null)
                throw new ArgumentException("Invalid symbol parameter");
            if (symbols.Length != 1)
                throw new InvalidOperationException("It is allowed to add only one symbol to snapshot subscription");
            AddSymbol(symbols[0]);
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
        public void RemoveSymbols(params string[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("Invalid symbol parameter");
            List<string> symbolList = new List<string>(symbols);
            if (symbolList.Contains(Symbol))
                Dispose();
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
        public void RemoveSymbols(params CandleSymbol[] symbols)
        {
            if (symbols == null)
                throw new ArgumentException("Invalid symbol parameter");
            foreach (CandleSymbol symbol in symbols)
            {
                if (symbol == null)
                    continue;
                if (this.Symbol.Equals(symbol.ToString()))
                    Dispose();
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
        public void SetSymbols(params string[] symbols)
        {
            if (symbols == null)
                throw new ArgumentException("Invalid symbol parameter");
            if (symbols.Length != 1)
                throw new InvalidOperationException("It is allowed to add only one symbol to snapshot subscription");

            if (snapshotPtr != InvalidSnapshot)
                Clear();

            AddSymbol(symbols[0]);
        }

        /// <summary>
        /// Set multiply symbols to subscription.
        /// This method applies only to candle subscription. For other events it does not make sense.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        public void SetSymbols(params CandleSymbol[] symbols)
        {
            if (symbols == null)
                throw new ArgumentException("Invalid symbol parameter");
            if (symbols.Length != 1)
                throw new InvalidOperationException("It is allowed to add only one symbol to snapshot subscription");

            if (snapshotPtr != InvalidSnapshot)
                Clear();

            AddSymbol(symbols[0]);
        }

        /// <summary>
        /// Clear all symbols from subscription.
        /// On snapshots call Dispose.
        /// </summary>
        /// <exception cref="DxException"></exception>
        public void Clear()
        {
            Dispose();
        }

        /// <summary>
        /// Get all symbols list from subscription.
        /// </summary>
        /// <returns>list of subscribed symbols</returns>
        /// <exception cref="DxException"></exception>
        public IList<string> GetSymbols()
        {
            IntPtr symbolPtr;
            C.Instance.dxf_get_snapshot_symbol(snapshotPtr, out symbolPtr);
            List<string> symbols = new List<string>();
            if (symbolPtr != IntPtr.Zero)
                symbols.Add(Marshal.PtrToStringUni(symbolPtr));
            return symbols;
        }

        /// <summary>
        /// Add order source to subscription.
        /// </summary>
        /// <param name="sources">list of souces</param>
        /// <exception cref="ArgumentException">Invalid source parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription</exception>
        /// <exception cref="DxException"></exception>
        public void AddSource(params string[] sources)
        {
            if (eventType != EventType.Order && eventType != EventType.None)
                return;
            if (!source.Equals(string.Empty))
                throw new InvalidOperationException("It is allowed to use up to one source.");

            SetSource(sources);
        }

        /// <summary>
        /// Remove existing sources and set new
        /// </summary>
        /// <param name="sources">list of sources</param>
        /// <exception cref="ArgumentException">Invalid source parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription</exception>
        /// <exception cref="DxException"></exception>
        public void SetSource(params string[] sources)
        {
            if (eventType != EventType.Order && eventType != EventType.None)
                return;
            if (sources == null)
                throw new ArgumentException("Invalid source parameter");
            if (sources.Length != 1)
                throw new InvalidOperationException("It is allowed to use up to one source.");
            string newSource = sources[0];
            if (newSource.Length == 0)
                throw new ArgumentException("Invalid source parameter");

            this.source = newSource;

            if (snapshotPtr != InvalidSnapshot)
            {
                string symbol = this.Symbol;
                Dispose();
                AddSymbol(symbol);
            }

        }

        #endregion
    }
}
