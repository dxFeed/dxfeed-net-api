using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;

namespace com.dxfeed.native {
    public class OrderViewSubscription : IDxSubscription, IDxFeedListener {

        private IDxOrderViewListener listener = null;
        private IDxConnection connection = null;
        private IDxSubscription subscription = null;

        /// <summary>
        /// All snapshots, acsess by unique snapshot key (ulong)
        /// </summary>
        private IDictionary<ulong, EventBuffer<IDxOrder>> snapshots = new Dictionary<ulong, EventBuffer<IDxOrder>>();
        /// <summary>
        /// Map between Symbol + Source string and unique snapashot key in snapshots dictionary
        /// </summary>
        private IDictionary<string, IList<ulong>> symbolSourceToKey = new Dictionary<string, IList<ulong>>();

        private ISet<string> sources = new SortedSet<string>();
        private ISet<string> symbols = new SortedSet<string>();

        /// <summary>
        /// All received snapshots, empty set means no received snapshots yet.
        /// Key - menas symbol, value - set of sources of received snapshots
        /// </summary>
        private IDictionary<string, ISet<string>> receivedSnapshots = new Dictionary<string, ISet<string>>();

        private bool snapshotsSent = false;

        public OrderViewSubscription(IDxConnection connection, IDxOrderViewListener listener) {
            this.connection = connection;
            this.listener = listener;
        }

        #region Implementation of IDxSubscription

        public void AddSymbol(string symbol) {
            if (subscription == null) {
                return;
            }
            subscription.AddSymbol(symbol);
            symbols.Add(symbol);
            receivedSnapshots.Add(symbol.ToUpper(), new SortedSet<string>());
        }

        public void AddSymbol(CandleSymbol symbol) {
            return;
        }

        public void AddSymbols(params string[] symbols) {
            if (subscription == null) {
                return;
            }
            subscription.AddSymbols(symbols);
            foreach (var symbol in symbols) {
                this.symbols.Add(symbol);
                receivedSnapshots.Add(symbol.ToUpper(), new SortedSet<string>());
            }
        }

        public void AddSymbols(params CandleSymbol[] symbols) {
            return;
        }

        public void RemoveSymbols(params string[] symbols) {
            if (subscription == null) {
                return;
            }
            subscription.RemoveSymbols(symbols);
            foreach (var symbol in symbols) {
                receivedSnapshots.Remove(symbol.ToUpper());
                foreach (var source in sources) {
                    var symbolSource = symbol.ToUpper() + source;
                    if (symbolSourceToKey.ContainsKey(symbolSource)) {
                        var keysList = symbolSourceToKey[symbol + source];
                        foreach (var key in keysList) {
                            snapshots.Remove(key);
                        }
                        symbolSourceToKey.Remove(symbolSource);
                    }
                }
                this.symbols.Remove(symbol.ToUpper());
            }
        }

        public void RemoveSymbols(params CandleSymbol[] symbols) {
            return;
        }

        public void SetSymbols(params string[] symbols) {
            if (subscription == null) {
                return;
            }
            subscription.SetSymbols(symbols);
            this.symbols.Clear();
            snapshots.Clear();
            symbolSourceToKey.Clear();
            receivedSnapshots.Clear();
            foreach (var symbol in symbols) {
                this.symbols.Add(symbol.ToUpper());
                receivedSnapshots.Add(symbol.ToUpper(), new SortedSet<string>());
            }
        }

        public void SetSymbols(params CandleSymbol[] symbols) {
            return;
        }

        public void Clear() {
            subscription.Clear();
            symbols.Clear();
            snapshots.Clear();
            symbolSourceToKey.Clear();
            receivedSnapshots.Clear();
        }

        public IList<string> GetSymbols() {
            if (subscription == null) {
                return null;
            }
            return subscription.GetSymbols();
        }

        public void AddSource(params string[] sources) {
            SetSource(sources);
        }

        public void SetSource(params string[] sources) {
            if (subscription != null) {
                throw new NativeDxException("Sources already set");
            }
            subscription = connection.CreateSubscription(EventType.Order, this);
            subscription.SetSource(sources);
            foreach(var source in sources) {
                this.sources.Add(source.ToUpper());
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (subscription != null) {
                        subscription.Dispose();
                        subscription = null;
                    }
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #region Implementation of IDxFeedListener

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote { }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade { }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            if (snapshots.ContainsKey(buf.EventParams.SnapshotKey)) {
                if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotBegin)) {
                    snapshots[buf.EventParams.SnapshotKey].Clear();
                }
                foreach (var order in buf) {
                    snapshots[buf.EventParams.SnapshotKey].AddEvent(order);
                }
                if (buf.EventParams.Flags == 0 ) {
                    return;
                }
                if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotEnd) || buf.EventParams.Flags.HasFlag(EventFlag.SnapshotSnip)) {
                    SnapshotEndFlagReceived<TB, TE>(buf);
                    return;
                }

