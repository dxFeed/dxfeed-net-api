using System;
using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native {
	public class NativeConnection : IDxConnection {
		private IntPtr handler = IntPtr.Zero;

		internal IntPtr Handler {
			get { return handler; }
		}

		#region Implementation of IDxConnection

		public void Connect(string address) {
			if (handler != IntPtr.Zero)
				throw new NativeDxException("Already connected");

			C.CheckOk(C.dxf_create_connection(address, null, null, null, IntPtr.Zero, out handler));
		}

		public void Disconnect() {
			if (handler == IntPtr.Zero)
				return;

			C.CheckOk(C.dxf_close_connection(handler));
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