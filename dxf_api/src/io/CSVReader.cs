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
using com.dxfeed.util;

namespace com.dxfeed.io {

    /// <summary>
    /// Reads data from the stream using Comma-Separated Values (CSV) format.
    /// See <a href="http://www.rfc-editor.org/rfc/rfc4180.txt">RFC 4180</a> for CSV format specification.
    /// <p>
    /// This reader supports records with arbitrary (variable) number of fields, multiline fields,
    /// custom separator and quote characters. It accepts <b>CR</b>, <b>LF</b> and <b>CRLF</b> sequence
    /// as record separators.
    /// <p>
    /// This reader provides its own buffering but does not perform decoding.
    /// The correct way to efficiently read CSV file with UTF-8 encoding is as follows:
    /// <pre>
    /// CSVReader reader = new CSVReader(new InputStreamReader(new FileInputStream(file), StandardCharsets.UTF_8));
    /// string[] header = reader.readRecord();
    /// List&lt;string[]&gt; records = reader.readAll();
    /// reader.close();
    /// </pre>
    /// </summary>
    public class CSVReader : IDisposable {
        private static readonly char CR = '\r';
        private static readonly char LF = '\n';

        private StreamReader reader;
        private char separator;
        private char quote;

        private StringCache strings = new StringCache(); // LRU cache to reduce memory footprint and garbage.

        private char[] buf = new char[8192];
        private int position;
        private int limit;
        private bool eol;
        private bool eof;
        private int lineNumber = 1;
        private int recordNumber = 1;

        /// <summary>
        /// Creates new CSVReader with default separator and quote characters.
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="NullReferenceException">If reader is null.</exception>
        public CSVReader(StreamReader reader)
            : this(reader, ',', '"') { }

        /// <summary>
        /// Creates new CSVReader with specified separator and quote characters.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="separator">Separator.</param>
        /// <param name="quote">Quote charachter.</param>
        /// <exception cref="NullReferenceException">If reader is null.</exception>
        /// <exception cref="ArgumentException">If separator or quote characters are invalid.</exception>
        public CSVReader(StreamReader reader, char separator, char quote) {
            if (reader == null)
                throw new NullReferenceException("reader is null");
            if (separator == CR || separator == LF)
                throw new ArgumentException("separator is CR or LF");
            if (quote == CR || quote == LF || quote == separator)
                throw new ArgumentException("quote is CR, LF or same as separator");
            this.reader = reader;
            this.separator = separator;
            this.quote = quote;
        }

        /// <summary>
        /// Returns current line number.
        /// Line numeration starts with 1 and counts new lines within record fields.
        /// Both <b>CR</b> and <b>LF</b> are counted as new lines, although <b>CRLF</b> sequence is counted only once.
        /// Line number points to new line after completion of the current record.
        /// </summary>
        /// <returns></returns>
        public int GetLineNumber() {
            return lineNumber;
        }

        /// <summary>
        /// Returns current record number.
        /// Record numeration starts with 1 and does not count new lines within record fields.
        /// Record number points to new record after completion of the current record.
        /// </summary>
        /// <returns></returns>
        public int GetRecordNumber() {
            return recordNumber;
        }

