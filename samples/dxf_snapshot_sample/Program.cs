using System;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_snapshot_sample {
    /// <summary>
    /// This sample class demonstrates subscription to snapshots.
    /// The sample configures via command line, subscribes to snapshot and prints received data.
    /// </summary>
    class Program {

        private const int hostIndex = 0;
        private const int eventIndex = 1;
        private const int symbolIndex = 2;
        private const int sourceIndex = 3;
        private const int defaultTime = 0;

        private static void OnDisconnect(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine(
                    "Usage: dxf_snapshot_sample <host:port> <event> <symbol> [<source>]\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    event     - snapshot event Order or Candle\n" +
                    "                for MarketMaker see source parameter\n" +
                    "    symbol    - symbol string, it is allowed to use only one symbol\n" +
                    "                a) event symbol: IBM, MSFT, ...\n" +
                    "                b) candle symbol attribute: XBT/USD{=d},\n" +
                    "                   AAPL{=d,price=mark}, ...\n" +
                    "    source    - a) source for Order (also it may be empty), e.g. NTV, BYX,\n" +
                    "                   BZX, DEA, ISE, DEX, IST\n" +
                    "                   it is allowed to use only one source\n" +
                    "                b) source for MarketMaker, one of following: COMPOSITE_BID\n" +
                    "                   or COMPOSITE_ASK\n\n" +
                    "order example: dxf_snapshot_sample demo.dxfeed.com:7300 Order AAPL NTV\n" +
                    "market maker example:\n" +
                    "    dxf_snapshot_sample demo.dxfeed.com:7300 Order AAPL COMPOSITE_BID\n" +
                    "candle example: dxf_snapshot_sample demo.dxfeed.com:7300 Candle XBT/USD{=d}"
                );
                return;
            }

            var address = args[hostIndex];
            var symbol = args[symbolIndex];

            EventType eventType;
            if (!Enum.TryParse(args[eventIndex], true, out eventType) ||
                eventType != EventType.Order && eventType != EventType.Candle) {

                Console.WriteLine("Unsupported event type: " + args[eventIndex]);
                return;
            }

            var source = string.Empty;
            if (args.Length == sourceIndex + 1)
                source = args[sourceIndex];

            if (eventType == EventType.Candle)
                Console.WriteLine(string.Format("Connecting to {0} for Candle snapshot on {1}...", 
                    address, symbol));
            else
                Console.WriteLine(string.Format("Connecting to {0} for Order snapshot on {1}{2}...",
                    address, symbol, source == string.Empty ? string.Empty : "#" + source));

            try {
                using (var con = new NativeConnection(address, OnDisconnect)) {
                    using (var s = con.CreateSnapshotSubscription(defaultTime, new SnapshotListener())) {
                        if (eventType == EventType.Order) {
                            if (source != string.Empty)
                                s.AddSource(source);
                            s.AddSymbol(symbol);
                        } else {
                            s.AddSymbol(CandleSymbol.ValueOf(symbol));
                        }

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
