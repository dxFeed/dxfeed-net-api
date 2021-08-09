#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.native;

namespace dxf_events_sample {
    /// <summary>
    ///     This sample class demonstrates subscription to events.
    ///     The sample configures via command line, subscribes to events and prints received data.
    /// </summary>
    internal class Program {
        private const int HOST_INDEX = 0;
        private const int EVENT_INDEX = 1;
        private const int SYMBOL_INDEX = 2;

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

        private static void Main(string[] args) {
            if (args.Length < 3 || args.Length > 7) {
                Console.WriteLine(
                    "Usage: dxf_events_sample <host:port> <event> <symbol> [<date>] [-T <token>] [-p]\n" +
                    "where\n" +
                    "    host:port  - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    event      - Any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary,\n" +
                    "                 TradeETH,SpreadOrder,Greeks,TheoPrice,Underlying,Series,\n" +
                    "                 Configuration}\n" +
                    "    symbol     - IBM, MSFT, ...\n\n" +
                    "    date       - The date of time series event in the format YYYY-MM-DD (optional)\n" +
                    "    -T <token> - The authorization token\n" +
                    "    -p         - Enables the data transfer logging\n\n" +
                    "example: dxf_events_sample demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST\n" +
                    "or: dxf_events_sample demo.dxfeed.com:7300 TimeAndSale MSFT,IBM 2016-10-10\n"
                );
                return;
            }

            var address = args[HOST_INDEX];

            EventType events;
            if (!Enum.TryParse(args[EVENT_INDEX], true, out events)) {
                Console.WriteLine($"Unsupported event type: {args[EVENT_INDEX]}");
                return;
            }

            var symbols = args[SYMBOL_INDEX].Split(',');
            var dateTime = new InputParam<DateTime?>(null);
            var token = new InputParam<string>(null);
            var logDataTransferFlag = false;

            for (var i = SYMBOL_INDEX + 1; i < args.Length; i++) {
                if (!dateTime.IsSet && TryParseDateTimeParam(args[i], dateTime)) {
                    continue;
                }

                if (!token.IsSet && i < args.Length - 1 &&
                    TryParseTaggedStringParam("-T", args[i], args[i + 1], token))
                {
                    i++;
                    continue;
                }

                if (logDataTransferFlag == false && args[i].Equals("-p")) {
                    logDataTransferFlag = true;
                    i++;
                }
            }

            Console.WriteLine($"Connecting to {address} for [{events}] on [{string.Join(", ", symbols)}] ...");

            using var c = new NativeConnection(address, con => {});
            var result = c.GetDataForPeriod(EventType.Candle, "AAPL&Q{=1m}", DateTime.Now.Subtract(TimeSpan.FromDays(5)),
                DateTime.Now.Subtract(TimeSpan.FromDays(1)));
            
            result.Result.ForEach(Console.WriteLine);
            
            try {
                NativeTools.InitializeLogging("dxf_events_sample.log", true, true, logDataTransferFlag);
                using (var con = token.IsSet
                    ? new NativeConnection(address, token.Value, DisconnectHandler)
                    : new NativeConnection(address, DisconnectHandler)) {
                    using (var s = con.CreateSubscription(events, dateTime.Value, new EventListener())) {
                        s.AddSymbols(symbols);

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            } catch (DxException dxException) {
                Console.WriteLine($"Native exception occurred: {dxException.Message}");
            } catch (Exception exc) {
                Console.WriteLine($"Exception occurred: {exc.Message}");
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