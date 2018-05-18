#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native.api;
using System.Globalization;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Spread order event is a snapshot for a full available market depth for all spreads
    /// on a given underlying symbol.The collection of spread order events of a symbol
    /// represents the most recent information that is available about spread orders on
    /// the market at any given moment of time.
    /// </summary>
    public class NativeSpreadOrder : NativeOrderBase, IDxSpreadOrder
    {
        internal unsafe NativeSpreadOrder(DxOrder* order, string symbol) : base(order, symbol)
        {
            DxOrder o = *order;
            SpreadSymbol = DxMarshal.ReadDxString(o.mm_or_ss);
        }

        internal NativeSpreadOrder(IDxSpreadOrder order) : base(order)
        {
            SpreadSymbol = (DxString)order.SpreadSymbol.Clone();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "SpreadOrder: {{{0} {1}, SpreadSymbol: '{2}'}}",
                EventSymbol, base.ToString(), SpreadSymbol);
        }

        #region Implementation of IDxSpreadOrder

        /// <summary>
        /// Returns spread symbol of this event.
        /// </summary>
        public DxString SpreadSymbol { get; private set; }

        #endregion
    }
}