namespace com.dxfeed.api.events
{
    public interface IDxSummary
    {
        int DayId { get; }
        double DayOpenPrice { get; }
        double DayHighPrice { get; }
        double DayLowPrice { get; }
        double DayClosePrice { get; }
        int PrevDayId { get; }
        double PrevDayClosePrice { get; }
        long OpenInterest { get; }
        long Flags { get; }
        char ExchangeCode { get; }
    }
}
