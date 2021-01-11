#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;
using com.dxfeed.native.api;
using System.Globalization;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// TradeETH event is a snapshot of the price and size of the last trade during extended
    /// trading hours and the extended hours day volume.
    /// This event is defined only for symbols (typically stocks and ETFs) with a designated
    /// extended trading hours (ETH, pre market and post market trading sessions).
    /// It represents the most recent information that is available about ETH last trade on
    /// the market at any given moment of time.
    /// </summary>
    public class NativeTradeETH : NativeTradeBase, IDxTradeETH
    {
        /// <summary>
        /// Creates new trade with the specified event symbol.
        /// </summary>
        /// <param name="trade">Native DxTrade object.</param>
        /// <param name="symbol">The event symbol.</param>
        internal unsafe NativeTradeETH(DxTrade* trade, string symbol) : base(trade, symbol) {}

        /// <summary>
        /// Creates copy of trade object.
        /// </summary>
        /// <param name="trade">The IDxTrade object.</param>
        internal NativeTradeETH(IDxTradeETH trade) : base(trade) {}

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeTradeETH(this);
        }
        #endregion

        #region Implementation of IDxTradeETH
        #endregion

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "TradeETH {{{0}, {1}}}",
                EventSymbol, base.ToString());
        }
    }
}