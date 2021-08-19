#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api
{
    public interface IDxPriceLevelBook : IDisposable
    {
        /// <summary>
        /// Sets the price level book symbol
        /// </summary>
        /// <param name="newSymbol">The new symbol</param>
        void SetSymbol(string newSymbol);

        /// <summary>
        /// Sets the price level book sources
        /// </summary>
        /// <param name="newSources">The new sources. null or empty array is equivalent to all sources.</param>
        void SetSources(string[] newSources);

        /// <summary>
        /// Sets the price level book sources
        /// </summary>
        /// <param name="newSources">The new sources. null or empty array is equivalent to all sources.</param>
        void SetSources(OrderSource[] newSources);
    }
}
