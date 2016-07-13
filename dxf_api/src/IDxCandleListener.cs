using com.dxfeed.api.events;

namespace com.dxfeed.api {

    /// <summary>
    /// Interface provides receiving candle events
    /// </summary>
    public interface IDxCandleListener {

        /// <summary>
        /// On Candle events received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle;
    }
}
