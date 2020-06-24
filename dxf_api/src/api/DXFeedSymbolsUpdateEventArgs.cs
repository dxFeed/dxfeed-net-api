#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace com.dxfeed.api
{
    /// <summary>
    ///     Represents class contains data for symbols update events.
    /// </summary>
    public class DXFeedSymbolsUpdateEventArgs : EventArgs
    {
        public DXFeedSymbolsUpdateEventArgs(ISet<object> symbols) : base()
        {
            Symbols = symbols;
        }

        public DXFeedSymbolsUpdateEventArgs(ICollection<object> symbols) : this(symbols.ToArray()) { }

        public DXFeedSymbolsUpdateEventArgs(params object[] symbols) : base()
        {
            Symbols = new HashSet<object>();
            symbols.All(s => Symbols.Add(s));
        }

        /// <summary>
        ///     Gets symbols that was been added or removed via update event.
        /// </summary>
        public ISet<object> Symbols
        {
            get; private set;
        }

    }
}
