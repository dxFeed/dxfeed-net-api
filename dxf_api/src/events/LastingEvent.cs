﻿#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

namespace com.dxfeed.api.events
{
    /// <summary>
    /// <para>
    /// Represents up-to-date information about some
    /// condition or state of an external entity that updates in real-time. For example,
    /// a <see cref="IDxQuote"/> is an up-to-date information about best bid and best offer for
    /// a specific symbol.
    /// </para>
    ///
    /// <para>
    /// Lasting events are conflated for
    /// each symbol. Last event for each symbol is always delivered to event listeners on
    /// subscription, but intermediate (next-to-last) events are not queued anywhere,
    /// they are simply discarded as stale events. More recent events
    /// represent an up-to-date information about some external entity.
    /// </para>
    ///
    /// <para>
    /// Lasting events can be used with {@link DXFeed#getLastEvent(LastingEvent) DXFeed.getLastEvent}
    /// and {@link DXFeed#getLastEvents(Collection) DXFeed.getLastEvents}
    /// methods to retrieve last events for each symbol.
    /// </para>
    ///
    /// <para>
    /// Note, that subscription to all lasting events of a specific type via
    /// {@link WildcardSymbol#ALL WildcardSymbol.ALL} symbol object does not benefit from the above
    /// advantages of lasting events.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of the event symbol for this event type.</typeparam>
    public interface LastingEvent<T> : IDxEventType<T>
    {
        //Note: no extra fields there
    }
    //TODO: comments
}
