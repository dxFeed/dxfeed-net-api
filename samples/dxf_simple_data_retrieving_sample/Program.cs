#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Threading;
using com.dxfeed.api.data;
using com.dxfeed.native;

namespace dxf_simple_data_retrieving_sample
{
    /// <summary>
    ///     This sample class demonstrates simple retrieving the event data.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine(
                    "Usage: dxf_simple_data_retrieving_sample <host:port>\n" +
                    "where\n" +
                    "    host:port - The address of dxfeed server (demo.dxfeed.com:7300)\n\n" +
                    "example: dxf_simple_data_retrieving_sample demo.dxfeed.com:7300\n\n"
                );
                return;
            }

            var address = args[0];
            var from = DateTime.Now.Subtract(TimeSpan.FromDays(5));
            var from2 = DateTime.Now.Subtract(TimeSpan.FromDays(15));
            var to = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            NativeTools.InitializeLogging("dxf_simple_data_retrieving_sample.log", true, true);

            var connection = new NativeConnection(address, con => { });

            // With default timeout
            var tns = connection.GetDataForPeriod(EventType.TimeAndSale, "AAPL", from, to).Result;

            // With custom timeout
            var tns2 = connection.GetDataForPeriod(EventType.TimeAndSale, "AAPL", from2, to,
                TimeSpan.FromSeconds(7)).Result;

            // With cancellation token
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var candles =
                connection.GetDataForPeriod(EventType.Candle, "AAPL&Q{=1m}", from, to, cancellationToken);

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            var candlesResult = candles.Result;

            Console.WriteLine("--------------------");
            Console.WriteLine($"AAPL TimeAndSales from {from} to {to}:");
            tns.ForEach(Console.WriteLine);
            Console.WriteLine("--------------------");
            Console.WriteLine($"AAPL TimeAndSales from {from2} to {to}:");
            tns2.ForEach(Console.WriteLine);
            Console.WriteLine("--------------------");
            Console.WriteLine($"AAPL&Q{{=1m}} Candles from {from} to {to}:");
            candlesResult.ForEach(Console.WriteLine);
            Console.WriteLine("--------------------");

            connection.Dispose();
        }
    }
}