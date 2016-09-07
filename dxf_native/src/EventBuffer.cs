using System.Collections;
using System.Collections.Generic;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native {
    class EventBuffer<T> : IDxEventBuf<T> {
        private readonly EventType type;
        private readonly DxString symbol;
        private readonly EventParams eventParams;
        private List<T> events = new List<T>();

        internal EventBuffer(EventType type, DxString symbol, EventParams eventParams) {
            this.type = type;
            this.symbol = symbol;
            this.eventParams = eventParams;
        }

        internal void AddEvent(T _event) {
            events.Add(_event);
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
