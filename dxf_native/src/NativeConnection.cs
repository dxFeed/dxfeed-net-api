#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.native.api;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using com.dxfeed.api.connection;
using com.dxfeed.api.events;

namespace com.dxfeed.native
{
    /// <summary>
    /// The handler type of a connection's incoming heartbeat notification
    ///
    /// Called when a server heartbeat arrives and contains non empty payload
    /// </summary>
    /// <param name="connection">The current connection</param>
    /// <param name="serverDateTime">The server UTC time (from the incoming heartbeat payload)</param>
    /// <param name="serverLagMark">The server's messages composing lag time in microseconds (from the incoming heartbeat payload)</param>
    /// <param name="connectionRtt">The calculated connection RTT in microseconds</param>
    public delegate void OnServerHeartbeatHandler(IDxConnection connection, DateTime serverDateTime, int serverLagMark,
        int connectionRtt);

    /// <inheritdoc />
    /// <summary>
    ///   Class provides operations with event subscription
    /// </summary>
    public class NativeConnection : IDxConnection
    {
        private IntPtr handle;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly C.dxf_conn_termination_notifier_t terminationNotifier;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly C.dxf_conn_status_notifier_t connectionStatusChangeHandler;
        private readonly C.dxf_socket_thread_creation_notifier_t creationNotifier;
        private C.ConnectionOnServerHeartbeatNotifier onServerHeartbeatNotifier;
        private readonly Action<IDxConnection> disconnectListener;
        private readonly Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener;
        private OnServerHeartbeatHandler onServerHeartbeatHandler;
        private readonly ISet<IDxSubscription> subscriptions = new HashSet<IDxSubscription>();

        public delegate void OnCreationEventHandler(object sender, EventArgs e);

        public event OnCreationEventHandler OnCreation;

        internal IntPtr Handle => handle;

        /// <summary>
        /// Creates the new connection
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="address">The server address to connect</param>
        /// <param name="disconnectListener">The listener that will be called when the connection is interrupted</param>
        /// <exception cref="DxException"></exception>
        public NativeConnection(string address, Action<IDxConnection> disconnectListener)
        {
            this.terminationNotifier = OnDisconnect;
            this.connectionStatusChangeHandler = ConnectionStatusChangeHandler;
            this.creationNotifier = OnNativeCreate;
            this.disconnectListener = disconnectListener;
            C.CheckOk(C.Instance.dxf_create_connection(address, terminationNotifier, connectionStatusChangeHandler,
                null, null, IntPtr.Zero, out handle));
        }

        /// <summary>
        /// Creates the new connection
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="address">The server address to connect</param>
        /// <param name="disconnectListener">The listener that will be called when the connection is interrupted</param>
        /// <param name="connectionStatusListener">The listener that will be called when the connection status is changed</param>
        /// <exception cref="DxException"></exception>
        public NativeConnection(string address, Action<IDxConnection> disconnectListener,
            Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener)
        {
            this.terminationNotifier = OnDisconnect;
            this.connectionStatusChangeHandler = ConnectionStatusChangeHandler;
            this.creationNotifier = OnNativeCreate;
            this.disconnectListener = disconnectListener;
            this.connectionStatusListener = connectionStatusListener;
            C.CheckOk(C.Instance.dxf_create_connection(address, terminationNotifier, connectionStatusChangeHandler,
                null, null, IntPtr.Zero, out handle));
        }

        /// <summary>
        /// Creates the new connection.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="address">The server address to connect.</param>
        /// <param name="credential">The user name and password to server access.</param>
        /// <param name="disconnectListener">The listener that will be called when the connection is interrupted.</param>
        /// <param name="connectionStatusListener">The listener that will be called when the connection status is changed</param>
        /// <exception cref="DxException">The connection creation was failed.</exception>
        public NativeConnection(string address, System.Net.NetworkCredential credential,
            Action<IDxConnection> disconnectListener,
            Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener)
        {
            this.terminationNotifier = OnDisconnect;
            this.connectionStatusChangeHandler = ConnectionStatusChangeHandler;
            this.disconnectListener = disconnectListener;
            this.connectionStatusListener = connectionStatusListener;
            C.CheckOk(C.Instance.dxf_create_connection_auth_basic(address, credential.UserName, credential.Password,
                terminationNotifier, connectionStatusChangeHandler, null, null, IntPtr.Zero, out handle));
        }

