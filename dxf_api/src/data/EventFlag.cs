#region License

/*
Copyright � 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Events flag enumeration
    /// </summary>
    [Flags]
    public enum EventFlag : int
    {
        TxPending     = 0x01,
        RemoveEvent   = 0x02,
        SnapshotBegin = 0x04,
        SnapshotEnd   = 0x08,
        SnapshotSnip  = 0x10,
        RemoveSymbol  = 0x20
    }
}
