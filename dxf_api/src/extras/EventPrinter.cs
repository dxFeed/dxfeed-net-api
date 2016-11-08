/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api.extras
{
    public class EventPrinter : 
        IDxFeedListener, 
        IDxTradeEthListener,
        IDxSpreadOrderListener,
        IDxCandleListener
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

        #region Implementation of IDxTradeEthListener

        public void OnTradeEth<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeEth
        {
            foreach (var te in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, te));
        }

        #endregion

        #region Implementation of IDxSpreadOrderListener

        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            foreach (var so in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, so));
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
