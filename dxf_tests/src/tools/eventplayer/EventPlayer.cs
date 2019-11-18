#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

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
                throw new ArgumentNullException("playEventsList");
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

        public void PlaySnapshot(string symbol, params IPlayedEvent[] playEventsList)
        {
            if (playEventsList == null)
                throw new ArgumentNullException("playEventsList");
            FieldInfo nativeSubscription = typeof(DXFeedSubscription<E>).GetField("subscriptionInstance", BindingFlags.NonPublic | BindingFlags.Instance);
            if (nativeSubscription == null)
                throw new InvalidOperationException("subscriptionInstance field not found!");
            MethodInfo onEvent = typeof(NativeSnapshotSubscription).GetMethod("OnEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            if (onEvent == null)
                throw new InvalidOperationException("OnEvent method not found!");

            char[] symbolChars = symbol.ToCharArray();
            IntPtr recordsPtr = IntPtr.Zero;
            IntPtr snapshotPtr = IntPtr.Zero;
            try
            {
                IntPtr nextRecordPtr = IntPtr.Zero;
                foreach (var playedEvent in playEventsList)
                {
                    if (!(playedEvent is E))
                        throw new ArgumentException("The one of played events is not " + typeof(E));
                    var dataSize = Marshal.SizeOf(playedEvent.Data);
                    if (recordsPtr == IntPtr.Zero)
                    {
                        recordsPtr = Marshal.AllocHGlobal(dataSize * playEventsList.Length);
                        nextRecordPtr = recordsPtr;
                    }
                    Marshal.StructureToPtr(playedEvent.Data, nextRecordPtr, false);
                    nextRecordPtr += dataSize;
                }

                DxTestSnapshotData snapshot;
                snapshot.event_type = EventTypeUtil.GetEventsType(typeof(E));
                snapshot.symbol = Marshal.UnsafeAddrOfPinnedArrayElement(symbolChars, 0);
                snapshot.records_count = playEventsList.Length;
                snapshot.records = recordsPtr;
                snapshotPtr = Marshal.AllocHGlobal(Marshal.SizeOf(snapshot));
                Marshal.StructureToPtr(snapshot, snapshotPtr, false);

                onEvent.Invoke(nativeSubscription.GetValue(subscription), new object[] {
                        snapshotPtr, IntPtr.Zero
                    });
            }
            finally
            {
                if (recordsPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(recordsPtr);
                if (snapshotPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(snapshotPtr);
            }
        }

        #region private members

        private DXFeedSubscription<E> subscription = null;

        #endregion
    }
}
