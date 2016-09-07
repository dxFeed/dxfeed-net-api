using com.dxfeed.api.events;
using com.dxfeed.native;
using System;
using System.Collections.Generic;
using System.Threading;

namespace com.dxfeed.samples.simple
{
    class PrintQuoteEvents {

        class EventListener<E> : DXFeedEventListener<E> {
            public void EventsReceived(IList<E> events) {
                foreach (E quote in events)
                    Console.WriteLine(quote);
            }
        }

        static void Main(string[] args) {
            string symbol = args[0];
            // Use default DXFeed instance
            DXFeedSubscription<IDxQuote> sub = DXFeed.GetInstance().CreateSubscription<IDxQuote>();
            sub.AddEventListener(new EventListener<IDxQuote>());
            sub.AddSymbols(symbol);
            Thread.Sleep(int.MaxValue);
        }
    }
}
