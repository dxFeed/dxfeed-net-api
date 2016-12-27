/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

namespace com.dxfeed.api.events
{
    //TODO: need a another solution?

    /// <summary>
    /// Marks all event types that can be received via dxFeed API.
    /// Events are considered instantaneous, non-persistent, and unconflateable
    /// (each event is individually delivered) unless they implement one of interfaces
    /// defined in this package to further refine their meaning.
    ///
    /// <p>Event types are POJOs (plain old java objects) that follow bean naming convention with
    /// getters and setters for their properties.
    /// All event types are serializable, because they are transferred over network from publishers to
    /// data feed consumers. However, they are using custom serialization format for this purpose.
    /// </summary>
    public interface IDxEventType
    {
        ///// <summary>
        ///// Returns event symbol that identifies this event type.
        ///// </summary>
        //object EventSymbol { get; }

        //TODO: Is need EventSymbol here?
    }

    /// <summary>
    /// Marks all event types that can be received via dxFeed API.
    /// Events are considered instantaneous, non-persistent, and unconflateable
    /// (each event is individually delivered) unless they implement one of interfaces
    /// defined in this package to further refine their meaning.
    ///
    /// <p>Event types are POJOs (plain old java objects) that follow bean naming convention with
    /// getters and setters for their properties.
    /// All event types are serializable, because they are transferred over network from publishers to
    /// data feed consumers. However, they are using custom serialization format for this purpose.
    /// </summary>
    public interface IDxEventType<T> : IDxEventType
    {
        /// <summary>
        /// Returns event symbol that identifies this event type.
        /// </summary>
        T EventSymbol { get; }
    }
}
