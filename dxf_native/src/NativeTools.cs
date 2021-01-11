#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.native.api;

namespace com.dxfeed.native
{
    public class NativeTools
    {
        public static void InitializeLogging(string fileName, bool overwrite, bool verbose, bool logDataTransfer = false)
        {
            C.CheckOk(C.Instance.dxf_initialize_logger_v2(fileName, overwrite, true, verbose, logDataTransfer));
        }
    }
}