        /// <summary>
        /// Creates the new connection.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="address">The Server address to connect.</param>
        /// <param name="token">Bearer scheme token to server access.</param>
        /// <param name="disconnectListener">The listener that will be called when the connection is interrupted.</param>
        /// <exception cref="DxException">The connection creation was failed.</exception>
        public NativeConnection(string address, string token, Action<IDxConnection> disconnectListener)
        {
            this.terminationNotifier = OnDisconnect;
            this.connectionStatusChangeHandler = ConnectionStatusChangeHandler;
            this.disconnectListener = disconnectListener;
            C.CheckOk(C.Instance.dxf_create_connection_auth_bearer(address, token, terminationNotifier,
                connectionStatusChangeHandler, null, null,
                IntPtr.Zero, out handle));
        }

        /// <summary>
        /// Creates the new connection.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="address">Server address to connect.</param>
        /// <param name="token">Bearer scheme token to server access.</param>
        /// <param name="disconnectListener">The listener that will be called when the connection is interrupted.</param>
        /// <param name="connectionStatusListener">The listener that will be called when the connection status is changed</param>
        /// <exception cref="DxException">The connection creation was failed.</exception>
        public NativeConnection(string address, string token, Action<IDxConnection> disconnectListener,
            Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener)
        {
            this.terminationNotifier = OnDisconnect;
            this.connectionStatusChangeHandler = ConnectionStatusChangeHandler;
            this.disconnectListener = disconnectListener;
            this.connectionStatusListener = connectionStatusListener;
            C.CheckOk(C.Instance.dxf_create_connection_auth_bearer(address, token, terminationNotifier,
                connectionStatusChangeHandler, null, null,
                IntPtr.Zero, out handle));
        }

        /// <summary>
        /// Creates the new connection.
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="address">The server address to connect.</param>
        /// <param name="authScheme">The authorization scheme.</param>
        /// <param name="authData">The authorization data.</param>
        /// <param name="disconnectListener">The listener that will be called when the connection is interrupted.</param>
        /// <param name="connectionStatusListener">The listener that will be called when the connection status is changed</param>
        /// <exception cref="DxException">The connection creation was failed.</exception>
        public NativeConnection(string address, string authScheme, string authData,
            Action<IDxConnection> disconnectListener,
            Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener)
        {
            this.terminationNotifier = OnDisconnect;
            this.connectionStatusChangeHandler = ConnectionStatusChangeHandler;
            this.disconnectListener = disconnectListener;
            this.connectionStatusListener = connectionStatusListener;
            C.CheckOk(C.Instance.dxf_create_connection_auth_custom(address, authScheme, authData, terminationNotifier,
                connectionStatusChangeHandler,
                null, null, IntPtr.Zero, out handle));
        }

        /// <summary>
        /// Sets a handler that will be called when a server heartbeat arrives and contains non empty payload
        ///
        /// <example>
        /// <code>
        /// connection.SetOnServerHeartbeatHandler((connection, time, lagMark, rtt) => {
        ///    Console.Error.WriteLine($"##### Server time (UTC) = {time}, Server lag = {lagMark} us, RTT = {rtt} us #####");
        /// });
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="handler">The handler that will be called</param>
        public void SetOnServerHeartbeatHandler(OnServerHeartbeatHandler handler)
        {
            if (onServerHeartbeatNotifier == null)
            {
                onServerHeartbeatNotifier = OnServerHeartbeat;
                C.CheckOk(C.Instance.SetOnServerHeartbeatNotifier(handle, onServerHeartbeatNotifier, IntPtr.Zero));
            }

            onServerHeartbeatHandler = handler;
        }

        private void OnDisconnect(IntPtr connection, IntPtr userData)
        {
            disconnectListener?.Invoke(this);
        }

        private void ConnectionStatusChangeHandler(IntPtr connection, ConnectionStatus oldStatus,
            ConnectionStatus newStatus, IntPtr userData)
        {
            connectionStatusListener?.Invoke(this, oldStatus, newStatus);
        }

        private void OnServerHeartbeat(IntPtr connection, long serverMillis, int serverLagMark, int connectionRtt,
            IntPtr userData)
        {
            onServerHeartbeatHandler?.Invoke(this, Tools.UnixTimeToDate(serverMillis), serverLagMark, connectionRtt);
        }

