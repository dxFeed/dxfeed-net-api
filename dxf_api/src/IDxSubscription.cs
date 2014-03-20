using System;
using System.Collections.Generic;

namespace com.dxfeed.api {
	public interface IDxSubscription : IDisposable {
		void AddSymbols(params string[] symbols);
		void RemoveSymbols(params string[] symbols);
		void SetSymbols(params string[] symbols);
		void Clear();
		IList<string> GetSymbols();
	}
}