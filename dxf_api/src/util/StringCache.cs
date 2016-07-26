/*
 * QDS - Quick Data Signalling Library
 * Copyright (C) 2002-2015 Devexperts LLC
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at
 * http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Linq;

namespace com.dxfeed.util {

    /// <summary>
    /// Caches strings and provides lookup methods by raw character data to avoid string construction.
    /// It is intended to be used in various parsers to reduce memory footprint and garbage.
    /// <p>
    /// The <tt>StringCache</tt> is a <b>N-way set associative cache</b> which distributes all strings
    /// into buckets based on hash function and then uses LRU algorithm within each bucket.
    /// The <tt>StringCache</tt> is a thread-safe, asynchronous, wait-free data structure.
    /// </summary>
    public class StringCache {

        private int bucketNumber;
        private int bucketSize;
        private string[] cache;

        private long requestCount; // counts requests except null and empty strings
        private long compareCount; // counts compares except first checks (i.e. add requestCount to get total)
        private long missCount; // counts misses when new string was placed into cache

        /**
         * Creates a <tt>StringCache</tt> with default parameters.
         */
        public StringCache() : this(997, 4) { }

        /// <summary>
        /// Creates a <tt>StringCache</tt> with the specified number of elements and default bucket size.
        /// </summary>
        /// <param name="size">The number of elements.</param>
        /// <exception cref="ArgumentException">If parameters are not positive or result in too large cache.</exception>
        public StringCache(int size) : this((size + 3) / 4, 4) { }

        /// <summary>
        /// Creates a <tt>StringCache</tt> with the specified number of buckets and their size.
        /// <b>NOTE:</b> cache uses linear search within each bucket, so do not use very large bucket sizes.
        /// </summary>
        /// <param name="bucketNumber">The number of buckets.</param>
        /// <param name="bucketSize">The size of each bucket.</param>
        /// <exception cref="ArgumentException">If parameters are not positive or result in too large cache.</exception>
        public StringCache(int bucketNumber, int bucketSize) {
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
        public string get(string s) {
            return get(s, false);
        }

        /// <summary>
        /// Returns string from the cache that matches specified string.
        /// If <tt>copy</tt> parameter is <tt>true</tt> then specified string will be copied before being put to cache.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        public string get(string s, bool copy) {
            if (s == null)
                return null;
            if (String.IsNullOrEmpty(s))
                return "";
            requestCount++;
            int hash = s.GetHashCode();
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (eq(cached, s, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (eq(cached = cache[n + k], s, hash))
                    return finish(cached, n, k);
            missCount++;
            //noinspection RedundantStringConstructorCall
            return finish(copy ? string.Copy(s) : s, n, bucketSize - 1);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string get(string s, int offset, int length) {
            if (length == 0)
                return "";
            if (offset == 0 && length == s.Length)
                return get(s, false);
            requestCount++;
            int hash = 0;
            for (int i = 0; i < length; i++)
                hash = 31 * hash + s[offset + i];
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (eq(cached, s, offset, length, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (eq(cached = cache[n + k], s, offset, length, hash))
                    return finish(cached, n, k);
            missCount++;
            return finish(s.Substring(offset, length), n, bucketSize - 1);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// </summary>
        /// <param name="charSequence"></param>
        /// <returns></returns>
        public string get(object charSequence) {
            if (charSequence.GetType() == typeof(string))
                return get((string)charSequence, false);
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
            if (eq(cached, cs, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (eq(cached = cache[n + k], cs, hash))
                    return finish(cached, n, k);
            missCount++;
            return finish(new string(cs), n, bucketSize - 1);
        }

        //TODO: after apotation crosses
        ///**
        // * Returns string from the cache that matches specified character sequence.
        // */
        //public string get(CharSequence cs, int offset, int length) {
        //    if (length == 0)
        //        return "";
        //    if (offset == 0 && length == cs.Length)
        //        return get(cs);
        //    requestCount++;
        //    int hash = 0;
        //    for (int i = 0; i < length; i++)
        //        hash = 31 * hash + cs.charAt(offset + i);
        //    int n = Math.Abs(hash % bucketNumber) * bucketSize;
        //    string cached = cache[n];
        //    if (eq(cached, cs, offset, length, hash))
        //        return cached;
        //    for (int k = 1; k < bucketSize; k++)
        //        if (eq(cached = cache[n + k], cs, offset, length, hash))
        //            return finish(cached, n, k);
        //    missCount++;
        //    return finish(cs.subSequence(offset, offset + length).toString(), n, bucketSize - 1);
        //}

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public string get(char[] c) {
            if (c == null)
                return null;
            return get(c, 0, c.Length);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string get(char[] c, int offset, int length) {
            if (length == 0)
                return "";
            requestCount++;
            int hash = 0;
            for (int i = 0; i < length; i++)
                hash = 31 * hash + c[offset + i];
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (eq(cached, c, offset, length, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (eq(cached = cache[n + k], c, offset, length, hash))
                    return finish(cached, n, k);
            missCount++;
            return finish(new string(c, offset, length), n, bucketSize - 1);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// This method uses only 7 lowest bits (ASCII range) of each character ignoring higher bits.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public string getASCII(byte[] b) {
            if (b == null)
                return null;
            return getASCII(b, 0, b.Length);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// This method uses only 7 lowest bits (ASCII range) of each character ignoring higher bits.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string getASCII(byte[] b, int offset, int length) {
            if (length == 0)
                return "";
            requestCount++;
            int hash = 0;
            for (int i = 0; i < length; i++)
                hash = 31 * hash + (b[offset + i] & 0x7F);
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (eqASCII(cached, b, offset, length, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (eqASCII(cached = cache[n + k], b, offset, length, hash))
                    return finish(cached, n, k);
            missCount++;
            char[] c = new char[length];
            for (int i = 0; i < length; i++)
                c[i] = (char)(b[offset + i] & 0x7F);
            return finish(new string(c), n, bucketSize - 1);
        }

        /// <summary>
        /// Returns string from the cache that matches specified character sequence.
        /// This method uses string encoding technique defined in {@code ShortString} class.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string getShortString(long code) {
            if (code == 0)
                return null;
            requestCount++;
            long reverse = 0; // normalized code in reverse order with zero bytes removed
            int length = 0;
            do {
                byte tempCode = (byte)code;
                if (tempCode != 0) {
                    reverse = (reverse << 8) | (tempCode & 0xFF);
                    length++;
                }
                code = (long)((ulong)code >> 8);
            } while (code != 0);
            int hash = 0;
            for (int i = 0; i < length; i++)
                hash = 31 * hash + ((int)((ulong)reverse >> (i << 3)) & 0xFF);
            int n = Math.Abs(hash % bucketNumber) * bucketSize;
            string cached = cache[n];
            if (eqShortString(cached, reverse, length, hash))
                return cached;
            for (int k = 1; k < bucketSize; k++)
                if (eqShortString(cached = cache[n + k], reverse, length, hash))
                    return finish(cached, n, k);
            missCount++;
            char[] c = new char[length];
            for (int i = 0; i < length; i++)
                c[i] = (char)((int)((ulong)reverse >> (i << 3)) & 0xFF);
            return finish(new string(c, 0, length), n, bucketSize - 1);
        }

        public string toString() {
            long rc = requestCount;
            return "StringCache(" + bucketNumber + "x" + bucketSize + ", " + rc + " requests, " +
                Math.Max(rc - missCount, 0) * 10000 / Math.Max(rc, 1) / 100.0 + "% hits, " +
                (rc + compareCount) * 100 / Math.Max(rc, 1) + "% compares)";
        }

        private string finish(string cached, int n, int k) {
            compareCount += k;
            while (k > 0)
                cache[n + k] = cache[n + --k];
            return cache[n] = cached;
        }

        private static bool eq(string cached, string s, int hash) {
            if (cached == null || cached.GetHashCode() != hash)
                return false;
            return cached.Equals(s);
        }

        private static bool eq(string cached, string s, int offset, int length, int hash) {
            if (cached == null || cached.GetHashCode() != hash || cached.Length != length)
                return false;
            //return cached.regionMatches(0, s, offset, length);
            return cached.Substring(0, length).Equals(s.Substring(offset, length));
        }

        private static bool eq(string cached, char[] cs, int hash) {
            if (cached == null || cached.GetHashCode() != hash)
                return false;
            return cached.SequenceEqual(cs);
        }

        //TODO: after adoptation crosses with eq(string,char[],int,int,int)
        //private static bool eq(string cached, CharSequence cs, int offset, int length, int hash) {
        //    if (cached == null || cached.GetHashCode() != hash || cached.Length != length)
        //        return false;
        //    for (int i = 0; i < length; i++)
        //        if (cached[i] != cs.charAt(offset + i))
        //            return false;
        //    return true;
        //}

        private static bool eq(string cached, char[] c, int offset, int length, int hash) {
            if (cached == null || cached.GetHashCode() != hash || cached.Length != length)
                return false;
            for (int i = 0; i < length; i++)
                if (cached[i] != c[offset + i])
                    return false;
            return true;
        }

        private static bool eqASCII(string cached, byte[] b, int offset, int length, int hash) {
            if (cached == null || cached.GetHashCode() != hash || cached.Length != length)
                return false;
            for (int i = 0; i < length; i++)
                if (cached[i] != (b[offset + i] & 0x7F))
                    return false;
            return true;
        }

        private static bool eqShortString(string cached, long reverse, int length, int hash) {
            if (cached == null || cached.GetHashCode() != hash || cached.Length != length)
                return false;
            for (int i = 0; i < length; i++)
                if (cached[i] != ((int)((ulong)reverse >> (i << 3)) & 0xFF))
                    return false;
            return true;
        }

    }
}
