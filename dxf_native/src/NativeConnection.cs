#region License

// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.

#endregion

using System;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.native.api;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using com.dxfeed.api.connection;

namespace com.dxfeed.native {
    /// <inheritdoc />
    /// <summary>
    ///   Class provides operations with event subscription
    /// </summary>
    public class NativeConnection : IDxConnection {
        private IntPtr handler = IntPtr.Zero;
        private readonly C.dxf_conn_termination_notifier_t termination_notifier;
        private readonly C.dxf_conn_status_notifier_t connectionStatusChangeHandler;
        private readonly C.dxf_socket_thread_creation_notifier_t creationNotifier;
        private readonly Action<IDxConnection> disconnectListener;
        private readonly Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener;
        private readonly ISet<IDxSubscription> subscriptions = new HashSet<IDxSubscription>();

        public delegate void OnCreationEventHandler(object sender, EventArgs e);

        public event OnCreationEventHandler OnCreation;

        internal IntPtr Handler => handler;

        /// <summary>
        ///   Creates new connection
        /// </summary>
        /// <param name="address">server address to connect</param>
        /// <param name="disconnectListener">listener will be called when the connection is interrupted</param>
        /// <exception cref="DxException"></exception>
        public NativeConnection(string address, Action<IDxConnection> disconnectListener) {
            this.termination_notifier = OnDisconnect;
            this.connectionStatusChangeHandler = ConnectionStatusChangeHandler;
            this.creationNotifier = OnNativeCreate;
            this.disconnectListener = disconnectListener;
            C.CheckOk(C.Instance.dxf_create_connection(address, termination_notifier, connectionStatusChangeHandler,
                null, null, IntPtr.Zero, out handler));
        }
        
        /// <summary>
        ///   Creates new connection
        /// </summary>
        /// <param name="address">server address to connect</param>
        /// <param name="disconnectListener">listener will be called when the connection is interrupted</param>
        /// <param name="connectionStatusListener">listener will be called when the connection status is changed</param>
        /// <exception cref="DxException"></exception>
        public NativeConnection(string address, Action<IDxConnection> disconnectListener,
            Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener) {
            this.termination_notifier = OnDisconnect;
            this.connectionStatusChangeHandler = ConnectionStatusChangeHandler;
            this.creationNotifier = OnNativeCreate;
            this.disconnectListener = disconnectListener;
            this.connectionStatusListener = connectionStatusListener;
            C.CheckOk(C.Instance.dxf_create_connection(address, termination_notifier, connectionStatusChangeHandler,
                null, null, IntPtr.Zero, out handler));
        }

        /// <summary>
        /// Creates new connection.
        /// </summary>
        /// <param name="address">Server address to connect.</param>
        /// <param name="credential">User name and password to server access.</param>
        /// <param name="disconnectListener">Listener will be called when the connection is interrupted.</param>
        /// <param name="connectionStatusListener">listener will be called when the connection status is changed</param>
        /// <exception cref="DxException">Create connection failed.</exception>
        public NativeConnection(string address, System.Net.NetworkCredential credential,
            Action<IDxConnection> disconnectListener,
            Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener) {
            this.termination_notifier = OnDisconnect;
            this.disconnectListener = disconnectListener;
            this.connectionStatusListener = connectionStatusListener;
            C.CheckOk(C.Instance.dxf_create_connection_auth_basic(address, credential.UserName, credential.Password,
                termination_notifier, connectionStatusChangeHandler, null, null, IntPtr.Zero, out handler));
        }

        /// <summary>
        /// Creates new connection.
        /// </summary>
        /// <param name="address">Server address to connect.</param>
        /// <param name="token">Bearer scheme token to server access.</param>
        /// <param name="disconnectListener">Listener will be called when the connection is interrupted.</param>
        /// <exception cref="DxException">Create connection failed.</exception>
        public NativeConnection(string address, string token, Action<IDxConnection> disconnectListener) {
            this.termination_notifier = OnDisconnect;
            this.disconnectListener = disconnectListener;
            C.CheckOk(C.Instance.dxf_create_connection_auth_bearer(address, token, termination_notifier,
                connectionStatusChangeHandler, null, null,
                IntPtr.Zero, out handler));
        }
        
