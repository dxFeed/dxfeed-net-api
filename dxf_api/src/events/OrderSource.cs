#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// <para>
    ///     Identifies source of <see cref="IDxOrder"/> and <see cref="IDxSpreadOrder"/> events.
    ///     There are the following kinds of order sources:
    /// </para>
    /// <para>
    ///     Synthetic sources COMPOSITE_BID, COMPOSITE_ASK, REGIONAL_BID, and REGIONAL_ASK are
    ///     provided for convenience of a consolidated order book and are automatically generated
    ///     based on the corresponding Quote events.
    /// </para>
    /// <para>
    ///     Aggregate sources AGGREGATE_BID and AGGREGATE_ASK provide futures depth (aggregated by
    ///     price level) and NASDAQ Level II (top of book for each market maker).
    /// </para>
    /// </summary>
    public class OrderSource : IndexedEventSource, IComparable
    {
        private static Dictionary<int, OrderSource> sourcesById = new Dictionary<int, OrderSource>();
        private static Dictionary<string, OrderSource> sourcesByName = new Dictionary<string, OrderSource>();

        /// <summary>
        ///     Bid side of a composite <see cref="IDxQuote"/>.
        ///     It is a synthetic source.
        ///     The subscription on composite <see cref="IDxQuote"/> event is observed when this
        ///     source is subscribed to.
        /// </summary>
        public static readonly OrderSource COMPOSITE_BID = new OrderSource(1, "COMPOSITE_BID");

        /// <summary>
        ///     Ask side of a composite <see cref="IDxQuote"/>.
        ///     It is a synthetic source.
        ///     The subscription on composite <see cref="IDxQuote"/> event is observed when this
        ///     source is subscribed to.
        /// </summary>
        public static readonly OrderSource COMPOSITE_ASK = new OrderSource(2, "COMPOSITE_ASK");

        /// <summary>
        ///     Bid side of a regional <see cref="IDxQuote"/>.
        ///     It is a synthetic source.
        ///     The subscription on regional <see cref="IDxQuote"/> event is observed when this
        ///     source is subscribed to.
        /// </summary>
        public static readonly OrderSource REGIONAL_BID = new OrderSource(3, "REGIONAL_BID");

        /// <summary>
        ///     Ask side of a regional <see cref="IDxQuote"/>.
        ///     It is a synthetic source.
        ///     The subscription on regional <see cref="IDxQuote"/> event is observed when this
        ///     source is subscribed to.
        /// </summary>
        public static readonly OrderSource REGIONAL_ASK = new OrderSource(4, "REGIONAL_ASK");

        /// <summary>
        ///     Bid side of an aggregate order book (futures depth and NASDAQ Level II).
        /// </summary>
        public static readonly OrderSource AGGREGATE_BID = new OrderSource(5, "AGGREGATE_BID");

        /// <summary>
        ///     Ask side of an aggregate order book (futures depth and NASDAQ Level II).
        /// </summary>
        public static readonly OrderSource AGGREGATE_ASK = new OrderSource(6, "AGGREGATE_ASK");

        /// <summary>
        ///     Empty order source.
        /// </summary>
        public static readonly OrderSource EMPTY = new OrderSource(7, string.Empty);

        /// <summary>
        /// NASDAQ Total View.
        /// </summary>
        public static readonly OrderSource NTV = new OrderSource("NTV");

        /// <summary>
        /// NASDAQ Futures Exchange.
        /// </summary>
        public static readonly OrderSource NFX = new OrderSource("NFX");

        /// <summary>
        /// NASDAQ eSpeed.
        /// </summary>
        public static readonly OrderSource ESPD = new OrderSource("ESPD");

        /// <summary>
        /// Intercontinental Exchange.
        /// </summary>
        public static readonly OrderSource ICE = new OrderSource("ICE");

        /// <summary>
        /// International Securities Exchange.
        /// </summary>
        public static readonly OrderSource ISE = new OrderSource("ISE");

        /// <summary>
        /// Direct-Edge EDGA Exchange.
        /// </summary>
        public static readonly OrderSource DEA = new OrderSource("DEA");

        /// <summary>
        /// Direct-Edge EDGX Exchange.
        /// </summary>
        public static readonly OrderSource DEX = new OrderSource("DEX");

        /// <summary>
        /// Bats BYX Exchange.
        /// </summary>
        public static readonly OrderSource BYX = new OrderSource("BYX");

        /// <summary>
        /// Bats BZX Exchange.
        /// </summary>
        public static readonly OrderSource BZX = new OrderSource("BZX");

        /// <summary>
        /// Bats Europe BXE Exchange.
        /// </summary>
        public static readonly OrderSource BATE = new OrderSource("BATE");

        /// <summary>
        /// Bats Europe CXE Exchange.
        /// </summary>
        public static readonly OrderSource CHIX = new OrderSource("CHIX");

        /// <summary>
        /// Bats Europe TRF.
        /// </summary>
        public static readonly OrderSource BXTR = new OrderSource("BXTR");

        /// <summary>
        /// Borsa Istanbul Exchange.
        /// </summary>
        public static readonly OrderSource IST = new OrderSource("IST");

        /// <summary>
        /// CME Globex.
        /// </summary>
        public static readonly OrderSource GLBX = new OrderSource("GLBX");

        /// <summary>
        /// Eurex Exchange.
        /// </summary>
        public static readonly OrderSource XEUR = new OrderSource("XEUR");

        /// <summary>
        /// CBOE Futures Exchange.
        /// </summary>
        public static readonly OrderSource CFE = new OrderSource("CFE");

        /// <summary>
        ///     Create a new order source
        /// </summary>
        /// <param name="id">Id of the new order source.</param>
        /// <param name="name">Name of the new order source.</param>
        private OrderSource(int id, string name) : base(id, name)
        {
            if (sourcesById.ContainsKey(id))
                throw new ArgumentException(string.Format("The source id '{0}' is already exist.", id));
            if (sourcesByName.ContainsKey(name))
                throw new ArgumentException(string.Format("The source name '{0}' is already exist.", name));
            Id = id;
            Name = name;
            sourcesById[Id] = this;
            sourcesByName[Name] = this;
        }

        /// <summary>
        ///     For transient objects only (statically unknown source id)
        /// </summary>
        /// <param name="id">Id of the new order source.</param>
        private OrderSource(int id) : this(id, DecodeName(id)) { }

        /// <summary>
        ///     For transient objects only (statically unknown source id)
        /// </summary>
        /// <param name="name">Name of the new order source.</param>
        private OrderSource(string name) : this(ComposeId(name), name) { }

        /// <summary>
        ///     Returns order source for the specified source identifier.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <returns>Order source.</returns>
        /// <exception cref="ArgumentException">If sourceId is negative or exceeds MAX_SOURCE_ID.</exception>
        public static OrderSource ValueOf(int sourceId)
        {
            if (sourcesById.ContainsKey(sourceId))
                return sourcesById[sourceId];
            return new OrderSource(sourceId);
        }

        /// <summary>
        ///     Returns order source for the specified source name.
        ///     The name must be either predefined, or contain at most 3 ASCII characters.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <returns>Order source.</returns>
        /// <exception cref="ArgumentException">If name is malformed.</exception>
        public static OrderSource ValueOf(string name)
        {
            string upName = name.ToUpper();
            if (sourcesByName.ContainsKey(upName))
                return sourcesByName[upName];
            return new OrderSource(upName);
        }

        /// <summary>
        ///     Returns string representation of source.
        /// </summary>
        /// <returns>String representation of source.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Returns string representation of source.
        /// </summary>
        /// <param name="os">OrderSource</param>
        public static implicit operator string(OrderSource os)
        {
            return os.ToString();
        }

        #region Implementation of IComparable

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer
        /// that indicates whether the current instance precedes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">obj is not the same type as this instance</exception>
        public int CompareTo(object obj)
        {
            if (obj is OrderSource)
            {
                return Id - (obj as OrderSource).Id;
            }
            else
                throw new ArgumentException("object is not the OrderSource Type. ");
        }
        #endregion

        #region  private helper methods

        private static void checkChar(char c) {
            if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9')
                return;
            throw new ArgumentException("Source name must contain only alphanumeric characters");
        }

        private static int ComposeId(string name)
        {
            int sourceId = 0;
            int n = name.Length;
            if (n == 0 || n > 4)
                throw new ArgumentException("Source name must be from 1 to 4 ASCII characters");
            for (int i = 0; i < n; i++)
            {
                char c = name[i];
                checkChar(c);
                sourceId = (sourceId << 8) | c;
            }
            return sourceId;
        }

        private static string DecodeName(int id)
        {
            StringBuilder sb = new StringBuilder(4);
            if (id <= 0)
                throw new ArgumentException("Source id must be from 1 to 4 ASCII characters");
            for (int i = 24; i >= 0; i -= 8)
            {
                if (id >> i == 0) // skip highest contiguous zeros
                    continue;
                char c = (char)((id >> i) & 0xff);
                checkChar(c);
                sb.Append(c);
            }
            return sb.ToString();
        }

        #endregion //private helper methods
    }

}
