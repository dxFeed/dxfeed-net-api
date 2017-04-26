#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Configuration event with application-specific attachment.
    /// </summary>
    public interface IDxConfiguration : IDxMarketEvent
    {
        /// <summary>
        /// Returns attachment.
        /// </summary>
        ICloneable Attachment { get; }
    }
}