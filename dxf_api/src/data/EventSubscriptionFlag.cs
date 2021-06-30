#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Event subscription flags. Used to change subscription behaviour
    /// </summary>
    [Flags]
    public enum EventSubscriptionFlag
    {
        /// <summary>
        /// Used for default subscription
        /// 
        /// Integer value = 0x0
        /// </summary>
        Default = 0x0,
        
        /// <summary>
        /// Used for forcing subscription to ticker data
        /// 
        /// Integer value = 0x20
        /// </summary>
        ForceTicker = 0x20,
        
        /// <summary>
        /// Used for forcing subscription to stream data
        /// 
        /// Integer value = 0x40
        /// </summary>
        ForceStream = 0x40,

        /// <summary>
        /// Used for forcing subscription to history data
        /// 
        /// Integer value = 0x80
        /// </summary>
        ForceHistory = 0x80
    }
}