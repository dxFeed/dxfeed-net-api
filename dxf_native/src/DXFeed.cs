#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.api.util;
using com.dxfeed.native;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.dxfeed.api
{
    public class DXFeed : IDisposable
    {

        //TODO: restore AssemblyInfo versions in all projects
        //TODO: update new samples with MSBuild commands

        private static readonly string DEFAULT_ADDRESS = "demo.dxfeed.com:7300";
        private static readonly string DEFAULT_USER = "demo";
        private static readonly string DEFAULT_PASSWORD = "demo";

        private static DXFeed dxFeedInstance = null;

        private IDxConnection connectionInstance = null;
        private string address = string.Empty;
        private HashSet<object> attachedSubscriptions = new HashSet<object>();

        private DXFeed(string address)
        {
            this.address = address;
            connectionInstance = new NativeConnection(address, OnDisconnect);
        }

        /// <summary>
        /// Returns a default application-wide singleton instance of feed. Most applications use only a single
        /// data-source and should rely on this method to get one.
        /// </summary>
        /// <returns>Singleton instance of feed.</returns>
        public static DXFeed GetInstance()
        {
            //TODO: connection to non-default address
            if (dxFeedInstance == null)
                dxFeedInstance = new DXFeed(DEFAULT_ADDRESS);
            return dxFeedInstance;
        }

        /// <summary>
        /// Creates new subscription for a single event type that is attached to this feed.
        /// For multiple event types in one subscription use
        /// createSubscription(Class... eventTypes)
        /// This method creates new DXFeedSubscription.
        ///
        /// @see DXFeedSubscription#DXFeedSubscription(Class)
        /// @see #attachSubscription(DXFeedSubscription)
        /// </summary>
        /// <typeparam name="E">The type of events.</typeparam>
        /// <param name="eventType">The class of event types.</param>
        /// <returns>New DXFeedSubscription for a single event type.</returns>
        public DXFeedSubscription<E> CreateSubscription<E>()
        {
            DXFeedSubscription<E> subscription = new DXFeedSubscription<E>(connectionInstance);
            AttachSubscription(subscription);
            return subscription;
        }

        /// <summary>
        /// Creates new subscription for multiple event types that is <i>attached</i> to this feed.
        /// For a single event type use CreateSubscription<E>().
        /// This method creates new DXFeedSubscription and invokes AttachSubscription.
        /// </summary>
        /// <typeparam name="E">The type of events.</typeparam>
        /// <param name="eventTypes">The classes of event types.</param>
        /// <returns>The new DXFeedSubscription.</returns>
        public DXFeedSubscription<E> CreateSubscription<E>(params Type[] eventTypes)
        {
            DXFeedSubscription<E> subscription = new DXFeedSubscription<E>(connectionInstance, eventTypes);
            AttachSubscription(subscription);
            return subscription;
        }

        /// <summary>
        /// Attaches the given subscription to this feed. This method does nothing if the
        /// corresponding subscription is already attached to this feed.
        /// This feed publishes data to the attached subscription.
        /// 
        /// Application can attach DXFeedEventListener via DXFeedSubscription.AddEventListener 
        /// to get notified about data changes and can change its data subscription via 
        /// DXFeedSubscription methods.
        /// </summary>
        /// <typeparam name="E">The type of events.</typeparam>
        /// <param name="subscription">The subscription.</param>
        public void AttachSubscription<E>(DXFeedSubscription<E> subscription)
        {
            if (attachedSubscriptions.Contains(subscription))
                return;
            attachedSubscriptions.Add(subscription);
        }

        /// <summary>
        /// Detaches the given subscription from this feed. This method does nothing if the
        /// corresponding subscription is not attached to this feed.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        public void DetachSubscription<E>(DXFeedSubscription<E> subscription)
        {
            attachedSubscriptions.Remove(subscription);
        }

        private static void OnDisconnect(IDxConnection con)
        {
            //TODO: todo
        }

        public void Dispose()
        {
            if (connectionInstance != null)
                connectionInstance.Dispose();
        }


        /**
	     * Requests time series of events for the specified event type, symbol, and a range of time.
	     * This method works only for event types that implement {@link TimeSeriesEvent} interface.
	     * This method requests the data from the the uplink data provider,
	     * creates a list of events of the specified {@code eventType},
	     * and {@link Promise#complete(Object) completes} the resulting promise with this list.
	     * The events are ordered by {@link TimeSeriesEvent#getTime() time} in the list.
	     *
	     * <p> This method is designed for retrieval of a snapshot only.
	     * Use {@link TimeSeriesEventModel} if you need a list of time-series events that updates in real time.
	     *
	     * <p>The range and depth of events that are available with this service is typically constrained by
	     * upstream data provider.
	     *
	     * <p>The promise is {@link Promise#cancel() cancelled} when the the underlying {@link DXEndpoint} is
	     * {@link DXEndpoint#close() closed}.
	     * If events are not available for any transient reason (no subscription, no connection to uplink, etc),
	     * then the resulting promise completes when the issue is resolved, which may involve an arbitrarily long wait.
	     * Use {@link Promise#await(long, TimeUnit)} method to specify timeout while waiting for promise to complete.
	     * If events are permanently not available (not supported), then the promise
	     * {@link Promise#completeExceptionally(Throwable) completes exceptionally} with {@link IllegalArgumentException}.
	     *
	     * <p>Note, that this method does not work when {@link DXEndpoint} was created with
	     * {@link DXEndpoint.Role#STREAM_FEED STREAM_FEED} role (promise completes exceptionally).
	     *
	     * <p>This method does not accept an instance of {@link TimeSeriesSubscriptionSymbol} as a {@code symbol}.
	     * The later class is designed for use with {@link DXFeedSubscription} and to observe time-series subscription
	     * in {@link DXPublisher}.
	     *
	     * <h3>Event flags</h3>
	     *
	     * This method completes promise only when a consistent snapshot of time series has been received from
	     * the data feed. The {@link IndexedEvent#getEventFlags() eventFlags} property of the events in the resulting list
	     * is always zero.
	     *
	     * <p>Note, that the resulting list <em>should not</em> be used with
	     * {@link DXPublisher#publishEvents(Collection) DXPublisher.publishEvents} method, because the later expects
	     * events in a different order and with an appropriate flags set. See documentation on a specific event class
	     * for details on how they should be published.
	     *
	     * <h3>Threads</h3>
	     *
	     * Use {@link Promise#whenDone(PromiseHandler) Promise.whenDone} method on the resulting promise to receive
	     * notification when the promise becomes {@link Promise#isDone() done}. This notification is invoked
	     * from inside this {@link DXEndpoint DXEndpoint} {@link DXEndpoint#executor(Executor) executor} thread.
	     *
	     * @param eventType the event type.
	     * @param symbol the symbol
	     * @param fromTime the time, inclusive, to request events from (see {@link TimeSeriesEvent#getTime() TimeSeriesEvent.getTime}).
	     * @param toTime the time, inclusive, to request events to (see {@link TimeSeriesEvent#getTime() TimeSeriesEvent.getTime}).
	     *               Use {@link Long#MAX_VALUE Long.MAX_VALUE} to retrieve events without an upper limit on time.
	     * @param <E> the type of event.
	     * @return the promise for the result of the request.
	     * @throws NullPointerException if the eventType or symbol are null.
	     */
        public async Task<List<E>> GetTimeSeriesPromise<E>(object symbol,
            long fromTime, long toTime, CancellationToken cancellationToken) 
            where E : TimeSeriesEvent
        {
            if (symbol == null)
                throw new NullReferenceException();

            //TODO: fetch day method here

            return await FetchOrSubscribeFromHistory<E>(symbol, fromTime, fromTime, toTime, cancellationToken);
        }

        /* private methods */

        private async Task<List<E>> FetchOrSubscribeFromHistory<E>(object symbol, 
            long fetchTime, long fromTime, long toTime, CancellationToken cancellationToken) 
            where E : IndexedEvent
        {
            return await Task.Run(() =>
            {
                EventType events = EventTypeUtil.GetEventsType(typeof(E));
                DXFeedSnapshotCollector<E> collector = new DXFeedSnapshotCollector<E>();

                using (var con = new NativeConnection(address, OnDisconnect))
                {
                    using (var s = con.CreateSnapshotSubscription(events, fromTime, collector))
                    {
                        if (typeof(E) == typeof(IDxCandle))
                            s.AddSymbol(symbol as CandleSymbol);
                        else
                            s.AddSymbol(symbol as string);

                        while(!collector.IsDone)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }

                return collector.Events;
            }, cancellationToken);
        }

    }
}
