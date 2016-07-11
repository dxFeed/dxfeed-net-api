using System;

namespace com.dxfeed.api.events {

    /// <summary>
    /// Received event parameters
    /// </summary>
    public class EventParams {

        /// <summary>
        /// Create new event parameters object
        /// </summary>
        /// <param name="flags">event flags</param>
        /// <param name="timeIntField">event time field</param>
        /// <param name="snapshotKey">Number represents snapshot key</param>
        public EventParams(EventFlag flags, UInt64 timeIntField, UInt64 snapshotKey) {
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
        /// Snapshot key
        /// </summary>
        public UInt64 SnapshotKey { get; private set; }
    }
}