                if (buf.EventParams.Flags.HasFlag(EventFlag.TxPending) && !buf.EventParams.Flags.HasFlag(EventFlag.RemoveEvent)) {
                    foreach (var order in buf) {
                        snapshots[buf.EventParams.SnapshotKey].ReplaceOrAdd(order);
                    }
                    if (snapshotsSent && snapshots[buf.EventParams.SnapshotKey].EventParams.Flags.HasFlag(EventFlag.SnapshotEnd)) {
                        listener.OnSnapshot<IDxEventBuf<IDxOrder>, IDxOrder>(snapshots[buf.EventParams.SnapshotKey]);
                    }
                }

                if (buf.EventParams.Flags.HasFlag(EventFlag.RemoveEvent)) {
                    foreach (var order in buf) {
                        snapshots[buf.EventParams.SnapshotKey].Remove(order);
                    }
                }
            }
            else {
                if (buf.EventParams.Flags == 0) {
                    if (snapshotsSent) {
                        listener.OnUpdate<TB, TE>(buf);
                    }
                    return;
                }
                if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotBegin)) {
                    EventBuffer<IDxOrder> buffer = new EventBuffer<IDxOrder>(buf.EventType, buf.Symbol, buf.EventParams);
                    foreach (var order in buf) {
                        buffer.AddEvent(order);
                    }
                    snapshots.Add(buf.EventParams.SnapshotKey, buffer);
                    var enmrtr = buf.GetEnumerator();
                    enmrtr.MoveNext();
                    string symbolSource = buf.Symbol.ToString().ToUpper() + enmrtr.Current.Source.ToUpper();
                    if (symbolSourceToKey.ContainsKey(symbolSource)) {
                        var snapshotKeys = symbolSourceToKey[symbolSource];
                        snapshotKeys.Add(buf.EventParams.SnapshotKey);
                    } else {
                        var snapshotKeys = new List<ulong>();
                        snapshotKeys.Add(buf.EventParams.SnapshotKey);
                        symbolSourceToKey.Add(symbolSource, snapshotKeys);
                    }

                    if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotEnd)) {
                        SnapshotEndFlagReceived<TB, TE>(buf);
                    }
                }
            }
        }

        private void SnapshotEndFlagReceived<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            // to set right flags befor sending
            snapshots[buf.EventParams.SnapshotKey].EventParams = buf.EventParams;

            if (snapshotsSent) {
                listener.OnSnapshot<IDxEventBuf<IDxOrder>, IDxOrder>(snapshots[buf.EventParams.SnapshotKey]);
            } else {
                var enmrtr = buf.GetEnumerator();
                enmrtr.MoveNext();
                var source = enmrtr.Current.Source.ToUpper();
                var symbol = buf.Symbol.ToString().ToUpper();
                receivedSnapshots[symbol].Add(source);

                if (receivedSnapshots[symbol].SetEquals(sources)) {
                    ISet<ulong> sentSnapshots = new SortedSet<ulong>();
                    EventBuffer<IDxOrder> buffer = new EventBuffer<IDxOrder>(buf.EventType, buf.Symbol, buf.EventParams);
                    foreach (var source_ in receivedSnapshots[symbol]) {
                        var symbolSource = symbol + source_;
                        foreach (var snapshotKey in symbolSourceToKey[symbolSource]) {
                            foreach (var order in snapshots[snapshotKey]) {
                                buffer.AddEvent(order);
                            }
                            sentSnapshots.Add(snapshotKey);
                        }
                    }
                    listener.OnSnapshot<IDxEventBuf<IDxOrder>, IDxOrder>(buffer);
                    foreach(var keyValuePair in snapshots) {
                        if (sentSnapshots.Contains(keyValuePair.Key)) {
                            continue;
                        }
                        if (keyValuePair.Value.EventParams.Flags.HasFlag(EventFlag.SnapshotEnd)) {
                            listener.OnSnapshot<IDxEventBuf<IDxOrder>, IDxOrder>(keyValuePair.Value);
                        }
                    }
                    snapshotsSent = true;
                }
            }
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile { }

        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary { }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale { }
        #endregion

    }
}
