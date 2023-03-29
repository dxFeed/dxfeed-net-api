#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.connection
{
    /// <summary>
    /// The native connection status
    /// </summary>
    public enum ConnectionStatus : int {
        /// <summary>
        /// No connection
        /// </summary>
        NotConnected = 0,
        /// <summary>
        /// Connected but not authorized
        /// </summary>
        Connected = 1,
        /// <summary>
        /// Connected but login required
        /// </summary>
        LoginRequired = 2,
        /// <summary>
        /// Connected and authorized
        /// </summary>
        Authorized = 3
    }
}