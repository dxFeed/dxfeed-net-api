using System;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_regional_book_sample
{
    class RegionalBookListener : IDxRegionalBookListener
    {
        public void OnChanged(DxPriceLevelBook book)
        {
            Console.WriteLine(string.Format("\nNew Regional Order Book for {0}:", book.Symbol));
            Console.WriteLine(string.Format("{0,-7} {1,-8} {2,-15} | {3,-7} {4,-8} {5,-15}",
                "Ask", "Size", "Time", "Bid", "Size", "Time"));
            for (int i = 0; i < Math.Max(book.Asks.Length, book.Bids.Length); ++i)
            {
                if (i < book.Asks.Length)
                {
                    Console.Write(string.Format("{0,-7:n2} {1,-8} {2,-15:yyyyMMdd-HHmmss}",
                        book.Asks[i].Price, book.Asks[i].Size, book.Asks[i].Time));
                }
                else
                {
                    Console.Write(string.Format("{0,-7} {1,-8} {2,-15}", "", "", ""));
                }
                Console.Write(" | ");
                if (i < book.Bids.Length)
                {
                    Console.Write(string.Format("{0,-7:n2} {1,-8} {2,-15:yyyyMMdd-HHmmss}",
                        book.Bids[i].Price, book.Bids[i].Size, book.Bids[i].Time));
                }
                Console.WriteLine();
            }
        }
    }

    class QuoteListener : IDxQuoteListener
    {
        public void OnQuote<TB, TE>(TB buf)
           where TB : IDxEventBuf<TE>
           where TE : IDxQuote
        {

            foreach (var q in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, q));
        }
    }

    class Program
    {
        private const int HostIndex = 0;
        private const int SymbolIndex = 1;

        private static void OnDisconnect(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(
                    "Usage: dxf_regional_book_sample <host:port> <symbol>\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    symbol    - IBM\n\n" +
                    "example: dxf_regional_book_sample demo.dxfeed.com:7300 MSFT\n"
                );
                return;
            }

            string address = args[HostIndex];
            string symbol = args[SymbolIndex];

            Console.WriteLine(string.Format("Connecting to {0} on {1}...", address, symbol));

            try
            {
                NativeTools.InitializeLogging("log.log", true, true);
                using (var con = new NativeConnection(address, OnDisconnect))
                {
                    using (var s = con.CreateRegionalBook(symbol, new RegionalBookListener(), new QuoteListener()))
                    {
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
