#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Threading;
using com.dxfeed.api.candle;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native;
using com.dxfeed.tests.tools;
using NUnit.Framework;

namespace com.dxfeed.api
{
    /// <summary>
    ///     This class creates all possible subscriptions and just wait events. It
    ///     is specified for make package operation.
    /// </summary>
    [TestFixture]
    public class FullTest
    {
        private class SnapshotCase : IDisposable
        {
            private readonly CandleSymbol candleSymbol;
            private readonly string source = string.Empty;
            private readonly string symbol = string.Empty;
            private readonly DateTime? time;
            private IDxSubscription snapshotSubscription;

            private SnapshotCase(DateTime? time)
            {
                this.time = time;
            }

            public SnapshotCase(string symbol, string source, DateTime? time) : this(time)
            {
                this.symbol = symbol;
                this.source = source;
                SnapshotType = typeof(IDxOrder);
            }

            public SnapshotCase(CandleSymbol symbol, DateTime? time) : this(time)
            {
                candleSymbol = symbol;
                SnapshotType = typeof(IDxCandle);
            }

            public Type SnapshotType { get; }

            public string Symbol => candleSymbol == null ? symbol : candleSymbol.ToString();

            public string Source => candleSymbol == null ? source : string.Empty;

            public void Dispose()
            {
                if (snapshotSubscription != null)
                    snapshotSubscription.Dispose();
            }

            public void Initialize(NativeConnection connection, IDxSnapshotListener listener)
            {
                snapshotSubscription = connection.CreateSnapshotSubscription(time, listener);
                if (candleSymbol != null)
                {
                    snapshotSubscription.AddSymbol(candleSymbol);
                }
                else
                {
                    snapshotSubscription.AddSource(source);
                    snapshotSubscription.AddSymbol(symbol);
                }
            }
        }

        private class SnapshotCollection : IDisposable
        {
            private readonly SnapshotCase[] snapshotCases;

            public SnapshotCollection(NativeConnection connection,
                IDxSnapshotListener listener, SnapshotCase[] snapshotCases)
            {
                this.snapshotCases = snapshotCases;
                foreach (var snapshot in this.snapshotCases)
                    snapshot.Initialize(connection, listener);
            }

            public void Dispose()
            {
                foreach (var snapshot in snapshotCases)
                    snapshot.Dispose();
            }
        }

        private static readonly string address = "mddqa.in.devexperts.com:7400";
        private static int isConnected;

        /// <summary>
        ///     Events timeout 3min
        /// </summary>
        private static readonly int eventsTimeout = 180000;

        /// <summary>
        ///     Events loop sleep time is 100 millis
        /// </summary>
        private static readonly int eventsSleepTime = 100;

        /// <summary>
        ///     The common time in milliseconds before test will be complete.
        /// </summary>
        private static readonly int testCommonTime = 60000;

        /// <summary>
        ///     The interval in milliseconds between test outputs.
        /// </summary>
        private static readonly int testPrintInterval = 1000;

        private static readonly DateTime oneMonth = DateTime.Now.Add(new TimeSpan(-30, 0, 0, 0));
        private static readonly string[] eventSymbols = { "AAPL", "IBM" };
        private static readonly string[] candleSymbols = { "XBT/USD{=d}" };

        private static readonly SnapshotCase[] snapshotCases =
        {
            new SnapshotCase("AAPL", "NTV", null),
            new SnapshotCase("IBM", OrderSource.AGGREGATE_BID, null),
            new SnapshotCase(CandleSymbol.ValueOf("XBT/USD{=d}"), oneMonth)
        };

        private static void OnDisconnect(IDxConnection con)
        {
            Interlocked.Exchange(ref isConnected, 0);
        }

        private static bool IsConnected()
        {
            return Thread.VolatileRead(ref isConnected) == 1;
        }

        private void PrintEvents<TE>(TestListener listener, params string[] symbols)
        {
            Console.WriteLine("Event {0}: Total data count: {1}", typeof(TE), listener.GetEventCount<TE>());
            foreach (var symbol in symbols)
                Console.WriteLine("    symbol {0}: data count: {1}", symbol, listener.GetEventCount<TE>(symbol));
        }

        private void PrintSnapshots<TE>(SnapshotTestListener listener, params SnapshotCase[] cases)
        {
            Console.WriteLine("Snapshots of {0}: Total count: {1}", typeof(TE), listener.GetSnapshotsCount<TE>());
            foreach (var snapshotCase in cases)
            {
                if (snapshotCase.SnapshotType != typeof(TE))
                    continue;
                if (snapshotCase.SnapshotType == typeof(IDxOrder))
                    Console.WriteLine("    symbol {0}#{1}: data count: {2}",
                        snapshotCase.Symbol, snapshotCase.Source,
                        listener.GetSnapshotsCount<IDxOrder>(snapshotCase.Symbol));
                else if (snapshotCase.SnapshotType == typeof(IDxCandle))
                    Console.WriteLine("    symbol {0}: data count: {1}",
                        snapshotCase.Symbol,
                        listener.GetSnapshotsCount<IDxCandle>(snapshotCase.Symbol));
            }
        }
        
        [Test]
        public void TestAll()
        {
            var eventListener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            var snapshotListener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            var events = EventType.Order | EventType.Profile |
                         EventType.Quote | EventType.Summary | EventType.TimeAndSale | EventType.Series |
                         EventType.Trade;
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription eventSubscription = con.CreateSubscription(events, eventListener),
                    candleSubscription = con.CreateSubscription(oneMonth, eventListener))
                using (var snapshotCollection = new SnapshotCollection(con, snapshotListener, snapshotCases))
                {
                    eventSubscription.AddSymbols(eventSymbols);
                    candleSubscription.AddSymbol(CandleSymbol.ValueOf(candleSymbols[0]));

                    var startTime = DateTime.Now;
                    while (testCommonTime >= (DateTime.Now - startTime).TotalMilliseconds)
                    {
                        Console.WriteLine();
                        PrintEvents<IDxCandle>(eventListener, candleSymbols);
                        PrintEvents<IDxOrder>(eventListener, eventSymbols);
                        PrintEvents<IDxProfile>(eventListener, eventSymbols);
                        PrintEvents<IDxQuote>(eventListener, eventSymbols);
                        PrintEvents<IDxSummary>(eventListener, eventSymbols);
                        PrintEvents<IDxTimeAndSale>(eventListener, eventSymbols);
                        PrintEvents<IDxSeries>(eventListener, eventSymbols);
                        PrintEvents<IDxTrade>(eventListener, eventSymbols);

                        PrintSnapshots<IDxOrder>(snapshotListener, snapshotCases);
                        PrintSnapshots<IDxCandle>(snapshotListener, snapshotCases);

                        Thread.Sleep(testPrintInterval);
                    }
                }
            }
        }
    }
}