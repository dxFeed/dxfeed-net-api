#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using com.dxfeed.api.candle;

namespace com.dxfeed.api
{
    /// <summary>
    ///     Interface provides operation with subscription parameters.
    /// </summary>
    public interface IDxSubscription : IDisposable
    {
        /// <summary>
        ///     Add symbol to subscription.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbol"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void AddSymbol(string symbol);

        /// <summary>
        ///     Add candle symbol to subscription.
        /// </summary>
        /// <param name="symbol"><see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbol"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void AddSymbol(CandleSymbol symbol);

        /// <summary>
        ///     Add multiply symbols to subscription.
        /// </summary>
        /// <param name="symbols">List of symbols.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbol"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void AddSymbols(params string[] symbols);

        /// <summary>
        ///     Add multiply candle symbols to subscription.
        /// </summary>
        /// <param name="symbols">List of <see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void AddSymbols(params CandleSymbol[] symbols);

        /// <summary>
        ///     <para>
        ///         Remove multiply symbols from subscription.
        ///     </para>
        ///     <para>
        ///         Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
        ///     </para>
        /// </summary>
        /// <param name="symbols">List of symbols.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void RemoveSymbols(params string[] symbols);

        /// <summary>
        ///     <para>
        ///         Remove multiply symbols from subscription.
        ///     </para>
        ///     <para>
        ///         Snapshot will be disposed if symbols contains snapshot symbol (for Snapshots only).
        ///     </para>
        /// </summary>
        /// <param name="symbols">List of <see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void RemoveSymbols(params CandleSymbol[] symbols);

        /// <summary>
        ///     Set multiply symbols to subscription.
        /// </summary>
        /// <param name="symbols">List of symbols.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void SetSymbols(params string[] symbols);

        /// <summary>
        ///     Set multiply symbols to subscription.
        /// </summary>
        /// <param name="symbols">List of <see cref="CandleSymbol"/>.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="symbols"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one symbol to snapshot subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void SetSymbols(params CandleSymbol[] symbols);

        /// <summary>
        ///     <para>
        ///         Clear all symbols from subscription.
        ///     </para>
        ///     <para>
        ///         Snapshot will be <see cref="IDisposable.Dispose()"/>.
        ///     </para>
        /// </summary>
        /// <exception cref="DxException">Internal error.</exception>
        void Clear();

        /// <summary>
        ///     Get all symbols from subscription.
        /// </summary>
        /// <returns>List of subscribed symbols.</returns>
        /// <exception cref="DxException">Internal error.</exception>
        IList<string> GetSymbols();

        /// <summary>
        ///     Add <see cref="events.OrderSource"/> to subscription.
        /// </summary>
        /// <param name="sources">List of <see cref="events.OrderSource"/> names.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="sources"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void AddSource(params string[] sources);

        /// <summary>
        ///     Remove existing <see cref="events.OrderSource"/> from subscription and set new.
        /// </summary>
        /// <param name="sources">List of <see cref="events.OrderSource"/> names.</param>
        /// <exception cref="ArgumentException">Invalid <paramref name="sources"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">You try to add more than one source to subscription.</exception>
        /// <exception cref="DxException">Internal error.</exception>
        void SetSource(params string[] sources);
    }
}