        /// <summary>
        /// Creates new connection.
        /// </summary>
        /// <param name="address">Server address to connect.</param>
        /// <param name="token">Bearer scheme token to server access.</param>
        /// <param name="disconnectListener">Listener will be called when the connection is interrupted.</param>
        /// <param name="connectionStatusListener">listener will be called when the connection status is changed</param>
        /// <exception cref="DxException">Create connection failed.</exception>
        public NativeConnection(string address, string token, Action<IDxConnection> disconnectListener,
            Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener) {
            this.termination_notifier = OnDisconnect;
            this.disconnectListener = disconnectListener;
            this.connectionStatusListener = connectionStatusListener;
            C.CheckOk(C.Instance.dxf_create_connection_auth_bearer(address, token, termination_notifier,
                connectionStatusChangeHandler, null, null,
                IntPtr.Zero, out handler));
        }

        /// <summary>
        /// Creates new connection.
        /// </summary>
        /// <param name="address">Server address to connect.</param>
        /// <param name="authscheme">The authorization scheme.</param>
        /// <param name="authdata">The authorization data.</param>
        /// <param name="disconnectListener">Listener will be called when the connection is interrupted.</param>
        /// <param name="connectionStatusListener">listener will be called when the connection status is changed</param>
        /// <exception cref="DxException">Create connection failed.</exception>
        public NativeConnection(string address, string authscheme, string authdata,
            Action<IDxConnection> disconnectListener,
            Action<IDxConnection, ConnectionStatus, ConnectionStatus> connectionStatusListener) {
            this.termination_notifier = OnDisconnect;
            this.disconnectListener = disconnectListener;
            this.connectionStatusListener = connectionStatusListener;
            C.CheckOk(C.Instance.dxf_create_connection_auth_custom(address, authscheme, authdata, termination_notifier,
                connectionStatusChangeHandler,
                null, null, IntPtr.Zero, out handler));
        }

        private void OnDisconnect(IntPtr connection, IntPtr userData) {
            disconnectListener?.Invoke(this);
        }

        private void ConnectionStatusChangeHandler(IntPtr connection, ConnectionStatus oldStatus,
            ConnectionStatus newStatus, IntPtr userData) {
            connectionStatusListener?.Invoke(this, oldStatus, newStatus);
        }

        private int OnNativeCreate(IntPtr connection, IntPtr userData) {
            OnCreation?.Invoke(this, new EventArgs());
            return 0;
        }

        internal void RemoveSubscription(IDxSubscription subscription) {
            subscriptions.Remove(subscription);
        }

        #region Implementation of IDxConnection

        /// <summary>
        ///   Disconnect from the server
        /// </summary>
        /// <exception cref="DxException"></exception>
        public void Disconnect() {
            if (handler == IntPtr.Zero)
                return;

            C.CheckOk(C.Instance.dxf_close_connection(handler));
            handler = IntPtr.Zero;
        }

