using System;
using System.Collections.Generic;
using com.dxfeed.api.candle;

namespace com.dxfeed.api {
    public interface IDxSubscription : IDisposable {

        /// <summary>
        /// Add symbol to subscription
        /// </summary>
        /// <param name="symbol">symbol</param>
        void AddSymbol(string symbol);

        /// <summary>
        /// Add candle symbol to subscription
        /// </summary>
        /// <param name="symbol">candle symbol</param>
        void AddSymbol(CandleSymbol symbol);

        /// <summary>
        /// Add multiply symbols to subscription.
        /// It's not applicable to Candle symbols.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        void AddSymbols(params string[] symbols);

        /// <summary>
        /// Remove multiply symbols from subscription.
        /// It's not applicable to Candle symbols.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        void RemoveSymbols(params string[] symbols);

        /// <summary>
        /// Set multiply symbols to subscription.
        /// It's not applicable to Candle symbols.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        void SetSymbols(params string[] symbols);

        /// <summary>
        /// Clear all symbols from subscription.
        /// It's not applicable to Candle symbols.
        /// </summary>
        void Clear();

        /// <summary>
        /// Get all symbols list from subscription.
        /// </summary>
        /// <returns>listof subscribed symbols</returns>
        IList<string> GetSymbols();

        /// <summary>
        /// Add order source to subscription.
        /// </summary>
        /// <param name="sources">list of souces</param>
        void AddSource(params string[] sources);

        /// <summary>
        /// Remove existing sources and set new
        /// </summary>
        /// <param name="sources">list of sources</param>
        void SetSource(params string[] sources);
    }
}