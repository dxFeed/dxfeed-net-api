﻿#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;

namespace com.dxfeed.api
{
    /// <summary>
    /// Interface provides receiving Greeks events.
    /// </summary>
    public interface IDxGreeksListener : IDxEventListener
    {
        /// <summary>
        /// On Greeks event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks;
    }
}
