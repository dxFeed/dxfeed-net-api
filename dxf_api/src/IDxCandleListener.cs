using com.dxfeed.api.events;

namespace com.dxfeed.api {
    public interface IDxCandleListener
    {
        void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle;
    }
}
