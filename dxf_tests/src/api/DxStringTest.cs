/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using NUnit.Framework;
using com.dxfeed.api.data;

namespace com.dxfeed.api
{
    [TestFixture]
    public class DxStringTest
    {
        private unsafe static DxString Create(char[] chars)
        {
            fixed (char* ptr = chars)
            {
                return new DxString(ptr);
            }
        }

        [Test]
        public void TestToString()
        {
            var dxStr = Create("haba".ToCharArray());
            Assert.True("haba" == dxStr.ToString());
        }


        [Test]
        public void TestEqualityToString()
        {
            var dxStr = Create("haba".ToCharArray());
            Assert.True("haba" == dxStr);
            Assert.True(dxStr == "haba");
        }

        [Test]
        public void TestNonZeroHash()
        {
            var dxStr = Create("haba".ToCharArray());
            Assert.True(dxStr.GetHashCode() != 0);
        }

        [Test]
        public void TestLongCharArray()
        {
            var dxStr = Create("hababubatubaubatumbarumba".ToCharArray());
            Assert.True("hababubatubaubatumbarumba" == dxStr);
            Assert.True(dxStr == "hababubatubaubatumbarumba");
        }

        [Test]
        public void TestHashCodeAfterResolve()
        {
            var dxStr = Create("haba".ToCharArray());
            var hc1 = dxStr.GetHashCode();
            dxStr.ToString();
            var hc2 = dxStr.GetHashCode();
            Assert.True(hc1 == hc2);
        }
    }
}
