﻿#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native
{
    internal class SimpleCandleDataProvider : IDxCandleListener, IDisposable
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
        private IDxConnection connection;
        private readonly object locker = new object();


        private readonly Dictionary<CandleSymbol, List<IDxCandle>> events =
            new Dictionary<CandleSymbol, List<IDxCandle>>();

        private TimeSpan timeout;
        private bool disconnected;

        public void OnCandle<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxCandle
        {
            lock (locker)
            {
                if (buf.Size <= 0) return;
                
                foreach (var candle in buf)
                {
                    events[CandleSymbol.ValueOf(buf.Symbol)].Add(candle);
                }
            }
        }

        internal SimpleCandleDataProvider()
        {
            disconnected = true;
        }

        public Task<Dictionary<CandleSymbol, List<IDxCandle>>> Run(string address,
            IEnumerable<CandleSymbol> symbols, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            return Task.Run(() =>
            {
                timeout = DefaultTimeout;
                var currentTime = DateTime.Now;
                var subs = new Dictionary<CandleSymbol, IDxSubscription>();

                lock (locker)
                {
                    disconnected = false;
                    connection = new NativeConnection(address, c =>
                    {
                        lock (locker)
                        {
                            disconnected = true;
                        }
                    });

                    foreach (var s in symbols)
                    {
                        events[s] = new List<IDxCandle>();
                        subs[s] = connection.CreateSubscription(EventType.Candle, 0, this);
                        subs[s].AddSymbol(s);
                    }
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    lock (locker)
                    {
                        if (disconnected || cancellationToken == CancellationToken.None &&
                            DateTime.Now > currentTime.AddMilliseconds(timeout.TotalMilliseconds))
                        {
                            break;
                        }
                    }

                    // ReSharper disable once MethodSupportsCancellation
                    Task.Delay(100).Wait();
                }

                foreach (var sub in subs.Values)
                {
                    sub.Clear();
                    sub.Dispose();
                }

                return events;
            });
        }

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}