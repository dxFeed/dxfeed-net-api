using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.dxfeed.ipf.option {
	class OptionChain {
		/// <summary>
		/// Symbol(product or underlying) of this option chain.
		/// </summary>
		/// <value>Gets symbol (product or underlying) of this option chain.</value>
		public string Symbol {
			get;
			private set;
		}

		OptionChain(string symbol) {
			Symbol = symbol;
		}

	}
}
