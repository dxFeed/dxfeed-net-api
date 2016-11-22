/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Type of the price value.
    /// </summary>
    public enum PriceType : sbyte
    {
        /// <summary>
        /// Regular price.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Indicative price (e.g. derived via math formula).
        /// </summary>
        Indicative = 1,

        /// <summary>
        /// Preliminary price (e.g. preliminary settlement price), usually posted prior to "PriceType.Final" price.
        /// </summary>
        Preliminary = 2,

        /// <summary>
        /// Final price (e.g. final settlement price).
        /// </summary>
        Final = 3
    }
}
