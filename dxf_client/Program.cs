#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.connection;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native;

namespace dxf_client {
    public class SnapshotPrinter :
        IDxOrderSnapshotListener,
        IDxCandleSnapshotListener,
        IDxTimeAndSaleSnapshotListener,
        IDxSpreadOrderSnapshotListener,
        IDxGreeksSnapshotListener,
        IDxSeriesSnapshotListener {
        private readonly int recordsPrintLimit;

        public SnapshotPrinter(int recordsPrintLimit) {
            this.recordsPrintLimit = recordsPrintLimit;
        }

        private void PrintSnapshot<TE>(IDxEventBuf<TE> buf) {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}",
                buf.EventType, buf.Symbol, buf.Size));

            var count = 0;
            foreach (var o in buf) {
                Console.WriteLine($"   {{ {o} }}");

                if (++count < recordsPrintLimit || recordsPrintLimit == 0) continue;

                Console.WriteLine($"   {{ ... {buf.Size - count} records left ...}}");

                break;
            }
        }

        #region Implementation of IDxOrderSnapshotListener

        /// <summary>
        /// On Order snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            PrintSnapshot(buf);
        }

        #endregion //IDxOrderSnapshotListener

        #region Implementation of IDxCandleSnapshotListener

        /// <summary>
        /// On Candle snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle {
            PrintSnapshot(buf);
        }

        #endregion //IDxCandleSnapshotListener

        #region Implementation of IDxTimeAndSaleSnapshotListener

        /// <summary>
        /// On TimeAndSale snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale {
            PrintSnapshot(buf);
        }

        #endregion //IDxTimeAndSaleSnapshotListener

        #region Implementation of IDxSpreadOrderSnapshotListener

        /// <summary>
        /// On SpreadOrder snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder {
            PrintSnapshot(buf);
        }

        #endregion

        #region Implementation of IDxGreeksSnapshotListener

        public void OnGreeksSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks {
            PrintSnapshot(buf);
        }

        #endregion

        #region Implementation of IDxSeriesSnapshotListener

        public void OnSeriesSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries {
            PrintSnapshot(buf);
        }

        #endregion
    }

    internal class InputParam<T> {
        private T value;

        private InputParam() {
            IsSet = false;
        }

        public InputParam(T defaultValue) : this() {
            value = defaultValue;
        }

        public bool IsSet { get; private set; }

        public T Value {
            get { return value; }
            set {
                this.value = value;
                IsSet = true;
            }
        }
    }

    class Program {
        private const int DEFAULT_RECORDS_PRINT_LIMIT = 7;
        private const int HOST_INDEX = 0;
        private const int EVENT_INDEX = 1;
        private const int SYMBOL_INDEX = 2;

        private static bool TryParseDateTimeParam(string stringParam, InputParam<DateTime?> param) {
            DateTime dateTimeValue;

            if (!DateTime.TryParse(stringParam, out dateTimeValue)) {
                return false;
            }

            param.Value = dateTimeValue;

            return true;
        }

        private static bool TryParseSnapshotParam(string stringParam, InputParam<bool> param) {
            if (!stringParam.ToLower().Equals("snapshot")) {
                return false;
            }

            param.Value = true;

            return true;
        }

        private static void TryParseSourcesParam(string stringParam, InputParam<string[]> param) {
            param.Value = stringParam.Split(',');
        }

        private static bool TryParseRecordsPrintLimitParam(string paramTagString, string paramString,
            InputParam<int> param) {
            if (!paramTagString.Equals("-l")) {
                return false;
            }

            int newRecordsPrintLimit;

            if (!int.TryParse(paramString, out newRecordsPrintLimit)) {
                return false;
            }

            param.Value = newRecordsPrintLimit;

            return true;
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParam<string> param) {
            if (!paramTagString.Equals(tag)) {
                return false;
            }

            param.Value = paramString;

            return true;
        }

        static void Main(string[] args) {
            if (args.Length < 3 || args.Length > 10) {
                Console.WriteLine(
                    "Usage: dxf_client <host:port> <event> <symbol> [<date>] [<source>] [snapshot] [-l <records_print_limit>] [-T <token>]\n" +
                    "where\n" +
                    "    host:port - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    event     - Any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary,\n" +
                    "                TradeETH,SpreadOrder,Candle,Greeks,TheoPrice,Underlying,Series,\n" +
                    "                Configuration}\n" +
                    "    symbol    - a) IBM, MSFT, ... ; * - all symbols\n" +
                    "                b) if it is Candle event you can specify candle symbol\n" +
                    "                   attribute by string, for example: XBT/USD{=d}\n" +
                    "    date      - The date of time series event in the format YYYY-MM-DD (optional)\n" +
                    "    source    - Used only for Order or MarketMaker subscription:\n" +
                    "                a) OPTIONAL source for order events is any combination of:\n" +
                    "                   NTV, BYX, BZX, DEA, ISE, DEX, IST;\n" +
                    "                b) source for Order snapshot can be one of following: NTV,\n" +
                    "                   BYX, BZX, DEA, ISE, DEX, IST\n" +
                    "                c) source for MarketMaker snapshot, can be COMPOSITE_ASK\n" +
                    "                   or COMPOSITE_BID\n" +
                    "    snapshot  - Use keyword 'snapshot' for create snapshot subscription,\n" +
                    "                otherwise leave empty\n" +
                    $"    -l <records_print_limit> - The number of displayed records (0 - unlimited, default: {DEFAULT_RECORDS_PRINT_LIMIT})\n" +
                    "    -T <token>               - The authorization token\n\n" +
                    "examples:\n" +
                    "  events: dxf_client demo.dxfeed.com:7300 Quote,Trade MSFT.TEST,IBM.TEST\n" +
                    "  order: dxf_client demo.dxfeed.com:7300 Order MSFT.TEST,IBM.TEST NTV,IST\n" +
                    "  candle: dxf_client demo.dxfeed.com:7300 Candle XBT/USD{=d} 2016-10-10\n" +
                    "  underlying: dxf_client demo.dxfeed.com:7300 Underlyingn AAPL\n" +
                    "  series: dxf_client demo.dxfeed.com:7300 Series AAPL\n" +
                    "  order snapshot: dxf_client demo.dxfeed.com:7300 Order AAPL NTV snapshot\n" +
                    "  order snapshot: dxf_client demo.dxfeed.com:7300 Order AAPL NTV snapshot -l 0\n" +
                    "  market maker snapshot: dxf_client demo.dxfeed.com:7300 Order AAPL COMPOSITE_BID snapshot\n" +
                    "  market maker snapshot: dxf_client demo.dxfeed.com:7300 Order AAPL COMPOSITE_BID snapshot -l 3\n" +
                    "  candle snapshot: dxf_client demo.dxfeed.com:7300 Candle XBT/USD{=d} 2016-10-10 snapshot\n" + 
                    "  candle snapshot: dxf_client demo.dxfeed.com:7300 Candle XBT/USD{=d} 2016-10-10 snapshot -l 10\n" +
                    "  candle snapshot: dxf_client demo.dxfeed.com:7300 Candle XBT/USD{=d,pl=0.5} 2016-10-10 snapshot -l 10\n"
                );
                return;
            }

            var address = args[HOST_INDEX];

            EventType events;
            if (!Enum.TryParse(args[EVENT_INDEX], true, out events)) {
                Console.WriteLine($"Unsupported event type: {args[EVENT_INDEX]}");
                return;
            }

            var symbols = args[SYMBOL_INDEX];
            var sources = new InputParam<string[]>(new string[] { });
            var isSnapshot = new InputParam<bool>(false);
            var dateTime = new InputParam<DateTime?>(null);
            var recordsPrintLimit = new InputParam<int>(DEFAULT_RECORDS_PRINT_LIMIT);
            var token = new InputParam<string>(null);

            for (var i = SYMBOL_INDEX + 1; i < args.Length; i++) {
                if (!dateTime.IsSet && TryParseDateTimeParam(args[i], dateTime))
                    continue;
                if (!isSnapshot.IsSet && TryParseSnapshotParam(args[i], isSnapshot))
                    continue;
                if (!recordsPrintLimit.IsSet && i < args.Length - 1 &&
                    TryParseRecordsPrintLimitParam(args[i], args[i + 1], recordsPrintLimit)) {
                    i++;

                    continue;
                }

                if (!token.IsSet && i < args.Length - 1 &&
                    TryParseTaggedStringParam("-T", args[i], args[i + 1], token)) {
                    i++;
                    
                    continue;
                }

                if (!sources.IsSet)
                    TryParseSourcesParam(args[i], sources);
            }

            var snapshotString = isSnapshot.Value ? " snapshot" : string.Empty;
            var timeSeriesString = dateTime.IsSet && !isSnapshot.Value ? " time-series" : string.Empty;

            Console.WriteLine(
                $"Connecting to {address} for [{events}{snapshotString}]{timeSeriesString} on [{symbols}] ...");

            NativeTools.InitializeLogging("dxf_client.log", true, true);

            var listener = new EventPrinter();
            using (var con = (token.IsSet)
                ? new NativeConnection(address, token.Value, DisconnectHandler, ConnectionStatusChangeHandler)
                : new NativeConnection(address, DisconnectHandler, ConnectionStatusChangeHandler)) {
                IDxSubscription s = null;
                try {
                    if (isSnapshot.Value) {
                        s = con.CreateSnapshotSubscription(events, dateTime.Value,
                            new SnapshotPrinter(recordsPrintLimit.Value));
                    } else if (dateTime.IsSet) {
                        s = con.CreateSubscription(events, dateTime.Value, listener);
                    } else {
                        s = con.CreateSubscription(events, listener);
                    }

                    if (events.HasFlag(EventType.Order) && sources.Value.Length > 0)
                        s.SetSource(sources.Value);

                    if (events == EventType.Candle) {
                        var candleSymbol = CandleSymbol.ValueOf(symbols);
                        s.AddSymbol(candleSymbol);
                    } else {
                        s.AddSymbols(symbols.Split(','));
                    }

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                } catch (DxException dxException) {
                    Console.WriteLine($"Native exception occured: {dxException.Message}");
                } catch (Exception exc) {
                    Console.WriteLine($"Exception occured: {exc.Message}");
                } finally {
                    s?.Dispose();
                }
            }
        }

        private static void ConnectionStatusChangeHandler(IDxConnection connection, ConnectionStatus oldStatus, ConnectionStatus newStatus) {
            if (newStatus == ConnectionStatus.Connected) {
                Console.WriteLine("Connected to {0}", connection.ConnectedAddress);
            } else if (newStatus == ConnectionStatus.Authorized) {
                Console.WriteLine("Authorized");
            }
        }

        private static void DisconnectHandler(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }
    }
}