using System;
using com.dxfeed.api.orderview;
using com.dxfeed.api.events;

namespace dxf_order_view_sample
{
    class OrderViewEventListener : IDxOrderViewListener {
        private const int recordsPrintlimit = 7;

        private void PrintSnapshot<TE>(IDxEventBuf<TE> buf) {
            string symbolStr = buf.Symbol.ToString();
            Console.WriteLine(string.Format("Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}",
                buf.EventType, symbolStr, buf.Size));
            int count = 0;
            foreach (var o in buf) {
                Console.WriteLine(string.Format("   {{ {0} }}", o));
                if (++count >= recordsPrintlimit) {
                    Console.WriteLine(string.Format("   {{ ... {0} records left ...}}", buf.Size - count));
                    break;
                }
            }
        }

        public void OnSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {

            PrintSnapshot(buf);
        }

        public void OnUpdate<TB, TE>(TB buf)
            where TB : IDxEventBuf<TB>
            where TE : IDxOrder {

            foreach (var t in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, t));
        }
    }
}
