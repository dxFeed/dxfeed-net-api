/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_candle_sample
{
    /// <summary>
    /// Candle event listener
    /// </summary>
    public class EventListener : IDxCandleListener
    {
        #region Implementation of IDxCandleListener

        /// <summary>
        /// On Candle events received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
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
