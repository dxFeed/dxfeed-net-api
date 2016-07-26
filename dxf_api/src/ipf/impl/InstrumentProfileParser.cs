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
using System.Text;
using com.dxfeed.io;

namespace com.dxfeed.ipf.impl {

    /// <summary>
    /// Parser for Instrument Profile Simple File Format.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    class InstrumentProfileParser : IDisposable {

        private Dictionary<string, object[]> formats = new Dictionary<string,object[]>();
        private CSVReader reader;

        public InstrumentProfileParser(Stream stream) {
            reader = new CSVReader(new StreamReader(stream, Encoding.UTF8));
        }

        public void Dispose() {
            reader.Dispose();
        }

        /// <summary>
        /// Return next instrument profile.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InstrumentProfileFormatException"></exception>
        public InstrumentProfile next() {
            while (true) {
                int line = reader.getLineNumber();
                string[] record;
                try {
                    record = reader.readRecord();
                } catch (CSVFormatException e) {
                    throw new InstrumentProfileFormatException(e.Message);
                }
                if (record == null) // EOF reached
                    return null;
                if (record.Length == 0 || record.Length == 1 && String.IsNullOrEmpty(record[0])) // skip empty lines
                    continue;
                if (record[0].StartsWith(Constants.METADATA_PREFIX)) {
                    switch (record[0]) {
                    case Constants.FLUSH_COMMAND:
                        onFlush();
                        break;
                    case Constants.COMPLETE_COMMAND:
                        onComplete();
                        break;
                    }
                    if (!record[0].EndsWith(Constants.METADATA_SUFFIX)) // skip comments
                        continue;
                    // detected valid metadata record - parse and remember new format
                    object[] newFormat = new object[record.Length];
                    newFormat[0] = InstrumentProfileField.TYPE;
                    for (int i = 1; i < record.Length; i++)
                        if ((newFormat[i] = InstrumentProfileField.find(record[i])) == null)
                            newFormat[i] = String.Intern(record[i]);
                    string key = record[0].Substring(
                        Constants.METADATA_PREFIX.Length, 
                        record[0].Length - Constants.METADATA_SUFFIX.Length - Constants.METADATA_PREFIX.Length);
                    formats.Add(key, newFormat);
                    continue;
                }
                // detected instrument profile record - parse and remember new profile
                object[] format = formats[record[0]];
                if (format == null)
                    throw new InstrumentProfileFormatException("undefined format " + record[0] + " (line " + line + ")");
                if (record.Length != format.Length)
                    throw new InstrumentProfileFormatException("wrong number of fields (line " + line + ")");
                InstrumentProfile ip = new InstrumentProfile();
                for (int i = 0; i < format.Length; i++)
                    try {
                        if (format[i].GetType() == typeof(InstrumentProfileField)) {
                            InstrumentProfileField field = (InstrumentProfileField)format[i];
                            field.setField(ip, (field.isNumericField()) ? record[i] : intern(record[i]));
                        } else {
                            ip.setField((string)format[i], intern(record[i]));
                        }
                    } catch (Exception e) {
                        throw new InstrumentProfileFormatException(e.Message + " (line " + line + ")");
                    }
                return ip;
            }
        }

        protected string intern(string value) {
            return value;
        }

        protected void onFlush() {}

        protected void onComplete() {}

    }
}
