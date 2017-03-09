﻿#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System;
using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.events;

namespace com.dxfeed.native
{
    /// <summary>
    ///   <para>
    ///     Client will be notified about first combined snapshot from different
    ///     sources, and, after that, about separate orders
    ///   </para>
    /// </summary>
    public class OrderViewSubscription : IDxSubscription, IDxOrderListener
    {
        private IDxOrderViewListener listener = null;
        private IDxConnection connection = null;
        private IDxSubscription subscription = null;

        /// <summary>
        ///   All snapshots, acsess by unique snapshot key (<see cref="ulong"/>)
        /// </summary>
        private IDictionary<ulong, EventBuffer<IDxOrder>> snapshots = new Dictionary<ulong, EventBuffer<IDxOrder>>();
        private enum SnapshotState { Unbroken, Broken };
        private IDictionary<ulong, SnapshotState> snapshotsStates = new Dictionary<ulong, SnapshotState>();

        /// <summary>
        ///   Map between Symbol + Source string and unique snapashot key in <see cref="snapshots"/> dictionary
        /// </summary>
        private IDictionary<string, IList<ulong>> symbolSourceToKey = new Dictionary<string, IList<ulong>>();

        private ISet<string> sources = new SortedSet<string>();
        private ISet<string> symbols = new SortedSet<string>();

        /// <summary>
        ///   All received snapshots, empty set means no received snapshots yet.
        ///   Key - means symbol, value - set of received snapshots sources.
        /// </summary>
        private IDictionary<string, ISet<string>> receivedSnapshots = new Dictionary<string, ISet<string>>();

        /// <summary>
        ///   States of the order view subscription:
        ///   <para>
        ///     <c>Update</c>  - when snapshot begins for one of the source in order view
        ///   </para>
        ///   <para>
        ///     <c>Ready</c>   - when snapshot ends for all sources in order view
        ///   </para>
        ///   <para>
        ///     <c>Pending</c> - when updating and received <see cref="EventFlag.TxPending"/> - begin collecting events
        ///   </para>
        /// </summary>
        [Flags]
        private enum OrderViewState : int { Update = 0x01, Ready = 0x02, Pending = 0x04 };

        /// <summary>
        ///   The dictionary os states for each order view in the format:
        ///   <code>
        ///     map[symbol-of-order-view] = state
        ///   </code>
        /// </summary>
        private IDictionary<string, OrderViewState> orderViewStates = new Dictionary<string, OrderViewState>();

        private const string AddSymbolBeforeSourceErrorText = @"You must configure subscription 
            with AddSource or SetSource before calling this method.";
        private const string AddCandleSymbolErrorText = "Candle symbols is not allowed for OrderViewSubscription.";

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="listener"></param>
        public OrderViewSubscription(IDxConnection connection, IDxOrderViewListener listener)
        {
            this.connection = connection;
            this.listener = listener;
        }

        #region Implementation of IDxSubscription

        /// <summary>
        ///   Add a symbol to subscription
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">
        ///   You try to add symbol before adding source via <see cref="AddSource(string[])"/> or
        ///   <see cref="SetSource(string[])"/> methods i.e. subscription is <c>null</c>
        /// </exception>
        /// <exception cref="DxException"></exception>
        public void AddSymbol(string symbol)
        {
            if (subscription == null)
            {
                throw new InvalidOperationException(AddSymbolBeforeSourceErrorText);
            }
            if (symbols.Contains(symbol.ToUpper()))
                return;
            subscription.AddSymbol(symbol);
            symbols.Add(symbol);
            receivedSnapshots.Add(symbol.ToUpper(), new SortedSet<string>());
        }

        /// <summary>
        ///   Inherited from <see cref="IDxSubscription"/>
        ///   This method is not allowed. Throws exception.
        /// </summary>
        /// <param name="symbol">candle symbol</param>
        /// <exception cref="InvalidOperationException">
        ///   Candle symbols is not allowed for <see cref="OrderViewSubscription"/>
        /// </exception>
        public void AddSymbol(CandleSymbol symbol)
        {
            throw new InvalidOperationException(AddCandleSymbolErrorText);
        }

