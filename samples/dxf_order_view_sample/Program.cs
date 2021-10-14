#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.native;

namespace dxf_order_view_sample {
    internal class Program {
        private const int DefaultRecordsPrintLimit = 7;
        private const int HostIndex = 0;

        private static void DisconnectHandler(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        private static bool TryParseRecordsPrintLimitParam(string stringParamTag, string stringParam,
            InputParam<int> param) {
            if (!stringParamTag.Equals("-l")) return false;

            int newRecordsPrintLimit;

            if (!int.TryParse(stringParam, out newRecordsPrintLimit)) return false;

            param.Value = newRecordsPrintLimit;

            return true;
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParam<string> param) {
            if (!paramTagString.Equals(tag)) return false;

            param.Value = paramString;

            return true;
        }

        private static void PrintUsage() {
            Console.WriteLine(
                "Usage: dxf_order_view_sample <host:port> [-l <records_print_limit>] [-T <token>] [-p]\n" +
                "where\n" +
                "    host:port           - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                $"    records_print_limit - The number of displayed records (0 - unlimited, default: {DefaultRecordsPrintLimit})\n" +
                "    -T <token>          - The authorization token\n" +
                "    -p                  - Enables the data transfer logging\n\n" +
                "examples: dxf_order_view_sample demo.dxfeed.com:7300\n" +
                "          dxf_order_view_sample demo.dxfeed.com:7300 -l 0\n"
            );
        }

        private static void Main(string[] args) {
            if (args.Length < 1 || args.Length > 6) {
                PrintUsage();

                return;
            }

            var address = args[HostIndex];
            var recordsPrintLimit = new InputParam<int>(DefaultRecordsPrintLimit);
            var token = new InputParam<string>(null);
            var logDataTransferFlag = false;

            for (var i = HostIndex + 1; i < args.Length; i++) {
                if (!recordsPrintLimit.IsSet && i < args.Length - 1 &&
                    TryParseRecordsPrintLimitParam(args[i], args[i + 1], recordsPrintLimit)) {
                    i++;

                    continue;
                }

                if (!token.IsSet && i < args.Length - 1 &&
                    TryParseTaggedStringParam("-T", args[i], args[i + 1], token)) {
                    i++;

                    continue;
                }

                if (logDataTransferFlag == false && args[i].Equals("-p")) {
                    logDataTransferFlag = true;
                    i++;
                }
            }

            Console.WriteLine($"Connecting to {address} for Order View");

            try {
                NativeTools.InitializeLogging("dxf_order_view_sample.log", true, true, logDataTransferFlag);
                using (var con = token.IsSet
                    ? new NativeConnection(address, token.Value, DisconnectHandler)
                    : new NativeConnection(address, DisconnectHandler)) {
                    using (var sub =
                        con.CreateOrderViewSubscription(new OrderViewEventListener(recordsPrintLimit.Value))) {
                        sub.SetSource("NTV");
                        sub.SetSymbols("AAPL");

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