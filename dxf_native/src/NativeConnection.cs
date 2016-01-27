using System;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native {
	public class NativeConnection : IDxConnection {
		private IntPtr handler = IntPtr.Zero;
		private readonly C.dxf_conn_termination_notifier_t callback;
		private readonly Action<IDxConnection> disconnectListener;

		internal IntPtr Handler {
			get { return handler; }
		}

		public NativeConnection(String address, Action<IDxConnection> disconnectListener) {
			callback = OnDisconnect;
			this.disconnectListener = disconnectListener;
			C.CheckOk(C.Instance.dxf_create_connection(address, callback, null, null, IntPtr.Zero, out handler));
		}

		private void OnDisconnect(IntPtr connection, IntPtr userData) {
			if (disconnectListener != null)
				disconnectListener(this);
		}

		#region Implementation of IDxConnection

		public void Disconnect() {
			if (handler == IntPtr.Zero)
				return;

			C.CheckOk(C.Instance.dxf_close_connection(handler));
			handler = IntPtr.Zero;
		}

		public IDxSubscription CreateSubscription(EventType type, IDxFeedListener listener) {
			if (handler == IntPtr.Zero)
				throw new NativeDxException("not connected");

			return new NativeSubscription(this, type, listener);
		}

		#endregion

		#region Implementation of IDisposable

		public void Dispose() {
			if (handler != IntPtr.Zero)
				Disconnect();
		}

		#endregion
	}
}