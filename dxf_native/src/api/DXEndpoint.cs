#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;
using com.dxfeed.native;
using System;
using System.Collections.Generic;

namespace com.dxfeed.api
{
    /// <summary>
    ///     Manages network connections to dxFeed.
    /// </summary>
    public class DXEndpoint : IDXEndpoint
    {
        /// <summary>
        ///     Returns a default application-wide singleton instance of DXEndpoint with a default
        ///     role.
        ///     Most applications use only a single data-source and should rely on this method to
        ///     get one.
        /// </summary>
        /// <returns>Instance of DXEndpoint with a default role.</returns>
        public static IDXEndpoint GetInstance()
        {
            if (endpointInstance == null)
            {
                lock (instanceLocker)
                {
                    endpointInstance = new DXEndpoint();
                    endpointInstance.Connect(DefaultAddress);
                }
            }
            return endpointInstance;
        }

        /// <summary>
        ///     Creates an new endpoint.
        /// </summary>
        /// <returns>The created endpoint.</returns>
        public static IDXEndpoint Create()
        {
            lock (instanceLocker)
            {
                if (endpointInstance != null && endpointInstance.State != DXEndpointState.Closed)
                {
                    endpointInstance.Close();
                }
                endpointInstance = new DXEndpoint();
                endpointInstance.Connect(DefaultAddress);
            }
            return endpointInstance;
        }

        /// <summary>
        ///     Event fires when <see cref="Close()"/> method was called.
        /// </summary>
        public event OnEndpointClosingEventHandler OnClosing;

        /// <summary>
        ///     Thread-safe state getter of this endpoint.
        /// </summary>
        public DXEndpointState State
        {
            get
            {
                DXEndpointState result;
                lock (stateLocker)
                {
                    result = assyncState;
                }
                return result;
            }

            private set
            {
                lock (stateLocker)
                {
                    assyncState = value;
                }
            }
        }

        /// <summary>
        ///     Changes user name for this endpoint.
        ///     This method shall be called before <see cref="Connect(string)"/> together
        ///     with <see cref="Password(string)"/> to configure service access credentials.
        /// </summary>
        /// <param name="user">User name.</param>
        /// <returns>This <see cref="DXEndpoint"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="user"/> is null.</exception>
        public IDXEndpoint User(string user)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("The user name is null!");
            this.user = user;
            return this;
        }

        /// <summary>
        ///     Changes password for this endpoint.
        ///     This method shall be called before <see cref="Connect(string)"/> together
        ///     with <see cref="User(string)"/> to configure service access credentials.
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>This <see cref="DXEndpoint"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="password"/> is null.</exception>
        public IDXEndpoint Password(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("The password is null!");
            this.password = password;
            return this;
        }

        /// <summary>
        ///     <para>
        ///         Connects to the specified remove address. Previously established connections
        ///         are closed if the new address is different from the old one.
        ///         This method does nothing if address does not change or if this endpoint is
        ///         <see cref="DXEndpointState.Closed"/>.
        ///         The endpoint <see cref="State"/> immediately becomes
        ///         <see cref="DXEndpointState.Connecting"/> otherwise.
        ///     </para>
        ///     <para>
        ///         The address string is provided with the market data vendor agreement.
        ///         Use "demo.dxfeed.com:7300" for a demo quote feed.
        ///     </para>
        ///     <list type="table">
        ///         <listheader>
        ///             <description>
        ///                 The simplest address strings have the following format:
        ///             </description>
        ///         </listheader>
        ///         <item>
        ///             <description>
        ///                 <c>host:port</c> to establish a TCP/IP connection
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <c>:port</c> to listen for a TCP/IP connection with a plain socket
        ///                 connector (good for up to a few hundred of connections).
        ///             </description>
        ///         </item>
        ///     </list>
        ///     <para>
        ///         For premium services access credentials must be configured before invocation of
        ///         <c>Connect</c> method using <see cref="User(string)"/> and
        ///         <see cref="Password(string)"/> methods.
        ///     </para>
        /// </summary>
        /// <param name="address">The data source address.</param>
        /// <returns>This <see cref="DXEndpoint"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="address"/> is null.</exception>
        public IDXEndpoint Connect(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("The address is null!");

            if (this.address.Equals(address) && State != DXEndpointState.NotConnected || State == DXEndpointState.Closed)
                return this;

            if (State == DXEndpointState.Connected || State == DXEndpointState.Connecting)
                Disconnect();

            State = DXEndpointState.Connecting;

            lock (stateLocker)
            {
                this.address = address;
                connection = new NativeConnection(this.address, null);
                connection.OnCreation += ConnectionInstance_OnCreation;
            }
            return this;
        }

