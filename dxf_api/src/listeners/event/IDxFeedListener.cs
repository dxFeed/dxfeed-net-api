#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api
{
    /// <summary>
    /// Interface provides receiving events
    /// </summary>
    public interface IDxFeedListener :
        IDxQuoteListener,
        IDxTradeListener,
        IDxOrderListener,
        IDxProfileListener,
        IDxFundamentalListener,
        IDxTimeAndSaleListener
    {
        //Note: no extra methods in this interface.
    }
}
