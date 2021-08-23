#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.io;

namespace com.dxfeed.native
{
    /// <summary>
    /// Class provides operations with candle data retrieving
    /// </summary>
    public class CandleDataConnection : IDxCandleDataConnection
    {
        private string address;
        private string login;
        private string password;
        private string token;

        private CandleDataConnection(string address, string login, string password, string token)
        {
            this.address = address ?? throw new ArgumentNullException(nameof(address));

            this.login = login;
            this.password = password;
            this.token = token;
        }

        /// <summary>
        /// Creates the new candle data connection
        /// </summary>
        /// <param name="address">Candle web service address</param>
        /// <param name="login">The user login</param>
        /// <param name="password">The user password</param>
        public CandleDataConnection(string address, string login, string password) : this(address, login, password,
            null)
        {
        }

        /// <summary>
        /// Creates the new candle data connection
        /// </summary>
        /// <param name="address">Candle web service address</param>
        /// <param name="token">The connection token (optional)</param>
        public CandleDataConnection(string address, string token = null) : this(address, null, null, token)
        {
        }
        
        private WebRequest OpenConnection(string connectionAddress)
        {
            if (!string.IsNullOrEmpty(token)) return URLInputStream.OpenConnection(connectionAddress, token);

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return URLInputStream.OpenConnection(connectionAddress);
            }

            return URLInputStream.OpenConnection(connectionAddress, login, password);
        }

        private static string CreateQuery(IEnumerable<CandleSymbol> symbols, DateTime fromTime, DateTime toTime)
        {
            var builder = new StringBuilder("records=Candle&symbols=");

            builder.Append(string.Join(",", symbols.Select(candleSymbol => candleSymbol.ToString()))
                .Replace("&", "[%26]"));
            builder.Append("&start=").Append(fromTime.ToUniversalTime().ToString("yyyyMMdd-HHmmss"));
            builder.Append("&stop=").Append(toTime.ToUniversalTime().ToString("yyyyMMdd-HHmmss"));
            builder.Append("&format=binary&compression=zip&skipServerTimeCheck");

            return builder.ToString();
        }

        /// <inheritdoc />
        public Task<Dictionary<CandleSymbol, List<IDxCandle>>> GetCandleData(List<CandleSymbol> symbols, DateTime fromTime, DateTime toTime,
            CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var result = new Dictionary<CandleSymbol, List<IDxCandle>>();
                var connectionAddress = address;

                var uri = new Uri(address);
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                {
                    var uriBuilder = new UriBuilder(uri)
                    {
                        Query = CreateQuery(symbols, fromTime, toTime)
                    };

                    connectionAddress = uriBuilder.ToString();
                }

                var request = OpenConnection(connectionAddress);
                var response = await request.GetResponseAsync();
                var isFileStream = request.GetType() == typeof(FileWebResponse);

                using (var inputStream = response.GetResponseStream())
                {
                    var compression = isFileStream
                        ? StreamCompression.DetectCompressionByExtension(new Uri(address))
                        : StreamCompression.DetectCompressionByMimeType(response.ContentType);
                    using (var decompressedIn = compression.Decompress(inputStream))
                    {
                        var fileToWriteTo = Path.GetTempFileName();
                        
                        using (var streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                        {
                            await decompressedIn.CopyToAsync(streamToWriteTo);
                        }

                        using (var dataProvider = new SimpleCandleDataProvider())
                        {
                            result = await dataProvider.Run(fileToWriteTo, symbols, cancellationToken);
                        }
                        
                        File.Delete(fileToWriteTo);
                    }
                }
                
                return result;
            }, cancellationToken);
        }
    }
}