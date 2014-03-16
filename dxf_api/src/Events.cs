using System;
using System.Collections.Generic;

namespace com.dxfeed.api {

	[Flags]
	public enum EventType : int {
		None = 0,
		Trade = 1,
		Quote = 2,
		Summary = 4,
		Profile = 8,
		Order = 16,
		TimeAndSale = 32
	}

	public interface IDxEventBuf<out T> : IEnumerable<T>, IDisposable {
		EventType EventType { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TS"></typeparam>
	/// <typeparam name="T"> declared as generic to prevent boxing</typeparam>
	/// <typeparam name="TE"></typeparam>
	/// <param name="src"></param>
	/// <param name="buf"></param>
	public delegate void DxEventListener<in TS, in T, TE>(TS src, T buf) where T : IDxEventBuf<TE>;
}