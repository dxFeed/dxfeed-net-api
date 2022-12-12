#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using com.dxfeed.api;

namespace com.dxfeed.ipf
{
    /// <summary>
    /// Represents basic profile information about market instrument.
    /// Please see <a href="http://www.dxfeed.com/downloads/documentation/dxFeed_Instrument_Profile_Format.pdf">Instrument Profile Format documentation</a>
    /// for complete description.
    /// </summary>
    [Serializable()]
    public sealed class InstrumentProfile : IComparable<InstrumentProfile>
    {
        private string type = "";
        private string symbol = "";
        private string description = "";
        private string localSymbol = "";
        private string localDescription = "";
        private string country = "";
        private string opol = "";
        private string exchangeData = "";
        private string exchanges = "";
        private string currency = "";
        private string baseCurrency = "";
        private string cfi = "";
        private string isin = "";
        private string sedol = "";
        private string cusip = "";
        private int icb;
        private int sic;
        private double multiplier;
        private string product = "";
        private string underlying = "";
        private double spc;
        private string additionalUnderlyings = "";
        private string mmy = "";
        private int expiration;
        private int lastTrade;
        private double strike;
        private string optionType = "";
        private string expirationStyle = "";
        private string settlementStyle = "";
        private string priceIncrements = "";
        private string tradingHours = "";

        private Dictionary<string, string> customFields = new Dictionary<string, string>();

        /// <summary>
        /// Creates an instrument profile with default values.
        /// </summary>
        public InstrumentProfile() { }

        /// <summary>
        /// Creates an instrument profile as a copy of the specified instrument profile.
        /// </summary>
        /// <param name="ip">Ip an instrument profile to copy.</param>
        public InstrumentProfile(InstrumentProfile ip)
        {
            type = ip.type;
            symbol = ip.symbol;
            description = ip.description;
            localSymbol = ip.localSymbol;
            localDescription = ip.localDescription;
            country = ip.country;
            opol = ip.opol;
            exchangeData = ip.exchangeData;
            exchanges = ip.exchanges;
            currency = ip.currency;
            baseCurrency = ip.baseCurrency;
            cfi = ip.cfi;
            isin = ip.isin;
            sedol = ip.sedol;
            cusip = ip.cusip;
            icb = ip.icb;
            sic = ip.sic;
            multiplier = ip.multiplier;
            product = ip.product;
            underlying = ip.underlying;
            spc = ip.spc;
            additionalUnderlyings = ip.additionalUnderlyings;
            mmy = ip.mmy;
            expiration = ip.expiration;
            lastTrade = ip.lastTrade;
            strike = ip.strike;
            optionType = ip.optionType;
            expirationStyle = ip.expirationStyle;
            settlementStyle = ip.settlementStyle;
            priceIncrements = ip.priceIncrements;
            tradingHours = ip.tradingHours;

            customFields = new Dictionary<string, string>(ip.customFields);
        }

        /// <summary>
        /// Returns type of instrument.
        /// It takes precedence in conflict cases with other fields.
        /// It is a mandatory field. It may not be empty.
        /// Example: "STOCK", "FUTURE", "OPTION".
        /// </summary>
        /// <returns>Type of instrument.</returns>
        public string GetTypeName()
        {
            return type;
        }

        /// <summary>
        /// Changes type of instrument.
        /// It takes precedence in conflict cases with other fields.
        /// It is a mandatory field. It may not be empty.
        /// Example: "STOCK", "FUTURE", "OPTION".
        /// </summary>
        /// <param name="type">Type type of instrument.</param>
        public void SetType(string type)
        {
            this.type = type == null || string.IsNullOrEmpty(type) ? "" : type;
        }

        /// <summary>
        /// Returns identifier of instrument,
        /// preferable an international one in Latin alphabet.
        /// It is a mandatory field. It may not be empty.
        /// Example: "GOOG", "/YGM9", ".ZYEAD".
        /// </summary>
        /// <returns>Identifier of instrument.</returns>
        public string GetSymbol()
        {
            return symbol;
        }

        /// <summary>
        /// Changes identifier of instrument,
        /// preferable an international one in Latin alphabet.
        /// It is a mandatory field. It may not be empty.
        /// Example: "GOOG", "/YGM9", ".ZYEAD".
        /// </summary>
        /// <param name="symbol">Symbol identifier of instrument.</param>
        public void SetSymbol(string symbol)
        {
            this.symbol = symbol == null || string.IsNullOrEmpty(symbol) ? "" : symbol;
        }

        /// <summary>
        /// Returns description of instrument,
        /// preferable an international one in Latin alphabet.
        /// Example: "Google Inc.", "Mini Gold Futures,Jun-2009,ETH".
        /// </summary>
        /// <returns>Description of instrument.</returns>
        public string GetDescription()
        {
            return description;
        }

        /// <summary>
        /// Changes description of instrument,
        /// preferable an international one in Latin alphabet.
        /// Example: "Google Inc.", "Mini Gold Futures,Jun-2009,ETH".
        /// </summary>
        /// <param name="description">Description of instrument.</param>
        public void SetDescription(string description)
        {
            this.description = description == null || string.IsNullOrEmpty(description) ? "" : description;
        }

        /// <summary>
        /// Returns identifier of instrument in national language.
        /// It shall be empty if same as {@link #GetSymbol symbol}.
        /// </summary>
        /// <returns>Identifier of instrument in national language.</returns>
        public string GetLocalSymbol()
        {
            return localSymbol;
        }

