#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Runtime.InteropServices;
using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native
{
    internal class NativeDxException : DxException
    {
        public int ErrorCode { get; private set; }

        public static NativeDxException Create()
        {
            int error;
            IntPtr desc;
            C.Instance.dxf_get_last_error(out error, out desc);
            var message = Marshal.PtrToStringUni(desc);
            return new NativeDxException(error, message);
        }

        public NativeDxException(int error, string message) : base(message)
        {
            ErrorCode = error;
        }

        public NativeDxException(string message) : base(message) { }
    }
}
