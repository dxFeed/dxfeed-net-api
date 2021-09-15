#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;
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
    /// official volume for the whole trading day.
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
        /// <param name="tradeNative">Native DxTrade object.</param>
        /// <param name="symbol">The event symbol.</param>
        internal unsafe NativeTradeBase(DxTrade* tradeNative, string symbol) : base(symbol)
        {
            DxTrade trade = *tradeNative;

            Time = TimeConverter.ToUtcDateTime(trade.time);
            Sequence = trade.sequence;
            TimeNanoPart = trade.time_nanos;
            ExchangeCode = trade.exchange_code;
            Price = trade.price;
            Size = trade.size;
            Change = trade.change;
            DayId = trade.day_id;
            DayVolume = trade.day_volume;
            DayTurnover = trade.day_turnover;
            TickDirection = trade.direction;
            IsExtendedTradingHours = trade.is_eth;
            RawFlags = trade.raw_flags;
            Scope = trade.scope;
        }

        /// <summary>
        /// Creates copy of trade object.
        /// </summary>
        /// <param name="trade">The IDxTrade object.</param>
        internal NativeTradeBase(IDxTradeBase trade) : base(trade.EventSymbol)
        {
            Time = trade.Time;
            Sequence = trade.Sequence;
            TimeNanoPart = trade.TimeNanoPart;
            ExchangeCode = trade.ExchangeCode;
            Price = trade.Price;
            Size = trade.Size;
            Change = trade.Change;
            DayId = trade.DayId;
            DayVolume = trade.DayVolume;
            DayTurnover = trade.DayTurnover;
            TickDirection = trade.TickDirection;
            IsExtendedTradingHours = trade.IsExtendedTradingHours;
            Scope = trade.Scope;
            RawFlags = trade.RawFlags;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Time: {0:o}, Sequence: {1}, TimeNanoPart: {2}, " +
                "ExchangeCode: {3}, Price: {4}, Size: {5},  " +
                "Change: {6}, DayId: {7}, DayVolume: {8}, DayTurnover: {9}, " +
                "Tick Direction: {10}, IsETH: {11}, " +
                "RawFlags: {12:x8}, Scope: {13}",
                Time, Sequence, TimeNanoPart,
                ExchangeCode, Price, Size, Change,
                DayId, DayVolume, DayTurnover,
                TickDirection, IsExtendedTradingHours,
                RawFlags, Scope
            );
        }

        #region Implementation of IDxTradeBase

        /// <summary>
        /// Returns time of the last trade. This time has precision up to milliseconds.
        /// </summary>
        public DateTime Time { get; internal set; }

        /// <summary>
        /// Returns sequence number of the last trade to distinguish trades that have the same
        /// time. This sequence number does not have to be unique and
        /// does not need to be sequential.
        /// </summary>
        public int Sequence { get; internal set; }

        /// <summary>
        /// Returns microseconds and nanoseconds time part of the last trade.
        /// </summary>
        public int TimeNanoPart { get; internal set; }

        /// <summary>
        /// Returns exchange code of the last trade.
        /// </summary>
        public char ExchangeCode { get; internal set; }

        /// <summary>
        /// Returns price of the last trade.
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        /// Returns size of the last trade.
        /// </summary>
        public double Size { get; internal set; }

        /// <summary>
        /// Returns price change of the last trade, if available.
        /// </summary>
        public double Change { get; internal set; }

        /// <summary>
        /// Returns identifier of the day that this event represents.
        /// Identifier of the day is the number of days passed since January 1, 1970.
        /// </summary>
        public int DayId { get; internal set; }

        /// <summary>
        /// Returns total volume traded for a day.
        /// </summary>
        public double DayVolume { get; internal set; }

        /// <summary>
        /// Returns total turnover traded for a day.
        /// Day VWAP can be computed with DayTurnover / DayVolume.
        /// </summary>
        public double DayTurnover { get; internal set; }

        /// <summary>
        /// Returns tick direction of the last trade.
        /// </summary>
        public Direction TickDirection { get; internal set; }

        /// <summary>
        /// Returns whether last trade was in extended trading hours.
        /// </summary>
        public bool IsExtendedTradingHours { get; internal set; }

        /// <summary>
        /// Returns whether last trade was a composite or regional (other constants are not used here).
        /// </summary>
        public Scope Scope { get; internal set; }

        /// <summary>
        /// Returns implementation-specific raw bit flags value
        /// </summary>
        public int RawFlags { get; internal set; }

        #endregion
    }
}