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
using System;

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
        IDxSeriesSnapshotListener,
        IDXFeedEventListener<E>
        where E : IDxEventType
    {

        private IList<E> events = new List<E>();
        private object eventsLock = new object();
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
            IList<IDxCandle> list = new List<IDxCandle>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as IList<E>);
        }

        public void OnGreeksSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            IList<IDxGreeks> list = new List<IDxGreeks>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as IList<E>);
        }

        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            IList<IDxOrder> list = new List<IDxOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as IList<E>);
        }

        public void OnSeriesSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            IList<IDxSeries> list = new List<IDxSeries>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as IList<E>);
        }

        public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            IList<IDxSpreadOrder> list = new List<IDxSpreadOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as IList<E>);
        }

        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            IList<IDxTimeAndSale> list = new List<IDxTimeAndSale>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(list as IList<E>);
        }

        public void EventsReceived(IList<E> events)
        {
            AddSnapshot(events);
        }

        /// <summary>
        /// Gets all collected events.
        /// </summary>
        public List<E> Events
        {
            get
            {
                List<E> result;
                lock(eventsLock)
                {
                    result = new List<E>(events);
                }
                return result;
            }
        }

        protected virtual IList<E> FilterEvents(IList<E> events)
        {
            return events;
        }

        private void AddSnapshot(IList<E> events)
        {
            if (events == null)
                throw new ArgumentNullException("events");
            lock (eventsLock)
            {
                this.events = FilterEvents(events);
                isDone = true;
            }
        }
    }
}
