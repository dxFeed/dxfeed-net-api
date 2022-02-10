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
using com.dxfeed.api.events;

namespace dxf_inc_order_snapshot_sample
{
    /// <summary>
    ///     Snapshots listener
    /// </summary>
    public class SnapshotListener : IDxIncOrderSnapshotListener
    {
        private readonly int recordsPrintLimit;

        public SnapshotListener(int recordsPrintLimit)
        {
            this.recordsPrintLimit = recordsPrintLimit;
        }

        #region Implementation of IDxIncOrderSnapshotListener

        /// <summary>
        ///     On Order snapshot or update event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        /// <param name="newSnapshot">Snapshot or update.</param>
        public void OnOrderSnapshot<TB, TE>(TB buf, bool newSnapshot)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            var symbolStr = buf.Symbol;
            Console.WriteLine("Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}, Type: {3}}}", buf.EventType, symbolStr,
                buf.Size, newSnapshot ? "full" : "update");
            var count = 0;
            foreach (var o in buf)
            {
                if (!o.EventFlags.HasFlag(EventFlag.RemoveEvent) && o.HasSize())
                    Console.WriteLine("   {{ {0} }}", o);
                else
                    Console.WriteLine("   REMOVAL {{ {0} }}", o.Index);

                if (++count < recordsPrintLimit || recordsPrintLimit == 0) continue;
                Console.WriteLine("   {{ ... {0} records left ...}}", buf.Size - count);
                break;
            }

            if (buf.Size > 0)
                Console.WriteLine();
        }

        #endregion //IDxIncOrderSnapshotListener
    }
}