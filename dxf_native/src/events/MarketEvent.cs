/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.api.events;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Abstract base class for all market events. All market events are plain java objects that
    /// extend this class. Market event classes are simple beans with setter and getter methods 
    /// for their properties and minimal business logic. All market events have Symbol property 
    /// that is defined by this class.
    /// </summary>
    public abstract class MarketEvent : IDxMarketEvent
    {
        /// <summary>
        /// Protected constructor for concrete implementation classes that initializes Symbol 
        /// property.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        protected MarketEvent(string symbol)
        {
            Symbol = symbol;
        }

        #region Implementation of IDxMarketEvent

        /// <summary>
        /// Returns symbol of this event.
        /// </summary>
        public string Symbol
        {
            get; private set;
        }

        #endregion
    }
}
