#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.Collections.Generic;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// The interface that describes the event buffer
    /// </summary>
    /// <typeparam name="T">The event class</typeparam>
    public interface IDxEventBuf<out T> : IEnumerable<T>
    {
        /// <summary>
        /// The event type
        /// </summary>
        EventType EventType { get; }
        
        /// <summary>
        /// The event symbol
        /// </summary>
        string Symbol { get; }
        
        /// <summary>
        /// The buffer size
        /// </summary>
        int Size { get; }
        
        /// <summary>
        /// The buffer's event params
        /// </summary>
        EventParams EventParams { get; }
    }
}
