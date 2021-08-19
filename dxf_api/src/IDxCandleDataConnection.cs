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
    public interface IDxCandleDataConnection : IDisposable
    {
        /// <summary>
        /// Asynchronously returns a "snapshot" of candle data for the specified symbols and period.
        /// </summary>
        /// <param name="symbols">The candle symbols</param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task for the result of the request.</returns>
        Task<List<IDxCandle>> GetCandleData(CandleSymbol[] symbols, DateTime fromTime, DateTime toTime,
            CancellationToken cancellationToken);
    }
}