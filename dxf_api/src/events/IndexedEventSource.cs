#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.events
{
    public class IndexedEventSource
    {
        /// <summary>
        ///     The default source with zero <see cref="Id"/> for all events that do not support
        ///     multiple sources.
        /// </summary>
        public static readonly IndexedEventSource DEFAULT = new IndexedEventSource(0, "DEFAULT");

        public IndexedEventSource(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        ///     Gets the identifier of this order source.
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        ///     Gets the name of this order source.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     Returns a string representation of the object.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Indicates whether some other indexed event source has the same id.
        ///     Return <c>true</c> if this object is the same id as the obj argument; <c>false</c>
        ///     otherwise.
        /// </summary>
        /// <param name="obj">Other object.</param>
        /// <returns><c>true</c> if this object is the same id as the obj argument; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj == this || obj is IndexedEventSource && Id == ((IndexedEventSource)obj).Id;
        }

        /// <summary>
        ///     Returns a hash code value for this object.
        ///     The result of this method is equal to <see cref="Id"/>.
        /// </summary>
        /// <returns>Hash code value for this object.</returns>
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
