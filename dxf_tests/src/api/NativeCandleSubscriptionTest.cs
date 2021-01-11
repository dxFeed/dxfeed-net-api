#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.native;
using com.dxfeed.tests.tools;

namespace com.dxfeed.api
{
    [TestFixture]
    public class NativeCandleSubscriptionTest
    {
        static string address = "mddqa.in.devexperts.com:7400";
        static int isConnected = 0;
        /// <summary>
        /// Events timeout 2min
        /// </summary>
        static int eventsTimeout = 120000;
        /// <summary>
        /// Events loop sleep time is 100 millis
        /// </summary>
        static int eventsSleepTime = 100;

        DateTime? defaultDateTime = null;

        private static void OnDisconnect(IDxConnection con)
        {
            Interlocked.Exchange(ref isConnected, 0);
        }

        private static bool IsConnected()
        {
            return (Thread.VolatileRead(ref isConnected) == 1);
        }

        [Test]
        public void TestAddSymbol()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {

                    //try to add non-candle symbol
                    s.AddSymbol("AAPL");
                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(0, returnedSymbolList.Count);

                    //add candle symbol
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);
                }
            }
        }

        [Test]
        public void TestAddSymbolCandle()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            string anotherCandleSymbolString = "AAPL{=d,price=mark}";
            CandleSymbol anotherCandleSymbol = CandleSymbol.ValueOf(anotherCandleSymbolString);
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    listener.WaitEvents<IDxCandle>(candleSymbolString);

                    s.AddSymbol(anotherCandleSymbol);
                    listener.ClearEvents<IDxCandle>();
                    listener.WaitEvents<IDxCandle>(candleSymbolString, anotherCandleSymbolString);
                }
            }
        }

        [Test]
        public void TestAddSymbols()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    //add candle symbol
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);

                    //try to add other non-candle symbols
                    s.AddSymbols("AAPL", "IBM");
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);

                    listener.WaitEvents<IDxCandle>(candleSymbolString);
                }
            }
        }

        [Test]
        public void TestAddSymbolsCandle()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            string aaplSymbolString = "AAPL{=d,price=mark}";
            string ibmSymbolString = "IBM{=d,price=mark}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    //add candle symbol
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);
                    listener.WaitEvents<IDxCandle>(candleSymbolString);

                    listener.ClearEvents<IDxCandle>();

                    //try to add other candle symbols
                    s.AddSymbols(new CandleSymbol[] {
                        CandleSymbol.ValueOf(aaplSymbolString),
                        CandleSymbol.ValueOf(ibmSymbolString)
                    });
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(3, returnedSymbolList.Count);
                    listener.WaitEvents<IDxCandle>(new string[] {
                        candleSymbolString,
                        aaplSymbolString,
                        ibmSymbolString
                    });
                }
            }
        }

        [Test]
        public void TestRemoveSymbols()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    //add candle symbol
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    listener.WaitEvents<IDxCandle>(candleSymbolString);

                    //try to remove symbols
                    s.RemoveSymbols("AAPL", "IBM", "XBT/USD");
                    IList<string> returnedSymbolList = s.GetSymbols();
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);

                    listener.ClearEvents<IDxCandle>();
                    listener.WaitEvents<IDxCandle>(candleSymbolString);
                }
            }
        }

        [Test]
        public void TestRemoveSymbolsCandle()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            string aaplSymbolString = "AAPL{=d,price=mark}";
            string ibmSymbolString = "IBM{=d,price=mark}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    //add candle symbols
                    s.AddSymbols(new CandleSymbol[] {
                        CandleSymbol.ValueOf(candleSymbolString),
                        CandleSymbol.ValueOf(aaplSymbolString),
                        CandleSymbol.ValueOf(ibmSymbolString)
                    });
                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(3, returnedSymbolList.Count);
                    listener.WaitEvents<IDxCandle>(new string[] {
                        candleSymbolString,
                        aaplSymbolString,
                        ibmSymbolString
                    });

                    //try to remove symbols
                    s.RemoveSymbols(new CandleSymbol[] {
                        CandleSymbol.ValueOf(aaplSymbolString),
                        CandleSymbol.ValueOf(ibmSymbolString)
                    });
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    listener.ClearEvents<IDxCandle>();
                    listener.WaitEvents<IDxCandle>(candleSymbolString);
                }
            }
        }

        [Test]
        public void TestSetSymbols()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    //add candle symbol
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);

                    //try to set other non-candle symbols
                    s.SetSymbols("AAPL", "IBM");
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);
                }
            }
        }

        [Test]
        public void TestSetSymbolsCandle()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            string aaplSymbolString = "AAPL{=d,price=mark}";
            string ibmSymbolString = "IBM{=d,price=mark}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    //add candle symbol
                    s.AddSymbols(new CandleSymbol[] {
                        CandleSymbol.ValueOf(aaplSymbolString),
                        CandleSymbol.ValueOf(ibmSymbolString)
                    });
                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(2, returnedSymbolList.Count);
                    listener.WaitEvents<IDxCandle>(aaplSymbolString, ibmSymbolString);

                    //try to set other non-candle symbols
                    s.SetSymbols(CandleSymbol.ValueOf(candleSymbolString));
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);
                    listener.ClearEvents<IDxCandle>();
                    listener.WaitEvents<IDxCandle>(candleSymbolString);
                }
            }
        }

        [Test]
        public void TestClearSymbols()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            string aaplSymbolString = "AAPL{=d,price=mark}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    listener.WaitEvents<IDxCandle>(candleSymbolString);

                    s.Clear();
                    listener.ClearEvents<IDxCandle>();
                    Thread.Sleep(10000);
                    Assert.AreEqual(0, listener.GetEventCount<IDxCandle>());

                    //try to restore symbols
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    listener.WaitEvents<IDxCandle>(candleSymbolString);

                    //set other symbol
                    s.Clear();
                    listener.ClearEvents<IDxCandle>();
                    s.AddSymbol(CandleSymbol.ValueOf(aaplSymbolString));
                    listener.WaitEvents<IDxCandle>(aaplSymbolString);
                    Assert.AreEqual(listener.GetEventCount<IDxCandle>(), listener.GetEventCount<IDxCandle>(aaplSymbolString));
                }
            }
        }

        [Test]
        public void TestGetSymbols()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            CandleSymbol candleSymbol = CandleSymbol.ValueOf(candleSymbolString);
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    s.AddSymbol(candleSymbol);
                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleSymbolString, returnedSymbolList[0]);
                }
            }
        }

        [Test]
        public void TestAddSource()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    s.AddSource("IST");
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    s.AddSource("NTV", "DEX");
                    listener.WaitEvents<IDxCandle>(candleSymbolString);
                }
            }
        }

        [Test]
        public void TestSetSource()
        {
            TestListener listener = new TestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleSymbolString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSubscription(defaultDateTime, listener))
                {
                    s.SetSource("IST");
                    s.AddSymbol(CandleSymbol.ValueOf(candleSymbolString));
                    s.SetSource("NTV", "DEX");
                    listener.WaitEvents<IDxCandle>(candleSymbolString);
                }
            }
        }
    }
}
