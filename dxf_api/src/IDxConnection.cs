#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System;
using com.dxfeed.api.data;
using System.Collections.Generic;

namespace com.dxfeed.api
{
    /// <summary>
    ///   Interface provides operations with event subscription
    /// </summary>
    public interface IDxConnection : IDisposable
    {
        /// <summary>
        ///   Disconnect from the server
        /// </summary>
        void Disconnect();

        /// <summary>
        ///   Create event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSubscription(EventType type, IDxEventListener listener);

        /// <summary>
        ///   Create candle event subscription
        /// </summary>
        /// <param name="time">date time in the past</param>
        /// <param name="listener">candle listener callback</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSubscription(DateTime? time, IDxCandleListener listener);

        /// <summary>
        ///   Create time event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="time">Unix time stamp (the number of milliseconds from 1.1.1970)</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSubscription(EventType type, long time, IDxEventListener listener);

        /// <summary>
        /// Create time event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="time">Date time in the past.</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSubscription(EventType type, DateTime? time, IDxEventListener listener);

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
        /// <param name="listener"></param>
        /// <returns></returns>
        IDxSubscription CreateSnapshotSubscription(Int64 time, IDxSnapshotListener listener);

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="time">Date time in the past</param>
        /// <param name="listener"></param>
        /// <returns></returns>
        IDxSubscription CreateSnapshotSubscription(DateTime? time, IDxSnapshotListener listener);

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="eventType">Single event type.</param>
        /// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSnapshotSubscription(EventType eventType, Int64 time,
            IDxSnapshotListener listener);

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="eventType">Single event type.</param>
        /// <param name="time">Date time in the past</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSnapshotSubscription(EventType eventType, DateTime? time,
            IDxSnapshotListener listener);

        /// <summary>
        ///   Creates Order View subscription
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        IDxSubscription CreateOrderViewSubscription(IDxOrderViewListener listener);

        /// <summary>
        ///   Creates regional book
        /// </summary>
        /// <param name="symbol">Single symbol name</param>
        /// <param name="book_listener">Regional book changes listener. Null is allowed.</param>
        /// <param name="quote_listener">Quotes listener. Null is allowed.</param>
        /// <returns>regional book object</returns>
        IDxRegionalBook CreateRegionalBook(string symbol, IDxRegionalBookListener book_listener,
            IDxQuoteListener quote_listener);

        /// <summary>
        ///   Add dumping raw data of incoming traffic of connection into specific file
        /// </summary>
        /// <param name="rawFileName">file name for raw data</param>
        /// <exception cref="ArgumentException">Invalid argument <c>rawFileName</c></exception>
        /// <exception cref="NativeDxException"></exception>
        void WriteRawData(string rawFileName);

        /// <summary>
        ///   Retrieves properties for specified connection. Properties can be changed during
        ///   reconnection. So returned array is a snapshot of properties at the moment of the call.
        /// </summary>
        IDictionary<string, string> Properties { get; }

        /// <summary>
        ///   Retrieves string with current connected address in format <host>:<port>. If return value is NULL
        ///   then connection is not connected (reconnection, no valid addresses, closed connection and others).
        /// </summary>
        string ConnectedAddress { get; }
    }
}
