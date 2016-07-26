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

namespace com.dxfeed.ipf {

    /// <summary>
    /// Signals that input stream does not conform to the Simple File Format.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    public class InstrumentProfileFormatException : IOException {
        private static readonly long serialVersionUID = 0;

        /// <summary>
        /// Constructs a InstrumentProfileFormatException without detail message.
        /// </summary>
        public InstrumentProfileFormatException() : base() { }

        /// <summary>
        /// Constructs a InstrumentProfileFormatException with specified detail message.
        /// </summary>
        /// <param name="s"></param>
        public InstrumentProfileFormatException(string s) : base(s) { }
    }
}
