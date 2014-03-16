using System;
using System.Collections.Concurrent;

namespace com.dxfeed.native.data {
	public class StringCache {
		public readonly StringCache PASS_THROUGH = new StringCache(null);

		private readonly ConcurrentDictionary<IntPtr, String> cache;

		public StringCache() {
			cache = new ConcurrentDictionary<IntPtr, string>();
		}

		private StringCache(ConcurrentDictionary<IntPtr, String> cache) {
			this.cache = cache;
		}

		public string this[IntPtr ptr] {
			get {
				if (cache == null)
					return DxMarshal.ReadString(ptr);

				string str;
				if (cache.TryGetValue(ptr, out str))
					return str;

				str = DxMarshal.ReadString(ptr);

				cache.TryAdd(ptr, str);
				return str;
			}
		}

		public void Clear() {
			if (cache != null)
				cache.Clear();
		}
	}
}