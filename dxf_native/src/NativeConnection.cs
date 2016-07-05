﻿using System;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native {
    /// <summary>
    /// Class provides operation with server
    /// </summary>
	public class NativeConnection : IDxConnection {
		private IntPtr handler = IntPtr.Zero;
		private readonly C.dxf_conn_termination_notifier_t callback;
		private readonly Action<IDxConnection> disconnectListener;

		internal IntPtr Handler {
			get { return handler; }
		}

        /// <summary>
        /// Creates new connection
        /// </summary>
        /// <param name="address">server address to connect</param>
        /// <param name="disconnectListener">listener will be called when the connection is interrupted</param>
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

        /// <summary>
        /// Disconnect from the server
        /// </summary>
		public void Disconnect() {
			if (handler == IntPtr.Zero)
				return;

			C.CheckOk(C.Instance.dxf_close_connection(handler));
			handler = IntPtr.Zero;
		}

        /// <summary>
        /// Create event subscription
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="listener">event listener callback</param>
        /// <returns></returns>
		public IDxSubscription CreateSubscription(EventType type, IDxFeedListener listener) {
			if (handler == IntPtr.Zero)
				throw new NativeDxException("not connected");

			return new NativeSubscription(this, type, listener);
		}

        /// <summary>
        /// Create candle event listener
        /// </summary>
        /// <param name="time">date time ini the past</param>
        /// <param name="listener">candle listener callback</param>
        /// <returns></returns>
        public IDxSubscription CreateSubscription(DateTime? time, IDxCandleListener listener) {
            if (handler == IntPtr.Zero)
                throw new NativeDxException("not connected");

            return new NativeSubscription(this, time, listener);
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