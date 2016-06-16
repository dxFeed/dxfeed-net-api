using com.dxfeed.api.events;

namespace com.dxfeed.api
{
	public interface IDxSnapshotListener
	{
		void OnOrderSnapshot<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxOrder;

		void OnCandleSnapshot<TB, TE>(TB buf)
			where TB : IDxEventBuf<TE>
			where TE : IDxOrder;
	}
}
