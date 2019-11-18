#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using System;
using System.Collections.Generic;
using System.Threading;

namespace com.dxfeed.tests.tools
{
    /// <summary>
    /// Order view listener class for tests.
    /// Allow to get any parameters from received order view and transfer to test method.
    ///
    /// WARNING: this handler do not differ order snapshot by source sequence,
    /// i.e. "Order#NTV&DEX AAPL" and "Order#DEX&DEA AAPL" is the same snapshots
    /// </summary>
    public class OrderViewTestListener : IDxOrderViewListener
    {
        public class ReceivedOrderView
        {
            List<IDxOrder> snapshotEvents;
            List<IDxOrder> updates;

            public ReceivedOrderView(string symbol, IList<IDxOrder> events)
            {
                Symbol = symbol.ToString();
                snapshotEvents = new List<IDxOrder>(events);
                updates = new List<IDxOrder>();
            }

            public string Symbol { get; private set; }

            public IList<IDxOrder> Events
            {
                get
                {
                    return snapshotEvents;
                }
            }

            public IList<IDxOrder> Updates
            {
                get
                {
                    return updates;
                }
            }

            /// <summary>
            /// Note: just add events to common list
            /// </summary>
            /// <param name="eventParams"></param>
            /// <param name="events"></param>
            public void Update(EventParams eventParams, List<IDxOrder> events)
            {
                updates.AddRange(Events);
            }
        }

        Dictionary<string, ReceivedOrderView> orderViews = new Dictionary<string, ReceivedOrderView>();

        ReaderWriterLock rwl = new ReaderWriterLock();

        int lockTimeout = 1000;
        int eventsTimeout = 120000;
        int eventsSleepTime = 100;
        Func<bool> IsConnected = null;
        public const string COMPOSITE_BID = "COMPOSITE_BID";
        public const string COMPOSITE_ASK = "COMPOSITE_ASK";

        public OrderViewTestListener(int eventsTimeout, int eventsSleepTime, Func<bool> IsConnected)
        {
            this.eventsTimeout = eventsTimeout;
            this.eventsSleepTime = eventsSleepTime;
            this.IsConnected = IsConnected;
        }

        private void AddSnapshot(ReceivedOrderView orderView)
        {
            rwl.AcquireWriterLock(lockTimeout);
            try
            {
                orderViews[orderView.Symbol] = orderView;
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        private void AddUpdate(string symbol, EventParams eventParams, List<IDxOrder> events)
        {
            rwl.AcquireWriterLock(lockTimeout);
            try
            {
                if (orderViews.ContainsKey(symbol))
                    orderViews[symbol].Update(eventParams, events);
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        public void ClearEvents()
        {
            rwl.AcquireWriterLock(lockTimeout);
            try
            {
                orderViews.Clear();
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Returns number of snapshot objects
        /// </summary>
        /// <returns>number of snapshot objects</returns>
        public int GetOrderViewsCount()
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                return orderViews.Count;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        public int GetOrderViewsCount(params string[] symbols)
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                List<string> symbolList = new List<string>(symbols);
                int count = 0;
                foreach (ReceivedOrderView ov in orderViews.Values)
                    if (symbolList.Contains(ov.Symbol))
                        count++;
                return count;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        public int GetOrderViewEventsCount(string symbol)
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                Dictionary<string, ReceivedOrderView> dict = orderViews;
                if (dict == null)
                    return 0;
                foreach (ReceivedOrderView orderView in dict.Values)
                {
                    if (!orderView.Symbol.Equals(symbol))
                        continue;
                    return orderView.Events.Count;
                }
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
            return 0;
        }

        public int GetOrderViewUpdatesCount(string symbol)
        {
            rwl.AcquireReaderLock(lockTimeout);
            try
            {
                Dictionary<string, ReceivedOrderView> dict = orderViews;
                if (dict == null)
                    return 0;
                foreach (ReceivedOrderView orderView in dict.Values)
                {
                    if (!orderView.Symbol.Equals(symbol))
                        continue;
                    return orderView.Updates.Count;
                }
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
            return 0;
        }

        #region IDxOrderViewListener implementation

        public void OnSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            List<IDxOrder> list = new List<IDxOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddSnapshot(new ReceivedOrderView(buf.Symbol, list));
        }

        public void OnUpdate<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            List<IDxOrder> list = new List<IDxOrder>();
            foreach (var o in buf)
                list.Add(o);
            AddUpdate(buf.Symbol.ToString(), buf.EventParams, list);
        }

        #endregion //IDxOrderViewListener implementation
    }
}
