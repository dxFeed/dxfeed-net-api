using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.dxfeed.ipf.option {
	class OptionSeries<T> {

		/// <summary>
		/// Day id of expiration.
		/// Example: {@link DayUtil#getDayIdByYearMonthDay DayUtil.getDayIdByYearMonthDay}(20090117).
		/// </summary>
		/// <value>Gets day id of expiration.</value>
		public int Expiration {
			get;
		}

		/// <summary>
		/// Day id of last trading day.
		/// Example: {@link DayUtil#getDayIdByYearMonthDay DayUtil.getDayIdByYearMonthDay}(20090116).
		/// </summary>
		/// <value>Gets day id of last trading day.</value>
		public int LastTrade {
			get;
		}

		/// <summary>
		/// Market value multiplier.
		/// Example: 100, 33.2.
		/// </summary>
		/// <value>Gets market value multiplier.</value>
		public double Multiplier {
			get;
		}

		/// <summary>
		/// Shares per contract for options.
		/// Example: 1, 100.
		/// </summary>
		/// <value>Gets shares per contract for options.</value>
		public double Spc {
			get;
		}

		/// <summary>
		/// Additional underlyings for options, including additional cash.
		/// It shall use following format:
		/// <pre>
		///     &lt;VALUE> ::= &lt;empty> | &lt;LIST>
		///     &lt;LIST> ::= &lt;AU> | &lt;AU> &lt;semicolon> &lt;space> &lt;LIST>
		///     &lt;AU> ::= &lt;UNDERLYING> &lt;space> &lt;SPC> </pre>
		/// the list shall be sorted by &lt;UNDERLYING>.
		/// Example: "SE 50", "FIS 53; US$ 45.46".
		/// </summary>
		/// <value>Gets additional underlyings for options, including additional cash.</value>
		public string AdditionalUnderlyings {
			get;
		}

		/// <summary>
		/// Maturity month-year as provided for corresponding FIX tag (200).
		/// It can use several different formats depending on data source:
		/// <ul>
		/// <li>YYYYMM – if only year and month are specified
		/// <li>YYYYMMDD – if full date is specified
		/// <li>YYYYMMwN – if week number(within a month) is specified
		/// </ul>
		/// </summary>
		/// <value>Gets maturity month-year as provided for corresponding FIX tag(200).</value>
		public string Mmy {
			get;
		}

		/// <summary>
		/// Type of option.
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
		/// <value>Gets type of option.</value>
		public string OptionType {
			get;
		}

		/// <summary>
		/// Expiration cycle style, such as "Weeklys", "Quarterlys".
		/// </summary>
		/// <value>gets expiration cycle style.</value>
		public string ExpirationStyle {
			get;
		}

		/// <summary>
		/// Settlement price determination style, such as "Open", "Close".
		/// </summary>
		/// <value>Gets settlement price determination style.</value>
		string SettlementStyle;

		/// <summary>
		/// Classification of Financial Instruments code of this option series.
		/// It shall use six-letter CFI code from ISO 10962 standard.
		/// It is allowed to use 'X' extensively and to omit trailing letters(assumed to be 'X').
		/// See<a href="http://en.wikipedia.org/wiki/ISO_10962"> ISO 10962 on Wikipedia</a>.
		/// It starts with "OX" as both {@link #getCalls() calls} and {@link #getPuts()} puts} are stored in a series.
		/// </summary>
		/// <value>Gets CFI code.</value>
		string cfi {
			get;
		}
	}
}
