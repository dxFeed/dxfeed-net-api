/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_events_sample
{
    /// <summary>
    /// This sample class demonstrates subscription to events.
    /// The sample configures via command line, subscribes to events and prints received data.
    /// </summary>
    class Program
    {
        private const int HostIndex = 0;
        private const int EventIndex = 1;
        private const int SymbolIndex = 2;
        private const int DateIndex = 3;

        private static void OnDisconnect(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine(
                    "Usage: dxf_events_sample <host:port> <event> <symbol> [<date>]\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    event     - any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary,\n" +
                    "                TradeETH,SpreadOrder}\n" +
                    "    symbol    - IBM, MSFT, ...\n\n" +
                    "    date      - date of time series event in the format YYYY-MM-DD (optional)\n" +
                    "example: dxf_events_sample demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST\n" +
                    "or: dxf_events_sample demo.dxfeed.com:7300 TimeAndSale MSFT,IBM 2016-10-10\n"
                );
                return;
            }

            var address = args[HostIndex];

            EventType events;
            if (!Enum.TryParse(args[EventIndex], true, out events))
            {
                Console.WriteLine("Unsupported event type: " + args[1]);
                return;
            }

            string[] symbols = args[SymbolIndex].Split(',');

            Console.WriteLine(string.Format("Connecting to {0} for [{1}] on [{2}] ...",
                address, events, string.Join(", ", symbols)));

            try
            {
                NativeTools.InitializeLogging("log.log", true, true);
                using (var con = new NativeConnection(address, OnDisconnect))
                {
                    using (var s = args.Length > DateIndex 
                        ? con.CreateSubscription(events, DateTime.Parse(args[DateIndex]), new EventListener())
                        : con.CreateSubscription(events, new EventListener()))
                    {
                        s.AddSymbols(symbols);

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            }
            catch (DxException dxException)
            {
                Console.WriteLine("Native exception occured: " + dxException.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception occured: " + exc.Message);
            }
        }
    }
}
