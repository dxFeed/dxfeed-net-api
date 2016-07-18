using System;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_candle_sample
{
    public class EventListener : IDxCandleListener
    {

        #region Implementation of IDxCandleListener

        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            foreach (var c in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, c));
        }

        #endregion
    }
}