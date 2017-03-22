#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;
using System;
using System.Globalization;

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
    public abstract class NativeTradeBase : MarketEventImpl, IDxTradeBase
    {
        /// <summary>
        /// Creates new trade with the specified event symbol.
        /// </summary>
        /// <param name="trade">Native DxTrade object.</param>
        /// <param name="symbol">The event symbol.</param>
        internal unsafe NativeTradeBase(DxTrade* t, string symbol) : base(symbol)
        {
            DxTrade trade = *t;

            Time = TimeConverter.ToUtcDateTime(trade.time);
            ExchangeCode = trade.exchange_code;
            Price = trade.price;
            Size = trade.size;
            Tick = trade.tick;
            Change = trade.change;
            DayVolume = trade.day_volume;
        }

        /// <summary>
        /// Creates new trade with the specified event symbol.
        /// </summary>
        /// <param name="trade">Native DxTradeEth object.</param>
        /// <param name="symbol">The event symbol.</param>
        internal unsafe NativeTradeBase(DxTradeEth* t, string symbol) : base(symbol)
        {
            DxTradeEth trade = *t;

            Time = TimeConverter.ToUtcDateTime(trade.time);
            ExchangeCode = trade.exchange_code;
            Price = trade.price;
            Size = trade.size;
            Tick = 0;
            Change = 0.0;
            DayVolume= trade.eth_volume;
        }

        /// <summary>
        /// Creates copy of trade object.
        /// </summary>
        /// <param name="trade">The IDxTrade object.</param>
        internal NativeTradeBase(IDxTradeBase trade) : base(trade.EventSymbol)
        {
            Time = trade.Time;
            ExchangeCode = trade.ExchangeCode;
            Price = trade.Price;
            Size = trade.Size;
            Tick = trade.Tick;
            Change = trade.Change;
            DayVolume = trade.DayVolume;
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
        /// Returns date time of the last trade.
        /// </summary>
        public DateTime Time
        {
            get; private set;
        }

        /// <summary>
        /// Returns exchange code of the last trade.
        /// </summary>
        public char ExchangeCode
        {
            get; private set;
        }

        /// <summary>
        /// Returns price of the last trade.
        /// </summary>
        public double Price
        {
            get; private set;
        }

        /// <summary>
        /// Returns size of the last trade.
        /// </summary>
        public long Size
        {
            get; private set;
        }

        /// <summary>
        /// Returns tick of the last trade.
        /// </summary>
        public long Tick
        {
            get; private set;
        }

        /// <summary>
        /// Returns change value of the last trade.
        /// </summary>
        public double Change
        {
            get; private set;
        }

        /// <summary>
        /// Returns total volume traded for a day.
        /// </summary>
        public double DayVolume
        {
            get; private set;
        }

        #endregion
    }
}