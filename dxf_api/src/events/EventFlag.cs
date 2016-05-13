using System;

namespace com.dxfeed.api.events {
	[Flags]
	public enum EventFlag : int {
		TxPending = 0x01,
		RemoveEvent = 0x02,
		SnapshotBegin = 0x04,
		SnapshotEnd = 0x08,
		SnapshotSnip = 0x10,
		RemoveSymbol = 0x20
	}
}