using System;
using System.Net;
using com.dxfeed.io;

namespace com.dxfeed.native
{
    public class DataConnection
    {
        protected readonly string address;
        private readonly string login;
        private readonly string password;
        private readonly string token;

        private DataConnection(string address, string login, string password, string token)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            this.address = address;
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