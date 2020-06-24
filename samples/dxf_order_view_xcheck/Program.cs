#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.native;

namespace dxf_order_view_xcheck {
    internal class Program {
        private const int HOST_INDEX = 0;

        private static void DisconnectHandler(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParam<string> param) {
            if (!paramTagString.Equals(tag)) return false;

            param.Value = paramString;

            return true;
        }

        private static void Main(string[] args) {
            if (args.Length < 1 || args.Length > 4) {
                Console.WriteLine(
                    "Usage: dxf_order_view_xcheck <host:port> [-T <token>] [-p]\n" +
                    "where\n" +
                    "    host:port  - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    -T <token> - The authorization token\n" +
                    "    -p         - Enables the data transfer logging\n\n" +
                    "example: dxf_order_view_xcheck demo.dxfeed.com:7300\n"
                );

                return;
            }

            var address = args[HOST_INDEX];
            var token = new InputParam<string>(null);
            var logDataTransferFlag = false;

            for (var i = HOST_INDEX + 1; i < args.Length; i++) {
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

            Console.WriteLine("Connecting to {0} for Order View", address);

            try {
                NativeTools.InitializeLogging("dxf_order_view_xcheck.log", true, true, logDataTransferFlag);
                using (var con = token.IsSet
                    ? new NativeConnection(address, token.Value, DisconnectHandler)
                    : new NativeConnection(address, DisconnectHandler)) {
                    var l = new OrderViewEventListener();
                    var subs = new List<IDxSubscription>();
                    /*
                     * We CAN NOT use one instance OrderViewSubscription here.
                     * If OrderViewSubscription is configured with multiple sources,
                     * it MIXES sources to one "view" and this "view" is NOT
                     * transactional-safe and atomic anymore.
                     * You could see "crosses" between orders with different sources
                     * in the mixed view, it is normal. Atomicity is only guaranteed
                     * for order book from one source!
                     */
                    foreach (var src in new[] {"NTV" /*, "DEX", "BZX" */}) {
                        var s = con.CreateOrderViewSubscription(l);
                        s.SetSource(src);
                        s.SetSymbols( /*"AAPL",*/ "FB" /*, "SPY"*/);
                        subs.Add(s);
                    }

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                    foreach (var sub in subs)
                        sub.Dispose();
                }
            } catch (DxException dxException) {
                Console.WriteLine($"Native exception occured: {dxException.Message}");
            } catch (Exception exc) {
                Console.WriteLine($"Exception occured: {exc.Message}");
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