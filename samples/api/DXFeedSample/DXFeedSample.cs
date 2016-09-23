﻿/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.api;
using com.dxfeed.api.events;
using System;
using System.Collections.Generic;
using System.Threading;

namespace com.dxfeed.samples.api
{
    class DXFeedSample
    {
        class MidPriceEventListener<E> : DXFeedEventListener<E>
        {
            public void EventsReceived(IList<E> quotes)
            {
                foreach (IDxQuote quote in quotes)
                    Console.WriteLine("Mid = " + (quote.BidPrice + quote.AskPrice) / 2);
            }
        }

        class EventListener<E> : DXFeedEventListener<E>
        {
            public void EventsReceived(IList<E> events)
            {
                foreach (E e in events)
                    Console.WriteLine(e);
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("usage: DXFeedSample <symbol>");
                Console.Error.WriteLine("where: <symbol>  is security symbol (e.g. IBM, C, SPX etc.)");
                return;
            }
            string symbol = args[0];
            TestQuoteListener(symbol);
            TestQuoteAndTradeListener(symbol);
            TestTradeSnapshots(symbol);
        }

        private static void TestQuoteListener(string symbol)
        {
            DXFeedSubscription<IDxQuote> sub = DXFeed.GetInstance().CreateSubscription<IDxQuote>();
            sub.AddEventListener(new MidPriceEventListener<IDxQuote>());
            sub.AddSymbols(symbol);
        }

        private static void TestQuoteAndTradeListener(string symbol)
        {
            DXFeedSubscription<IDxMarketEvent> sub = DXFeed.GetInstance().CreateSubscription<IDxMarketEvent>(typeof(IDxQuote), typeof(IDxTrade));
            sub.AddEventListener(new EventListener<IDxMarketEvent>());
            sub.AddSymbols(symbol);
        }

        private static void TestTradeSnapshots(string symbol)
        {
            DXFeed feed = DXFeed.GetInstance();
            DXFeedSubscription<IDxTrade> sub = feed.CreateSubscription<IDxTrade>();
            sub.AddSymbols(symbol);
            while (true)
            {
                //TODO: lasting event
                //Console.WriteLine(feed.getLastEvent(new Trade(symbol)));
                Thread.Sleep(1000);
            }
        }
    }
}