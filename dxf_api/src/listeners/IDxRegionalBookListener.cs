#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api
{
    /// <summary>
    /// The Regional Book listener interface
    /// </summary>
    public interface IDxRegionalBookListener
    {
        /// <summary>
        /// On "Book has been changed" event received 
        /// </summary>
        /// <param name="book">The book</param>
        void OnChanged(DxPriceLevelBook book);
    }
}
