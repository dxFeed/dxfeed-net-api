using System;

namespace com.dxfeed.api.events
{
    public class EventParams
    {
        public EventParams(EventFlag flags, UInt64 timeIntField, UInt64 snapshotKey)
        {
            this.Flags = flags;
            this.TimeIntField = timeIntField;
            this.SnapshotKey = snapshotKey;
        }

        public EventFlag Flags { get; private set; }

        public UInt64 TimeIntField { get; private set; }

        public UInt64 SnapshotKey { get; private set; }
    }
}
