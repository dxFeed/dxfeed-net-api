#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native
{
    internal class SimpleTimeAndSaleDataProvider : IDxTimeAndSaleListener, IDisposable
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
        private NativeConnection connection;
        private readonly object locker = new object();


        private readonly Dictionary<string, List<IDxTimeAndSale>> events =
            new Dictionary<string, List<IDxTimeAndSale>>();

        private TimeSpan timeout;
        private bool disconnected;

        public void OnTimeAndSale<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxTimeAndSale
        {
            lock (locker)
            {
                if (buf.Size <= 0) return;
                
                foreach (var tns in buf)
                {
                    events[tns.EventSymbol].Add(tns);
                }
            }
        }

        internal SimpleTimeAndSaleDataProvider()
        {
            disconnected = true;
        }

        public Task<Dictionary<string, List<IDxTimeAndSale>>> Run(string address,
            IEnumerable<string> symbols, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            return Task.Run(() =>
            {
                timeout = DefaultTimeout;
                var currentTime = DateTime.Now;
                var subs = new Dictionary<string, IDxSubscription>();

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
                        events[s] = new List<IDxTimeAndSale>();
                        subs[s] = connection.CreateSubscription(EventType.TimeAndSale, 0L, this);
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