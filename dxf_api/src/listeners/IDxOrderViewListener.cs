#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api
{
    /// <summary>
    /// Interface provides receiving Order View events.
    /// </summary>
    [Obsolete("IDxOrderViewListener interface is deprecated and will removed in 9.0.0 version. Please use NativeConnection\\NativeSubscription")]
    public interface IDxOrderViewListener
    {
        /// <summary>
        /// On snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        void OnSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder;

        /// <summary>
        /// On event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        void OnUpdate<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder;
    }
}
