#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native;

// using Newtonsoft.Json;

namespace dxf_simple_order_book_sample {
    public class OrderListener : IDxOrderSnapshotListener {
        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "{3}: Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}", buf.EventType, buf.Symbol, buf.Size,
                DateTime.Now.ToString("o")));

            var book = buf.Select(o => new Offer {
                    Side = o.Side,
                    Price = o.Price,
                    Size = o.Size,
                    Timestamp = o.Time,
                    Sequence = o.Sequence,
                    Source = o.Source?.Name,
                    MarketMaker = o.MarketMaker
                })
                .ToList();

            Console.Write("Bids:\n");
            var bids = book.Where(o => o.Side == Side.Buy).OrderByDescending(o => o.Price).Take(10);
            foreach (var o in bids)
                Console.WriteLine($"{o.Price} {o.Size}");

            Console.WriteLine();

            Console.Write("Asks:\n");
            var asks = book.Where(o => o.Side == Side.Sell).OrderBy(o => o.Price).Take(10);
            foreach (var o in asks)
                Console.WriteLine($"{o.Price} {o.Size}");
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class Offer {
            public Side Side { get; set; }
            public double Price { get; set; }
            public DateTime Timestamp { get; set; }
            public long Size { get; set; }
            public long Sequence { get; set; }
            public string Source { get; set; }
            public string MarketMaker { get; set; }
        }
    }

    internal class Program {
        public void Run() {
            const string ADDRESS = "demo.dxfeed.com:7300";
            using (var con = new NativeConnection(ADDRESS, DisconnectHandler)) {
                IDxSubscription s = null;
                try {
                    s = con.CreateSnapshotSubscription(EventType.Order, 0, new OrderListener());
                    s.SetSource(OrderSource.NTV);
                    s.AddSymbol("IBM");
                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                } catch (DxException dxException) {
                    Console.WriteLine("Native exception occured: " + dxException.Message);
                } catch (Exception exc) {
                    Console.WriteLine("Exception occured: " + exc.Message);
                } finally {
                    s?.Dispose();
                }
            }
        }

        private static void DisconnectHandler(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        private static void Main() {
            Console.ReadKey();
            new Program().Run();
        }
    }
}