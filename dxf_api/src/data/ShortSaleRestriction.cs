#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Short sale restriction on an instrument.
    /// </summary>
    public enum ShortSaleRestriction : int
    {
        /// <summary>
        /// Short sale restriction is undefined, unknown or inapplicable.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Short sale restriction is active.
        /// </summary>
        Active = 1,
        /// <summary>
        /// Short sale restriction is inactive.
        /// </summary>
        Inactive = 2
    }
}
