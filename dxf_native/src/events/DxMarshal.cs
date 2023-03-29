#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Runtime.InteropServices;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    ///     The marshalling utility class
    /// </summary>
    public unsafe class DxMarshal
    {
        private static readonly int QuoteSize;
        private static readonly int OrderSize;
        private static readonly int TradeSize;
        private static readonly int SummarySize;
        private static readonly int ProfileSize;
        private static readonly int TimeAndSaleSize;
        private static readonly int CandleSize;
        private static readonly int TradeEthSize;
        private static readonly int SpreadOrderSize;
        private static readonly int GreeksSize;
        private static readonly int TheoPriceSize;
        private static readonly int UnderlyingSize;
        private static readonly int SeriesSize;
        private static readonly int ConfigurationSize;

        static DxMarshal()
        {
            QuoteSize = sizeof(DxQuote);
            OrderSize = sizeof(DxOrder);
            TradeSize = sizeof(DxTrade);
            SummarySize = sizeof(DxSummary);
            ProfileSize = sizeof(DxProfile);
            TimeAndSaleSize = sizeof(DxTimeAndSale);
            CandleSize = sizeof(DxCandle);
            TradeEthSize = sizeof(DxTrade);
            SpreadOrderSize = sizeof(DxOrder);
            GreeksSize = sizeof(DxGreeks);
            TheoPriceSize = sizeof(DxTheoPrice);
            UnderlyingSize = sizeof(DxUnderlying);
            SeriesSize = sizeof(DxSeries);
            ConfigurationSize = sizeof(DxConfiguration);
        }

        /// <summary>
        ///     Reads a string from address
        /// </summary>
        /// <param name="ptr">The address</param>
        /// <returns>The resulting string</returns>
        public static string ReadString(IntPtr ptr)
        {
            return Marshal.PtrToStringUni(ptr);
        }

        /// <summary>
        ///     Reads the Quote event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The quote symbol</param>
        /// <returns>The new Quote event by data from address</returns>
        public static NativeQuote ReadQuote(IntPtr head, int offset, string symbol)
        {
            return new NativeQuote((DxQuote*)IntPtr.Add(head, offset * QuoteSize), symbol);
        }

        /// <summary>
        ///     Reads the Order event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The order symbol</param>
        /// <returns>The new Order event by data from address</returns>
        public static NativeOrder ReadOrder(IntPtr head, int offset, string symbol)
        {
            return new NativeOrder((DxOrder*)IntPtr.Add(head, offset * OrderSize), symbol);
        }

        /// <summary>
        ///     Reads the Trade event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The trade symbol</param>
        /// <returns>The new Trade event by data from address</returns>
        public static NativeTrade ReadTrade(IntPtr head, int offset, string symbol)
        {
            return new NativeTrade((DxTrade*)IntPtr.Add(head, offset * TradeSize), symbol);
        }

        /// <summary>
        ///     Reads the Summary event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The summary symbol</param>
        /// <returns>The new Summary event by data from address</returns>
        public static NativeSummary ReadSummary(IntPtr head, int offset, string symbol)
        {
            return new NativeSummary((DxSummary*)IntPtr.Add(head, offset * SummarySize), symbol);
        }

        /// <summary>
        ///     Reads the Profile event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The profile symbol</param>
        /// <returns>The new Profile event by data from address</returns>
        public static NativeProfile ReadProfile(IntPtr head, int offset, string symbol)
        {
            return new NativeProfile((DxProfile*)IntPtr.Add(head, offset * ProfileSize), symbol);
        }

        /// <summary>
        ///     Reads the TnS event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The TnS symbol</param>
        /// <returns>The new TnS event by data from address</returns>
        public static NativeTimeAndSale ReadTimeAndSale(IntPtr head, int offset, string symbol)
        {
            return new NativeTimeAndSale((DxTimeAndSale*)IntPtr.Add(head, offset * TimeAndSaleSize), symbol);
        }

        /// <summary>
        ///     Reads the Candle event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The candle symbol</param>
        /// <returns>The new Candle event by data from address</returns>
        public static NativeCandle ReadCandle(IntPtr head, int offset, string symbol)
        {
            return new NativeCandle((DxCandle*)IntPtr.Add(head, offset * CandleSize), symbol);
        }

        /// <summary>
        ///     Reads the TradeETH event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The TradeETh symbol</param>
        /// <returns>The new TradeETH event by data from address</returns>
        public static NativeTradeETH ReadTradeEth(IntPtr head, int offset, string symbol)
        {
            return new NativeTradeETH((DxTrade*)IntPtr.Add(head, offset * TradeEthSize), symbol);
        }

        /// <summary>
        ///     Reads the SpreadOrder event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The SpreadOrder symbol</param>
        /// <returns>The new SpreadOrder event by data from address</returns>
        public static NativeSpreadOrder ReadSpreadOrder(IntPtr head, int offset, string symbol)
        {
            return new NativeSpreadOrder((DxOrder*)IntPtr.Add(head, offset * SpreadOrderSize), symbol);
        }

        /// <summary>
        ///     Reads the Greeks event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The Greeks symbol</param>
        /// <returns>The new Greeks event by data from address</returns>
        public static NativeGreeks ReadGreeks(IntPtr head, int offset, string symbol)
        {
            return new NativeGreeks((DxGreeks*)IntPtr.Add(head, offset * GreeksSize), symbol);
        }

        /// <summary>
        ///     Reads the TheoPrice event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The TheoPrice symbol</param>
        /// <returns>The new TheoPrice event by data from address</returns>
        public static NativeTheoPrice ReadTheoPrice(IntPtr head, int offset, string symbol)
        {
            return new NativeTheoPrice((DxTheoPrice*)IntPtr.Add(head, offset * TheoPriceSize), symbol);
        }

        /// <summary>
        ///     Reads the Underlying event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The Underlying symbol</param>
        /// <returns>The new Underlying event by data from address</returns>
        public static NativeUnderlying ReadUnderlying(IntPtr head, int offset, string symbol)
        {
            return new NativeUnderlying((DxUnderlying*)IntPtr.Add(head, offset * UnderlyingSize), symbol);
        }

        /// <summary>
        ///     Reads the Series event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The Series symbol</param>
        /// <returns>The new Series event by data from address</returns>
        public static NativeSeries ReadSeries(IntPtr head, int offset, string symbol)
        {
            return new NativeSeries((DxSeries*)IntPtr.Add(head, offset * SeriesSize), symbol);
        }

        /// <summary>
        ///     Reads the Configuration event from address
        /// </summary>
        /// <param name="head">The base address</param>
        /// <param name="offset">The offset</param>
        /// <param name="symbol">The Configuration symbol</param>
        /// <returns>The new Configuration event by data from address</returns>
        public static NativeConfiguration ReadConfiguration(IntPtr head, int offset, string symbol)
        {
            return new NativeConfiguration((DxConfiguration*)IntPtr.Add(head, offset * ConfigurationSize), symbol);
        }
    }
}