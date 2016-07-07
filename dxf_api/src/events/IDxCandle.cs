using System;

namespace com.dxfeed.api.events {
    public interface IDxCandle {
        DateTime DateTime { get; }
        int Sequence { get; }
        double Count { get; }
        double Open { get; }
        double High { get; }
        double Low { get; }
        double Close { get; }
        double Volume { get; }
        double VWAP { get; }
        double BidVolume { get; }
        double AskVolume { get; }
    }
}
