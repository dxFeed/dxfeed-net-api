using com.dxfeed.api.events;
using com.dxfeed.ipf;
using com.dxfeed.native;
using com.dxfeed.samples.api;
using System;
using System.Collections.Generic;
using System.Threading;

namespace com.dxfeed.samples.ipf {
    class DXFeedIpfConnect {

        class MarketEventListener<E> : DXFeedEventListener<E> where E : IDxMarketEvent {
            public void EventsReceived(IList<E> events) {
                foreach (E e in events)
                        Console.WriteLine(e.Symbol + ": " + e);
            }
        }

        static void Main(string[] args) {
            if (args.Length != 2) {
                string eventTypeNames = DXFeedConnect.GetEventTypeNames(typeof(IDxMarketEvent));
                Console.Error.WriteLine("usage: DXFeedIpfConnect <type> <ipf-file>");
                Console.Error.WriteLine("where: <type>     is dxfeed event type (" + eventTypeNames + ")");
                Console.Error.WriteLine("       <ipf-file> is name of instrument profiles file");
                return;
            }
            string argType = args[0];
            string argIpfFile = args[1];

            Type eventType = DXFeedConnect.FindEventType(argType, typeof(IDxMarketEvent));
            DXFeedSubscription<IDxMarketEvent> sub = DXFeed.GetInstance().CreateSubscription<IDxMarketEvent>(eventType);
            sub.AddEventListener(new MarketEventListener<IDxMarketEvent>());
            sub.AddSymbols(getSymbols(argIpfFile));
            Thread.Sleep(int.MaxValue);
        }

        private static List<string> getSymbols(string filename) {
            Console.WriteLine(string.Format("Reading instruments from {0} ...", filename));
            IList<InstrumentProfile> profiles = new InstrumentProfileReader().ReadFromFile(filename);
            ProfileFilter filter = new ProfileFilter();
            List<string> result = new List<string>();
            Console.WriteLine("Selected symbols are:");
            foreach (InstrumentProfile profile in profiles)
                if (filter.Accept(profile)) {
                    result.Add(profile.GetSymbol());
                    Console.WriteLine(profile.GetSymbol() + " (" + profile.GetDescription() + ")");
                }
            return result;
        }

        private class ProfileFilter {
            public bool Accept(InstrumentProfile profile) {
                // This is just a sample, any arbitrary filtering may go here.
                return
                    profile.GetTypeName().Equals("STOCK") && // stocks
                    profile.GetSIC() / 10 == 357 && // Computer And Office Equipment
                    profile.GetExchanges().Contains("XNYS"); // traded at NYSE
            }
        }

    }
}
