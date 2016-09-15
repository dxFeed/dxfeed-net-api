using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    public struct NativeMarketMaker : IDxMarketMaker
    {
        private readonly DxMarketMaker mm;

        internal unsafe NativeMarketMaker(DxMarketMaker* mm)
        {
            this.mm = *mm;
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

        #endregion
    }
}