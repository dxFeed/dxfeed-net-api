#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Runtime.InteropServices;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.api.plb;
using com.dxfeed.native.api;
using DxPriceLevelBook = com.dxfeed.native.api.DxPriceLevelBook;

namespace com.dxfeed.native
{
    /// <summary>
    /// Wrapper for the formation of the price level book
    /// </summary>
    public class PriceLevelBook : IPriceLevelBook
    {
        /// <summary>
        /// Creates the new price level book instance for the specified symbol and source
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">The current connection</param>
        /// <param name="symbol">The book symbol</param>
        /// <param name="source">The order source</param>
        /// <param name="levelsNumber">The price level book levels number (0 -- all)</param>
        /// <exception cref="ArgumentException"></exception>
        public PriceLevelBook(NativeConnection connection, string symbol, string source, int levelsNumber)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Invalid symbol parameter.");
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Invalid source parameter.");
            }

            this.symbol = symbol;
            this.source = source;
            this.levelsNumber = levelsNumber;

            C.CheckOk(C.Instance.CreatePriceLevelBook2(connection.Handle, symbol, source, levelsNumber,
                out bookHandle));
        }

        /// <summary>
        /// Creates the new price level book instance for the specified symbol and sources
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">The current connection</param>
        /// <param name="symbol">The book symbol</param>
        /// <param name="source">The order source</param>
        /// <param name="levelsNumber">The price level book levels number (0 -- all)</param>
        /// <exception cref="ArgumentException"></exception>
        public PriceLevelBook(NativeConnection connection, string symbol, OrderSource source,
            int levelsNumber) :
            this(connection, symbol, source.ToString(), levelsNumber)
        {
        }


        /// <summary>
        ///     This code added to correctly implement the disposable pattern.
        /// </summary>
        /// <param name="disposing">The disposing flag</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            C.Instance.ClosePriceLevelBook2(bookHandle);
            disposedValue = true;
        }

        /// <inheritdoc />
        ~PriceLevelBook()
        {
            Dispose(false);
        }

        private void SetHandlersImpl()
        {
            nativeOnNewBookListener = null;
            nativeOnBookUpdateListener = null;
            nativeOnBookIncChangeListener = null;

            if (onNewPriceLevelBookHandler != null)
            {
                nativeOnNewBookListener = OnNewBook;
            }

            if (onPriceLevelBookUpdateHandler != null)
            {
                nativeOnBookUpdateListener = OnBookUpdate;
            }

            if (onPriceLevelBookIncChangeHandler != null)
            {
                nativeOnBookIncChangeListener = OnBookIncrementalChange;
            }

            C.CheckOk(C.Instance.SetPriceLevelBookListeners(bookHandle, nativeOnNewBookListener,
                nativeOnBookUpdateListener, nativeOnBookIncChangeListener, IntPtr.Zero));
        }

        /// <inheritdoc />
        public void SetHandlers(IDxOnNewPriceLevelBookHandler newPriceLevelBookHandler,
            IDxOnPriceLevelBookUpdateHandler priceLevelBookUpdateHandler,
            IDxOnPriceLevelBookIncChangeHandler priceLevelBookIncChangeHandler)
        {
            onNewPriceLevelBookHandler = newPriceLevelBookHandler;
            onPriceLevelBookUpdateHandler = priceLevelBookUpdateHandler;
            onPriceLevelBookIncChangeHandler = priceLevelBookIncChangeHandler;

            SetHandlersImpl();
        }

        /// <summary>
        /// Disposes the price level book
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static dxfeed.api.events.DxPriceLevelBook CreateDxPriceLevelBook(string symbol, IntPtr priceLevelBook)
        {
            if (priceLevelBook == IntPtr.Zero)
            {
                return new dxfeed.api.events.DxPriceLevelBook(
                    "", new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel[] { },
                    new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel[] { });
            }

            DxPriceLevelBook nativeBook;
            unsafe
            {
                nativeBook = *(DxPriceLevelBook*)priceLevelBook;
            }

            var asks =
                new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel[nativeBook.asks_count];

            for (var i = 0; i < nativeBook.asks_count; ++i)
            {
                asks[i] = CreateLevel(GetLevel(nativeBook.asks, i));
            }

            var bids =
                new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel[nativeBook.bids_count];

            for (var i = 0; i < nativeBook.bids_count; ++i)
            {
                bids[i] = CreateLevel(GetLevel(nativeBook.bids, i));
            }

            return new dxfeed.api.events.DxPriceLevelBook(
                symbol, bids, asks);
        }

        private void OnNewBook(IntPtr priceLevelBook, IntPtr userData)
        {
            onNewPriceLevelBookHandler?.OnNewBook(CreateDxPriceLevelBook(symbol, priceLevelBook));
        }

        private void OnBookUpdate(IntPtr bookUpdate, IntPtr userData)
        {
            onPriceLevelBookUpdateHandler?.OnBookUpdate(CreateDxPriceLevelBook(symbol, bookUpdate));
        }

        private void OnBookIncrementalChange(IntPtr removals, IntPtr additions, IntPtr updates, IntPtr userData)
        {
            onPriceLevelBookIncChangeHandler?.OnBookIncrementalChange(CreateDxPriceLevelBook(symbol, removals),
                CreateDxPriceLevelBook(symbol, additions), CreateDxPriceLevelBook(symbol, updates));
        }

        private static unsafe DxPriceLevel GetLevel(IntPtr levels, int index)
        {
            return *(DxPriceLevel*)IntPtr.Add(levels, sizeof(DxPriceLevel) * index);
        }

        private static dxfeed.api.events.DxPriceLevelBook.DxPriceLevel CreateLevel(DxPriceLevel level)
        {
            return new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel(
                level.price, level.size, TimeConverter.ToUtcDateTime(level.time));
        }

        private bool disposedValue; // To detect redundant calls
        private IDxOnNewPriceLevelBookHandler onNewPriceLevelBookHandler;
        private IDxOnPriceLevelBookUpdateHandler onPriceLevelBookUpdateHandler;
        private IDxOnPriceLevelBookIncChangeHandler onPriceLevelBookIncChangeHandler;
        private readonly string symbol;
        private string source;
        private int levelsNumber;

        // ReSharper disable once NotAccessedField.Local
        private C.dxf_price_level_book_listener_t nativeOnNewBookListener; //to prevent from being garbage collected
        private C.dxf_price_level_book_listener_t nativeOnBookUpdateListener; //to prevent from being garbage collected

        private C.dxf_price_level_book_inc_listener_t
            nativeOnBookIncChangeListener; //to prevent from being garbage collected

        private IntPtr bookHandle;
    }
}