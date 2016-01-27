using System;
using System.Runtime.InteropServices;
using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native {
	internal class NativeDxException : DxException {
		public int ErrorCode { get; private set; }

		public static NativeDxException Create() {
			int error;
			IntPtr desc;
			C.Instance.dxf_get_last_error(out error, out desc);
			var message = Marshal.PtrToStringUni(desc);
			return new NativeDxException(error, message);
		}

		public NativeDxException(int error, string message) : base(message) {
			ErrorCode = error;
		}

		public NativeDxException(string message) : base(message) {}
	}
}