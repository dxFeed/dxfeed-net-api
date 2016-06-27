using System;
using System.Collections.Generic;
using System.Text;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.native.api;
using com.dxfeed.native.events;

namespace com.dxfeed.native
{
	public class NativeSnapshotSubscription : IDxSubscription
	{
		private readonly IntPtr connectionPtr;
		private IntPtr snapshotPtr = NativeSnapshotSubscription.InvalidSnapshot;
		private readonly IDxSnapshotListener listener;
		//to prevent callback from being garbage collected
		private C.dxf_snapshot_listener_t callback;
		private readonly int eventId;
		private int time = 0;
		private string symbol = string.Empty;
		private string source = string.Empty;

		public static IntPtr InvalidSnapshot = IntPtr.Zero;

		public NativeSnapshotSubscription(NativeConnection connection, EventType eventType, int time, IDxSnapshotListener listener)
		{
			//TODO: add candle
			if (eventType != EventType.Order) {
				throw new ArgumentException("Invalid parameter", "eventType");
			}
			if (listener == null)
				throw new ArgumentNullException("listener");

			this.connectionPtr = connection.Handler;
			this.listener = listener;
			this.time = time;

			int eventId = 0;
			int mask = (int)eventType;
			while ((mask >>= 1) > 0)
				++eventId;
			this.eventId = eventId;
		}

		private void OnEvent(DxSnapshotData snapshotData, IntPtr userData)
		{
			switch (snapshotData.event_type)
			{
					//TODO: add candle
				case EventType.Order:
					var orderBuf = NativeBufferFactory.CreateOrderBuf(snapshotData.symbol, snapshotData.records, snapshotData.records_count, null);
					listener.OnOrderSnapshot<NativeEventBuffer<NativeOrder>, NativeOrder>(orderBuf);
					break;
			}
		}

		private void CreateSnapshot()
		{
			if (symbol == string.Empty)
				return;

			byte[] sourceBytes = null;
			if (source != string.Empty)
			{
				Encoding ascii = Encoding.ASCII;
				sourceBytes = ascii.GetBytes(source);
			}
			C.CheckOk(C.Instance.dxf_create_snapshot(connectionPtr, eventId, symbol, sourceBytes, time, out snapshotPtr));
			try
			{
				C.CheckOk(C.Instance.dxf_attach_snapshot_listener(snapshotPtr, callback = OnEvent, IntPtr.Zero));
			}
			catch (DxException)
			{
				Dispose();
				throw;
			}
		}

		#region Implementation of IDisposable

		public void Dispose() {
			if (snapshotPtr == InvalidSnapshot) return;

			C.CheckOk(C.Instance.dxf_close_snapshot(snapshotPtr));
			snapshotPtr = InvalidSnapshot;
		}

		#endregion

		#region Implementation of IDxSubscription

		public void AddSymbols(params string[] symbols) {
			SetSymbols(symbols);
		}

		public void RemoveSymbols(params string[] symbols) {
			Dispose();
		}

		public void SetSymbols(params string[] symbols) {
			if (symbols.Length != 1)
				throw new ArgumentException("Error: it is allowed to use only one symbol.");

			this.symbol = symbols[0];
			if (this.symbol.Length == 0)
				throw new ArgumentException("Error: invalid parameter");

			if (snapshotPtr != InvalidSnapshot)
				Dispose();

			CreateSnapshot();
		}

		public void Clear() {
			Dispose();
		}

		public IList<string> GetSymbols() {
			List<string> list =new List<string>();
			list.Add(symbol);
			return list;
		}

		public void AddSource(params string[] sources) {
			SetSource(sources);
		}

		public void SetSource(params string[] sources) {
			if (sources.Length > 1)
				throw new ArgumentException("Error: it is allowed to use up to one source.");
			this.source = sources[0];
			if (snapshotPtr != InvalidSnapshot)
				Dispose();
			CreateSnapshot();
		}

		#endregion
	}
}
