#region License

/*
Copyright © 2010-2019 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.dxfeed.api;
using com.dxfeed.api.events;

namespace dxf_promises_sample {
    internal class PromisesSample {
        private static void GetLastEventPromisesSample() {
            string[] symbols = {"C", "IBM", "MSFT"};
            var feed = DXFeed.GetInstance();
            var promises = feed.GetLastEventsPromises<IDxTrade>(
                symbols,
                new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
            // combine the list of promises into one with Task utility method and wait
            try {
                Task.WaitAll(promises.Cast<Task>().ToArray());
            } catch (AggregateException ae) {
                foreach (var exc in ae.InnerExceptions)
                    if (!(exc is OperationCanceledException))
                        Console.WriteLine(exc);
            }

            // now iterate the promises to retrieve results
            Console.WriteLine("Last events for {0} symbols:", string.Join(", ", symbols));
            foreach (var promise in promises)
                // result received exceptionally if this event was not found
                // so first check that task completes successfully
                if (promise.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine(promise.Result);
                else
                    Console.WriteLine("not found");
        }

        private static void GetIndexedEventsPromise() {
            var feed = DXFeed.GetInstance();
            var tsPromise = feed.GetIndexedEventsPromise<IDxTimeAndSale>("IBM", IndexedEventSource.DEFAULT,
                new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);
            Console.WriteLine("TimeAndSale events:");
            try {
                foreach (var result in tsPromise.Result)
                    Console.WriteLine(result);
            } catch (AggregateException ae) {
                foreach (var exc in ae.InnerExceptions)
                    if (exc is OperationCanceledException)
                        Console.WriteLine("not found");
                    else
                        Console.WriteLine(exc);
            }

            var orderPromise = feed.GetIndexedEventsPromise<IDxOrder>("IBM", OrderSource.NTV,
                new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);
            Console.WriteLine("Order#NTV events:");
            try {
                foreach (var result in orderPromise.Result)
                    Console.WriteLine(result);
            } catch (AggregateException ae) {
                foreach (var exc in ae.InnerExceptions)
                    if (exc is OperationCanceledException)
                        Console.WriteLine("not found");
                    else
                        Console.WriteLine(exc);
            }
        }

        private static void Main() {
            GetLastEventPromisesSample();
            GetIndexedEventsPromise();
        }
    }
}