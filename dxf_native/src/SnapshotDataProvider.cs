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
    internal class SnapshotDataProvider : IDxOrderSnapshotListener,
        IDxCandleSnapshotListener,
        IDxTimeAndSaleSnapshotListener,
        IDxSpreadOrderSnapshotListener,
        IDxGreeksSnapshotListener,
        IDxSeriesSnapshotListener
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(5000);
        private readonly IDxConnection connection;
        private readonly EventType eventType;
        private readonly OrderSource source;
        private readonly string symbol;
        private readonly DateTime fromTime;
        private readonly DateTime toTime;
        private TimeSpan timeout;

        private readonly object locker = new object();
        private List<IDxIndexedEvent> events = new List<IDxIndexedEvent>();
        private bool subscribe = true;

        public SnapshotDataProvider(IDxConnection connection, EventType eventType, OrderSource source, string symbol,
            DateTime fromTime,
            DateTime toTime, TimeSpan timeout)
        {
            this.connection = connection;
            this.eventType = eventType;
            this.source = source;
            this.symbol = symbol;
            this.fromTime = fromTime;
            this.toTime = toTime;
            this.timeout = timeout;
        }

        public Task<List<IDxIndexedEvent>> Run(CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            return Task.Run(() =>
            {
                if (timeout == Timeout.InfiniteTimeSpan && cancellationToken == CancellationToken.None)
                {
                    timeout = DefaultTimeout;
                }

                var currentTime = DateTime.Now;

                using (var sub = connection.CreateSnapshotSubscription(eventType, fromTime, this))
                {
                    if (eventType == EventType.Order || eventType == EventType.SpreadOrder)
                    {
                        if (source.Equals(OrderSource.EMPTY))
                        {
                            return new List<IDxIndexedEvent>();
                        }

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

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        lock (locker)
                        {
                            if (!subscribe || cancellationToken == CancellationToken.None &&
                                DateTime.Now > currentTime.AddMilliseconds(timeout.TotalMilliseconds))
                            {
                                break;
                            }
                        }

                        // ReSharper disable once MethodSupportsCancellation
                        Task.Delay(100).Wait();
                    }

                    sub.Clear();

                    return events;
                }
            });
        }

        public void OnOrderSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxOrder
        {
            if (buf.Size <= 0) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxOrder>)buf.Where(e => e.Time < toTime));

                if (buf.First().Time > toTime)
                {
                    subscribe = false;
                }
            }
        }

        public void OnCandleSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxCandle
        {
            if (buf.Size <= 0) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxCandle>)buf.Where(e => e.Time < toTime));

                if (buf.First().Time > toTime)
                {
                    subscribe = false;
                }
            }
        }

        public void OnTimeAndSaleSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxTimeAndSale
        {
            if (buf.Size <= 0) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxTimeAndSale>)buf.Where(e => e.Time < toTime));

                if (buf.First().Time > toTime)
                {
                    subscribe = false;
                }
            }
        }

        public void OnSpreadOrderSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxSpreadOrder
        {
            if (buf.Size <= 0) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxSpreadOrder>)buf.Where(e => e.Time < toTime));

                if (buf.First().Time > toTime)
                {
                    subscribe = false;
                }
            }
        }

        public void OnGreeksSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxGreeks
        {
            if (buf.Size <= 0) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxGreeks>)buf.Where(e => e.Time < toTime));

                if (buf.First().Time > toTime)
                {
                    subscribe = false;
                }
            }
        }

        public void OnSeriesSnapshot<TB, TE>(TB buf) where TB : IDxEventBuf<TE> where TE : IDxSeries
        {
            if (buf.Size <= 0) return;

            lock (locker)
            {
                events = new List<IDxIndexedEvent>((IEnumerable<IDxSeries>)buf.Where(e => e.Time < toTime));

                if (buf.First().Time > toTime)
                {
                    subscribe = false;
                }
            }
        }
    }
}