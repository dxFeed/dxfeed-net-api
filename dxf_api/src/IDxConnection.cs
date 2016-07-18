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
		/// Create event subscription.
		/// </summary>
		/// <param name="type">event type.</param>
		/// <param name="listener">event listener callback.</param>
		/// <returns>Event subscription.</returns>
		IDxSubscription CreateSubscription(EventType type, IDxFeedListener listener);

		/// <summary>
		/// Create candle event subscription.
		/// </summary>
		/// <param name="time">Date time in the past.</param>
		/// <param name="listener">Candle listener callback.</param>
		/// <returns>Candle event subscription.</returns>
		IDxSubscription CreateSubscription(DateTime? time, IDxCandleListener listener);

		/// <summary>
		/// Creates snapshot subscription.
		/// </summary>
		/// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time).</param>
		/// <param name="listener">The snapshot listener callback.</param>
		/// <returns>Snapshot subscription.</returns>
		IDxSubscription CreateSnapshotSubscription(Int64 time, IDxSnapshotListener listener);

		/// <summary>
		/// Creates snapshot subscription.
		/// </summary>
		/// <param name="time">Date time in the past.</param>
		/// <param name="listener">The snapshot listener callback.</param>
		/// <returns>Snapshot subscription.</returns>
		IDxSubscription CreateSnapshotSubscription(DateTime? time, IDxSnapshotListener listener);
	}
}