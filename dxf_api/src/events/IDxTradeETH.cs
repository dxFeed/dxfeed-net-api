#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.events
{
    /// <summary>
    /// TradeETH event is a snapshot of the price and size of the last trade during extended
    /// trading hours and the extended hours day volume.
    /// This event is defined only for symbols (typically stocks and ETFs) with a designated
    /// extended trading hours (ETH, pre market and post market trading sessions).
    /// It represents the most recent information that is available about ETH last trade on
    /// the market at any given moment of time.
    /// </summary>
    [EventTypeAttribute("TradeETH")]
    public interface IDxTradeETH : IDxTradeBase
    {
    }
}