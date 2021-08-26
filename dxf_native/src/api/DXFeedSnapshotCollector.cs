#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;
using System;
using System.Collections.Generic;

namespace com.dxfeed.api
{
    /// <summary>
    ///     <para>
    ///         Collector of snapshot events.
    ///     </para>
    ///     <para>
    ///         It is also listener of snapshot events that stores all events into list. Events
    ///         are updated with snapshot. You can get events list at any time. This class
    ///         supports only one snapshot.
    ///     </para>
    /// </summary>
    /// <typeparam name="E">The event type.</typeparam>
    [Obsolete("DXFeedSnapshotCollector class is deprecated and will removed in 9.0.0 version. Please use NativeConnection\\NativeSubscription")]
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DXFeedSnapshotCollector() { }

        /// <summary>
        /// Returns <c>true</c> if collector contains full snapshot.
        /// </summary>
        public bool IsDone
        {
            get
            {
                return isDone;
            }
        }

        /// <summary>
        /// Gets all collected events of this snapshot.
        /// </summary>
        public List<E> Events
        {
            get
            {
                List<E> result;
                lock (eventsLock)
                {
                    result = new List<E>(events);
                }
                return result;
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
