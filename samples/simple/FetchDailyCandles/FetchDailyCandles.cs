#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.dxfeed.sample._simple_
{
    /// <summary>
    /// Fetches last 20 days of candles for a specified symbol, prints them, and exits.
    /// </summary>
    class FetchDailyCandles
    {
        //TODO: fill AssemblyInfo
        private static readonly int Days = 20;

        static void Main(string[] args)
        {
            string baseSymbol = args[0];
            CandleSymbol candleSymbol = CandleSymbol.ValueOf(baseSymbol, CandlePeriod.DAY);
            long toTime = Tools.DateToUnixTime(DateTime.Now);
            long fromTime = Tools.DateToUnixTime(DateTime.Now.AddDays(-1 * Days));
            Console.WriteLine(string.Format("Fetching last {0} days of candles for {1}...", Days, baseSymbol));
            try
            {
                FetchAndPrint(candleSymbol, toTime, fromTime);
            }
            finally
            {
                Environment.Exit(0); // Exit when done
            }
        }

        private static void FetchAndPrint(CandleSymbol candleSymbol, long toTime, long fromTime)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            // Use default DXFeed instance
            Task<List<IDxCandle>> task = DXFeed.GetInstance().
                GetTimeSeriesPromise<IDxCandle>(candleSymbol, fromTime, toTime, cancelTokenSource.Token);
            foreach (IDxCandle candle in task.Result)
                Console.WriteLine(candle);
        }
    }
}
