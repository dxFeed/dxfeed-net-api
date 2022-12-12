#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.candle;
using NUnit.Framework;

namespace com.dxfeed.api
{
    [TestFixture]
    public class CandleSymbolTest
    {
        private const double Delta = 0.0001;

        public class CandleSymbolTestCase
        {
            public ICandleSymbolAttribute[] attrs;
            public string baseSymbol;
            public string expected;

            public CandleSymbolTestCase(string baseSymbol, string expected)
            {
                this.baseSymbol = baseSymbol;
                this.expected = expected;
            }

            public CandleSymbolTestCase(string baseSymbol, ICandleSymbolAttribute[] attrs, string expected)
            {
                this.baseSymbol = baseSymbol;
                this.attrs = attrs;
                this.expected = expected;
            }
        }

        private static readonly double defaultPeriodValue = 1d;
        private static readonly double userPeriodValue = 2d;

        private static readonly CandleSymbolTestCase[] candleSymbolParamsTestCases =
        {
            new CandleSymbolTestCase("AAPL", "AAPL"),
            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Exchange.NewExchange('A')}, "AAPL&A"),

            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.TICK)}, "AAPL"),
            new CandleSymbolTestCase("AAPL",
                new[]
                {
                    CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.TICK),
                    CandleSymbolAttributes.PriceLevel.NewPriceLevel(0.5)
                }, "AAPL{pl=0.5}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.SECOND)}, "AAPL{=s}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.MINUTE)}, "AAPL{=m}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.HOUR)}, "AAPL{=h}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.DAY)}, "AAPL{=d}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.WEEK)}, "AAPL{=w}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.MONTH)}, "AAPL{=mo}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.OPTEXP)}, "AAPL{=o}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.YEAR)}, "AAPL{=y}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.VOLUME)}, "AAPL{=v}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.PRICE)}, "AAPL{=p}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.PRICE_MOMENTUM)},
                "AAPL{=pm}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(defaultPeriodValue, CandleType.PRICE_RENKO)},
                "AAPL{=pr}"),

            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.TICK)}, "AAPL{=2t}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.SECOND)}, "AAPL{=2s}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.MINUTE)}, "AAPL{=2m}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.HOUR)}, "AAPL{=2h}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.DAY)}, "AAPL{=2d}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.WEEK)}, "AAPL{=2w}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.MONTH)}, "AAPL{=2mo}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.OPTEXP)}, "AAPL{=2o}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.YEAR)}, "AAPL{=2y}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.VOLUME)}, "AAPL{=2v}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.PRICE)}, "AAPL{=2p}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.PRICE_MOMENTUM)},
                "AAPL{=2pm}"),
            new CandleSymbolTestCase("AAPL",
                new[] {CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.PRICE_RENKO)}, "AAPL{=2pr}"),

            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Alignment.MIDNIGHT}, "AAPL"),
            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Alignment.SESSION}, "AAPL{a=s}"),

            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Price.LAST}, "AAPL"),
            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Price.BID}, "AAPL{price=bid}"),
            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Price.ASK}, "AAPL{price=ask}"),
            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Price.MARK}, "AAPL{price=mark}"),
            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Price.SETTLEMENT}, "AAPL{price=s}"),

            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Session.ANY}, "AAPL"),
            new CandleSymbolTestCase("AAPL", new[] {CandleSymbolAttributes.Session.REGULAR}, "AAPL{tho=true}"),

            new CandleSymbolTestCase("AAPL", new[]
            {
                CandleSymbolAttributes.Exchange.NewExchange('A'),
                CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.DAY),
                CandleSymbolAttributes.Alignment.SESSION,
                CandleSymbolAttributes.Price.MARK,
                CandleSymbolAttributes.Session.REGULAR
            }, "AAPL&A{=2d,a=s,price=mark,tho=true}"),

            new CandleSymbolTestCase("AAPL&A", new[]
            {
                CandleSymbolAttributes.Period.NewPeriod(userPeriodValue, CandleType.DAY),
                CandleSymbolAttributes.Alignment.SESSION,
                CandleSymbolAttributes.Price.MARK,
                CandleSymbolAttributes.Session.REGULAR
            }, "AAPL&A{=2d,a=s,price=mark,tho=true}"),
            new CandleSymbolTestCase("AAPL&A{=d}", new[]
            {
                CandleSymbolAttributes.Alignment.SESSION,
                CandleSymbolAttributes.Price.MARK,
                CandleSymbolAttributes.Session.REGULAR
            }, "AAPL&A{=d,a=s,price=mark,tho=true}"),
            new CandleSymbolTestCase("AAPL&A{=2d}", new[]
            {
                CandleSymbolAttributes.Alignment.SESSION,
                CandleSymbolAttributes.Price.MARK,
                CandleSymbolAttributes.Session.REGULAR
            }, "AAPL&A{=2d,a=s,price=mark,tho=true}"),
            new CandleSymbolTestCase("AAPL&A{=2d,a=s}", new[]
            {
                CandleSymbolAttributes.Price.MARK,
                CandleSymbolAttributes.Session.REGULAR
            }, "AAPL&A{=2d,a=s,price=mark,tho=true}"),
            new CandleSymbolTestCase("AAPL&A{=2d,a=s,price=mark}", new[]
            {
                CandleSymbolAttributes.Session.REGULAR,
                CandleSymbolAttributes.PriceLevel.NewPriceLevel(0.5)
            }, "AAPL&A{=2d,a=s,pl=0.5,price=mark,tho=true}")
        };

        public void CandleSymbolPeriodTest(string symbolString, double expectedValue,
            CandleType expectedType)
        {
            var symbol = CandleSymbol.ValueOf(symbolString);
            Assert.AreEqual(symbolString, symbol.ToString());
            Assert.AreEqual(expectedValue, symbol.PeriodValue, Delta);
            Assert.AreEqual(expectedType.Id, symbol.PeriodId);
        }

        public void CandleSymbolAlignmentTest(string symbolString, int expectedId)
        {
            var symbol = CandleSymbol.ValueOf(symbolString);
            Assert.AreEqual(symbolString, symbol.ToString());
            Assert.AreEqual(expectedId, symbol.AlignmentId);
        }

        public void CandleSymbolPriceTest(string symbolString, int expectedId)
        {
            var symbol = CandleSymbol.ValueOf(symbolString);
            Assert.AreEqual(symbolString, symbol.ToString());
            Assert.AreEqual(expectedId, symbol.PriceId);
        }

        public void CandleSymbolSessionTest(string symbolString, int expectedId)
        {
            var symbol = CandleSymbol.ValueOf(symbolString);
            Assert.AreEqual(symbolString, symbol.ToString());
            Assert.AreEqual(expectedId, symbol.SessionId);
        }

        public void CandleSymbolPriceLevelTest(string symbolString, double expectedPriceLevel)
        {
            var symbol = CandleSymbol.ValueOf(symbolString);
            Assert.AreEqual(symbolString, symbol.ToString());
            Assert.AreEqual(expectedPriceLevel, symbol.PriceLevel, Delta);
        }

        [Test]
        [TestCaseSource("candleSymbolParamsTestCases")]
        public void CandleSymbolParamsTest(CandleSymbolTestCase testCase)
        {
            if (testCase.attrs == null)
            {
                Assert.AreEqual(testCase.expected, CandleSymbol.ValueOf(testCase.baseSymbol).ToString());
            } else if (testCase.attrs.Length == 1)
            {
                Assert.AreEqual(testCase.expected,
                    CandleSymbol.ValueOf(testCase.baseSymbol, testCase.attrs[0]).ToString());
            } else
            {
                var nextAttrs = new ICandleSymbolAttribute[testCase.attrs.Length - 1];
                Array.Copy(testCase.attrs, 1, nextAttrs, 0, testCase.attrs.Length - 1);
                Assert.AreEqual(testCase.expected,
                    CandleSymbol.ValueOf(testCase.baseSymbol, testCase.attrs[0], nextAttrs).ToString());
            }
        }

        [Test]
        public void CandleSymbolStringTest()
        {
            CandleSymbol symbol;

            symbol = CandleSymbol.ValueOf("AAPL");
            Assert.AreEqual("AAPL", symbol.ToString());

            symbol = CandleSymbol.ValueOf("AAPL&A");
            Assert.AreEqual("AAPL&A", symbol.ToString());
            Assert.AreEqual('A', symbol.ExchangeCode);

            CandleSymbolPeriodTest("AAPL{=t}", defaultPeriodValue, CandleType.TICK);
            CandleSymbolPeriodTest("AAPL{=s}", defaultPeriodValue, CandleType.SECOND);
            CandleSymbolPeriodTest("AAPL{=m}", defaultPeriodValue, CandleType.MINUTE);
            CandleSymbolPeriodTest("AAPL{=h}", defaultPeriodValue, CandleType.HOUR);
            CandleSymbolPeriodTest("AAPL{=d}", defaultPeriodValue, CandleType.DAY);
            CandleSymbolPeriodTest("AAPL{=w}", defaultPeriodValue, CandleType.WEEK);
            CandleSymbolPeriodTest("AAPL{=mo}", defaultPeriodValue, CandleType.MONTH);
            CandleSymbolPeriodTest("AAPL{=o}", defaultPeriodValue, CandleType.OPTEXP);
            CandleSymbolPeriodTest("AAPL{=y}", defaultPeriodValue, CandleType.YEAR);
            CandleSymbolPeriodTest("AAPL{=v}", defaultPeriodValue, CandleType.VOLUME);
            CandleSymbolPeriodTest("AAPL{=p}", defaultPeriodValue, CandleType.PRICE);
            CandleSymbolPeriodTest("AAPL{=pm}", defaultPeriodValue, CandleType.PRICE_MOMENTUM);
            CandleSymbolPeriodTest("AAPL{=pr}", defaultPeriodValue, CandleType.PRICE_RENKO);

            CandleSymbolPeriodTest("AAPL{=2t}", userPeriodValue, CandleType.TICK);
            CandleSymbolPeriodTest("AAPL{=2s}", userPeriodValue, CandleType.SECOND);
            CandleSymbolPeriodTest("AAPL{=2m}", userPeriodValue, CandleType.MINUTE);
            CandleSymbolPeriodTest("AAPL{=2h}", userPeriodValue, CandleType.HOUR);
            CandleSymbolPeriodTest("AAPL{=2d}", userPeriodValue, CandleType.DAY);
            CandleSymbolPeriodTest("AAPL{=2w}", userPeriodValue, CandleType.WEEK);
            CandleSymbolPeriodTest("AAPL{=2mo}", userPeriodValue, CandleType.MONTH);
            CandleSymbolPeriodTest("AAPL{=2o}", userPeriodValue, CandleType.OPTEXP);
            CandleSymbolPeriodTest("AAPL{=2y}", userPeriodValue, CandleType.YEAR);
            CandleSymbolPeriodTest("AAPL{=2v}", userPeriodValue, CandleType.VOLUME);
            CandleSymbolPeriodTest("AAPL{=2p}", userPeriodValue, CandleType.PRICE);
            CandleSymbolPeriodTest("AAPL{=2pm}", userPeriodValue, CandleType.PRICE_MOMENTUM);
            CandleSymbolPeriodTest("AAPL{=2pr}", userPeriodValue, CandleType.PRICE_RENKO);

            CandleSymbolAlignmentTest("AAPL{a=m}", 0);
            CandleSymbolAlignmentTest("AAPL{a=s}", 1);

            CandleSymbolPriceTest("AAPL{price=last}", 0);
            CandleSymbolPriceTest("AAPL{price=bid}", 1);
            CandleSymbolPriceTest("AAPL{price=ask}", 2);
            CandleSymbolPriceTest("AAPL{price=mark}", 3);
            CandleSymbolPriceTest("AAPL{price=s}", 4);

            CandleSymbolSessionTest("AAPL{tho=false}", 0);
            CandleSymbolSessionTest("AAPL{tho=true}", 1);

            CandleSymbolPriceLevelTest("AAPL", double.NaN);
            CandleSymbolPriceLevelTest("AAPL{pl=0}", 0.0);
            CandleSymbolPriceLevelTest("AAPL{pl=0.5}", 0.5);

            symbol = CandleSymbol.ValueOf("AAPL&A{tho=true,price=mark,a=s,=2d}");
            //Assert.AreEqual("AAPL&A{=2d,a=s,price=mark,tho=true}", symbol.ToString());
            Assert.AreEqual('A', symbol.ExchangeCode);
            Assert.AreEqual(3, symbol.PriceId);
            Assert.AreEqual(1, symbol.AlignmentId);
            Assert.AreEqual(1, symbol.SessionId);
            Assert.AreEqual(4, symbol.PeriodId);
            Assert.AreEqual(userPeriodValue, symbol.PeriodValue, 0.00001);
        }
    }
}