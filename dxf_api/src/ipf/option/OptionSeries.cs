using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.dxfeed.ipf.option {
	class OptionSeries {

		public int Expiration {
			/// <summary>
			///Returns day id of expiration.
			///Example: {@link DayUtil#getDayIdByYearMonthDay DayUtil.getDayIdByYearMonthDay}(20090117).
			/// </summary>
			///<returns>Day id of expiration.</returns>
			get;
		}

	}
}