        /// <summary>
        /// Reads and returns a single field of the current record or <b>null</b> if record has ended.
        /// Returns empty string if field is empty.
        /// This method does not advance to the next record - it keeps to return <b>null</b>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSVFormatException">If input stream does not conform to the CSV format.</exception>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        public string ReadField() {
            if (eol || eof)
                return null;
            if (position > buf.Length / 2) {
                Array.Copy(buf, position, buf, 0, limit - position);
                limit -= position;
                position = 0;
            }
            int pos = position;
            int firstLine = lineNumber;
            bool inQuotes = false; // true if field is quoted
            bool hasQuotes = false; // true if quoted field has quotes inside
            bool oddQuote = false; // true if we just met odd quote inside quoted field; become false if pair quote follows
            while (true) {
                if (pos >= limit)
                    Read();
                if (pos >= limit) {
                    if (inQuotes && !oddQuote)
                        throw new CSVFormatException(Fail("quoted field does not have terminating quote character", firstLine));
                    eof = true;
                    return Good(pos, inQuotes, hasQuotes, false);
                }
                char c = buf[pos];
                if (c == quote) {
                    if (inQuotes) {
                        hasQuotes |= oddQuote;
                        oddQuote = !oddQuote;
                    } else if (pos > position)
                        throw new CSVFormatException(Fail("unquoted field has quote character", firstLine));
                    inQuotes = true;
                    pos++;
                    continue;
                }
                if ((c == separator || c == CR || c == LF) && (!inQuotes || oddQuote))
                    return Good(pos, inQuotes, hasQuotes, c == separator);
                if (oddQuote)
                    throw new CSVFormatException(Fail("quoted field has unpaired quote character", firstLine));
                // below: CR or LF imply inQuotes=true, thus buf[pos-1] exists as buf holds at least quote char
                if (c == CR || c == LF && buf[pos - 1] != CR) // count CRLF only once
                    lineNumber++;
                pos++;
            }
        }

        private string Good(int fieldEnd, bool inQuotes, bool hasQuotes, bool bySeparator) {
            // NOTE: this method not only unquotes field, but it also updates state machine
            int pos = position + (inQuotes ? 1 : 0);
            int end = fieldEnd - (inQuotes ? 1 : 0);
            position = fieldEnd + (bySeparator ? 1 : 0);
            eol = !bySeparator;
            if (hasQuotes) {
                int n = pos;
                for (int i = pos; i < end; i++)
                    if ((buf[n++] = buf[i]) == quote)
                        i++;
                end = n;
            }
            return strings.get(buf, pos, end - pos);
        }

        private string Fail(string message, int firstLine) {
            // NOTE: this method not only formats error, but it also updates state machine
            if (firstLine == lineNumber)
                return message + " (line " + firstLine + ")";
            message = message + " (lines from " + firstLine + " to " + lineNumber + ")";
            lineNumber = firstLine;
            return message;
        }

        private void Read() {
            if (buf.Length - limit < buf.Length / 4)
                Array.Resize(ref buf, 0);
            int n;
            do {
                n = reader.Read(buf, limit, buf.Length - limit);
            } while (n == 0);
            if (n > 0)
                limit += n;
        }

        /// <summary>
        /// Reads and returns a remaining fields of the current record or <b>null</b> if stream has ended.
        /// Returns empty array (length 0) if all record fields were already read by {@link #readField} method.
        /// Returns array of length 1 with single empty string if record is empty (empty line).
        /// Returns empty strings for those fields that are empty.
        /// This method advances to the next record upon completion.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSVFormatException">If input stream does not conform to the CSV format.</exception>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        public string[] ReadRecord() {
            List<string> fields = new List<string>();
            for (string field = ReadField(); field != null; field = ReadField())
                fields.Add(field);
            if (eol) {
                eol = false;
                bool skipLF = false;
                while (!eof) {
                    if (position >= limit)
                        Read();
                    if (position >= limit) {
                        eof = true;
                        break;
                    }
                    if (skipLF) { // second loop after CR
                        if (buf[position] == LF)
                            position++;
                        break;
                    }
                    // first loop - either CR or LF
                    lineNumber++;
                    if (buf[position++] == LF)
                        break;
                    skipLF = true; // it was CR - to to second loop
                }
                recordNumber++;
                return fields.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Reads and returns all records or empty list if stream has ended.
        /// Empty records are represented by arrays of length 1 with single empty string.
        /// Empty fields are represented by empty strings.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSVFormatException">If input stream does not conform to the CSV format.</exception>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        public List<string[]> ReadAll() {
            List<string[]> records = new List<string[]>();
            for (string[] record = ReadRecord(); record != null; record = ReadRecord())
                records.Add(record);
            return records;
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        public void Dispose() {
            reader.Close();
        }

    }
}

