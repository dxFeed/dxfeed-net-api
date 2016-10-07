/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using System;

namespace com.dxfeed.native
{
    public class OrderViewSubscription : IDxSubscription, IDxFeedListener
    {
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
        /// Key - means symbol, value - set of received snapshots sources.
        /// </summary>
        private IDictionary<string, ISet<string>> receivedSnapshots = new Dictionary<string, ISet<string>>();

        /// <summary>
        /// State of the order view subscription
        ///     Update - when snapshot begins for one of the source in order view
        ///     Ready  - when snapshot ends for all sources in order view
        /// </summary>
        private enum OrderViewState { Update, Ready };
        /// <summary>
        /// The dictionary os states for each order view in the format:
        ///     map[symbol-of-order-view] = state
        /// </summary>
        private IDictionary<string, OrderViewState> orderViewStates = new Dictionary<string, OrderViewState>();

        private const string AddSymbolBeforeSourceErrorText = @"You must configure subscription 
            with AddSource or SetSource before calling this method.";
        private const string AddCandleSymbolErrorText = "Candle symbols is not allowed for OrderViewSubscription.";

        public OrderViewSubscription(IDxConnection connection, IDxOrderViewListener listener)
        {
            this.connection = connection;
            this.listener = listener;
        }

        #region Implementation of IDxSubscription

        public void AddSymbol(string symbol)
        {
            if (subscription == null)
            {
                throw new InvalidOperationException(AddSymbolBeforeSourceErrorText);
            }
            subscription.AddSymbol(symbol);
            symbols.Add(symbol);
            receivedSnapshots.Add(symbol.ToUpper(), new SortedSet<string>());
        }

        public void AddSymbol(CandleSymbol symbol)
        {
            throw new InvalidOperationException(AddCandleSymbolErrorText);
        }

        public void AddSymbols(params string[] symbols)
        {
            if (subscription == null)
            {
                throw new InvalidOperationException(AddSymbolBeforeSourceErrorText);
            }
            subscription.AddSymbols(symbols);
            foreach (var symbol in symbols)
            {
                this.symbols.Add(symbol);
                receivedSnapshots.Add(symbol.ToUpper(), new SortedSet<string>());
            }
        }

        public void AddSymbols(params CandleSymbol[] symbols)
        {
            throw new InvalidOperationException(AddCandleSymbolErrorText);
        }

        public void RemoveSymbols(params string[] symbols)
        {
            if (subscription == null)
            {
                return;
            }
            subscription.RemoveSymbols(symbols);
            foreach (var symbol in symbols)
            {
                string upperSymbol = symbol.ToUpper();
                receivedSnapshots.Remove(upperSymbol);
                foreach (var source in sources)
                {
                    var symbolSource = upperSymbol + source;
                    if (symbolSourceToKey.ContainsKey(symbolSource))
                    {
                        var keysList = symbolSourceToKey[symbol + source];
                        foreach (var key in keysList)
                        {
                            snapshots.Remove(key);
                        }
                        symbolSourceToKey.Remove(symbolSource);
                    }
                }
                this.symbols.Remove(upperSymbol);
                orderViewStates.Remove(upperSymbol);
            }
        }

        public void RemoveSymbols(params CandleSymbol[] symbols)
        {
            throw new InvalidOperationException(AddCandleSymbolErrorText);
        }

        public void SetSymbols(params string[] symbols)
        {
            if (subscription == null)
            {
                throw new InvalidOperationException(AddSymbolBeforeSourceErrorText);
            }
            subscription.SetSymbols(symbols);
            this.symbols.Clear();
            snapshots.Clear();
            symbolSourceToKey.Clear();
            receivedSnapshots.Clear();
            foreach (var symbol in symbols)
            {
                string upperSymbol = symbol.ToUpper();
                this.symbols.Add(upperSymbol);
                receivedSnapshots.Add(upperSymbol, new SortedSet<string>());
            }
            orderViewStates.Clear();
        }

        public void SetSymbols(params CandleSymbol[] symbols)
        {
            throw new InvalidOperationException(AddCandleSymbolErrorText);
        }

        public void Clear()
        {
            subscription.Clear();
            symbols.Clear();
            snapshots.Clear();
            symbolSourceToKey.Clear();
            receivedSnapshots.Clear();
            orderViewStates.Clear();
        }

        public IList<string> GetSymbols()
        {
            if (subscription == null)
            {
                return null;
            }
            return subscription.GetSymbols();
        }

        public void AddSource(params string[] sources)
        {
            SetSource(sources);
        }

