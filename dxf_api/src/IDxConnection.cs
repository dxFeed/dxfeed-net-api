using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api {

	/// <summary>
	/// Interface provides operations with event subscription
	/// </summary>
	public interface IDxConnection : IDisposable {

		/// <summary>
		/// Disconnect from the server
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Create event subscription
		/// </summary>
		/// <param name="type">event type</param>
		/// <param name="listener">event listener callback</param>
		/// <returns> subscription object</returns>
		/// <exception cref="ArgumentNullException">If listener is null.</exception>
		/// <exception cref="DxException"></exception>
		IDxSubscription CreateSubscription(EventType type, IDxFeedListener listener);

		/// <summary>
		/// Create candle event subscription
		/// </summary>
		/// <param name="time">date time in the past</param>
		/// <param name="listener">candle listener callback</param>
		/// <returns></returns>
		IDxSubscription CreateSubscription(DateTime? time, IDxCandleListener listener);

		/// <summary>
		/// Creates snapshot subscription
		/// </summary>
		/// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
		/// <param name="listener"></param>
		/// <returns></returns>
		IDxSubscription CreateSnapshotSubscription(Int64 time, IDxSnapshotListener listener);

		/// <summary>
		/// Creates snapshot subscription
		/// </summary>
		/// <param name="time">Date time in the past</param>
		/// <param name="listener"></param>
		/// <returns></returns>
		IDxSubscription CreateSnapshotSubscription(DateTime? time, IDxSnapshotListener listener);
	}
}