using System;

namespace com.dxfeed.api.events {
	public interface IDxProfile {
		double Beta { get; }
		double Eps { get; }
		long DivFreq { get; }
		double ExdDivAmount { get; }
		int ExdDivDate { get; }
		double _52HighPrice { get; }
		double _52LowPrice { get; }
		double Shares { get; }
		string Description { get; }
		long Flags { get; }
		string StatusReason { get; }
		DateTime HaltStartTime { get; }
		DateTime HaltEndTime { get; }
		double HighLimitPrice { get; }
		double LowLimitPrice { get; }
	}
}