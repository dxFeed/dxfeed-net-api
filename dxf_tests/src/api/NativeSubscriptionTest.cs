using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.data;
using com.dxfeed.native;
using com.dxfeed.tests.tools;


namespace com.dxfeed.api {
    /// <summary>
    /// Class tests methods of native subscription to create once
    /// </summary>
    [TestFixture]
    public class NativeSubscriptionTest {

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

        private static void OnDisconnect(IDxConnection con) {
            Interlocked.Exchange(ref isConnected, 0);
        }

        private static bool IsConnected() {
            return (Thread.VolatileRead(ref isConnected) == 1);
        }

        [Test]
        public void TestAddSymbol() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbol("AAPL");

                    listener.WaitEvents<IDxOrder>();

                    TestListener.ReceivedEvent<IDxOrder> e = listener.GetLastEvent<IDxOrder>();
                    Assert.AreEqual("AAPL", e.Symbol);

                    s.AddSymbol("IBM");
                    listener.ClearEvents<IDxOrder>();
                    listener.WaitEvents<IDxOrder>("AAPL", "IBM");
                }
            }
        }

        [Test]
        public void TestAddSymbolCandle() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            string symbol = "AAPL";
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbol(symbol);

                    listener.WaitEvents<IDxOrder>();

                    CandleSymbol candleSymbol = CandleSymbol.ValueOf("XBT/USD{=d}");
                    s.AddSymbol(candleSymbol);
                    listener.ClearEvents<IDxOrder>();
                    Assert.AreEqual(1, s.GetSymbols().Count);
                    Assert.AreEqual(symbol, s.GetSymbols()[0]);
                }
            }
        }

        [Test]
        public void TestAddSymbols() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbols("AAPL", "XBT/USD");

                    listener.WaitEvents<IDxOrder>("AAPL", "XBT/USD");

                    listener.ClearEvents<IDxOrder>();
                    s.AddSymbols("IBM", "MSFT");
                    listener.WaitEvents<IDxOrder>("AAPL", "XBT/USD", "IBM", "MSFT");
                }
            }
        }

        [Test]
        public void TestAddSymbolsCandle() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            string symbol = "AAPL";
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbol(symbol);

                    listener.WaitEvents<IDxOrder>();

                    CandleSymbol[] candleSymbols = new CandleSymbol[] { 
                        CandleSymbol.ValueOf("XBT/USD{=d}"), 
                        CandleSymbol.ValueOf("AAPL{=d}"),
                        CandleSymbol.ValueOf("IBM{=d}")
                    };
                    s.AddSymbols(candleSymbols);
                    listener.ClearEvents<IDxOrder>();
                    Assert.AreEqual(1, s.GetSymbols().Count);
                    Assert.AreEqual(symbol, s.GetSymbols()[0]);
                }
            }
        }

        [Test]
        public void TestRemoveSymbols() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            string[] symbols = { "AAPL", "IBM", "XBT/USD" };
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbols("AAPL", "IBM", "XBT/USD");

                    listener.WaitEvents<IDxOrder>(symbols);

                    s.RemoveSymbols("IBM", "XBT/USD");
                    listener.ClearEvents<IDxOrder>();

                    Thread.Sleep(10000);
                    listener.WaitEvents<IDxOrder>("AAPL");
                    Assert.GreaterOrEqual(listener.GetEventCount<IDxOrder>("AAPL"), 1);
                    Assert.AreEqual(0, listener.GetEventCount<IDxOrder>("IBM", "XBT/USD"));
                }
            }
        }

        [Test]
        public void TestRemoveSymbolsCandle() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            string[] symbols = { "AAPL", "IBM", "XBT/USD" };
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbols(symbols);

                    listener.WaitEvents<IDxOrder>(symbols);

                    CandleSymbol[] candleSymbols = new CandleSymbol[] { 
                        CandleSymbol.ValueOf("XBT/USD{=d}"), 
                        CandleSymbol.ValueOf("AAPL{=d}"),
                        CandleSymbol.ValueOf("IBM{=d}")
                    };
                    s.RemoveSymbols(candleSymbols);

                    listener.ClearEvents<IDxOrder>();
                    Assert.AreEqual(symbols.Length, s.GetSymbols().Count);
                    listener.WaitEvents<IDxOrder>(symbols);
                }
            }
        }

        [Test]
        public void TestSetSymbols() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbols("AAPL", "IBM");

                    listener.WaitEvents<IDxOrder>("AAPL", "IBM");

                    s.SetSymbols("XBT/USD");
                    listener.ClearEvents<IDxOrder>();

                    Thread.Sleep(10000);
                    listener.WaitEvents<IDxOrder>("XBT/USD");
                    Assert.GreaterOrEqual(listener.GetEventCount<IDxOrder>("XBT/USD"), 1);
                    Assert.AreEqual(0, listener.GetEventCount<IDxOrder>("AAPL", "IBM"));

                    // add another symbols
                    s.AddSymbols("AAPL", "IBM");
                    listener.WaitEvents<IDxOrder>("XBT/USD", "AAPL", "IBM");
                }
            }
        }

        [Test]
        public void TestSetSymbolsCandle() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbols("AAPL", "IBM");

                    listener.WaitEvents<IDxOrder>("AAPL", "IBM");

                    CandleSymbol[] candleSymbols = new CandleSymbol[] { 
                        CandleSymbol.ValueOf("XBT/USD{=d}"), 
                        CandleSymbol.ValueOf("AAPL{=d}"),
                        CandleSymbol.ValueOf("IBM{=d}")
                    };
                    s.SetSymbols(candleSymbols);

                    listener.ClearEvents<IDxOrder>();
                    Assert.AreEqual(2, s.GetSymbols().Count);
                    listener.WaitEvents<IDxOrder>("AAPL", "IBM");
                }
            }
        }

        [Test]
        public void TestClearSymbols() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbols("AAPL", "IBM");

                    listener.WaitEvents<IDxOrder>("AAPL", "IBM");

                    s.Clear();
                    listener.ClearEvents<IDxOrder>();

                    Thread.Sleep(10000);
                    Assert.AreEqual(0, listener.GetEventCount<IDxOrder>("AAPL", "IBM"));

                    //add another symbol
                    s.AddSymbols("XBT/USD");
                    listener.WaitEvents<IDxOrder>("XBT/USD");
                }
            }
        }

        [Test]
        public void TestGetSymbols() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    List<string> symbols = new List<string>(new string[] { "AAPL", "IBM" });
                    s.AddSymbols(symbols.ToArray());

                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(symbols.Count, returnedSymbolList.Count);
                    foreach (string symbol in returnedSymbolList)
                        Assert.True(symbols.Contains(symbol));

                    s.Clear();
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(0, returnedSymbolList.Count);
                }
            }
        }

        [Test]
        public void TestSetSource() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            string source = "NTV";
            string[] sources2 = new string[] { "DEX", "DEA" };
            string[] symbols = new string[] { "AAPL", "IBM" };
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbols(symbols);
                    Thread.Sleep(3000);

                    s.SetSource(source);
                    Thread.Sleep(10000);
                    listener.ClearEvents<IDxOrder>();
                    Thread.Sleep(3000);
                    listener.WaitOrders(source);
                    listener.WaitEvents<IDxOrder>(symbols);
                    Assert.AreEqual(0, listener.GetOrderCount(sources2));
                }
            }
        }

        [Test]
        public void TestSetSource2() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            string source = "NTV";
            string[] sources2 = new string[] { "DEX", "DEA" };
            string[] allSource = new string[] { "NTV", "DEX", "DEA" };
            string[] symbols = new string[] { "AAPL", "IBM", "XBT/USD" };
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.SetSource(source);
                    s.AddSymbols(symbols);

                    Thread.Sleep(3000);
                    listener.WaitOrders(source);
                    listener.WaitEvents<IDxOrder>(symbols);
                    Assert.AreEqual(0, listener.GetOrderCount(sources2));

                    s.SetSource(sources2);
                    Thread.Sleep(1000);
                    listener.ClearEvents<IDxOrder>();
                    Thread.Sleep(3000);
                    listener.WaitOrders(sources2);
                    listener.WaitEvents<IDxOrder>(symbols);
                    Assert.AreEqual(0, listener.GetOrderCount(source));
                }

            }
        }

        [Test]
        public void TestSetSource3() {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            EventType events = EventType.Order;
            string source = "NTV";
            string[] sources2 = new string[] { "DEX", "DEA" };
            string[] allSource = new string[] { "NTV", "DEX", "DEA" };
            string[] symbols = new string[] { "AAPL", "IBM", "XBT/USD" };
            using (var con = new NativeConnection(address, OnDisconnect)) {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(events, listener)) {
                    s.AddSymbols(symbols);
                    listener.WaitOrders(allSource);
                    listener.WaitEvents<IDxOrder>(symbols);

                    s.SetSource(source);
                    Thread.Sleep(12000);
                    listener.ClearEvents<IDxOrder>();
                    Thread.Sleep(3000);
                    listener.WaitOrders(source);
                    listener.WaitEvents<IDxOrder>(symbols);
                    Assert.AreEqual(0, listener.GetOrderCount(sources2));

                    s.AddSource(sources2);
                    Thread.Sleep(1000);
                    listener.ClearEvents<IDxOrder>();
                    Thread.Sleep(3000);
                    listener.WaitOrders(allSource);
                    listener.WaitEvents<IDxOrder>(symbols);
                }
            }
        }

    }
}
