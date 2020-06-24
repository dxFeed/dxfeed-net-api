#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.connection
{
    public enum ConnectionStatus : int {
        NotConnected = 0,
        Connected = 1,
        LoginRequired = 2,
        Authorized = 3
    }
}