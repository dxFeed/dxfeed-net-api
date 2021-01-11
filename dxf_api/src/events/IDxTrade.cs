#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Trade event is a snapshot of the price and size of the last trade during regular trading hours
    /// and an overall day volume.
    /// It represents the most recent information that is available about the regular last trade price on
    /// the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("Trade")]
    public interface IDxTrade : IDxTradeBase
    {
        /// <summary>
        /// Returns Trend indicator – in which direction price is moving. The values are: Up (Tick = 1), Down (Tick = 2),
        /// and Undefined (Tick = 0).
        /// Should be used if IDxTradeBase.TickDirection is Undefined 
        /// </summary>
        int Tick { get; }
    }
}