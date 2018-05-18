#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System.Collections;
using System.Collections.Generic;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native
{
    /// <summary>
    ///   Simple buffer to collect events
    /// </summary>
    /// <remarks>
    ///   This buffer correctly works only with <see cref="IDxOrder"/> type events
    /// </remarks>
    /// <typeparam name="T">Type of event</typeparam>
    class EventBuffer<T> : IDxEventBuf<T>
    {
        private readonly EventType type;
        private readonly string symbol;
        private EventParams eventParams;
        private List<T> events = new List<T>();

        internal EventBuffer(EventType type, string symbol, EventParams eventParams)
        {
            this.type = type;
            this.symbol = symbol;
            this.eventParams = eventParams;
        }

        /// <summary>
        ///   Adds an object to the end of the buffer
        /// </summary>
        /// <remarks>
        ///   Try to minimize of use of this method because it can lead to the appearance of the same indexes doubled.
        ///   Use <see cref="ReplaceOrAdd(T)"/> instead.
        /// </remarks>
        /// <param name="_event">The object to be added to the end of the buffer</param>
        internal void AddEvent(T _event)
        {
            events.Add(_event);
        }

        internal void Clear()
        {
            events.Clear();
        }

        /// <summary>
        ///   Replaces or adds an object to the end of buffer. This method checks index of object/
        /// </summary>
        /// <param name="_event">The new object wich will replace old or to be added to the end of the buffer</param>
        internal void ReplaceOrAdd(T _event)
        {
            if (_event is IDxOrder)
            {
                int index = events.FindIndex(x => (x as IDxOrder).Index == (_event as IDxOrder).Index);
                if (index == -1)
                {
                    events.Add(_event);
                }
                else
                {
                    events[index] = _event;
                }
            }
        }

        internal void Remove(T _event)
        {
            if (_event is IDxOrder)
            {
                int index = events.FindIndex(x => (x as IDxOrder).Index == (_event as IDxOrder).Index);
                if (index != -1)
                {
                    events.RemoveAt(index);
                }
            }
        }


        #region Implementation of IDxEventBuf<out T>

        public EventType EventType
        {
            get { return type; }
        }

        public string Symbol
        {
            get { return symbol; }
        }

        public int Size
        {
            get { return events.Count; }
        }

        public EventParams EventParams
        {
            get { return eventParams; }
            internal set { eventParams = value; }
        }

        #endregion

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return events.GetEnumerator();
        }
        #endregion
    }
}
