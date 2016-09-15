/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.Serialization;

namespace com.dxfeed.api
{
    public class DxException : Exception
    {
        public DxException() { }
        public DxException(string message) : base(message) { }
        public DxException(string message, Exception innerException) : base(message, innerException) { }
        protected DxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
