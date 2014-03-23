using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api {
	public interface IDxConnection : IDisposable {
		void Disconnect();
		IDxSubscription CreateSubscription(EventType type, IDxFeedListener listener);
	}
}