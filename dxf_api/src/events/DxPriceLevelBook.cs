#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Class describing the price level book
    /// </summary>
    public struct DxPriceLevelBook
    {
        
        /// <summary>
        /// The book's aggregation level 
        /// </summary>
        public struct DxPriceLevel
        {
            /// <summary>
            /// The level's price value
            /// </summary>
            public double Price { get; }
            
            /// <summary>
            /// The level's number of elements with current price
            /// </summary>
            public double Size { get; }
            
            /// <summary>
            /// The level's time
            /// </summary>
            public DateTime Time { get; }

            /// <summary>
            /// Creates the price level by price, size and time
            /// </summary>
            /// <param name="price">The level price</param>
            /// <param name="size">The number of elements of the same price</param>
            /// <param name="time">The level time</param>
            public DxPriceLevel(double price, double size, DateTime time)
            {
                Price = price;
                Size = size;
                Time = time;
            }
        }

        /// <summary>
        /// The price level book's symbol
        /// </summary>
        public string Symbol { get; }
        
        /// <summary>
        /// The array of bid levels of the book
        /// </summary>
        public DxPriceLevel[] Bids { get; }
        
        /// <summary>
        /// The array of ask levels of the book
        /// </summary>
        public DxPriceLevel[] Asks { get; }

        /// <summary>
        /// Creates a price level book by symbol, bids and asks
        /// </summary>
        /// <param name="symbol">The price level book symbol</param>
        /// <param name="bids">The book's bid levels</param>
        /// <param name="asks">The book's ask levels</param>
        public DxPriceLevelBook(string symbol, DxPriceLevel[] bids, DxPriceLevel[] asks)
        {
            Symbol = symbol;
            Bids = bids;
            Asks = asks;
        }
    }
}
