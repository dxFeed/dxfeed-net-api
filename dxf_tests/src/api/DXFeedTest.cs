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

            var playedOrder = new PlayedOrder(symbol, 0, 0x4e54560000000006, 0, 0, 0, 0, 100, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA");

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

            //thread-safety case
            DXEndpoint.Create();
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    DXFeed.GetInstance().CreateSubscription<IDxOrder>();
                });
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
                DXFeed.GetInstance().CreateSubscription<IDxIndexedEvent>(typeof(IDxCandle), typeof(IDxQuote));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateSubscription<IDxLastingEvent>(typeof(IDxOrder), typeof(IDxQuote));
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
            var playedOrder = new PlayedOrder(symbol, 0, 0x4e54560000000006, 0, 0, 0, 0, 100, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA");
            eventPlayer.PlayEvents(symbol, playedOrder);
            var playedTrade = new PlayedTrade(symbol, 0, 0, 0, 'B', 123.456,  100, 123, 1.1, 0, 2.2, 0.0, Direction.Undefined, false, Scope.Regional);
            eventPlayer.PlayEvents(symbol, playedTrade);

            Assert.AreEqual(eventListener.GetEventCount<IDxOrder>(symbol), 1);
            Assert.AreEqual(eventListener.GetEventCount<IDxTrade>(symbol), 1);

            //thread-safety case
            DXEndpoint.Create();
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    DXFeed.GetInstance().CreateSubscription<IDxEventType>(typeof(IDxOrder), typeof(IDxTrade));
                });
            });
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

            //thread-safety case
            DXEndpoint.Create();
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    DXEndpoint.GetInstance().Feed.CreateTimeSeriesSubscription<IDxCandle>();
                });
            });
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
                DXFeed.GetInstance().CreateTimeSeriesSubscription<IDxTimeSeriesEvent>(typeof(IDxCandle), typeof(IDxQuote));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateTimeSeriesSubscription<IDxTimeSeriesEvent>(typeof(IDxOrder), typeof(IDxQuote));
            });
            Assert.Catch(typeof(ArgumentException), () =>
            {
                DXFeed.GetInstance().CreateTimeSeriesSubscription<IDxTimeSeriesEvent>(typeof(string));
            });

            var symbol = "SYMA";
            var s = DXFeed.GetInstance().CreateTimeSeriesSubscription<IDxTimeSeriesEvent>(typeof(IDxCandle), typeof(IDxGreeks));
            s.AddSymbols(symbol);
            TestListener eventListener = new TestListener();
            s.AddEventListener(eventListener);

            EventPlayer<IDxTimeSeriesEvent> eventPlayer = new EventPlayer<IDxTimeSeriesEvent>(s as DXFeedSubscription<IDxTimeSeriesEvent>);
            var playedCandle = new PlayedCandle(symbol, Tools.DateToUnixTime(DateTime.Now), 123, 100, 12.34, 56.78, 9.0, 43.21, 1000, 999, 1001, 1002, 1, 777, 888, EventFlag.RemoveSymbol);
            eventPlayer.PlayEvents(symbol, playedCandle);
            var playedGreeks = new PlayedGreeks(symbol, EventFlag.RemoveSymbol, 1, Tools.DateToUnixTime(DateTime.Now), 456.789, 11, 555, 666, 777, 888, 999);
            eventPlayer.PlayEvents(symbol, playedGreeks);

            Assert.AreEqual(eventListener.GetEventCount<IDxCandle>(symbol), 1);
            Assert.AreEqual(eventListener.GetEventCount<IDxGreeks>(symbol), 1);

            //thread-safety case
            DXEndpoint.Create();
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    DXFeed.GetInstance().CreateTimeSeriesSubscription<IDxTimeSeriesEvent>(typeof(IDxCandle), typeof(IDxGreeks));
                });
            });
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

            //thread-safety case
            DXEndpoint.Create();
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    DXFeed.GetInstance().AttachSubscription(new DXFeedSubscription<IDxOrder>(DXEndpoint.GetInstance() as DXEndpoint));
                });
            });
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

            //thread-safety case
            DXEndpoint.Create();
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    var newSubscription = new DXFeedSubscription<IDxOrder>(DXEndpoint.GetInstance() as DXEndpoint);
                    DXFeed.GetInstance().AttachSubscription(newSubscription);
                    DXFeed.GetInstance().DetachSubscription(newSubscription);
                });
            });
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
            Task<IDxLastingEvent> promise = DXFeed.GetInstance().GetLastEventPromise<IDxTrade>(symbol, cancelSource.Token);
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
            var playedTrade = new PlayedTrade(symbol, Tools.DateToUnixTime(DateTime.Now), 0, 0, 'B', 123.456,  100, 123, 1.1, 0, 2.2, 0.0, Direction.Undefined, false, Scope.Regional);
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

            //thread-safety case
            DXEndpoint.Create();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var threadSymbol = symbol + i.ToString();
                var threadPlayedTrade = new PlayedTrade(threadSymbol, Tools.DateToUnixTime(DateTime.Now), 0, 0, 'B', 123.456,  100, 123, 1.1, 0, 2.2, 0.0, Direction.Undefined, false, Scope.Regional);
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    var eventPlayer = new EventPlayer<IDxTrade>(GetSubscriptionFromFeed<IDxTrade>(threadSymbol));
                    eventPlayer.PlayEvents(threadSymbol, threadPlayedTrade);
                }));
                var threadPromise = DXEndpoint.GetInstance().Feed
                    .GetLastEventPromise<IDxTrade>(threadSymbol, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token)
                    .ContinueWith((resultPromise) =>
                {
                    IDxTrade threadTrade = resultPromise.Result as IDxTrade;
                    Assert.AreEqual(threadSymbol, threadTrade.EventSymbol);
                });
                tasks.Add(threadPromise);
            };
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(tasks.ToArray());
            });
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
            List<Task<IDxLastingEvent>> promises = DXFeed.GetInstance().GetLastEventsPromises<IDxTrade>(symbols, cancelSource.Token);
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
                new PlayedTrade(symbols[0], Tools.DateToUnixTime(DateTime.Now), 0, 0, 'B', 123.456,  100, 123, 1.1, 0, 2.2, 0.0, Direction.Undefined, false, Scope.Regional),
                new PlayedTrade(symbols[1], Tools.DateToUnixTime(DateTime.Now), 0, 0, 'B', 234.567,  101, 234, 3.2, 0, 4.3, 0.0, Direction.Undefined, false, Scope.Regional)
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

            //thread-safety case
            DXEndpoint.Create();
            List<Task> threadTasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var threadSymbols = new string[] { symbols[0] + i.ToString(), symbols[1] + i.ToString() };
                var threadPlayedTrades = new PlayedTrade[] {
                    new PlayedTrade(threadSymbols[0], Tools.DateToUnixTime(DateTime.Now), 0, 0, 'B', 123.456,  100, 123, 1.1, 0, 2.2, 0.0, Direction.Undefined, false, Scope.Regional),
                    new PlayedTrade(threadSymbols[1], Tools.DateToUnixTime(DateTime.Now), 0, 0, 'B', 234.567,  101, 234, 3.2, 0, 4.3, 0.0, Direction.Undefined, false, Scope.Regional)
                };
                HashSet<string> threadPlayedSymbols = new HashSet<string>();
                foreach(var t in threadPlayedTrades)
                {
                    threadTasks.Add(Task.Run(() =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        var eventPlayer = new EventPlayer<IDxTrade>(GetSubscriptionFromFeed<IDxTrade>(t.EventSymbol));
                        eventPlayer.PlayEvents(t.EventSymbol, t);
                    }));
                    threadPlayedSymbols.Add(t.EventSymbol);
                }
                foreach (var p in DXEndpoint.GetInstance().Feed.GetLastEventsPromises<IDxTrade>(threadSymbols, new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token))
                {
                    threadTasks.Add(p.ContinueWith((resultPromise) =>
                    {
                        IDxTrade threadTrade = resultPromise.Result as IDxTrade;
                        Assert.True(threadPlayedSymbols.Remove(threadTrade.EventSymbol));
                    }));
                }
            };
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(threadTasks.ToArray());
            });
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
                new PlayedOrder(symbol, 0, 0x4e54560000000006, Tools.DateToUnixTime(date), 0, 0, 100, 100, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                new PlayedOrder(symbol, 0, 0x4e54560000000005, Tools.DateToUnixTime(date.AddMinutes(-1)), 0, 0, 100.5, 101, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                new PlayedOrder(symbol, 0, 0x4e54560000000004, Tools.DateToUnixTime(date.AddMinutes(-2)), 0, 0, 101, 102, 25, Scope.Order, Side.Sell, 'A', OrderSource.NTV, "AAAA"),
                new PlayedOrder(symbol, 0, 0x4e54560000000003, Tools.DateToUnixTime(date.AddMinutes(-3)), 0, 0, 100, 103, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                new PlayedOrder(symbol, 0, 0x4e54560000000002, Tools.DateToUnixTime(date.AddMinutes(-4)), 0, 0, 100.4, 104, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                new PlayedOrder(symbol, 0, 0x4e54560000000001, Tools.DateToUnixTime(date.AddMinutes(-5)), 0, 0, 100.3, 105, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                new PlayedOrder(symbol, 0, 0x4e54560000000000, Tools.DateToUnixTime(date.AddMinutes(-6)), 0, 0, 100.2, 106, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA")
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

            //thread-safety case
            DXEndpoint.Create();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var threadSymbol = symbol + i.ToString();
                var threadPlayedOrders = new PlayedOrder[] {
                    new PlayedOrder(threadSymbol, 0, 0x4e54560000000006, Tools.DateToUnixTime(date), 0, 0, 100, 100, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                    new PlayedOrder(threadSymbol, 0, 0x4e54560000000005, Tools.DateToUnixTime(date.AddMinutes(-1)), 0, 0, 100.5, 101, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                    new PlayedOrder(threadSymbol, 0, 0x4e54560000000004, Tools.DateToUnixTime(date.AddMinutes(-2)), 0, 0, 101, 102, 25, Scope.Order, Side.Sell, 'A', OrderSource.NTV, "AAAA"),
                    new PlayedOrder(threadSymbol, 0, 0x4e54560000000003, Tools.DateToUnixTime(date.AddMinutes(-3)), 0, 0, 100, 103, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                    new PlayedOrder(threadSymbol, 0, 0x4e54560000000002, Tools.DateToUnixTime(date.AddMinutes(-4)), 0, 0, 100.4, 104, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                    new PlayedOrder(threadSymbol, 0, 0x4e54560000000001, Tools.DateToUnixTime(date.AddMinutes(-5)), 0, 0, 100.3, 105, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA"),
                    new PlayedOrder(threadSymbol, 0, 0x4e54560000000000, Tools.DateToUnixTime(date.AddMinutes(-6)), 0, 0, 100.2, 106, 25, Scope.Order, Side.Buy, 'A', OrderSource.NTV, "AAAA")
                };
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    var eventPlayer = new EventPlayer<IDxOrder>(GetSubscriptionFromFeed<IDxOrder>(threadSymbol));
                    eventPlayer.PlaySnapshot(threadSymbol, threadPlayedOrders);
                }));
                var threadPromise = DXEndpoint.GetInstance().Feed
                    .GetIndexedEventsPromise<IDxOrder>(threadSymbol, OrderSource.NTV, new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token)
                    .ContinueWith((resultPromise) =>
                    {
                        foreach (var o in resultPromise.Result)
                            Assert.AreEqual(threadSymbol, o.EventSymbol);
                    });
                tasks.Add(threadPromise);
            };
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(tasks.ToArray());
            });
        }

        [Test]
        public void GetTimeSeriesPromiseTest()
        {
            //create default endpoint
            DXEndpoint.Create();
            var symbol = "SYMA";

            //try to create promise with invalid parameters
            Assert.Catch(typeof(ArgumentException), () =>
            {
                try
                {
                    DXFeed.GetInstance().GetTimeSeriesPromise<IDxGreeks>(null, 0, 0, CancellationToken.None).Wait();
                }
                catch (AggregateException ae)
                {
                    foreach (var inner in ae.InnerExceptions)
                        throw inner;
                }
            });

            //try to cancel promise
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            Task<List<IDxGreeks>> promise = DXFeed.GetInstance().GetTimeSeriesPromise<IDxGreeks>(symbol, 0, 0, cancelSource.Token);
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
            promise = DXFeed.GetInstance().GetTimeSeriesPromise<IDxGreeks>(symbol, 0, 0, cancelSource.Token);
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
            promise = DXFeed.GetInstance().GetTimeSeriesPromise<IDxGreeks>(symbol, 0, 0, CancellationToken.None);
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
            promise = DXFeed.GetInstance().GetTimeSeriesPromise<IDxGreeks>(symbol, 0, 0, CancellationToken.None);
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
            var playedGreeks = new PlayedGreeks[] {
                new PlayedGreeks(symbol, 0, 5, Tools.DateToUnixTime(date), 156.789, 111, 155, 166, 177, 188, 199),
                new PlayedGreeks(symbol, 0, 4, Tools.DateToUnixTime(date.AddMinutes(-1)), 256.789, 211, 255, 266, 277, 288, 299),
                new PlayedGreeks(symbol, 0, 3, Tools.DateToUnixTime(date.AddMinutes(-2)), 356.789, 311, 355, 366, 377, 388, 399),
                new PlayedGreeks(symbol, 0, 2, Tools.DateToUnixTime(date.AddMinutes(-3)), 456.789, 411, 455, 466, 477, 488, 499),
                new PlayedGreeks(symbol, 0, 1, Tools.DateToUnixTime(date.AddMinutes(-4)), 556.789, 511, 555, 566, 577, 588, 599),
            };
            var expectedGreeks = new PlayedGreeks[] {
                new PlayedGreeks(symbol, 0, 4, Tools.DateToUnixTime(date.AddMinutes(-1)), 256.789, 211, 255, 266, 277, 288, 299),
                new PlayedGreeks(symbol, 0, 3, Tools.DateToUnixTime(date.AddMinutes(-2)), 356.789, 311, 355, 366, 377, 388, 399),
                new PlayedGreeks(symbol, 0, 2, Tools.DateToUnixTime(date.AddMinutes(-3)), 456.789, 411, 455, 466, 477, 488, 499)
            };
            Task eventPlayerTask = Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                EventPlayer<IDxGreeks> eventPlayer = new EventPlayer<IDxGreeks>(GetSubscriptionFromFeed<IDxGreeks>(symbol));
                eventPlayer.PlaySnapshot(symbol, playedGreeks);
            });
            cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            promise = DXEndpoint.Create().Feed.GetTimeSeriesPromise<IDxGreeks>(symbol, Tools.DateToUnixTime(date.AddMinutes(-3)), Tools.DateToUnixTime(date.AddMinutes(-1)), cancelSource.Token);
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(promise, eventPlayerTask);
            });

            var receivedGreeks = promise.Result;
            receivedGreeks.Reverse();
            Assert.AreEqual(expectedGreeks.Length, receivedGreeks.Count);
            for (int i = 0; i < expectedGreeks.Length; i++)
            {
                Assert.AreEqual(symbol, receivedGreeks[i].EventSymbol.ToString());
                CompareGreeks(expectedGreeks[i], receivedGreeks[i]);
            }

            //thread-safety case
            DXEndpoint.Create();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var threadSymbol = symbol + i.ToString();
                var threadPlayedGreeks = new PlayedGreeks[] {
                    new PlayedGreeks(symbol, 0, 5, Tools.DateToUnixTime(date), 156.789, 111, 155, 166, 177, 188, 199),
                    new PlayedGreeks(symbol, 0, 4, Tools.DateToUnixTime(date.AddMinutes(-1)), 256.789, 211, 255, 266, 277, 288, 299),
                    new PlayedGreeks(symbol, 0, 3, Tools.DateToUnixTime(date.AddMinutes(-2)), 356.789, 311, 355, 366, 377, 388, 399),
                    new PlayedGreeks(symbol, 0, 2, Tools.DateToUnixTime(date.AddMinutes(-3)), 456.789, 411, 455, 466, 477, 488, 499),
                    new PlayedGreeks(symbol, 0, 1, Tools.DateToUnixTime(date.AddMinutes(-4)), 556.789, 511, 555, 566, 577, 588, 599),
                };
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    var eventPlayer = new EventPlayer<IDxGreeks>(GetSubscriptionFromFeed<IDxGreeks>(threadSymbol));
                    eventPlayer.PlaySnapshot(threadSymbol, threadPlayedGreeks);
                }));
                var threadPromise = DXEndpoint.GetInstance().Feed
                    .GetTimeSeriesPromise<IDxGreeks>(threadSymbol, Tools.DateToUnixTime(date.AddMinutes(-3)), Tools.DateToUnixTime(date.AddMinutes(-1)), new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token)
                    .ContinueWith((resultPromise) =>
                    {
                        foreach (var g in resultPromise.Result)
                            Assert.AreEqual(threadSymbol, g.EventSymbol);
                    });
                tasks.Add(threadPromise);
            };
            Assert.DoesNotThrow(() =>
            {
                Task.WaitAll(tasks.ToArray());
            });
        }

        #region internal static methods

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
        internal static DXFeedSubscription<E> GetSubscriptionFromFeed<E>(string symbol)
            where E : class, IDxEventType
        {
            FieldInfo attachedSubscriptionsInfo = typeof(DXFeed).GetField("attachedSubscriptions", BindingFlags.NonPublic | BindingFlags.Instance);
            if (attachedSubscriptionsInfo == null)
                throw new InvalidOperationException("attachedSubscriptions field not found!");
            HashSet<object> attachedSubscriptionsSet = attachedSubscriptionsInfo.GetValue(DXFeed.GetInstance()) as HashSet<object>;
            if (attachedSubscriptionsSet == null)
                throw new InvalidOperationException("Cannot get the set of attached subscriptions!");
            FieldInfo attachLockInfo = typeof(DXFeed).GetField("attachLock", BindingFlags.NonPublic | BindingFlags.Instance); ;
            if (attachLockInfo == null)
                throw new InvalidOperationException("attachLock field not found!");
            object attachLocker = attachLockInfo.GetValue(DXFeed.GetInstance());
            if (attachLocker == null)
                throw new InvalidOperationException("Cannot get the locker of attached subscriptions!");

            HashSet<object> subscriptions = null;
            lock (attachLocker)
            {
                subscriptions = new HashSet<object>(attachedSubscriptionsSet);
            }
            if (subscriptions.Count == 0)
                throw new InvalidOperationException("There is no attached subscriptions to feed!");
            DXFeedSubscription<E> subscription = null;
            foreach (var s in subscriptions)
            {
                subscription = s as DXFeedSubscription<E>;
                if (subscription != null && subscription.GetSymbols().Contains(symbol))
                    break;
            }
            if (subscription == null)
                throw new InvalidOperationException(string.Format("The {0} subscription is not found in this feed!", typeof(E)));
            return subscription;
        }

        internal static void CompareOrders(IDxOrder playedOrder, IDxOrder receivedOrder)
        {
            Assert.AreEqual(playedOrder.EventSymbol, receivedOrder.EventSymbol);
            Assert.AreEqual(playedOrder.Count, receivedOrder.Count);
            Assert.AreEqual(playedOrder.EventFlags, receivedOrder.EventFlags);
            Assert.AreEqual(playedOrder.ExchangeCode, receivedOrder.ExchangeCode);
            Assert.AreEqual(playedOrder.Index, receivedOrder.Index);
            Assert.AreEqual(playedOrder.Side, receivedOrder.Side);
            Assert.AreEqual(playedOrder.Price, receivedOrder.Price);
            Assert.AreEqual(playedOrder.Scope, receivedOrder.Scope);
            Assert.AreEqual(playedOrder.Sequence, receivedOrder.Sequence);
            Assert.AreEqual(playedOrder.Size, receivedOrder.Size);
            Assert.AreEqual(playedOrder.Source, receivedOrder.Source);
            Assert.AreEqual(playedOrder.Time, receivedOrder.Time);
            Assert.AreEqual(playedOrder.TimeNanoPart, receivedOrder.TimeNanoPart);
            Assert.AreEqual(playedOrder.MarketMaker.ToString(), receivedOrder.MarketMaker.ToString());
        }

        internal static void CompareCandles(IDxCandle playedCandle, IDxCandle receivedCandle)
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

        internal static void CompareTrades(IDxTrade playedTrade, IDxTrade lastTrade)
        {
            Assert.AreEqual(playedTrade.EventSymbol, lastTrade.EventSymbol);
            Assert.AreEqual(playedTrade.Time, lastTrade.Time);
            Assert.AreEqual(playedTrade.ExchangeCode, lastTrade.ExchangeCode);
            Assert.AreEqual(playedTrade.Price, lastTrade.Price);
            Assert.AreEqual(playedTrade.Size, lastTrade.Size);
            Assert.AreEqual(playedTrade.Change, lastTrade.Change);
            Assert.AreEqual(playedTrade.DayVolume, lastTrade.DayVolume);
        }

        internal static void CompareGreeks(IDxGreeks playedGreek, IDxGreeks receivedGreek)
        {
            Assert.AreEqual(playedGreek.EventSymbol, receivedGreek.EventSymbol);
            Assert.AreEqual(playedGreek.EventFlags, receivedGreek.EventFlags);
            Assert.AreEqual(playedGreek.Delta, receivedGreek.Delta);
            Assert.AreEqual(playedGreek.Gamma, receivedGreek.Gamma);
            Assert.AreEqual(playedGreek.Price, receivedGreek.Price);
            Assert.AreEqual(playedGreek.Rho, receivedGreek.Rho);
            Assert.AreEqual(playedGreek.Theta, receivedGreek.Theta);
            Assert.AreEqual(playedGreek.TimeStamp, receivedGreek.TimeStamp);
            Assert.AreEqual(playedGreek.Time, receivedGreek.Time);
            Assert.AreEqual(playedGreek.Vega, receivedGreek.Vega);
            Assert.AreEqual(playedGreek.Volatility, receivedGreek.Volatility);
            Assert.AreEqual(playedGreek.Index, receivedGreek.Index);
        }

        #endregion

        #region private fields and methods

        private const int ParallelFrom = 0;
        private const int ParallelTo = 101;

        #endregion

    }
}
