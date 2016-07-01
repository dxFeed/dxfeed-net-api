using System;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native;

namespace dxf_client {
	class Program {
		static void Main(string[] args) {
			if (args.Length < 3 || args.Length > 4) {
				Console.WriteLine(
					"Usage: dxf_client <host:port> <event> <symbol> <source>\n" +
					"where\n" +
					"    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
					"    event     - any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary,Candle}\n" +
					"    symbol    - IBM, MSFT, ...\n" +
					"    source    - order sources NTV, BYX, BZX, DEA, DEX, IST, ISE,...\n\n" +
					"example: dxf_client demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST NTV,IST"
				);
				return;
			}

			var address = args[0];
			var symbols = args[2].Split(',');
			var sources = args.Length == 4 ? args[3].Split(',') : new string[0];
			EventType events;
			if (!Enum.TryParse(args[1], true, out events)) {
				Console.WriteLine("Unsupported event type: " + args[1]);
				return;
			}

			Console.WriteLine(string.Format("Connecting to {0} for [{1}] on [{2}] ...", address, events, string.Join(", ", symbols)));

            // NativeTools.InitializeLogging("dxf_client.log", true, true);
			var listener = new EventPrinter();
			using (var con = new NativeConnection(address, OnDisconnect)) {
                IDxSubscription s;
                if (events == EventType.Candle) {
                    ICandleSymbolAttribute periodAttr = CandleSymbolAttributes.Period.NewPeriod(1, CandleType.DAY);
                    CandleSymbol candleSymbol = CandleSymbol.ValueOf(symbols[0], CandleSymbolAttributes.Price.MARK, periodAttr);
                    s = con.CreateSubscription(null, listener);
                    s.AddSymbol(candleSymbol);
                } else {
                    s = con.CreateSubscription(events, listener);
                    if (sources.Length > 0) {
                        s.SetSource(sources);
                    }
                    s.AddSymbols(symbols);
                }
				Console.WriteLine("Press enter to stop");
				Console.ReadLine();
			}
		}

		private static void OnDisconnect(IDxConnection con) {
			Console.WriteLine("Disconnected");
		}
	}
}
