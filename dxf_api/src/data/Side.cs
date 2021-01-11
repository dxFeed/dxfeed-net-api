#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Side of an order or a trade.
    /// </summary>
    public enum Side : int
    {
        /// <summary>
        /// Undefined side.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Buy side (bid).
        /// </summary>
        Buy = 1,
        /// <summary>
        /// Sell side (ask or offer).
        /// </summary>
        Sell = 2
    }
}
