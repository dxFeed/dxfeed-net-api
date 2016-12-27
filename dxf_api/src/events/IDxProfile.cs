/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
{
    [EventTypeAttribute("Profile")]
    public interface IDxProfile : IDxMarketEvent, LastingEvent<string>
    {
        double Beta { get; }
        double Eps { get; }
        long DivFreq { get; }
        double ExdDivAmount { get; }
        int ExdDivDate { get; }
        double _52HighPrice { get; }
        double _52LowPrice { get; }
        double Shares { get; }
        string Description { get; }
        long Flags { get; }
        string StatusReason { get; }
        DateTime HaltStartTime { get; }
        DateTime HaltEndTime { get; }
        double HighLimitPrice { get; }
        double LowLimitPrice { get; }
    }
}