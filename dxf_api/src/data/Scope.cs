#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Scope of an order.
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// Represents best bid or best offer for the whole market.
        /// </summary>
        Composite = 0,
        /// <summary>
        /// Represents best bid or best offer for a given exchange code.
        /// </summary>
        Regional = 1,
        /// <summary>
        /// Represents aggregate information for a given price level or
	    /// best bid or best offer for a given market maker.
        /// </summary>
        Aggregate = 2,
        /// <summary>
        /// Represents individual order on the market.
        /// </summary>
        Order = 3
    }
}
