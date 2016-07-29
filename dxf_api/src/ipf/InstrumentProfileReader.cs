/*
 * QDS - Quick Data Signalling Library
 * Copyright (C) 2002-2015 Devexperts LLC
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at
 * http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using com.dxfeed.io;
using com.dxfeed.ipf.impl;

namespace com.dxfeed.ipf {

    /// <summary>
    /// Reads instrument profiles from the stream using Simple File Format.
    /// Please see Instrument Profile Format documentation for complete description.
    /// This reader automatically uses data formats as specified in the stream.
    /// Use {@link InstrumentProfileConnection} if support for streaming updates of instrument profiles is needed.
    /// </summary>
    public class InstrumentProfileReader {
        private static readonly string LIVE_PROP_KEY = "X-Live";
        private static readonly string LIVE_PROP_REQUEST_NO = "no";

        private DateTime lastModified;

        /// <summary>
        /// Creates instrument profile reader.
        /// </summary>
        public InstrumentProfileReader() {}

        /// <summary>
        /// Returns last modification time (in milliseconds) from last {@link #readFromFile} operation
        /// or zero if it is unknown.
        /// </summary>
        /// <returns>Last modification time (in milliseconds)</returns>
        public DateTime GetLastModified() {
            return lastModified;
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified file.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
        /// If file name ends with ".zip" then all compressed files will be read independently one by one
        /// in their order of appearing and total concatenated list of instrument profiles will be returned.
        /// If file name ends with ".gz" then compressed content will be read and returned.
        /// In other cases file will be considered uncompressed and will be read as is.
        ///
        /// Authentication information can be supplied to this method as part of URL user info
        /// like {@code "http://user:password@host:port/path/file.ipf"}.
        ///
        /// This is a shortcut for
        /// <code>{@link #readFromFile(string, string, string) readFromFile}(address, <b>null</b>, <b>null</b>)</code>.
        ///
        /// This operation updates {@link #getLastModified() lastModified}.
        /// </summary>
        /// <param name="address">URL of file to read from.</param>
        /// <returns>List of instrument profiles.</returns>
        /// <exception cref="com.dxfeed.ipf.InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public IList<InstrumentProfile> ReadFromFile(string address) {
            return ReadFromFile(address, null, null);
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified address with a specified basic user and password credentials.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
        /// If file name ends with ".zip" then all compressed files will be read independently one by one
        /// in their order of appearing and total concatenated list of instrument profiles will be returned.
        /// If file name ends with ".gz" then compressed content will be read and returned.
        /// In other cases file will be considered uncompressed and will be read as is.
        ///
        /// Specified user and password take precedence over authentication information that is supplied to this method
        /// as part of URL user info like {@code "http://user:password@host:port/path/file.ipf"}.
        ///
        /// This operation updates {@link #getLastModified() lastModified}.
        /// </summary>
        /// <param name="address">URL of file to read from.</param>
        /// <param name="user">The user name (may be null).</param>
        /// <param name="password">The password (may be null).</param>
        /// <returns>List of instrument profiles.</returns>
        /// <exception cref="com.dxfeed.ipf.InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public IList<InstrumentProfile> ReadFromFile(string address, string user, string password)  {
            string url = ResolveSourceURL(address);
            try {
                WebRequest webRequest = URLInputStream.openConnection(URLInputStream.ResolveURL(url), user, password);
                webRequest.Headers.Add(LIVE_PROP_KEY, LIVE_PROP_REQUEST_NO);
                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse()) {
                    using (Stream dataStream = response.GetResponseStream()) {
                        URLInputStream.checkConnectionResponseCode(response);
                        lastModified = response.LastModified;
                        return Read(dataStream, url);
                    }
                }
            } catch (InstrumentProfileFormatException) {
                throw;
            } catch (IOException) {
                throw;
            } catch (Exception exc) {
                throw new IOException("Read profiles from file failed", exc);
            }
        }

        /// <summary>
        /// Converts a specified string address specification into an URL that will be read by
        /// {@link #readFromFile} method using {@link URLInputStream}.
        /// </summary>
        /// <param name="address">Address to convert.</param>
        /// <returns>A new resolved URL.</returns>
        public static string ResolveSourceURL(string address) {
            // Detect simple "host:port" source and convert it to full HTTP URL
            if (address.IndexOf(':') > 0 && address.IndexOf('/') < 0)
                try {
                    int j = address.IndexOf('?');
                    string query = "";
                    if (j >= 0) {
                        query = address.Substring(j);
                        address = address.Substring(0, j);
                    }
                    int port = Int32.Parse(address.Substring(address.IndexOf(':') + 1));
                    if (port > 0 && port < 65536)
                        address = "http://" + address + "/ipf/all.ipf.gz" + query;
                } catch (FormatException) {
                    // source does not end with valid port number, so just use it as is
                }
            return address;
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified stream using specified name to select data compression format.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
        /// If file name ends with ".zip" then all compressed files will be read independently one by one
        /// in their order of appearing and total concatenated list of instrument profiles will be returned.
        /// If file name ends with ".gz" then compressed content will be read and returned.
        /// In other cases file will be considered uncompressed and will be read as is.
        /// </summary>
        /// <param name="inputStream">Stream from which read profiles.</param>
        /// <param name="name">Profile name.</param>
        /// <returns>Instrument profile list.</returns>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        /// <exception cref="com.dxfeed.ipf.InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        public IList<InstrumentProfile> Read(Stream inputStream, string name) {
            try { 
                // NOTE: decompression streams (zip and gzip) require explicit call to "close()" method to release native Inflater resources.
                // However we shall not close underlying stream here to allow proper nesting of data streams.
                if (name.ToLower().EndsWith(".zip")) {
                    //TODO: uncloseable streams
                    using (ZipArchive zip = new ZipArchive(inputStream)) {
                        List<InstrumentProfile> profiles = new List<InstrumentProfile>();
                        foreach (ZipArchiveEntry entry in zip.Entries) {
                            //TODO: check directory (archive can contains directory?)
                            profiles.AddRange(Read(entry.Open(), entry.Name));
                        }
                        return profiles;
                    }
                }
                if (name.ToLower().EndsWith(".gz")) {
                    //TODO: uncloseable streams
                    using (GZipStream gzip = new GZipStream(inputStream, CompressionMode.Decompress)) {
                        return Read(gzip);
                    }
                }
                return Read(inputStream);
            } catch (InstrumentProfileFormatException) {
                throw;
            } catch (IOException) {
                throw;
            } catch (Exception exc) {
                throw new IOException("Read profiles from stream failed", exc);
            }
        }

        /// <summary>
        /// Reads and returns instrument profiles from specified stream.
        /// </summary>
        /// <param name="inputStream">Stream from which read profiles.</param>
        /// <returns>Instrument profiles from specified stream.</returns>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        /// <exception cref="com.dxfeed.ipf.InstrumentProfileFormatException">If input stream does not conform to the Simple File Format.</exception>
        public IList<InstrumentProfile> Read(Stream inputStream) {
            IList<InstrumentProfile> profiles = new List<InstrumentProfile>();
            InstrumentProfileParser parser = new InstrumentProfileParser(inputStream);
            //TODO:
            //{
                
            //    protected override string intern(string value) {
            //        return InstrumentProfileReader.this.intern(value);
            //    }
            //};
            InstrumentProfile ip;
            while ((ip = parser.Next()) != null) {
                profiles.Add(ip);
            }
            return profiles;
        }

        /// <summary>
        /// To be overridden in subclasses to allow {@link string#intern() intern} strings using pools
        /// (like {@link com.devexperts.util.StringCache StringCache}) to reduce memory footprint. Default implementation does nothing
        /// (returns value itself).
        ///
        /// @param value string value to intern
        /// @return canonical representation of the given string value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string Intern(string value) {
            return value;
        }

    }
}
