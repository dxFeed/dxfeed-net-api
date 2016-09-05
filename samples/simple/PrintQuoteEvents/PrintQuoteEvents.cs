using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.dxfeed.native;
using com.dxfeed.api.events;

namespace com.dxfeed.samples.simple {
    class PrintQuoteEvents {
        static void Main(string[] args) {
            string symbol = args[0];
            // Use default DXFeed instance for that data feed address is defined by dxfeed.properties file
            DXFeedSubscription<IDxQuote> sub = DXFeed.GetInstance().CreateSubscription<IDxQuote>();
            sub.addEventListener(new DXFeedEventListener<Quote>() {
                public void eventsReceived(List<Quote> events)
                {
                    for (Quote quote : events)
                        System.out.println(quote);
                }
            });
            sub.addSymbols(symbol);
            Thread.sleep(Long.MAX_VALUE);
        }
    }
}
