using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dxfeed.io {
    public class StreamCompression {

        public enum CompressionType { None, Gzip, Zip};

        /**
         * No compression.
         */
        public static StreamCompression NONE = new StreamCompression(CompressionType.None, "none", "");

        /**
         * Gzip compression format.
         */
        public static StreamCompression GZIP = new StreamCompression(CompressionType.Gzip, "gzip", ".gz");

        /**
         * Zip compression format.
         */
        public static StreamCompression ZIP = new StreamCompression(CompressionType.Zip, "zip", ".zip");

        private CompressionType type;
        private string name;
        private string extension;

        private StreamCompression(CompressionType type, string name, string extension) {
            this.type = type;
            this.name = name;
            this.extension = extension;
        }

        /**
         * Detects compression format by the magic number in the file header. This method
         * {@link InputStream#mark(int) marks} the stream, read first 4 bytes, and
         * {@link InputStream#reset() resets} the stream to an original state.
         *
         * @param in the input stream.
         * @return detected compression format or {@link #NONE} is the header is not recognized.
         * @throws IOException              if an I/O error occurs.
         * @throws IllegalArgumentException if {@code in} does not {@link InputStream#markSupported() support mark}.
         */
        public static StreamCompression DetectCompressionByHeader(Stream inputStream) {
            int n = 4;
            byte[] buffer = new byte[n];
            int mark = n;
            int read = inputStream.Read(buffer, 0, n);
            inputStream.Position = mark;
            if (read != n)
                return NONE;
            if (buffer[0] == (byte)0x1f && buffer[1] == (byte)0x8b)
                return GZIP;
            if (buffer[0] == 'P' && buffer[1] == 'K' && buffer[2] == 0x03 && buffer[3] == 0x04)
                return ZIP;
            return NONE;
        }

        /**
         * Detects compression format by the magic number in the file header and decompresses
         * the given input stream. This method wraps the input stream in {@link BufferedInputStream} if
         * the original stream does not {@link InputStream#markSupported() support mark} before using
         * {@link #detectCompressionByHeader(InputStream) detectCompressionByHeader} method.
         *
         * @param in the input stream.
         * @return the decompressed stream.
         * @throws IOException if an I/O error occurs.
         */
        public static Stream DetectCompressionByHeaderAndDecompress(Stream inputStream) {
            return DetectCompressionByHeader(inputStream).Decompress(inputStream);
        }

        /**
         * Decompresses the given input stream with this compression format.
         *
         * @param in the input stream.
         * @return the decompressed stream or an original stream if this compression format is {@link #NONE}.
         * @throws IOException if an I/O error occurs.
         */
        public Stream Decompress(Stream inputStream) {
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
                    return zipStream;
                default:
                    throw new InvalidOperationException("Unsupported compression type: " + type);
            }
        }

    }
}
