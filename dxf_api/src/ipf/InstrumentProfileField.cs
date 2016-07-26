/*
 * QDS - Quick Data Signalling Library
 * Copyright (C) 2002-2015 Devexperts LLC
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at
 * http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading;
using com.dxfeed.api;

namespace com.dxfeed.ipf {

    /// <summary>
    /// Defines standard fields of {@link InstrumentProfile} and provides data access methods.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    class InstrumentProfileField {
        public static readonly InstrumentProfileField TYPE = new InstrumentProfileField(F_TYPE);
        public static readonly InstrumentProfileField SYMBOL = new InstrumentProfileField(F_SYMBOL);
        public static readonly InstrumentProfileField DESCRIPTION = new InstrumentProfileField(F_DESCRIPTION);
        public static readonly InstrumentProfileField LOCAL_SYMBOL = new InstrumentProfileField(F_LOCAL_SYMBOL);
        public static readonly InstrumentProfileField LOCAL_DESCRIPTION = new InstrumentProfileField(F_LOCAL_DESCRIPTION);
        public static readonly InstrumentProfileField COUNTRY = new InstrumentProfileField(F_COUNTRY);
        public static readonly InstrumentProfileField OPOL = new InstrumentProfileField(F_OPOL);
        public static readonly InstrumentProfileField EXCHANGE_DATA = new InstrumentProfileField(F_EXCHANGE_DATA);
        public static readonly InstrumentProfileField EXCHANGES = new InstrumentProfileField(F_EXCHANGES);
        public static readonly InstrumentProfileField CURRENCY = new InstrumentProfileField(F_CURRENCY);
        public static readonly InstrumentProfileField BASE_CURRENCY = new InstrumentProfileField(F_BASE_CURRENCY);
        public static readonly InstrumentProfileField CFI = new InstrumentProfileField(F_CFI);
        public static readonly InstrumentProfileField ISIN = new InstrumentProfileField(F_ISIN);
        public static readonly InstrumentProfileField SEDOL = new InstrumentProfileField(F_SEDOL);
        public static readonly InstrumentProfileField CUSIP = new InstrumentProfileField(F_CUSIP);
        public static readonly InstrumentProfileField ICB = new InstrumentProfileField(F_ICB);
        public static readonly InstrumentProfileField SIC = new InstrumentProfileField(F_SIC);
        public static readonly InstrumentProfileField MULTIPLIER = new InstrumentProfileField(F_MULTIPLIER);
        public static readonly InstrumentProfileField PRODUCT = new InstrumentProfileField(F_PRODUCT);
        public static readonly InstrumentProfileField UNDERLYING = new InstrumentProfileField(F_UNDERLYING);
        public static readonly InstrumentProfileField SPC = new InstrumentProfileField(F_SPC);
        public static readonly InstrumentProfileField ADDITIONAL_UNDERLYINGS = new InstrumentProfileField(F_ADDITIONAL_UNDERLYINGS);
        public static readonly InstrumentProfileField MMY = new InstrumentProfileField(F_MMY);
        public static readonly InstrumentProfileField EXPIRATION = new InstrumentProfileField(F_EXPIRATION);
        public static readonly InstrumentProfileField LAST_TRADE = new InstrumentProfileField(F_LAST_TRADE);
        public static readonly InstrumentProfileField STRIKE = new InstrumentProfileField(F_STRIKE);
        public static readonly InstrumentProfileField OPTION_TYPE = new InstrumentProfileField(F_OPTION_TYPE);
        public static readonly InstrumentProfileField EXPIRATION_STYLE = new InstrumentProfileField(F_EXPIRATION_STYLE);
        public static readonly InstrumentProfileField SETTLEMENT_STYLE = new InstrumentProfileField(F_SETTLEMENT_STYLE);
        public static readonly InstrumentProfileField PRICE_INCREMENTS = new InstrumentProfileField(F_PRICE_INCREMENTS);
        public static readonly InstrumentProfileField TRADING_HOURS = new InstrumentProfileField(F_TRADING_HOURS);

        private const string F_TYPE = "TYPE";
        private const string F_SYMBOL = "SYMBOL";
        private const string F_DESCRIPTION = "DESCRIPTION";
        private const string F_LOCAL_SYMBOL = "LOCAL_SYMBOL";
        private const string F_LOCAL_DESCRIPTION = "LOCAL_DESCRIPTION";
        private const string F_COUNTRY = "COUNTRY";
        private const string F_OPOL = "OPOL";
        private const string F_EXCHANGE_DATA = "EXCHANGE_DATA";
        private const string F_EXCHANGES = "EXCHANGES";
        private const string F_CURRENCY = "CURRENCY";
        private const string F_BASE_CURRENCY = "BASE_CURRENCY";
        private const string F_CFI = "CFI";
        private const string F_ISIN = "ISIN";
        private const string F_SEDOL = "SEDOL";
        private const string F_CUSIP = "CUSIP";
        private const string F_ICB = "ICB";
        private const string F_SIC = "SIC";
        private const string F_MULTIPLIER = "MULTIPLIER";
        private const string F_PRODUCT = "PRODUCT";
        private const string F_UNDERLYING = "UNDERLYING";
        private const string F_SPC = "SPC";
        private const string F_ADDITIONAL_UNDERLYINGS = "ADDITIONAL_UNDERLYINGS";
        private const string F_MMY = "MMY";
        private const string F_EXPIRATION = "EXPIRATION";
        private const string F_LAST_TRADE = "LAST_TRADE";
        private const string F_STRIKE = "STRIKE";
        private const string F_OPTION_TYPE = "OPTION_TYPE";
        private const string F_EXPIRATION_STYLE = "EXPIRATION_STYLE";
        private const string F_SETTLEMENT_STYLE = "SETTLEMENT_STYLE";
        private const string F_PRICE_INCREMENTS = "PRICE_INCREMENTS";
        private const string F_TRADING_HOURS = "TRADING_HOURS";

        private static Dictionary<string, InstrumentProfileField> MAP = new Dictionary<string, InstrumentProfileField>();

        private InstrumentProfileField(string name) {
            this.Name = name;
            MAP.Add(name, this);
        }

        /// <summary>
        /// Get name of instrument profile field.
        /// </summary>
        public string Name {
            get;
            private set;
        }

        public static InstrumentProfileField[] Values {
            get {
                return (new List<InstrumentProfileField>(MAP.Values)).ToArray();
            }
        }

        /// <summary>
        /// Returns string representation of field.
        /// </summary>
        /// <returns>String representation of field.</returns>
        public override string ToString() {
            return Name;
        }

        /// <summary>
        /// Returns field for specified name or <b>null</b> if field is not found.
        /// </summary>
        /// <param name="name">Name of field to find.</param>
        /// <returns>Field for specified name or <b>null</b> if field is not found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static InstrumentProfileField find(string name)  {
            InstrumentProfileField value;
            if (!MAP.TryGetValue(name, out value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns value of this field for specified profile in textual representation.
        /// </summary>
        /// <param name="ip">Profile fot which get field.</param>
        /// <returns>Value of this field for specified profile in textual representation.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string getField(InstrumentProfile ip) {
            switch (this.Name) {
                case F_TYPE: return ip.getType();
                case F_SYMBOL: return ip.getSymbol();
                case F_DESCRIPTION: return ip.getDescription();
                case F_LOCAL_SYMBOL: return ip.getLocalSymbol();
                case F_LOCAL_DESCRIPTION: return ip.getLocalDescription();
                case F_COUNTRY: return ip.getCountry();
                case F_OPOL: return ip.getOPOL();
                case F_EXCHANGE_DATA: return ip.getExchangeData();
                case F_EXCHANGES: return ip.getExchanges();
                case F_CURRENCY: return ip.getCurrency();
                case F_BASE_CURRENCY: return ip.getBaseCurrency();
                case F_CFI: return ip.getCFI();
                case F_ISIN: return ip.getISIN();
                case F_SEDOL: return ip.getSEDOL();
                case F_CUSIP: return ip.getCUSIP();
                case F_ICB: return formatNumber(ip.getICB());
                case F_SIC: return formatNumber(ip.getSIC());
                case F_MULTIPLIER: return formatNumber(ip.getMultiplier());
                case F_PRODUCT: return ip.getProduct();
                case F_UNDERLYING: return ip.getUnderlying();
                case F_SPC: return formatNumber(ip.getSPC());
                case F_ADDITIONAL_UNDERLYINGS: return ip.getAdditionalUnderlyings();
                case F_MMY: return ip.getMMY();
                case F_EXPIRATION: return formatDate(ip.getExpiration());
                case F_LAST_TRADE: return formatDate(ip.getLastTrade());
                case F_STRIKE: return formatNumber(ip.getStrike());
                case F_OPTION_TYPE: return ip.getOptionType();
                case F_EXPIRATION_STYLE: return ip.getExpirationStyle();
                case F_SETTLEMENT_STYLE: return ip.getSettlementStyle();
                case F_PRICE_INCREMENTS: return ip.getPriceIncrements();
                case F_TRADING_HOURS: return ip.getTradingHours();
            }
            throw new InvalidOperationException("cannot process field " + this);
        }

        /// <summary>
        /// Sets value of this field (in textual representation) to specified profile.
        /// </summary>
        /// <param name="ip">Profile to set field.</param>
        /// <param name="value">Value that set into field.</param>
        /// <exception cref="InvalidOperationException">if text uses wrong format or contains invalid values</exception>
        public void setField(InstrumentProfile ip, string value) {
            switch (this.Name) {
                case F_TYPE: ip.setType(value); return;
                case F_SYMBOL: ip.setSymbol(value); return;
                case F_DESCRIPTION: ip.setDescription(value); return;
                case F_LOCAL_SYMBOL: ip.setLocalSymbol(value); return;
                case F_LOCAL_DESCRIPTION: ip.setLocalDescription(value); return;
                case F_COUNTRY: ip.setCountry(value); return;
                case F_OPOL: ip.setOPOL(value); return;
                case F_EXCHANGE_DATA: ip.setExchangeData(value); return;
                case F_EXCHANGES: ip.setExchanges(value); return;
                case F_CURRENCY: ip.setCurrency(value); return;
                case F_BASE_CURRENCY: ip.setBaseCurrency(value); return;
                case F_CFI: ip.setCFI(value); return;
                case F_ISIN: ip.setISIN(value); return;
                case F_SEDOL: ip.setSEDOL(value); return;
                case F_CUSIP: ip.setCUSIP(value); return;
                case F_ICB: ip.setICB((int)parseNumber(value)); return;
                case F_SIC: ip.setSIC((int)parseNumber(value)); return;
                case F_MULTIPLIER: ip.setMultiplier(parseNumber(value)); return;
                case F_PRODUCT: ip.setProduct(value); return;
                case F_UNDERLYING: ip.setUnderlying(value); return;
                case F_SPC: ip.setSPC(parseNumber(value)); return;
                case F_ADDITIONAL_UNDERLYINGS: ip.setAdditionalUnderlyings(value); return;
                case F_MMY: ip.setMMY(value); return;
                case F_EXPIRATION: ip.setExpiration(parseDate(value)); return;
                case F_LAST_TRADE: ip.setLastTrade(parseDate(value)); return;
                case F_STRIKE: ip.setStrike(parseNumber(value)); return;
                case F_OPTION_TYPE: ip.setOptionType(value); return;
                case F_EXPIRATION_STYLE: ip.setExpirationStyle(value); return;
                case F_SETTLEMENT_STYLE: ip.setSettlementStyle(value); return;
                case F_PRICE_INCREMENTS: ip.setPriceIncrements(value); return;
                case F_TRADING_HOURS: ip.setTradingHours(value); return;
            }
            throw new InvalidOperationException("cannot process field " + this);
        }

        /// <summary>
        /// Returns type of this field.
        /// </summary>
        /// <returns>Type of this field.</returns>
        public Type getType() {
            switch (this.Name) {
                case F_ICB: return typeof(Double);
                case F_SIC: return typeof(Double);
                case F_MULTIPLIER: return typeof(Double);
                case F_SPC: return typeof(Double);
                    //TODO: check this Date
                case F_EXPIRATION: return typeof(DateTime);
                case F_LAST_TRADE: return typeof(DateTime);
                case F_STRIKE: return typeof(Double);
            }
            return typeof(string);
        }

        /// <summary>
        /// Returns "true" if this field supports numeric representation of a value.
        /// </summary>
        /// <returns>"true" if this field supports numeric representation of a value.</returns>
        public bool isNumericField() {
            switch (this.Name) {
                case F_ICB: return true;
                case F_SIC: return true;
                case F_MULTIPLIER: return true;
                case F_SPC: return true;
                case F_EXPIRATION: return true;
                case F_LAST_TRADE: return true;
                case F_STRIKE: return true;
            }
            return false;
        }

        /// <summary>
        /// Returns value of this field for specified profile in numeric representation.
        /// </summary>
        /// <param name="ip">Profile from get field.</param>
        /// <returns>Value of this field for specified profile in numeric representation.</returns>
        /// <exception cref="ArgumentException">If this field has no numeric representation.</exception>
        public double getNumericField(InstrumentProfile ip) {
            switch (this.Name) {
                case F_ICB: return ip.getICB();
                case F_SIC: return ip.getSIC();
                case F_MULTIPLIER: return ip.getMultiplier();
                case F_SPC: return ip.getSPC();
                case F_EXPIRATION: return ip.getExpiration();
                case F_LAST_TRADE: return ip.getLastTrade();
                case F_STRIKE: return ip.getStrike();
            }
            throw new ArgumentException("textual field " + this);
        }

        /// <summary>
        /// Sets value of this field (in numeric representation) to specified profile.
        /// </summary>
        /// <param name="ip">Profile tos set value.</param>
        /// <param name="value">Value ehich set to field.</param>
        /// <exception cref="ArgumentException">If this field has no numeric representation</exception>
        public void setNumericField(InstrumentProfile ip, double value) {
            switch (this.Name) {
                case F_ICB: ip.setICB((int)value); return;
                case F_SIC: ip.setSIC((int)value); return;
                case F_MULTIPLIER: ip.setMultiplier(value); return;
                case F_SPC: ip.setSPC(value); return;
                case F_EXPIRATION: ip.setExpiration((int)value); return;
                case F_LAST_TRADE: ip.setLastTrade((int)value); return;
                case F_STRIKE: ip.setStrike(value); return;
            }
            throw new ArgumentException("textual field " + this);
        }


        // ========== Internal Implementation ==========

        private static readonly ThreadLocal<NumberFormatInfo> NUMBER_FORMATTER = new ThreadLocal<NumberFormatInfo>();
        private static readonly ThreadLocal<DateFormat> DATE_FORMATTER = new ThreadLocal<DateFormat>();

        private static readonly string[] FORMATTED_NUMBERS = new string[20000]; // A "sparse" cache for small numbers
        private static readonly string[] FORMATTED_DATES = new string[30000]; // A "sparse" cache for common dates (1970-2052)
        private static readonly ConcurrentDictionary<string, Double> PARSED_NUMBERS = new ConcurrentDictionary<string, Double>();
        private static readonly ConcurrentDictionary<string, int> PARSED_DATES = new ConcurrentDictionary<string, int>();

        private static readonly long DAY = 24 * 3600 * 1000;

        public static string formatNumber(double d) {
            if (d == 0)
                return "";
            int n4 = (int)(d * 4) + 4000;
            if (n4 == d * 4 + 4000 && n4 >= 0 && n4 < FORMATTED_NUMBERS.Length) {
                string cached = FORMATTED_NUMBERS[n4];
                if (cached == null)
                    FORMATTED_NUMBERS[n4] = cached = formatNumberImpl(d);
                return cached;
            }
            return formatNumberImpl(d);
        }

        private static string formatNumberImpl(double d) {
            if (d == (double)(int)d)
                return Convert.ToString((int)d);
            if (d == (double)(long)d)
                return Convert.ToString((long)d);
            double ad = Math.Abs(d);
            if (ad > 1e-9 && ad < 1e12) {
                NumberFormatInfo nf = NUMBER_FORMATTER.Value;
                if (nf == null) {
                    nf = NumberFormatInfo.GetInstance(new CultureInfo("en-US"));
                    //nf.SetMaximumFractionDigits(20);
                    //nf.setGroupingUsed(false);
                    //TODO: check number format
                    nf.NumberDecimalDigits = 20;
                    NUMBER_FORMATTER.Value = nf;
                }
                return Convert.ToString(d, nf);
                //return nf.format(d);
            }
            //return Double.ToString(d);
            return d.ToString();
        }

        public static double parseNumber(string s) {
            if (s == null || String.IsNullOrEmpty(s))
                return 0;
            if (PARSED_NUMBERS.ContainsKey(s)) {
                return PARSED_NUMBERS[s];
            } else {
                if (PARSED_NUMBERS.Count > 10000)
                    PARSED_NUMBERS.Clear();
                double cached = Double.Parse(s);
                PARSED_NUMBERS[s] = cached;
                return cached;
            }
        }

        public static string formatDate(int d) {
            if (d == 0)
                return "";
            if (d >= 0 && d < FORMATTED_DATES.Length) {
                string cached = FORMATTED_DATES[d];
                if (cached == null)
                    FORMATTED_DATES[d] = cached = getDateFormat().Format(Tools.UnixTimeToDate(d * DAY));
                return cached;
            }
            return getDateFormat().Format(Tools.UnixTimeToDate(d * DAY));
        }

        public static int parseDate(string s) {
            if (s == null || String.IsNullOrEmpty(s))
                return 0;

            if (PARSED_DATES.ContainsKey(s)) {
                return PARSED_DATES[s];
            } else {
                try
                {
                    if (PARSED_DATES.Count > 10000)
                        PARSED_DATES.Clear();
                    int cached = (int)(Tools.DateToUnixTime(getDateFormat().Parse(s)) / DAY);
                    PARSED_DATES[s] = cached;
                    return cached;
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.Message);
                }
            }
        }

        private static DateFormat getDateFormat() {
            DateFormat df = DATE_FORMATTER.Value;
            if (df == null) {
                df = new DateFormat("yyyy-MM-dd");
                //TODO: make timezone
                //df.SetTimeZone(TimeZone.getTimeZone("GMT"));
                DATE_FORMATTER.Value = df;
            }
            return df;
        }
    }
}
