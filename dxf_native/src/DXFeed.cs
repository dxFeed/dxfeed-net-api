using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.dxfeed.api;

namespace com.dxfeed.native {
    public class DXFeed {

        private string DEFAULT_ADDRESS = "demo.dxfeed.com:7300";
        private string DEFAULT_USER = "demo";
        private string DEFAULT_PASSWORD = "demo";

        //TODO: remains open
        private IDxConnection connection = null;

        private DXFeed() {
            connection = new NativeConnection(DEFAULT_ADDRESS, OnDisconnect);
        }

        /// <summary>
        /// Returns a default application-wide singleton instance of feed. Most applications use only a single
        /// data-source and should rely on this method to get one.
        /// </summary>
        /// <returns>Singleton instance of feed.</returns>
        public static DXFeed GetInstance() {
            return new DXFeed();
        }

        /// <summary>
        /// Creates new subscription for a single event type that is attached to this feed.
        /// For multiple event types in one subscription use
        /// createSubscription(Class... eventTypes)
        /// This method creates new DXFeedSubscription.
        ///
        /// @see DXFeedSubscription#DXFeedSubscription(Class)
        /// @see #attachSubscription(DXFeedSubscription)
        /// </summary>
        /// <typeparam name="E">The type of events.</typeparam>
        /// <param name="eventType">The class of event types.</param>
        /// <returns>New DXFeedSubscription for a single event type.</returns>
        public DXFeedSubscription<E> CreateSubscription<E>() {
            DXFeedSubscription<E> subscription = new DXFeedSubscription<E>();
            AttachSubscription(subscription);
            return subscription;
        }

        public void AttachSubscription<E>(DXFeedSubscription<E> subscription) {

        }

        private static void OnDisconnect(IDxConnection con) {
            //TODO: todo
        }

    }
}
