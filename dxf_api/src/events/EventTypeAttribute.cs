#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Attribute used to denote types of events
    /// </summary>
    public class EventTypeAttribute : Attribute
    {
        /// <summary>
        /// Creates an attribute by the event name
        /// </summary>
        /// <param name="eventName">The event name</param>
        public EventTypeAttribute(string eventName) : base()
        {
            EventName = eventName;
        }

        /// <summary>
        /// The event name property
        /// </summary>
        public string EventName { get; private set; }

    }
}
