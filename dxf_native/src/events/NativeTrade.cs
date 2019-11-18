#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

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
    /// Trade event is a snapshot of the price and size of the last trade during regular trading hours
    /// and an overall day volume.
    /// It represents the most recent information that is available about the regular last trade price on
    /// the market at any given moment of time.
    /// </summary>
    public class NativeTrade : NativeTradeBase, IDxTrade
    {
        /// <summary>
        /// Creates new trade with the specified event symbol.
        /// </summary>
        /// <param name="trade">Native DxTrade object.</param>
        /// <param name="symbol">The event symbol.</param>
        internal unsafe NativeTrade(DxTrade* trade, string symbol) : base(trade, symbol)
        {
            Change = (*trade).change;
        }

        /// <summary>
        /// Creates copy of trade object.
        /// </summary>
        /// <param name="trade">The IDxTrade object.</param>
        internal NativeTrade(IDxTrade trade) : base(trade)
        {
            Change = trade.Change;
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeTrade(this);
        }
        #endregion

        #region Implementation of IDxTrade
        /// <summary>
        /// Returns price change of the last trade, if availiable.
        /// </summary>
        public double Change { get; private set; }
        #endregion

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Trade {{{0}, {1}, Change: {2}}}",
                EventSymbol, base.ToString(), Change);
        }
    }
}