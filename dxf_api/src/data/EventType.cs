#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.data
{
    [Flags]
    public enum EventType : int
    {
        None          = 0,
        Trade         = (1 << 0),
        Quote         = (1 << 1),
        Summary       = (1 << 2),
        Profile       = (1 << 3),
        Order         = (1 << 4),
        TimeAndSale   = (1 << 5),
        Candle        = (1 << 6),
        TradeETH      = (1 << 7),
        SpreadOrder   = (1 << 8),
        Greeks        = (1 << 9),
        TheoPrice     = (1 << 10),
        Underlying    = (1 << 11),
        Series        = (1 << 12),
        Configuration = (1 << 13)
    }
}
