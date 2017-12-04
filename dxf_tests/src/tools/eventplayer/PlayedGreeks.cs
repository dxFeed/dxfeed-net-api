using com.dxfeed.api;
using com.dxfeed.api.events;
using System;

namespace com.dxfeed.tests.tools.eventplayer
{
    /// <summary>
    ///     Greeks event implementation for test event player.
    /// </summary>
    internal class PlayedGreeks : IPlayedEvent<DxTestGreeks>, IDxGreeks
    {
        /// <summary>
        ///     Creates Greeks events via all parameters.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="time"></param>
        /// <param name="sequence"></param>
        /// <param name="greeksPrice"></param>
        /// <param name="volatility"></param>
        /// <param name="delta"></param>
        /// <param name="gamma"></param>
        /// <param name="theta"></param>
        /// <param name="rho"></param>
        /// <param name="vega"></param>
        /// <param name="index"></param>
        /// <param name="eventFlags"></param>
        internal PlayedGreeks(string symbol, long time, int sequence, double greeksPrice, 
            double volatility, double delta, double gamma, double theta, double rho, double vega, 
            long index, EventFlag eventFlags)
        {
            EventSymbol = symbol;
            EventFlags = eventFlags;

            Delta = delta;
            Gamma = gamma;
            GreeksPrice = greeksPrice;
            Rho = rho;
            Sequence = sequence;
            Theta = theta;
            TimeStamp = time;
            Time = Tools.UnixTimeToDate(TimeStamp);
            Vega = vega;
            Volatility = volatility;
            Index = index;

            Params = new EventParams(EventFlags, ((ulong)TimeStamp << 32) + (uint)Sequence, 0);
            Data = new DxTestGreeks(TimeStamp, Sequence, GreeksPrice, Volatility, Delta, Gamma, Theta, Rho, Vega, Index, EventFlags);
        }

        /// <summary>
        ///     Creates Greeks event from another object.
        /// </summary>
        /// <param name="greeks">Other Greeks object.</param>
        internal PlayedGreeks(IDxGreeks greeks)
        {
            EventSymbol = greeks.EventSymbol;
            EventFlags = greeks.EventFlags;

            Delta = greeks.Delta;
            Gamma = greeks.Gamma;
            GreeksPrice = greeks.GreeksPrice;
            Rho = greeks.Rho;
            Sequence = greeks.Sequence;
            Theta = greeks.Theta;
            TimeStamp = greeks.TimeStamp;
            Time = Tools.UnixTimeToDate(TimeStamp);
            Vega = greeks.Vega;
            Volatility = greeks.Volatility;
            Index = greeks.Index;

            Params = new EventParams(EventFlags, ((ulong)TimeStamp << 32) + (uint)Sequence, 0);
            Data = new DxTestGreeks(TimeStamp, Sequence, GreeksPrice, Volatility, Delta, Gamma, Theta, Rho, Vega, Index, EventFlags);
        }

        public DxTestGreeks Data
        {
            get; private set;
        }

        public double Delta
        {
            get; private set;
        }

        public EventFlag EventFlags
        {
            get; set;
        }

        public string EventSymbol
        {
            get; private set;
        }

        public double Gamma
        {
            get; private set;
        }

        public double GreeksPrice
        {
            get; private set;
        }

        public long Index
        {
            get; private set;
        }

        public int MaxSequence
        {
            get; private set;
        }

        public EventParams Params
        {
            get; private set;
        }

        public double Rho
        {
            get; private set;
        }

        public int Sequence
        {
            get; private set;
        }

        public IndexedEventSource Source
        {
            get
            {
                return IndexedEventSource.DEFAULT;
            }
        }

        public double Theta
        {
            get; private set;
        }

        public DateTime Time
        {
            get; private set;
        }

        public long TimeStamp
        {
            get; private set;
        }

        public double Vega
        {
            get; private set;
        }

        public double Volatility
        {
            get; private set;
        }

        object IPlayedEvent.Data
        {
            get
            {
                return Data as object;
            }
        }

        object IDxEventType.EventSymbol
        {
            get
            {
                return EventSymbol as object;
            }
        }

        public object Clone()
        {
            return new PlayedGreeks(this);
        }
    }
}
