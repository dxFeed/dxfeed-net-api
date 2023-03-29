#region License

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
    /// Interface provides receiving order snapshot events.
    /// </summary>
    public interface IDxIncOrderSnapshotListener : IDxSnapshotListener
    {
        /// <summary>
        /// On Order snapshot event received.
        /// </summary>
        /// <typeparam name="TB">Event buffer type.</typeparam>
        /// <typeparam name="TE">Event type.</typeparam>
        /// <param name="buf">Event buffer object.</param>
        /// <param name="newSnapshot">Snapshot or update.</param>
        void OnOrderSnapshot<TB, TE>(TB buf, bool newSnapshot)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder;
    }
}
