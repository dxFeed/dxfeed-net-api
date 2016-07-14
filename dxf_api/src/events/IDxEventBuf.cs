using System.Collections.Generic;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events {
	public interface IDxEventBuf<out T> : IEnumerable<T> {
		EventType EventType { get; }
		DxString Symbol { get; }
		int Size { get; }
		EventParams EventParams { get; }
	}

}