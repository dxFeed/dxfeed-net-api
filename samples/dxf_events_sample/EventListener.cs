/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_events_sample
{
    /// <summary>
    /// Events listener
    /// </summary>
    public class EventListener : 
        IDxFeedListener, 
        IDxTradeEthListener, 
        IDxSpreadOrderListener,
        IDxGreeksListener,
        IDxTheoPriceListener,
        IDxUnderlyingListener,
        IDxSeriesListener,
        IDxConfigurationListener
    {
        #region Implementation of IDxFeedListener

        /// <summary>
        /// On Quote event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {

            foreach (var q in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, q));
        }

        /// <summary>
        /// On Trade event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {

            foreach (var t in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, t));
        }

        /// <summary>
        /// On Order event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {

            foreach (var o in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, o));
        }

        /// <summary>
        /// On Profile event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {

            foreach (var p in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, p));
        }

        /// <summary>
        /// On Fundamental event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {

            foreach (var f in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, f));
        }

        /// <summary>
        /// On Time and Sale event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {

            foreach (var ts in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, ts));
        }

        #endregion

        #region Implementation of IDxTradeEthListener

        /// <summary>
        /// On TradeETH event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnTradeEth<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeEth
        {
            foreach (var te in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, te));
        }

        #endregion

        #region Implementation of IDxSpreadOrderListener

        /// <summary>
        /// On SpreadOrder event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            foreach (var so in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, so));
        }

        #endregion

        #region Implementation of IDxGreeksListener

        /// <summary>
        /// On Greeks event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            foreach (var g in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, g));
        }

        #endregion

        #region Implementation of IDxTheoPriceListener

        /// <summary>
        /// On TheoPrice event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
            foreach (var tp in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, tp));
        }

        #endregion

        #region Implementation of IDxUnderlyingListener

        /// <summary>
        /// On Underlying event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnUnderlying<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxUnderlying
        {
            foreach (var u in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, u));
        }

        #endregion

        #region Implementation of IDxSeriesListener

        /// <summary>
        /// On Series event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            foreach (var s in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, s));
        }

        #endregion

        #region Implementation of IDxConfigurationListener

        public void OnConfiguration<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxConfiguration
        {
            foreach (var s in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, s));
        }

        #endregion
    }
}
