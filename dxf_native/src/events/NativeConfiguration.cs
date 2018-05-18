#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.events;
using com.dxfeed.api.data;
using com.dxfeed.native.api;
using System;
using System.Globalization;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Configuration event with application-specific attachment.
    /// </summary>
    public class NativeConfiguration : MarketEventImpl, IDxConfiguration
    {
        internal unsafe NativeConfiguration(DxConfiguration* configuration, string symbol) : base(symbol)
        {
            DxConfiguration c = *configuration;
            Version = c.version;
            Attachment = new string((char*)c.string_object.ToPointer());
        }

        internal NativeConfiguration(IDxConfiguration c) : base(c.EventSymbol)
        {
            Version = c.Version;
            Attachment = c.Attachment;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Configuration: {{{0}, "          +
                "Version: {1}, Attachment: '{2}'" +
                "}}",
                EventSymbol,
                Version, Attachment
            );
        }

        #region Implementation of ICloneable

        public override object Clone()
        {
            return new NativeConfiguration(this);
        }

        #endregion

        #region Implementation of IDxConfiguration

        /// Returns version.
        /// </summary>
        public int Version { get; private set; }
        /// <summary>
        /// Returns attachment.
        /// </summary>
        public string Attachment { get; private set; }

        #endregion

    }
}