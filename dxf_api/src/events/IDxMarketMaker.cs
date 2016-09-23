namespace com.dxfeed.api.events
{
    [EventTypeAttribute("MarketMaker")]
    public interface IDxMarketMaker : IDxMarketEvent
    {
        char Exchange { get; }
        int Id { get; }
        double BidPrice { get; }
        int BidSize { get; }
        double AskPrice { get; }
        int AskSize { get; }
    }
}
