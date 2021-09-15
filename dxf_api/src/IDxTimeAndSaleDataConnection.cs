#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;

namespace com.dxfeed.api
{
    /// <summary>
    /// Interface provides operations with candle data retrieving
    /// </summary>
    public interface IDxTimeAndSaleDataConnection
    {
        /// <summary>
        /// Asynchronously returns a "snapshot" of TnS data for the specified symbols and period.
        /// </summary>
        /// <param name="symbols">
        /// The TnS symbols (Example: composite: AAPL, regional: AAPL&amp;A ... AAPL&amp;Z)
        /// It is possible to subscribe to regional TnS by specifying, for example, the AAPL&amp;Q symbol. 
        /// In this case, the service will return composite (Scope = Composite !) TnS with the TimeAndSale (not TimeAndSale&amp;Q) record type and the AAPL&amp;Q symbol (like a candle) 
        /// </param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task for the result of the request.</returns>
        Task<Dictionary<string, List<IDxTimeAndSale>>> GetTimeAndSaleData(List<string> symbols, DateTime fromTime, DateTime toTime,
            CancellationToken cancellationToken);
    }
}