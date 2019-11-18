#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Linq;

namespace com.dxfeed.util
{
    /// <summary>
    /// Caches strings and provides lookup methods by raw character data to avoid string construction.
    /// It is intended to be used in various parsers to reduce memory footprint and garbage.
    ///
    /// The <tt>StringCache</tt> is a <b>N-way set associative cache</b> which distributes all strings
    /// into buckets based on hash function and then uses LRU algorithm within each bucket.
    /// The <tt>StringCache</tt> is a thread-safe, asynchronous, wait-free data structure.
    /// </summary>
    public class StringCache
    {
        private int bucketNumber;
        private int bucketSize;
        private string[] cache;

        private long requestCount; // counts requests except null and empty strings
        private long compareCount; // counts compares except first checks (i.e. add requestCount to get total)
        private long missCount; // counts misses when new string was placed into cache

        /// <summary>
        /// Creates a <tt>StringCache</tt> with default parameters.
        /// </summary>
        public StringCache() : this(997, 4) { }

        /// <summary>
        /// Creates a <tt>StringCache</tt> with the specified number of elements and default bucket size.
        /// </summary>
        /// <param name="size">The number of elements.</param>
        /// <exception cref="System.ArgumentException">If parameters are not positive or result in too large cache.</exception>
        public StringCache(int size) : this((size + 3) / 4, 4) { }

        /// <summary>
        /// Creates a <tt>StringCache</tt> with the specified number of buckets and their size.
        /// <b>NOTE:</b> cache uses linear search within each bucket, so do not use very large bucket sizes.
        /// </summary>
        /// <param name="bucketNumber">The number of buckets.</param>
        /// <param name="bucketSize">The size of each bucket.</param>
        /// <exception cref="System.ArgumentException">If parameters are not positive or result in too large cache.</exception>
        public StringCache(int bucketNumber, int bucketSize)
        {
            if (bucketNumber <= 0 || bucketSize <= 0 || bucketSize >= int.MaxValue / 2 / bucketNumber)
                throw new ArgumentException();
            this.bucketNumber = bucketNumber;
            this.bucketSize = bucketSize;
            cache = new string[bucketNumber * bucketSize];
        }

        /// <summary>
        /// Returns string from the cache that matches specified string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string Get(string s)
        {
            return Get(s, false);
        }

        /// <summary>
        /// Returns string from the cache that matches specified string.
        /// If <tt>copy</tt> parameter is <tt>true</tt> then specified string will be copied before being put to cache.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        public string Get(string s, bool copy)
        {
            if (s == null)
                return null;
            if (string.IsNullOrEmpty(s))
                return "";
            requestCount++;
            int hash = s.GetHashCode();
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (Eq(cached, s, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (Eq(cached = cache[n + k], s, hash))
                    return Finish(cached, n, k);
            missCount++;
            //noinspection RedundantStringConstructorCall
            return Finish(copy ? string.Copy(s) : s, n, bucketSize - 1);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string Get(string s, int offset, int length)
        {
            if (length == 0)
                return "";
            if (offset == 0 && length == s.Length)
                return Get(s, false);
            requestCount++;
            int hash = 0;
            for (int i = 0; i < length; i++)
                hash = 31 * hash + s[offset + i];
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (Eq(cached, s, offset, length, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (Eq(cached = cache[n + k], s, offset, length, hash))
                    return Finish(cached, n, k);
            missCount++;
            return Finish(s.Substring(offset, length), n, bucketSize - 1);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// </summary>
        /// <param name="charSequence"></param>
        /// <returns></returns>
        public string Get(object charSequence)
        {
            if (charSequence.GetType() == typeof(string))
                return Get((string)charSequence, false);
            if (charSequence == null)
                return null;
            char[] cs = (char[])charSequence;
            int length = cs.Length;
            if (length == 0)
                return "";
            requestCount++;
            int hash = 0;
            for (int i = 0; i < length; i++)
                hash = 31 * hash + cs[i];
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (Eq(cached, cs, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (Eq(cached = cache[n + k], cs, hash))
                    return Finish(cached, n, k);
            missCount++;
            return Finish(new string(cs), n, bucketSize - 1);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public string Get(char[] c)
        {
            if (c == null)
                return null;
            return Get(c, 0, c.Length);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string Get(char[] c, int offset, int length)
        {
            if (length == 0)
                return "";
            requestCount++;
            int hash = 0;
            for (int i = 0; i < length; i++)
                hash = 31 * hash + c[offset + i];
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (Eq(cached, c, offset, length, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (Eq(cached = cache[n + k], c, offset, length, hash))
                    return Finish(cached, n, k);
            missCount++;
            return Finish(new string(c, offset, length), n, bucketSize - 1);
        }

        public override string ToString()
        {
            long rc = requestCount;
            return "StringCache(" + bucketNumber + "x" + bucketSize + ", " + rc + " requests, " +
                Math.Max(rc - missCount, 0) * 10000 / Math.Max(rc, 1) / 100.0 + "% hits, " +
                (rc + compareCount) * 100 / Math.Max(rc, 1) + "% compares)";
        }

        private string Finish(string cached, int n, int k)
        {
            compareCount += k;
            while (k > 0)
                cache[n + k] = cache[n + --k];
            return cache[n] = cached;
        }

        private static bool Eq(string cached, string s, int hash)
        {
            if (cached == null || cached.GetHashCode() != hash)
                return false;
            return cached.Equals(s);
        }

        private static bool Eq(string cached, string s, int offset, int length, int hash)
        {
            if (cached == null || cached.GetHashCode() != hash || cached.Length != length)
                return false;
            return cached.Substring(0, length).Equals(s.Substring(offset, length));
        }

        private static bool Eq(string cached, char[] cs, int hash)
        {
            if (cached == null || cached.GetHashCode() != hash)
                return false;
            return cached.SequenceEqual(cs);
        }

        private static bool Eq(string cached, char[] c, int offset, int length, int hash)
        {
            if (cached == null || cached.GetHashCode() != hash || cached.Length != length)
                return false;
            for (int i = 0; i < length; i++)
                if (cached[i] != c[offset + i])
                    return false;
            return true;
        }
    }
}