        private int OnNativeCreate(IntPtr connection, IntPtr userData)
        {
            OnCreation?.Invoke(this, new EventArgs());
            return 0;
        }

        internal void RemoveSubscription(IDxSubscription subscription)
        {
            subscriptions.Remove(subscription);
        }

        #region Implementation of IDxConnection

        /// <summary>
        ///     Disconnect from the server
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <exception cref="DxException"></exception>
        public void Disconnect()
        {
            if (handle == IntPtr.Zero)
                return;

            C.CheckOk(C.Instance.dxf_close_connection(handle));
            handle = IntPtr.Zero;
        }

        /// <summary>
        ///     Creates an event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="type">Event type.</param>
        /// <param name="eventSubscriptionFlags">Event subscription flags</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, EventSubscriptionFlag eventSubscriptionFlags,
            IDxEventListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");
            IDxSubscription result = new NativeSubscription(this, type, eventSubscriptionFlags, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates an event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="type">Event type.</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, IDxEventListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");
            IDxSubscription result = new NativeSubscription(this, type, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a candle event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="time">Date time in the past.</param>
        /// <param name="eventSubscriptionFlags">Event subscription flags</param>
        /// <param name="listener">Candle listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(DateTime? time, EventSubscriptionFlag eventSubscriptionFlags,
            IDxCandleListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSubscription(this, time, eventSubscriptionFlags, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a candle event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="time">Date time in the past.</param>
        /// <param name="listener">Candle listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(DateTime? time, IDxCandleListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSubscription(this, time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a time event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="type">Event type.</param>
        /// <param name="time">Unix time stamp (the number of milliseconds from 1.1.1970)</param>
        /// <param name="eventSubscriptionFlags">Event subscription flags</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, long time,
            EventSubscriptionFlag eventSubscriptionFlags, IDxEventListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSubscription(this, type, time, eventSubscriptionFlags, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a time event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="type">Event type.</param>
        /// <param name="time">Unix time stamp (the number of milliseconds from 1.1.1970)</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, long time, IDxEventListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSubscription(this, type, time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a time event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="type">Event type.</param>
        /// <param name="time">Date time in the past.</param>
        /// <param name="eventSubscriptionFlags">Event subscription flags</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, DateTime? time,
            EventSubscriptionFlag eventSubscriptionFlags, IDxEventListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = (time == null)
                ? new NativeSubscription(this, type, 0L, eventSubscriptionFlags, listener)
                : new NativeSubscription(this, type, (DateTime)time, eventSubscriptionFlags, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a time event subscription.
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="type">Event type.</param>
        /// <param name="time">Date time in the past.</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, DateTime? time, IDxEventListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = (time == null)
                ? new NativeSubscription(this, type, 0L, listener)
                : new NativeSubscription(this, type, (DateTime)time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a snapshot subscription
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSnapshotSubscription(long time, IDxSnapshotListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSnapshotSubscription(this, time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a snapshot subscription
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="time">Date time in the past</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSnapshotSubscription(DateTime? time, IDxSnapshotListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            var unixTime = time == null ? 0 : Tools.DateToUnixTime((DateTime)time);
            IDxSubscription result = new NativeSnapshotSubscription(this, unixTime, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a snapshot subscription
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="eventType">Single event type.</param>
        /// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSnapshotSubscription(EventType eventType, long time,
            IDxSnapshotListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSnapshotSubscription(this, eventType, time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        /// Creates the new native order subscription on snapshot with incremental updates.
        /// </summary>
        /// <remarks>
        /// Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="listener">The incremental snapshot listener callback</param>
        /// <returns>The subscription object</returns>
        /// <exception cref="NativeDxException">There isn't an active connection</exception>
        public IDxSubscription CreateIncOrderSnapshotSubscription(IDxIncOrderSnapshotListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSnapshotSubscription(this, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a snapshot subscription
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="eventType">Single event type.</param>
        /// <param name="time">Date time in the past</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSnapshotSubscription(EventType eventType, DateTime? time,
            IDxSnapshotListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            var unixTime = time == null ? 0 : Tools.DateToUnixTime((DateTime)time);
            IDxSubscription result = new NativeSnapshotSubscription(this, eventType, unixTime, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates an Order View subscription
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="listener"></param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateOrderViewSubscription(IDxOrderViewListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new OrderViewSubscription(this, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///     Creates a regional book (10 levels)
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbol">The book symbol</param>
        /// <param name="bookListener">Regional book changes listener. Null is allowed.</param>
        /// <param name="quoteListener">Quotes listener. Null is allowed.</param>
        /// <returns>regional book object</returns>
        public IDxRegionalBook CreateRegionalBook(string symbol, IDxRegionalBookListener bookListener,
            IDxQuoteListener quoteListener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            return new NativeRegionalBook(this, symbol, bookListener, quoteListener);
        }

        private Task<List<IDxIndexedEvent>> GetDataForPeriodImpl(EventType eventType, string symbol,
            DateTime fromTime,
            DateTime toTime, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            if (eventType != EventType.TimeAndSale && eventType != EventType.Candle && eventType != EventType.Series &&
                eventType != EventType.Greeks)
            {
                throw new NativeDxException(
                    "The event type must be equal to one of these values: TimeAndSale, Candle, Series, Greeks. ");
            }

            var provider =
                new SnapshotDataProvider(this, eventType, OrderSource.EMPTY, symbol, fromTime, toTime, timeout);

            return provider.Run(cancellationToken);
        }

        /// <inheritdoc />
        public Task<List<IDxIndexedEvent>> GetDataForPeriod(EventType eventType, string symbol, DateTime fromTime,
            DateTime toTime)
        {
            return GetDataForPeriodImpl(eventType, symbol, fromTime, toTime, Timeout.InfiniteTimeSpan,
                CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<List<IDxIndexedEvent>> GetDataForPeriod(EventType eventType, string symbol, DateTime fromTime,
            DateTime toTime, TimeSpan timeout)
        {
            return GetDataForPeriodImpl(eventType, symbol, fromTime, toTime, timeout, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<List<IDxIndexedEvent>> GetDataForPeriod(EventType eventType, string symbol, DateTime fromTime,
            DateTime toTime, long timeout)
        {
            return GetDataForPeriodImpl(eventType, symbol, fromTime, toTime, TimeSpan.FromMilliseconds(timeout),
                CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<List<IDxIndexedEvent>> GetDataForPeriod(EventType eventType, string symbol, DateTime fromTime,
            DateTime toTime,
            CancellationToken cancellationToken)
        {
            return GetDataForPeriodImpl(eventType, symbol, fromTime, toTime, Timeout.InfiniteTimeSpan,
                cancellationToken);
        }

        private Task<List<IDxIndexedEvent>> GetOrderDataForPeriodImpl(EventType eventType,
            OrderSource orderSource,
            string symbol,
            DateTime fromTime,
            DateTime toTime, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            if (eventType != EventType.Order && eventType != EventType.SpreadOrder)
            {
                throw new NativeDxException("The event type should be either Order or SpreadOrder");
            }

            var provider = new SnapshotDataProvider(this, eventType, orderSource, symbol, fromTime, toTime, timeout);

            return provider.Run(cancellationToken);
        }

        /// <inheritdoc />
        public Task<List<IDxIndexedEvent>> GetOrderDataForPeriod(EventType eventType, OrderSource orderSource,
            string symbol,
            DateTime fromTime,
            DateTime toTime)
        {
            return GetOrderDataForPeriodImpl(eventType, orderSource, symbol, fromTime, toTime,
                Timeout.InfiniteTimeSpan, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<List<IDxIndexedEvent>> GetOrderDataForPeriod(EventType eventType, OrderSource orderSource,
            string symbol, DateTime fromTime,
            DateTime toTime, TimeSpan timeout)
        {
            return GetOrderDataForPeriodImpl(eventType, orderSource, symbol, fromTime, toTime, timeout,
                CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<List<IDxIndexedEvent>> GetOrderDataForPeriod(EventType eventType, OrderSource orderSource,
            string symbol, DateTime fromTime,
            DateTime toTime, long timeout)
        {
            return GetOrderDataForPeriodImpl(eventType, orderSource, symbol, fromTime, toTime,
                TimeSpan.FromMilliseconds(timeout), CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<List<IDxIndexedEvent>> GetOrderDataForPeriod(EventType eventType, OrderSource orderSource,
            string symbol, DateTime fromTime,
            DateTime toTime, CancellationToken cancellationToken)
        {
            return GetOrderDataForPeriodImpl(eventType, orderSource, symbol, fromTime, toTime,
                Timeout.InfiniteTimeSpan, cancellationToken);
        }

        /// <summary>
        /// Creates the new price level book (10 levels) instance for the specified symbol and sources
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbol">Single symbol name</param>
        /// <param name="sources">The order sources</param>
        /// <param name="listener">Price level book changes listener. Null is allowed.</param>
        /// <returns>The price level book object</returns>
        public IDxPriceLevelBook CreatePriceLevelBook(string symbol, string[] sources,
            IDxPriceLevelBookListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            return new NativePriceLevelBook(this, symbol, sources, listener);
        }

        /// <summary>
        /// Creates the new price level book (10 levels) instance for the specified symbol and sources
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbol">Single symbol name</param>
        /// <param name="sources">The order sources</param>
        /// <param name="listener">Price level book changes listener. Null is allowed.</param>
        /// <returns>The price level book object</returns>
        public IDxPriceLevelBook CreatePriceLevelBook(string symbol, OrderSource[] sources,
            IDxPriceLevelBookListener listener)
        {
            if (handle == IntPtr.Zero)
                throw new NativeDxException("not connected");

            return new NativePriceLevelBook(this, symbol, sources, listener);
        }

        /// <summary>
        /// Creates the new price level book (10 levels) instance for the specified symbol and all sources
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="symbol">Single symbol name</param>
        /// <param name="listener">Price level book changes listener. Null is allowed.</param>
        /// <returns>The price level book object</returns>
        public IDxPriceLevelBook CreatePriceLevelBook(string symbol, IDxPriceLevelBookListener listener)
        {
            return new NativePriceLevelBook(this, symbol, listener);
        }

        /// <summary>
        ///     Enables the dumping raw data of incoming traffic of connection into specific file
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="rawFileName">file name for raw data</param>
        /// <exception cref="ArgumentException">Invalid argument <c>rawFileName</c></exception>
        /// <exception cref="NativeDxException"></exception>
        public void WriteRawData(string rawFileName)
        {
            if (string.IsNullOrEmpty(rawFileName))
                throw new ArgumentException("Invalid file name");

            var ascii = Encoding.ASCII;
            var fileName = ascii.GetBytes(rawFileName);

            C.CheckOk(C.Instance.dxf_write_raw_data(handle, fileName));
        }

        /// <summary>
        ///     Returns the current connection properties
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        public IDictionary<string, string> Properties
        {
            get
            {
                IDictionary<string, string> result = new Dictionary<string, string>();
                IntPtr properties;
                int count;
                C.CheckOk(C.Instance.dxf_get_connection_properties_snapshot(handle, out properties, out count));
                if (properties == IntPtr.Zero) return result;
                try
                {
                    for (var i = 0; i < count; ++i)
                    {
                        var elem = properties + i * 2 * IntPtr.Size;
                        unsafe
                        {
                            var key = new IntPtr(*(char**)elem.ToPointer());
                            var value = new IntPtr(*(char**)(elem + IntPtr.Size).ToPointer());
                            var keyString = Marshal.PtrToStringUni(key);

                            if (keyString != null)
                            {
                                result.Add(keyString, Marshal.PtrToStringUni(value));
                            }
                        }
                    }
                }
                finally
                {
                    C.Instance.dxf_free_connection_properties_snapshot(properties, count);
                }

                return result;
            }
        }

        /// <summary>
        ///     Returns the current connection address
        /// </summary>
        /// <remarks>
        ///     Don't use this property inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        public string ConnectedAddress
        {
            get
            {
                IntPtr address;
                C.CheckOk(C.Instance.dxf_get_current_connected_address(handle, out address));
                if (address == IntPtr.Zero)
                {
                    return null;
                }

                try
                {
                    return Marshal.PtrToStringAnsi(address);
                }
                finally
                {
                    C.Instance.dxf_free(address);
                }
            }
        }

        /// <summary>
        ///     Returns the current connection status
        /// </summary>
        /// <remarks>
        ///     Don't use this property inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        public ConnectionStatus Status
        {
            get
            {
                ConnectionStatus status;
                C.CheckOk(C.Instance.dxf_get_current_connection_status(handle, out status));

                return status;
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Disposes the connection object
        ///
        /// Clears the subscriptions and disconnects
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        public void Dispose()
        {
            subscriptions.Clear();
            if (handle != IntPtr.Zero)
                Disconnect();
        }

        #endregion
    }
}