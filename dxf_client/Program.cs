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
				var s = (NativeSubscription) con.CreateSubscribtion(
					EventType.Quote|EventType.Profile|EventType.Trade,
					listener);
				Console.WriteLine("Press enter to stop");
				s.AddSymbol("IBM.TEST");
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
			
		}

		public void OnProfile<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxProfile {
			Console.WriteLine("Got profile for " + buf.Symbol);
			foreach (var p in buf) {
				Console.WriteLine(p.Description);
			}
		}

		public void OnTimeAndSale<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxTimeAndSale {
			
		}

		#endregion
	}
}
