using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.util;
using com.dxfeed.native;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Provides sending events manually through native subscription instance. 
    ///     This class uses Reflection to get access to native subscription and C data types 
    ///     struct model.
    /// </summary>
    /// <typeparam name="E">The type of played events.</typeparam>
    public class EventPlayer<E>
        where E : class, IDxEventType
    {
        /// <summary>
        ///     Creates new event player.
        /// </summary>
        /// <param name="subscription">DXFeed subscription instance.</param>
        public EventPlayer(DXFeedSubscription<E> subscription)
        {
            this.subscription = subscription;
        }

        public void PlayEvents(string symbol, params IPlayedEvent[] playEventsList)
        {
            if (playEventsList == null)
                throw new ArgumentNullException("eventsData");
            FieldInfo nativeSubscription = typeof(DXFeedSubscription<E>).GetField("subscriptionInstance", BindingFlags.NonPublic | BindingFlags.Instance);
            if (nativeSubscription == null)
                throw new InvalidOperationException("subscriptionInstance field not found!");
            MethodInfo onEvent = typeof(NativeSubscription).GetMethod("OnEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            if (onEvent == null)
                throw new InvalidOperationException("OnEvent method not found!");

            char[] symbolChars = symbol.ToCharArray();
            IntPtr symbolCharsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(symbolChars, 0);

            foreach (var playedEvent in playEventsList)
            {
                DxTestEventParams nativeEventParams = new DxTestEventParams(playedEvent.Params.Flags, playedEvent.Params.TimeIntField, playedEvent.Params.SnapshotKey);
                IntPtr dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(playedEvent.Data));
                IntPtr paramsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(nativeEventParams));
                const int dataCount = 1;
                try
                {
                    Marshal.StructureToPtr(playedEvent.Data, dataPtr, false);
                    Marshal.StructureToPtr(nativeEventParams, paramsPtr, false);
                    onEvent.Invoke(nativeSubscription.GetValue(subscription), new object[] {
                        EventTypeUtil.GetEventsType(playedEvent.GetType()), symbolCharsPtr, dataPtr, dataCount, paramsPtr, IntPtr.Zero
                    });
                }
                finally
                {
                    Marshal.FreeHGlobal(dataPtr);
                    Marshal.FreeHGlobal(paramsPtr);
                }
            }
        }

        #region private members

        private DXFeedSubscription<E> subscription = null;

        #endregion
    }
}
