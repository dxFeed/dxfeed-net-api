/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.
using System.Globalization;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    /// Configuration event with application-specific attachment.
    /// </summary>
    public class NativeConfiguration : MarketEvent, IDxConfiguration
    {
        private readonly DxConfiguration configuration;
        private readonly DxString stringObject;

        internal unsafe NativeConfiguration(DxConfiguration* configuration, string symbol) : base(symbol)
        {
            this.configuration = *configuration;
            stringObject = DxMarshal.ReadDxString(this.configuration.string_object);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Configuration: {{{0}, Object: '{1}'}}",
                EventSymbol, Attachment);
        }

        #region Implementation of IDxOrder

        /// <summary>
        /// Returns attachment.
        /// </summary>
        public object Attachment
        {
            get { return stringObject.ToString(); }
        }

        #endregion
    }
}