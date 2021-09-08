﻿#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class TimeAndSaleDataConnection : DataConnection, IDxTimeAndSaleDataConnection
    {
        /// <summary>
        /// Creates the new candle data connection
        /// </summary>
        /// <param name="address">Candle web service address</param>
        /// <param name="login">The user login</param>
        /// <param name="password">The user password</param>
        public TimeAndSaleDataConnection(string address, string login, string password) : base(address, login, password)
        {
        }

        /// <summary>
        /// Creates the new candle data connection
        /// </summary>
        /// <param name="address">Candle web service address</param>
        /// <param name="token">The connection token (optional)</param>
        public TimeAndSaleDataConnection(string address, string token = null) : base(address, token)
        {
        }

        private static string CreateQuery(IEnumerable<string> symbols, DateTime fromTime, DateTime toTime)
        {
            return "records=TimeAndSale&" +
                   $"symbols={string.Join(",", symbols).Replace("&", "[%26]")}&" +
                   $"start={fromTime.ToUniversalTime():yyyyMMdd-HHmmss}&" +
                   $"stop={toTime.ToUniversalTime():yyyyMMdd-HHmmss}&" +
                   "format=binary&" +
                   "compression=zip&" +
                   "skipServerTimeCheck";
        }

        /// <inheritdoc />
        public Task<Dictionary<string, List<IDxTimeAndSale>>> GetTimeAndSaleData(List<string> symbols,
            DateTime fromTime, DateTime toTime,
            CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var result = new Dictionary<string, List<IDxTimeAndSale>>();
                var connectionAddress = address;
                var uri = new Uri(address);
                
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                {
                    connectionAddress = $"{address}?{CreateQuery(symbols, fromTime, toTime)}";
                }

                try
                {
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

                            using (var dataProvider = new SimpleTimeAndSaleDataProvider())
                            {
                                result = await dataProvider.Run(fileToWriteTo, symbols, cancellationToken);
                            }

                            try
                            {
                                File.Delete(fileToWriteTo);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    }
                }
                catch (WebException e)
                {
                    var response = e.Response as HttpWebResponse;

                    if (response == null) throw;
                    
                    if (response.StatusCode != HttpStatusCode.BadRequest) throw;
                        
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream == null) throw;
                                
                        using (var reader = new StreamReader(stream, Encoding.ASCII))
                        {
                            var line = await reader.ReadLineAsync();
                            throw new WebException(line, e);
                        }
                    }
                }

                return result;
            }, cancellationToken);
        }
    }
}