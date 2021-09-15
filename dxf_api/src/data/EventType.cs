#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Event types
    /// </summary>
    [Flags]
    public enum EventType
    {
        /// <summary>
        /// None event type
        /// </summary>
        None          = 0,
        /// <summary>
        /// Trade event type
        /// </summary>
        Trade         = (1 << 0),
        /// <summary>
        /// Quote event type
        /// </summary>
        Quote         = (1 << 1),
        /// <summary>
        /// Summary event type
        /// </summary>
        Summary       = (1 << 2),
        /// <summary>
        /// Profile event type
        /// </summary>
        Profile       = (1 << 3),
        /// <summary>
        /// Order event type
        /// </summary>
        Order         = (1 << 4),
        /// <summary>
        /// TimeAndSale event type
        /// </summary>
        TimeAndSale   = (1 << 5),
        /// <summary>
        /// Candle event type
        /// </summary>
        Candle        = (1 << 6),
        /// <summary>
        /// TradeETH event type
        /// </summary>
        TradeETH      = (1 << 7),
        /// <summary>
        /// SpreadOrder event type
        /// </summary>
        SpreadOrder   = (1 << 8),
        /// <summary>
        /// Greeks event type
        /// </summary>
        Greeks        = (1 << 9),
        /// <summary>
        /// TheoPrice event type
        /// </summary>
        TheoPrice     = (1 << 10),
        /// <summary>
        /// Underlying event type
        /// </summary>
        Underlying    = (1 << 11),
        /// <summary>
        /// Series event type
        /// </summary>
        Series        = (1 << 12),
        /// <summary>
        /// Configuration event type
        /// </summary>
        Configuration = (1 << 13)
    }
}
