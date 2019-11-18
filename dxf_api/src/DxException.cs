﻿#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

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
