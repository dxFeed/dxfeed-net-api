using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.dxfeed.api;

namespace com.dxfeed.native {
    public class DXFeed : IDisposable {

        private static readonly string DEFAULT_ADDRESS = "demo.dxfeed.com:7300";
        private static readonly string DEFAULT_USER = "demo";
        private static readonly string DEFAULT_PASSWORD = "demo";

        private static DXFeed dxFeedInstance = null;

        private IDxConnection connectionInstance = null;
        private string address = string.Empty;

        private DXFeed(string address) {
            this.address = address;
            connectionInstance = new NativeConnection(address, OnDisconnect);
        }

        /// <summary>
        /// Returns a default application-wide singleton instance of feed. Most applications use only a single
        /// data-source and should rely on this method to get one.
        /// </summary>
        /// <returns>Singleton instance of feed.</returns>
        public static DXFeed GetInstance() {
            if (dxFeedInstance == null)
                dxFeedInstance = new DXFeed(DEFAULT_ADDRESS);
            return dxFeedInstance;
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
            DXFeedSubscription<E> subscription = new DXFeedSubscription<E>(connectionInstance);
            AttachSubscription(subscription);
            return subscription;
        }

        /// <summary>
        /// Creates new subscription for multiple event types that is <i>attached</i> to this feed.
        /// For a single event type use CreateSubscription<E>().
        /// This method creates new DXFeedSubscription and invokes AttachSubscription.
        /// </summary>
        /// <typeparam name="E">The type of events.</typeparam>
        /// <param name="eventTypes">The classes of event types.</param>
        /// <returns>The new DXFeedSubscription.</returns>
        public DXFeedSubscription<E> CreateSubscription<E>(params Type[] eventTypes) {
            DXFeedSubscription<E> subscription = new DXFeedSubscription<E>(eventTypes);
            AttachSubscription(subscription);
            return subscription;
        }

        public void AttachSubscription<E>(DXFeedSubscription<E> subscription) {
            //TODO:
        }

        private static void OnDisconnect(IDxConnection con) {
            //TODO: todo
        }

        public void Dispose() {
            if (connectionInstance != null)
                connectionInstance.Dispose();
        }
    }
}
