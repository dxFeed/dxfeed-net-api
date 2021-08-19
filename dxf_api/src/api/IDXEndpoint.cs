#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;

namespace com.dxfeed.api
{

    /// <summary>
    ///     Represents the current state of endpoint.
    /// </summary>
    [Obsolete("DXEndpointState enum is deprecated and will removed in 9.0.0 version. Please use NativeConnection\\NativeSubscription")]
    public enum DXEndpointState
    {
        /// <summary>
        ///     Endpoint was created by is not connected to remote endpoints.
        /// </summary>
        NotConnected,

        /// <summary>
        ///     The <see cref="Connect(string)"/>  method was called to establish connection
        ///     to remove endpoint, but connection is not actually established yet or was lost.
        /// </summary>
        Connecting,

        /// <summary>
        ///     The connection to remote endpoint is established.
        /// </summary>
        Connected,

        /// <summary>
        ///     Endpoint was <see cref="Close()"/>.
        /// </summary>
        Closed
    }

    /// <summary>
    ///     Handler for <see cref="IDXEndpoint"/> closing event.
    /// </summary>
    /// <param name="sender"><see cref="IDXEndpoint"/> instance.</param>
    /// <param name="e">Event arguments.</param>
    [Obsolete("OnEndpointClosingEventHandler delegate is deprecated and will removed in 9.0.0 version. Please use NativeConnection\\NativeSubscription")]
    public delegate void OnEndpointClosingEventHandler(object sender, EventArgs e);

    /// <summary>
    ///     Manages network connections to dxFeed.
    /// </summary>
    [Obsolete("IDXEndpoint interface is deprecated and will removed in 9.0.0 version. Please use NativeConnection\\NativeSubscription")]
    public interface IDXEndpoint : IDisposable
    {
        /// <summary>
        ///     Event fires when <see cref="Close()"/> method was called.
        /// </summary>
        event OnEndpointClosingEventHandler OnClosing;

        /// <summary>
        ///     Thread-safe state getter of this endpoint.
        /// </summary>
        DXEndpointState State { get; }

        /// <summary>
        ///     Changes user name for this endpoint.
        ///     This method shall be called before <see cref="Connect(string)"/> together
        ///     with <see cref="Password(string)"/> to configure service access credentials.
        /// </summary>
        /// <param name="user">User name.</param>
        /// <returns>This <see cref="DXEndpoint"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="user"/> is null.</exception>
        IDXEndpoint User(string user);

        /// <summary>
        ///     Changes password for this endpoint.
        ///     This method shall be called before <see cref="Connect(string)"/> together
        ///     with <see cref="User(string)"/> to configure service access credentials.
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>This <see cref="DXEndpoint"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="password"/> is null.</exception>
        IDXEndpoint Password(string password);

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
        IDXEndpoint Connect(string address);

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
        void Disconnect();

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
        void Close();

        /// <summary>
        ///     Returns a set of all event types supported by this endpoint. The resulting set
        ///     cannot be modified.
        /// </summary>
        /// <returns>Set of all event types.</returns>
        ISet<Type> GetEventTypes();

        /// <summary>
        ///     Returns feed that is associated with this endpoint.
        /// </summary>
        /// <returns>The feed.</returns>
        IDXFeed Feed { get; }
    }
}
