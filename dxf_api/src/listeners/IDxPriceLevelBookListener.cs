#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;

namespace com.dxfeed.api
{
    /// <summary>
    /// The PLB listener interface
    /// </summary>
    public interface IDxPriceLevelBookListener
    {
        /// <summary>
        /// On "PLB has been changed" event received
        /// </summary>
        /// <param name="book">The PLB</param>
        void OnChanged(DxPriceLevelBook book);
    }
}
