using System.Collections.Generic;

namespace com.dxfeed.api {

    /// <summary>
    /// The listener interface for receiving events of the specified type E.
    /// </summary>
    /// <typeparam name="E">The type of events.</typeparam>
    public interface DXFeedEventListener<E> {

        /// <summary>
        /// Invoked when events of type E are received.
        /// </summary>
        /// <param name="events">The list of received events.</param>
        void EventsReceived(IList<E> events);

    }
}
