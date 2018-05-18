/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.data
{
    [Flags]
    public enum EventType : int
    {
        None          = 0,
        Trade         = (1 << 1),
        Quote         = (1 << 2),
        Summary       = (1 << 3),
        Profile       = (1 << 4),
        Order         = (1 << 5),
        TimeAndSale   = (1 << 6),
        Candle        = (1 << 7),
        TradeETH      = (1 << 8),
        SpreadOrder   = (1 << 9),
        Greeks        = (1 << 10),
        TheoPrice     = (1 << 11),
        Underlying    = (1 << 12),
        Series        = (1 << 13),
        Configuration = (1 << 14)
    }
}
