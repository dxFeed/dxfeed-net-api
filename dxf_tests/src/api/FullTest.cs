#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using NUnit.Framework;
using com.dxfeed.api.candle;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native;
using com.dxfeed.tests.tools;
using System.Threading;

namespace com.dxfeed.api
{
    /// <summary>
    /// This class creates all possible subscriptions and just wait events. It
    /// is specified for make package operation.
    ///
    /// </summary>
    [TestFixture]
    public class FullTest
    {

        private class SnapshotCase : IDisposable
        {
            private DateTime? time = null;
            private string symbol = string.Empty;
            private string source = string.Empty;
            private CandleSymbol candleSymbol = null;
            private IDxSubscription snapshotSubscription = null;

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

            public Type SnapshotType { get; private set; }

            public string Symbol
            {
                get
                {
                    return (candleSymbol == null ? symbol : candleSymbol.ToString());
                }
            }

            public string Source
            {
                get
                {
                    return (candleSymbol == null ? source : string.Empty);
                }
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

            public void Dispose()
            {
                if (snapshotSubscription != null)
                    snapshotSubscription.Dispose();
            }
        }

        private class SnapshotCollection : IDisposable
        {

            private SnapshotCase[] snapshotCases = null;

            public SnapshotCollection(NativeConnection connection,
                IDxSnapshotListener listener, SnapshotCase[] snapshotCases)
            {
                this.snapshotCases = snapshotCases;
                foreach (SnapshotCase snapshot in this.snapshotCases)
                    snapshot.Initialize(connection, listener);
            }

            public void Dispose()
            {
                foreach (SnapshotCase snapshot in snapshotCases)
                    snapshot.Dispose();
            }
        }

        static string address = "mddqa.in.devexperts.com:7400";
        static int isConnected = 0;
        /// <summary>
        /// Events timeout 3min
        /// </summary>
        static int eventsTimeout = 180000;
        /// <summary>
        /// Events loop sleep time is 100 millis
        /// </summary>
        static int eventsSleepTime = 100;
        /// <summary>
        /// The common time in milliseconds before test will be complete.
        /// </summary>
        static int testCommonTime = 60000;
        /// <summary>
        /// The interval in milliseconds between test outputs.
        /// </summary>
        static int testPrintInterval = 1000;

        static DateTime oneMonth = DateTime.Now.Add(new TimeSpan(-30, 0, 0, 0));
        static string[] eventSymbols = { "AAPL", "IBM" };
        static string[] candleSymbols = { "XBT/USD{=d}" };
        static SnapshotCase[] snapshotCases = {
            new SnapshotCase("AAPL", "NTV", null),
            new SnapshotCase("IBM", SnapshotTestListener.COMPOSITE_BID, null),
            new SnapshotCase(CandleSymbol.ValueOf("XBT/USD{=d}"), oneMonth)
        };
        static string[] orderViewSymbols = { "AAPL", "IBM" };
        static string[] orderViewSources = { "NTV", "DEA", "DEX" };

        private static void OnDisconnect(IDxConnection con)
        {
            Interlocked.Exchange(ref isConnected, 0);
        }

        private static bool IsConnected()
        {
            return (Thread.VolatileRead(ref isConnected) == 1);
        }

        private void PrintEvents<TE>(TestListener listener, params string[] symbols)
        {
            Console.WriteLine(string.Format("Event {0}: Total data count: {1}", typeof(TE), listener.GetEventCount<TE>()));
            foreach (string symbol in symbols)
                Console.WriteLine("    symbol {0}: data count: {1}", symbol, listener.GetEventCount<TE>(symbol));
        }

        private void PrintSnapshots<TE>(SnapshotTestListener listener, params SnapshotCase[] cases)
        {
            Console.WriteLine(string.Format("Snapshots of {0}: Total count: {1}", typeof(TE), listener.GetSnapshotsCount<TE>()));
            foreach (SnapshotCase snapshotCase in cases)
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

        private void PrintOrderViews(OrderViewTestListener listener, params string[] symbols)
        {
            Console.WriteLine(string.Format("OrderViews, count: {0}", listener.GetOrderViewsCount()));
            foreach (string symbol in symbols)
                Console.WriteLine("    symbol {0}: snapshot size: {1}, updates size: {2}",
                    symbol, listener.GetOrderViewEventsCount(symbol), listener.GetOrderViewUpdatesCount(symbol));
        }

        [Test]
        public void TestAll()
        {
            TestListener eventListener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            SnapshotTestListener snapshotListener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            OrderViewTestListener orderViewListener = new OrderViewTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order | EventType.Profile |
                EventType.Quote | EventType.Summary | EventType.TimeAndSale | EventType.Series |
                EventType.Trade;
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription eventSubscription = con.CreateSubscription(events, eventListener),
                    candleSubscription = con.CreateSubscription(oneMonth, eventListener),
                    orderViewSubscription = con.CreateOrderViewSubscription(orderViewListener))
                using (SnapshotCollection snapshotCollection = new SnapshotCollection(con, snapshotListener, snapshotCases))
                {
                    eventSubscription.AddSymbols(eventSymbols);
                    candleSubscription.AddSymbol(CandleSymbol.ValueOf(candleSymbols[0]));
                    orderViewSubscription.AddSource(orderViewSources);
                    orderViewSubscription.AddSymbols(orderViewSymbols);

                    DateTime startTime = DateTime.Now;
                    while(testCommonTime >= (DateTime.Now - startTime).TotalMilliseconds)
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

                        PrintOrderViews(orderViewListener, orderViewSymbols);
                        Thread.Sleep(testPrintInterval);
                    }
                }
            }
        }
    }
}
