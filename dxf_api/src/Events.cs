using System;
using System.Collections.Generic;

namespace com.dxfeed.api {

	[Flags]
	public enum EventType : int {
		None = 0,
		Trade = 1,
		Quote = 2,
		Summary = 4,
		Profile = 8,
		Order = 16,
		TimeAndSale = 32
	}

	public interface IDxEventBuf<out T> : IEnumerable<T> {
		EventType EventType { get; }
		DxString Symbol { get; }
		int Size { get; }
	}

	
	public interface IDxFeedListener {
		
		void OnQuote<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxQuote;

		void OnTrade<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxTrade;

		void OnOrder<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxOrder;

		void OnProfile<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxProfile;

		void OnTimeAndSale<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxTimeAndSale;

	}

}