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
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// This reader automatically uses data formats as specified in the stream.
    /// <p>Use {@link InstrumentProfileConnection} if support for streaming updates of instrument profiles is needed.
    /// </summary>
    public class InstrumentProfileReader {
        private static readonly string LIVE_PROP_KEY = "X-Live";
        private static readonly string LIVE_PROP_REQUEST_NO = "no";

        private DateTime lastModified;

        /**
         * Creates instrument profile reader.
         */
        public InstrumentProfileReader() {}

        /**
         * Returns last modification time (in milliseconds) from last {@link #readFromFile} operation
         * or zero if it is unknown.
         */
        public DateTime getLastModified() {
            return lastModified;
        }

        /**
         * Reads and returns instrument profiles from specified file.
         * This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
         * If file name ends with ".zip" then all compressed files will be read independently one by one
         * in their order of appearing and total concatenated list of instrument profiles will be returned.
         * If file name ends with ".gz" then compressed content will be read and returned.
         * In other cases file will be considered uncompressed and will be read as is.
         *
         * <p>Authentication information can be supplied to this method as part of URL user info
         * like {@code "http://user:password@host:port/path/file.ipf"}.
         *
         * <p>This is a shortcut for
         * <code>{@link #readFromFile(string, string, string) readFromFile}(address, <b>null</b>, <b>null</b>)</code>.
         *
         * <p>This operation updates {@link #getLastModified() lastModified}.
         *
         * @param address URL of file to read from
         * @return list of instrument profiles
         *
         * @throws InstrumentProfileFormatException if input stream does not conform to the Simple File Format
         * @throws IOException  If an I/O error occurs
         */
        //throws IOException
        public IList<InstrumentProfile> readFromFile(string address) {
            return readFromFile(address, null, null);
        }

        /**
         * Reads and returns instrument profiles from specified address with a specified basic user and password credentials.
         * This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
         * If file name ends with ".zip" then all compressed files will be read independently one by one
         * in their order of appearing and total concatenated list of instrument profiles will be returned.
         * If file name ends with ".gz" then compressed content will be read and returned.
         * In other cases file will be considered uncompressed and will be read as is.
         *
         * <p>Specified user and password take precedence over authentication information that is supplied to this method
         * as part of URL user info like {@code "http://user:password@host:port/path/file.ipf"}.
         *
         * <p>This operation updates {@link #getLastModified() lastModified}.
         *
         * @param address URL of file to read from.
         * @param user the user name (may be null).
         * @param password the password (may be null).
         * @return list of instrument profiles.
         *
         * @throws InstrumentProfileFormatException if input stream does not conform to the Simple File Format.
         * @throws IOException  If an I/O error occurs.
         */
        //throws IOException
        public IList<InstrumentProfile> readFromFile(string address, string user, string password)  {
            string url = resolveSourceURL(address);
            WebRequest webRequest = URLInputStream.openConnection(URLInputStream.ResolveURL(url), user, password);
            webRequest.Headers.Add(LIVE_PROP_KEY, LIVE_PROP_REQUEST_NO);
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse()) {
                using (Stream dataStream = response.GetResponseStream()) {
                    URLInputStream.checkConnectionResponseCode(response);
                    lastModified = response.LastModified;
                    return read(dataStream, url);
                }
            }
        }

        /// <summary>
        /// Converts a specified string address specification into an URL that will be read by
        /// {@link #readFromFile} method using {@link URLInputStream}.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>A new resolved URL.</returns>
        public static string resolveSourceURL(string address) {
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

        /**
         * Reads and returns instrument profiles from specified stream using specified name to select data compression format.
         * This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
         * If file name ends with ".zip" then all compressed files will be read independently one by one
         * in their order of appearing and total concatenated list of instrument profiles will be returned.
         * If file name ends with ".gz" then compressed content will be read and returned.
         * In other cases file will be considered uncompressed and will be read as is.
         *
         * @throws InstrumentProfileFormatException if input stream does not conform to the Simple File Format
         * @throws IOException  If an I/O error occurs
         */
        public IList<InstrumentProfile> read(Stream inputStream, string name) {
            // NOTE: decompression streams (zip and gzip) require explicit call to "close()" method to release native Inflater resources.
            // However we shall not close underlying stream here to allow proper nesting of data streams.
            if (name.ToLower().EndsWith(".zip")) {

                //TODO: uncloseable streams
                using (ZipArchive zip = new ZipArchive(inputStream)) {
                    List<InstrumentProfile> profiles = new List<InstrumentProfile>();
                    foreach (ZipArchiveEntry entry in zip.Entries) {
                        //TODO: check directory
                        profiles.AddRange(read(entry.Open(), entry.Name));
                    }
                    return profiles;
                }
            }
            if (name.ToLower().EndsWith(".gz")) {
                //TODO: uncloseable streams
                using (GZipStream gzip = new GZipStream(inputStream, CompressionMode.Decompress)) {
                    return read(gzip);
                }
            }
            return read(inputStream);
        }

        /**
         * Reads and returns instrument profiles from specified stream.
         *
         * @throws InstrumentProfileFormatException if input stream does not conform to the Simple File Format
         * @throws IOException  If an I/O error occurs
         */
        public IList<InstrumentProfile> read(Stream inputStream) {
            IList<InstrumentProfile> profiles = new List<InstrumentProfile>();
            InstrumentProfileParser parser = new InstrumentProfileParser(inputStream);
            //TODO:
            //{
                
            //    protected override string intern(string value) {
            //        return InstrumentProfileReader.this.intern(value);
            //    }
            //};
            InstrumentProfile ip;
            while ((ip = parser.next()) != null)
                profiles.Add(ip);
            return profiles;
        }

        /**
         * To be overridden in subclasses to allow {@link string#intern() intern} strings using pools
         * (like {@link com.devexperts.util.StringCache StringCache}) to reduce memory footprint. Default implementation does nothing
         * (returns value itself).
         *
         * @param value string value to intern
         * @return canonical representation of the given string value
         */
        protected string intern(string value) {
            return value;
        }

    }
}
