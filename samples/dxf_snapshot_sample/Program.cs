#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_snapshot_sample
{
    /// <summary>
    ///     This sample class demonstrates subscription to snapshots.
    ///     The sample configures via command line, subscribes to snapshot and prints received data.
    /// </summary>
    internal class Program
    {
        private const int DefaultRecordsPrintLimit = 7;
        private const int HostIndex = 0;
        private const int EventIndex = 1;
        private const int SymbolIndex = 2;
        private const int DefaultTime = 0;

        private static void DisconnectHandler(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
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

        private static void TryParseStringParam(string paramString,
            InputParam<string> param)
        {
            if (string.IsNullOrEmpty(paramString)) return;

            param.Value = paramString;
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
            if (args.Length < 3 || args.Length > 9)
            {
                Console.WriteLine(
                    "Usage: dxf_snapshot_sample <host:port> <event> <symbol> [<source>] [-l <records_print_limit>] [-T <token>] [-p]\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    event     - snapshot event Order, Candle, TimeAndSale, SpreadOrder,\n" +
                    "                Greeks, Series for MarketMaker see source parameter\n" +
                    "    symbol    - symbol string, it is allowed to use only one symbol\n" +
                    "                a) event symbol: IBM, MSFT, ...\n" +
                    "                b) candle symbol attribute: XBT/USD{=d},\n" +
                    "                   AAPL{=d,price=mark}, ...\n" +
                    "    source    - used only for Order or MarketMaker subscription,\n" +
                    "                also it is allowed to use only one source\n" +
                    "                a) source for Order, e.g. NTV, BYX, BZX, DEA, ISE, \n" +
                    "                   DEX, IST\n" +
                    "                b) source for MarketMaker, one of following: AGGREGATE_ASK\n" +
                    "                   or AGGREGATE_BID (default value for Order snapshots)\n" +
                    "                If source is not specified MarketMaker snapshot will be\n" +
                    "                subscribed by default.\n\n" +
                    $"    -l <records_print_limit> - The number of displayed records (0 - unlimited, default: {DefaultRecordsPrintLimit})\n" +
                    "    -T <token>               - The authorization token\n" +
                    "    -p                       - Enables the data transfer logging\n\n" +
                    "order example: dxf_snapshot_sample demo.dxfeed.com:7300 Order AAPL NTV\n" +
                    "market maker example:\n" +
                    "    dxf_snapshot_sample demo.dxfeed.com:7300 Order AAPL AGGREGATE_BID\n" +
                    "or just:\n" +
                    "    dxf_snapshot_sample demo.dxfeed.com:7300 Order AAPL\n" +
                    "candle example: dxf_snapshot_sample demo.dxfeed.com:7300 Candle XBT/USD{=d}"
                );
                return;
            }

            var address = args[HostIndex];
            var symbol = args[SymbolIndex];

            EventType eventType;
            if (!Enum.TryParse(args[EventIndex], true, out eventType) ||
                eventType != EventType.Order && eventType != EventType.Candle &&
                eventType != EventType.TimeAndSale && eventType != EventType.SpreadOrder &&
                eventType != EventType.Greeks && eventType != EventType.Series)
            {
                Console.WriteLine($"Unsupported event type: {args[EventIndex]}");
                return;
            }

            var source = new InputParam<string>(OrderSource.AGGREGATE_BID);
            var recordsPrintLimit = new InputParam<int>(DefaultRecordsPrintLimit);
            var token = new InputParam<string>(null);
            var logDataTransferFlag = false;

            for (var i = SymbolIndex + 1; i < args.Length; i++)
            {
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

                if (logDataTransferFlag == false && args[i].Equals("-p"))
                {
                    logDataTransferFlag = true;
                    i++;

                    continue;
                }

                if (!source.IsSet) TryParseStringParam(args[i], source);
            }

            if (eventType == EventType.Order)
            {
                if (source.Value.Equals(OrderSource.AGGREGATE_BID) || source.Value.Equals(OrderSource.AGGREGATE_ASK))
                    Console.WriteLine("Connecting to {0} for MarketMaker snapshot on {1}...", address, symbol);
                else
                    Console.WriteLine("Connecting to {0} for Order#{1} snapshot on {2}...", address, source.Value,
                        symbol);
            }
            else
            {
                Console.WriteLine("Connecting to {0} for {1} snapshot on {2}...", address, eventType, symbol);
            }

            try
            {
                NativeTools.InitializeLogging("dxf_snapshot_sample.log", true, true, logDataTransferFlag);
                using (var con = token.IsSet
                    ? new NativeConnection(address, token.Value, DisconnectHandler)
                    : new NativeConnection(address, DisconnectHandler))
                {
                    using (var s = con.CreateSnapshotSubscription(eventType, DefaultTime,
                        new SnapshotListener(recordsPrintLimit.Value)))
                    {
                        switch (eventType)
                        {
                            case EventType.Order:
                                s.AddSource(source.Value);
                                s.AddSymbol(symbol);
                                break;
                            case EventType.Candle:
                                s.AddSymbol(CandleSymbol.ValueOf(symbol));
                                break;
                            default:
                                s.AddSymbol(symbol);
                                break;
                        }

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            }
            catch (DxException dxException)
            {
                Console.WriteLine($"Native exception occurred: {dxException.Message}");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exception occurred: {exc.Message}");
            }
        }

        private class InputParam<T>
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
    }
}