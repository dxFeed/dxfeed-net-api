#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.Globalization;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events
{
    /// <summary>
    ///     Configuration event with application-specific attachment.
    /// </summary>
    public class NativeConfiguration : MarketEventImpl, IDxConfiguration
    {
        internal unsafe NativeConfiguration(DxConfiguration* configuration, string symbol) : base(symbol)
        {
            var c = *configuration;
            Version = c.version;
            Attachment = new string((char*)c.string_object.ToPointer());
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="c">The original Configuration event</param>
        public NativeConfiguration(IDxConfiguration c) : base(c.EventSymbol)
        {
            Version = c.Version;
            Attachment = c.Attachment;
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        public NativeConfiguration()
        {
        }

        #region Implementation of ICloneable

        /// <inheritdoc />
        public override object Clone()
        {
            return new NativeConfiguration(this);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Configuration: {{{0}, " +
                "Version: {1}, Attachment: '{2}'" +
                "}}",
                EventSymbol,
                Version, Attachment
            );
        }

        #region Implementation of IDxConfiguration

        /// <summary>
        ///     Returns version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        ///     Returns attachment.
        /// </summary>
        public string Attachment { get; set; }

        #endregion
    }
}