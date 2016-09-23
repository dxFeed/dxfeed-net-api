using System.Globalization;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    public struct NativeSummary : IDxSummary
    {
        private readonly DxSummary summary;
        private readonly string symbol;

        internal unsafe NativeSummary(DxSummary* summary, string symbol)
        {
            this.summary = *summary;
            this.symbol = symbol;
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Summary: {{{10}, DayId: {0}, DayOpenPrice: {1}, DayHighPrice: {2}, DayLowPrice: {3}, " +
                "DayClosePrice: {4}, PrevDayId: {5}, PrevDayClosePrice: {6}, OpenInterest: {7}, " +
                "Flags: {8}, ExchangeCode: {9} }}",
                DayId, DayOpenPrice, DayHighPrice, DayLowPrice,
                DayClosePrice, PrevDayId, PrevDayClosePrice, OpenInterest,
                Flags, ExchangeCode, Symbol);
        }

        #region Implementation of IDxSummary

        public int DayId
        {
            get { return summary.day_id; }
        }

        public double DayOpenPrice
        {
            get { return summary.day_open_price; }
        }

        public double DayHighPrice
        {
            get { return summary.day_high_price; }
        }

        public double DayLowPrice
        {
            get { return summary.day_low_price; }
        }

        public double DayClosePrice
        {
            get { return summary.day_close_price; }
        }

        public int PrevDayId
        {
            get { return summary.prev_day_id; }
        }

        public double PrevDayClosePrice
        {
            get { return summary.prev_day_close_price; }
        }

        public long OpenInterest
        {
            get { return summary.open_interest; }
        }

        public long Flags
        {
            get { return summary.flags; }
        }

        public char ExchangeCode
        {
            get { return summary.exchange_code; }
        }

        public string Symbol
        {
            get { return symbol; }
        }

        #endregion
    }
}