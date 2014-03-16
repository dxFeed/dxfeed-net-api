using System;
using System.Runtime.Serialization;

namespace com.dxfeed.api {
	public class DxException : Exception {
		public DxException() {}
		public DxException(string message) : base(message) {}
		public DxException(string message, Exception innerException) : base(message, innerException) {}
		protected DxException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}