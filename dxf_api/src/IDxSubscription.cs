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
        /// It's not applicable to Candle subscription.
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        void AddSymbol(string symbol);

        /// <summary>
        /// Add candle symbol to subscription.
        /// This method applies only to candle subscription. For other events it does not make sense.
        /// </summary>
        /// <param name="symbol">candle symbol</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        void AddSymbol(CandleSymbol symbol);

        /// <summary>
        /// Add multiply symbols to subscription.
        /// It's not applicable to Candle subscription.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        void AddSymbols(params string[] symbols);

        /// <summary>
        /// Add multiply candle symbols to subscription.
        /// This method applies only to candle subscription. For other events it does not make sense.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        void AddSymbols(params CandleSymbol[] symbols);

        /// <summary>
        /// Remove multiply symbols from subscription.
        /// It's not applicable to Candle subscription.
        /// 
        /// Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="DxException"></exception>
        void RemoveSymbols(params string[] symbols);

        /// <summary>
        /// Remove multiply symbols from subscription.
        /// This method applies only to candle subscription. For other events it does not make sense.
        /// 
        /// Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="DxException"></exception>
        void RemoveSymbols(params CandleSymbol[] symbols);

        /// <summary>
        /// Set multiply symbols to subscription.
        /// It's not applicable to Candle subscription.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        void SetSymbols(params string[] symbols);

        /// <summary>
        /// Set multiply symbols to subscription.
        /// This method applies only to candle subscription. For other events it does not make sense.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <exception cref="ArgumentException">Invalid symbol parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription</exception>
        /// <exception cref="DxException"></exception>
        void SetSymbols(params CandleSymbol[] symbols);

        /// <summary>
        /// Clear all symbols from subscription.
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
        /// <exception cref="ArgumentException">Invalid source parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription</exception>
        /// <exception cref="DxException"></exception>
        void AddSource(params string[] sources);

        /// <summary>
        /// Remove existing sources and set new
        /// </summary>
        /// <param name="sources">list of sources</param>
        /// <exception cref="ArgumentException">Invalid source parameter</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription</exception>
        /// <exception cref="DxException"></exception>
        void SetSource(params string[] sources);
    }
}