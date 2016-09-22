/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    public struct NativeTrade : IDxTrade
    {
        private readonly DxTrade trade;

        internal unsafe NativeTrade(DxTrade* trade)
        {
            this.trade = *trade;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Trade {{Time: {0:o}, ExchangeCode: '{1}', " +
                "Price: {2}, Size: {3}, Tick: {4}, Change: {5}, DayVolume: {5}}}",
                Time, ExchangeCode, Price, Size, Tick, Change, DayVolume);
        }

        #region Implementation of IDxTrade

        public DateTime Time
        {
            get { return TimeConverter.ToUtcDateTime(trade.time); }
        }

        public char ExchangeCode
        {
            get { return trade.exchange_code; }
        }

        public double Price
        {
            get { return trade.price; }
        }

        public long Size
        {
            get { return trade.size; }
        }

        public long Tick
        {
            get { return trade.tick; }
        }

        public double Change
        {
            get { return trade.change; }
        }

        public double DayVolume
        {
            get { return trade.day_volume; }
        }

        #endregion
    }
}
