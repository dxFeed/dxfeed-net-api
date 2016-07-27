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
using com.dxfeed.ipf.impl;

namespace com.dxfeed.ipf {

    /// <summary>
    /// Writes instrument profiles to the stream using Simple File Format.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// This writer automatically derives data formats needed to write all meaningful fields.
    /// </summary>
    class InstrumentProfileWriter {
        
        /// <summary>
        /// Creates instrument profile writer.
        /// </summary>
        public InstrumentProfileWriter() {}

        /// <summary>
        /// Writes specified instrument profiles into specified file.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
        /// If file name ends with ".zip" then profiles will be written as a single compressed entry in a "zip" format.
        /// If file name ends with ".gz" then profiles will be compressed and written using "gzip" format.
        /// In other cases file will be considered uncompressed and profiles will be written as is.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="profiles"></param>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void WriteToFile(string file, List<InstrumentProfile> profiles) {
            using (FileStream outStream = new FileStream(file, FileMode.Create)) {
                Write(outStream, file, profiles);
            }
        }

        /// <summary>
        /// Writes specified instrument profiles into specified stream using specified name to select data compression format.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
        /// If file name ends with ".zip" then profiles will be written as a single compressed entry in a "zip" format.
        /// If file name ends with ".gz" then profiles will be compressed and written using "gzip" format.
        /// In other cases file will be considered uncompressed and profiles will be written as is.
        /// </summary>
        /// <param name="outStream"></param>
        /// <param name="name"></param>
        /// <param name="profiles"></param>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void Write(Stream outStream, string name, List<InstrumentProfile> profiles) {
            // NOTE: compression streams (zip and gzip) require explicit call to "close()" method to properly
            // finish writing of compressed file format and to release native Deflater resources.
            // However we shall not close underlying stream here to allow proper nesting of data streams.
            if (name.ToLower().EndsWith(".zip")) {
                name = Path.GetFileNameWithoutExtension(name);
                //TODO: ucloseable stream
                using (ZipArchive zip = new ZipArchive(outStream)) {
                    ZipArchiveEntry entry = zip.CreateEntry(name);
                    Write(entry.Open(), name, profiles);
                }
                return;
            }
            if (name.ToLower().EndsWith(".gz")) {
                //TODO: ucloseable stream
                using (GZipStream gzip = new GZipStream(outStream, CompressionMode.Compress)) {
                    Write(gzip, profiles);
                }
                return;
            }
            Write(outStream, profiles);
        }

        /// <summary>
        /// Writes specified instrument profiles into specified stream.
        /// </summary>
        /// <param name="outStream"></param>
        /// <param name="profiles"></param>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void Write(Stream outStream, List<InstrumentProfile> profiles) {
            InstrumentProfileComposer composer = new InstrumentProfileComposer(outStream);
            composer.compose(profiles, false);
            composer.composeNewLine();
        }
    }
}
