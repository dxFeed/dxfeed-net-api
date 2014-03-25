using com.dxfeed.api.events;

namespace com.dxfeed.api {
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

		void OnFundamental<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxSummary;

		void OnTimeAndSale<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxTimeAndSale;

	}

}