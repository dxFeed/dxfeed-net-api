using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.tests.tools;
using com.dxfeed.tests.tools.eventplayer;
using NUnit.Framework;
using System;

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
                DXFeed.GetInstance().CreateSubscription<IndexedEvent>(typeof(IDxCandle), typeof(IDxQuote));
                DXFeed.GetInstance().CreateSubscription<LastingEvent>(typeof(IDxOrder), typeof(IDxQuote));
                DXFeed.GetInstance().CreateSubscription<IDxMarketEvent>(typeof(IDxCandle));
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

    }
}
