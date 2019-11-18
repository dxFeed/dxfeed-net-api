#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Type of a time and sale event.
    /// </summary>
    public enum TimeAndSaleType : int
    {
        /// <summary>
        /// Represents new time and sale event.
        /// </summary>
        New = 0,
        /// <summary>
        /// Represents correction time and sale event.
        /// </summary>
        Correction = 1,
        /// <summary>
        /// Represents cancel time and sale event.
        /// </summary>
        Cancel = 2
    }
}
