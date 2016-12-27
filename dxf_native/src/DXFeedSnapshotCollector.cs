#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System.Collections.Generic;
using com.dxfeed.api.events;
using System.Threading;

namespace com.dxfeed.api
{
    /// <summary>
    /// Collector of snapshot events.
    /// 
    /// It is also listener of snapshot events that stores all events into 
    /// list. Events are updated with snapshot. You can get events list at any 
    /// time. This class supports only one snapshot.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    class DXFeedSnapshotCollector<E> :
        IDxOrderSnapshotListener,
        IDxCandleSnapshotListener,
        IDxTimeAndSaleSnapshotListener,
        IDxSpreadOrderSnapshotListener,
        IDxGreeksSnapshotListener,
        IDxSeriesSnapshotListener where E : IDxEventType
    {

        private List<E> events = new List<E>();
        private ReaderWriterLock rwl = new ReaderWriterLock();
        private const int LockTimeout = 5000;
        private volatile bool isDone = false;

        public DXFeedSnapshotCollector() { }

        public bool IsDone
        {
            get
            {
                return isDone;
            }
        }

        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            List<IDxCandle> list = new List<IDxCandle>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as List<E>);
        }

        public void OnGreeksSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            List<IDxGreeks> list = new List<IDxGreeks>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as List<E>);
        }

        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            List<IDxOrder> list = new List<IDxOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as List<E>);
        }

        public void OnSeriesSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            List<IDxSeries> list = new List<IDxSeries>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as List<E>);
        }

        public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            List<IDxSpreadOrder> list = new List<IDxSpreadOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as List<E>);
        }

        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            List<IDxTimeAndSale> list = new List<IDxTimeAndSale>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as List<E>);
        }

        /// <summary>
        /// Gets all collected events.
        /// </summary>
        public List<E> Events
        {
            get
            {
                List<E> result;
                rwl.AcquireReaderLock(LockTimeout);
                try
                {
                    result = new List<E>(events);
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
                return result;
            }
        }

        private void AddSnapshot(List<E> events)
        {
            rwl.AcquireWriterLock(LockTimeout);
            try
            {
                this.events = events;
                isDone = true;
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

    }
}