        /// <summary>
        /// Changes identifier of instrument in national language.
        /// It shall be empty if same as {@link #SetSymbol symbol}.
        /// </summary>
        /// <param name="localSymbol">Identifier of instrument in national language.</param>
        public void SetLocalSymbol(string localSymbol)
        {
            this.localSymbol = localSymbol == null || string.IsNullOrEmpty(localSymbol) ? "" : localSymbol;
        }

        /// <summary>
        /// Returns description of instrument in national language.
        /// It shall be empty if same as {@link #GetDescription description}.
        /// </summary>
        /// <returns>Description of instrument in national language.</returns>
        public string GetLocalDescription()
        {
            return localDescription;
        }

        /// <summary>
        /// Changes description of instrument in national language.
        /// It shall be empty if same as {@link #GetDescription description}.
        /// </summary>
        /// <param name="localDescription">Description of instrument in national language.</param>
        public void SetLocalDescription(string localDescription)
        {
            this.localDescription = localDescription == null || string.IsNullOrEmpty(localDescription) ? "" : localDescription;
        }

        /// <summary>
        /// Returns country of origin (incorporation) of corresponding company or parent entity.
        /// It shall use two-letter country code from ISO 3166-1 standard.
        /// See <a href="http://en.wikipedia.org/wiki/ISO_3166-1">ISO 3166-1 on Wikipedia</a>.
        /// Example: "US", "RU".
        /// </summary>
        /// <returns>Country of origin (incorporation) of corresponding company or parent entity.</returns>
        public string GetCountry()
        {
            return country;
        }

        /// <summary>
        /// Changes country of origin (incorporation) of corresponding company or parent entity.
        /// It shall use two-letter country code from ISO 3166-1 standard.
        /// See <a href="http://en.wikipedia.org/wiki/ISO_3166-1">ISO 3166-1 on Wikipedia</a>.
        /// Example: "US", "RU".
        /// </summary>
        /// <param name="country">Country of origin (incorporation) of corresponding company or parent entity.</param>
        public void SetCountry(string country)
        {
            this.country = country == null || string.IsNullOrEmpty(country) ? "" : country;
        }

        /// <summary>
        /// Returns official Place Of Listing, the organization that have listed this instrument.
        /// Instruments with multiple listings shall use separate profiles for each listing.
        /// It shall use Market Identifier Code (MIC) from ISO 10383 standard.
        /// See <a href="http://en.wikipedia.org/wiki/ISO_10383">ISO 10383 on Wikipedia</a>
        /// or <a href="http://www.iso15022.org/MIC/homepageMIC.htm">MIC homepage</a>.
        /// Example: "XNAS", "RTSX"/
        /// </summary>
        /// <returns>Official Place Of Listing, the organization that have listed this instrument.</returns>
        public string GetOPOL()
        {
            return opol;
        }

        /// <summary>
        /// Changes official Place Of Listing, the organization that have listed this instrument.
        /// Instruments with multiple listings shall use separate profiles for each listing.
        /// It shall use Market Identifier Code (MIC) from ISO 10383 standard.
        /// See <a href="http://en.wikipedia.org/wiki/ISO_10383">ISO 10383 on Wikipedia</a>
        /// or <a href="http://www.iso15022.org/MIC/homepageMIC.htm">MIC homepage</a>.
        /// Example: "XNAS", "RTSX"/
        /// </summary>
        /// <param name="opol">Official Place Of Listing, the organization that have listed this instrument.</param>
        public void SetOPOL(string opol)
        {
            this.opol = opol == null || string.IsNullOrEmpty(opol) ? "" : opol;
        }

        /// <summary>
        /// Returns exchange-specific data required to properly identify instrument when communicating with exchange.
        /// It uses exchange-specific format.
        /// </summary>
        /// <returns>Exchange-specific data required to properly identify instrument when communicating with exchange.</returns>
        public string GetExchangeData()
        {
            return exchangeData;
        }

        /// <summary>
        /// Changes exchange-specific data required to properly identify instrument when communicating with exchange.
        /// It uses exchange-specific format.
        /// </summary>
        /// <param name="exchangeData">Exchange-specific data required to properly identify instrument when communicating with exchange.</param>
        public void SetExchangeData(string exchangeData)
        {
            this.exchangeData = exchangeData == null || string.IsNullOrEmpty(exchangeData) ? "" : exchangeData;
        }

        /// <summary>
        /// Returns list of exchanges where instrument is quoted or traded.
        /// Its shall use the following format:
        /// <pre>
        ///     &lt;VALUE> ::= &lt;empty> | &lt;LIST>
        ///     &lt;IST> ::= &lt;MIC> | &lt;MIC> &lt;semicolon> </pre>
        /// &lt;LIST> the list shall be sorted by MIC.
        /// Example: "ARCX;CBSX ;XNAS;XNYS".
        /// </summary>
        /// <returns>List of exchanges where instrument is quoted or traded.</returns>
        public string GetExchanges()
        {
            return exchanges;
        }

        /// <summary>
        /// Changes list of exchanges where instrument is quoted or traded.
        /// It shall use the following format:
        /// <pre>
        ///     &lt;VALUE> ::= &lt;empty> | &lt;LIST>
        ///     &lt;IST> ::= &lt;MIC> | &lt;MIC> &lt;semicolon> </pre>
        /// &lt;LIST> the list shall be sorted by MIC.
        /// Example: "ARCX;CBSX ;XNAS;XNYS".
        /// </summary>
        /// <param name="exchanges">List of exchanges where instrument is quoted or traded.</param>
        public void SetExchanges(string exchanges)
        {
            this.exchanges = exchanges == null || string.IsNullOrEmpty(exchanges) ? "" : exchanges;
        }

