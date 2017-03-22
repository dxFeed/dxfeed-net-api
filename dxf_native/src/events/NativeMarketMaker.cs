#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.events;
using com.dxfeed.native.api;
using System.Globalization;

namespace com.dxfeed.native.events
{
    public class NativeMarketMaker : MarketEventImpl, IDxMarketMaker
    {
        internal unsafe NativeMarketMaker(DxMarketMaker* marketMaker, string symbol) : base(symbol)
        {
            DxMarketMaker mm = *marketMaker;

            Exchange = mm.mm_exchange;
            Id = mm.mm_id;
            BidPrice = mm.mmbid_price;
            BidSize = mm.mmbid_size;
            AskPrice = mm.mmask_price;
            AskSize = mm.mmask_size;
        }

        internal NativeMarketMaker(IDxMarketMaker mm) : base(mm.EventSymbol)
        {
            Exchange = mm.Exchange;
            Id = mm.Id;
            BidPrice = mm.BidPrice;
            BidSize = mm.BidSize;
            AskPrice = mm.AskPrice;
            AskSize = mm.AskSize;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "MarketMaker {{{6}, " +
                "Exchange: '{0}', Id: {1}, BidPrice: {2}, BidSize: {3}, AskPrice: {4}, " +
                "AskSize: {5}}}",
                Exchange, Id, BidPrice, BidSize, AskPrice, AskSize, EventSymbol);
        }

        #region Implementation of ICloneable
        public override object Clone()
        {
            return new NativeMarketMaker(this);
        }
        #endregion

        #region Implementation of IDxMarketMaker

        public char Exchange
        {
            get; private set;
        }

        public int Id
        {
            get; private set;
        }

        public double BidPrice
        {
            get; private set;
        }

        public int BidSize
        {
            get; private set;
        }

        public double AskPrice
        {
            get; private set;
        }

        public int AskSize
        {
            get; private set;
        }

        #endregion
    }
}