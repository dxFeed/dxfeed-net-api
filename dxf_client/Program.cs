using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.native;

namespace dxf_client {
	class Program {
		static void Main(string[] args) {
			var listener = new EventPrinter();
			using(var con = new NativeConnection()) {
				con.Connect("demo.dxfeed.com:7300");
				var s = con.CreateSubscription(
					EventType.Fundamental|EventType.Profile|EventType.Order|EventType.Quote|EventType.TimeAndSale|EventType.Trade,
					//EventType.Profile,
					listener);
				Console.WriteLine("Press enter to stop");
				s.AddSymbols("IBM.TEST", "MSFT.TEST");
				Console.ReadLine();
			}
		}
	}

	class EventPrinter : IDxFeedListener {
		#region Implementation of IDxFeedListener

		public void OnQuote<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxQuote {
			Console.WriteLine("Got quote buffer on " + buf.Symbol + " of size " + buf.Size);
			foreach (var q in buf) {
				Console.WriteLine(q);
			}
		}

		public void OnTrade<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxTrade {
			Console.WriteLine("Got " + buf.Size + "trades for " + buf.Symbol);
			foreach (var t in buf) {
				Console.WriteLine(t);
			}
		}

		public void OnOrder<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxOrder {
			Console.WriteLine("Got orders for " + buf.Symbol);
			foreach (var o in buf) {
				Console.WriteLine(o);
			}
		}

		public void OnProfile<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxProfile {
			Console.WriteLine("Got profile for " + buf.Symbol);
			foreach (var p in buf) {
				Console.WriteLine(p);
			}
		}

		public void OnFundamental<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxFundamental {
			Console.WriteLine("Got Fundamental for " + buf.Symbol);
			foreach (var f in buf) {
				Console.WriteLine(f);
			}
		}

		public void OnTimeAndSale<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxTimeAndSale {
			Console.WriteLine("Got T&S for " + buf.Symbol);
			foreach (var ts in buf) {
				Console.WriteLine(ts);
			}
		}

		#endregion
	}
}
