#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;
using com.dxfeed.api.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.dxfeed.api
{
    /// <summary>
    ///     <para>
    ///         Extends <see cref="IDXFeedSubscription{E}"/> to conveniently subscribe to
    ///         time-series of events for a set of symbols and event types.
    ///     </para>
    ///     <para>
    ///         Only events that implement <see cref="IDxTimeSeriesEvent{T}"/> interface can be
    ///         subscribed to with <see cref="IDXFeedTimeSeriesSubscription{E}"/>.
    ///     </para>
    ///     <para>
    ///         From time
    ///     </para>
    ///     <para>
    ///         The value of <see cref="FromTimeStamp"/> property defines the time-span of
    ///         events that are subscribed to. Only events that satisfy
    ///         <c>event.TimeStamp >= thisSubscription.FromTime</c> are looked for.
    ///     </para>
    ///     <para>
    ///         The value <see cref="FromTimeStamp"/> is initially set to <see cref="long.MaxValue"/>
    ///         with a special meaning that no events will be received until <c>FromTime</c>
    ///         is changed with <see cref="FromTimeStamp"/> setter.
    ///     </para>
    ///     <para>
    ///         Threads and locks.
    ///     </para>
    ///     <para>
    ///         This class is thread-safe and can be used concurrently from multiple threads
    ///         without external synchronization.
    ///     </para>
    /// </summary>
    /// <typeparam name="E">The type of events.</typeparam>
    public class DXFeedTimeSeriesSubscription<E> : DXFeedSubscription<E>, IDXFeedTimeSeriesSubscription<E>
        where E : IDxTimeSeriesEvent
    {
        /// <summary>
        ///     Creates detached time-series subscription for a single event type.
        /// </summary>
        /// <param name="endpoint">The <see cref="DXEndpoint"/> instance.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="endpoint"/> is null.</exception>
        /// <exception cref="ArgumentException">If type E is not event class.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        public DXFeedTimeSeriesSubscription(DXEndpoint endpoint) : base()
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            endpointInstance = endpoint;
            endpoint.OnClosing += Endpoint_OnClosing;

            subscriptionInstance = endpoint.Connection.CreateSubscription(
                EventTypeUtil.GetEventsType(typeof(E)),
                FromTimeStamp,
                new DXFeedEventHandler<E>(eventListeners, eventListenerLocker));
        }

        /// <summary>
        ///     Creates detached subscription for the given list of event types.
        /// </summary>
        /// <param name="endpoint">The <see cref="DXEndpoint"/> instance.</param>
        /// <param name="eventTypes">The list of event types.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="endpoint"/> or <paramref name="eventTypes"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="eventTypes"/> are empty or any type of
        ///     <paramref name="eventTypes"/> is not event class.
        /// </exception>
        /// <exception cref="DxException">Internal error.</exception>
        public DXFeedTimeSeriesSubscription(DXEndpoint endpoint, params Type[] eventTypes) : base(eventTypes)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            foreach (Type t in eventTypes)
                if (!typeof(E).IsAssignableFrom(t))
                    throw new ArgumentException(string.Format("The type {0} is not {1}", t, typeof(E)));

            endpointInstance = endpoint;
            endpoint.OnClosing += Endpoint_OnClosing;

            subscriptionInstance = endpoint.Connection.CreateSubscription(
                EventTypeUtil.GetEventsType(eventTypes),
                FromTimeStamp,
                new DXFeedEventHandler<E>(eventListeners, eventListenerLocker));
        }

        /// <summary>
        ///     Gets or sets the earliest timestamp from which time-series of events shall be
        ///     received.
        ///     The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        public long FromTimeStamp
        {
            get
            {
                long result;
                lock (timeLocker)
                {
                    result = fromTime;
                }
                return result;
            }
            set
            {
                lock (timeLocker)
                {
                    fromTime = value;

                    ISet<object> symbols = GetSymbols();

                    subscriptionInstance.Dispose();
                    subscriptionInstance = endpointInstance.Connection.CreateSubscription(
                        EventTypeUtil.GetEventsType(EventTypes.ToArray()),
                        fromTime,
                        new DXFeedEventHandler<E>(eventListeners, eventListenerLocker));
                    AddSymbols(false, symbols.ToArray());
                }
            }
        }

        private long fromTime = long.MaxValue;
        private DXEndpoint endpointInstance;
        private object timeLocker = new object();
    }
}
