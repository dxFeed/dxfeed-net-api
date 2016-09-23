/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.api;
using com.dxfeed.api.events;
using System;
using System.Collections.Generic;
using System.Threading;

namespace com.dxfeed.samples.simple
{
    class PrintQuoteEvents
    {
        class EventListener<E> : DXFeedEventListener<E>
        {
            public void EventsReceived(IList<E> events)
            {
                foreach (E quote in events)
                    Console.WriteLine(quote);
            }
        }

        static void Main(string[] args)
        {
            string symbol = args[0];
            // Use default DXFeed instance
            DXFeedSubscription<IDxQuote> sub = DXFeed.GetInstance().CreateSubscription<IDxQuote>();
            sub.AddEventListener(new EventListener<IDxQuote>());
            sub.AddSymbols(symbol);
            Thread.Sleep(int.MaxValue);
        }
    }
}
