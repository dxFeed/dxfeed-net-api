using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api.extras
{
    public class EventPrinter : IDxFeedListener, IDxCandleListener
    {
        #region Implementation of IDxFeedListener

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
            foreach (var q in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, q));
        }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {
            foreach (var t in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, t));
        }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            foreach (var o in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, o));
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {
            foreach (var p in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, p));
        }

        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {
            foreach (var f in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, f));
        }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            foreach (var ts in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, ts));
        }

        #endregion

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
