#region License

/*
Copyright � 2010-2019 dxFeed Solutions DE GmbH

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
        /// Returns price change of the last trade, if availiable.
        /// </summary>
        double Change { get; }
    }
}