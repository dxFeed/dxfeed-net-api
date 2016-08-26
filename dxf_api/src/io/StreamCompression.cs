using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dxfeed.io
{

    /// <summary>
    /// Defines byte stream compression format.
    ///
    /// Supported compression formats are: NONE, GZIP(.gz), ZIP(.zip).
    /// NONE compression format serves as a null object and does not do anything.
    /// </summary>
    public class StreamCompression {

        /// <summary>
        /// Supported compression types.
        /// </summary>
        public enum CompressionType { None, Gzip, Zip};

        /// <summary>
        /// No compression.
        /// </summary>
        public static StreamCompression NONE = new StreamCompression(CompressionType.None, "none", "");

        /// <summary>
        /// Gzip compression format.
        /// </summary>
        public static StreamCompression GZIP = new StreamCompression(CompressionType.Gzip, "gzip", ".gz");

        /// <summary>
        /// Zip compression format.
        /// </summary>
        public static StreamCompression ZIP = new StreamCompression(CompressionType.Zip, "zip", ".zip");

        private CompressionType type;
        private string name;
        private string extension;

        private StreamCompression(CompressionType type, string name, string extension) {
            this.type = type;
            this.name = name;
            this.extension = extension;
        }

        /// <summary>
        /// Detects compression format by the extension at the end of the file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>Detected compression format or NONE is the file name extension is not recognized.</returns>
        /// <exception cref="ArgumentException">If fileName is null.</exception>
        public static StreamCompression DetectCompressionByExtension(string fileName) {
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
        public static StreamCompression DetectCompressionByExtension(Uri fileUri) {
            if (fileUri == null)
                throw new ArgumentException("File Uri is null");
            return DetectCompressionByExtension(fileUri.AbsolutePath);
        }

        /// <summary>
        /// Detects compression format by the magic number in the file header. This method
        /// marks the stream, read first 4 bytes, and resets the stream to an original state.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>Detected compression format or NONE is the header is not recognized.</returns>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static StreamCompression DetectCompressionByHeader(Stream inputStream) {
            int n = 4;
            byte[] buffer = new byte[n];
            int mark = n;
            try {
                int read = inputStream.Read(buffer, 0, n);
                inputStream.Position = mark;
                if (read != n)
                    return NONE;
                if (buffer[0] == (byte)0x1f && buffer[1] == (byte)0x8b)
                    return GZIP;
                if (buffer[0] == 'P' && buffer[1] == 'K' && buffer[2] == 0x03 && buffer[3] == 0x04)
                    return ZIP;
                return NONE;
            } catch (IOException) {
                throw;
            } catch (Exception e) {
                throw new InvalidOperationException("Detect compression exception: " + e);
            }
        }

        /// <summary>
        /// Detects compression format by the magic number in the file header and decompresses
        /// the given input stream. 
        /// </summary>
        /// <param name="inputStream">In the input stream.</param>
        /// <returns>The decompressed stream.</returns>
        /// <exception cref="ArgumentException">If input stream is null.</exception>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static Stream DetectCompressionByHeaderAndDecompress(Stream inputStream) {
            return DetectCompressionByHeader(inputStream).Decompress(inputStream);
        }

        /// <summary>
        /// Decompresses the given input stream with this compression format.
        /// </summary>
        /// <param name="inputStream">In the input stream.</param>
        /// <returns>The decompressed stream or an original stream if this compression format is NONE.</returns>
        /// <exception cref="ArgumentException">If input stream is null.</exception>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        public Stream Decompress(Stream inputStream) {
            if (inputStream == null)
                throw new ArgumentException("Input stream is null.");
            try {
                switch (type) {
                    case CompressionType.None:
                        return inputStream;
                    case CompressionType.Gzip:
                        return new GZipStream(inputStream, CompressionMode.Decompress);
                    case CompressionType.Zip:
                        MemoryStream zipStream = new MemoryStream();
                        using (ZipArchive zip = new ZipArchive(inputStream)) {
                            foreach (ZipArchiveEntry entry in zip.Entries) {
                                entry.Open().CopyTo(zipStream);
                            }
                        }
                        zipStream.Position = 0;
                        return zipStream;
                    default:
                        throw new InvalidOperationException("Unsupported compression type: " + type);
                }
            } catch (IOException) {
                throw;
            } catch (Exception e) {
                throw new IOException("Internal error:" + e);
            }
        }

    }
}
