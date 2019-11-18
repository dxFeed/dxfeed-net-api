﻿#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.IO;

namespace com.dxfeed.io
{
    /// <summary>
    /// Signals that input stream does not conform to the CSV format.
    /// See <a href="http://www.rfc-editor.org/rfc/rfc4180.txt">RFC 4180</a> for CSV format specification.
    /// </summary>
    public class CSVFormatException : IOException
    {
        /// <summary>
        /// Constructs a CSVFormatException without detail message.
        /// </summary>
        public CSVFormatException() : base() { }

        /// <summary>
        /// Constructs a CSVFormatException with specified detail message.
        /// </summary>
        /// <param name="s"></param>
        public CSVFormatException(string s) : base(s) { }
    }
}
