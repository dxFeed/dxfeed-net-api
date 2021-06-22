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
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_read_write_raw_data_sample {
    /// <summary>
    ///     This sample class demonstrates how to save incoming binary traffic to file and how to read
    /// </summary>
    internal class Program {
        private static void DisconnectHandler(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        private static void Main() {
            const string ADDRESS = "demo.dxfeed.com:7300";
            const string SYMBOL = "IBM";
            const string SOURCE = "NTV";
            const string RAW_FILE_NAME = "test.raw";
            const EventType EVENT_TYPE = EventType.Order;

            Console.WriteLine("Connecting to {0} for Order#{1} snapshot on {2}...", ADDRESS, SOURCE, SYMBOL);

            try {
                NativeTools.InitializeLogging("dxf_read_write_raw_data_sample.log", true, true);
                Console.WriteLine("Writing to raw file");
                using (var con = new NativeConnection(ADDRESS, DisconnectHandler)) {
                    con.WriteRawData(RAW_FILE_NAME);
                    using (var s = con.CreateSubscription(EVENT_TYPE, new EventListener())) {
                        s.AddSource(SOURCE);
                        s.AddSymbol(SYMBOL);

                        Console.WriteLine("Receiving events for 15 seconds");
                        Thread.Sleep(15000);
                    }
                }

                Console.WriteLine("Reading from raw file");
                using (var con = new NativeConnection(RAW_FILE_NAME, DisconnectHandler)) {
                    using (var s = con.CreateSubscription(EVENT_TYPE, new EventListener())) {
                        s.AddSource(SOURCE);
                        s.AddSymbol(SYMBOL);

                        Thread.Sleep(2000);
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

        private class EventListener : IDxOrderListener {
            public void OnOrder<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxOrder {
                foreach (var o in buf)
                    Console.WriteLine("{0} {1}", buf.Symbol, o);
            }
        }
    }
}