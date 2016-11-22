/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Scope of an order.
    /// </summary>
    public class Scope
    {
        private static List<Scope> scopes = new List<Scope>(4);

        private Scope()
        {
            //Note: hide default constructor
        }

        private Scope(string name, int code)
        {
            Name = name;
            Code = code;
            scopes.Insert(Code, this);
        }

        private string Name { get; set; }
        public int Code { get; private set; }

        /// <summary>
        /// Returns scope object by integer code.
        /// </summary>
        /// <param name="code">The integer value of scope.</param>
        /// <returns>Scope object by integer code.</returns>
        public static Scope ValueOf(int code)
        {
            return scopes[code];
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return GetType().Equals(obj.GetType()) && Code.Equals(((Scope)obj).Code);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Represents best bid or best offer for the whole market.
        /// </summary>
        public static readonly Scope COMPOSITE = new Scope("COMPOSITE", 0);
        /// <summary>
        /// Represents best bid or best offer for a given exchange code.
        /// </summary>
        public static readonly Scope REGIONAL = new Scope("REGIONAL", 1);
        /// <summary>
        /// Represents aggregate information for a given price level or
        /// best bid or best offer for a given market maker.
        /// </summary>
        public static readonly Scope AGGREGATE = new Scope("AGGREGATE", 2);
        /// <summary>
        /// Represents individual order on the market.
        /// </summary>
        public static readonly Scope ORDER = new Scope("ORDER", 3);

    }
}
