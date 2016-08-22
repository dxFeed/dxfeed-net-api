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
using System.Threading;
using com.dxfeed.api;

namespace com.dxfeed.ipf {

    /// <summary>
    /// Defines standard fields of {@link InstrumentProfile} and provides data access methods.
    /// Please see <b>Instrument Profile Format</b> documentation for complete description.
    /// </summary>
    class InstrumentProfileField {

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

        private static Dictionary<string, InstrumentProfileField> fieldsMap = new Dictionary<string, InstrumentProfileField>();

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

        private InstrumentProfileField(string name) {
            Name = name;
            fieldsMap.Add(name, this);
        }

        /// <summary>
        /// Get name of instrument profile field.
        /// </summary>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Get all default instrument profile fields.
        /// </summary>
        public static InstrumentProfileField[] Values {
            get {
                return (new List<InstrumentProfileField>(fieldsMap.Values)).ToArray();
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
        /// <exception cref="System.ArgumentNullException">If name is null.</exception>
        public static InstrumentProfileField Find(string name)  {
            InstrumentProfileField value;
            if (!fieldsMap.TryGetValue(name, out value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns value of this field for specified profile in textual representation.
        /// </summary>
        /// <param name="ip">Profile fot which get field.</param>
        /// <returns>Value of this field for specified profile in textual representation.</returns>
        /// <exception cref="System.InvalidOperationException">Can't format certain field.</exception>
        public string GetField(InstrumentProfile ip) {
            try {
                switch (Name) {
                    case F_TYPE: return ip.GetTypeName();
                    case F_SYMBOL: return ip.GetSymbol();
                    case F_DESCRIPTION: return ip.GetDescription();
                    case F_LOCAL_SYMBOL: return ip.GetLocalSymbol();
                    case F_LOCAL_DESCRIPTION: return ip.GetLocalDescription();
                    case F_COUNTRY: return ip.GetCountry();
                    case F_OPOL: return ip.GetOPOL();
                    case F_EXCHANGE_DATA: return ip.GetExchangeData();
                    case F_EXCHANGES: return ip.GetExchanges();
                    case F_CURRENCY: return ip.GetCurrency();
                    case F_BASE_CURRENCY: return ip.GetBaseCurrency();
                    case F_CFI: return ip.GetCfi();
                    case F_ISIN: return ip.GetISIN();
                    case F_SEDOL: return ip.GetSEDOL();
                    case F_CUSIP: return ip.GetCUSIP();
                    case F_ICB: return FormatNumber(ip.GetICB());
                    case F_SIC: return FormatNumber(ip.GetSIC());
                    case F_MULTIPLIER: return FormatNumber(ip.GetMultiplier());
                    case F_PRODUCT: return ip.GetProduct();
                    case F_UNDERLYING: return ip.GetUnderlying();
                    case F_SPC: return FormatNumber(ip.GetSPC());
                    case F_ADDITIONAL_UNDERLYINGS: return ip.GetAdditionalUnderlyings();
                    case F_MMY: return ip.GetMmy();
                    case F_EXPIRATION: return FormatDate(ip.GetExpiration());
                    case F_LAST_TRADE: return FormatDate(ip.GetLastTrade());
                    case F_STRIKE: return FormatNumber(ip.GetStrike());
                    case F_OPTION_TYPE: return ip.GetOptionType();
                    case F_EXPIRATION_STYLE: return ip.GetExpirationStyle();
                    case F_SETTLEMENT_STYLE: return ip.GetSettlementStyle();
                    case F_PRICE_INCREMENTS: return ip.GetPriceIncrements();
                    case F_TRADING_HOURS: return ip.GetTradingHours();
                    default: throw new InvalidOperationException("cannot process field " + this);
                }
            } catch (Exception exc) {
                throw new InvalidOperationException(String.Format("Can't get field '{0}': {1}", this, exc));
            }
        }

        /// <summary>
        /// Sets value of this field (in textual representation) to specified profile.
        /// </summary>
        /// <param name="ip">Profile to set field.</param>
        /// <param name="value">Value that set into field.</param>
        /// <exception cref="System.InvalidOperationException">If text uses wrong format or contains invalid values.</exception>
        public void SetField(InstrumentProfile ip, string value) {
            try {
                switch (Name) {
                    case F_TYPE: ip.SetType(value); return;
                    case F_SYMBOL: ip.SetSymbol(value); return;
                    case F_DESCRIPTION: ip.SetDescription(value); return;
                    case F_LOCAL_SYMBOL: ip.SetLocalSymbol(value); return;
                    case F_LOCAL_DESCRIPTION: ip.SetLocalDescription(value); return;
                    case F_COUNTRY: ip.SetCountry(value); return;
                    case F_OPOL: ip.SetOPOL(value); return;
                    case F_EXCHANGE_DATA: ip.SetExchangeData(value); return;
                    case F_EXCHANGES: ip.SetExchanges(value); return;
                    case F_CURRENCY: ip.SetCurrency(value); return;
                    case F_BASE_CURRENCY: ip.SetBaseCurrency(value); return;
                    case F_CFI: ip.SetCFI(value); return;
                    case F_ISIN: ip.SetISIN(value); return;
                    case F_SEDOL: ip.SetSEDOL(value); return;
                    case F_CUSIP: ip.SetCUSIP(value); return;
                    case F_ICB: ip.SetICB((int)ParseNumber(value)); return;
                    case F_SIC: ip.SetSIC((int)ParseNumber(value)); return;
                    case F_MULTIPLIER: ip.SetMultiplier(ParseNumber(value)); return;
                    case F_PRODUCT: ip.SetProduct(value); return;
                    case F_UNDERLYING: ip.SetUnderlying(value); return;
                    case F_SPC: ip.SetSPC(ParseNumber(value)); return;
                    case F_ADDITIONAL_UNDERLYINGS: ip.SetAdditionalUnderlyings(value); return;
                    case F_MMY: ip.SetMMY(value); return;
                    case F_EXPIRATION: ip.SetExpiration(ParseDate(value)); return;
                    case F_LAST_TRADE: ip.SetLastTrade(ParseDate(value)); return;
                    case F_STRIKE: ip.SetStrike(ParseNumber(value)); return;
                    case F_OPTION_TYPE: ip.SetOptionType(value); return;
                    case F_EXPIRATION_STYLE: ip.SetExpirationStyle(value); return;
                    case F_SETTLEMENT_STYLE: ip.SetSettlementStyle(value); return;
                    case F_PRICE_INCREMENTS: ip.SetPriceIncrements(value); return;
                    case F_TRADING_HOURS: ip.SetTradingHours(value); return;
                    default: throw new InvalidOperationException("cannot process field " + this);
                }
            } catch (Exception exc) {
                throw new InvalidOperationException(String.Format("Can't set field '{0}': {1}", this, exc));
            }
        }

        /// <summary>
        /// Returns type of this field.
        /// </summary>
        /// <returns>Type of this field.</returns>
        public Type GetFieldType() {
            switch (Name) {
                case F_ICB: return typeof(double);
                case F_SIC: return typeof(double);
                case F_MULTIPLIER: return typeof(double);
                case F_SPC: return typeof(double);
                case F_EXPIRATION: return typeof(DateTime);
                case F_LAST_TRADE: return typeof(DateTime);
                case F_STRIKE: return typeof(double);
            }
            return typeof(string);
        }

        /// <summary>
        /// Returns "true" if this field supports numeric representation of a value.
        /// </summary>
        /// <returns>"true" if this field supports numeric representation of a value.</returns>
        public bool IsNumericField() {
            switch (Name) {
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
        /// <exception cref="System.ArgumentException">If this field has no numeric representation.</exception>
        public double GetNumericField(InstrumentProfile ip) {
            switch (Name) {
                case F_ICB: return ip.GetICB();
                case F_SIC: return ip.GetSIC();
                case F_MULTIPLIER: return ip.GetMultiplier();
                case F_SPC: return ip.GetSPC();
                case F_EXPIRATION: return ip.GetExpiration();
                case F_LAST_TRADE: return ip.GetLastTrade();
                case F_STRIKE: return ip.GetStrike();
            }
            throw new ArgumentException("textual field " + this);
        }

        /// <summary>
        /// Sets value of this field (in numeric representation) to specified profile.
        /// </summary>
        /// <param name="ip">Profile tos set value.</param>
        /// <param name="value">Value ehich set to field.</param>
        /// <exception cref="System.ArgumentException">If this field has no numeric representation</exception>
        public void SetNumericField(InstrumentProfile ip, double value) {
            switch (Name) {
                case F_ICB: ip.SetICB((int)value); return;
                case F_SIC: ip.SetSIC((int)value); return;
                case F_MULTIPLIER: ip.SetMultiplier(value); return;
                case F_SPC: ip.SetSPC(value); return;
                case F_EXPIRATION: ip.SetExpiration((int)value); return;
                case F_LAST_TRADE: ip.SetLastTrade((int)value); return;
                case F_STRIKE: ip.SetStrike(value); return;
            }
            throw new ArgumentException("textual field " + this);
        }


        // ========== Internal Implementation ==========

        private static readonly ThreadLocal<NumberFormatInfo> numberFormatter = new ThreadLocal<NumberFormatInfo>();
        private static readonly ThreadLocal<DateFormat> dateFormatter = new ThreadLocal<DateFormat>();

        private static readonly string[] formattedNumbers = new string[20000]; // A "sparse" cache for small numbers
        private static readonly string[] formattedDates = new string[30000]; // A "sparse" cache for common dates (1970-2052)
        private static readonly ConcurrentDictionary<string, double> parsedNumbers = new ConcurrentDictionary<string, double>();
        private static readonly ConcurrentDictionary<string, int> parsedDates = new ConcurrentDictionary<string, int>();

        private static readonly long DAY = 24 * 3600 * 1000;

        /// <summary>
        /// Format number
        /// </summary>
        /// <param name="d">The number to format.</param>
        /// <returns>Strig representation of number in specified format.</returns>
        /// <exception cref="System.InvalidOperationException">Can't format number.</exception>
        public static string FormatNumber(double d) {
            if (d == 0)
                return "";
            int n4 = (int)(d * 4) + 4000;
            if (n4 == d * 4 + 4000 && n4 >= 0 && n4 < formattedNumbers.Length) {
                string cached = formattedNumbers[n4];
                if (cached == null)
                    formattedNumbers[n4] = cached = FormatNumberImpl(d);
                return cached;
            }
            return FormatNumberImpl(d);
        }

        /// <summary>
        /// Format number implementztion.
        /// </summary>
        /// <param name="d">Number to format.</param>
        /// <returns>String representation of number.</returns>
        /// <exception cref="System.InvalidOperationException">If number formatter error occurs.</exception>
        private static string FormatNumberImpl(double d) {
            if (d == (double)(int)d)
                return Convert.ToString((int)d);
            if (d == (double)(long)d)
                return Convert.ToString((long)d);
            double ad = Math.Abs(d);
            if (ad > 1e-9 && ad < 1e12) {
                NumberFormatInfo nf;
                try {
                    nf = numberFormatter.Value;
                    if (nf == null) {
                        nf = NumberFormatInfo.GetInstance(new CultureInfo("en-US"));
                        nf.NumberDecimalDigits = 20;
                        numberFormatter.Value = nf;
                    }
                } catch (InvalidOperationException) {
                    throw;
                } catch (Exception exc) {
                    throw new InvalidOperationException("FormatNumber failed: " + exc);
                }
                return Convert.ToString(d, nf);
            }
            return d.ToString();
        }

        /// <summary>
        /// Parse number from string.
        /// </summary>
        /// <param name="s">String representation of number.</param>
        /// <returns>Double number.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        public static double ParseNumber(string s) {
            if (s == null || string.IsNullOrEmpty(s))
                return 0;
            try {
                if (parsedNumbers.ContainsKey(s)) {
                    return parsedNumbers[s];
                } else {
                    if (parsedNumbers.Count > 10000)
                        parsedNumbers.Clear();
                    double cached = double.Parse(s, CultureInfo.GetCultureInfo("en-US"));
                    parsedNumbers[s] = cached;
                    return cached;
                }
            } catch (FormatException) {
                throw;
            } catch (ArgumentNullException) {
                throw;
            } catch (Exception exc) {
                throw new FormatException("ParseNumber failed: " + exc);
            }
        }

        /// <summary>
        /// Convert unix time to specified date fromat and returns its string representation.
        /// </summary>
        /// <param name="d">Date time in unix time.</param>
        /// <returns>String representation of date tinme.</returns>
        /// <exception cref="System.FormatException"></exception>
        /// <exception cref="System.InvalidOperationException">If number formatter error occurs.</exception>
        public static string FormatDate(int d) {
            if (d == 0)
                return "";
            if (d >= 0 && d < formattedDates.Length) {
                string cached = formattedDates[d];
                if (cached == null)
                    formattedDates[d] = cached = GetDateFormat().Format(Tools.UnixTimeToDate(d * DAY));
                return cached;
            }
            return GetDateFormat().Format(Tools.UnixTimeToDate(d * DAY));
        }

        /// <summary>
        /// Parse date from string.
        /// </summary>
        /// <param name="s">String representation of date.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException">If number formatter error occurs.</exception>
        /// <exception cref="System.FormatException"></exception>
        public static int ParseDate(string s) {
            if (s == null || string.IsNullOrEmpty(s))
                return 0;

            if (parsedDates.ContainsKey(s)) {
                return parsedDates[s];
            } else {
                if (parsedDates.Count > 10000)
                    parsedDates.Clear();
                int cached = (int)(Tools.DateToUnixTime(GetDateFormat().Parse(s)) / DAY);
                parsedDates[s] = cached;
                return cached;
            }
        }

        /// <summary>
        /// Returns date formatter instance.
        /// </summary>
        /// <returns>Date formatter instance.</returns>
        /// <exception cref="System.InvalidOperationException">If number formatter error occurs.</exception>
        private static DateFormat GetDateFormat() {
            try {
                DateFormat df = dateFormatter.Value;
                if (df == null) {
                    df = new DateFormat("yyyy-MM-dd");
                    dateFormatter.Value = df;
                }
                return df;
            } catch (Exception exc) {
                throw new InvalidOperationException("GetDateFormat failed: " + exc);
            }
        }
    }
}
