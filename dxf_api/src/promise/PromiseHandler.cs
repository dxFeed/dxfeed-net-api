#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

namespace com.dxfeed.promise
{
    public interface PromiseHandler<T> where T : class
    {
        //TODO: comments
        /// <summary>
        /// Invoked when promised computation has
        /// {@link Promise#complete(Object) completed normally},
        /// or {@link Promise#completeExceptionally(Throwable) exceptionally},
        /// or was {@link Promise#cancel() canceled}.
        /// @param promise the promise.
        /// </summary>
        /// <param name="promise"></param>
        void PromiseDone(Promise<T> promise);
    }
}
