using System;
using com.dxfeed.api;
using com.dxfeed.api.extras;
using com.dxfeed.native.api;
using com.dxfeed.native.events;

namespace com.dxfeed.native
{
    public class NativeRegionalBook : IDxRegionalBook
    {
        public NativeRegionalBook(NativeConnection con, string symbol, IDxRegionalBookListener book_listener,
            IDxQuoteListener quote_listener)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Invalid symbol parameter.");
            }
            this.symbol = symbol;
            this.book_listener = book_listener;
            this.quote_listener = quote_listener;

            if (book_listener != null || quote_listener != null)
            {
                C.CheckOk(C.Instance.dxf_create_regional_book(con.Handler, this.symbol, out book));
                try
                {
                    if (this.book_listener != null)
                    {
                        C.CheckOk(C.Instance.dxf_attach_regional_book_listener(book, native_book_listener = OnBook, IntPtr.Zero));
                    }
                    if (this.quote_listener != null)
                    {
                        C.CheckOk(C.Instance.dxf_attach_regional_book_listener_v2(book, native_quote_listener = OnQuote, IntPtr.Zero));
                    }
                }
                catch (DxException)
                {
                    C.Instance.dxf_close_regional_book(book);
                    throw;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                if (book != null)
                {
                    C.Instance.dxf_close_regional_book(book);
                }
                disposedValue = true;
            }
        }

        ~NativeRegionalBook() {
           Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnBook(IntPtr price_level_book, IntPtr user_data)
        {
            if (price_level_book == IntPtr.Zero)
            {
                return;
            }
            DxPriceLevelBook native_book;
            unsafe
            {
                native_book = *((DxPriceLevelBook*)price_level_book);
            }
            dxfeed.api.events.DxPriceLevelBook.DxPriceLevel[] asks =
                new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel[native_book.asks_count];
            for (int i = 0; i < native_book.asks_count; ++i)
            {
                asks[i] = CreateLevel(GetLevel(native_book.asks, i));
            }
            dxfeed.api.events.DxPriceLevelBook.DxPriceLevel[] bids =
                new dxfeed.api.events.DxPriceLevelBook.DxPriceLevel[native_book.bids_count];
            for (int i = 0; i < native_book.bids_count; ++i)
            {
                bids[i] = CreateLevel(GetLevel(native_book.bids, i));
            }
            dxfeed.api.events.DxPriceLevelBook book = new dxfeed.api.events.DxPriceLevelBook(
                ToString(native_book.symbol), bids, asks);

            book_listener.OnChanged(book);
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

        private void OnQuote(IntPtr symbol, IntPtr quote, int count, IntPtr user_data)
        {
            var quoteBuf = NativeBufferFactory.CreateQuoteBuf(symbol, quote, count, null);
            quote_listener.OnQuote<NativeEventBuffer<NativeQuote>, NativeQuote>(quoteBuf);
        }

        private bool disposedValue = false; // To detect redundant calls
        private readonly string symbol;
        private readonly IDxRegionalBookListener book_listener;
        private readonly IDxQuoteListener quote_listener;
        private readonly C.dxf_regional_quote_listener_t native_quote_listener;//to prevent from being garbage collected
        private readonly C.dxf_price_level_book_listener_t native_book_listener;//to prevent from being garbage collected
        private readonly IntPtr book;        
    }
}
