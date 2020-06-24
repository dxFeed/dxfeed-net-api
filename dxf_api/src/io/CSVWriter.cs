#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.IO;

namespace com.dxfeed.io
{
    /// <summary>
    /// Writes data to the stream using Comma-Separated Values (CSV) format.
    /// See <a href="http://www.rfc-editor.org/rfc/rfc4180.txt">RFC 4180</a> for CSV format specification.
    /// <p>
    /// This writer supports records with arbitrary (variable) number of fields, multiline fields,
    /// custom separator and quote characters. It uses <b>CRLF</b> sequence to separate records.
    /// <p>
    /// This writer does not provide buffering of any sort and does not perform encoding.
    /// The correct way to efficiently write CSV file with UTF-8 encoding is as follows:
    /// <pre>
    /// CSVWriter writer = new CSVWriter(new BufferedWriter(new OutputStreamWriter(new FileOutputStream(file), StandardCharsets.UTF_8)));
    /// writer.writeRecord(header);
    /// writer.writeAll(records);
    /// writer.close();
    /// </pre>
    /// </summary>
    class CSVWriter : IDisposable
    {
        private static readonly char CR = '\r';
        private static readonly char LF = '\n';
        private static readonly char[] CRLF = { CR, LF };

        private readonly StreamWriter writer;
        private readonly char separator;
        private readonly char quote;

        private bool needCRLF;
        private bool insideRecord;
        private int lineNumber = 1;
        private int recordNumber = 1;

        private char[] quoteBuf; // Used for quoting fields; lazy initialized.

        /// <summary>
        /// Creates new CSVWriter with default separator and quote characters.
        /// </summary>
        /// <param name="writer"></param>
        /// <exception cref="System.NullReferenceException">If writer is null.</exception>
        public CSVWriter(StreamWriter writer) : this(writer, ',', '"') { }

        /// <summary>
        /// Creates new CSVWriter with specified separator and quote characters.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="separator"></param>
        /// <param name="quote"></param>
        /// <exception cref="System.NullReferenceException">If writer is null.</exception>
        /// <exception cref="System.ArgumentException">If separator or quote characters are invalid.</exception>
        public CSVWriter(StreamWriter writer, char separator, char quote)
        {
            if (writer == null)
                throw new NullReferenceException("writer is null");
            if (separator == CR || separator == LF)
                throw new ArgumentException("separator is CR or LF");
            if (quote == CR || quote == LF || quote == separator)
                throw new ArgumentException("quote is CR, LF or same as separator");
            this.writer = writer;
            this.separator = separator;
            this.quote = quote;
        }

        /// <summary>
        /// Returns current line number.
        /// Line numeration starts with 1 and counts new lines within record fields.
        /// Both <b>CR</b> and <b>LF</b> are counted as new lines, although <b>CRLF</b> sequence is counted only once.
        /// Line number points to new line after completion of the current record.
        /// </summary>
        /// <returns>Current line number.</returns>
        public int GetLineNumber()
        {
            return lineNumber;
        }

        /// <summary>
        /// Returns current record number.
        /// Record numeration starts with 1 and does not count new lines within record fields.
        /// Record number points to new record after completion of the current record.
        /// </summary>
        /// <returns>Current record number.</returns>
        public int GetRecordNumber()
        {
            return recordNumber;
        }

        /// <summary>
        /// Writes specified field to the end of current record.
        /// Empty and <b>null</b> strings are written as empty fields.
        /// This method does not advance to the next record - it only adds field to the current record.
        /// </summary>
        /// <param name="field"></param>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void WriteField(string field)
        {
            try
            {
                if (needCRLF)
                {
                    writer.Write(CRLF);
                    needCRLF = false;
                    lineNumber++;
                }
                if (insideRecord)
                    writer.Write(separator);
                insideRecord = true;
                if (field == null || string.IsNullOrEmpty(field))
                    return;
                if (field.IndexOf(separator) < 0 && field.IndexOf(quote) < 0 && field.IndexOf(CR) < 0 && field.IndexOf(LF) < 0)
                {
                    writer.Write(field);
                    return;
                }
                int capacity = field.Length * 2 + 2;
                char[] buf = capacity > 512 ? new char[capacity] : quoteBuf != null ? quoteBuf : (quoteBuf = new char[512]);
                int n = 0;
                buf[n++] = quote;
                for (int i = 0; i < field.Length; i++)
                {
                    char c = field[i];
                    // below: buf[n-1] exists as buf holds at least quote char
                    if (c == CR || c == LF && buf[n - 1] != CR) // count CRLF only once
                        lineNumber++;
                    if (c == quote)
                        buf[n++] = quote;
                    buf[n++] = c;
                }
                buf[n++] = quote;
                writer.Write(buf, 0, n);
            }
            catch (Exception exc)
            {
                throw new IOException("WriteField failed: " + exc);
            }
        }

        /// <summary>
        /// Writes specified record and advances to the next record upon completion.
        /// Empty and <b>null</b> arrays are normally prohibited because records must
        /// contain at least one field, but they can be used to complete current record.
        /// If there is incomplete record (written by {@link #writeField} method)
        /// then specified fields will be added to the end of current record,
        /// the record will be completed and writer will advance to the next record.
        /// </summary>
        /// <param name="record"></param>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void WriteRecord(string[] record)
        {
            if (record != null)
                foreach (string field in record)
                    WriteField(field);
            if (!insideRecord)
                throw new ArgumentException("records without fields are not allowed");
            needCRLF = true;
            insideRecord = false;
            recordNumber++;
        }

        /// <summary>
        /// Writes specified records to the output. Does nothing if specified list is empty.
        /// Empty and <b>null</b> arrays are normally prohibited because records must
        /// contain at least one field, but they can be used to complete current record.
        /// If there is incomplete record (written by {@link #writeField} method)
        /// then fields from first specified record will be added to the end of current record,
        /// the record will be completed and writer will advance to the next record.
        /// </summary>
        /// <param name="records"></param>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void WriteAll(IList<string[]> records)
        {
            foreach (string[] record in records)
                WriteRecord(record);
        }

        /// <summary>
        /// Flushes the stream.
        /// </summary>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void Flush()
        {
            try
            {
                writer.Flush();
            }
            catch (Exception exc)
            {
                throw new IOException("Flush failed: " + exc);
            }
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void Dispose()
        {
            try
            {
                writer.Close();
            }
            catch (Exception exc)
            {
                throw new IOException("Dispose failed: " + exc);
            }
        }
    }
}
