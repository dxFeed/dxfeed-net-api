using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.data;
using com.dxfeed.api.events;

namespace com.dxfeed.native
{
    public class SnapshotDataProvider : IDxOrderSnapshotListener,
        IDxCandleSnapshotListener,
        IDxTimeAndSaleSnapshotListener,
        IDxSpreadOrderSnapshotListener,
        IDxGreeksSnapshotListener,
        IDxSeriesSnapshotListener
    {
        private IDxSubscription sub;
        private IDxConnection connection;
        private EventType eventType;
        private OrderSource source;
        private string symbol;
        private DateTime fromTime;
        private DateTime toTime;
        private object locker = new object();
        private List<IDxIndexedEvent> events;
        private bool subscribe = true;

        public SnapshotDataProvider(IDxConnection connection, EventType eventType, OrderSource source, string symbol,
            DateTime fromTime,
            DateTime toTime)
        {
            this.connection = connection;
            this.eventType = eventType;
            this.source = source;
            this.symbol = symbol;
            this.fromTime = fromTime;
            this.toTime = toTime;
        }

        public async Task<List<IDxIndexedEvent>> Run()
        {
            return await Task.Run(() =>
            {
                sub = connection.CreateSnapshotSubscription(eventType, fromTime, this);

                if (eventType == EventType.Order || eventType == EventType.SpreadOrder)
                {
                    sub.AddSource(source);
                    sub.AddSymbol(symbol);
                }
                else if (eventType == EventType.Candle)
                {
                    sub.AddSymbol(CandleSymbol.ValueOf(symbol));
                }
                else
                {
                    sub.AddSymbol(symbol);
                }

                while (true)
                {
                    lock (locker)
                    {
                        if (!subscribe)
                        {
                            sub.Clear();
                            sub.Dispose();
                            sub = null;
                            connection = null;
                            
                            break;
                        }
                    }

                    Thread.Sleep(100);
                }

                return events;
            });
        }

        public void OnOrderSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxOrder
        {
            if (buf.Size <= 0) return;
            if (buf.First().Time <= toTime) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxIndexedEvent>)buf.Skip(1).ToList());
                subscribe = false;
            }
        }

        public void OnCandleSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxCandle
        {
            if (buf.Size <= 0) return;
            if (buf.First().Time <= toTime) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxIndexedEvent>)buf.Skip(1).ToList());
                subscribe = false;
            }
        }

        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxTimeAndSale
        {
            if (buf.Size <= 0) return;
            if (buf.First().Time <= toTime) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxIndexedEvent>)buf.Skip(1).ToList());
                subscribe = false;
            }
        }

        public void OnSpreadOrderSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxSpreadOrder
        {
            if (buf.Size <= 0) return;
            if (buf.First().Time <= toTime) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxIndexedEvent>)buf.Skip(1).ToList());
                subscribe = false;
            }
        }

        public void OnGreeksSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxGreeks
        {
            if (buf.Size <= 0) return;
            if (buf.First().Time <= toTime) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxIndexedEvent>)buf.Skip(1).ToList());
                subscribe = false;
            }
        }

        public void OnSeriesSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxSeries
        {
            if (buf.Size <= 0) return;
            if (buf.First().Time <= toTime) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxIndexedEvent>)buf.Skip(1).ToList());
                subscribe = false;
            }
        }
    }
}