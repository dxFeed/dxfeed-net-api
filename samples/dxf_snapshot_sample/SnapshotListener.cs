using System;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_snapshot_sample
{
    /// <summary>
    /// Snapshots listener
    /// </summary>
    public class SnapshotListener : IDxSnapshotListener
    {
        #region Implementation of IDxSnapshotListener

        private const int recordsPrintlimit = 7;

        private void PrintSnapshot<TE>(IDxEventBuf<TE> buf)
        {
            Console.WriteLine(string.Format("Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}",
                buf.EventType, buf.Symbol, buf.Size));
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

        /// <summary>
        /// On Order snapshot event received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            PrintSnapshot(buf);
        }

        /// <summary>
        /// On Candle shopshot event received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            PrintSnapshot(buf);
        }

        #endregion
    }
}
