#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.native;
using System;

namespace dxf_order_view_xcheck
{
    class Program
    {
        private const int hostIndex = 0;

        private static void OnDisconnect(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine(
                    "Usage: dxf_order_view_xcheck <host:port>\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "example: dxf_order_view_xcheck demo.dxfeed.com:7300\n"
                );
                return;
            }

            var address = args[hostIndex];

            Console.WriteLine(string.Format("Connecting to {0} for Order View", address));

            try
            {
                NativeTools.InitializeLogging("log.log", true, true);
                using (var con = new NativeConnection(address, OnDisconnect))
                {
                    var l = new OrderViewEventListener();
                    var subs = new List<IDxSubscription>();
                    /*
                     * We CAN NOT use one instance OrderViewSubscription here.
                     * If OrderViewSubscription is configured with multiple sources,
                     * it MIXES sources to one "view" and this "view" is NOT
                     * transactional-safe and atomic anymore.
                     * You could see "crosses" between orders with different sources
                     * in the mixed view, it is nornmal. Atomicity is only guranteed
                     * for order book from one source!
                     */
                    foreach (var src in new[] { "NTV" /*, "DEX", "BZX" */})
                    {
                        var s = con.CreateOrderViewSubscription(l);
                        s.SetSource(src);
                        s.SetSymbols(/*"AAPL",*/ "FB"/*, "SPY"*/);
                        subs.Add(s);
                    }
                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                    foreach (var sub in subs)
                        sub.Dispose();
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
