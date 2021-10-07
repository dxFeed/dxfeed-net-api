#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using static System.String;

namespace com.dxfeed.ipf
{
    /// <summary>
    ///     Defines standard types of {@link InstrumentProfile}. Note that other (unknown) types
    ///     can be used without listing in this class - use it for convenience only.
    ///     Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    public class InstrumentProfileType
    {
        private const string Currency = "CURRENCY";
        private const string Forex = "FOREX";
        private const string Bond = "BOND";
        private const string Index = "INDEX";
        private const string Stock = "STOCK";
        private const string Etf = "ETF";
        private const string MutualFund = "MUTUAL_FUND";
        private const string MoneyMarketFund = "MONEY_MARKET_FUND";
        private const string Product = "PRODUCT";
        private const string Future = "FUTURE";
        private const string Option = "OPTION";
        private const string Spread = "SPREAD";
        private const string Other = "OTHER";
        private const string Removed = "REMOVED";

        private static readonly Dictionary<string, InstrumentProfileType> TypesMap =
            new Dictionary<string, InstrumentProfileType>();

        /// <summary>
        ///     The currency type
        /// </summary>
        public static readonly InstrumentProfileType CURRENCY = new InstrumentProfileType(Currency);

        /// <summary>
        ///     The forex type
        /// </summary>
        public static readonly InstrumentProfileType FOREX = new InstrumentProfileType(Forex);

        /// <summary>
        ///     The bond type
        /// </summary>
        public static readonly InstrumentProfileType BOND = new InstrumentProfileType(Bond);

        /// <summary>
        ///     The index type
        /// </summary>
        public static readonly InstrumentProfileType INDEX = new InstrumentProfileType(Index);

        /// <summary>
        ///     The stock type
        /// </summary>
        public static readonly InstrumentProfileType STOCK = new InstrumentProfileType(Stock);

        /// <summary>
        ///     The ETF type
        /// </summary>
        public static readonly InstrumentProfileType ETF = new InstrumentProfileType(Etf);

        /// <summary>
        ///     The mutual fund type
        /// </summary>
        public static readonly InstrumentProfileType MUTUAL_FUND = new InstrumentProfileType(MutualFund);

        /// <summary>
        ///     The money market fund type
        /// </summary>
        public static readonly InstrumentProfileType MONEY_MARKET_FUND = new InstrumentProfileType(MoneyMarketFund);

        /// <summary>
        ///     The product type
        /// </summary>
        public static readonly InstrumentProfileType PRODUCT = new InstrumentProfileType(Product);

        /// <summary>
        ///     The future type
        /// </summary>
        public static readonly InstrumentProfileType FUTURE = new InstrumentProfileType(Future);

        /// <summary>
        ///     The option type
        /// </summary>
        public static readonly InstrumentProfileType OPTION = new InstrumentProfileType(Option);

        /// <summary>
        ///     The spread type
        /// </summary>
        public static readonly InstrumentProfileType SPREAD = new InstrumentProfileType(Spread);

        /// <summary>
        ///     The other type
        /// </summary>
        public static readonly InstrumentProfileType OTHER = new InstrumentProfileType(Other);

        /// <summary>
        ///     Current instrument was removed
        /// </summary>
        public static readonly InstrumentProfileType REMOVED = new InstrumentProfileType(Removed);

        private InstrumentProfileType(string name)
        {
            Name = name;
            TypesMap[name] = this;
        }

        /// <summary>
        ///     Get the name of instrument profile type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Returns field for specified name or <b>null</b> if field is not found.
        /// </summary>
        /// <param name="name">Name of type.</param>
        /// <returns>Field for specified name or <b>null</b> if field is not found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static InstrumentProfileType Find(string name)
        {
            if (TypesMap.ContainsKey(name))
                return TypesMap[name];
            return null;
        }

        /// <summary>
        ///     Compares two specified types for order. Returns a negative integer, zero, or a positive integer
        ///     as this object is less than, equal to, or greater than the specified object.
        ///     <p />
        ///     Unlike natual ordering of {@link InstrumentProfileType} enum itself this method supports
        ///     unknown types and orders them alphabetically after standard ones.
        ///     <p />
        ///     The natural ordering implied by this method is designed for convenient data representation
        ///     in a file and shall not be used for business purposes.
        /// </summary>
        /// <param name="type1">First type to compare.</param>
        /// <param name="type2">Second type to compare.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static int CompareTypes(string type1, string type2)
        {
            var t1 = Find(type1);
            var t2 = Find(type2);

            if (t1 == null)
                return t2 == null ? Compare(type1, type2, StringComparison.InvariantCulture) : +1;
            if (t2 == null)
                return -1;

            return t1.CompareTo(t2);
        }

        /// <summary>
        ///     Compare this type with other.
        /// </summary>
        /// <param name="other">Other type to compare.</param>
        /// <returns></returns>
        public int CompareTo(InstrumentProfileType other)
        {
            return Compare(Name, other.Name, StringComparison.InvariantCulture);
        }
    }
}