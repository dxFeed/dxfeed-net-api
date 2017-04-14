using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.tests.tools;
using com.dxfeed.tests.tools.eventplayer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace com.dxfeed.api
{
    [TestFixture]
    public class DXFeedTest
    {
        [Test]
        public void CreateSubscriptionTest()
        {
            var symbol = "SYMA";
            var s = DXEndpoint.Create().Feed.CreateSubscription<IDxOrder>();
            s.AddSymbols(symbol);
            TestListener eventListener = new TestListener();
            s.AddEventListener(eventListener);

            var playedOrder = new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000006, 3, Side.Buy, 0, Scope.ORDER, 0, 100, OrderSource.NTV, 0, 0, "AAAA");

            EventPlayer<IDxOrder> eventPlayer = new EventPlayer<IDxOrder>(s as DXFeedSubscription<IDxOrder>);
            eventPlayer.PlayEvents(symbol, playedOrder);
            Assert.AreEqual(eventListener.GetEventCount<IDxOrder>(symbol), 1);

            IDxOrder receivedOrder = eventListener.GetLastEvent<IDxOrder>().Event;
            Assert.AreEqual(symbol, receivedOrder.EventSymbol);
            CompareOrders(playedOrder, receivedOrder);

            //try to create subscription on closed endpoint
            DXEndpoint.GetInstance().Close();
            Assert.Catch(typeof(InvalidOperationException), () =>
            {
                DXFeed.GetInstance().CreateSubscription<IDxOrder>();
            });
        }

        [Test]
        public void CreateSubscriptionTypesTest()
        {
            //create default endpoint
            DXEndpoint.Create();

            //try to create subscription with invalid event types parameters
            //all attempts to create subscription in this block must be failed with exception
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateSubscription<IDxOrder>(typeof(IDxCandle), typeof(IDxQuote));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateSubscription<IndexedEvent>(typeof(IDxCandle), typeof(IDxQuote));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateSubscription<LastingEvent>(typeof(IDxOrder), typeof(IDxQuote));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateSubscription<IDxMarketEvent>(typeof(IDxCandle));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateSubscription<IDxMarketEvent>(typeof(string));
            });

            var symbol = "SYMA";
            var s = DXFeed.GetInstance().CreateSubscription<IDxEventType>(typeof(IDxOrder), typeof(IDxTrade));
            s.AddSymbols(symbol);
            TestListener eventListener = new TestListener();
            s.AddEventListener(eventListener);

            EventPlayer<IDxEventType> eventPlayer = new EventPlayer<IDxEventType>(s as DXFeedSubscription<IDxEventType>);
            var playedOrder = new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000006, 3, Side.Buy, 0, Scope.ORDER, 0, 100, OrderSource.NTV, 0, 0, "AAAA");
            eventPlayer.PlayEvents(symbol, playedOrder);
            var playedTrade = new PlayedTrade(symbol, 0, 'B', 123.456, 100, 123, 1.1, 2.2);
            eventPlayer.PlayEvents(symbol, playedTrade);

            Assert.AreEqual(eventListener.GetEventCount<IDxOrder>(symbol), 1);
            Assert.AreEqual(eventListener.GetEventCount<IDxTrade>(symbol), 1);
        }

        [Test]
        public void CreateTimeSeriesSubscriptionTest()
        {
            var symbol = "SYMA";
            var s = DXEndpoint.Create().Feed.CreateTimeSeriesSubscription<IDxCandle>();
            s.AddSymbols(symbol);
            TestListener eventListener = new TestListener();
            s.AddEventListener(eventListener);

            var playedCandle = new PlayedCandle(symbol, Tools.DateToUnixTime(DateTime.Now), 123, 100, 12.34, 56.78, 9.0, 43.21, 1000, 999, 1001, 1002, 1, 777, 888, EventFlag.RemoveSymbol);

            EventPlayer<IDxCandle> eventPlayer = new EventPlayer<IDxCandle>(s as DXFeedSubscription<IDxCandle>);
            eventPlayer.PlayEvents(symbol, playedCandle);
            Assert.AreEqual(eventListener.GetEventCount<IDxCandle>(symbol), 1);
            IDxCandle receivedCandle = eventListener.GetLastEvent<IDxCandle>().Event;
            Assert.AreEqual(symbol, receivedCandle.EventSymbol.ToString());
            CompareCandles(playedCandle, receivedCandle);
        }

        [Test]
        public void CreateTimeSeriesSubscriptionTypesTest()
        {
            //create default endpoint
            DXEndpoint.Create();

            //try to create subscription with invalid event types parameters
            //all attempts to create subscription in this block must be failed with exception
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateTimeSeriesSubscription<TimeSeriesEvent>(typeof(IDxCandle), typeof(IDxQuote));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateTimeSeriesSubscription<TimeSeriesEvent>(typeof(IDxOrder), typeof(IDxQuote));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateTimeSeriesSubscription<TimeSeriesEvent>(typeof(string));
            });

            var symbol = "SYMA";
            var s = DXFeed.GetInstance().CreateTimeSeriesSubscription<TimeSeriesEvent>(typeof(IDxCandle), typeof(IDxGreeks));
            s.AddSymbols(symbol);
            TestListener eventListener = new TestListener();
            s.AddEventListener(eventListener);

            EventPlayer<TimeSeriesEvent> eventPlayer = new EventPlayer<TimeSeriesEvent>(s as DXFeedSubscription<TimeSeriesEvent>);
            var playedCandle = new PlayedCandle(symbol, Tools.DateToUnixTime(DateTime.Now), 123, 100, 12.34, 56.78, 9.0, 43.21, 1000, 999, 1001, 1002, 1, 777, 888, EventFlag.RemoveSymbol);
            eventPlayer.PlayEvents(symbol, playedCandle);
            var playedGreeks = new PlayedGreeks(symbol, Tools.DateToUnixTime(DateTime.Now), 123, 456.789, 11, 555, 666, 777, 888, 999, 1, EventFlag.RemoveSymbol);
            eventPlayer.PlayEvents(symbol, playedGreeks);

            Assert.AreEqual(eventListener.GetEventCount<IDxCandle>(symbol), 1);
            Assert.AreEqual(eventListener.GetEventCount<IDxGreeks>(symbol), 1);
        }

        [Test]
        public void AttachSubscriptionTest()
        {
            //create default endpoint
            DXEndpoint.Create();

            //try to attach null subscription
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                DXFeed.GetInstance().AttachSubscription<IDxOrder>(null);
            });

            //try to attach already attached subscription
            var s = DXFeed.GetInstance().CreateSubscription<IDxOrder>();
            DXFeed.GetInstance().AttachSubscription(s);

            //try to attach another not attached subscription
            DXFeedSubscription<IDxOrder> other = new DXFeedSubscription<IDxOrder>(DXEndpoint.GetInstance() as DXEndpoint);
            DXFeed.GetInstance().AttachSubscription(other);

            //try to reattach another subscription
            DXFeed.GetInstance().DetachSubscription(other);
            DXFeed.GetInstance().AttachSubscription(other);
        }

        [Test]
        public void DetachSubscriptionTest()
        {
            //create default endpoint
            DXEndpoint.Create();

            //try to detach null subscription
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                DXFeed.GetInstance().DetachSubscription<IDxOrder>(null);
            });

            //try to detach already detached subscription
            var s = DXFeed.GetInstance().CreateSubscription<IDxOrder>();
            DXFeed.GetInstance().DetachSubscription(s);
            DXFeed.GetInstance().DetachSubscription(s);

            //try to detach another not attached subscription
            DXFeedSubscription<IDxOrder> other = new DXFeedSubscription<IDxOrder>(DXEndpoint.GetInstance() as DXEndpoint);
            DXFeed.GetInstance().DetachSubscription(other);
        }

        [Test]
        public void GetLastEventPromiseTest()
        {
            //create default endpoint
            DXEndpoint.Create();
            var symbol = "SYMA";

            //try to create promise with invalid parameters
            Assert.Catch(typeof(ArgumentException), () =>
            {
                try
                {
                    DXFeed.GetInstance().GetLastEventPromise<IDxTrade>(null, CancellationToken.None).Wait();
                }
                catch (AggregateException ae)
                {
                    foreach (var inner in ae.InnerExceptions)
                        throw inner;
                }
            });

            //try to cancel promise
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            Task<LastingEvent> promise = DXFeed.GetInstance().GetLastEventPromise<IDxTrade>(symbol, cancelSource.Token);
            cancelSource.CancelAfter(TimeSpan.FromSeconds(2));
            try
            {
                Task.WaitAll(promise);
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try wait promise with timeout
            cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            promise = DXFeed.GetInstance().GetLastEventPromise<IDxTrade>(symbol, cancelSource.Token);
            try
            {
                Task.WaitAll(promise);
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try close endpoint while promise waiting
            Task closeEndpointTask = Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                DXEndpoint.GetInstance().Close();
            });
            promise = DXFeed.GetInstance().GetLastEventPromise<IDxTrade>(symbol, CancellationToken.None);
            try
            {
                Task.WaitAll(promise, closeEndpointTask);
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try to get promise on closed endpoint
            promise = DXFeed.GetInstance().GetLastEventPromise<IDxTrade>(symbol, CancellationToken.None);
            try
            {
                Task.WaitAll(promise);
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try to get last event succesfully
            var playedTrade = new PlayedTrade(symbol, Tools.DateToUnixTime(DateTime.Now), 'B', 123.456, 100, 123, 1.1, 2.2);
            Task eventPlayerTask = Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                EventPlayer<IDxTrade> eventPlayer = new EventPlayer<IDxTrade>(GetSubscriptionFromFeed<IDxTrade>(symbol));
                eventPlayer.PlayEvents(symbol, playedTrade);
            });
            cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            promise = DXEndpoint.Create().Feed.GetLastEventPromise<IDxTrade>(symbol, cancelSource.Token);
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(promise, eventPlayerTask);
            });

            IDxTrade lastTrade = promise.Result as IDxTrade;
            Assert.AreEqual(symbol, lastTrade.EventSymbol);
            CompareTrades(playedTrade, lastTrade);
        }

        [Test]
        public void GetLastEventsPromisesTest()
        {
            //create default endpoint
            DXEndpoint.Create();
            var symbols = new string[] { "SYMA", "SYMB" };

            //try to create promise with invalid parameters
            Assert.Catch(typeof(ArgumentException), () =>
            {
                try
                {
                    Task.WaitAll(DXFeed.GetInstance().GetLastEventsPromises<IDxTrade>(null, CancellationToken.None).ToArray());
                }
                catch (AggregateException ae)
                {
                    foreach (var inner in ae.InnerExceptions)
                        throw inner;
                }
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                try
                {
                    Task.WaitAll(DXFeed.GetInstance().GetLastEventsPromises<IDxTrade>(new object[] { null }, CancellationToken.None).ToArray());
                }
                catch (AggregateException ae)
                {
                    foreach (var inner in ae.InnerExceptions)
                        throw inner;
                }
            });

            //try to cancel promise
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            List<Task<LastingEvent>> promises = DXFeed.GetInstance().GetLastEventsPromises<IDxTrade>(symbols, cancelSource.Token);
            cancelSource.CancelAfter(TimeSpan.FromSeconds(2));
            try
            {
                Task.WaitAll(promises.ToArray());
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }


            //try wait promise with timeout
            cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            promises = DXFeed.GetInstance().GetLastEventsPromises<IDxTrade>(symbols, cancelSource.Token);
            try
            {
                Task.WaitAll(promises.ToArray());
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try close endpoint while promise waiting
            Task closeEndpointTask = Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                DXEndpoint.GetInstance().Close();
            });
            promises = DXFeed.GetInstance().GetLastEventsPromises<IDxTrade>(symbols, CancellationToken.None);
            List<Task> allTasks = new List<Task>();
            foreach (var p in promises)
                allTasks.Add(p);
            allTasks.Add(closeEndpointTask);
            try
            {
                Task.WaitAll(allTasks.ToArray());
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try to get promise on closed endpoint
            promises = DXFeed.GetInstance().GetLastEventsPromises<IDxTrade>(symbols, CancellationToken.None);
            try
            {
                Task.WaitAll(promises.ToArray());
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try to get last event succesfully
            var playedTrades = new PlayedTrade[] {
                new PlayedTrade(symbols[0], Tools.DateToUnixTime(DateTime.Now), 'B', 123.456, 100, 123, 1.1, 2.2),
                new PlayedTrade(symbols[1], Tools.DateToUnixTime(DateTime.Now), 'B', 234.567, 101, 234, 3.2, 4.3)
            };
            Task eventPlayerTask = Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                foreach (var t in playedTrades)
                {
                    EventPlayer<IDxTrade> eventPlayer = new EventPlayer<IDxTrade>(GetSubscriptionFromFeed<IDxTrade>(t.EventSymbol));
                    eventPlayer.PlayEvents(t.EventSymbol, t);
                }
            });
            cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            promises = DXEndpoint.Create().Feed.GetLastEventsPromises<IDxTrade>(symbols, cancelSource.Token);
            allTasks = new List<Task>();
            foreach (var p in promises)
                allTasks.Add(p);
            allTasks.Add(eventPlayerTask);
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(allTasks.ToArray());
            });

            Dictionary<string, PlayedTrade> playedTradeDictionary = new Dictionary<string, PlayedTrade>();
            foreach (var t in playedTrades)
                playedTradeDictionary[t.EventSymbol] = t;
            foreach (var p in promises)
            {
                IDxTrade lastTrade = p.Result as IDxTrade;
                Assert.True(playedTradeDictionary.ContainsKey(lastTrade.EventSymbol));
                var playedTrade = playedTradeDictionary[lastTrade.EventSymbol];
                playedTradeDictionary.Remove(lastTrade.EventSymbol);
                CompareTrades(playedTrade, lastTrade);
            }
        }

        [Test]
        public void GetIndexedEventsPromiseTest()
        {
            //create default endpoint
            DXEndpoint.Create();
            var symbol = "SYMA";

            //try to create promise with invalid parameters
            Assert.Catch(typeof(ArgumentException), () =>
            {
                try
                {
                    DXFeed.GetInstance().GetIndexedEventsPromise<IDxOrder>(null, OrderSource.NTV, CancellationToken.None).Wait();
                }
                catch (AggregateException ae)
                {
                    foreach (var inner in ae.InnerExceptions)
                        throw inner;
                }
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                try
                {
                    DXFeed.GetInstance().GetIndexedEventsPromise<IDxOrder>(symbol, null, CancellationToken.None).Wait();
                }
                catch (AggregateException ae)
                {
                    foreach (var inner in ae.InnerExceptions)
                        throw inner;
                }
            });

            //try to cancel promise
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            Task<List<IDxOrder>> promise = DXFeed.GetInstance().GetIndexedEventsPromise<IDxOrder>(symbol, OrderSource.NTV, cancelSource.Token);
            cancelSource.CancelAfter(TimeSpan.FromSeconds(2));
            try
            {
                Task.WaitAll(promise);
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try wait promise with timeout
            cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            promise = DXFeed.GetInstance().GetIndexedEventsPromise<IDxOrder>(symbol, OrderSource.NTV, cancelSource.Token);
            try
            {
                Task.WaitAll(promise);
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try close endpoint while promise waiting
            Task closeEndpointTask = Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                DXEndpoint.GetInstance().Close();
            });
            promise = DXFeed.GetInstance().GetIndexedEventsPromise<IDxOrder>(symbol, OrderSource.NTV, CancellationToken.None);
            try
            {
                Task.WaitAll(promise, closeEndpointTask);
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try to get promise on closed endpoint
            promise = DXFeed.GetInstance().GetIndexedEventsPromise<IDxOrder>(symbol, OrderSource.NTV, CancellationToken.None);
            try
            {
                Task.WaitAll(promise);
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (!(inner is OperationCanceledException))
                        Assert.Fail("Unexpected exception: " + inner);
                }
            }

            //try to get last event succesfully
            DateTime date = DateTime.Now;
            var playedOrders = new PlayedOrder[] {
                new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000006, 3, Side.Buy, 100, Scope.ORDER, 0, 100, OrderSource.NTV, Tools.DateToUnixTime(date), 0, "AAAA"),
                new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000005, 3, Side.Buy, 100.5, Scope.ORDER, 0, 101, OrderSource.NTV, Tools.DateToUnixTime(date.AddMinutes(-1)), 0, "AAAA"),
                new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000004, 3, Side.Sell, 101, Scope.ORDER, 0, 102, OrderSource.NTV, Tools.DateToUnixTime(date.AddMinutes(-2)), 0, "AAAA"),
                new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000003, 3, Side.Buy, 100, Scope.ORDER, 0, 103, OrderSource.NTV, Tools.DateToUnixTime(date.AddMinutes(-3)), 0, "AAAA"),
                new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000002, 3, Side.Buy, 100.4, Scope.ORDER, 0, 104, OrderSource.NTV, Tools.DateToUnixTime(date.AddMinutes(-4)), 0, "AAAA"),
                new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000001, 3, Side.Buy, 100.3, Scope.ORDER, 0, 105, OrderSource.NTV, Tools.DateToUnixTime(date.AddMinutes(-5)), 0, "AAAA"),
                new PlayedOrder(symbol, 25, 0, 'A', 0x4e54560000000000, 3, Side.Buy, 100.2, Scope.ORDER, 0, 106, OrderSource.NTV, Tools.DateToUnixTime(date.AddMinutes(-6)), 0, "AAAA")
            };
            Task eventPlayerTask = Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                EventPlayer<IDxOrder> eventPlayer = new EventPlayer<IDxOrder>(GetSubscriptionFromFeed<IDxOrder>(symbol));
                eventPlayer.PlaySnapshot(symbol, playedOrders);
            });
            cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            promise = DXEndpoint.Create().Feed.GetIndexedEventsPromise<IDxOrder>(symbol, OrderSource.NTV, cancelSource.Token);
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(promise, eventPlayerTask);
            });

            var receivedOrders = promise.Result;
            receivedOrders.Reverse();
            Assert.AreEqual(playedOrders.Length, receivedOrders.Count);
            for (int i = 0; i < playedOrders.Length; i++)
            {
                Assert.AreEqual(symbol, receivedOrders[i].EventSymbol);
                CompareOrders(playedOrders[i], receivedOrders[i]);
            }
        }

        [Test]
        public void GetTimeSeriesPromiseTest()
        {
            //create default endpoint
            DXEndpoint.Create();
            var symbol = "SYMAB";

            ////try to create promise with invalid parameters
            //Assert.Catch(typeof(ArgumentException), () =>
            //{
            //    try
            //    {
            //        DXFeed.GetInstance().GetTimeSeriesPromise<IDxCandle>(null, 0, 0, CancellationToken.None).Wait();
            //    }
            //    catch (AggregateException ae)
            //    {
            //        foreach (var inner in ae.InnerExceptions)
            //            throw inner;
            //    }
            //});

            ////try to cancel promise
            //CancellationTokenSource cancelSource = new CancellationTokenSource();
            //Task<List<IDxCandle>> promise = DXFeed.GetInstance().GetTimeSeriesPromise<IDxCandle>(symbol, 0, 0, cancelSource.Token);
            //cancelSource.CancelAfter(TimeSpan.FromSeconds(2));
            //try
            //{
            //    Task.WaitAll(promise);
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var inner in ae.InnerExceptions)
            //    {
            //        if (!(inner is OperationCanceledException))
            //            Assert.Fail("Unexpected exception: " + inner);
            //    }
            //}

            ////try wait promise with timeout
            //cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            //promise = DXFeed.GetInstance().GetTimeSeriesPromise<IDxCandle>(symbol, 0, 0, cancelSource.Token);
            //try
            //{
            //    Task.WaitAll(promise);
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var inner in ae.InnerExceptions)
            //    {
            //        if (!(inner is OperationCanceledException))
            //            Assert.Fail("Unexpected exception: " + inner);
            //    }
            //}

            ////try close endpoint while promise waiting
            //Task closeEndpointTask = Task.Run(() =>
            //{
            //    Thread.Sleep(TimeSpan.FromSeconds(2));
            //    DXEndpoint.GetInstance().Close();
            //});
            //promise = DXFeed.GetInstance().GetTimeSeriesPromise<IDxCandle>(symbol, 0, 0, CancellationToken.None);
            //try
            //{
            //    Task.WaitAll(promise, closeEndpointTask);
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var inner in ae.InnerExceptions)
            //    {
            //        if (!(inner is OperationCanceledException))
            //            Assert.Fail("Unexpected exception: " + inner);
            //    }
            //}

            ////try to get promise on closed endpoint
            //promise = DXFeed.GetInstance().GetTimeSeriesPromise<IDxCandle>(symbol, 0, 0, CancellationToken.None);
            //try
            //{
            //    Task.WaitAll(promise);
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var inner in ae.InnerExceptions)
            //    {
            //        if (!(inner is OperationCanceledException))
            //            Assert.Fail("Unexpected exception: " + inner);
            //    }
            //}

            //try to get last event succesfully
            DateTime date = DateTime.Now;
            var playedCandles = new PlayedCandle[] {
                new PlayedCandle(symbol, Tools.DateToUnixTime(date),                12, 100, 112.34, 156.78, 19.0, 143.21, 1000, 199, 10011, 10021, 5, 777, 888, 0),
                new PlayedCandle(symbol, Tools.DateToUnixTime(date.AddMinutes(-1)), 34, 101, 212.34, 256.78, 19.0, 143.21, 1001, 299, 10012, 10022, 4, 777, 888, 0),
                new PlayedCandle(symbol, Tools.DateToUnixTime(date.AddMinutes(-2)), 56, 102, 312.34, 356.78, 19.0, 143.21, 1002, 399, 10013, 10023, 3, 777, 888, 0),
                new PlayedCandle(symbol, Tools.DateToUnixTime(date.AddMinutes(-3)), 78, 103, 412.34, 456.78, 19.0, 143.21, 1003, 499, 10014, 10024, 2, 777, 888, 0),
                new PlayedCandle(symbol, Tools.DateToUnixTime(date.AddMinutes(-4)), 90, 104, 512.34, 556.78, 19.0, 143.21, 1004, 599, 10015, 10025, 1, 777, 888, 0)
            };
            var expectedCandles = new PlayedCandle[] {
                new PlayedCandle(symbol, Tools.DateToUnixTime(date.AddMinutes(-1)), 34, 101, 212.34, 256.78, 19.0, 143.21, 1001, 299, 10012, 10022, 4, 777, 888, 0),
                new PlayedCandle(symbol, Tools.DateToUnixTime(date.AddMinutes(-2)), 56, 102, 312.34, 356.78, 19.0, 143.21, 1002, 399, 10013, 10023, 3, 777, 888, 0),
                new PlayedCandle(symbol, Tools.DateToUnixTime(date.AddMinutes(-3)), 78, 103, 412.34, 456.78, 19.0, 143.21, 1003, 499, 10014, 10024, 2, 777, 888, 0)
            };
            Task eventPlayerTask = Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                EventPlayer<IDxCandle> eventPlayer = new EventPlayer<IDxCandle>(GetSubscriptionFromFeed<IDxCandle>(symbol));
                eventPlayer.PlaySnapshot(symbol, playedCandles);
            });
            //TODO: temp
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            //cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            Task<List<IDxCandle>> promise = DXEndpoint.Create().Feed.GetTimeSeriesPromise<IDxCandle>(symbol, Tools.DateToUnixTime(date.AddMinutes(-3)), Tools.DateToUnixTime(date.AddMinutes(-1)), cancelSource.Token);
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(promise, eventPlayerTask);
            });

            var receivedCandles = promise.Result;
            receivedCandles.Reverse();
            Assert.AreEqual(expectedCandles.Length, receivedCandles.Count);
            for (int i = 0; i < expectedCandles.Length; i++)
            {
                Assert.AreEqual(symbol, receivedCandles[i].EventSymbol.ToString());
                CompareCandles(expectedCandles[i], receivedCandles[i]);
            }
        }

        #region private methods

        /// <summary>
        ///     Tries to get <see cref="DXFeedSubscription{E}"/> instance from current 
        ///     <see cref="DXFeed"/>. This method uses reflection to get access to private 
        ///     hashset of attached subscriptions. This methods returns first finded subscription 
        ///     of type E that contains <paramref name="symbol"/>.
        /// </summary>
        /// <typeparam name="E">Event type.</typeparam>
        /// <typeparam name="symbol">Event type.</typeparam>
        /// <returns>
        ///     First finded subscription of type E that contains <paramref name="symbol"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Cannot access to private fileds or such subscription is not exist.
        /// </exception>
        private DXFeedSubscription<E> GetSubscriptionFromFeed<E>(string symbol)
            where E : class, IDxEventType
        {
            FieldInfo attachedSubscriptionsInfo = typeof(DXFeed).GetField("attachedSubscriptions", BindingFlags.NonPublic | BindingFlags.Instance);
            if (attachedSubscriptionsInfo == null)
                throw new InvalidOperationException("attachedSubscriptions field not found!");
            HashSet<object> attachedSubscriptionsSet = attachedSubscriptionsInfo.GetValue(DXFeed.GetInstance()) as HashSet<object>;
            if (attachedSubscriptionsSet == null)
                throw new InvalidOperationException("Cannot get the set of attached subscriptions!");
            if (attachedSubscriptionsSet.Count == 0)
                throw new InvalidOperationException("There is not attached subscriptions to feed!");
            DXFeedSubscription<E> subscription = null;
            foreach (var s in attachedSubscriptionsSet)
            {
                subscription = s as DXFeedSubscription<E>;
                if (subscription != null && subscription.GetSymbols().Contains(symbol))
                    break;
            }
            if (subscription == null)
                throw new InvalidOperationException(string.Format("The {0} subscription is not found in this feed!", typeof(E)));
            return subscription;
        }

        private void CompareOrders(IDxOrder playedOrder, IDxOrder receivedOrder)
        {
            Assert.AreEqual(playedOrder.EventSymbol, receivedOrder.EventSymbol);
            Assert.AreEqual(playedOrder.Count, receivedOrder.Count);
            Assert.AreEqual(playedOrder.EventFlags, receivedOrder.EventFlags);
            Assert.AreEqual(playedOrder.ExchangeCode, receivedOrder.ExchangeCode);
            Assert.AreEqual(playedOrder.Index, receivedOrder.Index);
            Assert.AreEqual(playedOrder.Level, receivedOrder.Level);
            Assert.AreEqual(playedOrder.Side, receivedOrder.Side);
            Assert.AreEqual(playedOrder.Price, receivedOrder.Price);
            Assert.AreEqual(playedOrder.Scope, receivedOrder.Scope);
            Assert.AreEqual(playedOrder.Sequence, receivedOrder.Sequence);
            Assert.AreEqual(playedOrder.Size, receivedOrder.Size);
            Assert.AreEqual(playedOrder.Source, receivedOrder.Source);
            Assert.AreEqual(playedOrder.Time, receivedOrder.Time);
            Assert.AreEqual(playedOrder.TimeSequence, receivedOrder.TimeSequence);
            Assert.AreEqual(playedOrder.MarketMaker.ToString(), receivedOrder.MarketMaker.ToString());
        }

        private void CompareCandles(IDxCandle playedCandle, IDxCandle receivedCandle)
        {
            Assert.AreEqual(playedCandle.EventSymbol, receivedCandle.EventSymbol);
            Assert.AreEqual(playedCandle.Time, receivedCandle.Time);
            Assert.AreEqual(playedCandle.Sequence, receivedCandle.Sequence);
            Assert.AreEqual(playedCandle.Count, receivedCandle.Count);
            Assert.AreEqual(playedCandle.Open, receivedCandle.Open);
            Assert.AreEqual(playedCandle.High, receivedCandle.High);
            Assert.AreEqual(playedCandle.Low, receivedCandle.Low);
            Assert.AreEqual(playedCandle.Close, receivedCandle.Close);
            Assert.AreEqual(playedCandle.Volume, receivedCandle.Volume);
            Assert.AreEqual(playedCandle.VWAP, receivedCandle.VWAP);
            Assert.AreEqual(playedCandle.BidVolume, receivedCandle.BidVolume);
            Assert.AreEqual(playedCandle.AskVolume, receivedCandle.AskVolume);
            Assert.AreEqual(playedCandle.Index, receivedCandle.Index);
            Assert.AreEqual(playedCandle.OpenInterest, receivedCandle.OpenInterest);
            Assert.AreEqual(playedCandle.ImpVolatility, receivedCandle.ImpVolatility);
            Assert.AreEqual(playedCandle.EventFlags, receivedCandle.EventFlags);
        }

        private void CompareTrades(IDxTrade playedTrade, IDxTrade lastTrade)
        {
            Assert.AreEqual(playedTrade.EventSymbol, lastTrade.EventSymbol);
            Assert.AreEqual(playedTrade.Time, lastTrade.Time);
            Assert.AreEqual(playedTrade.ExchangeCode, lastTrade.ExchangeCode);
            Assert.AreEqual(playedTrade.Price, lastTrade.Price);
            Assert.AreEqual(playedTrade.Size, lastTrade.Size);
            Assert.AreEqual(playedTrade.Tick, lastTrade.Tick);
            Assert.AreEqual(playedTrade.Change, lastTrade.Change);
            Assert.AreEqual(playedTrade.DayVolume, lastTrade.DayVolume);
        }

        #endregion

    }
}
