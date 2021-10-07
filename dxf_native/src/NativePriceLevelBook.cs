#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Linq;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;
using DxPriceLevelBook = com.dxfeed.native.api.DxPriceLevelBook;

namespace com.dxfeed.native
{
    /// <summary>
    /// Wrapper for the formation of the price level book 
    /// </summary>
    public class NativePriceLevelBook : IDxPriceLevelBook
    {
        private static void CheckSources(string[] sources)
        {
            if (sources == null || sources.Length == 0) return;

            foreach (var source in sources)
            {
                var trimmed = source.Trim();

                if (string.IsNullOrEmpty(trimmed)) throw new ArgumentException("Null or empty source");
                if (OrderSource.IsSpecialSourceName(source))
                    throw new ArgumentException("The special order source (AGGREGATE_BID|ASK etc)");
                if (!OrderSource.HasDefaultSourceName(source))
                    throw new ArgumentException($"Unknown order source: '{source}'");
            }
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
        /// <param name="sources">The order sources</param>
        /// <param name="listener">The price level book listener implementation</param>
        /// <exception cref="ArgumentException"></exception>
        public NativePriceLevelBook(NativeConnection connection, string symbol, string[] sources,
            IDxPriceLevelBookListener listener)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Invalid symbol parameter.");
            }

            this.connection = connection;
            bookListener = listener;
            this.symbol = symbol;
            this.sources = sources;

            if (bookListener == null) return;

            if (sources == null)
            {
                C.CheckOk(C.Instance.CreatePriceLevelBook(connection.Handle, symbol, new string[] { }, 0,
                    out bookHandle));
            }
            else
            {
                CheckSources(sources);
                C.CheckOk(C.Instance.CreatePriceLevelBook(connection.Handle, symbol, sources, sources.Length,
                    out bookHandle));
            }

            try
            {
                C.CheckOk(C.Instance.AttachPriceLevelBookListener(bookHandle, nativeBookListener = OnBook,
                    IntPtr.Zero));
            }
            catch (DxException)
            {
                C.Instance.ClosePriceLevelBook(bookHandle);
                throw;
            }
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
        /// <param name="sources">The order sources</param>
        /// <param name="listener">The price level book listener implementation</param>
        /// <exception cref="ArgumentException"></exception>
        public NativePriceLevelBook(NativeConnection connection, string symbol, OrderSource[] sources,
            IDxPriceLevelBookListener listener) :
            this(connection, symbol, sources?.Select(s => s.ToString()).ToArray(), listener)
        {
        }

        /// <summary>
        /// Creates the new price level book instance for the specified symbol and all known sources
        /// </summary>
        /// <remarks>
        ///     Don't call this constructor inside any listeners and callbacks of NativeSubscription, NativeConnection,
        /// NativeRegionalBook, NativePriceLevelBook, NativeSnapshotSubscription classes
        /// </remarks>
        /// <param name="connection">The current connection</param>
        /// <param name="symbol">The book symbol</param>
        /// <param name="listener">The price level book listener implementation</param>
        /// <exception cref="ArgumentException"></exception>
        public NativePriceLevelBook(NativeConnection connection, string symbol, IDxPriceLevelBookListener listener) :
            this(connection, symbol, new OrderSource[] { }, listener)
        {
        }

        /// <summary>
        ///     This code added to correctly implement the disposable pattern.
        /// </summary>
        /// <param name="disposing">The disposing flag</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (nativeBookListener != null)
            {
                C.Instance.DetachPriceLevelBookListener(bookHandle, nativeBookListener);
            }

            C.Instance.ClosePriceLevelBook(bookHandle);
            connection = null;
            disposedValue = true;
        }

        /// <inheritdoc />
        ~NativePriceLevelBook()
        {
            Dispose(false);
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

        /// <inheritdoc />
        public void SetSymbol(string newSymbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Invalid symbol parameter.");
            }
            
            if (nativeBookListener != null)
            {
                C.Instance.DetachPriceLevelBookListener(bookHandle, nativeBookListener);
            }

            C.Instance.ClosePriceLevelBook(bookHandle);

            symbol = newSymbol;
            C.CheckOk(C.Instance.CreatePriceLevelBook(connection.Handle, symbol, sources, sources.Length,
                out bookHandle));

            try
            {
                if (bookListener != null)
                {
                    C.CheckOk(C.Instance.AttachPriceLevelBookListener(bookHandle, nativeBookListener = OnBook,
                        IntPtr.Zero));
                }
            }
            catch (DxException)
            {
                C.Instance.ClosePriceLevelBook(bookHandle);
                throw;
            }
        }

        /// <inheritdoc />
        public void SetSources(string[] newSources)
        {
            if (nativeBookListener != null)
            {
                C.Instance.DetachPriceLevelBookListener(bookHandle, nativeBookListener);
            }
            
            if (bookListener == null) return;

            sources = newSources;

            C.Instance.ClosePriceLevelBook(bookHandle);
            
            if (sources == null)
            {
                C.CheckOk(C.Instance.CreatePriceLevelBook(connection.Handle, symbol, new string[] { }, 0,
                    out bookHandle));
            }
            else
            {
                CheckSources(sources);
                C.CheckOk(C.Instance.CreatePriceLevelBook(connection.Handle, symbol, sources, sources.Length,
                    out bookHandle));
            }
            
            try
            {
                C.CheckOk(C.Instance.AttachPriceLevelBookListener(bookHandle, nativeBookListener = OnBook,
                    IntPtr.Zero));
            }
            catch (DxException)
            {
                C.Instance.ClosePriceLevelBook(bookHandle);
                throw;
            }
        }

        /// <inheritdoc />
        public void SetSources(OrderSource[] newSources)
        {
            SetSources(sources?.Select(s => s.ToString()).ToArray());
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

            var book = new dxfeed.api.events.DxPriceLevelBook(
                ToString(nativeBook.symbol), bids, asks);

            bookListener.OnChanged(book);
        }

        private static unsafe DxPriceLevel GetLevel(IntPtr levels, int index)
        {
            return *(DxPriceLevel*)IntPtr.Add(levels, sizeof(DxPriceLevel) * index);
        }

        private dxfeed.api.events.DxPriceLevelBook.DxPriceLevel CreateLevel(DxPriceLevel level)
        {
            return new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel(
                level.price, level.size, TimeConverter.ToUtcDateTime(level.time));
        }

        private static unsafe string ToString(IntPtr str)
        {
            return new string((char*)str.ToPointer());
        }

        private bool disposedValue; // To detect redundant calls
        private readonly IDxPriceLevelBookListener bookListener;
        private string symbol;
        private string[] sources;

        // ReSharper disable once NotAccessedField.Local
        private C.dxf_price_level_book_listener_t nativeBookListener; //to prevent from being garbage collected
        private IntPtr bookHandle;
        private NativeConnection connection;
    }
}