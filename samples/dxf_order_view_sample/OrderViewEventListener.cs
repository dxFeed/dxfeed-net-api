#region License

/*
Copyright (C) 2010-2018 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_order_view_sample {
    internal class OrderViewEventListener : IDxOrderViewListener {
        private readonly int recordsPrintLimit;

        public OrderViewEventListener(int recordsPrintLimit) {
            this.recordsPrintLimit = recordsPrintLimit;
        }

        public void OnSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            PrintSnapshot(buf);
        }

        public void OnUpdate<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            Console.Write("Flags:{0} SK:{1} ", buf.EventParams.Flags, buf.EventParams.SnapshotKey);
            foreach (var t in buf)
                Console.WriteLine($"{buf.Symbol} {t}");
        }

        private void PrintSnapshot<TE>(IDxEventBuf<TE> buf) {
            var symbolStr = buf.Symbol;
            Console.WriteLine($"Snapshot {buf.EventType} {{Symbol: '{symbolStr}', RecordsCount: {buf.Size}}}");
            var count = 0;
            foreach (var o in buf) {
                Console.WriteLine($"   {{ {o} }}");

                if (++count < recordsPrintLimit || recordsPrintLimit == 0) continue;

                Console.WriteLine($"   {{ ... {buf.Size - count} records left ...}}");
                break;
            }
        }
    }
}