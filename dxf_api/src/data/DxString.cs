/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Stack allocated string representation 
    /// </summary>
    public unsafe struct DxString : IEquatable<DxString>, ICloneable
    {
        private const int SizeLimit = 15;
        private fixed char chars[SizeLimit];
        private string resolved;

        public DxString(char* src)
        {
            var src_ptr = src;
            fixed (char* dptr = chars)
            {
                if (src == null)
                {
                    resolved = null;
                    return;
                }
                var dst_ptr = dptr;
                var len = 0;
                while (len < SizeLimit - 1 && *src_ptr != 0)
                {
                    *dst_ptr = *src_ptr;
                    dst_ptr++;
                    src_ptr++;
                    len++;
                }

                if (*src_ptr == 0)
                {
                    resolved = null;
                    return;
                }

                //out of SizeLimit
                dptr[0] = (char)0;
                resolved = new string(src);
            }
        }

        public override string ToString()
        {
            if (resolved != null)
                return resolved;

            fixed (char* data = chars)
            {
                resolved = new string(data);
            }

            return resolved;
        }

        public bool Equals(DxString other)
        {
            if (resolved != null && other.resolved != null)
                return resolved == other.resolved;

            fixed (char* data = chars)
            {
                for (var i = 0; i < SizeLimit; i++)
                {
                    if (data[i] != other.chars[i])
                        return false;

                    if (data[i] == 0)
                        return true;
                }
            }

            return false;
        }

        private bool Equals(string other)
        {
            if (other == null)
                return false;

            if (resolved != null)
                return (resolved == other);

            fixed (char* data = chars)
            {
                for (var i = 0; i < SizeLimit; i++)
                {
                    if (i == other.Length)
                        return data[i] == 0;
                    if (data[i] != other[i])
                        return false;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (obj.GetType() != typeof(DxString))
                return false;
            return Equals((DxString)obj);
        }

        public override int GetHashCode()
        {
            var hc = 0;
            fixed (char* data = chars)
            {
                //long string. fallback to string hashcode
                if (data[0] == 0 & resolved != null)
                    return resolved.GetHashCode();

                for (var i = 0; i < SizeLimit; i++)
                {
                    int c = data[i];
                    if (c == 0)
                        break;

                    if ((i & 1) == 1)
                        c <<= 16;
                    hc ^= c;
                }
            }
            return hc;
        }

        public object Clone()
        {
            fixed (char* pChars = chars)
            {
                return new DxString(pChars);
            }
        }

        public static bool operator ==(DxString left, string right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DxString left, string right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(string left, DxString right)
        {
            return right.Equals(right);
        }

        public static bool operator !=(string left, DxString right)
        {
            return !right.Equals(right);
        }

        public static bool operator ==(DxString left, DxString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DxString left, DxString right)
        {
            return !left.Equals(right);
        }
    }
}