        public void SetSource(params string[] sources)
        {
            if (subscription != null)
            {
                throw new InvalidOperationException("Sources is already configured for this subscription.");
            }
            subscription = connection.CreateSubscription(EventType.Order, this);
            subscription.SetSource(sources);
            foreach (var source in sources)
            {
                this.sources.Add(source.ToUpper());
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (subscription != null)
                    {
                        subscription.Dispose();
                        subscription = null;
                    }
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #region Implementation of IDxFeedListener

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        { }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        { }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            string symbol = buf.Symbol.ToString().ToUpper();

            if (snapshots.ContainsKey(buf.EventParams.SnapshotKey))
            {
                if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotBegin))
                {
                    snapshots[buf.EventParams.SnapshotKey].Clear();
                    foreach (var order in buf)
                    {
                        receivedSnapshots[symbol].Remove(order.Source);
                    }
                    orderViewStates[symbol] = OrderViewState.Update;
                }

                // if order view is ready just call OnUpdate
                if (orderViewStates.ContainsKey(symbol) && orderViewStates[symbol] == OrderViewState.Ready)
                {
                    listener.OnUpdate<TB, TE>(buf);
                    return;
                }

                //...otherwise continue fill current snapshot
                foreach (var order in buf)
                {
                    snapshots[buf.EventParams.SnapshotKey].AddEvent(order);
                }
                if (buf.EventParams.Flags == 0)
                {
                    return;
                }
                if (buf.EventParams.Flags.HasFlag(EventFlag.RemoveEvent))
                {
                    foreach (var order in buf)
                    {
                        snapshots[buf.EventParams.SnapshotKey].Remove(order);
                    }
                }
                if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotEnd) || buf.EventParams.Flags.HasFlag(EventFlag.SnapshotSnip))
                {
                    SnapshotEndFlagReceived<TB, TE>(buf);
                    return;
                }
            }
            else
            {
                if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotBegin))
                {
                    orderViewStates[symbol] = OrderViewState.Update;
                    EventBuffer<IDxOrder> outputBuffer = new EventBuffer<IDxOrder>(buf.EventType, buf.Symbol, buf.EventParams);
                    foreach (var order in buf)
                    {
                        outputBuffer.AddEvent(order);
                    }
                    snapshots.Add(buf.EventParams.SnapshotKey, outputBuffer);
                    var enmrtr = buf.GetEnumerator();
                    enmrtr.MoveNext();
                    string symbolSource = symbol + enmrtr.Current.Source.ToUpper();
                    if (symbolSourceToKey.ContainsKey(symbolSource))
                    {
                        var snapshotKeys = symbolSourceToKey[symbolSource];
                        snapshotKeys.Add(buf.EventParams.SnapshotKey);
                    }
                    else
                    {
                        var snapshotKeys = new List<ulong>();
                        snapshotKeys.Add(buf.EventParams.SnapshotKey);
                        symbolSourceToKey.Add(symbolSource, snapshotKeys);
                    }

                    if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotEnd))
                    {
                        SnapshotEndFlagReceived<TB, TE>(buf);
                    }
                }
            }
        }

        private void SnapshotEndFlagReceived<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            // to set right flags befor sending
            snapshots[buf.EventParams.SnapshotKey].EventParams = buf.EventParams;

            var enmrtr = buf.GetEnumerator();
            enmrtr.MoveNext();
            var source = enmrtr.Current.Source.ToUpper();
            var symbol = buf.Symbol.ToString().ToUpper();
            receivedSnapshots[symbol].Add(source);

            if (receivedSnapshots[symbol].SetEquals(sources))
            {
                ISet<ulong> sentSnapshots = new SortedSet<ulong>();
                EventBuffer<IDxOrder> buffer = new EventBuffer<IDxOrder>(buf.EventType, buf.Symbol, buf.EventParams);
                foreach (var receivedSource in receivedSnapshots[symbol])
                {
                    var symbolSource = symbol + receivedSource;
                    foreach (var snapshotKey in symbolSourceToKey[symbolSource])
                    {
                        foreach (var order in snapshots[snapshotKey])
                        {
                            buffer.AddEvent(order);
                        }
                        sentSnapshots.Add(snapshotKey);
                    }
                }
                listener.OnSnapshot<IDxEventBuf<IDxOrder>, IDxOrder>(buffer);
                foreach (var keyValuePair in snapshots)
                {
                    if (sentSnapshots.Contains(keyValuePair.Key))
                    {
                        continue;
                    }
                    if (keyValuePair.Value.EventParams.Flags.HasFlag(EventFlag.SnapshotEnd))
                    {
                        listener.OnSnapshot<IDxEventBuf<IDxOrder>, IDxOrder>(keyValuePair.Value);
                    }
                }
                orderViewStates[symbol] = OrderViewState.Ready;
            }
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        { }

        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        { }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        { }
        #endregion
    }
}
