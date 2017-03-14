#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_order_view_sample
{
    class OrderViewEventListener : IDxOrderViewListener
    {
        private const int recordsPrintlimit = 7;

        private void PrintSnapshot<TE>(IDxEventBuf<TE> buf)
        {
            string symbolStr = buf.Symbol.ToString();
            Console.WriteLine(string.Format("Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}",
                buf.EventType, symbolStr, buf.Size));
            int count = 0;
            foreach (var o in buf)
            {
                Console.WriteLine(string.Format("   {{ {0} }}", o));
                if (++count >= recordsPrintlimit)
                {
                    Console.WriteLine(string.Format("   {{ ... {0} records left ...}}", buf.Size - count));
                    break;
                }
            }
        }

        public void OnSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {

            PrintSnapshot(buf);
        }

        public void OnUpdate<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            Console.Write("Flags:{0} SK:{1} ", buf.EventParams.Flags, buf.EventParams.SnapshotKey);
            foreach (var t in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, t));
        }
    }
}
