#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Price type enumeration
    /// </summary>
    public enum PriceType
    {
        /// <summary>
        /// Regular price.
        /// </summary>
        Regular = 0,
        /// <summary>
        /// Indicative price (derived via math formula).
        /// </summary>
        Indicative = 1,
        /// <summary>
        /// Preliminary price (preliminary settlement price), usually posted prior to Final price.
        /// </summary>
        Preliminary = 2,
        /// <summary>
        /// Final price (final settlement price).
        /// </summary>
        Final = 3
    }
}
