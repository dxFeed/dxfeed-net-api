#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System.Collections.Generic;

namespace com.dxfeed.api
{
    /// <summary>
    ///     The listener interface for receiving events of the specified type E.
    /// </summary>
    /// <typeparam name="E">The type of events.</typeparam>
    public interface IDXFeedEventListener<E>
    {
        /// <summary>
        ///     Invoked when events of type E are received.
        /// </summary>
        /// <param name="events">The list of received events.</param>
        void EventsReceived(IList<E> events);
    }
}
