#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Abstract base class for all market events. All market events are objects that
    /// extend this class. Market event classes are simple beans with setters and getters
    /// for their properties and minimal business logic. All market events have EventSymbol
    /// property that is defined by this class.
    /// </summary>
    public interface IDxMarketEvent : IDxEventType<string>
    {
        //Note: no extra fields
    }
}