        /// <summary>
        ///   Add multiply symbols to subscription.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">
        ///   You try to add symbols before adding source via <see cref="AddSource(string[])"/> or
        ///   <see cref="SetSource(string[])"/> methods i.e. subscription is <c>null</c>
        /// </exception>
        /// <exception cref="DxException"></exception>
        public void AddSymbols(params string[] symbols)
        {
            if (subscription == null)
            {
                throw new InvalidOperationException(AddSymbolBeforeSourceErrorText);
            }
            subscription.AddSymbols(symbols);
            foreach (var symbol in symbols)
            {
                if (this.symbols.Contains(symbol.ToUpper()))
                    continue;
                this.symbols.Add(symbol);
                receivedSnapshots.Add(symbol.ToUpper(), new SortedSet<string>());
            }
        }

        /// <summary>
        ///   Inherited from <see cref="IDxSubscription"/>
        ///   This method is not allowed. Throws exception.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="InvalidOperationException">
        ///   Candle symbols is not allowed for <see cref="OrderViewSubscription"/>
        /// </exception>
        public void AddSymbols(params CandleSymbol[] symbols)
        {
            throw new InvalidOperationException(AddCandleSymbolErrorText);
        }

        private void RemoveSymbolSourcePair(string symbolSource)
        {
            if (symbolSourceToKey.ContainsKey(symbolSource))
            {
                var keysList = symbolSourceToKey[symbolSource];
                foreach (var key in keysList)
                {
                    snapshots.Remove(key);
                }
                symbolSourceToKey.Remove(symbolSource);
            }
        }

