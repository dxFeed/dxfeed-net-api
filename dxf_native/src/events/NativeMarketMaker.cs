using com.dxfeed.api.events;
using com.dxfeed.native.api;
using System.Globalization;

namespace com.dxfeed.native.events
{
    public struct NativeMarketMaker : IDxMarketMaker
    {
        private readonly DxMarketMaker mm;
        private readonly string symbol;

        internal unsafe NativeMarketMaker(DxMarketMaker* mm, string symbol)
        {
            this.mm = *mm;
            this.symbol = symbol;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "MarketMaker {{{6}, " +
                "Exchange: '{0}', Id: {1}, BidPrice: {2}, BidSize: {3}, AskPrice: {4}, " +
                "AskSize: {5}}}",
                Exchange, Id, BidPrice, BidSize, AskPrice, AskSize, Symbol);
        }

        #region Implementation of IDxMarketMaker

        public char Exchange
        {
            get { return mm.mm_exchange; }
        }

        public int Id
        {
            get { return mm.mm_id; }
        }

        public double BidPrice
        {
            get { return mm.mmbid_price; }
        }

        public int BidSize
        {
            get { return mm.mmbid_size; }
        }

        public double AskPrice
        {
            get { return mm.mmask_price; }
        }

        public int AskSize
        {
            get { return mm.mmask_size; }
        }

        public string Symbol
        {
            get { return symbol; }
        }

        #endregion
    }
}