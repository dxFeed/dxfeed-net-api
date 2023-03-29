#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Received event parameters
    /// </summary>
    public class EventParams
    {
        /// <summary>
        /// Create new event parameters object
        /// </summary>
        /// <param name="flags">event flags</param>
        /// <param name="timeIntField">event time field</param>
        /// <param name="snapshotKey">Number represents snapshot key</param>
        public EventParams(EventFlag flags, UInt64 timeIntField, UInt64 snapshotKey)
        {
            this.Flags = flags;
            this.TimeIntField = timeIntField;
            this.SnapshotKey = snapshotKey;
        }

        /// <summary>
        /// Event flags
        /// </summary>
        public EventFlag Flags { get; private set; }

        /// <summary>
        /// Event time field
        /// </summary>
        public UInt64 TimeIntField { get; private set; }

        /// <summary>
        /// The 64 bit unsigned decimal representing unique id in subscription
        /// for snapshot object. This value is generated in C API using
        /// record_info_id, symbol and source.
        ///
        /// Snapshot key format:
        /// 64 - 56     55 - 24      23 - 0
        /// rec_inf_id | symbol | order_source
        ///
        /// rec_inf_id - record type of snapshot subscription (record_info_id).
        /// symbol - string symbol of snapshot subscription.
        /// order_source - source for Order records or keyword for MarketMaker;
        ///                can be NULL also.
        /// </summary>
        public UInt64 SnapshotKey { get; private set; }
    }
}