        /// <summary>
        ///   Remove multiply symbols from subscription.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="DxException"></exception>
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
                    RemoveSymbolSourcePair(upperSymbol + source);
                }
                // removing snapshots with empty source string i.e. ""
                RemoveSymbolSourcePair(upperSymbol);
                this.symbols.Remove(upperSymbol);
                orderViewStates.Remove(upperSymbol);
            }
        }

        /// <summary>
        ///   Inherited from <see cref="IDxSubscription"/>
        ///   This method is not allowed. Throws exception.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="InvalidOperationException">
        ///   Candle symbols is not allowed for <see cref="OrderViewSubscription"/>
        /// </exception>
        public void RemoveSymbols(params CandleSymbol[] symbols)
        {
            throw new InvalidOperationException(AddCandleSymbolErrorText);
        }

        /// <summary>
        ///   Set multiply symbols to subscription.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">
        ///   You try to set symbols before adding source via <see cref="AddSource(string[])"/> or
        ///   <see cref="SetSource(string[])"/> methods i.e. subscription is <c>null</c>
        /// </exception>
        /// <exception cref="DxException"></exception>
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

        /// <summary>
        ///   Inherited from <see cref="IDxSubscription"/>
        ///   This method is not allowed. Throws exception.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="InvalidOperationException">
        ///   Candle symbols is not allowed for <see cref="OrderViewSubscription"/>
        /// </exception>
        public void SetSymbols(params CandleSymbol[] symbols)
        {
            throw new InvalidOperationException(AddCandleSymbolErrorText);
        }

        /// <summary>
        ///   Clear all symbols from subscription.
        /// </summary>
        /// <exception cref="DxException"></exception>
        public void Clear()
        {
            subscription.Clear();
            symbols.Clear();
            snapshots.Clear();
            symbolSourceToKey.Clear();
            receivedSnapshots.Clear();
            orderViewStates.Clear();
        }

        /// <summary>
        ///   Get all symbols list from subscription.
        /// </summary>
        /// <returns>list of subscribed symbols</returns>
        /// <exception cref="DxException"></exception>
        public IList<string> GetSymbols()
        {
            if (subscription == null)
            {
                return null;
            }
            return subscription.GetSymbols();
        }

        /// <summary>
        ///   Sets order source to subscription.
        /// </summary>
        /// <remarks>
        ///   This method calls <see cref="SetSource(string[])"/> and can be called only one times
        /// </remarks>
        /// <param name="sources">list of souces</param>
        /// <exception cref="ArgumentException">Invalid source parameter</exception>
        /// <exception cref="InvalidOperationException">
        ///   Sources can be configured for this subscription only once
        /// </exception>
        /// <exception cref="DxException"></exception>
        public void AddSource(params string[] sources)
        {
            SetSource(sources);
        }

        /// <summary>
        ///   Sets order source to subscription.
        /// </summary>
        /// <remarks>
        ///   This method can be called only one times
        /// </remarks>
        /// <param name="sources">list of souces</param>
        /// <exception cref="ArgumentException">Invalid source parameter</exception>
        /// <exception cref="InvalidOperationException">
        ///   Sources can be configured for this subscription only once
        /// </exception>
        /// <exception cref="DxException"></exception>
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

        /// <summary>
        ///   This code added to correctly implement the disposable pattern.
        /// </summary>
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

        /// <summary>
        ///   This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #region Implementation of IDxOrderListener

        /// <summary>
        ///   
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            string symbol = buf.Symbol.ToString().ToUpper();

            var enmrtr = buf.GetEnumerator();
            enmrtr.MoveNext();
            string source = enmrtr.Current.Source.Name.ToUpper();

            // in case if already have this snapshot
            if (snapshots.ContainsKey(buf.EventParams.SnapshotKey))
            {
                // flag SnapshotBegin came again
                if (buf.EventParams.Flags.HasFlag(EventFlag.SnapshotBegin))
                {
                    // in case when events Sell/Buy came in pair and both have SnapshotBegin flag. To keep both events
                    var snapshotEnmrtr = snapshots[buf.EventParams.SnapshotKey].GetEnumerator();
                    snapshotEnmrtr.MoveNext();
                    if (snapshots[buf.EventParams.SnapshotKey].Size != 1 || buf.Size != 1 || snapshotEnmrtr.Current.Side == enmrtr.Current.Side)
                    {
                        snapshots[buf.EventParams.SnapshotKey].Clear();
                    }
                    foreach (var order in buf)
                    {
                        receivedSnapshots[symbol].Remove(order.Source);
                    }
                    orderViewStates[symbol] = OrderViewState.Update;
                }

                // if order view is ready just call OnUpdate.
                // third condition for case when snapshots sent, but single event with SnapshotEnd flag appears
                // it will be sent as separate snapshot
                if (orderViewStates.ContainsKey(symbol) &&
                    orderViewStates[symbol].HasFlag(OrderViewState.Ready) &&
                    !buf.EventParams.Flags.HasFlag(EventFlag.SnapshotEnd))
                {
                    if (buf.EventParams.Flags.HasFlag(EventFlag.TxPending) || orderViewStates[symbol].HasFlag(OrderViewState.Pending))
                    {
                        // if pending begins
                        if (buf.EventParams.Flags.HasFlag(EventFlag.TxPending) && !orderViewStates[symbol].HasFlag(OrderViewState.Pending))
                        {
                            orderViewStates[symbol] |= OrderViewState.Pending;
                        }
                        foreach (var order in buf)
                        {
                            if (buf.EventParams.Flags.HasFlag(EventFlag.RemoveEvent))
                            {
                                NativeOrder no = new NativeOrder(order)
                                {
                                    Size = 0,
                                    Price = double.NaN,
                                    Time = TimeConverter.ToUtcDateTime(0),
                                    Sequence = 0,
                                    ExchangeCode = '\0',
                                    Count = 0
                                };
                                snapshots[buf.EventParams.SnapshotKey].AddEvent(no);
                            }
                            else
                                snapshots[buf.EventParams.SnapshotKey].AddEvent(order);
                        }
                        // if pending ends
                        if (!buf.EventParams.Flags.HasFlag(EventFlag.TxPending) && orderViewStates[symbol].HasFlag(OrderViewState.Pending))
                        {
                            orderViewStates[symbol] -= OrderViewState.Pending;

                            EventBuffer<IDxOrder> buffer = new EventBuffer<IDxOrder>(buf.EventType, buf.Symbol, buf.EventParams);
                            foreach (var order in snapshots[buf.EventParams.SnapshotKey])
                            {
                                buffer.AddEvent(order);
                            }
                            snapshots[buf.EventParams.SnapshotKey].Clear();
                            listener.OnUpdate<IDxEventBuf<IDxOrder>, IDxOrder>(buffer);
                        }
                    }
                    else
                    {
                        listener.OnUpdate<TB, TE>(buf);
                    }
                    return;
                }

                // ...otherwise continue fill current snapshot
                foreach (var order in buf)
                {
                    // order with zeros and NaN's, same as RemoveEvent flag
                    if (IsZeroOrder(order))
                    {
                        snapshots[buf.EventParams.SnapshotKey].Remove(order);
                        continue;
                    }
                    // if previos flag was TxPending we continue updating
                    if (snapshots[buf.EventParams.SnapshotKey].EventParams.Flags.HasFlag(EventFlag.TxPending) &&
                        !snapshots[buf.EventParams.SnapshotKey].EventParams.Flags.HasFlag(EventFlag.SnapshotEnd))
                    {
                        snapshots[buf.EventParams.SnapshotKey].ReplaceOrAdd(order);
                        snapshots[buf.EventParams.SnapshotKey].EventParams = buf.EventParams;
                        continue;
                    }
                    // if snapshot is consistent (fully received), it means that update transaction consist from
                    // only one event without flags
                    if (receivedSnapshots[symbol].Contains(source) && buf.EventParams.Flags == 0)
                    {
                        snapshots[buf.EventParams.SnapshotKey].ReplaceOrAdd(order);
                        continue;
                    }
                    // removing
                    if (buf.EventParams.Flags.HasFlag(EventFlag.RemoveEvent))
                    {
                        snapshots[buf.EventParams.SnapshotKey].Remove(order);
                        continue;
                    }
                    // ...or just add events
                    snapshots[buf.EventParams.SnapshotKey].ReplaceOrAdd(order);
                }

                if (buf.EventParams.Flags == 0 && snapshotsStates[buf.EventParams.SnapshotKey] == SnapshotState.Broken)
                {
                    snapshotsStates[buf.EventParams.SnapshotKey] = SnapshotState.Unbroken;
                    SnapshotEndFlagReceived<TB, TE>(buf);
                    return;
                }

                // no flags no actions
                if (buf.EventParams.Flags == 0)
                {
                    return;
                }
                // begin updating
                if (buf.EventParams.Flags.HasFlag(EventFlag.TxPending) && !buf.EventParams.Flags.HasFlag(EventFlag.RemoveEvent))
                {
                    // saving TxPending flag for future updating events
                    snapshots[buf.EventParams.SnapshotKey].EventParams = buf.EventParams;
                    if (receivedSnapshots[symbol].Contains(source))
                    {
                        // snapshot is no longer consistent (fully received) and should wait for SnapshotEnd flag again
                        receivedSnapshots[symbol].Remove(source);
                    }
                    foreach (var order in buf)
                    {
                        snapshots[buf.EventParams.SnapshotKey].ReplaceOrAdd(order);
                    }
                }

                if ((buf.EventParams.Flags.HasFlag(EventFlag.SnapshotEnd) || buf.EventParams.Flags.HasFlag(EventFlag.SnapshotSnip)) 
                    && !buf.EventParams.Flags.HasFlag(EventFlag.TxPending))
                {
                    SnapshotEndFlagReceived<TB, TE>(buf);
                    return;
                }
                else
                {
                    // snapshot is broken
                    snapshotsStates[buf.EventParams.SnapshotKey] = SnapshotState.Broken;
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
                        if (!IsZeroOrder(order))
                        {
                            outputBuffer.AddEvent(order);
                        }
                    }
                    snapshots.Add(buf.EventParams.SnapshotKey, outputBuffer);
                    snapshotsStates.Add(buf.EventParams.SnapshotKey, SnapshotState.Unbroken);
                    string symbolSource = symbol + source;
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

        private bool IsZeroOrder<TE>(TE order) where TE : IDxOrder
        {
            if (TimeConverter.ToUnixTime((order as IDxOrder).Time) == 0 &&
                double.IsNaN((order as IDxOrder).Price) &&
                (order as IDxOrder).Size == 0)
            {
                return true;
            }
            return false;
        }

        private void SnapshotEndFlagReceived<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            // to set right flags befor sending
            snapshots[buf.EventParams.SnapshotKey].EventParams = buf.EventParams;

            var enmrtr = buf.GetEnumerator();
            enmrtr.MoveNext();
            var source = enmrtr.Current.Source.Name.ToUpper();
            var symbol = buf.Symbol.ToString().ToUpper();
            receivedSnapshots[symbol].Add(source);

            if (receivedSnapshots[symbol].IsSupersetOf(sources))
            {
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
                        snapshots[snapshotKey].Clear();
                    }
                }
                listener.OnSnapshot<IDxEventBuf<IDxOrder>, IDxOrder>(buffer);
                orderViewStates[symbol] = OrderViewState.Ready;
            }
        }
        #endregion
    }
}
