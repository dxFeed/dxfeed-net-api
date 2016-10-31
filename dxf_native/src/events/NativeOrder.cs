/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System.Globalization;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Order event is a snapshot for a full available market depth for a symbol.
    /// The collection of order events of a symbol represents the most recent information
    /// that is available about orders on the market at any given moment of time.
    /// Order events give information on several levels of details, called scopes - see Scope.
    /// Scope of an order is available via Scope property.
    /// </summary>
    public class NativeOrder : NativeOrderBase, IDxOrder
    {
        private readonly DxOrder order;
        private readonly DxString marketMaker;
        private readonly string source;

        internal unsafe NativeOrder(DxOrder* order, string symbol) : base(order, symbol)
        {
            this.order = *order;
            marketMaker = DxMarshal.ReadDxString(this.order.market_maker);

            fixed (char* charPtr = this.order.source)
            {
                source = new string(charPtr);
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Order: {{{0} {1}, MarketMaker: '{2}'}}",
                Symbol, base.ToString(), MarketMaker);
        }

        #region Implementation of IDxOrder

        /// <summary>
        /// Returns market maker or other aggregate identifier of this order.
        /// This value is defined for Scope.AGGREGATE and Scope.ORDER orders.
        /// </summary>
        public DxString MarketMaker
        {
            get { return marketMaker; }
        }

        #endregion
    }
}