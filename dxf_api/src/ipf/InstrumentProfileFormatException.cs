﻿#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.IO;

namespace com.dxfeed.ipf
{
    /// <summary>
    /// Signals that input stream does not conform to the Simple File Format.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    public class InstrumentProfileFormatException : IOException
    {
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
