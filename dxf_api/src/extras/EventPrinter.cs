#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api.extras
{
    /// <summary>
    /// The simple event printer class
    /// </summary>
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

        /// <summary>
        /// Prints the Quote event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
            foreach (var q in buf)
                Console.WriteLine($"{buf.Symbol} {q}");
        }

        /// <summary>
        /// Prints the Trade event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {
            foreach (var t in buf)
                Console.WriteLine($"{buf.Symbol} {t}");
        }

        /// <summary>
        /// Prints the Order event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            foreach (var o in buf)
                Console.WriteLine($"{buf.Symbol} {o}");
        }

        /// <summary>
        /// Prints the Profile event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {
            foreach (var p in buf)
                Console.WriteLine($"{buf.Symbol} {p}");
        }

        /// <summary>
        /// Prints the Summary event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnSummary<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {
            foreach (var f in buf)
                Console.WriteLine($"{buf.Symbol} {f}");
        }

        /// <summary>
        /// Prints the TimeAndSale event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            foreach (var ts in buf)
                Console.WriteLine($"{buf.Symbol} {ts}");
        }

        #endregion

        #region Implementation of IDxTradeEthListener

        /// <summary>
        /// Prints the TradeETH event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnTradeETH<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeETH
        {
            foreach (var te in buf)
                Console.WriteLine($"{buf.Symbol} {te}");
        }

        #endregion

        #region Implementation of IDxSpreadOrderListener

        /// <summary>
        /// Prints the SpreadOrder event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            foreach (var so in buf)
                Console.WriteLine($"{buf.Symbol} {so}");
        }

        #endregion

        #region Implementation of IDxCandleListener

        /// <summary>
        /// Prints the Candle event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            foreach (var c in buf)
                Console.WriteLine($"{buf.Symbol} {c}");
        }

        #endregion

        #region Implementation of IDxGreeksListener

        /// <summary>
        /// Prints the Greeks event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            foreach (var g in buf)
                Console.WriteLine($"{buf.Symbol} {g}");
        }

        #endregion

        #region Implementation of IDxTheoPriceListener

        /// <summary>
        /// Prints the TheoPrice event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
            foreach (var tp in buf)
                Console.WriteLine($"{buf.Symbol} {tp}");
        }

        #endregion

        #region Implementation of IDxUnderlyingListener

        /// <summary>
        /// Prints the Underlying event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnUnderlying<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxUnderlying
        {
            foreach (var u in buf)
                Console.WriteLine($"{buf.Symbol} {u}");
        }

        #endregion

        #region Implementation of IDxSeriesListener

        /// <summary>
        /// Prints the Series event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            foreach (var s in buf)
                Console.WriteLine($"{buf.Symbol} {s}");
        }

        #endregion

        #region Implementation of IDxConfigurationListener

        /// <summary>
        /// Prints the Configuration event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
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