        /// <summary>
        ///     <para>
        ///         Terminates all remote network connections.
        ///         This method does nothing if this endpoint is <see cref="DXEndpointState.Closed"/>.
        ///         The endpoint <see cref="State"/> immediately becomes
        ///         <see cref="DXEndpointState.NotConnected"/> otherwise.
        ///     </para>
        ///     <para>
        ///         This method does not release all resources that are associated with this
        ///         endpoint.
        ///         Use <see cref="Close()"/> method to release all resources.
        ///     </para>
        /// </summary>
        public void Disconnect()
        {
            if (State == DXEndpointState.Closed)
                return;

            lock (stateLocker)
            {
                assyncState = DXEndpointState.NotConnected;
                UnsafeCloseConnection();
            }
        }

        /// <summary>
        ///     <para>
        ///         Closes this endpoint. All network connection are terminated as with
        ///         <see cref="Disconnect()"/> method and no further connections
        ///         can be established.
        ///     </para>
        ///     <para>
        ///         The endpoint <see cref="State"/> immediately becomes <see cref="DXEndpointState.Closed"/>.
        ///         All resources associated with this endpoint are released.
        ///     </para>
        /// </summary>
        public void Close()
        {
            if (State == DXEndpointState.Closed)
                return;

            lock (stateLocker)
            {
                OnClosing?.Invoke(this, EventArgs.Empty);
                assyncState = DXEndpointState.Closed;
                UnsafeCloseConnection();
            }
        }

        /// <summary>
        ///     Returns a set of all event types supported by this endpoint. The resulting set
        ///     cannot be modified.
        /// </summary>
        /// <returns>Set of all event types.</returns>
        public ISet<Type> GetEventTypes()
        {
            return new HashSet<Type>(new Type[] {
                typeof(IDxCandle),
                typeof(IDxConfiguration),
                typeof(IDxGreeks),
                typeof(IDxOrder),
                typeof(IDxProfile),
                typeof(IDxQuote),
                typeof(IDxSeries),
                typeof(IDxSpreadOrder),
                typeof(IDxSummary),
                typeof(IDxTheoPrice),
                typeof(IDxTimeAndSale),
                typeof(IDxTrade),
                typeof(IDxTrade),
                typeof(IDxUnderlying)
            });
        }

        /// <summary>
        ///     Returns feed that is associated with this endpoint.
        /// </summary>
        /// <returns>The feed.</returns>
        public IDXFeed Feed { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Close();
        }

        internal IDxConnection Connection
        {
            get
            {
                return connection;
            }
        }

        /// <summary>
        ///     Protected constructor for implementations of DXEndpoint.
        /// </summary>
        protected DXEndpoint()
        {
            assyncState = DXEndpointState.NotConnected;
            Feed = new DXFeed(this);
        }

        private static readonly string DefaultAddress = "demo.dxfeed.com:7300";
        private static readonly string DefaultUser = "demo";
        private static readonly string DefaultPassword = "demo";
        private static DXEndpoint endpointInstance = null;
        private static object instanceLocker = new object();
        private object stateLocker = new object();
        private DXEndpointState assyncState = DXEndpointState.NotConnected;
        private string address = DefaultAddress;
        private string user = DefaultUser;
        private string password = DefaultPassword;
        private NativeConnection connection = null;

        private void ConnectionInstance_OnCreation(object sender, EventArgs e)
        {
            State = DXEndpointState.Connected;
        }

        private void UnsafeCloseConnection()
        {
            if (connection != null)
            {
                connection.OnCreation -= ConnectionInstance_OnCreation;
                connection.Disconnect();
                connection = null;
            }
        }

    }
}
