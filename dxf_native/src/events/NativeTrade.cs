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
            Tick = (*trade).tick;
        }

        /// <summary>
        /// Creates copy of trade object.
        /// </summary>
        /// <param name="trade">The IDxTrade object.</param>
        internal NativeTrade(IDxTrade trade) : base(trade)
        {
            Tick = trade.Tick;
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeTrade(this);
        }

        #endregion

        #region Implementation of IDxTrade

        /// <summary>
        /// Returns Trend indicator – in which direction price is moving. The values are: Up (Tick = 1), Down (Tick = 2),
        /// and Undefined (Tick = 0).
        /// Should be used if IDxTradeBase.TickDirection is Undefined 
        /// </summary>
        public int Tick { get; private set; }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Trade {{{0}, {1}, Tick: {2}}}",
                EventSymbol, base.ToString(), Tick);
        }
    }
}