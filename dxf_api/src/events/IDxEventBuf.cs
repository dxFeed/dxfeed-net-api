#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System.Collections.Generic;
using com.dxfeed.api.data;

namespace com.dxfeed.api.events
{
    public interface IDxEventBuf<out T> : IEnumerable<T>
    {
        EventType EventType { get; }
        string Symbol { get; }
        int Size { get; }
        EventParams EventParams { get; }
    }
}
