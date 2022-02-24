#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Threading;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.plb;
using com.dxfeed.native;

namespace dxf_price_level_book_sample
{
    internal class PriceLevelBookHandler : IDxOnNewPriceLevelBookHandler, IDxOnPriceLevelBookUpdateHandler, IDxOnPriceLevelBookIncChangeHandler
    {
        private static void DumpBook(DxPriceLevelBook book)
        {
            Console.WriteLine($"{"Ask",-15} {"Size",-8} {"Time",-15} | {"Bid",-15} {"Size",-8} {"Time",-15}");
            for (var i = 0; i < Math.Max(book.Asks.Length, book.Bids.Length); ++i)
            {
                if (i < book.Asks.Length)
                    Console.Write("{0,-15:n6} {1,-8:n2} {2,-15:yyyyMMdd-HHmmss}", book.Asks[i].Price,
                        book.Asks[i].Size,
                        book.Asks[i].Time);
                else
                    Console.Write("{0,-15} {1,-8} {2,-15}", "", "", "");
                Console.Write(" | ");
                if (i < book.Bids.Length)
                    Console.Write("{0,-15:n6} {1,-8:n2} {2,-15:yyyyMMdd-HHmmss}", book.Bids[i].Price,
                        book.Bids[i].Size,
                        book.Bids[i].Time);
                Console.WriteLine();
            }
        }
        public void OnNewBook(DxPriceLevelBook book)
        {
            Console.WriteLine($"\nNew Price Level Book for {book.Symbol}:");
            DumpBook(book);
        }

        public void OnBookUpdate(DxPriceLevelBook book)
        {
            Console.WriteLine($"\nThe Update of The Price Level Book for {book.Symbol}:");
            DumpBook(book);
        }

        public void OnBookIncrementalChange(DxPriceLevelBook removals, DxPriceLevelBook additions, DxPriceLevelBook updates)
        {
            Console.WriteLine($"\nThe Incremental Update of The Price Level Book for {removals.Symbol}:");
            
            if (removals.Asks.Length > 0 || removals.Bids.Length > 0)
            {
                Console.WriteLine("\nREMOVALS:");
                DumpBook(removals);
            }
            
            if (additions.Asks.Length > 0 || additions.Bids.Length > 0)
            {
                Console.WriteLine("\nADDITIONS:");
                DumpBook(additions);
            }
            
            if (updates.Asks.Length > 0 || updates.Bids.Length > 0)
            {
                Console.WriteLine("\nUPDATES:");
                DumpBook(updates);
            }
        }
    }

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
                "Usage: dxf_price_level_book_sample <host:port> <symbol> <source> <levels number> [-T <token>] [-p]\n" +
                "where\n" +
                "    host:port       - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                "    symbol          - The price level book symbol (IBM, AAPL etc)\n" +
                "    source          - One order source, e.g. one of the: NTV, BYX, BZX, DEA etc. or AGGREGATE_ASK|BID\n" +
                "    <levels number> - The The number of PLB price levels (0 -- all)\n" +
                "    -T <token>      - The authorization token\n" +
                "    -p         - Enables the data transfer logging\n\n" +
                "examples: \n" +
                "dxf_price_level_book_sample demo.dxfeed.com:7300 MSFT NTV 5\n" +
                "dxf_price_level_book_sample demo.dxfeed.com:7300 AAPL AGGREGATE_ASK 0\n" +
                "\n"
            );
        }

        private static void Main(string[] args)
        {
            if (args.Length < 4 || args.Length > 6)
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
                    i++;
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
                    using (var plb = con.CreatePriceLevelBook(symbol, source, levelsNumber))
                    {
                        var handler = new PriceLevelBookHandler();
                        plb.SetHandlers(handler, handler, handler);
                        
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