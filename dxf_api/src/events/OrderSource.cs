#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

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
    ///     Synthetic sources <see cref="COMPOSITE_BID"/>, <see cref="COMPOSITE_ASK"/>, <see cref="REGIONAL_BID"/>, and <see cref="REGIONAL_ASK"/> are
    ///     provided for convenience of a consolidated order book and are automatically generated
    ///     based on the corresponding <see cref="IDxQuote"/> events.
    /// </para>
    /// <para>
    ///     Aggregate sources <see cref="AGGREGATE_BID"/> and <see cref="AGGREGATE_ASK"/> provide futures depth (aggregated by
    ///     price level) and NASDAQ Level II (top of book for each market maker).
    /// </para>
    /// </summary>
    public class OrderSource : IndexedEventSource, IComparable
    {
        private static readonly Dictionary<int, OrderSource> SourcesById = new Dictionary<int, OrderSource>();
        private static readonly Dictionary<string, OrderSource> SourcesByName = new Dictionary<string, OrderSource>();

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
        /// NASDAQ Total View. Record for price level book.
        /// </summary>
        public static readonly OrderSource ntv = new OrderSource("ntv");

        /// <summary>
        /// NASDAQ Futures Exchange.
        /// </summary>
        public static readonly OrderSource NFX = new OrderSource("NFX");

        /// <summary>
        /// NASDAQ eSpeed.
        /// </summary>
        public static readonly OrderSource ESPD = new OrderSource("ESPD");

        /// <summary>
        /// NASDAQ Fixed Income.
        /// </summary>
        public static readonly OrderSource XNFI = new OrderSource("XNFI");

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
        /// Bats Europe DXE Exchange.
        /// </summary>
        public static readonly OrderSource CEUX = new OrderSource("CEUX");

        /// <summary>
        /// Bats Europe TRF.
        /// </summary>
        public static readonly OrderSource BXTR = new OrderSource("BXTR");

        /// <summary>
        /// Borsa Istanbul Exchange.
        /// </summary>
        public static readonly OrderSource IST = new OrderSource("IST");

        /// <summary>
        /// Borsa Istanbul Exchange. Record for particular top 20 order book.
        /// </summary>
        public static readonly OrderSource BI20 = new OrderSource("BI20");

        /// <summary>
        /// ABE (abe.io) exchange.
        /// </summary>
        public static readonly OrderSource ABE = new OrderSource("ABE");

        /// <summary>
        /// FAIR (FairX) exchange.
        /// </summary>
        public static readonly OrderSource FAIR = new OrderSource("FAIR");
        
        /// <summary>
        /// CME Globex.
        /// </summary>
        public static readonly OrderSource GLBX = new OrderSource("GLBX");

        /// <summary>
        /// CME Globex. Record for price level book.
        /// </summary>
        public static readonly OrderSource glbx = new OrderSource("glbx");

        /// <summary>
        /// Eris Exchange group of companies.
        /// </summary>
        public static readonly OrderSource ERIS = new OrderSource("ERIS");

        /// <summary>
        /// Eurex Exchange.
        /// </summary>
        public static readonly OrderSource XEUR = new OrderSource("XEUR");

        /// <summary>
        /// Eurex Exchange. Record for price level book.
        /// </summary>
        public static readonly OrderSource xeur = new OrderSource("xeur");

        /// <summary>
        /// CBOE Futures Exchange.
        /// </summary>
        public static readonly OrderSource CFE = new OrderSource("CFE");

        /// <summary>
        /// CBOE Options C2 Exchange.
        /// </summary>
        public static readonly OrderSource C2OX = new OrderSource("C2OX");

        /// <summary>
        /// Small Exchange.
        /// </summary>
        public static readonly OrderSource SMFE = new OrderSource("SMFE");

        /// <summary>
        /// Investors exchange. Record for price level book.
        /// </summary>
        public static readonly OrderSource iex = new OrderSource("iex");

        /// <summary>
        ///     Create a new order source
        /// </summary>
        /// <param name="id">Id of the new order source.</param>
        /// <param name="name">Name of the new order source.</param>
        private OrderSource(int id, string name) : base(id, name)
        {
            if (SourcesById.ContainsKey(id))
                throw new ArgumentException($"The source id '{id}' is already exist.");
            if (SourcesByName.ContainsKey(name))
                throw new ArgumentException($"The source name '{name}' is already exist.");
            Id = id;
            Name = name;
            SourcesById[Id] = this;
            SourcesByName[Name] = this;
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
            if (SourcesById.ContainsKey(sourceId))
                return SourcesById[sourceId];
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
            return SourcesByName.ContainsKey(name) ? SourcesByName[name] : new OrderSource(name);
        }

        /// <summary>
        /// Determines whether specified source name exists in the list of default source names
        /// </summary>
        /// <param name="name">The source name</param>
        /// <returns><c>true</c> if the source name exists in the list of default source names</returns>
        public static bool HasSourceName(string name)
        {
            return SourcesByName.ContainsKey(name);
        }

        /// <summary>
        /// Determines whether specified source identifier refers to special order source.
        /// Special order sources are used for wrapping non-order events into order events.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <returns><c>true</c> if it is a special source</returns>
        public static bool IsSpecialSourceId(int sourceId)
        {
            return sourceId >= 1 && sourceId <= 6;
        }

        /// <summary>
        /// Determines whether specified source name refers to special order source.
        /// Special order sources are used for wrapping non-order events into order events.
        /// </summary>
        /// <param name="name">The source name.</param>
        /// <returns><c>true</c> if it is a special source</returns>
        public static bool IsSpecialSourceName(string name)
        {
            return IsSpecialSourceId(ValueOf(name).Id);
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

        private static void CheckChar(char c) {
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
                CheckChar(c);
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
                CheckChar(c);
                sb.Append(c);
            }
            return sb.ToString();
        }

        #endregion //private helper methods
    }

}
