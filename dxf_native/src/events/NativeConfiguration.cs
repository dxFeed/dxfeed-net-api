#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.events;
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
            Attachment = DxMarshal.ReadDxString(c.string_object).ToString();
        }

        internal NativeConfiguration(IDxConfiguration configuration) : base(configuration.EventSymbol)
        {
            Attachment = (ICloneable)configuration.Attachment.Clone();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Configuration: {{{0}, Object: '{1}'}}",
                EventSymbol, Attachment);
        }

        #region Implementation of ICloneable

        public override object Clone()
        {
            return new NativeConfiguration(this);
        }

        #endregion

        #region Implementation of IDxConfiguration

        /// <summary>
        /// Returns attachment.
        /// </summary>
        public ICloneable Attachment
        {
            get; private set;
        }

        #endregion

    }
}