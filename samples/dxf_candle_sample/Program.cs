using System;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.native;
using System.Text.RegularExpressions;
using System.Globalization;

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

        private static void WriteHelp() {
            Console.WriteLine(
                "Usage: dxf_candle_sample <host:port> <date> <base symbol> <attributes> ... \n" +
                "where\n" +
                "    host:port   - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                "    date        - date of Candle in the format YYYY-MM-DD (may be empty)\n" +
                "    base symbol - base market symbol without attributes\n" +
                "    attributes  - candle attributes\n\n" +
                "attributes must use the style of \"name=value\" and may be as follows:\n" +
                "    exchange  - exchange code letter (may be empty)\n" +
                "    period    - aggregation period of this symbol (may be empty, default: 1)\n" +
                "    type      - Type of the candle aggregation period (use \"t\" for TICK, \"s\"\n" +
                "                for SECOND, \"m\" for MINUTE, \"h\" for HOUR, \"d\" for DAY, \"w\" for\n" +
                "                WEEK, \"mo\" for MONTH, \"o\" for OPTEXP, \"y\" for YEAR, \"v\" for\n" +
                "                VOLUME, \"p\" for PRICE, \"pm\" for PRICE_MOMENTUM, \"pr\" for\n" +
                "                PRICE_RENKO or may be empty, default: TICK\n" +
                "    price     - defines price that is used to build the candle (use \"last\" for\n" +
                "                LAST, \"ask\" for ASK, \"mark\" for MARK, \"s\" for SETTLEMENT or may\n" +
                "                be empty, default: LAST\n" +
                "    session   - Session attribute defines trading that is used to build the\n" +
                "                candles. (use \"false\" for ANY, \"true\" for REGULAR or may be\n" +
                "                empty, default: ANY\n" +
                "    alignment - alignment defines how candle are aligned with respect to time\n" +
                "                (use \"m\" for MIDNIGHT, \"s\" for SESSION or may be empty,\n" +
                "                default: MIDNIGHT\n\n" +
                "All missed attributes values will be set to defaults\n" +
                "example: dxf_candle_sample demo.dxfeed.com:7300 2016-06-18 XBT/USD exchange=a\n" +
                "period=1 type=d price=last session=false alignment=m"
            );
        }

        static void Main(string[] args) {
            if (args.Length < 3) {
                WriteHelp();
                return;
            }

            try
            {
                var address = args[hostIndex];
                string baseSymbol;
                ICandleSymbolAttribute exchange = CandleSymbolAttributes.Exchange.DEFAULT;
                double period_value = 1.0;
                ICandleSymbolAttribute period = CandleSymbolAttributes.Period.DEFAULT;
                ICandleSymbolAttribute price = CandleSymbolAttributes.Price.DEFAULT;
                ICandleSymbolAttribute session = CandleSymbolAttributes.Session.DEFAULT;
                ICandleSymbolAttribute alignment = CandleSymbolAttributes.Alignment.DEFAULT;

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

                for (int i = index; i < args.Length; i++) {
                    const string regEx = @"([a-z]+)(=)([a-z]+|\d+\.?\d*)";
                    Match match = Regex.Match(args[i], regEx, RegexOptions.IgnoreCase);
                    if (match.Groups.Count < 4 || !match.Success) {
                        Console.WriteLine("Invalid Attributes");
                        WriteHelp();
                        return;
                    }
                    if (match.Groups[1].Value.Equals("exchange")) {
                        if (match.Groups[3].Length == 1 && char.IsLetter(match.Groups[3].Value[0])) {
                            exchange = CandleSymbolAttributes.Exchange.NewExchange(match.Groups[3].Value[0]);
                        }
                    } else if (match.Groups[1].Value.Equals("period")) {
                        period_value = double.Parse(match.Groups[3].Value, new CultureInfo("en-US"));
                    } else if (match.Groups[1].Value.Equals("type")) {
                        period = CandleSymbolAttributes.Period.NewPeriod(period_value, CandleType.Parse(match.Groups[3].Value));
                    } else if (match.Groups[1].Value.Equals("price")) {
                        price = CandleSymbolAttributes.Price.Parse(match.Groups[3].Value);
                    } else if (match.Groups[1].Value.Equals("session")) {
                        session = CandleSymbolAttributes.Session.Parse(match.Groups[3].Value);
                    } else if (match.Groups[1].Value.Equals("alignment")) {
                        alignment = CandleSymbolAttributes.Alignment.Parse(match.Groups[3].Value);
                    }
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
                Console.WriteLine("Exception occured: " + exc.GetType().ToString() + ", message: " + exc.Message);
            }
        }
    }
}
