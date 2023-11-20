#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Threading;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.api.plb;
using com.dxfeed.native;

namespace dxf_price_level_book_sample
{
    internal class Program
    {
        private const int HostIndex = 0;
        private const int SymbolIndex = 1;
        private const int SourceIndex = 2;
        private const int LevelsNumberIndex = 3;

        private static void DisconnectHandler(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParam<string> param)
        {
            if (!paramTagString.Equals(tag)) return false;

            param.Value = paramString;

            return true;
        }

        private static void ShowHelp()
        {
            Console.WriteLine(
                "Usage: dxf_price_level_book_sample <host:port> <symbol> <source> <levels number> [-T <token>] [-p] [-b] [-q]\n" +
                "where\n" +
                "    host:port       - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                "    symbol          - The price level book symbol (IBM, AAPL etc)\n" +
                "    source          - One order source, e.g. one of the: NTV, BYX, BZX, DEA etc. or AGGREGATE_ASK|BID\n" +
                "    <levels number> - The The number of PLB price levels (0 -- all)\n" +
                "    -T <token>      - The authorization token\n" +
                "    -p              - Enables the data transfer logging\n" +
                "    -b              - Enables the server's heartbeat logging to console\n" +
                "    -q              - Quiet mode (do not print price levels)\n\n" +
                "examples: \n" +
                "dxf_price_level_book_sample demo.dxfeed.com:7300 MSFT NTV 5\n" +
                "dxf_price_level_book_sample demo.dxfeed.com:7300 AAPL AGGREGATE_ASK 0\n" +
                "\n"
            );
        }

        private static void Main(string[] args)
        {
            if (args.Length < 4 || (args.Length > 0 && args[0].Equals("-h")))
            {
                ShowHelp();

                return;
            }

            var address = args[HostIndex];
            var symbol = args[SymbolIndex];
            var source = args[SourceIndex];
            var levelsNumberString = args[LevelsNumberIndex];
            int levelsNumber;

            if (!int.TryParse(levelsNumberString, out levelsNumber))
            {
                Console.Error.WriteLine($"Can't parse the <levels number> = '{levelsNumberString}'");

                return;
            }

            var token = new InputParam<string>(null);
            var logDataTransferFlag = false;
            var logServerHeartbeatsFlag = false;
            var quiteMode = false;

            for (var i = SymbolIndex + 1; i < args.Length; i++)
            {
                if (!token.IsSet && i < args.Length - 1 &&
                    TryParseTaggedStringParam("-T", args[i], args[i + 1], token))
                {
                    i++;

                    continue;
                }

                if (logDataTransferFlag == false && args[i].Equals("-p"))
                {
                    logDataTransferFlag = true;

                    continue;
                }

                if (logServerHeartbeatsFlag == false && args[i].Equals("-b"))
                {
                    logServerHeartbeatsFlag = true;

                    continue;
                }

                if (quiteMode == false && args[i].Equals("-q"))
                {
                    quiteMode = true;

                    continue;
                }
            }

            Console.WriteLine($"Connecting to {address} on {symbol}, sources - {source} ...");

            try
            {
                NativeTools.InitializeLogging("dxf_price_level_book_sample.log", true, true, logDataTransferFlag);
                using (var con = token.IsSet
                           ? new NativeConnection(address, token.Value, DisconnectHandler)
                           : new NativeConnection(address, DisconnectHandler))
                {
                    if (logServerHeartbeatsFlag)
                    {
                        con.SetOnServerHeartbeatHandler((connection, time, lagMark, rtt) =>
                        {
                            Console.Error.WriteLine(
                                $"##### Server time (UTC) = {time}, Server lag = {lagMark} us, RTT = {rtt} us #####");
                        });
                    }

                    using (var plb = con.CreatePriceLevelBook(symbol, source, levelsNumber))
                    {
                        var priceLevelBookPrinter = quiteMode
                            ? (IPriceLevelBookPrinter) new DummyPriceLevelBookPrinter()
                            : new PriceLevelBookPrinter();

                        plb.SetHandlers(priceLevelBookPrinter, priceLevelBookPrinter, priceLevelBookPrinter);

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            }
            catch (DxException dxException)
            {
                Console.WriteLine("Native exception occurred: " + dxException.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception occurred: " + exc.Message);
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