using System;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_candle_sample {
    /// <summary>
    /// Candle event listener
    /// </summary>
    public class EventListener : IDxCandleListener {

        #region Implementation of IDxCandleListener

        /// <summary>
        /// On Candle events received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle {

            foreach (var c in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, c));
        }

        #endregion
    }
}