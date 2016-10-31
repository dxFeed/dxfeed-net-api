/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
{
    [Flags]
    public enum EventType : int
    {
        None = 0,
        Trade = 1,
        Quote = 2,
        Summary = 4,
        Profile = 8,
        Order = 16,
        TimeAndSale = 32,
        Candle = 64, 
        TradeETH = 128,
        SpreadOrder = 256
    }
}
