#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using com.dxfeed.native;

namespace dxf_tns_data_retrieving_sample
{
    /// <summary>
    ///     This sample class demonstrates how to retrieve TnS data from the candle web service.
    /// </summary>
    internal static class Program
    {
        private static void ShowUsage()
        {
            Console.WriteLine(
                "Usage: dxf_tns_data_retrieving_sample <host:port> [<login> <password>|<token>] <symbols> <from-date-time> <to-date-time>\n" +
                "where\n" +
                "    host:port - The address of candle web service https://tools.dxfeed.com/candledata-preview\n" +
                "    login     - The user login.\n" +
                "    password  - The user password.\n" +
                "    token     - The connection token.\n" +
                "    symbols   - The tns symbols, comma separated list (composite: AAPL, regional: AAPL&A..AAPL&Z). Example: 'IBM,AAPL&Q,AAPL'\n" +
                "    from-date-time - The UTC date\\time, inclusive, to request events from. (format: yyyyMMdd-HHmmss)\n" +
                "    to-date-time   - The UTC date\\time, inclusive, to request events to. (format: yyyyMMdd-HHmmss)\n" +
                "Example: dxf_candle_data_retrieving_sample https://tools.dxfeed.com/candledata-preview demo demo \"IBM,AAPL&Q\" 20210819-030000 20210823-100000\n\n"
            );
        }
        
        private static void Main(string[] args)
        {
            if (args.Length < 4 || args.Length > 6)
            {
                ShowUsage();
                
                return;
            }

            var address = args[0];
            string login = null;
            string password = null;
            string token = null;
            var symbolsIndex = 1;

            if (args.Length == 5)
            {
                token = args[1];
                symbolsIndex = 2;
            }
            else if (args.Length == 6)
            {
                login = args[1];
                password = args[2];
                symbolsIndex = 3;
            }

            var symbols = args[symbolsIndex].Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToList();

            if (symbols.Count == 0)
            {
                Console.Error.WriteLine("The symbols list is empty\n");
                ShowUsage();
                
                return;
            }

            DateTime fromDateTime;
            
            if (!DateTime.TryParseExact(args[symbolsIndex + 1], "yyyyMMdd-HHmmss",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out fromDateTime))
            {
                Console.Error.WriteLine("Can't parse the <from-date-time>\n");
                ShowUsage();
                
                return;
            }

            DateTime toDateTime;

            if (!DateTime.TryParseExact(args[symbolsIndex + 2], "yyyyMMdd-HHmmss",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out toDateTime))
            {
                Console.Error.WriteLine("Can't parse the <to-date-time>\n");
                ShowUsage();
                
                return;
            }

            NativeTools.InitializeLogging("dxf_candle_data_retrieving_sample.log", true, true);

            var con = !string.IsNullOrEmpty(login)
                ? new TimeAndSaleDataConnection(address, login, password)
                : new TimeAndSaleDataConnection(address, token);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var getTimeAndSaleDataResultTask = con.GetTimeAndSaleData(symbols,
                fromDateTime, toDateTime, cancellationToken);
            
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));

            var tnsResult = getTimeAndSaleDataResultTask.Result;
            const int LIMIT = 100; 

            foreach (var timeAndSales in tnsResult)
            {
                Console.WriteLine($"{timeAndSales.Key}, {timeAndSales.Value.Count} events:");
                foreach (var timeAndSale in timeAndSales.Value.Take(Math.Min(LIMIT, timeAndSales.Value.Count)))
                {
                    Console.WriteLine("  " + timeAndSale);
                }

                if (timeAndSales.Value.Count > LIMIT)
                {
                    Console.WriteLine($"... {timeAndSales.Value.Count - LIMIT} events were not displayed");
                }
            }
        }
    }
}