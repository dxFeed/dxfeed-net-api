#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using com.dxfeed.io;

namespace com.dxfeed.ipf.impl
{
    /// <summary>
    /// Composer for Instrument Profile Simple File Format.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    class InstrumentProfileComposer : IDisposable
    {
        private static readonly InstrumentProfileField[] fields = InstrumentProfileField.Values;
        private static readonly string REMOVED_TYPE = InstrumentProfileType.REMOVED.Name;

        private Dictionary<string, HashSet<InstrumentProfileField>> enumFormats = new Dictionary<string, HashSet<InstrumentProfileField>>();
        private Dictionary<string, HashSet<string>> customFormats = new Dictionary<string, HashSet<string>>();
        private List<string> types = new List<string>(); // atomically captures types
        private CSVWriter writer;

        /// <summary>
        /// Creates a new instrument profile composer.
        /// </summary>
        /// <param name="outStream"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public InstrumentProfileComposer(Stream outStream)
        {
            writer = new CSVWriter(new StreamWriter(outStream, System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// Dispose object.
        /// </summary>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void Dispose()
        {
            writer.Dispose();
        }

        /// <summary>
        /// Atomically captures profile types to work correctly when profile type is being changed concurrently,
        /// otherwise, the method is not thread-safe.
        /// </summary>
        /// <param name="profiles">List of instrument profiles.</param>
        /// <param name="skipRemoved">When skipRemoved == true, it ignores removed instruments when composing.</param>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        /// <exception cref="System.InvalidOperationException">Can't format certain profile.</exception>
        public void Compose(IList<InstrumentProfile> profiles, bool skipRemoved)
        {
            CaptureTypes(profiles);
            WriteFormats(profiles, skipRemoved);
            WriteProfiles(profiles, skipRemoved);
            types.Clear();
        }

        /// <summary>
        /// Writes a new line.
        /// </summary>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void ComposeNewLine()
        {
            writer.WriteRecord(new string[] { "" }); // Force CRLF
            writer.Flush();
        }

        /// <summary>
        /// Writes FLUSH command
        /// </summary>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void ComposeFlush()
        {
            writer.WriteRecord(new string[] { Constants.FLUSH_COMMAND });
            ComposeNewLine();
        }

        /// <summary>
        /// Writes COMPLETE command.
        /// </summary>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        public void ComposeComplete()
        {
            writer.WriteRecord(new string[] { Constants.COMPLETE_COMMAND });
            ComposeNewLine();
        }

        private void CaptureTypes(IList<InstrumentProfile> profiles)
        {
            types.Clear();
            foreach (InstrumentProfile ip in profiles)
                types.Add(ip.GetTypeName());
        }

        /// <summary>
        /// Writes formats from list.
        /// </summary>
        /// <param name="profiles"></param>
        /// <param name="skipRemoved"></param>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        /// <exception cref="System.InvalidOperationException">Can't format profile field.</exception>
        private void WriteFormats(IList<InstrumentProfile> profiles, bool skipRemoved)
        {
            HashSet<string> updated = new HashSet<string>();
            for (int i = 0; i < profiles.Count; i++)
            {
                string type = types[i]; // atomically captured
                if (REMOVED_TYPE.Equals(type) && skipRemoved)
                    continue;
                InstrumentProfile ip = profiles[i];
                HashSet<InstrumentProfileField> enumFormat;
                HashSet<string> customFormat;
                customFormats.TryGetValue(type, out customFormat);
                if (!enumFormats.TryGetValue(type, out enumFormat))
                {
                    updated.Add(type);
                    enumFormat = new HashSet<InstrumentProfileField>();
                    enumFormat.Add(InstrumentProfileField.SYMBOL);
                    // always write symbol (type is always written by a special code)
                    enumFormats[type] = enumFormat;
                    customFormat = new HashSet<string>();
                    customFormats[type] = customFormat;
                }
                if (!REMOVED_TYPE.Equals(type))
                {
                    // collect actual used fields for non-removed instrument profiles
                    foreach (InstrumentProfileField ipf in fields)
                        if (ipf != InstrumentProfileField.TYPE && ipf.GetField(ip).Length > 0)
                            if (enumFormat.Add(ipf))
                                updated.Add(type);
                    if (ip.AddNonEmptyCustomFieldNames(customFormat))
                        updated.Add(type);
                }
            }
            foreach (string type in updated)
                WriteFormat(type);
        }

        /// <summary>
        /// Writes format.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="System.ArgumentException">If attempt to write record without fields was made.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        private void WriteFormat(string type)
        {
            writer.WriteField(Constants.METADATA_PREFIX + type + Constants.METADATA_SUFFIX);
            foreach (InstrumentProfileField field in enumFormats[type])
                writer.WriteField(field.Name);
            foreach (string field in customFormats[type])
                writer.WriteField(field);
            writer.WriteRecord(null);
        }

        /// <summary>
        /// Writes profiles from list.
        /// </summary>
        /// <param name="profiles"></param>
        /// <param name="skipRemoved"></param>
        /// <exception cref="System.InvalidOperationException">Can't format certain profile.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        private void WriteProfiles(IList<InstrumentProfile> profiles, bool skipRemoved)
        {
            for (int i = 0; i < profiles.Count; i++)
            {
                string type = types[i]; // atomically captured
                if (REMOVED_TYPE.Equals(type) && skipRemoved)
                    continue;
                InstrumentProfile ip = profiles[i];
                WriteProfile(type, ip);
            }
        }

        /// <summary>
        /// Write profile.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ip"></param>
        /// <exception cref="System.InvalidOperationException">Can't format certain profile.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        private void WriteProfile(string type, InstrumentProfile ip)
        {
            writer.WriteField(type);
            foreach (InstrumentProfileField field in enumFormats[type])
                writer.WriteField(field.GetField(ip));
            foreach (string field in customFormats[type])
                writer.WriteField(ip.GetField(field));
            writer.WriteRecord(null);
        }
    }
}
