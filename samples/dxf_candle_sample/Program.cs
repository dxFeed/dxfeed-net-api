#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.native;

namespace dxf_candle_sample {
    /// <summary>
    ///     This sample class demonstrates subscription to candle events.
    ///     The sample configures via command line, subscribes to candle events and prints received data.
    /// </summary>
    internal class Program {
        private const int HOST_INDEX = 0;
        private const int SYMBOL_INDEX = 1;

        private static bool TryParseDateTimeParam(string stringParam, InputParam<DateTime?> param) {
            DateTime dateTimeValue;

            if (!DateTime.TryParse(stringParam, out dateTimeValue)) return false;

            param.Value = dateTimeValue;

            return true;
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParam<string> param) {
            if (!paramTagString.Equals(tag)) return false;

            param.Value = paramString;

            return true;
        }

        private static void DisconnectHandler(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        private static void WriteHelp() {
            Console.WriteLine(
                "Usage: dxf_candle_sample <host:port>|<path> <base symbol> [<date>] [-T <token>] [-p] [<attributes> ...] \n" +
                "where\n" +
                "    host:port   - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                "    path        - The path to file with candle data (tape or non zipped Candle Web Service output)\n" +
                "    base symbol - The base market symbol without attributes\n" +
                "    date        - The date of Candle in the format YYYY-MM-DD (may be empty)\n" +
                "    -T <token>  - The authorization token\n" +
                "    -p          - Enables the data transfer logging\n" +
                "    attributes  - The candle attributes\n\n" +
                "attributes must use the style of \"name=value\" and may be as follows:\n" +
                "    exchange  - The exchange code letter (may be empty)\n" +
                "    period    - The aggregation period of this symbol (may be empty, default: 1)\n" +
                "    type      - The type of the candle aggregation period (use \"t\" for TICK, \"s\"\n" +
                "                for SECOND, \"m\" for MINUTE, \"h\" for HOUR, \"d\" for DAY, \"w\" for\n" +
                "                WEEK, \"mo\" for MONTH, \"o\" for OPTEXP, \"y\" for YEAR, \"v\" for\n" +
                "                VOLUME, \"p\" for PRICE, \"pm\" for PRICE_MOMENTUM, \"pr\" for\n" +
                "                PRICE_RENKO or may be empty, default: TICK\n" +
                "    price     - Defines price that is used to build the candle (use \"last\" for\n" +
                "                LAST, \"ask\" for ASK, \"mark\" for MARK, \"s\" for SETTLEMENT or may\n" +
                "                be empty, default: LAST\n" +
                "    session   - The session attribute defines trading that is used to build the\n" +
                "                candles. (use \"false\" for ANY, \"true\" for REGULAR or may be\n" +
                "                empty, default: ANY\n" +
                "    alignment - The alignment defines how candle are aligned with respect to time\n" +
                "                (use \"m\" for MIDNIGHT, \"s\" for SESSION or may be empty,\n" +
                "    priceLevel - The candle price level\n" +
                "                 default: NaN\n\n" +
                "All missed attributes values will be set to defaults\n" +
                "examples: \n" +
                "    dxf_candle_sample demo.dxfeed.com:7300 XBT/USD 2016-06-18 exchange=A period=1 type=d price=last session=false alignment=m priceLevel=0.5\n" +
                "    dxf_candle_sample demo.dxfeed.com:7300 XBT/USD&A{=1d,price=last,a=m,pl=0.5} 2016-06-18\n" +
                "    dxf_candle_sample ./candledata_file AAPL type=m exchange=Q\n" +
                "    dxf_candle_sample ./candledata_file AAPL&Q{=m}\n"
            );
        }

        private static void Main(string[] args) {
            if (args.Length < 2) {
                WriteHelp();
                return;
            }

            try {
                var address = args[HOST_INDEX];
                var baseSymbol = args[SYMBOL_INDEX];
                var dateTime = new InputParam<DateTime?>(new DateTime(1970, 1, 1, 0, 0, 0, 0));
                var token = new InputParam<string>(null);
                var exchange = CandleSymbolAttributes.Exchange.DEFAULT;
                var periodValue = 1.0;
                var period = CandleSymbolAttributes.Period.DEFAULT;
                var price = CandleSymbolAttributes.Price.DEFAULT;
                var session = CandleSymbolAttributes.Session.DEFAULT;
                var alignment = CandleSymbolAttributes.Alignment.DEFAULT;
                var priceLevel = CandleSymbolAttributes.PriceLevel.DEFAULT;
                var logDataTransferFlag = false;

                var attributesAreSet = false;
                for (var i = SYMBOL_INDEX + 1; i < args.Length; i++) {
                    if (!dateTime.IsSet && TryParseDateTimeParam(args[i], dateTime))
                        continue;

                    if (!token.IsSet && i < args.Length - 1 &&
                        TryParseTaggedStringParam("-T", args[i], args[i + 1], token)) {
                        i++;

                        continue;
                    }

                    if (logDataTransferFlag == false && args[i].Equals("-p")) {
                        logDataTransferFlag = true;
                        i++;

                        continue;
                    }

                    const string KEY_VALUE_REGEX = @"([a-z]+)(=)([a-z]+|\d+\.?\d*)";
                    var match = Regex.Match(args[i], KEY_VALUE_REGEX, RegexOptions.IgnoreCase);

                    if (match.Groups.Count < 4 || !match.Success) {
                        Console.WriteLine("Invalid Attributes");
                        WriteHelp();
                        return;
                    }

                    if (match.Groups[1].Value.Equals("exchange")) {
                        if (match.Groups[3].Length == 1 && char.IsLetter(match.Groups[3].Value[0]))
                        {
                            exchange = CandleSymbolAttributes.Exchange.NewExchange(match.Groups[3].Value[0]);
                            attributesAreSet = true;
                        }
                    } else if (match.Groups[1].Value.Equals("period")) {
                        periodValue = double.Parse(match.Groups[3].Value, new CultureInfo("en-US"));
                        attributesAreSet = true;
                    } else if (match.Groups[1].Value.Equals("type")) {
                        period = CandleSymbolAttributes.Period.NewPeriod(periodValue,
                            CandleType.Parse(match.Groups[3].Value));
                        attributesAreSet = true;
                    } else if (match.Groups[1].Value.Equals("price")) {
                        price = CandleSymbolAttributes.Price.Parse(match.Groups[3].Value);
                        attributesAreSet = true;
                    } else if (match.Groups[1].Value.Equals("session")) {
                        session = CandleSymbolAttributes.Session.Parse(match.Groups[3].Value);
                        attributesAreSet = true;
                    } else if (match.Groups[1].Value.Equals("alignment")) {
                        alignment = CandleSymbolAttributes.Alignment.Parse(match.Groups[3].Value);
                        attributesAreSet = true;
                    } else if (match.Groups[1].Value.Equals("priceLevel")) {
                        priceLevel = CandleSymbolAttributes.PriceLevel.Parse(match.Groups[3].Value);
                        attributesAreSet = true;
                    }
                }

                var symbol = (attributesAreSet)
                    ? CandleSymbol.ValueOf(baseSymbol, exchange, period, price, session, alignment, priceLevel)
                    : CandleSymbol.ValueOf(baseSymbol);

                Console.WriteLine($"Connecting to {address} for Candle on {symbol} ...");

                NativeTools.InitializeLogging("dxf_candle_sample.log", true, true, logDataTransferFlag);
                using (var con = token.IsSet
                    ? new NativeConnection(address, token.Value, DisconnectHandler)
                    : new NativeConnection(address, DisconnectHandler)) {
                    using (var s = con.CreateSubscription(dateTime.Value, new EventListener())) {
                        s.AddSymbol(symbol);

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            } catch (DxException dxException) {
                Console.WriteLine($"Native exception occurred: {dxException.Message}");
            } catch (Exception exc) {
                Console.WriteLine($"Exception occurred: {exc.GetType()}, message: {exc.Message}");
            }
        }

        private class InputParam<T> {
            private T value;

            private InputParam() {
                IsSet = false;
            }

            public InputParam(T defaultValue) : this() {
                value = defaultValue;
            }

            public bool IsSet { get; private set; }

            public T Value {
                get { return value; }
                set {
                    this.value = value;
                    IsSet = true;
                }
            }
        }
    }
}