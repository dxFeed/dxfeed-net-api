#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.events;
using com.dxfeed.api.plb;

namespace com.dxfeed.api
{
    /// <summary>
    /// Wrapper interface for the formation of the price level book
    /// </summary>
    public interface IPriceLevelBook : IDisposable
    {
        /// <summary>
        /// Sets the PLB handlers
        /// </summary>
        /// <param name="newPriceLevelBookHandler">The handler that will be called when a new book is created
        /// (for example, when trading starts)</param>
        /// <param name="priceLevelBookUpdateHandler">The listener that will be called when the book is updated. In
        /// this case, all price levels will be passed (taking into account the maximum number of price levels)</param>
        /// <param name="priceLevelBookIncChangeHandler">The listener that will be called on incremental updates. All
        /// deletions, additions, and level updates will be passed.</param>
        void SetHandlers(IDxOnNewPriceLevelBookHandler newPriceLevelBookHandler,
            IDxOnPriceLevelBookUpdateHandler priceLevelBookUpdateHandler,
            IDxOnPriceLevelBookIncChangeHandler priceLevelBookIncChangeHandler);
    }
}