        /// <summary>
        /// Returns currency of quotation, pricing and trading.
        /// It shall use three-letter currency code from ISO 4217 standard.
        /// See <a href="http://en.wikipedia.org/wiki/ISO_4217">ISO 4217 on Wikipedia</a>.
        /// Example: "USD", "RUB".
        /// </summary>
        /// <returns>Currency of quotation, pricing and trading.</returns>
        public string GetCurrency()
        {
            return currency;
        }

        /// <summary>
        /// Changes currency of quotation, pricing and trading.
        /// It shall use three-letter currency code from ISO 4217 standard.
        /// See <a href="http://en.wikipedia.org/wiki/ISO_4217">ISO 4217 on Wikipedia</a>.
        /// Example: "USD", "RUB".
        /// </summary>
        /// <param name="currency">Currency currency of quotation, pricing and trading.</param>
        public void SetCurrency(string currency)
        {
            this.currency = currency == null || string.IsNullOrEmpty(currency) ? "" : currency;
        }

        /// <summary>
        /// Returns base currency of currency pair (FOREX instruments).
        /// It shall use three-letter currency code similarly to {@link #GetCurrency currency}.
        /// </summary>
        /// <returns>Base currency of currency pair (FOREX instruments).</returns>
        public string GetBaseCurrency()
        {
            return baseCurrency;
        }

        /// <summary>
        /// Changes base currency of currency pair (FOREX instruments).
        /// It shall use three-letter currency code similarly to {@link #SetCurrency currency}.
        /// </summary>
        /// <param name="baseCurrency">BaseCurrency base currency of currency pair (FOREX instruments).</param>
        public void SetBaseCurrency(string baseCurrency)
        {
            this.baseCurrency = baseCurrency == null || string.IsNullOrEmpty(baseCurrency) ? "" : baseCurrency;
        }

        /// <summary>
        /// Returns Classification of Financial Instruments code.
        /// It is a mandatory field for OPTION instruments as it is the only way to distinguish Call/Put type,
        /// American/European exercise, Cash/Physical delivery.
        /// It shall use six-letter CFI code from ISO 10962 standard.
        /// It is allowed to use 'X' extensively and to omit trailing letters (assumed to be 'X').
        /// See <a href="http://en.wikipedia.org/wiki/ISO_10962">ISO 10962 on Wikipedia</a>.
        /// Example: "ESNTPB", "ESXXXX", "ES" , "OPASPS".
        /// </summary>
        /// <returns>CFI code.</returns>
        public string GetCFI()
        {
            return cfi;
        }

        /// <summary>
        /// Changes Classification of Financial Instruments code.
        /// It is a mandatory field for OPTION instruments as it is the only way to distinguish Call/Put type,
        /// American/European exercise, Cash/Physical delivery.
        /// It shall use six-letter CFI code from ISO 10962 standard.
        /// It is allowed to use 'X' extensively and to omit trailing letters (assumed to be 'X').
        /// See <a href="http://en.wikipedia.org/wiki/ISO_10962">ISO 10962 on Wikipedia</a>.
        /// Example: "ESNTPB", "ESXXXX", "ES" , "OPASPS".
        /// </summary>
        /// <param name="cfi">CFI code.</param>
        public void SetCFI(string cfi)
        {
            this.cfi = cfi == null || string.IsNullOrEmpty(cfi) ? "" : cfi;
        }

        /// <summary>
        /// Returns International Securities Identifying Number.
        /// It shall use twelve-letter code from ISO 6166 standard.
        /// See <a href="http://en.wikipedia.org/wiki/ISO_6166">ISO 6166 on Wikipedia</a>
        /// or <a href="http://en.wikipedia.org/wiki/International_Securities_Identifying_Number">ISIN on Wikipedia</a>.
        /// Example: "DE0007100000", "US38259P5089".
        /// @return International Securities Identifying Number.
        /// </summary>
        /// <returns></returns>
        public string GetISIN()
        {
            return isin;
        }

        /// <summary>
        /// Changes International Securities Identifying Number.
        /// It shall use twelve-letter code from ISO 6166 standard.
        /// See <a href="http://en.wikipedia.org/wiki/ISO_6166">ISO 6166 on Wikipedia</a>
        /// or <a href="http://en.wikipedia.org/wiki/International_Securities_Identifying_Number">ISIN on Wikipedia</a>.
        /// Example: "DE0007100000", "US38259P5089".
        /// </summary>
        /// <param name="isin">International Securities Identifying Number.</param>
        public void SetISIN(string isin)
        {
            this.isin = isin == null || string.IsNullOrEmpty(isin) ? "" : isin;
        }

        /// <summary>
        /// Returns Stock Exchange Daily Official List.
        /// It shall use seven-letter code assigned by London Stock Exchange.
        /// See <a href="http://en.wikipedia.org/wiki/SEDOL">SEDOL on Wikipedia</a> or
        /// <a href="http://www.londonstockexchange.com/en-gb/products/informationproducts/sedol/">SEDOL on LSE</a>.
        /// Example: "2310967", "5766857".
        /// </summary>
        /// <returns>Exchange Daily Official List.</returns>
        public string GetSEDOL()
        {
            return sedol;
        }

