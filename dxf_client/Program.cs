using System;
using com.dxfeed.api;
using com.dxfeed.native;

namespace dxf_client {
	class Program {
		static void Main(string[] args) {
			if (args.Length != 3) {
				Console.WriteLine("Usage: dxf_client <host:port> <event> <symbol>\n" +
				                  "where\n" +
				                  "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
				                  "    event     - any of the {Profile,Order,Quote,Trade,TimeAndSale,Fundamental}\n" +
				                  "    symbol    - IBM, MSFT, ...\n\n" +
				                  "example: dxf_client demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST");
				return;
			}
			
			var address = args[0];
			var symbols = args[2].Split(',');
			EventType events;
			if (!Enum.TryParse(args[1], true, out events)) {
				Console.WriteLine("Unsupported event type: " + args[1]);
				return;
			}

			Console.WriteLine(string.Format("Connecting to {0} for {1} on {2} ...", address, events, symbols));


			var listener = new EventPrinter();
			using(var con = new NativeConnection()) {
				con.Connect(address);
				var s = con.CreateSubscription(events, listener);
				Console.WriteLine("Press enter to stop");
				s.AddSymbols(symbols);
				Console.ReadLine();
			}
		}
	}

	class EventPrinter : IDxFeedListener {
		#region Implementation of IDxFeedListener

		public void OnQuote<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxQuote {
			foreach (var q in buf) 
				Console.WriteLine(string.Format("{0} {1}", buf.Symbol, q));
		}

		public void OnTrade<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxTrade {
			foreach (var t in buf)
				Console.WriteLine(string.Format("{0} {1}", buf.Symbol, t));
		}

		public void OnOrder<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxOrder {
			foreach (var o in buf)
				Console.WriteLine(string.Format("{0} {1}", buf.Symbol, o));
		}

		public void OnProfile<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxProfile {
			foreach (var p in buf)
				Console.WriteLine(string.Format("{0} {1}", buf.Symbol, p));
		}

		public void OnFundamental<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxFundamental {
			foreach (var f in buf)
				Console.WriteLine(string.Format("{0} {1}", buf.Symbol, f));
		}

		public void OnTimeAndSale<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxTimeAndSale {
			foreach (var ts in buf)
				Console.WriteLine(string.Format("{0} {1}", buf.Symbol, ts));
		}

		#endregion
	}
}
