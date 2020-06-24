#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.IO;
using System.IO.Compression;

namespace com.dxfeed.io
{
    /// <summary>
    /// Defines byte stream compression format.
    ///
    /// Supported compression formats are: NONE, GZIP(.gz), ZIP(.zip).
    /// NONE compression format serves as a null object and does not do anything.
    /// </summary>
    public class StreamCompression
    {
        /// <summary>
        /// Supported compression types.
        /// </summary>
        public enum CompressionType { None, Gzip, Zip };

        /// <summary>
        /// No compression.
        /// </summary>
        public static StreamCompression NONE = new StreamCompression(CompressionType.None, "none", "", "");

        /// <summary>
        /// Gzip compression format.
        /// </summary>
        public static StreamCompression GZIP = new StreamCompression(CompressionType.Gzip, "gzip", "application/gzip", ".gz");

        /// <summary>
        /// Zip compression format.
        /// </summary>
        public static StreamCompression ZIP = new StreamCompression(CompressionType.Zip, "zip", "application/zip", ".zip");

        private CompressionType type;
        private string name;
        private string mimeType;
        private string extension;

        private StreamCompression(CompressionType type, string name, string mimeType, string extension)
        {
            this.type = type;
            this.name = name;
            this.mimeType = mimeType;
            this.extension = extension;
        }

        /// <summary>
        /// Detects compression format by the mime type.
        /// </summary>
        /// <param name="mimeType">The mime type.</param>
        /// <returns>Detected compression format or NONE is the mime type is not recognized.</returns>
        /// <exception cref="ArgumentNullException">If mimeType is null.</exception>
        public static StreamCompression DetectCompressionByMimeType(string mimeType)
        {
            if (mimeType == null)
                throw new ArgumentNullException("Parameter is null.");
            if (mimeType.Equals(GZIP.mimeType) || mimeType.Equals("application/gzip") || mimeType.Equals("application/x-gzip"))
                return GZIP;
            if (mimeType.Equals(ZIP.mimeType))
                return ZIP;
            return NONE;
        }

        /// <summary>
        /// Detects compression format by the extension at the end of the file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>Detected compression format or NONE is the file name extension is not recognized.</returns>
        /// <exception cref="ArgumentException">If fileName is null.</exception>
        public static StreamCompression DetectCompressionByExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext.Equals(GZIP.extension))
                return GZIP;
            if (ext.Equals(ZIP.extension))
                return ZIP;
            return NONE;
        }

        /// <summary>
        /// Detects compression format by the extension at the end of the file name.
        /// </summary>
        /// <param name="fileUri">The file name Uri object.</param>
        /// <returns>Detected compression format or NONE is the file name extension is not recognized.</returns>
        /// <exception cref="ArgumentException">If fileUri is null.</exception>
        public static StreamCompression DetectCompressionByExtension(Uri fileUri)
        {
            if (fileUri == null)
                throw new ArgumentException("File Uri is null");
            return DetectCompressionByExtension(fileUri.AbsolutePath);
        }

        /// <summary>
        /// Decompresses the given input stream with this compression format.
        /// </summary>
        /// <param name="inputStream">In the input stream.</param>
        /// <returns>The decompressed stream or an original stream if this compression format is NONE.</returns>
        /// <exception cref="ArgumentException">If input stream is null.</exception>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        public Stream Decompress(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentException("Input stream is null.");
            try
            {
                switch (type)
                {
                    case CompressionType.None:
                        return inputStream;
                    case CompressionType.Gzip:
                        return new GZipStream(inputStream, CompressionMode.Decompress);
                    case CompressionType.Zip:
                        MemoryStream zipStream = new MemoryStream();
                        using (ZipArchive zip = new ZipArchive(inputStream))
                        {
                            foreach (ZipArchiveEntry entry in zip.Entries)
                            {
                                entry.Open().CopyTo(zipStream);
                            }
                        }
                        zipStream.Position = 0;
                        return zipStream;
                    default:
                        throw new InvalidOperationException("Unsupported compression type: " + type);
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new IOException("Internal error:" + e);
            }
        }
    }
}