        /// <summary>
        /// Changes Stock Exchange Daily Official List.
        /// It shall use seven-letter code assigned by London Stock Exchange.
        /// See <a href="http://en.wikipedia.org/wiki/SEDOL">SEDOL on Wikipedia</a> or
        /// <a href="http://www.londonstockexchange.com/en-gb/products/informationproducts/sedol/">SEDOL on LSE</a>.
        /// Example: "2310967", "5766857".
        /// </summary>
        /// <param name="sedol">Stock Exchange Daily Official List.</param>
        public void SetSEDOL(string sedol)
        {
            this.sedol = sedol == null || string.IsNullOrEmpty(sedol) ? "" : sedol;
        }

        /// <summary>
        /// Returns Committee on Uniform Security Identification Procedures code.
        /// It shall use nine-letter code assigned by CUSIP Services Bureau.
        /// See <a href="http://en.wikipedia.org/wiki/CUSIP">CUSIP on Wikipedia</a>.
        /// Example: "38259P508".
        /// </summary>
        /// <returns>CUSIP code.</returns>
        public string GetCUSIP()
        {
            return cusip;
        }

        /// <summary>
        /// Changes Committee on Uniform Security Identification Procedures code.
        /// It shall use nine-letter code assigned by CUSIP Services Bureau.
        /// See <a href="http://en.wikipedia.org/wiki/CUSIP">CUSIP on Wikipedia</a>.
        /// Example: "38259P508".
        /// </summary>
        /// <param name="cusip">CUSIP code.</param>
        public void SetCUSIP(string cusip)
        {
            this.cusip = cusip == null || string.IsNullOrEmpty(cusip) ? "" : cusip;
        }

        /// <summary>
        /// Returns Industry Classification Benchmark.
        /// It shall use four-digit number from ICB catalog.
        /// See <a href="http://en.wikipedia.org/wiki/Industry_Classification_Benchmark">ICB on Wikipedia</a>
        /// or <a href="http://www.icbenchmark.com/">ICB homepage</a>.
        /// Example: "9535".
        /// </summary>
        /// <returns>Industry Classification Benchmark.</returns>
        public int GetICB()
        {
            return icb;
        }

        /// <summary>
        /// Changes Industry Classification Benchmark.
        /// It shall use four-digit number from ICB catalog.
        /// See <a href="http://en.wikipedia.org/wiki/Industry_Classification_Benchmark">ICB on Wikipedia</a>
        /// or <a href="http://www.icbenchmark.com/">ICB homepage</a>.
        /// Example: "9535".
        /// </summary>
        /// <param name="icb">Industry Classification Benchmark.</param>
        public void SetICB(int icb)
        {
            this.icb = icb;
        }

        /// <summary>
        /// Returns Standard Industrial Classification.
        /// It shall use four-digit number from SIC catalog.
        /// See <a href="http://en.wikipedia.org/wiki/Standard_Industrial_Classification">SIC on Wikipedia</a>
        /// or <a href="https://www.osha.gov/pls/imis/sic_manual.html">SIC structure</a>.
        /// Example: "7371".
        /// </summary>
        /// <returns>Standard Industrial Classification.</returns>
        public int GetSIC()
        {
            return sic;
        }

        /// <summary>
        /// Changes Standard Industrial Classification.
        /// It shall use four-digit number from SIC catalog.
        /// See <a href="http://en.wikipedia.org/wiki/Standard_Industrial_Classification">SIC on Wikipedia</a>
        /// or <a href="https://www.osha.gov/pls/imis/sic_manual.html">SIC structure</a>.
        /// Example: "7371".
        /// </summary>
        /// <param name="sic">Standard Industrial Classification.</param>
        public void SetSIC(int sic)
        {
            this.sic = sic;
        }

        /// <summary>
        /// Returns market value multiplier.
        /// Example: 100, 33.2.
        /// </summary>
        /// <returns>Market value multiplier.</returns>
        public double GetMultiplier()
        {
            return multiplier;
        }

        /// <summary>
        /// Changes market value multiplier.
        /// Example: 100, 33.2.
        /// </summary>
        /// <param name="multiplier">Multiplier market value multiplier.</param>
        public void SetMultiplier(double multiplier)
        {
            this.multiplier = multiplier;
        }

        /// <summary>
        /// Returns product for futures and options on futures (underlying asset name).
        /// Example: "/YG".
        /// </summary>
        /// <returns>Product for futures and options on futures (underlying asset name).</returns>
        public string GetProduct()
        {
            return product;
        }

        /// <summary>
        /// Changes product for futures and options on futures (underlying asset name).
        /// Example: "/YG".
        /// </summary>
        /// <param name="product">Product product for futures and options on futures (underlying asset name).</param>
        public void SetProduct(string product)
        {
            this.product = product == null || string.IsNullOrEmpty(product) ? "" : product;
        }

        /// <summary>
        /// Returns primary underlying symbol for options.
        /// Example: "C", "/YGM9"
        /// </summary>
        /// <returns>Primary underlying symbol for options.</returns>
        public string GetUnderlying()
        {
            return underlying;
        }

        /// <summary>
        /// Changes primary underlying symbol for options.
        /// Example: "C", "/YGM9"
        /// </summary>
        /// <param name="underlying">Underlying primary underlying symbol for options.</param>
        public void SetUnderlying(string underlying)
        {
            this.underlying = underlying == null || string.IsNullOrEmpty(underlying) ? "" : underlying;
        }

        /// <summary>
        /// Returns shares per contract for options.
        /// Example: 1, 100.
        /// </summary>
        /// <returns>Shares per contract for options.</returns>
        public double GetSPC()
        {
            return spc;
        }

