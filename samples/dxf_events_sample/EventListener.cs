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
    public class EventListener : IDxFeedListener
    {
        #region Implementation of IDxFeedListener

        /// <summary>
        /// On Quote event received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {

            foreach (var q in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, q));
        }

        /// <summary>
        /// On Trade event received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {

            foreach (var t in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, t));
        }

        /// <summary>
        /// On Order event received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {

            foreach (var o in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, o));
        }

        /// <summary>
        /// On Profile event received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {

            foreach (var p in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, p));
        }

        /// <summary>
        /// On Fundamental event received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {

            foreach (var f in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, f));
        }

        /// <summary>
        /// On Time and Sale event received
        /// </summary>
        /// <typeparam name="TB">event buffer type</typeparam>
        /// <typeparam name="TE">event type</typeparam>
        /// <param name="buf">event buffer object</param>
        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {

            foreach (var ts in buf)
                Console.WriteLine(string.Format("{0} {1}", buf.Symbol, ts));
        }

        #endregion
    }
}
