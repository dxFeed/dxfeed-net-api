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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.io;

namespace com.dxfeed.native
{
    /// <summary>
    ///     Class provides operations with candle data retrieving
    /// </summary>
    public class CandleDataConnection : DataConnection, IDxCandleDataConnection
    {
        /// <summary>
        ///     Creates the new candle data connection
        /// </summary>
        /// <param name="address">Candle web service address</param>
        /// <param name="login">The user login</param>
        /// <param name="password">The user password</param>
        public CandleDataConnection(string address, string login, string password) : base(address, login, password)
        {
        }

        /// <summary>
        ///     Creates the new candle data connection
        /// </summary>
        /// <param name="address">Candle web service address</param>
        /// <param name="token">The connection token (optional)</param>
        public CandleDataConnection(string address, string token = null) : base(address, token)
        {
        }

        /// <inheritdoc />
        public Task<Dictionary<CandleSymbol, List<IDxCandle>>> GetCandleData(List<CandleSymbol> symbols,
            DateTime fromTime, DateTime toTime,
            CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var result = new Dictionary<CandleSymbol, List<IDxCandle>>();
                var connectionAddress = Address;
                var uri = new Uri(Address);

                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                    connectionAddress = $"{Address}?{CreateQuery(symbols, fromTime, toTime)}";

                try
                {
                    var request = OpenConnection(connectionAddress);
                    var response = await request.GetResponseAsync().ConfigureAwait(false);
                    var isFileStream = request.GetType() == typeof(FileWebResponse);

                    using (var inputStream = response.GetResponseStream())
                    {
                        var compression = isFileStream
                            ? StreamCompression.DetectCompressionByExtension(new Uri(Address))
                            : StreamCompression.DetectCompressionByMimeType(response.ContentType);
                        using (var decompressedIn = compression.Decompress(inputStream))
                        {
                            var fileToWriteTo = Path.GetTempFileName();

                            using (var streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                            {
                                await decompressedIn.CopyToAsync(streamToWriteTo).ConfigureAwait(false);
                            }

                            using (var dataProvider = new SimpleCandleDataProvider())
                            {
                                result = await dataProvider.Run(fileToWriteTo, symbols, cancellationToken)
                                    .ConfigureAwait(false);
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
                            var line = await reader.ReadLineAsync().ConfigureAwait(false);
                            throw new WebException(line, e);
                        }
                    }
                }

                return result;
            }, cancellationToken);
        }

        private static string CreateQuery(IEnumerable<CandleSymbol> symbols, DateTime fromTime, DateTime toTime)
        {
            return "records=Candle&" +
                   $"symbols={string.Join(",", symbols).Replace("&", "[%26]")}&" +
                   $"start={fromTime.ToUniversalTime():yyyyMMdd-HHmmss}Z&" +
                   $"stop={toTime.ToUniversalTime():yyyyMMdd-HHmmss}Z&" +
                   "format=binary&" +
                   "compression=zip&" +
                   "skipServerTimeCheck";
        }
    }
}