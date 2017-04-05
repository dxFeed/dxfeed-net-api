#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using System.Collections.Generic;

namespace com.dxfeed.api
{
    //TODO: comments
    public interface IDXFeedSubscription<E>
    {
        /// <summary>
        ///     Attaches subscription to the specified feed.
        /// </summary>
        /// <param name="feed">Feed to attach to.</param>
        void Attach(IDXFeed feed);

        /// <summary>
        ///     Detaches subscription from the specified feed.
        /// </summary>
        /// <param name="feed">Feed to detach from.</param>
        void Detach(IDXFeed feed);

        /// <summary>
        ///     Returns <c>true</c> if this subscription is closed.
        /// </summary>
        bool IsClosed
        {
            get;
        }

        /// <summary>
        ///     Closes this subscription and makes it <i>permanently detached</i>. 
        /// </summary>
        void Close();

        /// <summary>
        ///     Clears the set of subscribed symbols.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Returns a set of subscribed symbols.
        /// </summary>
        /// <returns>Set of subscribed symbols.</returns>
        HashSet<object> GetSymbols();

        /// <summary>
        /// Changes the set of subscribed symbols so that it contains just the symbols from the specified collection.
        /// To conveniently set subscription for just one or few symbols you can use
        /// SetSymbols(params string[] symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">The collection of symbols.</param>
        void SetSymbols(ICollection<object> symbols);

        /// <summary>
        /// Changes the set of subscribed symbols so that it contains just the symbols from the specified array.
        /// This is a convenience method to set subscription to one or few symbols at a time.
        /// When setting subscription to multiple symbols at once it is preferable to use
        /// SetSymbols(ICollection<string> symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">The array of symbols.</param>
        void SetSymbols(params object[] symbols);

        /// <summary>
        /// Adds the specified collection of symbols to the set of subscribed symbols.
        /// To conveniently add one or few symbols you can use
        /// AddSymbols(params string[] symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">Symbols the collection of symbols.</param>
        void AddSymbols(ICollection<object> symbols);

        /// <summary>
        /// Adds the specified array of symbols to the set of subscribed symbols.
        /// This is a convenience method to subscribe to one or few symbols at a time.
        /// When subscribing to multiple symbols at once it is preferable to use
        /// AddSymbols(ICollection<string> symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbols">The array of symbols.</param>
        void AddSymbols(params object[] symbols);

        /// <summary>
        /// Adds the specified symbol to the set of subscribed symbols.
        /// This is a convenience method to subscribe to one symbol at a time that
        /// has a return fast-path for a case when the symbol is already in the set.
        /// When subscribing to multiple symbols at once it is preferable to use
        /// AddSymbols(ICollection<string> symbols) method.
        /// All registered event listeners will receive update on the last events for all
        /// newly added symbols.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void AddSymbols(object symbol);

        /// <summary>
        ///     Removes the specified collection of symbols from the set of subscribed symbols.
        ///     To conveniently remove one or few symbols you can use
        ///     <see cref="RemoveSymbols(object[])"/> method.
        ///
        ///     TODO: implementation notes
        ///
        /// </summary>
        /// <param name="symbols">The collection of symbols.</param>
        void RemoveSymbols(ICollection<object> symbols);

        /// <summary>
        ///     Removes the specified array of symbols from the set of subscribed symbols.
        ///     This is a convenience method to remove one or few symbols at a time.
        ///     When removing multiple symbols at once it is preferable to use
        ///     <see cref="RemoveSymbols(ICollection{object})"/> method.
        ///
        ///     TODO: implementation notes
        ///
        /// </summary>
        /// <param name="symbols">The array of symbols.</param>
        void RemoveSymbols(params object[] symbols);

        /// <summary>
        /// Adds listener for events.
        /// Newly added listeners start receiving only new events.
        /// </summary>
        /// <param name="listener">The event listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        void AddEventListener(IDXFeedEventListener<E> listener);

        /// <summary>
        /// Removes listener for events.
        /// </summary>
        /// <param name="listener">Listener the event listener.</param>
        /// <exception cref="ArgumentNullException">If listener is null.</exception>
        void RemoveEventListener(IDXFeedEventListener<E> listener);
    }
}
