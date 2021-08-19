using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;

namespace com.dxfeed.native
{
    /// <summary>
    /// Class provides operations with candle data retrieving
    /// </summary>
    public class CandleDataConnection : IDxCandleDataConnection
    {
        private string address;
        private NetworkCredential credential;
        
        public CandleDataConnection(string address, NetworkCredential credential)
        {
            this.address = address;
            this.credential = credential;
        }
        
        public CandleDataConnection(string address)
        {
            this.address = address;
            this.credential = null;
        }

        public void Dispose()
        {
        }

        public Task<List<IDxCandle>> GetCandleData(CandleSymbol[] symbols, DateTime fromTime, DateTime toTime, CancellationToken cancellationToken)
        {
            return Task.Run(async ()  =>
            {
                var result = new List<IDxCandle>();

                using (var httpClient = new HttpClient())
                {
                    var builder = new UriBuilder(address);

                    var stream = await httpClient.GetStreamAsync(address);
                }

                return result;
            }, cancellationToken);
        }
    }
}