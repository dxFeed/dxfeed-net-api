using System;
using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.native;

namespace dxf_candle_sample
{
    /// <summary>
    /// This Sample class demonstrates subscription to candle events.
    /// The sample configures via command line, subscribes to candle events and prints received data.
    /// </summary>
    class Program
    {
        private const int hostIndex = 0;
        private const int dateIndex = 1;

        private static void OnDisconnect(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine(
                    "Usage: dxf_candle_sample <host:port> <date> <symbols>\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    date      - date of Candle in the format YYYY-MM-DD (it may be empty)\n" +
                    "    symbols   - candle symbol attribute string\n\n" +
                    "example: dxf_client demo.dxfeed.com:7300 2016-06-18 XBT/USD{=d} AAPL{=d,price=mark}"
                );
                return;
            }

            var address = args[hostIndex];

            int symbolsIndex = dateIndex;
            DateTime dateValue;
            if (DateTime.TryParse(args[1], out dateValue))
                symbolsIndex = dateIndex + 1;

            var symbols = new List<CandleSymbol>();
            for (int i = symbolsIndex; i < args.Length; i++)
                symbols.Add(CandleSymbol.ValueOf(args[i]));

            Console.WriteLine(string.Format("Connecting to {0} for Candle on [{1}] ...",
                address, String.Join(", ", symbols)));

            try
            {
                using (var con = new NativeConnection(address, OnDisconnect))
                {
                    using (var s = con.CreateSubscription(dateValue, new EventListener()))
                    {
                        s.AddSymbols(symbols.ToArray());

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
