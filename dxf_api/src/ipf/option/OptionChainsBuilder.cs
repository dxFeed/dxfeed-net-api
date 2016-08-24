using System.Collections.Generic;

namespace com.dxfeed.ipf.option {
    /// <summary>
    /// Builder class for a set of option chains grouped by product or underlying symbol.
    /// <h3>Threads and clocks</h3>
    /// This class is <b>NOT</b> thread-safe and cannot be used from multiple threads without external synchronization.
    /// @param <T> The type of option instrument instances.
    /// </summary>
    public class OptionChainsBuilder<T> {

        /// <summary>
        /// Builds options chains for all options from the given collections of {@link InstrumentProfile instrument profiles}.
        /// </summary>
        /// <param name="instruments">collection of instrument profiles.</param>
        /// <returns>builder with all the options from instruments collection.</returns>
        public static OptionChainsBuilder<InstrumentProfile> Build(ICollection<InstrumentProfile> instruments) {
            OptionChainsBuilder<InstrumentProfile> ocb = new OptionChainsBuilder<InstrumentProfile>();
            foreach(var ip in instruments) {
                if (!"OPTION".Equals(ip.GetTypeName())) {
                    continue;
                }
                ocb.Product = ip.GetProduct();
                ocb.Underlying = ip.GetUnderlying();
                ocb.SetExpiration(ip.GetExpiration());
                ocb.SetLastTrade(ip.GetLastTrade());
                ocb.SetMultiplier(ip.GetMultiplier());
                ocb.SetSPC(ip.GetSPC());
                ocb.SetAdditionalUnderlyings(ip.GetAdditionalUnderlyings());
                ocb.SetMmy((ip.GetMmy()));
                ocb.SetOptionType(ip.GetOptionType());
                ocb.SetExpirationStyle(ip.GetExpirationStyle());
                ocb.SetSettlementStyle(ip.GetSettlementStyle());
                ocb.Cfi = ip.GetCfi();
                ocb.Strike = ip.GetStrike();
                ocb.AddOption(ip);
            }
            return ocb;
        }

        private readonly Dictionary<string, OptionChain<T>> chains = new Dictionary<string, OptionChain<T>>();
        OptionSeries<T> series = new OptionSeries<T>();

        /// <summary>
        /// Creates new option chains builder.
        /// </summary>
        public OptionChainsBuilder() {
            Product = "";
            Underlying = "";
            Cfi = "";
        }

        /// <summary>
        /// Product for futures and options on futures (underlying asset name).
        /// Example: "/YG".
        /// </summary>
        public string Product {
            internal get { return Product; }
            set { Product = value == null || value.Length == 0 ? "" : value; }
        }

        /// <summary>
        /// Primary underlying symbol for options.
        /// Example: "C", "/YGM9"
        /// </summary>
        public string Underlying {
            internal get { return Underlying; }
            set { Underlying = value == null || value.Length == 0 ? "" : value; }
        }

        /// <summary>
        /// Classification of Financial Instruments code (CFI code).
        /// It is a mandatory field as it is the only way to distinguish Call/Put option type,
        /// American/European exercise, Cash/Physical delivery.
        /// It shall use six-letter CFI code from ISO 10962 standard.
        /// It is allowed to use 'X' extensively and to omit trailing letters (assumed to be 'X').
        /// See<a href="http://en.wikipedia.org/wiki/ISO_10962"> ISO 10962 on Wikipedia</a>.
        /// Example: "OC" for generic call, "OP" for generic put.
        /// </summary>
        public string Cfi {
            internal get { return Cfi; }
            set {
                Cfi = value == null || value.Length == 0 ? "" : value;
                series.Cfi = Cfi.Length < 2 ? Cfi : Cfi[0] + "X" + Cfi.Substring(2);
            }
        }

        /// <summary>
        /// Strike price for options.
        /// Example: 80, 22.5.
        /// </summary>
        public double Strike {
            internal get;
            set;
        }

        /// <summary>
        /// Returns a view of chains created by this builder.
        /// It updates as new options are added with {@link #addOption(Object) addOption} method.
        /// @return view of chains created by this builder.
        /// </summary>
        public Dictionary<string, OptionChain<T>> Chains {
            get {
                return chains;
            }
        }

        /// <summary>
        /// Changes day id of expiration.
        /// Example: {@link DayUtil#getDayIdByYearMonthDay DayUtil.getDayIdByYearMonthDay}(20090117).
        /// </summary>
        /// <param name="expiration">day id of expiration</param>
        public void SetExpiration(int expiration) {
            series.Expiration = expiration;
        }

        /// <summary>
        /// Changes day id of last trading day.
        /// Example: {@link DayUtil#getDayIdByYearMonthDay DayUtil.getDayIdByYearMonthDay}(20090116).
        /// </summary>
        /// <param name="lastTrade">day id of last trading day.</param>
        public void SetLastTrade(int lastTrade) {
            series.LastTrade = lastTrade;
        }

