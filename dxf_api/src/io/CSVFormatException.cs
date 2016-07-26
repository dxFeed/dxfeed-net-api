/*
 * QDS - Quick Data Signalling Library
 * Copyright (C) 2002-2015 Devexperts LLC
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at
 * http://mozilla.org/MPL/2.0/.
 */
using System;
using System.IO;

namespace com.dxfeed.io {

    /// <summary>
    /// Signals that input stream does not conform to the CSV format.
    /// See <a href="http://www.rfc-editor.org/rfc/rfc4180.txt">RFC 4180</a> for CSV format specification.
    /// </summary>
    public class CSVFormatException : IOException {

        /// <summary>
        /// Constructs a CSVFormatException without detail message.
        /// </summary>
        public CSVFormatException() : base() {
        }

        /// <summary>
        /// Constructs a CSVFormatException with specified detail message.
        /// </summary>
        /// <param name="s"></param>
        public CSVFormatException(String s) : base(s) {
        }
    }
}
