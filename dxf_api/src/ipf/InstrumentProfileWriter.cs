#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using com.dxfeed.ipf.impl;

namespace com.dxfeed.ipf
{
    /// <summary>
    /// Writes instrument profiles to the stream using Simple File Format.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// This writer automatically derives data formats needed to write all meaningful fields.
    /// </summary>
    public class InstrumentProfileWriter
    {
        private const string FILE_EXTENSION = ".ipf";

        /// <summary>
        /// Creates instrument profile writer.
        /// </summary>
        public InstrumentProfileWriter() { }

        /// <summary>
        /// Writes specified instrument profiles into specified file.
        /// This method recognizes popular data compression formats "zip" and "gzip" by analysing file name.
        /// If file name ends with ".zip" then profiles will be written as a single compressed entry in a "zip" format.
        /// If file name ends with ".gz" then profiles will be compressed and written using "gzip" format.
        /// In other cases file will be considered uncompressed and profiles will be written as is.
        /// </summary>
        /// <param name="file">Path to output file.</param>
        /// <param name="profiles">Params list which will be written.</param>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        /// <exception cref="System.InvalidOperationException">Can't format certain profile.</exception>
        public void WriteToFile(string file, IList<InstrumentProfile> profiles)
        {
            using (FileStream outStream = new FileStream(file, FileMode.Create))
            {
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
        /// <param name="outStream">Where writes profiles.</param>
        /// <param name="name">Name of output entry.</param>
        /// <param name="profiles">Params list which will be written.</param>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        /// <exception cref="System.InvalidOperationException">Can't format certain profile.</exception>
        public void Write(Stream outStream, string name, IList<InstrumentProfile> profiles)
        {
            if (name.ToLower().EndsWith(".zip"))
            {
                name = Path.GetFileNameWithoutExtension(name);
                using (ZipArchive zip = new ZipArchive(outStream, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry entry = zip.CreateEntry(Path.GetFileNameWithoutExtension(name) + FILE_EXTENSION);
                    Write(entry.Open(), name, profiles);
                }
                return;
            }
            if (name.ToLower().EndsWith(".gz"))
            {
                using (GZipStream gzip = new GZipStream(outStream, CompressionMode.Compress))
                {
                    Write(gzip, profiles);
                }
                return;
            }
            Write(outStream, profiles);
        }

        /// <summary>
        /// Writes specified instrument profiles into specified stream.
        /// </summary>
        /// <param name="outStream">Where writes profiles.</param>
        /// <param name="profiles">Params list which will be written.</param>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        /// <exception cref="System.InvalidOperationException">Can't format certain profile.</exception>
        public void Write(Stream outStream, IList<InstrumentProfile> profiles)
        {
            InstrumentProfileComposer composer = new InstrumentProfileComposer(outStream);
            composer.Compose(profiles, false);
            composer.ComposeNewLine();
        }
    }
}
