#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;
using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Configuration event with application-specific attachment.
    /// </summary>
    public interface IDxConfiguration : IDxMarketEvent
    {
        /// <summary>
        /// Returns version.
        /// </summary>
        int Version { get; }
        /// <summary>
        /// Returns attachment.
        /// </summary>
        string Attachment { get; }
    }
}