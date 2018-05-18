/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Direction of a trade.
    /// </summary>
    public enum Direction : int
    {
        /// <summary>
        /// Direction is undefined, unknown or inapplicable.
        /// It includes cases with undefined price value or when direction computation was not performed.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Current price is lower than previous price.
        /// </summary>
        Down = 0,
        /// <summary>
        /// Current price is the same as previous price and is lower than the last known price of different value.
        /// </summary>
        ZeroDown = 1,
        /// <summary>
        /// Current price is equal to the only known price value suitable for price direction computation.
        /// </summary>
        Zero = 3,
        /// <summary>
        /// Current price is the same as previous price and is higher than the last known price of different value.
        /// </summary>
        ZeroUp = 4,
        /// <summary>
        /// Current price is higher than previous price.
        /// </summary>
        Up = 5
    }
}
