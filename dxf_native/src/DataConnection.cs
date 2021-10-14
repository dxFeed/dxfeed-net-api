#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Net;
using com.dxfeed.io;

namespace com.dxfeed.native
{
    /// <summary>
    /// Class that describes common connection methods which used to retrieve data from CandleWebService
    /// </summary>
    public class DataConnection
    {
        /// <summary>
        /// The connection address
        /// </summary>
        protected readonly string Address;
        private readonly string login;
        private readonly string password;
        private readonly string token;

        private DataConnection(string address, string login, string password, string token)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            this.Address = address;
            this.login = login;
            this.password = password;
            this.token = token;
        }
        
        /// <summary>
        /// Creates the new data connection
        /// </summary>
        /// <param name="address">Candle web service address</param>
        /// <param name="login">The user login</param>
        /// <param name="password">The user password</param>
        protected DataConnection(string address, string login, string password) : this(address, login, password,
            null)
        {
        }

        /// <summary>
        /// Creates the new data connection
        /// </summary>
        /// <param name="address">Candle web service address</param>
        /// <param name="token">The connection token (optional)</param>
        protected DataConnection(string address, string token = null) : this(address, null, null, token)
        {
        }
        
        /// <summary>
        /// Creates the web connection by address 
        /// </summary>
        /// <param name="connectionAddress">The connection address</param>
        /// <returns>The new web request</returns>
        protected WebRequest OpenConnection(string connectionAddress)
        {
            if (!string.IsNullOrEmpty(token)) return URLInputStream.OpenConnection(connectionAddress, token);

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return URLInputStream.OpenConnection(connectionAddress);
            }

            return URLInputStream.OpenConnection(connectionAddress, login, password);
        }
    }
}