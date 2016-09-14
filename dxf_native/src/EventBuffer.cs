using System.Collections;
using System.Collections.Generic;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native {
    class EventBuffer<T> : IDxEventBuf<T> {
        private readonly EventType type;
        private readonly DxString symbol;
        private EventParams eventParams;
        private List<T> events = new List<T>();

        internal EventBuffer(EventType type, DxString symbol, EventParams eventParams) {
            this.type = type;
            this.symbol = symbol;
            this.eventParams = eventParams;
        }

        internal void AddEvent(T _event) {
            events.Add(_event);
        }

        internal void Clear() {
            events.Clear();
        }

        internal void ReplaceOrAdd(T _event) {
            if (_event is IDxOrder) {
                int index = events.FindIndex(x => (x as IDxOrder).Index == (_event as IDxOrder).Index);
                if (index == -1) {
                    events.Add(_event);
                } else {
                    events[index] = _event;
                }
            }
        }

        internal void Remove(T _event) {
            if (_event is IDxOrder) {
                int index = events.FindIndex(x => (x as IDxOrder).Index == (_event as IDxOrder).Index);
                if (index != -1) {
                    events.RemoveAt(index);
                }
            }
        }


        #region Implementation of IDxEventBuf<out T>

        public EventType EventType {
            get { return type; }
        }

        public DxString Symbol {
            get { return symbol; }
        }

        public int Size {
            get { return events.Count; }
        }

        public EventParams EventParams {
            get { return eventParams; }
            internal set { eventParams = value; }
        }

        #endregion

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator() {
            return events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return events.GetEnumerator();
        }
        #endregion
    }
}
