#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Runtime.Serialization;

namespace com.dxfeed.api
{
    /// <summary>
    /// The dxFeed exceptions wrapper
    /// </summary>
    public class DxException : Exception
    {
        /// <summary>
        /// Creates default dxFeed exception
        /// </summary>
        public DxException() { }
        
        /// <summary>
        /// Creates an exception with message
        /// </summary>
        /// <param name="message">The exception message</param>
        public DxException(string message) : base(message) { }
        
        /// <summary>
        /// Creates an exception with message and inherited (inner) exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inherited (inner) exception </param>
        public DxException(string message, Exception innerException) : base(message, innerException) { }
        
        /// <summary>
        /// Creates an exception with serialization info and streaming context
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected DxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
