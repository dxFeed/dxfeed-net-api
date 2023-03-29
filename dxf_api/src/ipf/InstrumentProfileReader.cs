#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using com.dxfeed.io;
using com.dxfeed.ipf.impl;

namespace com.dxfeed.ipf
{
    /// <summary>
    /// Reads instrument profiles from the stream using Simple File Format.
    /// Please see Instrument Profile Format documentation for complete description.
    /// This reader automatically uses data formats as specified in the stream.
    /// Use InstrumentProfileConnection if support for streaming updates of instrument profiles is needed.
    /// </summary>
    public class InstrumentProfileReader
    {
        private DateTime lastModified;

        /// <summary>
        /// Returns last modification time (in milliseconds) from last ReadFromFile operation
        /// or zero if it is unknown.
        /// </summary>
        /// <returns>Last modification time (in milliseconds)</returns>
        public DateTime GetLastModified()
        {
            return lastModified;
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified file.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analyzing file name.
        /// If file name ends with ".zip" then all compressed files will be read independently one by one
        /// in their order of appearing and total concatenated list of instrument profiles will be returned.
        /// If file name ends with ".gz" then compressed content will be read and returned.
        /// In other cases file will be considered uncompressed and will be read as is.
        ///
        /// Authentication information can be supplied to this method as part of URL user info
        /// like "http://user:password@host:port/path/file.ipf".
        ///
        /// This is a shortcut for ReadFromFile(address, null, null).
        ///
        /// This operation updates GetLastModified().
        /// </summary>
        /// <param name="address">URL of file to read from.</param>
        /// <returns>List of instrument profiles.</returns>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        /// <exception cref="InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        public IList<InstrumentProfile> ReadFromFile(string address)
        {
            return ReadFromFile(address, null, null);
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified address with a specified basic user and password credentials.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analyzing file name.
        /// If file name ends with ".zip" then all compressed files will be read independently one by one
        /// in their order of appearing and total concatenated list of instrument profiles will be returned.
        /// If file name ends with ".gz" then compressed content will be read and returned.
        /// In other cases file will be considered uncompressed and will be read as is.
        ///
        /// Specified user and password take precedence over authentication information that is supplied to this method
        /// as part of URL user info like "http://user:password@host:port/path/file.ipf".
        ///
        /// This operation updates GetLastModified().
        /// </summary>
        /// <param name="address">URL of file or service output to read from.</param>
        /// <param name="user">The user name (may be null).</param>
        /// <param name="password">The password (may be null).</param>
        /// <returns>List of instrument profiles.</returns>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        /// <exception cref="InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        public IList<InstrumentProfile> ReadFromFile(string address, string user, string password)
        {
            return ReadFromFileImpl(address, user, password, null);
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified address with a specified token.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analyzing file name.
        /// If file name ends with ".zip" then all compressed files will be read independently one by one
        /// in their order of appearing and total concatenated list of instrument profiles will be returned.
        /// If file name ends with ".gz" then compressed content will be read and returned.
        /// In other cases file will be considered uncompressed and will be read as is.
        ///
        /// This operation updates GetLastModified().
        /// </summary>
        /// <param name="address">URL of file or service output to read from.</param>
        /// <param name="token">The bearer token.</param>
        /// <returns>List of instrument profiles.</returns>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        /// <exception cref="InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        public IList<InstrumentProfile> ReadFromFile(string address, string token)
        {
            return ReadFromFileImpl(address, null, null, token);
        }

        private IList<InstrumentProfile> ReadFromFileImpl(string address, string user, string password, string token)
        {
            var url = ResolveSourceUrl(address);
            try
            {
                var webRequest = string.IsNullOrEmpty(token)
                    ? URLInputStream.OpenConnection(URLInputStream.ResolveUrl(url), user, password)
                    : URLInputStream.OpenConnection(URLInputStream.ResolveUrl(url), token);
                webRequest.Headers.Add(Constants.LIVE_PROP_KEY, Constants.LIVE_PROP_REQUEST_NO);
                using (var response = webRequest.GetResponse())
                {
                    DateTime modificationTime;
                    if (response.GetType() == typeof(FileWebResponse))
                    {
                        var fileUri = new Uri(address);
                        modificationTime = File.GetLastWriteTime(fileUri.AbsolutePath);
                    }
                    else
                    {
                        URLInputStream.CheckConnectionResponseCode(response);
                        modificationTime = response.GetType() == typeof(FtpWebResponse)
                            ? ((FtpWebResponse) response).LastModified
                            : ((HttpWebResponse) response).LastModified;
                    }

                    IList<InstrumentProfile> list;
                    using (var dataStream = response.GetResponseStream())
                    {
                        list = Read(dataStream, url);
                    }

                    lastModified = modificationTime;
                    return list;
                }
            }
            catch (InstrumentProfileFormatException)
            {
                throw;
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception exc)
            {
                throw new IOException("Read profiles from file failed: " + exc);
            }
        }

        /// <summary>
        /// Converts a specified string address specification into an URL that will be read by
        /// ReadFromFile method using URLInputStream.
        /// </summary>
        /// <param name="address">Address to convert.</param>
        /// <returns>A new resolved URL.</returns>
        public static string ResolveSourceUrl(string address)
        {
            // Detect simple "host:port" source and convert it to full HTTP URL
            if (address.IndexOf(':') > 0 && address.IndexOf('/') < 0)
                try
                {
                    var j = address.IndexOf('?');
                    var query = "";
                    if (j >= 0)
                    {
                        query = address.Substring(j);
                        address = address.Substring(0, j);
                    }

                    var port = int.Parse(address.Substring(address.IndexOf(':') + 1));
                    if (port > 0 && port < 65536)
                        address = "http://" + address + "/ipf/all.ipf.gz" + query;
                }
                catch (FormatException)
                {
                    // source does not end with valid port number, so just use it as is
                }

            return address;
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified stream using specified name to select data compression format.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analyzing file name.
        /// If file name ends with ".zip" then all compressed files will be read independently one by one
        /// in their order of appearing and total concatenated list of instrument profiles will be returned.
        /// If file name ends with ".gz" then compressed content will be read and returned.
        /// In other cases file will be considered uncompressed and will be read as is.
        /// </summary>
        /// <param name="inputStream">Stream from which read profiles.</param>
        /// <param name="name">Profile name.</param>
        /// <returns>Instrument profile list.</returns>
        /// <exception cref="ArgumentException">Stream does not support reading.</exception>
        /// <exception cref="ArgumentNullException">Stream is null.</exception>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        /// <exception cref="InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        public IList<InstrumentProfile> Read(Stream inputStream, string name)
        {
            try
            {
                if (name.ToLower().EndsWith(".zip"))
                {
                    using (ZipArchive zip = new ZipArchive(inputStream))
                    {
                        var profiles = new List<InstrumentProfile>();
                        foreach (var entry in zip.Entries)
                        {
                            profiles.AddRange(Read(entry.Open(), entry.Name));
                        }

                        return profiles;
                    }
                }

                if (name.ToLower().EndsWith(".gz"))
                {
                    using (GZipStream gzip = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        return Read(gzip);
                    }
                }

                return Read(inputStream);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InstrumentProfileFormatException)
            {
                throw;
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception exc)
            {
                throw new IOException("Read profiles from stream failed: " + exc);
            }
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified stream.
        /// </summary>
        /// <param name="inputStream">Stream from which read profiles.</param>
        /// <returns>Instrument profiles from specified stream.</returns>
        /// <exception cref="ArgumentException">Stream does not support reading.</exception>
        /// <exception cref="ArgumentNullException">Stream is null.</exception>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        /// <exception cref="InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        public IList<InstrumentProfile> Read(Stream inputStream)
        {
            IList<InstrumentProfile> profiles = new List<InstrumentProfile>();
            InstrumentProfileParser parser = new InstrumentProfileParser(inputStream);
            InstrumentProfile ip;
            while ((ip = parser.Next()) != null)
            {
                try
                {
                    profiles.Add(ip);
                }
                catch (Exception exc)
                {
                    throw new IOException("Read failed: " + exc);
                }
            }

            return profiles;
        }
    }
}