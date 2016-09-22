/*
 * QDS - Quick Data Signalling Library
 * Copyright (C) 2002-2015 Devexperts LLC
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at
 * http://mozilla.org/MPL/2.0/.
 */

namespace com.dxfeed.ipf.impl {
    class Constants {
        public const string METADATA_PREFIX = "#";
        public const string METADATA_SUFFIX = "::=TYPE";
        public const string FLUSH_COMMAND = "##";
        public const string COMPLETE_COMMAND = "##COMPLETE";

        public static readonly string LIVE_PROP_KEY = "X-Live";
        public static readonly string LIVE_PROP_REQUEST_NO = "no";
        public static readonly string LIVE_PROP_REQUEST_YES = "yes";
        public static readonly string LIVE_PROP_RESPONSE = "provided";
    }
}
