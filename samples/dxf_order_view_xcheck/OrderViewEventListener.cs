#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_order_view_xcheck {
    internal enum Side {
        Sell,
        Buy
    }

    internal class SimpleOrder {
        private readonly long index;
        public readonly double price;
        public readonly Side side;
        private readonly double size;

        public SimpleOrder(IDxOrder dxo) {
            index = dxo.Index;
            side = dxo.Side == com.dxfeed.api.data.Side.Buy ? Side.Buy : Side.Sell;
            price = dxo.Price;
            size = dxo.Size;
        }

        public override string ToString() {
            return $"Order[{index}, Side = {(side == Side.Sell ? "Ask" : "Bid")}, Price = {price}, Size = {size}]";
        }
    }

    internal class OrderViewEventListener : IDxOrderViewListener {
        private readonly Dictionary<string, Dictionary<long, SimpleOrder>> state =
            new Dictionary<string, Dictionary<long, SimpleOrder>>();

        public void OnSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            var s = new Dictionary<long, SimpleOrder>();
            var count = 0;
            IndexedEventSource src = null;
            var sym = "";

            foreach (var o in buf) {
                if (src == null) {
                    src = o.Source;
                    sym = $"{buf.Symbol}#{src.Name}";
                } else if (!Equals(src, o.Source)) {
                    Console.WriteLine("ERROR: Mixed!");
                }

                s.Add(o.Index, new SimpleOrder(o));
                Console.WriteLine($"\"{sym}\" add {s[o.Index]}");
                count++;
            }

            /* Override old, if have one */
            state[sym] = s;
            Console.WriteLine($"New snapshot for \"{sym}\" with {count} orders");
            CheckBidAsk(sym, s);
        }

        public void OnUpdate<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            var count = 0;
            int u = 0, a = 0, r = 0;

            using (var e = buf.GetEnumerator()) {
                if (!e.MoveNext()) {
                    Console.WriteLine($"ERROR: Update for \"{buf.Symbol}#???\" without orders");
                    return;
                }

                var sym = $"{buf.Symbol}#{e.Current?.Source}";

                if (!state.ContainsKey(sym)) {
                    Console.WriteLine($"ERROR: Update for \"{sym}\" without snapshot");
                    return;
                }

                var s = state[sym];

                foreach (var o in buf) {
                    if (!o.HasSize()) {
                        if (s.ContainsKey(o.Index)) {
                            Console.WriteLine($"\"{sym}\" removal of {o.Index}");
                            s.Remove(o.Index);
                            r++;
                        }
                    } else if (s.ContainsKey(o.Index)) {
                        var so = new SimpleOrder(o);
                        Console.WriteLine($"\"{sym}\" replace {s[o.Index]} with {so}");
                        s[o.Index] = so;
                        u++;
                    } else {
                        s[o.Index] = new SimpleOrder(o);
                        Console.WriteLine($"\"{sym}\" add {s[o.Index]}");
                        a++;
                    }

                    count++;
                }

                Console.WriteLine($"\"{sym}\" update with {count} orders, A/U/R:{a}/{u}/{r}");
                CheckBidAsk(sym, s);
            }
        }

        private static void CheckBidAsk(string sym, Dictionary<long, SimpleOrder> s) {
            SimpleOrder ask = null;
            SimpleOrder bid = null;
            foreach (var o in s.Values) {
                if (o.side == Side.Buy && (bid == null || bid.price < o.price))
                    bid = o;
                if (o.side == Side.Sell && (ask == null || ask.price > o.price))
                    ask = o;
            }

            if (bid != null && ask != null && bid.price >= ask.price)
                Console.WriteLine($"ERROR: \"{sym}\": Cross: Bid {bid} vs Ask {ask}");
        }
    }
}