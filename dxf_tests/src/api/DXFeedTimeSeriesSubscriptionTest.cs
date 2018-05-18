using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.tests.tools;
using com.dxfeed.tests.tools.eventplayer;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace com.dxfeed.api
{
    [TestFixture]
    public class DXFeedTimeSeriesSubscriptionTest
    {

        [Test]
        public void SetFromTimeStampTest()
        {
            string symbol = "SYMA";
            long fromTime = Tools.DateToUnixTime(DateTime.Now.AddMonths(-1));
            TestListener eventListener = new TestListener();
            var s = DXEndpoint.Create().Feed.CreateTimeSeriesSubscription<IDxCandle>();
            s.OnSubscriptionClosed += OnSubscriptionClosed;
            s.OnSymbolsAdded += OnSymbolsAdded;
            s.OnSymbolsRemoved += OnSymbolsRemoved;

            //set FromTimeStamp property initially
            isFiresOnSubscriptionClosed = false;
            isFiresOnSymbolsAdded = false;
            isFiresOnSymbolsRemoved = false;
            s.FromTimeStamp = fromTime;
            Assert.False(isFiresOnSubscriptionClosed);
            Assert.False(isFiresOnSymbolsAdded);
            Assert.False(isFiresOnSymbolsRemoved);

            //complete subscription initialization
            s.AddEventListener(eventListener);
            s.AddSymbols(symbol);
            Assert.AreEqual(fromTime, s.FromTimeStamp);
            Assert.True(s.GetSymbols().SetEquals(new object[] { symbol }));
            Assert.True(s.EventTypes.SetEquals(new Type[] { typeof(IDxCandle) }));

            var playedCandle = new PlayedCandle(symbol, Tools.DateToUnixTime(DateTime.Now), 123, 100, 12.34, 56.78, 9.0, 43.21, 1000, 999, 1001, 1002, 1, 777, 888, EventFlag.RemoveSymbol);

            EventPlayer<IDxCandle> eventPlayer = new EventPlayer<IDxCandle>(s as DXFeedSubscription<IDxCandle>);
            eventPlayer.PlayEvents(symbol, playedCandle);
            Assert.AreEqual(eventListener.GetEventCount<IDxCandle>(symbol), 1);
            IDxCandle receivedCandle = eventListener.GetLastEvent<IDxCandle>().Event;
            Assert.AreEqual(symbol, receivedCandle.EventSymbol.ToString());
            DXFeedTest.CompareCandles(playedCandle, receivedCandle);
        }

        [Test]
        public void ResetFromTimeStampTest()
        {
            string symbol = "SYMA";
            TestListener eventListener = new TestListener();
            var s = DXEndpoint.Create().Feed.CreateTimeSeriesSubscription<IDxCandle>();
            s.OnSubscriptionClosed += OnSubscriptionClosed;
            s.OnSymbolsAdded += OnSymbolsAdded;
            s.OnSymbolsRemoved += OnSymbolsRemoved;
            s.AddEventListener(eventListener);
            s.AddSymbols(symbol);
            Assert.AreEqual(long.MaxValue, s.FromTimeStamp);

            //try to change FromTimeStamp property
            isFiresOnSubscriptionClosed = false;
            isFiresOnSymbolsAdded = false;
            isFiresOnSymbolsRemoved = false;
            long fromTime = Tools.DateToUnixTime(DateTime.Now.AddMonths(-1));
            s.FromTimeStamp = fromTime;
            Assert.AreEqual(fromTime, s.FromTimeStamp);
            Assert.True(s.GetSymbols().SetEquals(new object[] { symbol }));
            Assert.True(s.EventTypes.SetEquals(new Type[] { typeof(IDxCandle) }));
            Assert.False(isFiresOnSubscriptionClosed);
            Assert.False(isFiresOnSymbolsAdded);
            Assert.False(isFiresOnSymbolsRemoved);

            var playedCandle = new PlayedCandle(symbol, Tools.DateToUnixTime(DateTime.Now), 123, 100, 12.34, 56.78, 9.0, 43.21, 1000, 999, 1001, 1002, 1, 777, 888, EventFlag.RemoveSymbol);

            EventPlayer<IDxCandle> eventPlayer = new EventPlayer<IDxCandle>(s as DXFeedSubscription<IDxCandle>);
            eventPlayer.PlayEvents(symbol, playedCandle);
            Assert.AreEqual(eventListener.GetEventCount<IDxCandle>(symbol), 1);
            IDxCandle receivedCandle = eventListener.GetLastEvent<IDxCandle>().Event;
            Assert.AreEqual(symbol, receivedCandle.EventSymbol.ToString());
            DXFeedTest.CompareCandles(playedCandle, receivedCandle);
        }

        [Test]
        public void ParallelFromTimeStampTest()
        {
            var s = DXEndpoint.Create().Feed.CreateTimeSeriesSubscription<IDxCandle>();

            Random random = new Random();
            Parallel.For(ParallelFrom, ParallelTo, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    s.FromTimeStamp = Tools.DateToUnixTime(DateTime.Now.AddDays(random.Next(-356, 0)));
                });
            });
        }

        #region Private fields and methods

        private const int ParallelFrom = 0;
        private const int ParallelTo = 101;

        private volatile bool isFiresOnSubscriptionClosed = false;
        private volatile bool isFiresOnSymbolsAdded = false;
        private volatile bool isFiresOnSymbolsRemoved = false;

        private void OnSubscriptionClosed(object sender, EventArgs args)
        {
            isFiresOnSubscriptionClosed = true;
        }

        private void OnSymbolsRemoved(object sender, DXFeedSymbolsUpdateEventArgs args)
        {
            isFiresOnSymbolsRemoved = true;
        }

        private void OnSymbolsAdded(object sender, DXFeedSymbolsUpdateEventArgs args)
        {
            isFiresOnSymbolsAdded = true;
        }

        #endregion
    }
}