        /// <summary>
        /// Changes shares per contract for options.
        /// Example: 1, 100.
        /// </summary>
        /// <param name="spc">Shares per contract for options.</param>
        public void SetSPC(double spc)
        {
            this.spc = spc;
        }

        /// <summary>
        /// Returns additional underlyings for options, including additional cash.
        /// It shall use following format:
        /// <pre>
        ///     &lt;VALUE> ::= &lt;empty> | &lt;LIST>
        ///     &lt;LIST> ::= &lt;AU> | &lt;AU> &lt;semicolon> &lt;space> &lt;LIST>
        ///     &lt;AU> ::= &lt;UNDERLYING> &lt;space> &lt;SPC> </pre>
        /// the list shall be sorted by &lt;UNDERLYING>.
        /// Example: "SE 50", "FIS 53; US$ 45.46".
        /// </summary>
        /// <returns>Additional underlyings for options, including additional cash.</returns>
        public string GetAdditionalUnderlyings()
        {
            return additionalUnderlyings;
        }

        /// <summary>
        /// Changes additional underlyings for options, including additional cash.
        /// It shall use following format:
        /// <pre>
        ///     &lt;VALUE> ::= &lt;empty> | &lt;LIST>
        ///     &lt;LIST> ::= &lt;AU> | &lt;AU> &lt;semicolon> &lt;space> &lt;LIST>
        ///     &lt;AU> ::= &lt;UNDERLYING> &lt;space> &lt;SPC> </pre>
        /// the list shall be sorted by &lt;UNDERLYING>.
        /// Example: "SE 50", "FIS 53; US$ 45.46".
        /// </summary>
        /// <param name="additionalUnderlyings">AdditionalUnderlyings additional underlyings for options, including additional cash.</param>
        public void SetAdditionalUnderlyings(string additionalUnderlyings)
        {
            this.additionalUnderlyings = additionalUnderlyings == null || string.IsNullOrEmpty(additionalUnderlyings) ? "" : additionalUnderlyings;
        }

        /// <summary>
        /// Returns maturity month-year as provided for corresponding FIX tag (200).
        /// It can use several different formats depending on data source:
        /// <ul>
        /// <li>YYYYMM – if only year and month are specified</li>
        /// <li>YYYYMMDD – if full date is specified</li>
        /// <li>YYYYMMwN – if week number (within a month) is specified</li>
        /// </ul>
        /// </summary>
        /// <returns>Maturity month-year as provided for corresponding FIX tag (200).</returns>
        public string GetMMY()
        {
            return mmy;
        }

        /// <summary>
        /// Changes maturity month-year as provided for corresponding FIX tag (200).
        /// It can use several different formats depending on data source:
        /// <ul>
        /// <li>YYYYMM – if only year and month are specified</li>
        /// <li>YYYYMMDD – if full date is specified</li>
        /// <li>YYYYMMwN – if week number (within a month) is specified</li>
        /// </ul>
        /// </summary>
        /// <param name="mmy">Maturity month-year as provided for corresponding FIX tag (200).</param>
        public void SetMMY(string mmy)
        {
            this.mmy = mmy == null || string.IsNullOrEmpty(mmy) ? "" : mmy;
        }

        /// <summary>
        /// Returns day id of expiration.
        /// Example: {@link com.dxfeed.util.DayUtil#GetDayIdByYearMonthDay DayUtil.GetDayIdByYearMonthDay}(20090117).
        /// </summary>
        /// <returns>Day id of expiration.</returns>
        public int GetExpiration()
        {
            return expiration;
        }

        /// <summary>
        /// Changes day id of expiration.
        /// Example: {@link com.dxfeed.util.DayUtil#GetDayIdByYearMonthDay DayUtil.GetDayIdByYearMonthDay}(20090117).
        /// </summary>
        /// <param name="expiration">Expiration day id of expiration.</param>
        public void SetExpiration(int expiration)
        {
            this.expiration = expiration;
        }

        /// <summary>
        /// Returns day id of last trading day.
        /// Example: {@link com.dxfeed.util.DayUtil#GetDayIdByYearMonthDay DayUtil.GetDayIdByYearMonthDay}(20090116).
        /// </summary>
        /// <returns>Day id of last trading day.</returns>
        public int GetLastTrade()
        {
            return lastTrade;
        }

        /// <summary>
        /// Changes day id of last trading day.
        /// Example: {@link com.dxfeed.util.DayUtil#GetDayIdByYearMonthDay DayUtil.GetDayIdByYearMonthDay}(20090116).
        /// </summary>
        /// <param name="lastTrade">Day id of last trading day.</param>
        public void SetLastTrade(int lastTrade)
        {
            this.lastTrade = lastTrade;
        }

        /// <summary>
        /// Returns strike price for options.
        /// Example: 80, 22.5.
        /// </summary>
        /// <returns>Strike price for options.</returns>
        public double GetStrike()
        {
            return strike;
        }

        /// <summary>
        /// Changes strike price for options.
        /// Example: 80, 22.5.
        /// </summary>
        /// <param name="strike">Strike strike price for options.</param>
        public void SetStrike(double strike)
        {
            this.strike = strike;
        }

        /// <summary>
        /// Returns type of option.
        /// It shall use one of following values:
        /// <ul>
        /// <li>STAN = Standard Options</li>
        /// <li>LEAP = Long-term Equity AnticiPation Securities</li>
        /// <li>SDO = Special Dated Options</li>
        /// <li>BINY = Binary Options</li>
        /// <li>FLEX = FLexible EXchange Options</li>
        /// <li>VSO = Variable Start Options</li>
        /// <li>RNGE = Range</li>
        /// </ul>
        /// </summary>
        /// <returns>Type of option.</returns>
        public string GetOptionType()
        {
            return optionType;
        }

