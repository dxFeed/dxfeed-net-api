#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.connection;
using com.dxfeed.api.data;
using com.dxfeed.api.extras;
using com.dxfeed.native;

namespace dxf_client
{
    internal class InputParam<T>
    {
        private T value;

        private InputParam()
        {
            IsSet = false;
        }

        public InputParam(T defaultValue) : this()
        {
            value = defaultValue;
        }

        public bool IsSet { get; private set; }

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
                IsSet = true;
            }
        }
    }

    internal class Program
    {
        private const int DefaultRecordsPrintLimit = 7;
        private const int HostIndex = 0;
        private const int EventIndex = 1;
        private const int SymbolIndex = 2;

        private static bool TryParseDateTimeParam(string stringParam, InputParam<DateTime?> param)
        {
            DateTime dateTimeValue;

            if (!DateTime.TryParse(stringParam, out dateTimeValue)) return false;

            param.Value = dateTimeValue;

            return true;
        }

        private static bool TryParseSnapshotParam(string stringParam, InputParam<bool> param)
        {
            if (!stringParam.ToLower().Equals("snapshot")) return false;

            param.Value = true;

            return true;
        }

        private static void TryParseSourcesParam(string stringParam, InputParam<string[]> param)
        {
            param.Value = stringParam.Split(',');
        }

        private static bool TryParseRecordsPrintLimitParam(string paramTagString, string paramString,
            InputParam<int> param)
        {
            if (!paramTagString.Equals("-l")) return false;

            int newRecordsPrintLimit;

            if (!int.TryParse(paramString, out newRecordsPrintLimit)) return false;

            param.Value = newRecordsPrintLimit;

            return true;
        }

        private static bool TryParseEventSubscriptionFlagParam(string tag, string paramTagString, string paramString,
            InputParam<EventSubscriptionFlag> param)
        {
            if (!paramTagString.Equals(tag)) return false;

            if (paramString.Equals("ticker", StringComparison.InvariantCultureIgnoreCase))
                param.Value = EventSubscriptionFlag.ForceTicker;
            else if (paramString.Equals("stream", StringComparison.InvariantCultureIgnoreCase))
                param.Value = EventSubscriptionFlag.ForceStream;
            else if (paramString.Equals("history", StringComparison.InvariantCultureIgnoreCase))
                param.Value = EventSubscriptionFlag.ForceHistory;
            else
                return false;

            return true;
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParam<string> param)
        {
            if (!paramTagString.Equals(tag)) return false;

            param.Value = paramString;

            return true;
        }

        private static void Main(string[] args)
        {
            if (args.Length < 3 || (args.Length > 0 && args[0].Equals("-h")))
            {
                Console.WriteLine(
                    "Usage: dxf_client <host:port>|<path> <event> <symbol> [<date>] [<source>] [snapshot] [-l <records_print_limit>] [-T <token>] [-s <subscr_data>] [-p] [-b] [-q]\n" +
                    "where\n" +
                    "    host:port - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    path      - The path to file with candle data (non zipped Candle Web Service output) or `tape` file\n" +
                    "    event     - Any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary,\n" +
                    "                TradeETH,SpreadOrder,Candle,Greeks,TheoPrice,Underlying,Series,\n" +
                    "                Configuration}\n" +
                    "    symbol    - a) IBM, MSFT, ... ; * - all symbols\n" +
                    "                b) if it is Candle event you can specify candle symbol\n" +
                    "                   attribute by string, for example: XBT/USD{=d}\n" +
                    "    date      - The date of time series event in the format YYYY-MM-DD (optional)\n" +
                    "    source    - Used only for Order or MarketMaker subscription:\n" +
                    "                a) OPTIONAL source for order events is any combination of:\n" +
                    "                   NTV,ntv,NFX,ESPD,XNFI,ICE,ISE,DEA,DEX,BYX,BZX,BATE,CHIX,CEUX,\n" +
                    "                   BXTR,IST,BI20,ABE,FAIR,GLBX,glbx,ERIS,XEUR,xeur,CFE,C2OX,SMFE," +
                    "                   smfe,iex,MEMX,memx;\n" +
                    "                b) source for Order snapshot can be one of following: " +
                    "                   NTV,ntv,NFX,ESPD,XNFI,ICE,ISE,DEA,DEX,BYX,BZX,BATE,CHIX,CEUX,\n" +
                    "                   BXTR,IST,BI20,ABE,FAIR,GLBX,glbx,ERIS,XEUR,xeur,CFE,C2OX,SMFE," +
                    "                   smfe,iex,MEMX,memx;\n" +
                    "                c) source for MarketMaker snapshot, can be AGGREGATE_ASK\n" +
                    "                   or AGGREGATE_BID\n" +
                    "    snapshot  - Use keyword 'snapshot' for create snapshot subscription,\n" +
                    "                otherwise leave empty\n" +
                    $"    -l <records_print_limit> - The number of displayed records (0 - unlimited, default: {DefaultRecordsPrintLimit})\n" +
                    "    -T <token>               - The authorization token\n\n" +
                    "    -s <subscr_data>         - The subscription data: ticker|TICKER, stream|STREAM, history|HISTORY\n" +
                    "    -p                       - Enables the data transfer logging\n" +
                    "    -b                       - Enables the server's heartbeat logging to console\n" +
                    "    -q                       - Quiet mode (do not print events and snapshots)\n\n" +
                    "examples:\n" +
                    "  events: dxf_client demo.dxfeed.com:7300 Quote,Trade MSFT.TEST,IBM.TEST\n" +
                    "  events: dxf_client demo.dxfeed.com:7300 Quote,Trade MSFT.TEST,IBM.TEST -s stream\n" +
                    "  order: dxf_client demo.dxfeed.com:7300 Order MSFT.TEST,IBM.TEST NTV,IST\n" +
                    "  candle: dxf_client demo.dxfeed.com:7300 Candle XBT/USD{=d} 2016-10-10\n" +
                    "  underlying: dxf_client demo.dxfeed.com:7300 Underlying AAPL\n" +
                    "  series: dxf_client demo.dxfeed.com:7300 Series AAPL\n" +
                    "  order snapshot: dxf_client demo.dxfeed.com:7300 Order AAPL NTV snapshot\n" +
                    "  order snapshot: dxf_client demo.dxfeed.com:7300 Order AAPL NTV snapshot -l 0\n" +
                    "  market maker snapshot: dxf_client demo.dxfeed.com:7300 Order AAPL AGGREGATE_BID snapshot\n" +
                    "  market maker snapshot: dxf_client demo.dxfeed.com:7300 Order AAPL AGGREGATE_BID snapshot -l 3\n" +
                    "  candle snapshot: dxf_client demo.dxfeed.com:7300 Candle XBT/USD{=d} 2016-10-10 snapshot\n" +
                    "  candle snapshot: dxf_client demo.dxfeed.com:7300 Candle XBT/USD{=d} 2016-10-10 snapshot -l 10\n" +
                    "  candle snapshot: dxf_client demo.dxfeed.com:7300 Candle XBT/USD{=d,pl=0.5} 2016-10-10 snapshot -l 10\n" +
                    "  tape file: dxf_client ./tape_file Order,TimeAndSale AAPL\n" +
                    "  candle data: dxf_client ./candledata.bin Candle AIV{=d}\n"
                );
                return;
            }

            var address = args[HostIndex];

            EventType events;
            if (!Enum.TryParse(args[EventIndex], true, out events))
            {
                Console.WriteLine($"Unsupported event type: {args[EventIndex]}");
                return;
            }

            var symbols = args[SymbolIndex];
            var sources = new InputParam<string[]>(new string[] { });
            var isSnapshot = new InputParam<bool>(false);
            var dateTime = new InputParam<DateTime?>(null);
            var recordsPrintLimit = new InputParam<int>(DefaultRecordsPrintLimit);
            var token = new InputParam<string>(null);
            var subscriptionData = new InputParam<EventSubscriptionFlag>(EventSubscriptionFlag.Default);
            var logDataTransferFlag = false;
            var logServerHeartbeatsFlag = false;
            var quiteMode = false;

            for (var i = SymbolIndex + 1; i < args.Length; i++)
            {
                if (!dateTime.IsSet && TryParseDateTimeParam(args[i], dateTime))
                    continue;
                if (!isSnapshot.IsSet && TryParseSnapshotParam(args[i], isSnapshot))
                    continue;
                if (!recordsPrintLimit.IsSet && i < args.Length - 1 &&
                    TryParseRecordsPrintLimitParam(args[i], args[i + 1], recordsPrintLimit))
                {
                    i++;

                    continue;
                }

                if (!token.IsSet && i < args.Length - 1 &&
                    TryParseTaggedStringParam("-T", args[i], args[i + 1], token))
                {
                    i++;

                    continue;
                }

                if (!subscriptionData.IsSet && i < args.Length - 1 &&
                    TryParseEventSubscriptionFlagParam("-s", args[i], args[i + 1], subscriptionData))
                {
                    i++;

                    continue;
                }

                if (!logDataTransferFlag && args[i].Equals("-p"))
                {
                    logDataTransferFlag = true;

                    continue;
                }

                if (!logServerHeartbeatsFlag && args[i].Equals("-b"))
                {
                    logServerHeartbeatsFlag = true;

                    continue;
                }

                if (!quiteMode && args[i].Equals("-q"))
                {
                    quiteMode = true;

                    continue;
                }

                if (!sources.IsSet)
                    TryParseSourcesParam(args[i], sources);
            }

            var snapshotString = isSnapshot.Value ? " snapshot" : string.Empty;
            var timeSeriesString = dateTime.IsSet && !isSnapshot.Value ? " time-series" : string.Empty;

            Console.WriteLine(
                $"Connecting to {address} for [{events}{snapshotString}]{timeSeriesString} on [{symbols}] ...");

            //NativeTools.LoadConfigFromString("network.heartbeatPeriod = 11\n");
            NativeTools.LoadConfigFromString("logger.level = \"debug\"\n");
            NativeTools.InitializeLogging("dxf_client.log", true, true, logDataTransferFlag);

            var eventPrinter = quiteMode ? (IEventPrinter) new DummyEventPrinter() : new EventPrinter();

            using (var con = token.IsSet
                       ? new NativeConnection(address, token.Value, DisconnectHandler, ConnectionStatusChangeHandler)
                       : new NativeConnection(address, DisconnectHandler, ConnectionStatusChangeHandler))
            {
                if (logServerHeartbeatsFlag)
                {
                    con.SetOnServerHeartbeatHandler((connection, time, lagMark, rtt) =>
                    {
                        Console.Error.WriteLine(
                            $"##### Server time (UTC) = {time}, Server lag = {lagMark} us, RTT = {rtt} us #####");
                    });
                }

                IDxSubscription s = null;
                try
                {
                    if (dateTime.IsSet && (events & (EventType.Order | EventType.SpreadOrder)) != 0)
                    {
                        Console.Error.WriteLine(
                            "Date and event type Order or SpreadOrder cannot be used for subscription");
                    }

                    if (isSnapshot.Value)
                    {
                        s = con.CreateSnapshotSubscription(events, dateTime.Value,
                            quiteMode
                                ? (ISnapshotPrinter) new DummySnapshotPrinter()
                                : new SnapshotPrinter(recordsPrintLimit.Value));
                    }
                    else if (dateTime.IsSet)
                        s = subscriptionData.IsSet
                            ? con.CreateSubscription(events, dateTime.Value, subscriptionData.Value, eventPrinter)
                            : con.CreateSubscription(events, dateTime.Value, eventPrinter);
                    else
                        s = subscriptionData.IsSet
                            ? con.CreateSubscription(events, subscriptionData.Value, eventPrinter)
                            : con.CreateSubscription(events, eventPrinter);

                    if (events.HasFlag(EventType.Order) && sources.Value.Length > 0)
                        s.SetSource(sources.Value);

                    if (events == EventType.Candle)
                    {
                        var candleSymbol = CandleSymbol.ValueOf(symbols);
                        s.AddSymbol(candleSymbol);
                    }
                    else
                    {
                        s.AddSymbols(symbols.Split(','));
                    }

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                }
                catch (DxException dxException)
                {
                    Console.WriteLine($"Native exception occurred: {dxException.Message}");
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Exception occurred: {exc.Message}");
                }
                finally
                {
                    s.Dispose();
                }
            }
        }

        private static void ConnectionStatusChangeHandler(IDxConnection connection, ConnectionStatus oldStatus,
            ConnectionStatus newStatus)
        {
            switch (newStatus)
            {
                case ConnectionStatus.Connected:
                    Console.WriteLine("Connected to {0}", connection.ConnectedAddress);
                    break;
                case ConnectionStatus.Authorized:
                    Console.WriteLine("Authorized");
                    break;
                default:
                    return;
            }
        }

        private static void DisconnectHandler(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }
    }
}