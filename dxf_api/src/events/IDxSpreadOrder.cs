/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Spread order event is a snapshot for a full available market depth for all spreads
    /// on a given underlying symbol.The collection of spread order events of a symbol
    /// represents the most recent information that is available about spread orders on
    /// the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("SpreadOrder")]
    public interface IDxSpreadOrder : IDxOrderBase
    {
        /// <summary>
        /// Returns spread symbol of this event.
        /// </summary>
        string SpreadSymbol { get; }
    }
}