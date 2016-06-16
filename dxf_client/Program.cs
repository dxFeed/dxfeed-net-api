using System;
using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native;

namespace dxf_client {

	public class SnapshotPrinter : IDxSnapshotListener {

		private const int RECORDS_PRINT_LIMIT = 7;

		#region Implementation of IDxSnapshotListener

		public void OnOrderSnapshot<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxOrder {
			Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}",
				buf.EventType, buf.Symbol, buf.Size));
			int count = 0;
			foreach (var o in buf) {
				Console.WriteLine(string.Format("   {{ {0} }}", o));
				if (++count >= RECORDS_PRINT_LIMIT) {
					Console.WriteLine(string.Format("   {{ ... {0} records left ...}}", buf.Size - count));
					break;
				}
			}
		}

		public void OnCandleSnapshot<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxOrder {

		}

		#endregion
	}

	class Program {

		static bool TryParseSnapshotParam(string param, out bool isSnapshot) {
			isSnapshot = param.ToLower().Equals("snapshot");
			return isSnapshot;
		}

		static void TryParseSourcesParam(string param, out string[] sources) {
			sources = param.Split(',');
		}

		static void Main(string[] args) {
			if (args.Length < 3 || args.Length > 5) {
				Console.WriteLine(
					"Usage: dxf_client <host:port> <event> <symbol> [<source>] [snapshot]\n" +
					"where\n" +
					"    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
					"    event     - any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary}\n" +
					"    symbol    - IBM, MSFT, ...\n" +
					"    source    - order sources NTV, BYX, BZX, DEA, DEX, IST, ISE,... (can be empty)\n" +
					"    snapshot  - use keyword 'snapshot' for create snapshot subscription, otherwise leave empty\n\n" +
					"example: dxf_client demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST NTV,IST"
				);
				return;
			}

			var address = args[0];
			var symbols = args[2].Split(',');
			string[] sources = new string[0];
			bool isSnapshot = false;
			if (args.Length == 4) {
				string param = args[3];
				if (!TryParseSnapshotParam(param, out isSnapshot))
					TryParseSourcesParam(param, out sources);
			} else if (args.Length == 5) {
				TryParseSourcesParam(args[3], out sources);
				TryParseSnapshotParam(args[4], out isSnapshot);
			}
			EventType events;
			if (!Enum.TryParse(args[1], true, out events)) {
				Console.WriteLine("Unsupported event type: " + args[1]);
				return;
			}

			Console.WriteLine(string.Format("Connecting to {0} for [{1}] on [{2}] ...", address, events, string.Join(", ", symbols)));

            // NativeTools.InitializeLogging("dxf_client.log", true, true);
			var listener = new EventPrinter();
			using (var con = new NativeConnection(address, OnDisconnect)) {
				var s = isSnapshot ? con.CreateSnapshot(events, 0, new SnapshotPrinter()) : con.CreateSubscription(events, listener);
				if (sources.Length > 0) {
					s.SetSource(sources);
				}
				Console.WriteLine("Press enter to stop");
				s.AddSymbols(symbols);
				Console.ReadLine();
			}
		}

		private static void OnDisconnect(IDxConnection con) {
			Console.WriteLine("Disconnected");
		}
	}
}