        /// <summary>
        /// Changes type of option.
        /// It shall use one of following values:
        /// <ul>
        /// <li>STAN = Standard Options</li>
        /// <li>LEAP = Long-term Equity AnticiPation Securities</li>
        /// <li>SDO = Special Dated Options</li>
        /// <li>BINY = Binary Options</li>
        /// <li>FLEX = FLexible EXchange Options</li>
        /// <li>VSO = Variable Start Options</li>
        /// <li>RNGE = Range</li>
        /// </ul>
        /// </summary>
        /// <param name="optionType">Type of option.</param>
        public void SetOptionType(string optionType)
        {
            this.optionType = optionType == null || string.IsNullOrEmpty(optionType) ? "" : optionType;
        }

        /// <summary>
        /// Returns expiration cycle style, such as "Weeklys", "Quarterlys".
        /// </summary>
        /// <returns>Expiration cycle style.</returns>
        public string GetExpirationStyle()
        {
            return expirationStyle;
        }

        /// <summary>
        /// Returns expiration cycle style, such as "Weeklys", "Quarterlys".
        /// </summary>
        /// <param name="expirationStyle">Expiration cycle style.</param>
        public void SetExpirationStyle(string expirationStyle)
        {
            this.expirationStyle = expirationStyle == null || string.IsNullOrEmpty(expirationStyle) ? "" : expirationStyle;
        }

        /// <summary>
        /// Returns settlement price determination style, such as "Open", "Close".
        /// </summary>
        /// <returns>Settlement price determination style.</returns>
        public string GetSettlementStyle()
        {
            return settlementStyle;
        }

        /// <summary>
        /// Changes settlement price determination style, such as "Open", "Close".
        /// </summary>
        /// <param name="settlementStyle">Settlement price determination style.</param>
        public void SetSettlementStyle(string settlementStyle)
        {
            this.settlementStyle = settlementStyle == null || string.IsNullOrEmpty(settlementStyle) ? "" : settlementStyle;
        }

        /// <summary>
        /// Returns minimum allowed price increments with corresponding price ranges.
        /// It shall use following format:
        /// <pre>
        ///     &lt;VALUE> ::= &lt;empty> | &lt;LIST>
        ///     &lt;LIST> ::= &lt;INCREMENT> | &lt;RANGE> &lt;semicolon> &lt;space> &lt;LIST>
        ///     &lt;RANGE> ::= &lt;INCREMENT> &lt;space> &lt;UPPER_LIMIT>
        /// </pre>
        /// the list shall be sorted by &lt;UPPER_LIMIT>.
        /// Example: "0.25", "0.01 3; 0.05".
        /// </summary>
        /// <returns>Minimum allowed price increments with corresponding price ranges.</returns>
        public string GetPriceIncrements()
        {
            return priceIncrements;
        }

        /// <summary>
        /// Changes minimum allowed price increments with corresponding price ranges.
        /// It shall use following format:
        /// <pre>
        ///     &lt;VALUE> ::= &lt;empty> | &lt;LIST>
        ///     &lt;LIST> ::= &lt;INCREMENT> | &lt;RANGE> &lt;semicolon> &lt;space> &lt;LIST>
        ///     &lt;RANGE> ::= &lt;INCREMENT> &lt;space> &lt;UPPER_LIMIT>
        /// </pre>
        /// the list shall be sorted by &lt;UPPER_LIMIT>.
        /// Example: "0.25", "0.01 3; 0.05".
        /// </summary>
        /// <param name="priceIncrements">Minimum allowed price increments with corresponding price ranges.</param>
        public void SetPriceIncrements(string priceIncrements)
        {
            this.priceIncrements = priceIncrements == null || string.IsNullOrEmpty(priceIncrements) ? "" : priceIncrements;
        }

        /// <summary>
        /// Returns trading hours specification.
        /// See Schedule#getInstance(string).
        /// </summary>
        /// <returns>Trading hours specification.</returns>
        public string GetTradingHours()
        {
            return tradingHours;
        }

        /// <summary>
        /// Changes trading hours specification.
        /// See Schedule#getInstance(string).
        /// </summary>
        /// <param name="tradingHours">Trading hours specification.</param>
        public void SetTradingHours(string tradingHours)
        {
            this.tradingHours = tradingHours == null || string.IsNullOrEmpty(tradingHours) ? "" : tradingHours;
        }

        /// <summary>
        /// Returns custom field value with a specified name.
        /// </summary>
        /// <param name="name">Name of custom field.</param>
        /// <returns>Custom field value with a specified name.</returns>
        private string GetCustomField(string name)
        {
            if (customFields.ContainsKey(name))
                return customFields[name];
            return null;
        }

        /// <summary>
        /// Changes custom field value with a specified name.
        /// </summary>
        /// <param name="name">Name of custom field.</param>
        /// <param name="value">Custom field value.</param>
        private void SetCustomField(string name, string value)
        {
            customFields[name] = value;
        }

        /// <summary>
        /// Returns field value with a specified name.
        /// </summary>
        /// <param name="name">Name of field.</param>
        /// <returns>Field value.</returns>
        /// <exception cref="System.ArgumentNullException">If name is null.</exception>
        /// <exception cref="System.InvalidOperationException">Can't format certain field.</exception>
        public string GetField(string name)
        {
            InstrumentProfileField ipf = InstrumentProfileField.Find(name);
            if (ipf != null)
                return ipf.GetField(this);
            string value = GetCustomField(name);
            return value == null ? "" : value;
        }

