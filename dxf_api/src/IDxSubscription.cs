using System;
using System.Collections.Generic;
using com.dxfeed.api.candle;

namespace com.dxfeed.api {

    /// <summary>
    /// Interface provides operation with subscription parameters
    /// </summary>
    public interface IDxSubscription : IDisposable {

        /// <summary>
        /// Add symbol to subscription
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <exception cref="NativeDxException"></exception>
        void AddSymbol(string symbol);

        /// <summary>
        /// Add candle symbol to subscription
        /// </summary>
        /// <param name="symbol">candle symbol</param>
        /// <exception cref="DxException"></exception>
        void AddSymbol(CandleSymbol symbol);

        /// <summary>
        /// Add multiply symbols to subscription.
        /// It's not applicable to Candle symbols.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="DxException"></exception>
        void AddSymbols(params string[] symbols);

        /// <summary>
        /// Remove multiply symbols from subscription.
        /// It's not applicable to Candle symbols.
        /// On snapshots call Dispose.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="DxException"></exception>
        void RemoveSymbols(params string[] symbols);

        /// <summary>
        /// Set multiply symbols to subscription.
        /// It's not applicable to Candle symbols.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="DxException"></exception>
        /// <exception cref="ArgumentException"></exception>
        void SetSymbols(params string[] symbols);

        /// <summary>
        /// Clear all symbols from subscription.
        /// It's not applicable to Candle symbols.
        /// On snapshots call Dispose.
        /// </summary>
        /// <exception cref="DxException"></exception>
        void Clear();

        /// <summary>
        /// Get all symbols list from subscription.
        /// </summary>
        /// <returns>list of subscribed symbols</returns>
        /// <exception cref="DxException"></exception>
        IList<string> GetSymbols();

        /// <summary>
        /// Add order source to subscription.
        /// </summary>
        /// <param name="sources">list of souces</param>
        /// <exception cref="DxException"></exception>
        void AddSource(params string[] sources);

        /// <summary>
        /// Remove existing sources and set new
        /// </summary>
        /// <param name="sources">list of sources</param>
        void SetSource(params string[] sources);
    }
}