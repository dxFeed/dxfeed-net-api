/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Trading status of an instrument.
    /// </summary>
    public enum TradingStatus : int
    {
        /// <summary>
        /// Trading status is undefined, unknown or inapplicable.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Trading is halted.
        /// </summary>
        Halted = 1,
        /// <summary>
        /// Trading is active.
        /// </summary>
        Active = 2
    }
}
