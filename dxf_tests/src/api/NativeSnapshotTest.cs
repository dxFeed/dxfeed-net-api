/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

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
    public class NativeSnapshotTest
    {
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
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string source = "NTV";
            string symbol = "AAPL";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbol((string)null); });
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbol(string.Empty); });
                    s.AddSource(source);
                    s.AddSymbol(symbol);

                    listener.WaitSnapshot<IDxOrder>(symbol, source);

                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbol("IBM"); });
                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbol(CandleSymbol.ValueOf("AAPL{=d,price=mark}")); });
                }
            }
        }

        [Test]
        public void TestAddSymbolCandle()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbol((CandleSymbol)null); });
                    s.AddSymbol(CandleSymbol.ValueOf(candleString));

                    listener.WaitSnapshot<IDxCandle>(candleString);

                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbol("IBM"); });
                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbol(CandleSymbol.ValueOf("AAPL{=d,price=mark}")); });
                }
            }
        }

        [Test]
        public void TestAddSymbols()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string source = "NTV";
            string symbol = "AAPL";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbols((string[])null); });
                    Assert.Throws<InvalidOperationException>(delegate { s.AddSymbols(new string[] { }); });
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbols(new string[] { string.Empty }); });
                    Assert.Throws<InvalidOperationException>(delegate { s.AddSymbols(new string[] { "AAPL", "XBT/USD" }); });

                    s.AddSource(source);
                    s.AddSymbols(new string[] { symbol });

                    listener.WaitSnapshot<IDxOrder>(symbol, source);

                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbols("IBM"); });
                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbols(CandleSymbol.ValueOf("AAPL{=d,price=mark}")); });
                }
            }
        }

        [Test]
        public void TestAddSymbolsCandle()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbols((CandleSymbol[])null); });
                    Assert.Throws<InvalidOperationException>(delegate { s.AddSymbols(new CandleSymbol[] { }); });
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbols(new CandleSymbol[] { null }); });
                    Assert.Throws<InvalidOperationException>(delegate
                    {
                        s.AddSymbols(new CandleSymbol[] {
                            CandleSymbol.ValueOf("AAPL{=d,price=mark}"),
                            CandleSymbol.ValueOf("XBT/USD{=d,price=mark}")
                        });
                    });

                    s.AddSymbols(new CandleSymbol[] { CandleSymbol.ValueOf(candleString) });

                    listener.WaitSnapshot<IDxCandle>(candleString);

                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbols("IBM"); });
                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbols(CandleSymbol.ValueOf("AAPL{=d,price=mark}")); });
                }
            }
        }

        [Test]
        public void TestRemoveSymbols()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string source = "NTV";
            string symbol = "AAPL";
            string[] unusingSymbols = { "IBM", "XBT/USD" };
            string[] usingSymbols = { symbol, "IBM", "XBT/USD" };
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSource(source);
                    s.AddSymbol(symbol);

                    Assert.AreEqual(1, s.GetSymbols().Count);
                    listener.WaitSnapshot<IDxOrder>(symbol, source);

                    Assert.Throws<ArgumentException>(delegate { s.RemoveSymbols((string[])null); });
                    s.RemoveSymbols(new string[] { string.Empty });
                    s.RemoveSymbols(new CandleSymbol[] {
                        CandleSymbol.ValueOf("AAPL{=d,price=mark}"),
                        CandleSymbol.ValueOf("XBT/USD{=d,price=mark}")
                    });
                    s.RemoveSymbols(unusingSymbols);
                    Thread.Sleep(5000);
                    Assert.AreEqual(1, s.GetSymbols().Count);

                    s.RemoveSymbols(usingSymbols);
                    Thread.Sleep(3000);
                    Assert.AreEqual(0, s.GetSymbols().Count);
                    listener.ClearEvents<IDxOrder>();
                    Thread.Sleep(10000);
                    Assert.AreEqual(0, listener.GetSnapshotsCount<IDxOrder>());
                }
            }
        }

        [Test]
        public void TestRemoveSymbolsCandle()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleString = "XBT/USD{=d}";
            CandleSymbol[] unusingSymbols = new CandleSymbol[] {
                CandleSymbol.ValueOf("AAPL{=d,price=mark}"),
                CandleSymbol.ValueOf("XBT/USD{=d,price=mark}")
            };
            CandleSymbol[] usingSymbols = new CandleSymbol[] {
                CandleSymbol.ValueOf(candleString),
                CandleSymbol.ValueOf("AAPL{=d,price=mark}"),
                CandleSymbol.ValueOf("XBT/USD{=d,price=mark}")
            };
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSymbol(CandleSymbol.ValueOf(candleString));

                    Assert.AreEqual(1, s.GetSymbols().Count);
                    listener.WaitSnapshot<IDxCandle>(candleString);

                    Assert.Throws<ArgumentException>(delegate { s.RemoveSymbols((CandleSymbol[])null); });
                    s.RemoveSymbols(new CandleSymbol[] { null });
                    s.RemoveSymbols("AAPL", "XBT/USD");
                    s.RemoveSymbols(unusingSymbols);
                    Thread.Sleep(5000);
                    Assert.AreEqual(1, s.GetSymbols().Count);

                    s.RemoveSymbols(usingSymbols);
                    Thread.Sleep(3000);
                    Assert.AreEqual(0, s.GetSymbols().Count);
                    listener.ClearEvents<IDxCandle>();
                    Thread.Sleep(10000);
                    Assert.AreEqual(0, listener.GetSnapshotsCount<IDxCandle>());
                }
            }
        }

        [Test]
        public void TestSetSymbols()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string source = "NTV";
            string symbol = "AAPL";
            string otherSymbol = "IBM";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.SetSymbols((string[])null); });
                    Assert.Throws<InvalidOperationException>(delegate { s.SetSymbols(new string[] { }); });
                    Assert.Throws<ArgumentException>(delegate { s.SetSymbols(new string[] { string.Empty }); });
                    Assert.Throws<InvalidOperationException>(delegate { s.SetSymbols(new string[] { "AAPL", "XBT/USD" }); });

                    s.AddSource(source);
                    s.SetSymbols(new string[] { symbol });
                    listener.WaitSnapshot<IDxOrder>(symbol, source);

                    s.SetSymbols(new string[] { otherSymbol });
                    listener.ClearEvents<IDxOrder>();
                    Thread.Sleep(10000);
                    listener.WaitSnapshot<IDxOrder>(otherSymbol, source);
                    Assert.AreEqual(1, listener.GetSnapshotsCount<IDxOrder>(otherSymbol));
                    Assert.AreEqual(0, listener.GetSnapshotsCount<IDxOrder>(symbol));

                    // add another symbols
                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbols("IBM"); });
                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbols(CandleSymbol.ValueOf("AAPL{=d,price=mark}")); });
                }
            }
        }

        [Test]
        public void TestSetSymbolsCandle()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleString = "XBT/USD{=d}";
            string otherCandleString = "XBT/USD{=2d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbols((CandleSymbol[])null); });
                    Assert.Throws<InvalidOperationException>(delegate { s.AddSymbols(new CandleSymbol[] { }); });
                    Assert.Throws<ArgumentException>(delegate { s.AddSymbols(new CandleSymbol[] { null }); });
                    Assert.Throws<InvalidOperationException>(delegate
                    {
                        s.AddSymbols(new CandleSymbol[] {
                            CandleSymbol.ValueOf("AAPL{=d,price=mark}"),
                            CandleSymbol.ValueOf("XBT/USD{=d,price=mark}")
                         });
                    });

                    s.SetSymbols(new CandleSymbol[] { CandleSymbol.ValueOf(candleString) });
                    listener.WaitSnapshot<IDxCandle>(candleString);

                    s.SetSymbols(new CandleSymbol[] { CandleSymbol.ValueOf(otherCandleString) });
                    listener.ClearEvents<IDxCandle>();
                    Thread.Sleep(10000);
                    listener.WaitSnapshot<IDxCandle>(otherCandleString);
                    Assert.AreEqual(1, listener.GetSnapshotsCount<IDxCandle>(otherCandleString));
                    Assert.AreEqual(0, listener.GetSnapshotsCount<IDxCandle>(candleString));

                    // add another symbols
                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbols("IBM"); });
                    Assert.Throws(typeof(InvalidOperationException), delegate { s.AddSymbols(CandleSymbol.ValueOf("AAPL{=d,price=mark}")); });
                }
            }
        }

        [Test]
        public void TestClearSymbols()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string source = "NTV";
            string symbol = "AAPL";
            string otherSymbol = "IBM";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSource(source);
                    s.AddSymbol(symbol);
                    listener.WaitSnapshot<IDxOrder>(symbol, source);

                    s.Clear();
                    listener.ClearEvents<IDxOrder>();
                    Assert.AreEqual(0, s.GetSymbols().Count);

                    Thread.Sleep(10000);
                    Assert.AreEqual(0, listener.GetSnapshotsCount<IDxOrder>(symbol));

                    //add another symbol
                    s.AddSymbols(otherSymbol);
                    listener.WaitSnapshot<IDxOrder>(otherSymbol);
                }
            }
        }

        [Test]
        public void TestClearSymbols2()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string source = "NTV";
            string symbol = "AAPL";
            string candleString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSource(source);
                    s.AddSymbol(symbol);
                    listener.WaitSnapshot<IDxOrder>(symbol, source);

                    s.Clear();
                    listener.ClearEvents<IDxOrder>();
                    Assert.AreEqual(0, s.GetSymbols().Count);

                    Thread.Sleep(10000);
                    Assert.AreEqual(0, listener.GetSnapshotsCount<IDxOrder>(symbol));

                    //add another symbol
                    s.AddSymbols(CandleSymbol.ValueOf(candleString));
                    listener.WaitSnapshot<IDxCandle>(candleString);
                }
            }
        }

        [Test]
        public void TestGetSymbols()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string source = "NTV";
            string symbol = "AAPL";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSource(source);
                    s.AddSymbol(symbol);
                    listener.WaitSnapshot<IDxOrder>(symbol, source);

                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(symbol, returnedSymbolList[0]);

                    s.Clear();
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(0, returnedSymbolList.Count);
                }
            }
        }

        [Test]
        public void TestGetSymbolsCandle()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string candleString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSymbol(CandleSymbol.ValueOf(candleString));
                    listener.WaitSnapshot<IDxCandle>(candleString);

                    IList<string> returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(1, returnedSymbolList.Count);
                    Assert.AreEqual(candleString, returnedSymbolList[0]);

                    s.Clear();
                    returnedSymbolList = s.GetSymbols();
                    Assert.AreEqual(0, returnedSymbolList.Count);
                }
            }
        }

        [Test]
        public void TestSetSource()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string initialSource = "NTV";
            string symbol = "AAPL";
            string otherSource = "DEX";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.SetSource((string[])null); });
                    Assert.Throws<InvalidOperationException>(delegate { s.SetSource(new string[] { }); });
                    Assert.Throws<ArgumentException>(delegate { s.SetSource(new string[] { string.Empty }); });
                    Assert.Throws<InvalidOperationException>(delegate { s.SetSource(new string[] { "DEA", "DEX" }); });
                    s.SetSource(initialSource);
                    s.AddSymbol(symbol);
                    listener.WaitSnapshot<IDxOrder>(symbol, initialSource);

                    //try reSet source
                    s.SetSource(otherSource);
                    Thread.Sleep(10000);
                    listener.ClearEvents<IDxOrder>();
                    Thread.Sleep(3000);
                    listener.WaitSnapshot<IDxOrder>(symbol, otherSource);
                    Assert.False(listener.HaveSnapshotEvents<IDxOrder>(symbol, initialSource));
                }
            }
        }

        [Test]
        public void TestSetSource2()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string initialSource = "NTV";
            string symbol = "AAPL";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.SetSource((string[])null); });
                    Assert.Throws<InvalidOperationException>(delegate { s.SetSource(new string[] { }); });
                    Assert.Throws<ArgumentException>(delegate { s.SetSource(new string[] { string.Empty }); });
                    Assert.Throws<InvalidOperationException>(delegate { s.SetSource(new string[] { "DEA", "DEX" }); });

                    s.AddSymbol(symbol);
                    Thread.Sleep(5000);

                    s.SetSource(initialSource);
                    listener.WaitSnapshot<IDxOrder>(symbol, initialSource);
                }
            }
        }

        [Test]
        public void TestAddSource()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string initialSource = "NTV";
            string symbol = "AAPL";
            string otherSource = "DEX";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.SetSource((string[])null); });
                    Assert.Throws<InvalidOperationException>(delegate { s.SetSource(new string[] { }); });
                    Assert.Throws<ArgumentException>(delegate { s.SetSource(new string[] { string.Empty }); });
                    Assert.Throws<InvalidOperationException>(delegate { s.SetSource(new string[] { "DEA", "DEX" }); });
                    s.AddSource(initialSource);
                    s.AddSymbol(symbol);
                    listener.WaitSnapshot<IDxOrder>(symbol, initialSource);

                    //try add source
                    Assert.Throws<InvalidOperationException>(delegate { s.AddSource(otherSource); });
                }
            }
        }

        [Test]
        public void TestAddSource2()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string initialSource = "NTV";
            string symbol = "AAPL";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    Assert.Throws<ArgumentException>(delegate { s.AddSource((string[])null); });
                    Assert.Throws<InvalidOperationException>(delegate { s.AddSource(new string[] { }); });
                    Assert.Throws<ArgumentException>(delegate { s.AddSource(new string[] { string.Empty }); });
                    Assert.Throws<InvalidOperationException>(delegate { s.AddSource(new string[] { "DEA", "DEX" }); });

                    s.AddSymbol(symbol);
                    Thread.Sleep(5000);

                    s.AddSource(initialSource);
                    listener.WaitSnapshot<IDxOrder>(symbol, initialSource);
                }
            }
        }

        [Test]
        public void TestSetSourceCandle()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string initialSource = "NTV";
            string candleString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.SetSource(initialSource);
                    s.AddSymbol(CandleSymbol.ValueOf(candleString));
                    listener.WaitSnapshot<IDxCandle>(candleString);
                    s.SetSource(initialSource);
                }
            }
        }

        [Test]
        public void TestSetSourceCandle2()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string initialSource = "NTV";
            string candleString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSymbol(CandleSymbol.ValueOf(candleString));
                    s.SetSource(initialSource);
                    listener.WaitSnapshot<IDxCandle>(candleString);
                    s.SetSource(initialSource);
                }
            }
        }

        [Test]
        public void TestAddSourceCandle()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string initialSource = "NTV";
            string candleString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSource(initialSource);
                    s.AddSymbol(CandleSymbol.ValueOf(candleString));
                    listener.WaitSnapshot<IDxCandle>(candleString);
                    s.AddSource(initialSource);
                }
            }
        }

        [Test]
        public void TestAddSourceCandle2()
        {
            SnapshotTestListener listener = new SnapshotTestListener(eventsTimeout, eventsSleepTime, IsConnected);
            string initialSource = "NTV";
            string candleString = "XBT/USD{=d}";
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                Interlocked.Exchange(ref isConnected, 1);
                using (IDxSubscription s = con.CreateSnapshotSubscription(0, listener))
                {
                    s.AddSymbol(CandleSymbol.ValueOf(candleString));
                    s.AddSource(initialSource);
                    listener.WaitSnapshot<IDxCandle>(candleString);
                    s.AddSource(initialSource);
                }
            }
        }
    }
}
