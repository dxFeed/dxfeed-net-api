using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.dxfeed.ipf.option {
	class OptionChain {
		/// <summary>
		///Returns symbol(product or underlying) of this option chain.
		/// </summary>
		/// <returns>Symbol (product or underlying) of this option chain.</returns>
		public string Symbol {
			get; private set;
		}

		OptionChain(string symbol) {
			Symbol = symbol;
		}

	}
}
