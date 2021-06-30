#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Events flag enumeration
    ///
    /// [EventFlags description](https://kb.dxfeed.com/display/DS/QD+Model+of+Market+Events#QDModelofMarketEvents-EventFlagsfield) or <see cref="com.dxfeed.api.events.IDxIndexedEvent"/>
    /// </summary>
    [Flags]
    public enum EventFlag
    {
        /// <summary>
        /// (0x01) TX_PENDING indicates a pending transactional update. When TX_PENDING is 1, it means that an ongoing transaction
        /// update, that spans multiple events, is in process
        /// </summary>
        TxPending     = 0x01,
        
        /// <summary>
        /// (0x02) REMOVE_EVENT indicates that the event with the corresponding index has to be removed
        /// </summary>
        RemoveEvent   = 0x02,
        
        /// <summary>
        /// (0x04) SNAPSHOT_BEGIN indicates when the loading of a snapshot starts. Snapshot load starts on new subscription and
        /// the first indexed event that arrives for each exchange code (in the case of a regional record) on a new
        /// subscription may have SNAPSHOT_BEGIN set to true. It means that an ongoing snapshot consisting of multiple
        /// events is incoming
        /// </summary>
        SnapshotBegin = 0x04,
        
        /// <summary>
        /// (0x08) SNAPSHOT_END or (0x10) SNAPSHOT_SNIP indicates the end of a snapshot. The difference between SNAPSHOT_END and
        /// SNAPSHOT_SNIP is the following: SNAPSHOT_END indicates that the data source sent all the data pertaining to
        /// the subscription for the corresponding indexed event, while SNAPSHOT_SNIP indicates that some limit on the
        /// amount of data was reached and while there still might be more data available, it will not be provided
        /// </summary>
        SnapshotEnd   = 0x08,

        /// <summary>
        /// (0x08) SNAPSHOT_END or (0x10) SNAPSHOT_SNIP indicates the end of a snapshot. The difference between SNAPSHOT_END and
        /// SNAPSHOT_SNIP is the following: SNAPSHOT_END indicates that the data source sent all the data pertaining to
        /// the subscription for the corresponding indexed event, while SNAPSHOT_SNIP indicates that some limit on the
        /// amount of data was reached and while there still might be more data available, it will not be provided
        /// </summary>
        SnapshotSnip  = 0x10,
        RemoveSymbol  = 0x20
    }
}
