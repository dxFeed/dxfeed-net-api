#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api.events;
using System;
using System.Collections.Generic;

namespace com.dxfeed.api
{
    /// <summary>
    ///     The universal event listener class for handling all native events. It is aggregates
    ///     as simple as snapshots events and calls <see cref="IDXFeedEventListener{E}"/> from
    ///     list. This class used as event listener converter from native events wrapper to
    ///     high-level event listener.
    /// </summary>
    /// <typeparam name="E">The type of event.</typeparam>
    internal class DXFeedEventHandler<E> :
        IDxCandleListener,
        IDxGreeksListener,
        IDxOrderListener,
        IDxProfileListener,
        IDxQuoteListener,
        IDxSeriesListener,
        IDxSpreadOrderListener,
        IDxFundamentalListener,
        IDxTheoPriceListener,
        IDxTimeAndSaleListener,
        IDxTradeListener,
        IDxTradeETHListener,
        IDxUnderlyingListener,
        IDxConfigurationListener,
        IDxOrderSnapshotListener,
        IDxCandleSnapshotListener,
        IDxTimeAndSaleSnapshotListener,
        IDxSpreadOrderSnapshotListener,
        IDxGreeksSnapshotListener,
        IDxSeriesSnapshotListener
        where E : IDxEventType
    {
        private IList<IDXFeedEventListener<E>> eventListeners = null;
        private object eventListenerLocker = null;
        private Type subscriptionType;

        /// <summary>
        ///     Creates event handler.
        /// </summary>
        /// <param name="eventListeners">Listeners to call on events received.</param>
        /// <param name="eventListenerLocker">Listeners list locker.</param>
        /// <exception cref="ArgumentNullException">If listener locker is null.</exception>
        public DXFeedEventHandler(IList<IDXFeedEventListener<E>> eventListeners, object eventListenerLocker)
        {
            if (eventListenerLocker == null)
                throw new ArgumentNullException("eventListenerLocker");
            this.eventListeners = eventListeners;
            this.eventListenerLocker = eventListenerLocker;
            subscriptionType = typeof(E);
        }

        #region Implementation of IDxFeedListener

        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxQuote)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxTrade)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxOrder)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxProfile)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxSummary)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxTimeAndSale)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxCandleListener

        public void OnCandle<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxCandle)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxGreeks

        public void OnGreeks<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxGreeks)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxSeries

        public void OnSeries<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxSeries)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxSpreadOrder

        public void OnSpreadOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxSpreadOrder)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxTheoPrice

        public void OnTheoPrice<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTheoPrice
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxTheoPrice)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxTradeEth

        public void OnTradeETH<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTradeETH
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxTrade)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxUnderlying

        public void OnUnderlying<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxUnderlying
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxUnderlying)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxConfiguration

        public void OnConfiguration<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxConfiguration
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxConfiguration)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxOrder snapshot

        public void OnOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxOrder)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxCandle snapshot

        public void OnCandleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxCandle
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxCandle)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxTimeAndSale snapshot

        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxTimeAndSale)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxSpreadOrder snapshot

        public void OnSpreadOrderSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSpreadOrder
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxSpreadOrder)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxGreeks snapshot

        public void OnGreeksSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxGreeks
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxGreeks)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        #region Implementation of IDxSeries snapshot

        public void OnSeriesSnapshot<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSeries
        {
            if (!subscriptionType.IsAssignableFrom(typeof(IDxSeries)))
                return;
            List<E> events = new List<E>();
            foreach (var item in buf)
                events.Add((E)(object)item);
            CallListeners(events);
        }

        #endregion

        private void CallListeners(IList<E> events)
        {
            lock (eventListenerLocker)
            {
                foreach (IDXFeedEventListener<E> listener in eventListeners)
                    listener.EventsReceived(events);
            }
        }
    }
}
