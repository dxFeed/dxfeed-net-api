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
using com.dxfeed.io;

namespace com.dxfeed.ipf.impl {

    /// <summary>
    /// Composer for Instrument Profile Simple File Format.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    class InstrumentProfileComposer : IDisposable {
        private static readonly InstrumentProfileField[] FIELDS = InstrumentProfileField.Values;
        private static readonly string REMOVED_TYPE = InstrumentProfileType.REMOVED.Name;

        private Dictionary<string, HashSet<InstrumentProfileField>> enumFormats = new Dictionary<string, HashSet<InstrumentProfileField>>();
        private Dictionary<string, HashSet<string>> customFormats = new Dictionary<string, HashSet<string>>();
        private List<string> types = new List<string>(); // atomically captures types
        private CSVWriter writer;

        /// <summary>
        /// Creates a new instrument profile composer.
        /// </summary>
        /// <param name="outStream"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public InstrumentProfileComposer(Stream outStream) {
            writer = new CSVWriter(new StreamWriter(outStream, System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// Dispose object.
        /// </summary>
        public void Dispose() {
            writer.Dispose();
        }

        /// <summary>
        /// Atomically captures profile types to work correctly when profile type is being changed concurrently,
        /// otherwise, the method is not thread-safe.
        /// </summary>
        /// <param name="profiles">List of instrument profiles.</param>
        /// <param name="skipRemoved">When skipRemoved == true, it ignores removed instruments when composing.</param>
        /// <exception cref="System.IO.IOException"></exception>
        public void compose(List<InstrumentProfile> profiles, bool skipRemoved)  {
            captureTypes(profiles);
            writeFormats(profiles, skipRemoved);
            writeProfiles(profiles, skipRemoved);
            types.Clear();
        }

        /// <summary>
        /// Writes a new line.
        /// </summary>
        public void composeNewLine() {
            writer.writeRecord(new string[] {""}); // Force CRLF
            writer.flush();
        }

        /// <summary>
        /// Writes FLUSH command
        /// </summary>
        public void composeFlush()  {
            writer.writeRecord(new string[] { Constants.FLUSH_COMMAND });
            composeNewLine();
        }

        /// <summary>
        /// Writes COMPLETE command.
        /// </summary>
        public void composeComplete()  {
            writer.writeRecord(new string[] { Constants.COMPLETE_COMMAND });
            composeNewLine();
        }

        private void captureTypes(List<InstrumentProfile> profiles) {
            types.Clear();
            foreach (InstrumentProfile ip in profiles)
                types.Add(ip.GetTypeName());
        }

        private void writeFormats(List<InstrumentProfile> profiles, bool skipRemoved) {
            HashSet<string> updated = new HashSet<string>();
            for (int i = 0; i < profiles.Count; i++) {
                string type = types[i]; // atomically captured
                if (REMOVED_TYPE.Equals(type) && skipRemoved)
                    continue;
                InstrumentProfile ip = profiles[i];
                HashSet<InstrumentProfileField> enumFormat = enumFormats[type];
                HashSet<string> customFormat = customFormats[type];
                if (enumFormat == null) {
                    updated.Add(type);
                    enumFormat = new HashSet<InstrumentProfileField>();
                    enumFormat.Add(InstrumentProfileField.SYMBOL);
                    // always write symbol (type is always written by a special code)
                    enumFormats.Add(type, enumFormat);
                    customFormats.Add(type, customFormat = new HashSet<string>());
                }
                if (!REMOVED_TYPE.Equals(type)) {
                    // collect actual used fields for non-removed instrument profiles
                    foreach (InstrumentProfileField ipf in FIELDS)
                        if (ipf != InstrumentProfileField.TYPE && ipf.GetField(ip).Length > 0)
                            if (enumFormat.Add(ipf))
                                updated.Add(type);
                    if (ip.AddNonEmptyCustomFieldNames(customFormat))
                        updated.Add(type);
                }
            }
            foreach (string type in updated)
                writeFormat(type);
        }

        private void writeFormat(string type) {
            writer.writeField(Constants.METADATA_PREFIX + type + Constants.METADATA_SUFFIX);
            foreach (InstrumentProfileField field in enumFormats[type])
                writer.writeField(field.Name);
            foreach (string field in customFormats[type])
                writer.writeField(field);
            writer.writeRecord(null);
        }

        private void writeProfiles(List<InstrumentProfile> profiles, bool skipRemoved) {
            for (int i = 0; i < profiles.Count; i++) {
                string type = types[i]; // atomically captured
                if (REMOVED_TYPE.Equals(type) && skipRemoved)
                    continue;
                InstrumentProfile ip = profiles[i];
                writeProfile(type, ip);
            }
        }

        private void writeProfile(string type, InstrumentProfile ip) {
            writer.writeField(type);
            foreach (InstrumentProfileField field in enumFormats[type])
                writer.writeField(field.GetField(ip));
            foreach (string field in customFormats[type])
                writer.writeField(ip.GetField(field));
            writer.writeRecord(null);
        }
    }
}
