/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Text;

namespace com.dxfeed.api.events
{
    /// <summary>
    /// Identifies source of Order and SpreadOrder events.
    /// There are the following kinds of order sources:
    /// 
    /// Synthetic sources COMPOSITE_BID, COMPOSITE_ASK, REGIONAL_BID, and REGIONAL_ASK are provided 
    /// for convenience of a consolidated order book and are automatically generated based on the 
    /// corresponding Quote events.
    /// 
    /// Aggregate sources AGGREGATE_BID and AGGREGATE_ASK provide futures depth (aggregated by 
    /// price level) and NASDAQ Level II (top of book for each market maker).
    /// </summary>
    public class OrderSource
    {
        private static Dictionary<int, OrderSource> sourcesById = new Dictionary<int, OrderSource>();
        private static Dictionary<string, OrderSource> sourcesByName = new Dictionary<string, OrderSource>();

        /// <summary>
        /// Maximal source identifier that is supported by Order and SpreadOrder.
        /// </summary>
        public static readonly int MAX_SOURCE_ID = (1 << 23) - 1;

        /// <summary>
        /// Bid side of a composite Quote.
        /// It is a synthetic source.
        /// The subscription on composite Quote event is observed when this source is subscribed to.
        /// </summary>
        public static readonly OrderSource COMPOSITE_BID = new OrderSource(1, "COMPOSITE_BID");

        /// <summary>
        /// Ask side of a composite Quote.
        /// It is a synthetic source.
        /// The subscription on composite Quote event is observed when this source is subscribed to.
        /// </summary>
        public static readonly OrderSource COMPOSITE_ASK = new OrderSource(2, "COMPOSITE_ASK");

        /// <summary>
        /// Bid side of a regional Quote.
        /// It is a synthetic source.
        /// The subscription on regional Quote event is observed when this source is subscribed to.
        /// </summary>
        public static readonly OrderSource REGIONAL_BID = new OrderSource(3, "REGIONAL_BID");

        /// <summary>
        /// Ask side of a regional Quote.
        /// It is a synthetic source.
        /// The subscription on regional Quote event is observed when this source is subscribed to.
        /// </summary>
        public static readonly OrderSource REGIONAL_ASK = new OrderSource(4, "REGIONAL_ASK");

        /// <summary>
        /// Bid side of an aggregate order book (futures depth and NASDAQ Level II).
        /// </summary>
        public static readonly OrderSource AGGREGATE_BID = new OrderSource(5, "AGGREGATE_BID");

        /// <summary>
        /// Ask side of an aggregate order book (futures depth and NASDAQ Level II).
        /// </summary>
        public static readonly OrderSource AGGREGATE_ASK = new OrderSource(6, "AGGREGATE_ASK");

        /// <summary>
        /// Default source for publishing custom order books.
        /// </summary>
        public static readonly OrderSource DEFAULT = new OrderSource(0, "DEFAULT");

        /// <summary>
        /// NASDAQ Total View.
        /// </summary>
        public static readonly OrderSource NTV = new OrderSource("NTV");

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
        /// Borsa Istanbul Exchange.
        /// </summary>
        public static readonly OrderSource IST = new OrderSource("IST");

        /// <summary>
        /// Empty order source.
        /// </summary>
        public static readonly OrderSource EMPTY = new OrderSource(7, string.Empty);

        /// <summary>
        /// Create a new order source
        /// </summary>
        /// <param name="id">Id of the new order source.</param>
        /// <param name="name">Name of the new order source.</param>
        private OrderSource(int id, string name)
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
        /// For transient objects only (statically unknown source id)
        /// </summary>
        /// <param name="id">Id of the new order source.</param>
        private OrderSource(int id) : this(id, DecodeName(id)) { }

        /// <summary>
        /// For transient objects only (statically unknown source id)
        /// </summary>
        /// <param name="name">Name of the new order source.</param>
        private OrderSource(string name) : this(ComposeId(name), name) { }

        /// <summary>
        /// Gets the identifier of this order source.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the name of this order source.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns order source for the specified source identifier.
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
        /// Returns order source for the specified source name.
        /// The name must be either predefined, or contain at most 3 ASCII characters.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <returns>Order source.</returns>
        /// <exception cref="ArgumentException">If name is malformed.</exception>
        public static OrderSource ValueOf(string name)
        {
            if (sourcesByName.ContainsKey(name))
                return sourcesByName[name];
            return new OrderSource(name);
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator string(OrderSource os)
        {
            return os.ToString();
        }

        #region  private helper methods

        private static int ComposeId(string name)
        {
            int sourceId = 0;
            int n = name.Length;
            if (n == 0 || n > 3)
                throw new ArgumentException("Source name must be from 1 to 3 ASCII characters");
            for (int i = 0; i < n; i++)
            {
                char c = name[i];
                if (c <= 0x20 || c >= 0x7f)
                    throw new ArgumentException("Source name must contain only ASCII characters");
                sourceId = (sourceId << 8) | c;
            }
            return sourceId;
        }

        private static string DecodeName(int id)
        {
            StringBuilder sb = new StringBuilder(3);
            if (id <= 0 || id > MAX_SOURCE_ID)
                throw new ArgumentException("Source id must be from 1 to 3 ASCII characters");
            for (int i = 3; --i >= 0;)
            {
                if (id >> (8 * i) == 0) // skip highest contiguous zeros
                    continue;
                char c = (char)((id >> (8 * i)) & 0xff);
                if (c <= 0x20 || c >= 0x7f)
                    throw new ArgumentException("Source id must contain only ASCII characters");
                sb.Append(c);
            }
            return sb.ToString();
        }

        #endregion //private helper methods
    }

}