        /// <summary>
        /// Changes field value with a specified name.
        /// </summary>
        /// <param name="name">Name of field.</param>
        /// <param name="value">Field value.</param>
        /// <exception cref="System.ArgumentNullException">If name is null.</exception>
        /// <exception cref="System.InvalidOperationException">If text uses wrong format or contains invalid values.</exception>
        public void SetField(string name, string value)
        {
            InstrumentProfileField ipf = InstrumentProfileField.Find(name);
            if (ipf != null)
                ipf.SetField(this, value);
            else
                SetCustomField(name, value == null || string.IsNullOrEmpty(value) ? "" : value);
        }

        /// <summary>
        /// Returns numeric field value with a specified name.
        /// </summary>
        /// <param name="name">Name of field.</param>
        /// <returns>Field value.</returns>
        /// <exception cref="System.ArgumentException">If this field has no numeric representation.</exception>
        /// <exception cref="System.InvalidOperationException">If number formatter error occurs.</exception>
        public double GetNumericField(string name)
        {
            try
            {
                InstrumentProfileField ipf = InstrumentProfileField.Find(name);
                if (ipf != null)
                    return ipf.GetNumericField(this);
                string value = GetCustomField(name);
                return value == null || string.IsNullOrEmpty(value) ? 0 :
                    value.Length == 10 && value[4] == '-' && value[7] == '-' ? InstrumentProfileField.ParseDate(value) :
                    InstrumentProfileField.ParseNumber(value);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException("GetNumericField failed: " + exc);
            }
        }

        /// <summary>
        /// Changes numeric field value with a specified name.
        /// </summary>
        /// <param name="name">Name of field.</param>
        /// <param name="value">Field value.</param>
        /// <exception cref="System.ArgumentException">If this field has no numeric representation.</exception>
        /// <exception cref="System.InvalidOperationException">If number formatter error occurs.</exception>
        public void SetNumericField(string name, double value)
        {
            InstrumentProfileField ipf = InstrumentProfileField.Find(name);
            if (ipf != null)
                ipf.SetNumericField(this, value);
            else
                SetCustomField(name, InstrumentProfileField.FormatNumber(value));
        }

        /// <summary>
        /// Returns day id value for a date field with a specified name.
        /// </summary>
        /// <param name="name">Name of field.</param>
        /// <returns>Day id value.</returns>
        /// <exception cref="System.ArgumentException">If this field has no numeric (date) representation.</exception>
        /// <exception cref="System.InvalidOperationException">If number formatter error occurs.</exception>
        public int GetDateField(string name)
        {
            try
            {
                InstrumentProfileField ipf = InstrumentProfileField.Find(name);
                if (ipf != null)
                    return (int)ipf.GetNumericField(this);
                string value = GetCustomField(name);
                return value == null || string.IsNullOrEmpty(value) ? 0 : InstrumentProfileField.ParseDate(value);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException("GetDateField failed: " + exc);
            }
        }

        /// <summary>
        /// Changes day id value for a date field with a specified name.
        /// </summary>
        /// <param name="name">Name of field.</param>
        /// <param name="value">Day id value.</param>
        /// <exception cref="System.ArgumentException">If this field has no numeric (date) representation.</exception>
        /// <exception cref="System.InvalidOperationException">If number formatter error occurs.</exception>
        public void SetDateField(string name, int value)
        {
            try
            {
                InstrumentProfileField ipf = InstrumentProfileField.Find(name);
                if (ipf != null)
                    ipf.SetNumericField(this, value);
                else
                    SetCustomField(name, InstrumentProfileField.FormatDate(value));
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException("SetDateField failed: " + exc);
            }
        }

        /// <summary>
        /// Adds names of non-empty custom fields to specified collection.
        /// </summary>
        /// <param name="targetFieldNames"></param>
        /// <returns><tt>true</tt> if `targetFieldNames` changed as a result of the call</returns>
        public bool AddNonEmptyCustomFieldNames(ICollection<string> targetFieldNames)
        {
            int size = targetFieldNames.Count;
            foreach (KeyValuePair<string, string> item in this.customFields)
            {
                string name = item.Key; // Atomic read.
                string value = item.Value; // Atomic read.
                if (name != null && value != null && value.Length > 0)
                    targetFieldNames.Add(name);
            }
            return targetFieldNames.Count > size;
        }

