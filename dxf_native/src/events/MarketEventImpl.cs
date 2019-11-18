﻿#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Abstract base class for all market events. All market events are objects that
    /// extend this class. Market event classes are simple beans with setters and getters
    /// for their properties and minimal business logic. All market events have EventSymbol
    /// property that is defined by this class.
    /// </summary>
    public abstract class MarketEventImpl : IDxMarketEvent
    {
        /// <summary>
        /// Protected constructor for concrete implementation classes that initializes EventSymbol
        /// property.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        protected MarketEventImpl(string symbol)
        {
            EventSymbol = string.Copy(symbol);
        }

        #region Implementation of IDxMarketEvent

        /// <summary>
        /// Returns symbol of this event.
        /// </summary>
        public string EventSymbol
        {
            get; private set;
        }

        object IDxEventType.EventSymbol
        {
            get
            {
                return EventSymbol;
            }
        }

        #endregion

        public abstract object Clone();
    }
}
