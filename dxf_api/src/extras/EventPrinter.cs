#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Globalization;
using com.dxfeed.api.candle;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.api.plb;

namespace com.dxfeed.api.extras
{
    public interface IEventPrinter : IDxFeedListener,
        IDxTradeETHListener,
        IDxSpreadOrderListener,
        IDxCandleListener,
        IDxGreeksListener,
        IDxTheoPriceListener,
        IDxUnderlyingListener,
        IDxSeriesListener,
        IDxConfigurationListener
    {
    }

    /// <summary>
    /// The simple event printer class
    /// </summary>
    public class EventPrinter :
        IEventPrinter
    {
        #region Implementation of IDxFeedListener

        /// <summary>
        /// Prints the Quote event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
            foreach (var q in buf)
                Console.WriteLine($"{buf.Symbol} {q}");
        }

        /// <summary>
        /// Prints the Trade event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {
            foreach (var t in buf)
                Console.WriteLine($"{buf.Symbol} {t}");
        }

        /// <summary>
        /// Prints the Order event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            foreach (var o in buf)
                Console.WriteLine($"{buf.Symbol} {o}");
        }

        /// <summary>
        /// Prints the Profile event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {
            foreach (var p in buf)
                Console.WriteLine($"{buf.Symbol} {p}");
        }

        /// <summary>
        /// Prints the Summary event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnSummary<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {
            foreach (var f in buf)
                Console.WriteLine($"{buf.Symbol} {f}");
        }

        /// <summary>
        /// Prints the TimeAndSale event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            foreach (var ts in buf)
                Console.WriteLine($"{buf.Symbol} {ts}");
        }

        #endregion

        #region Implementation of IDxTradeEthListener

        /// <summary>
        /// Prints the TradeETH event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnTradeETH<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeETH
        {
            foreach (var te in buf)
                Console.WriteLine($"{buf.Symbol} {te}");
        }

        #endregion

        #region Implementation of IDxSpreadOrderListener

        /// <summary>
        /// Prints the SpreadOrder event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            foreach (var so in buf)
                Console.WriteLine($"{buf.Symbol} {so}");
        }

        #endregion

        #region Implementation of IDxCandleListener

        /// <summary>
        /// Prints the Candle event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            foreach (var c in buf)
                Console.WriteLine($"{buf.Symbol} {c}");
        }

        #endregion

        #region Implementation of IDxGreeksListener

        /// <summary>
        /// Prints the Greeks event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            foreach (var g in buf)
                Console.WriteLine($"{buf.Symbol} {g}");
        }

        #endregion

        #region Implementation of IDxTheoPriceListener

        /// <summary>
        /// Prints the TheoPrice event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
            foreach (var tp in buf)
                Console.WriteLine($"{buf.Symbol} {tp}");
        }

        #endregion

        #region Implementation of IDxUnderlyingListener

        /// <summary>
        /// Prints the Underlying event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnUnderlying<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxUnderlying
        {
            foreach (var u in buf)
                Console.WriteLine($"{buf.Symbol} {u}");
        }

        #endregion

        #region Implementation of IDxSeriesListener

        /// <summary>
        /// Prints the Series event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            foreach (var s in buf)
                Console.WriteLine($"{buf.Symbol} {s}");
        }

        #endregion

        #region Implementation of IDxConfigurationListener

        /// <summary>
        /// Prints the Configuration event buffer
        /// </summary>
        /// <param name="buf">The event buffer</param>
        /// <typeparam name="TB">The event buffer type</typeparam>
        /// <typeparam name="TE">The event type</typeparam>
        public void OnConfiguration<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxConfiguration
        {
            foreach (var s in buf)
                Console.WriteLine($"{buf.Symbol} {s}");
        }

        #endregion
    }

    /// <summary>
    /// The dummy event printer class (prints nothing)
    /// </summary>
    public class DummyEventPrinter :
        IEventPrinter
    {
        #region Implementation of IDxFeedListener

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
        }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {
        }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {
        }

        public void OnSummary<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {
        }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
        }

        #endregion

        #region Implementation of IDxTradeEthListener

        public void OnTradeETH<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeETH
        {
        }

        #endregion

        #region Implementation of IDxSpreadOrderListener

        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
        }

        #endregion

        #region Implementation of IDxCandleListener

        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
        }

        #endregion

        #region Implementation of IDxGreeksListener

        public void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
        }

        #endregion

        #region Implementation of IDxTheoPriceListener

        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
        }

        #endregion

        #region Implementation of IDxUnderlyingListener

        public void OnUnderlying<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxUnderlying
        {
        }

        #endregion

        #region Implementation of IDxSeriesListener

        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
        }

        #endregion

        #region Implementation of IDxConfigurationListener

        public void OnConfiguration<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxConfiguration
        {
        }

        #endregion
    }

    public interface ISnapshotPrinter :
        IDxOrderSnapshotListener,
        IDxCandleSnapshotListener,
        IDxTimeAndSaleSnapshotListener,
        IDxSpreadOrderSnapshotListener,
        IDxGreeksSnapshotListener,
        IDxSeriesSnapshotListener
    {
    }

    /// <summary>
    /// The simple events snapshot printer class
    /// </summary>
    public class SnapshotPrinter :
        ISnapshotPrinter
    {
        private readonly int recordsPrintLimit;

        public SnapshotPrinter(int recordsPrintLimit)
        {
            this.recordsPrintLimit = recordsPrintLimit;
        }

        #region Implementation of IDxCandleSnapshotListener

        /// <summary>
        ///     On Candle snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            PrintSnapshot(buf);
        }

        #endregion //IDxCandleSnapshotListener

        #region Implementation of IDxGreeksSnapshotListener

        public void OnGreeksSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            PrintSnapshot(buf);
        }

        #endregion

        #region Implementation of IDxOrderSnapshotListener

        /// <summary>
        ///     On Order snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            PrintSnapshot(buf);
        }

        #endregion //IDxOrderSnapshotListener

        #region Implementation of IDxSeriesSnapshotListener

        public void OnSeriesSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            PrintSnapshot(buf);
        }

        #endregion

        #region Implementation of IDxSpreadOrderSnapshotListener

        /// <summary>
        ///     On SpreadOrder snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            PrintSnapshot(buf);
        }

        #endregion

        #region Implementation of IDxTimeAndSaleSnapshotListener

        /// <summary>
        ///     On TimeAndSale snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            PrintSnapshot(buf);
        }

        #endregion //IDxTimeAndSaleSnapshotListener

        private void PrintSnapshot<TE>(IDxEventBuf<TE> buf)
        {
            var symbolStr = buf.Symbol;

            if (buf.EventType == EventType.Candle)
            {
                var candleSymbol = CandleSymbol.ValueOf(symbolStr);
                if (candleSymbol.IsDefault())
                    symbolStr = candleSymbol.ToFullString();
            }

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}}}",
                buf.EventType, symbolStr, buf.Size));

            var count = 0;
            foreach (var o in buf)
            {
                Console.WriteLine($"   {{ {o} }}");

                if (++count < recordsPrintLimit || recordsPrintLimit == 0) continue;

                Console.WriteLine($"   {{ ... {buf.Size - count} records left ...}}");

                break;
            }
        }
    }

    /// <summary>
    /// The dummy events snapshot printer class (prints nothing)
    /// </summary>
    public class DummySnapshotPrinter :
        ISnapshotPrinter
    {
        #region Implementation of IDxCandleSnapshotListener

        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
        }

        #endregion //IDxCandleSnapshotListener

        #region Implementation of IDxGreeksSnapshotListener

        public void OnGreeksSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
        }

        #endregion

        #region Implementation of IDxOrderSnapshotListener

        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
        }

        #endregion //IDxOrderSnapshotListener

        #region Implementation of IDxSeriesSnapshotListener

        public void OnSeriesSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
        }

        #endregion

        #region Implementation of IDxSpreadOrderSnapshotListener

        public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
        }

        #endregion

        #region Implementation of IDxTimeAndSaleSnapshotListener

        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
        }

        #endregion //IDxTimeAndSaleSnapshotListener
    }

    public interface IPriceLevelBookPrinter : IDxOnNewPriceLevelBookHandler, IDxOnPriceLevelBookUpdateHandler,
        IDxOnPriceLevelBookIncChangeHandler
    {
    }

    /// <summary>
    /// The simple PLB printer class
    /// </summary>
    public class PriceLevelBookPrinter : IPriceLevelBookPrinter
    {
        private static void DumpBook(DxPriceLevelBook book)
        {
            Console.WriteLine($"{"Ask",-15} {"Size",-8} {"Time",-15} | {"Bid",-15} {"Size",-8} {"Time",-15}");
            for (var i = 0; i < Math.Max(book.Asks.Length, book.Bids.Length); ++i)
            {
                if (i < book.Asks.Length)
                    Console.Write("{0,-15:n6} {1,-8:n2} {2,-15:yyyyMMdd-HHmmss}", book.Asks[i].Price,
                        book.Asks[i].Size,
                        book.Asks[i].Time);
                else
                    Console.Write("{0,-15} {1,-8} {2,-15}", "", "", "");
                Console.Write(" | ");
                if (i < book.Bids.Length)
                    Console.Write("{0,-15:n6} {1,-8:n2} {2,-15:yyyyMMdd-HHmmss}", book.Bids[i].Price,
                        book.Bids[i].Size,
                        book.Bids[i].Time);
                Console.WriteLine();
            }
        }

        public void OnNewBook(DxPriceLevelBook book)
        {
            Console.WriteLine($"\nNew Price Level Book for {book.Symbol}:");
            DumpBook(book);
        }

        public void OnBookUpdate(DxPriceLevelBook book)
        {
            Console.WriteLine($"\nThe Update of The Price Level Book for {book.Symbol}:");
            DumpBook(book);
        }

        public void OnBookIncrementalChange(DxPriceLevelBook removals, DxPriceLevelBook additions,
            DxPriceLevelBook updates)
        {
            Console.WriteLine($"\nThe Incremental Update of The Price Level Book for {removals.Symbol}:");

            if (removals.Asks.Length > 0 || removals.Bids.Length > 0)
            {
                Console.WriteLine("\nREMOVALS:");
                DumpBook(removals);
            }

            if (additions.Asks.Length > 0 || additions.Bids.Length > 0)
            {
                Console.WriteLine("\nADDITIONS:");
                DumpBook(additions);
            }

            if (updates.Asks.Length > 0 || updates.Bids.Length > 0)
            {
                Console.WriteLine("\nUPDATES:");
                DumpBook(updates);
            }
        }
    }

    /// <summary>
    /// The dummy PLB printer class (prints nothing)
    /// </summary>
    public class DummyPriceLevelBookPrinter : IPriceLevelBookPrinter
    {
        public void OnNewBook(DxPriceLevelBook book)
        {
        }

        public void OnBookUpdate(DxPriceLevelBook book)
        {
        }

        public void OnBookIncrementalChange(DxPriceLevelBook removals, DxPriceLevelBook additions,
            DxPriceLevelBook updates)
        {
        }
    }

    /// <summary>
    ///     Incremental order snapshot printer
    /// </summary>
    public class IncOrderSnapshotPrinter : IDxIncOrderSnapshotListener
    {
        private readonly int recordsPrintLimit;

        public IncOrderSnapshotPrinter(int recordsPrintLimit)
        {
            this.recordsPrintLimit = recordsPrintLimit;
        }

        #region Implementation of IDxIncOrderSnapshotListener

        /// <summary>
        ///     On Order snapshot or update event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        /// <param name="newSnapshot">Snapshot or update.</param>
        public void OnOrderSnapshot<TB, TE>(TB buf, bool newSnapshot)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            var symbolStr = buf.Symbol;
            Console.WriteLine("Snapshot {0} {{Symbol: '{1}', RecordsCount: {2}, Type: {3}}}", buf.EventType, symbolStr,
                buf.Size, newSnapshot ? "full" : "update");
            var count = 0;
            foreach (var o in buf)
            {
                if (!o.EventFlags.HasFlag(EventFlag.RemoveEvent) && o.HasSize())
                    Console.WriteLine("   {{ {0} }}", o);
                else
                    Console.WriteLine("   REMOVAL {{ {0} }}", o.Index);

                if (++count < recordsPrintLimit || recordsPrintLimit == 0) continue;
                Console.WriteLine("   {{ ... {0} records left ...}}", buf.Size - count);
                break;
            }

            if (buf.Size > 0)
                Console.WriteLine();
        }

        #endregion //IDxIncOrderSnapshotListener
    }

    /// <summary>
    ///     Dummy incremental order snapshot printer (prints nothing)
    /// </summary>
    public class DummyIncOrderSnapshotPrinter : IDxIncOrderSnapshotListener
    {
        #region Implementation of IDxIncOrderSnapshotListener

        /// <summary>
        ///     On Order snapshot or update event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        /// <param name="newSnapshot">Snapshot or update.</param>
        public void OnOrderSnapshot<TB, TE>(TB buf, bool newSnapshot)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
        }

        #endregion //IDxIncOrderSnapshotListener
    }
}