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
    /// <summary>
    /// Base class for common fields of IDxTrade and IDxTradeETH events. Trade events represent 
    /// the most recent information that is available about the last trade on the market at any 
    /// given moment of time.
    ///
    /// IDxTrade event represents last trade information for regular trading hours(RTH) with an 
    /// official volumefor the whole trading day.
    ///
    /// IDxTradeETH event is defined only for symbols (typically stocks and ETFs) with a designated
    /// extended trading hours (ETH, pre market and post market trading sessions). It represents
    /// last trade price during ETH and accumulated volume during ETH.
    /// </summary>
    public abstract class NativeTradeBase : MarketEvent, IDxTradeBase
    {
        private readonly DxTrade trade;

        /// <summary>
        /// Creates new trade with the specified event symbol.
        /// </summary>
        /// <param name="trade">Native DxTrade object.</param>
        /// <param name="symbol">The event symbol.</param>
        internal unsafe NativeTradeBase(DxTrade* trade, string symbol) : base(symbol)
        {
            this.trade = *trade;
        }

        /// <summary>
        /// Creates new trade with the specified event symbol.
        /// </summary>
        /// <param name="trade">Native DxTradeEth object.</param>
        /// <param name="symbol">The event symbol.</param>
        internal unsafe NativeTradeBase(DxTradeEth* trade, string symbol) : base(symbol)
        {
            this.trade.time = trade->time;
            this.trade.exchange_code = trade->exchange_code;
            this.trade.price = trade->price;
            this.trade.size = trade->size;
            this.trade.tick = 0;
            this.trade.change = 0.0;
            this.trade.day_volume = trade->eth_volume;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Time: {0:o}, " +
                "ExchangeCode: '{1}', Price: {2}, Size: {3}, Tick: {4}, Change: {5}, " +
                "DayVolume: {6}",
                Time, ExchangeCode, Price, Size, Tick, Change, DayVolume);
        }

        #region Implementation of IDxTradeBase

        /// <summary>
        /// Returns time of the last trade. Time is measured in milliseconds between the current 
        /// time and midnight, January 1, 1970 UTC.
        /// </summary>
        public DateTime Time
        {
            get { return TimeConverter.ToUtcDateTime(trade.time); }
        }

        /// <summary>
        /// Returns exchange code of the last trade.
        /// </summary>
        public char ExchangeCode
        {
            get { return trade.exchange_code; }
        }

        /// <summary>
        /// Returns price of the last trade.
        /// </summary>
        public double Price
        {
            get { return trade.price; }
        }

        /// <summary>
        /// Returns size of the last trade.
        /// </summary>
        public long Size
        {
            get { return trade.size; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Tick
        {
            get { return trade.tick; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Change
        {
            get { return trade.change; }
        }

        /// <summary>
        /// Returns total volume traded for a day.
        /// </summary>
        public double DayVolume
        {
            get { return trade.day_volume; }
        }

        #endregion
    }
}