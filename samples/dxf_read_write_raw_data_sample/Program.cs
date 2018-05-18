#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_read_write_raw_data_sample
{
    /// <summary>
    ///   This sample class demonstrates how to save incoming binnary traffic to file and how to read
    /// </summary>
    class Program
    {
        public class EventListener : IDxOrderListener
        {
            public void OnOrder<TB, TE>(TB buf)
                where TB : IDxEventBuf<TE>
                where TE : IDxOrder
            {
                foreach (var o in buf)
                    Console.WriteLine(string.Format("{0} {1}", buf.Symbol, o));
            }
        }

        private static void OnDisconnect(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        static void Main(string[] args)
        {
            var address = "demo.dxfeed.com:7300";
            var symbol = "IBM";
            var source = "NTV";
            var rawFileName = "test.raw";

            EventType eventType = EventType.Order;

            Console.WriteLine(string.Format("Connecting to {0} for Order#{1} snapshot on {2}...",
                address, source, symbol));

            try
            {
                NativeTools.InitializeLogging("log.log", true, true);
                Console.WriteLine("Writing to raw file");
                using (var con = new NativeConnection(address, OnDisconnect))
                {
                    con.WriteRawData(rawFileName);
                    using (var s = con.CreateSubscription(eventType, new EventListener()))
                    {
                        s.AddSource(source);
                        s.AddSymbol(symbol);

                        Console.WriteLine("Receiving events for 15 seconds");
                        System.Threading.Thread.Sleep(15000);
                    }
                }
                Console.WriteLine("Reading from raw file");
                using (var con = new NativeConnection(rawFileName, OnDisconnect))
                {
                    using (var s = con.CreateSubscription(eventType, new EventListener()))
                    {
                        s.AddSource(source);
                        s.AddSymbol(symbol);

                        System.Threading.Thread.Sleep(2000);
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
