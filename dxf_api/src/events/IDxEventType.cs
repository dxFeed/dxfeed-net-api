#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    ///     <para>
    ///         Marks all event types that can be received via dxFeed API.
    ///         Events are considered instantaneous, non-persistent, and unconflateable
    ///         (each event is individually delivered) unless they implement one of interfaces
    ///         defined in this package to further refine their meaning.
    ///     </para>
    ///     <para>
    ///         Event types are plain objects that follow bean naming convention with
    ///         getters and setters for their properties.
    ///         All event types are serializable, because they are transferred over network from
    ///         publishers to data feed consumers. However, they are using custom serialization
    ///         format for this purpose.
    ///     </para>
    /// </summary>
    public interface IDxEventType : ICloneable
    {
        /// <summary>
        ///     Returns event symbol that identifies this event type.
        /// </summary>
        object EventSymbol { get; }
    }

    /// <summary>
    ///     <para>
    ///         Marks all event types that can be received via dxFeed API.
    ///         Events are considered instantaneous, non-persistent, and unconflateable
    ///         (each event is individually delivered) unless they implement one of interfaces
    ///         defined in this package to further refine their meaning.
    ///     </para>
    ///     <para>
    ///         Event types are plain objects that follow bean naming convention with
    ///         getters and setters for their properties.
    ///         All event types are serializable, because they are transferred over network from
    ///         publishers to data feed consumers. However, they are using custom serialization
    ///         format for this purpose.
    ///     </para>
    /// </summary>
    /// <typeparam name="T">Type of the event symbol for this event type.</typeparam>
    public interface IDxEventType<T> : IDxEventType
    {
        /// <summary>
        ///     Returns event symbol that identifies this event type.
        /// </summary>
        new T EventSymbol { get; }
    }
}
