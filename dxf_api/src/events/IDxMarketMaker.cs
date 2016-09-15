namespace com.dxfeed.api.events
{
    public interface IDxMarketMaker
    {
        char Exchange { get; }
        int Id { get; }
        double BidPrice { get; }
        int BidSize { get; }
        double AskPrice { get; }
        int AskSize { get; }
    }
}
