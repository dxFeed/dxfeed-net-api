/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;

namespace dxf_snapshot_sample
{
    /// <summary>
    /// Snapshots listener
    /// </summary>
    public class SnapshotListener :
        IDxOrderSnapshotListener,
        IDxCandleSnapshotListener,
        IDxTimeAndSaleSnapshotListener,
        IDxSpreadOrderSnapshotListener
    {

        private const int recordsPrintlimit = 7;

        private void PrintSnapshot<TE>(IDxEventBuf<TE> buf)
        {
            string symbolStr = buf.Symbol.ToString();
            if (buf.EventType == EventType.Candle)
            {
                CandleSymbol candleSymbol = CandleSymbol.ValueOf(symbolStr);
                if (candleSymbol.IsDefault())
                    symbolStr = candleSymbol.ToFullString();
            }
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

        #region Implementation of IDxOrderSnapshotListener

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

        #endregion //IDxOrderSnapshotListener

        #region IDxCandleSnapshotListener

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

        #endregion //IDxCandleSnapshotListener

        #region IDxTimeAndSaleSnapshotListener

        /// <summary>
        /// On TimeAndSale snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {

            PrintSnapshot(buf);
        }

        #endregion //IDxTimeAndSaleSnapshotListener

        #region  IDxSpreadOrderSnapshotListener

        /// <summary>
        /// On SpreadOrder snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {

            PrintSnapshot(buf);
        }

        #endregion
    }
}
