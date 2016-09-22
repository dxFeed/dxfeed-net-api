/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.events
{
    public interface IDxQuote
    {
        DateTime BidTime { get; }
        char BidExchangeCode { get; }
        double BidPrice { get; }
        long BidSize { get; }
        DateTime AskTime { get; }
        char AskExchangeCode { get; }
        double AskPrice { get; }
        long AskSize { get; }
    }
}
