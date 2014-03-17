using System;

namespace com.dxfeed.api {
	public interface IDxConnection : IDisposable {
		void Connect(string address);
		void Disconnect();
		IDxSubscription CreateSubscription(EventType type, IDxFeedListener listener);
	}
}