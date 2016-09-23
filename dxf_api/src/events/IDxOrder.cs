/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    [EventTypeAttribute("Order")]
    public interface IDxOrder : IDxMarketEvent
    {
        long Index { get; }
        Side Side { get; }
        int Level { get; }
        DateTime Time { get; }
        char ExchangeCode { get; }
        DxString MarketMaker { get; }
        double Price { get; }
        long Size { get; }
        string Source { get; }
    }
}