#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.events;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Stores data for one incoming event.
    /// </summary>
    public interface IPlayedEvent
    {
        EventParams Params { get; }
        object Data { get; }
    }

    /// <summary>
    ///     Stores data for one incoming event.
    /// </summary>
    /// <typeparam name="NE">Native event data, e.g. <see cref="DxTestOrder"/></typeparam>
    public interface IPlayedEvent<NE> : IPlayedEvent
    {
        new NE Data { get; }
    }
}
