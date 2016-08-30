using com.dxfeed.api;
using com.dxfeed.native;
using System;

namespace dxf_order_view_sample {
    class Program {
        private const int hostIndex = 0;

        private static void OnDisconnect(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine(
                    "Usage: dxf_events_sample <host:port>\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "example: dxf_events_sample demo.dxfeed.com:7300"
                );
                return;
            }

            var address = args[hostIndex];

            Console.WriteLine(string.Format("Connecting to {0} for Order View", address));

            try {
                NativeTools.InitializeLogging("log.log", true, true);
                using (var con = new NativeConnection(address, OnDisconnect)) {
                    using (var sub = con.CreateOrderViewSubscription(new OrderViewEventListener())) {
                        sub.SetSource("NTV", "ISE", "DEA", "DEX");
                        sub.SetSymbols("AAPL", "GOOG", "IBM", "F");
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
