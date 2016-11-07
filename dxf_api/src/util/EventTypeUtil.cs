/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.api.events;
using System;

namespace com.dxfeed.api.util
{
    /// <summary>
    /// Class provides operations with EventType.
    /// </summary>
    public class EventTypeUtil
    {
        /// <summary>
        /// Convert event type to event id, accepting by C API.
        /// Warning: eventType must contain only one event flag, otherwise exception will be raised.
        /// </summary>
        /// <param name="eventType">Type of event to convert.</param>
        /// <exception cref="InvalidOperationException">Event type is empty or contains several flags.</exception>
        /// <returns>Event id, accepting by C API.</returns>
        public static int GetEventId(EventType eventType)
        {            
            uint eventTypeValue = (uint)eventType;
            if (eventType == 0)
                throw new InvalidOperationException("Empty event type.");
            int id = 0;
            while ((eventTypeValue & 0x1) == 0)
            {
                id++;
                eventTypeValue >>= 1;
            }

            if (eventTypeValue > 1)
                throw new InvalidOperationException("Event type has several flags.");

            return id;
        }
    }
}
