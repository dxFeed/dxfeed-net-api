#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api.extras
{
    public class EventPrinter :
        IDxFeedListener,
        IDxTradeETHListener,
        IDxSpreadOrderListener,
        IDxCandleListener,
        IDxGreeksListener,
        IDxTheoPriceListener,
        IDxUnderlyingListener,
        IDxSeriesListener,
        IDxConfigurationListener
    {
        #region Implementation of IDxFeedListener

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
            foreach (var q in buf)
                Console.WriteLine($"{buf.Symbol} {q}");
        }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {
            foreach (var t in buf)
                Console.WriteLine($"{buf.Symbol} {t}");
        }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            foreach (var o in buf)
                Console.WriteLine($"{buf.Symbol} {o}");
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {
            foreach (var p in buf)
                Console.WriteLine($"{buf.Symbol} {p}");
        }

        public void OnSummary<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {
            foreach (var f in buf)
                Console.WriteLine($"{buf.Symbol} {f}");
        }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            foreach (var ts in buf)
                Console.WriteLine($"{buf.Symbol} {ts}");
        }

        #endregion

        #region Implementation of IDxTradeEthListener

        public void OnTradeETH<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeETH
        {
            foreach (var te in buf)
                Console.WriteLine($"{buf.Symbol} {te}");
        }

        #endregion

        #region Implementation of IDxSpreadOrderListener

        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            foreach (var so in buf)
                Console.WriteLine($"{buf.Symbol} {so}");
        }

        #endregion

        #region Implementation of IDxCandleListener

        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            foreach (var c in buf)
                Console.WriteLine($"{buf.Symbol} {c}");
        }

        #endregion

        #region Implementation of IDxGreeksListener

        public void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            foreach (var g in buf)
                Console.WriteLine($"{buf.Symbol} {g}");
        }

        #endregion

        #region Implementation of IDxTheoPriceListener

        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
            foreach (var tp in buf)
                Console.WriteLine($"{buf.Symbol} {tp}");
        }

        #endregion

        #region Implementation of IDxUnderlyingListener

        public void OnUnderlying<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxUnderlying
        {
            foreach (var u in buf)
                Console.WriteLine($"{buf.Symbol} {u}");
        }

        #endregion

        #region Implementation of IDxSeriesListener

        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            foreach (var s in buf)
                Console.WriteLine($"{buf.Symbol} {s}");
        }

        #endregion

        #region Implementation of IDxConfigurationListener

        public void OnConfiguration<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxConfiguration
        {
            foreach (var s in buf)
                Console.WriteLine($"{buf.Symbol} {s}");
        }

        #endregion
    }
}
