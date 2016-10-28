/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    public enum TimeAndSaleType
    {
        New = 0, Correction = 1, Cancel = 2
    }

    public interface IDxTimeAndSale : IDxMarketEvent
    {
        long EventId { get; }
        DateTime Time { get; }
        char ExchangeCode { get; }
        double Price { get; }
        long Size { get; }
        double BidPrice { get; }
        double AskPrice { get; }
        DxString ExchangeSaleConditions { get; }
        bool IsTrade { get; }
        TimeAndSaleType Type { get; }
    }
}