/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
{
    [EventTypeAttribute("Candle")]
    public interface IDxCandle : IDxMarketEvent
    {
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
