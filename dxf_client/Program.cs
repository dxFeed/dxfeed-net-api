/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native;

namespace dxf_client
{
    public class SnapshotPrinter : IDxSnapshotListener
    {
        #region Implementation of IDxSnapshotListener

        private const int RECORDS_PRINT_LIMIT = 7;

        private void PrintSnapshot<TE>(IDxEventBuf<TE> buf)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}",
                buf.EventType, buf.Symbol, buf.Size));
            int count = 0;
            foreach (var o in buf)
            {
                Console.WriteLine(string.Format("   {{ {0} }}", o));
                if (++count >= RECORDS_PRINT_LIMIT)
                {
                    Console.WriteLine(string.Format("   {{ ... {0} records left ...}}", buf.Size - count));
                    break;
                }
            }
        }

        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            PrintSnapshot(buf);
        }

        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            PrintSnapshot(buf);
        }

        #endregion
    }

    class Program
    {
        static bool TryParseSnapshotParam(string param, out bool isSnapshot)
        {
            isSnapshot = param.ToLower().Equals("snapshot");
            return isSnapshot;
        }

        static void TryParseSourcesParam(string param, out string[] sources)
        {
            sources = param.Split(',');
        }

        static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 5)
            {
                Console.WriteLine(
                    "Usage: dxf_client <host:port> <event> <symbol> [<source>] [snapshot]\n" +
                    "where\n" +
                    "    host:port - address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    event     - any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary,Candle}\n" +
                    "    symbol    - a) IBM, MSFT, ...\n" +
                    "                b) if it is Candle event you can specify candle symbol attribute by \n" +
                    "                   string, for example: XBT/USD{=d}\n" +
                    "    source    - order sources NTV, BYX, BZX, DEA, DEX, IST, ISE,... (can be empty)\n" +
                    "    snapshot  - use keyword 'snapshot' for create snapshot subscription, otherwise leave empty\n\n" +
                    "example: dxf_client demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST NTV,IST"
                );
                return;
            }

            var address = args[0];

            EventType events;
            if (!Enum.TryParse(args[1], true, out events))
            {
                Console.WriteLine("Unsupported event type: " + args[1]);
                return;
            }

            string symbols = args[2];
            string[] sources = new string[0];
            bool isSnapshot = false;
            if (args.Length == 4)
            {
                string param = args[3];
                if (!TryParseSnapshotParam(param, out isSnapshot))
                    TryParseSourcesParam(param, out sources);
            }
            else if (args.Length == 5)
            {
                TryParseSourcesParam(args[3], out sources);
                TryParseSnapshotParam(args[4], out isSnapshot);
            }

            Console.WriteLine(string.Format("Connecting to {0} for [{1}{2}] on [{3}] ...",
                address, events, isSnapshot ? " snapshot" : string.Empty, symbols));

            // NativeTools.InitializeLogging("dxf_client.log", true, true);

            var listener = new EventPrinter();
            using (var con = new NativeConnection(address, OnDisconnect))
            {
                IDxSubscription s = null;
                try
                {
                    if (isSnapshot)
                    {
                        s = con.CreateSnapshotSubscription(0, new SnapshotPrinter());
                    }
                    else if (events == EventType.Candle)
                    {
                        s = con.CreateSubscription(null, listener);
                    }
                    else
                    {
                        s = con.CreateSubscription(events, listener);
                    }

                    if (events == EventType.Order && sources.Length > 0)
                        s.SetSource(sources);

                    if (events == EventType.Candle)
                    {
                        CandleSymbol candleSymbol = CandleSymbol.ValueOf(symbols);
                        s.AddSymbol(candleSymbol);
                    }
                    else
                    {
                        s.AddSymbols(symbols.Split(','));
                    }

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                }
                catch (DxException dxException)
                {
                    Console.WriteLine("Native exception occured: " + dxException.Message);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Exception occured: " + exc.Message);
                }
                finally
                {
                    if (s != null)
                        s.Dispose();
                }
            }
        }

        private static void OnDisconnect(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }
    }
}
