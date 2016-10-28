/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
{
    public interface IDxTrade : IDxMarketEvent
    {
        DateTime Time { get; }
        char ExchangeCode { get; }
        double Price { get; }
        long Size { get; }
        long Tick { get; }
        double Change { get; }
        double DayVolume { get; }
    }
}