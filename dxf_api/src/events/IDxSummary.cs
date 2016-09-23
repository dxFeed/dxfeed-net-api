/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

namespace com.dxfeed.api.events
{
    [EventTypeAttribute("Summary")]
    public interface IDxSummary : IDxMarketEvent
    {
        int DayId { get; }
        double DayOpenPrice { get; }
        double DayHighPrice { get; }
        double DayLowPrice { get; }
        double DayClosePrice { get; }
        int PrevDayId { get; }
        double PrevDayClosePrice { get; }
        long OpenInterest { get; }
        long Flags { get; }
        char ExchangeCode { get; }
    }
}
