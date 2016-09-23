/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Interface class for all market events. All market events are plain objects that
    /// extend this class. Market event classes are simple beans with setter and getter methods for their
    /// properties and minimal business logic.
    /// </summary>
    public interface IDxMarketEvent
    {

        /// <summary>
        /// Returns symbol of this event.
        /// </summary>
        string Symbol { get; }
    }
}
