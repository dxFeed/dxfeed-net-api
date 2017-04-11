using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.dxfeed.api
{
    [TestFixture]
    public class DXFeedSubscriptionTest
    {

        //TODO: multithreaded test

        [Test]
        public void IsClosedTest()
        {
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            Assert.False(s.IsClosed);
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.False(s.IsClosed);
            });

            s.Close();
            Assert.True(s.IsClosed);
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.True(s.IsClosed);
            });
        }

        [Test]
        public void CloseTest()
        {
            isFiresOnSubscriptionClosed = false;
            isFiresOnSymbolsRemoved = false;
            var expectedEventSet = new HashSet<Type>(new Type[] { typeof(IDxOrder) });
            var expectedSymbolSet = new HashSet<string>();
            var eventListener = new EventListener();

            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxEventType>(typeof(IDxOrder));
            s.OnSubscriptionClosed += OnSubscriptionClosed;
            s.OnSymbolsAdded += OnSymbolsAdded;
            s.OnSymbolsRemoved += OnSymbolsRemoved;
            s.AddSymbols("IBM");
            s.Close();

            //try to call methods after closing subscription
            Assert.True(isFiresOnSubscriptionClosed);
            Assert.True(s.IsClosed);
            Assert.True(s.EventTypes.SetEquals(expectedEventSet));
            Assert.True(s.ContainsEventType(typeof(IDxOrder)));

            s.Clear();
            Assert.True(s.GetSymbols().SetEquals(expectedSymbolSet));

            isFiresOnSymbolsAdded = false;
            s.SetSymbols("AAPL");
            Assert.False(isFiresOnSymbolsAdded);
            Assert.True(s.GetSymbols().SetEquals(expectedSymbolSet));

            s.AddSymbols("C");
            Assert.False(isFiresOnSymbolsAdded);
            Assert.True(s.GetSymbols().SetEquals(expectedSymbolSet));

            s.RemoveSymbols("IBM");
            Assert.False(isFiresOnSymbolsRemoved);
            Assert.True(s.GetSymbols().SetEquals(expectedSymbolSet));

            s.AddEventListener(eventListener);
            s.RemoveEventListener(eventListener);

            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                s.Close();
            });
        }

        [Test]
        public void EventTypesTest()
        {
            var orderSubscription = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            Assert.True(orderSubscription.EventTypes.SetEquals(new HashSet<Type>(new Type[] { typeof(IDxOrder) })));

            ISet<Type> allTypesSet = DXEndpoint.GetEventTypes();
            Type[] subscriptionTypes = allTypesSet.AsEnumerable().ToArray();
            var allTypeSubscription = DXEndpoint.GetInstance().Feed.CreateSubscription<IDxEventType>(subscriptionTypes);
            Assert.True(allTypeSubscription.EventTypes.SetEquals(allTypesSet));

            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.True(allTypeSubscription.EventTypes.SetEquals(allTypesSet));
            });
        }

        [Test]
        public void ContainEventTypeTest()
        {
            ISet<Type> allTypesSet = DXEndpoint.GetEventTypes();

            var orderSubscription = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            Assert.True(orderSubscription.ContainsEventType(typeof(IDxOrder)));
            foreach (Type t in allTypesSet.Except(new Type[] { typeof(IDxOrder) }))
                Assert.False(orderSubscription.ContainsEventType(t));

            Type[] subscriptionTypes = new Type[] { typeof(IDxOrder), typeof(IDxCandle) };
            var orderCandleSubscription = DXEndpoint.GetInstance().Feed.CreateSubscription<IDxEventType>(subscriptionTypes);
            foreach (Type t in subscriptionTypes)
                Assert.True(orderCandleSubscription.ContainsEventType(t));
            foreach (Type t in allTypesSet.Except(subscriptionTypes))
                Assert.False(orderCandleSubscription.ContainsEventType(t));

            subscriptionTypes = allTypesSet.AsEnumerable().ToArray();
            var allTypeSubscription = DXEndpoint.GetInstance().Feed.CreateSubscription<IDxEventType>(subscriptionTypes);
            foreach (Type t in allTypesSet)
                Assert.True(allTypeSubscription.ContainsEventType(t));

            Assert.False(allTypeSubscription.ContainsEventType(null));

            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                foreach (Type t in allTypesSet)
                    Assert.True(allTypeSubscription.ContainsEventType(t));
            });
        }

        [Test]
        public void ClearTest()
        {
            object[] symbols = new object[] { "IBM", "AAPL", "C" };
            isFiresOnSymbolsRemoved = false;
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            s.OnSymbolsRemoved += OnSymbolsRemoved;
            s.AddSymbols(symbols);
            Assert.Greater(s.GetSymbols().Count, 0);

            s.Clear();
            Assert.True(isFiresOnSymbolsRemoved);
            Assert.AreEqual(s.GetSymbols().Count, 0);

            //try to add symbols again after Clear
            s.AddSymbols(symbols);
            Assert.Greater(s.GetSymbols().Count, 0);

            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                s.Clear();
            });
        }

        [Test]
        public void GetSymbolsTest()
        {
            object[] symbols = new object[] { "IBM", "AAPL", "C" };
            isFiresOnSymbolsRemoved = false;
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            s.AddSymbols(symbols);
            Assert.AreEqual(symbols.Length, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(symbols));

            //try to change returned symbols set
            s.GetSymbols().Add("MSFT");
            Assert.AreEqual(symbols.Length, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(symbols));

            //try to add new symbol into subscription
            s.AddSymbols("XBT/USD");
            IList<object> newSymbols = new List<object>(symbols);
            newSymbols.Add("XBT/USD");
            Assert.AreEqual(newSymbols.Count, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(newSymbols));

            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.True(s.GetSymbols().SetEquals(newSymbols));
            });
        }

        [Test]
        public void SetSymbolsTest()
        {
            object[] symbolsSet1 = new object[] { "IBM", CandleSymbol.ValueOf("AAPL{=d}"), "C" };
            object[] symbolsSet1String = new object[] { "IBM", "AAPL{=d}", "C" };
            object[] symbolsSet2 = new object[] { "XBT/USD", "MSFT" };
            isFiresOnSymbolsAdded = false;
            isFiresOnSymbolsRemoved = false;
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            s.OnSymbolsAdded += OnSymbolsAdded;
            s.OnSymbolsRemoved += OnSymbolsRemoved;

            s.SetSymbols(null);
            s.SetSymbols(new List<object>());
            s.SetSymbols(new object[] { });
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                s.SetSymbols(new List<object>(new object[] { null }));
            });

            s.SetSymbols(symbolsSet1);
            Assert.True(isFiresOnSymbolsAdded);
            Assert.True(isFiresOnSymbolsRemoved);
            Assert.AreEqual(symbolsSet1String.Length, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(symbolsSet1String));

            isFiresOnSymbolsAdded = false;
            isFiresOnSymbolsRemoved = false;
            s.SetSymbols(symbolsSet2);
            Assert.True(isFiresOnSymbolsAdded);
            Assert.True(isFiresOnSymbolsRemoved);
            Assert.AreEqual(symbolsSet2.Length, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(symbolsSet2));

            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                s.SetSymbols(SimulatedSymbolsSet[i % SimulatedSymbolsSet.Length]);
            });
        }

        [Test]
        public void AddSymbolsTest()
        {
            object[] symbolsSet1 = new object[] { "IBM", CandleSymbol.ValueOf("AAPL{=d}"), "C" };
            object[] symbolsSetString = new object[] { "IBM", "AAPL{=d}", "C" };
            object symbolSet2 = "XBT/USD";
            isFiresOnSymbolsAdded = false;
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            s.OnSymbolsAdded += OnSymbolsAdded;

            s.AddSymbols(null);
            s.AddSymbols(new List<object>());
            s.AddSymbols(new object[] { });
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                s.AddSymbols(new List<object>(new object[] { null }));
            });

            s.AddSymbols(symbolsSet1);
            Assert.True(isFiresOnSymbolsAdded);
            Assert.AreEqual(symbolsSetString.Length, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(symbolsSetString));

            isFiresOnSymbolsAdded = false;
            object[] newSymbolSet = symbolsSetString.Concat(new object[] { symbolSet2 }).ToArray();
            s.AddSymbols(symbolSet2);
            Assert.True(isFiresOnSymbolsAdded);
            Assert.AreEqual(newSymbolSet.Length, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(newSymbolSet));

            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                s.AddSymbols(SimulatedSymbolsSet[i % SimulatedSymbolsSet.Length]);
            });
            foreach (var set in SimulatedSymbolsSet)
                newSymbolSet = newSymbolSet.Concat(set).ToArray();
            Assert.AreEqual(newSymbolSet.Length, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(newSymbolSet));
        }

        [Test]
        public void RemoveSymbolsTest()
        {
            object[] symbolsSet = new object[] { "IBM", CandleSymbol.ValueOf("AAPL{=d}"), "C", "XBT/USD" };
            isFiresOnSymbolsRemoved = false;
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            s.OnSymbolsRemoved += OnSymbolsRemoved;
            s.AddSymbols(symbolsSet);

            s.RemoveSymbols(null);
            s.RemoveSymbols(new List<object>());
            s.RemoveSymbols(new object[] { });
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                s.RemoveSymbols(new List<object>(new object[] { null }));
            });

            ICollection<object> removeCollection = new List<object>(
                new object[] { CandleSymbol.ValueOf("AAPL{=d}"), "C", "XBT/USD" }
                );
            s.RemoveSymbols(removeCollection);
            Assert.True(isFiresOnSymbolsRemoved);
            Assert.AreEqual(1, s.GetSymbols().Count);
            Assert.True(s.GetSymbols().SetEquals(new object[] { "IBM" }));
            isFiresOnSymbolsRemoved = false;
            s.RemoveSymbols("IBM");
            Assert.True(isFiresOnSymbolsRemoved);
            Assert.AreEqual(0, s.GetSymbols().Count);

            var totalSymbolsCount = 0;
            foreach (var set in SimulatedSymbolsSet)
            {
                s.AddSymbols(set);
                totalSymbolsCount += set.Length;
            }
            Assert.AreEqual(totalSymbolsCount, s.GetSymbols().Count);
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                s.RemoveSymbols(SimulatedSymbolsSet[i % SimulatedSymbolsSet.Length]);
            });
            Assert.AreEqual(0, s.GetSymbols().Count);
        }

        [Test]
        public void AddEventListenerTest()
        {
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxEventType>(typeof(IDxOrder));

            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                s.AddEventListener(null);
            });

            var listenersCount = 10;
            var listenersList = new List<IDXFeedEventListener<IDxEventType>>(listenersCount);
            for (var i = 0; i < listenersCount; i++)
                listenersList.Add(new EventListener());
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                s.AddEventListener(listenersList[i % listenersCount]);
            });
        }

        [Test]
        public void RemoveEventListenerTest()
        {
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxEventType>(typeof(IDxOrder));

            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                s.RemoveEventListener(null);
            });

            var listenersCount = 10;
            var listenersList = new List<IDXFeedEventListener<IDxEventType>>(listenersCount);
            for (var i = 0; i < listenersCount; i++)
            {
                listenersList.Add(new EventListener());

                s.AddEventListener(listenersList.Last());
            }
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                s.RemoveEventListener(listenersList[i % listenersCount]);
            });
        }

        #region Private fields and methods

        private const int ParallelFrom = 0;
        private const int ParallelTo = 101;

        private bool isFiresOnSubscriptionClosed = false;
        private bool isFiresOnSymbolsAdded = false;
        private bool isFiresOnSymbolsRemoved = false;
        private ISet<object> updatedSymbols = null;

        private readonly string[][] SimulatedSymbolsSet = new string[][] {
            new string[] { "SYMA", "SYMB", "SYMC" },
            new string[] { "SYMD", "SYME", "SYMF" },
            new string[] { "SYMG", "SYMH", "SYMI" },
            new string[] { "SYMJ", "SYMK", "SYML" },
            new string[] { "SYMM", "SYMN", "SYMO" }
        };

        private void OnSubscriptionClosed(object sender, EventArgs args)
        {
            isFiresOnSubscriptionClosed = true;
        }

        private void OnSymbolsRemoved(object sender, DXFeedSymbolsUpdateEventArgs args)
        {
            isFiresOnSymbolsRemoved = true;
            updatedSymbols = args.Symbols;
        }

        private void OnSymbolsAdded(object sender, DXFeedSymbolsUpdateEventArgs args)
        {
            isFiresOnSymbolsAdded = true;
            updatedSymbols = args.Symbols;
        }

        private class EventListener : IDXFeedEventListener<IDxEventType>
        {
            public void EventsReceived(IList<IDxEventType> events) { }
        }

        #endregion
    }
}
