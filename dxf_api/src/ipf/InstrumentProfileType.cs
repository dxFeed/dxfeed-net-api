/*
 * QDS - Quick Data Signalling Library
 * Copyright (C) 2002-2015 Devexperts LLC
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at
 * http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;

namespace com.dxfeed.ipf {

    /// <summary>
    /// Defines standard types of {@link InstrumentProfile}. Note that other (unknown) types
    /// can be used without listing in this class - use it for convenience only.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    class InstrumentProfileType {
        
        public static readonly InstrumentProfileType CURRENCY = new InstrumentProfileType("CURRENCY");
        public static readonly InstrumentProfileType FOREX = new InstrumentProfileType("FOREX");
        public static readonly InstrumentProfileType BOND = new InstrumentProfileType("BOND");
        public static readonly InstrumentProfileType INDEX = new InstrumentProfileType("INDEX");
        public static readonly InstrumentProfileType STOCK = new InstrumentProfileType("STOCK");
        public static readonly InstrumentProfileType ETF = new InstrumentProfileType("ETF");
        public static readonly InstrumentProfileType MUTUAL_FUND = new InstrumentProfileType("MUTUAL_FUND");
        public static readonly InstrumentProfileType MONEY_MARKET_FUND = new InstrumentProfileType("MONEY_MARKET_FUND");
        public static readonly InstrumentProfileType PRODUCT = new InstrumentProfileType("PRODUCT");
        public static readonly InstrumentProfileType FUTURE = new InstrumentProfileType("FUTURE");
        public static readonly InstrumentProfileType OPTION = new InstrumentProfileType("OPTION");
        public static readonly InstrumentProfileType SPREAD = new InstrumentProfileType("SPREAD");
        public static readonly InstrumentProfileType OTHER = new InstrumentProfileType("OTHER");
        public static readonly InstrumentProfileType REMOVED = new InstrumentProfileType("REMOVED");

        private const string T_CURRENCY = "CURRENCY";
        private const string T_FOREX = "FOREX";
        private const string T_BOND = "BOND";
        private const string T_INDEX = "INDEX";
        private const string T_STOCK = "STOCK";
        private const string T_ETF = "ETF";
        private const string T_MUTUAL_FUND = "MUTUAL_FUND";
        private const string T_MONEY_MARKET_FUND = "MONEY_MARKET_FUND";
        private const string T_PRODUCT = "PRODUCT";
        private const string T_FUTURE = "FUTURE";
        private const string T_OPTION = "OPTION";
        private const string T_SPREAD = "SPREAD";
        private const string T_OTHER = "OTHER";
        private const string T_REMOVED = "REMOVED";
        private static Dictionary<string, InstrumentProfileType> MAP = new Dictionary<string, InstrumentProfileType>();

        private string type;

        private InstrumentProfileType(string type) {
            this.type = type;
            MAP[type] = this;
        }

        /// <summary>
        /// Returns field for specified name or <b>null</b> if field is not found.
        /// </summary>
        /// <param name="name">Name of type.</param>
        /// <returns>Field for specified name or <b>null</b> if field is not found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static InstrumentProfileType find(string name) {
            if (MAP.ContainsKey(name))
                return MAP[name];
            return null;
        }

        /// <summary>
        /// Compares two specified types for order. Returns a negative integer, zero, or a positive integer
        /// as this object is less than, equal to, or greater than the specified object.
        /// <p>
        /// Unlike natual ordering of {@link InstrumentProfileType} enum itself this method supports
        /// unknown types and orders them alphabetically after standard ones.
        /// <p>
        /// The natural ordering implied by this method is designed for convenient data representation
        /// in a file and shall not be used for business purposes.
        /// </summary>
        /// <param name="type1">First type to compare.</param>
        /// <param name="type2">Second type to compare.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int CompareTypes(string type1, string type2) {
            InstrumentProfileType t1 = find(type1);
            InstrumentProfileType t2 = find(type2);
            if (t1 == null)
                return t2 == null ? type1.CompareTo(type2) : +1;
            if (t2 == null)
                return -1;
            return t1.CompareTo(t2);
        }

        /// <summary>
        /// Compare this type with other.
        /// </summary>
        /// <param name="other">Other type to compare.</param>
        /// <returns></returns>
        public int CompareTo(InstrumentProfileType other) {
            return this.type.CompareTo(other.type);
        }
    }
}