        /// <summary>
        /// Indicates whether some other object is "equal to" this one.
        /// </summary>
        /// <param name="o">The reference object with which to compare.</param>
        /// <returns>`true` if this object is the same as the obj argument; `false` otherwise.</returns>
        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (!(o.GetType() == typeof(InstrumentProfile))) return false;
            InstrumentProfile that = (InstrumentProfile)o;
            if (!type.Equals(that.type)) return false;
            if (!symbol.Equals(that.symbol)) return false;
            if (!description.Equals(that.description)) return false;
            if (!localSymbol.Equals(that.localSymbol)) return false;
            if (!localDescription.Equals(that.localDescription)) return false;
            if (!country.Equals(that.country)) return false;
            if (!opol.Equals(that.opol)) return false;
            if (!exchangeData.Equals(that.exchangeData)) return false;
            if (!exchanges.Equals(that.exchanges)) return false;
            if (!currency.Equals(that.currency)) return false;
            if (!baseCurrency.Equals(that.baseCurrency)) return false;
            if (!cfi.Equals(that.cfi)) return false;
            if (!isin.Equals(that.isin)) return false;
            if (!sedol.Equals(that.sedol)) return false;
            if (!cusip.Equals(that.cusip)) return false;
            if (icb != that.icb) return false;
            if (sic != that.sic) return false;
            if (!Tools.IsEquals(that.multiplier, multiplier)) return false;
            if (!product.Equals(that.product)) return false;
            if (!underlying.Equals(that.underlying)) return false;
            if (!Tools.IsEquals(that.spc, spc)) return false;
            if (!additionalUnderlyings.Equals(that.additionalUnderlyings)) return false;
            if (!mmy.Equals(that.mmy)) return false;
            if (expiration != that.expiration) return false;
            if (lastTrade != that.lastTrade) return false;
            if (!Tools.IsEquals(that.strike, strike)) return false;
            if (!optionType.Equals(that.optionType)) return false;
            if (!expirationStyle.Equals(that.expirationStyle)) return false;
            if (!settlementStyle.Equals(that.settlementStyle)) return false;
            if (!priceIncrements.Equals(that.priceIncrements)) return false;
            if (!tradingHours.Equals(that.tradingHours)) return false;
            return CustomEquals(customFields, that.customFields);
        }

        /// <summary>
        /// Returns a hash code value for the object.
        /// </summary>
        /// <returns>A hash code value for this object.</returns>
        public override int GetHashCode()
        {
            int result;
            long temp;
            result = type.GetHashCode();
            result = 31 * result + symbol.GetHashCode();
            result = 31 * result + description.GetHashCode();
            result = 31 * result + localSymbol.GetHashCode();
            result = 31 * result + localDescription.GetHashCode();
            result = 31 * result + country.GetHashCode();
            result = 31 * result + opol.GetHashCode();
            result = 31 * result + exchangeData.GetHashCode();
            result = 31 * result + exchanges.GetHashCode();
            result = 31 * result + currency.GetHashCode();
            result = 31 * result + baseCurrency.GetHashCode();
            result = 31 * result + cfi.GetHashCode();
            result = 31 * result + isin.GetHashCode();
            result = 31 * result + sedol.GetHashCode();
            result = 31 * result + cusip.GetHashCode();
            result = 31 * result + icb;
            result = 31 * result + sic;
            temp = BitConverter.DoubleToInt64Bits(multiplier);
            result = 31 * result + (int)(temp ^ (long)((ulong)temp >> 32));
            result = 31 * result + product.GetHashCode();
            result = 31 * result + underlying.GetHashCode();
            temp = BitConverter.DoubleToInt64Bits(spc);
            result = 31 * result + (int)(temp ^ (long)((ulong)temp >> 32));
            result = 31 * result + additionalUnderlyings.GetHashCode();
            result = 31 * result + mmy.GetHashCode();
            result = 31 * result + expiration;
            result = 31 * result + lastTrade;
            temp = BitConverter.DoubleToInt64Bits(strike);
            result = 31 * result + (int)(temp ^ (long)((ulong)temp >> 32));
            result = 31 * result + optionType.GetHashCode();
            result = 31 * result + expirationStyle.GetHashCode();
            result = 31 * result + settlementStyle.GetHashCode();
            result = 31 * result + priceIncrements.GetHashCode();
            result = 31 * result + tradingHours.GetHashCode();
            return 31 * result + CustomHashCode(customFields);
        }

        private static int CustomHashCode(Dictionary<string, string> dict)
        {
            int hash = 0;
            foreach (var item in dict)
            {
                string key = item.Key;
                string value = item.Value;
                if (key != null && value != null && !string.IsNullOrEmpty(value))
                    hash += key.GetHashCode() ^ value.GetHashCode();
            }
            return hash;
        }

        private static bool CustomEquals<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            if (dict1 == dict2) return true;
            if ((dict1 == null) || (dict2 == null)) return false;
            if (dict1.Count != dict2.Count) return false;

            var valueComparer = EqualityComparer<TValue>.Default;

            foreach (var item in dict1)
            {
                TValue value2;
                if (!dict2.TryGetValue(item.Key, out value2)) return false;
                if (!valueComparer.Equals(item.Value, value2)) return false;
            }
            return true;
        }

        /// <summary>
        /// Compares this profile with the specified profile for order. Returns a negative integer, zero,
        /// or a positive integer as this object is less than, equal to, or greater than the specified object.
        /// <p/>
        /// The natural ordering implied by this method is designed for convenient data representation
        /// in a file and shall not be used for business purposes.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public int CompareTo(InstrumentProfile ip)
        {
            int i = InstrumentProfileType.CompareTypes(type, ip.type);
            if (i != 0)
                return i;
            i = product.CompareTo(ip.product);
            if (i != 0)
                return i;
            i = underlying.CompareTo(ip.underlying);
            if (i != 0)
                return i;
            i = lastTrade > ip.lastTrade ? 1 : lastTrade < ip.lastTrade ? -1 : 0;
            if (i != 0)
                return i;
            i = strike > ip.strike ? 1 : strike < ip.strike ? -1 : 0;
            if (i != 0)
                return i;
            i = symbol.CompareTo(ip.symbol);
            if (i != 0)
                return i;
            return 0;
        }

        /// <summary>
        /// Returns a string representation of the instrument profile.
        /// </summary>
        /// <returns>String representation of the instrument profile.</returns>
        public override string ToString()
        {
            return type + " " + symbol;
        }
    }
}
