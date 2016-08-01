using System;
using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.api.candle;
//using com.dxfeed.api.candle.;
using com.dxfeed.native;

namespace dxf_candle_sample {
    /// <summary>
    /// This sample class demonstrates subscription to candle events.
    /// The sample configures via command line, subscribes to candle events and prints received data.
    /// </summary>
    class Program {
        private const int hostIndex = 0;
        private const int dateIndex = 1;

        private static void OnDisconnect(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine(
                    "Usage: dxf_candle_sample <host:port> <date> <base symbol> <exchange>\n" +
                    "<period value> <type> <price> <session> <alignment>\n" +
                    "where\n" +
                    "    host:port    - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    date         - date of Candle in the format YYYY-MM-DD (may be empty)\n" +
                    "    base symbol  - base market symbol without attributes\n" +
                    "    exchange     - exchange code letter (may be empty)\n" +
                    "    period value - aggregation period of this symbol (may be empty, default: 1)\n" +
                    "    type         - Type of the candle aggregation period (use \"t\" for TICK, \"s\"\n" +
                    "                   for SECOND, \"m\" for MINUTE, \"h\" for HOUR, \"d\" for DAY, \"w\"\n" +
                    "                   for WEEK, \"mo\" for MONTH, \"o\" for OPTEXP, \"y\" for YEAR, \"v\"\n" +
                    "                   for VOLUME, \"p\" for PRICE, \"pm\" for PRICE_MOMENTUM, \"pr\" for\n" +
                    "                   PRICE_RENKO or may be empty, default: TICK\n" +
                    "    price        - defines price that is used to build the candle (use \"last\"\n" +
                    "                   for LAST, \"ask\" for ASK, \"mark\" for MARK, \"s\" for SETTLEMENT\n" +
                    "                   or may be empty, default: LAST\n" +
                    "    session      - Session attribute defines trading that is used to build the\n" +
                    "                   candles. (use \"false\" for ANY, \"true\" for REGULAR or may be\n" +
                    "                   empty, default: ANY\n" +
                    "    alignment    - alignment defines how candle are aligned with respect to\n" +
                    "                   time (use \"m\" for MIDNIGHT, \"s\" for SESSION or may be empty,\n" +
                    "                   default: MIDNIGHT\n\n" +
                    "All empty optional fields will be set to defaults" +
                    "example: dxf_candle_sample demo.dxfeed.com:7300 2016-06-18 XBT/USD a 1 d last false m"
                );
                return;
            }

            try
            {
                var address = args[hostIndex];
                string baseSymbol;
                ICandleSymbolAttribute exchange = CandleSymbolAttributes.Exchange.DEFAULT;
                double period_value;
                ICandleSymbolAttribute period;
                ICandleSymbolAttribute price;
                ICandleSymbolAttribute session;
                ICandleSymbolAttribute alignment;

                int index = dateIndex;
                DateTime dateTime = new DateTime();
                if (DateTime.TryParse(args[index], out dateTime)) {
                    index++;
                }
                else {
                    dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                }

                baseSymbol = args[index];
                index++;

                if (args[index].Length == 1 && char.IsLetter(args[index][0])) {
                    exchange = CandleSymbolAttributes.Exchange.NewExchange(args[index][0]);
                    index++;
                }

                if (double.TryParse(args[index], out period_value)) {
                    index++;
                }
                else {
                    period_value = 1.0;
                }

                try {
                    period = CandleSymbolAttributes.Period.NewPeriod(period_value, CandleType.Parse(args[index]));
                    index++;
                } catch {
                    period = CandleSymbolAttributes.Period.DEFAULT;
                }

                try {
                    price = CandleSymbolAttributes.Price.Parse(args[index]);
                    index++;
                } catch {
                    price = CandleSymbolAttributes.Price.DEFAULT;
                }

                try {
                    session = CandleSymbolAttributes.Session.Parse(args[index]);
                    index++;
                } catch {
                    session = CandleSymbolAttributes.Session.DEFAULT;
                }

                try {
                    alignment = CandleSymbolAttributes.Alignment.Parse(args[index]);
                } catch {
                    alignment = CandleSymbolAttributes.Alignment.DEFAULT;
                }


                CandleSymbol symbol = CandleSymbol.ValueOf(baseSymbol, exchange, period, price, session, alignment);

                Console.WriteLine(string.Format("Connecting to {0} for Candle on {1} ...",
                    address, symbol));

                NativeTools.InitializeLogging("log.log", true, true);
                using (var con = new NativeConnection(address, OnDisconnect)) {
                    using (var s = con.CreateSubscription(dateTime, new EventListener())) {
                        s.AddSymbol(symbol);

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            } catch (DxException dxException) {
                Console.WriteLine("Native exception occured: " + dxException.Message);
            } catch (Exception exc) {
                Console.WriteLine("Exception occured: " + exc.Message);
            }
        }
    }
}