        /// <summary>
        /// Changes market value multiplier.
        /// Example: 100, 33.2.
        /// </summary>
        /// <param name="multiplier">market value multiplier.</param>
        public void SetMultiplier(double multiplier) {
            series.Multiplier = multiplier;
        }

        /// <summary>
        /// Changes shares per contract for options.
        /// Example: 1, 100.
        /// </summary>
        /// <param name="spc">shares per contract for options.</param>
        public void SetSPC(double spc) {
            series.Spc = spc;
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
        /// <param name="additionalUnderlyings">additional underlyings for options, including additional cash.</param>
        public void SetAdditionalUnderlyings(string additionalUnderlyings) {
            series.AdditionalUnderlyings = additionalUnderlyings == null || additionalUnderlyings.Length == 0 ? "" : additionalUnderlyings;
        }

        /// <summary>
        ///  Changes maturity month-year as provided for corresponding FIX tag (200).
        /// It can use several different formats depending on data source:
        /// <ul>
        /// <li>YYYYMM – if only year and month are specified
        /// <li>YYYYMMDD – if full date is specified
        /// <li>YYYYMMwN – if week number(within a month) is specified
        /// </ul>
        /// </summary>
        /// <param name="mmy">maturity month-year as provided for corresponding FIX tag (200).</param>
        public void SetMmy(string mmy) {
            series.Mmy = mmy == null || mmy.Length == 0 ? "" : mmy;
        }


        /// <summary>
        /// Changes type of option.
        /// It shall use one of following values:
        /// <ul>
        /// <li>STAN = Standard Options
        /// <li>LEAP = Long-term Equity AnticiPation Securities
        /// <li>SDO = Special Dated Options
        /// <li>BINY = Binary Options
        /// <li>FLEX = FLexible EXchange Options
        /// <li>VSO = Variable Start Options
        /// <li>RNGE = Range
        /// </ul>
        /// </summary>
        /// <param name="optionType">type of option.</param>
        public void SetOptionType(string optionType) {
            series.OptionType = optionType == null || optionType.Length == 0 ? "" : optionType;
        }

        /// <summary>
        /// Changes expiration cycle style, such as "Weeklys", "Quarterlys".
        /// </summary>
        /// <param name="expirationStyle">expiration cycle style.</param>
        public void SetExpirationStyle(string expirationStyle) {
            series.ExpirationStyle = expirationStyle == null || expirationStyle.Length == 0 ? "" : expirationStyle;
        }

        /// <summary>
        /// Changes settlement price determination style, such as "Open", "Close".
        /// </summary>
        /// <param name="settlementStyle">settlement price determination style.</param>
        public void SetSettlementStyle(string settlementStyle) {
            series.SettlementStyle = settlementStyle == null || settlementStyle.Length == 0 ? "" : settlementStyle;
        }

        /// <summary>
        /// Adds an option instrument to this builder.
        /// Option is added to chains for the currently set {@link #setProduct(String) product} and/or
        /// {@link #setUnderlying(String) underlying} to the {@link OptionSeries series} that corresponding
        /// to all other currently set attributes.This method is safe in the sense that is ignores
        /// illegal state of the builder.It only adds an option when all of the following conditions are met:
        /// <ul>
        ///  <li>{@link #setCFI(String) CFI code} is set and starts with either "OC" for call or "OP" for put.
        ///  <li>{@link #setExpiration(int) expiration} is set and is not zero;
        ///  <li>{@link #setStrike(double) strike} is set and is not {@link Double#NaN NaN} nor {@link Double#isInfinite() infinite};
        ///  <li>{@link #setProduct(String) product} or {@link #setUnderlying(String) underlying symbol} are set;
        /// </ul>
        /// All the attributes remain set as before after the call to this method, but
        /// {@link #getChains() chains} are updated correspondingly.
        /// </summary>
        /// <param name="option">option to add.</param>
        public void AddOption(T option) {
            bool isCall = Cfi.StartsWith("OC");
            if (!isCall && !Cfi.StartsWith("OP"))
                return;
            if (series.Expiration == 0)
                return;
            if (double.IsNaN(Strike) || double.IsInfinity(Strike))
                return;
            if (Product.Length > 0)
                GetOrCreateChain(Product).AddOption(series, isCall, Strike, option);
            if (Underlying.Length > 0)
                GetOrCreateChain(Underlying).AddOption(series, isCall, Strike, option);
        }

        private OptionChain<T> GetOrCreateChain(string symbol) {
            OptionChain<T> chain;
            if (!Chains.TryGetValue(symbol, out chain)) {
                chain = new OptionChain<T>(symbol);
                Chains[symbol] = chain;
            }
            return chain;
        }

    }
}
