#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;

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
    public interface IDXFeedTimeSeriesSubscription<E> : IDXFeedSubscription<E>
        where E : IDxTimeSeriesEvent
    {
        /// <summary>
        ///     Gets or sets the earliest timestamp from which time-series of events shall be
        ///     received.
        ///     The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
        /// </summary>
        long FromTimeStamp { get; set; }
    }
}
