using System;

namespace com.dxfeed.api.events
{
    public interface IDxQuote
    {
        DateTime BidTime { get; }
        char BidExchangeCode { get; }
        double BidPrice { get; }
        long BidSize { get; }
        DateTime AskTime { get; }
        char AskExchangeCode { get; }
        double AskPrice { get; }
        long AskSize { get; }
    }
}
