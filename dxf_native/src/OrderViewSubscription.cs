using System;
using System.Collections.Generic;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.events;

namespace com.dxfeed.native {
    public class OrderViewSubscription : IDxSubscription, IDxFeedListener {

        private IDxOrderViewListener listener = null;
        private IDxConnection connection = null;
        private IDxSubscription subscription = null;

        public OrderViewSubscription(IDxConnection connection, IDxOrderViewListener listener) {
            this.connection = connection;
            this.listener = listener;
        }

        #region Implementation of IDxSubscription

        public void AddSymbol(string symbol) {
            if (subscription == null) {
                return;
            }
            throw new NotImplementedException();
        }

        public void AddSymbol(CandleSymbol symbol) {
            if (subscription == null) {
                return;
            }
            throw new NotImplementedException();
        }

        public void AddSymbols(params string[] symbols) {
            if (subscription == null) {
                return;
            }
            throw new NotImplementedException();
        }

        public void AddSymbols(params CandleSymbol[] symbols) {
            if (subscription == null) {
                return;
            }
            throw new NotImplementedException();
        }

        public void RemoveSymbols(params string[] symbols) {
            if (subscription == null) {
                return;
            }
            throw new NotImplementedException();
        }

        public void RemoveSymbols(params CandleSymbol[] symbols) {
            if (subscription == null) {
                return;
            }
            throw new NotImplementedException();
        }

        public void SetSymbols(params string[] symbols) {
            if (subscription == null) {
                return;
            }
            subscription.SetSymbols(symbols);
        }

        public void SetSymbols(params CandleSymbol[] symbols) {
            if (subscription == null) {
                return;
            }
            throw new NotImplementedException();
        }

        public void Clear() {
            subscription.Clear();
        }

        public IList<string> GetSymbols() {
            if (subscription == null) {
                return null;
            }
            return subscription.GetSymbols();
        }

        public void AddSource(params string[] sources) {
            SetSource(sources);
        }

        public void SetSource(params string[] sources) {
            if (subscription != null) {
                return;
            }
            subscription = connection.CreateSubscription(EventType.Order, this);
            subscription.SetSource(sources);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (subscription != null) {
                        subscription.Dispose();
                        subscription = null;
                    }
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #region Implementation of IDxFeedListener
        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote {
            throw new NotImplementedException();
        }

        public void OnTrade<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTrade {
            throw new NotImplementedException();
        }

        public void OnOrder<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxOrder {
            //listener.OnUpdate<TB, TE>(buf);
            //listener.OnUpdate<NativeEventBuffer<NativeOrder>, NativeOrder>(buf);
            throw new NotImplementedException();
        }

        public void OnProfile<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxProfile {
            throw new NotImplementedException();
        }

        public void OnFundamental<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxSummary {
            throw new NotImplementedException();
        }

        public void OnTimeAndSale<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxTimeAndSale {
            throw new NotImplementedException();
        }
        #endregion
    }
}
