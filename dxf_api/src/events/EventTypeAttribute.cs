using System;

namespace com.dxfeed.api.events {
    public class EventTypeAttribute : Attribute {
        public EventTypeAttribute(string eventName) : base() {
            EventName = eventName;
        }

        public string EventName { get; private set; }

    }
}
