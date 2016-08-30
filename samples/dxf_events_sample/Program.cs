using System;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_events_sample {
    /// <summary>
    /// This sample class demonstrates subscription to events.
    /// The sample configures via command line, subscribes to events and prints received data.
    /// </summary>
    class Program {

        private const int hostIndex = 0;
        private const int eventIndex = 1;
        private const int symbolIndex = 2;

        private static void OnDisconnect(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine(
                    "Usage: dxf_events_sample <host:port> <event> <symbol>\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    event     - any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary}\n" +
                    "    symbol    - IBM, MSFT, ...\n\n" +
                    "example: dxf_events_sample demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST"
                );
                return;
            }

            var address = args[hostIndex];

            EventType events;
            if (!Enum.TryParse(args[eventIndex], true, out events)) {
                Console.WriteLine("Unsupported event type: " + args[1]);
                return;
            }

            string[] symbols = args[symbolIndex].Split(',');

            Console.WriteLine(string.Format("Connecting to {0} for [{1}] on [{2}] ...",
                address, events, string.Join(", ", symbols)));

            try {
                NativeTools.InitializeLogging("log.log", true, true);
                using (var con = new NativeConnection(address, OnDisconnect)) {
                    using (var s = con.CreateSubscription(events, new EventListener())) {
                        s.AddSymbols(symbols);

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            } catch (DxException dxException) {
                Console.WriteLine("Native exception occured: " + dxException.Message);
            } catch (Exception exc) {
                Console.WriteLine("Exception occured: " + exc.Message);
            }
        }
    }
}
