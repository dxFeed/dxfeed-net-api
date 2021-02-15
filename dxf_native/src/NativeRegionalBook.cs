#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;
using com.dxfeed.native.events;

namespace com.dxfeed.native
{
    public class NativeRegionalBook : IDxRegionalBook
    {
        /// <summary>
        /// Creates the new regional book instance
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">The current connection</param>
        /// <param name="symbol">The book symbol</param>
        /// <param name="bookListener">The book listener implementation</param>
        /// <param name="quoteListener">The quote listener implementation</param>
        /// <exception cref="ArgumentException"></exception>
        public NativeRegionalBook(NativeConnection connection, string symbol, IDxRegionalBookListener bookListener,
            IDxQuoteListener quoteListener)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Invalid symbol parameter.");
            }
            
            this.bookListener = bookListener;
            this.quoteListener = quoteListener;

            if (bookListener == null && quoteListener == null) return;
            
            C.CheckOk(C.Instance.dxf_create_regional_book(connection.Handle, symbol, out bookHandle));
            try
            {
                if (this.bookListener != null)
                {
                    C.CheckOk(C.Instance.dxf_attach_regional_book_listener(bookHandle, nativeBookListener = OnBook, IntPtr.Zero));
                }
                if (this.quoteListener != null)
                {
                    C.CheckOk(C.Instance.dxf_attach_regional_book_listener_v2(bookHandle, nativeQuoteListener = OnQuote, IntPtr.Zero));
                }
            }
            catch (DxException)
            {
                C.Instance.dxf_close_regional_book(bookHandle);
                throw;
            }
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            
            C.Instance.dxf_close_regional_book(bookHandle);
            disposedValue = true;
        }

        ~NativeRegionalBook() {
           Dispose(false);
        }

        /// <summary>
        /// Disposes the regional book
        /// </summary>
        /// <remarks>
        ///     Don't call this method inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativeSnapshotSubscription classes
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnBook(IntPtr priceLevelBook, IntPtr userData)
        {
            if (priceLevelBook == IntPtr.Zero)
            {
                return;
            }
            DxPriceLevelBook nativeBook;
            unsafe
            {
                nativeBook = *((DxPriceLevelBook*)priceLevelBook);
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
            
            var book = new dxfeed.api.events.DxPriceLevelBook(
                ToString(nativeBook.symbol), bids, asks);

            bookListener.OnChanged(book);
        }

        private unsafe DxPriceLevel GetLevel(IntPtr levels, int index)
        {
            return *((DxPriceLevel*)IntPtr.Add(levels, sizeof(DxPriceLevel) * index));
        }

        private dxfeed.api.events.DxPriceLevelBook.DxPriceLevel CreateLevel(DxPriceLevel level)
        {
            return new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel(
                        level.price, level.size, TimeConverter.ToUtcDateTime(level.time));
        }

        private unsafe string ToString(IntPtr str)
        {
            return new string((char*)str.ToPointer());
        }

        private void OnQuote(IntPtr symbol, IntPtr quote, int count, IntPtr userData)
        {
            var quoteBuf = NativeBufferFactory.CreateQuoteBuf(symbol, quote, count, null);
            quoteListener.OnQuote<NativeEventBuffer<NativeQuote>, NativeQuote>(quoteBuf);
        }

        private bool disposedValue; // To detect redundant calls
        private readonly IDxRegionalBookListener bookListener;
        private readonly IDxQuoteListener quoteListener;
        // ReSharper disable once NotAccessedField.Local
        private readonly C.dxf_regional_quote_listener_t nativeQuoteListener;//to prevent from being garbage collected
        // ReSharper disable once NotAccessedField.Local
        private readonly C.dxf_price_level_book_listener_t nativeBookListener;//to prevent from being garbage collected
        private readonly IntPtr bookHandle;        
    }
}
