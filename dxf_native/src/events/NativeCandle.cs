using System;
using System.Globalization;
using com.dxfeed.api.data;
using com.dxfeed.api.events;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events {
    public struct NativeCandle : IDxCandle {
        private readonly DxCandle candle;
        private string symbol;

        internal unsafe NativeCandle(DxCandle* c, string symbol) {
            candle = *c;
            this.symbol = symbol;
        }

        public override string ToString() {
            return string.Format(CultureInfo.InvariantCulture, "Candle: {{{11}, DateTime: {0:o}, " +
            "Sequence: {1}, Count: {2:0.00}, Open: {3:0.000000}, High: {4:0.000000}, " +
            "Low: {5:0.000000}, Close: {6:0.000000}, Volume: {7:0.0}, VWAP: {8:0.0}, " +
            "BidVolume: {9:0.0}, AskVolume: {10:0.0} }}",
                DateTime, Sequence, Count, Open, High, Low, Close, Volume, VWAP, BidVolume, AskVolume, Symbol);
        }

        #region Implementation of IDxCandle

        public DateTime DateTime {
            get { 
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                dateTime = dateTime.AddMilliseconds(candle.time);
                return dateTime;
            }
        }

        public int Sequence { 
            get { 
                return candle.sequence; 
            } 
        }

        public double Count {
            get {
                return candle.count;
            }
        }

        public double Open {
            get {
                return candle.open;
            } 
        }

        public double High {
            get {
                return candle.high;
            }
        }

        public double Low {
            get {
                return candle.low;
            } 
        }

        public double Close {
            get {
                return candle.close;
            } 
        }

        public double Volume {
            get {
                return candle.volume;
            } 
        }

        public double VWAP {
            get {
                return candle.vwap;
            } 
        }

        public double BidVolume {
            get {
                return candle.bid_volume;
            } 
        }

        public double AskVolume {
            get {
                return candle.ask_volume;
            } 
        }

        public string Symbol {
            get {
                return symbol;
            }
        }

        #endregion
    }
}