        /// <summary>
        ///   Create event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, IDxEventListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");
            IDxSubscription result = new NativeSubscription(this, type, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///   Create candle event subscription.
        /// </summary>
        /// <param name="time">Date time in the past.</param>
        /// <param name="listener">Candle listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(DateTime? time, IDxCandleListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSubscription(this, time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///   Create time event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="time">Unix time stamp (the number of milliseconds from 1.1.1970)</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, long time, IDxEventListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSubscription(this, type, time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        /// Create time event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="time">Date time in the past.</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSubscription(EventType type, DateTime? time, IDxEventListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = (time == null)
                ? new NativeSubscription(this, type, 0L, listener)
                : new NativeSubscription(this, type, (DateTime) time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSnapshotSubscription(Int64 time, IDxSnapshotListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSnapshotSubscription(this, time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="time">Date time in the past</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSnapshotSubscription(DateTime? time, IDxSnapshotListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            long unixTime = time == null ? 0 : Tools.DateToUnixTime((DateTime) time);
            IDxSubscription result = new NativeSnapshotSubscription(this, unixTime, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="eventType">Single event type.</param>
        /// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSnapshotSubscription(EventType eventType, Int64 time,
            IDxSnapshotListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new NativeSnapshotSubscription(this, eventType, time, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="eventType">Single event type.</param>
        /// <param name="time">Date time in the past</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateSnapshotSubscription(EventType eventType, DateTime? time,
            IDxSnapshotListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            long unixTime = time == null ? 0 : Tools.DateToUnixTime((DateTime) time);
            IDxSubscription result = new NativeSnapshotSubscription(this, eventType, unixTime, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///   Creates Order View subscription
        /// </summary>
        /// <param name="listener"></param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        public IDxSubscription CreateOrderViewSubscription(IDxOrderViewListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            IDxSubscription result = new OrderViewSubscription(this, listener);
            subscriptions.Add(result);
            return result;
        }

        /// <summary>
        ///   Creates regional book
        /// </summary>
        /// <param name="symbol">Single symbol name</param>
        /// <param name="book_listener">Regional book changes listener. Null is allowed.</param>
        /// <param name="quote_listener">Quotes listener. Null is allowed.</param>
        /// <returns>regional book object</returns>
        public IDxRegionalBook CreateRegionalBook(string symbol, IDxRegionalBookListener book_listener,
            IDxQuoteListener quote_listener) {
            return new NativeRegionalBook(this, symbol, book_listener, quote_listener);
        }

        /// <summary>
        ///   Add dumping raw data of incoming traffic of connection into specific file
        /// </summary>
        /// <param name="rawFileName">file name for raw data</param>
        /// <exception cref="ArgumentException">Invalid argument <c>rawFileName</c></exception>
        /// <exception cref="NativeDxException"></exception>
        public void WriteRawData(string rawFileName) {
            if (rawFileName == null || rawFileName == "")
                throw new ArgumentException("Invalid file name");
            Encoding ascii = Encoding.ASCII;
            byte[] fileName = ascii.GetBytes(rawFileName);
            C.CheckOk(C.Instance.dxf_write_raw_data(handler, fileName));
        }

        public IDictionary<string, string> Properties {
            get {
                IDictionary<string, string> result = new Dictionary<string, string>();
                IntPtr properties;
                int count;
                C.CheckOk(C.Instance.dxf_get_connection_properties_snapshot(handler, out properties, out count));
                if (properties != IntPtr.Zero) {
                    try {
                        for (int i = 0; i < count; ++i) {
                            IntPtr elem = properties + i * 2 * IntPtr.Size;
                            unsafe {
                                IntPtr key = new IntPtr(*(char**) elem.ToPointer());
                                IntPtr value = new IntPtr(*(char**) (elem + IntPtr.Size).ToPointer());
                                result.Add(Marshal.PtrToStringUni(key), Marshal.PtrToStringUni(value));
                            }
                        }
                    } finally {
                        C.Instance.dxf_free_connection_properties_snapshot(properties, count);
                    }
                }

                return result;
            }
        }

        public string ConnectedAddress {
            get {
                IntPtr address;
                C.CheckOk(C.Instance.dxf_get_current_connected_address(handler, out address));
                if (address == IntPtr.Zero) {
                    return null;
                }

                try {
                    return Marshal.PtrToStringAnsi(address);
                } finally {
                    C.Instance.dxf_free(address);
                }
            }
        }

        public ConnectionStatus Status {
            get {
                ConnectionStatus status;

                C.CheckOk(C.Instance.dxf_get_current_connection_status(handler, out status));

                return status;
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose() {
            subscriptions.Clear();
            if (handler != IntPtr.Zero)
                Disconnect();
        }

        #endregion
    }
}