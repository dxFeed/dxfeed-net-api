#region License
// Copyright (C) 2010-2016 Devexperts LLC
//
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// http://mozilla.org/MPL/2.0/.
#endregion

using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.api.events.market;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.dxfeed.sample.console
{
    //TODO: fill AssemblyInfo
    class LastEventsConsole
    {
        static void Main(string[] args)
        {
            //TODO: comments
            /*
             * Permanent subscription to the world is performed with a special property named "dxfeed.qd.subscribe.ticker".
             * Its value consists of a comma-separated list of records, followed by a space, followed by a comma-separated
             * list of symbols. Record names for composite (NBBO) events are the same as the corresponding event classes
             * in API. The string below defines subscription for quote, trade, summary, and profile composite events:
             */
            string records = "Quote,Trade,Summary,Profile";

            /*
             * Records for regional-exchange events are derived by appending "&" (ampersand) and the a single-digit
             * exchange code. Regexp-like syntax can be used instead of listing them all. For example, the commented
             * line below and to the mix a subscription on regional quotes from all potential exchange codes A-Z
             */
            // String records = "Quote,Trade,Summary,Profile,Quote&[A-Z]";
            /*
             * There is an important trade-off between a resource consumption and speed of access to the last events.
             * The whole world of stocks and options from all the exchanges is very large and will consume gigabytes
             * of memory to cache. Moreover, this cache has to be constantly kept up-to-date which consumes a lot of
             * network and CPU.
             *
             * A particular application's uses cases has to be studied to figure out what is option for this particular
             * application. If some events are known be rarely needed and a small delay while access them can be
             * tolerated, then it is not worth configuring a permanent subscription for them. The code in this
             * sample works using DXFeed.getLastEventPromise method that will request event from upstream data provider
             * if it is not present in local in-memory cache.
             */

            /*
             * There are multiple ways to specify a list of symbols. It is typically taken from IPF file and its
             * specification consists of an URL to the file which has to contain ".ipf" in order to be recognized.
             * The string below defines subscription for all symbols that are available on the demo feed.
             */
            string symbols = "http://dxfeed.s3.amazonaws.com/masterdata/ipf/demo/mux-demo.ipf.zip";

            /*
             * Permanent subscription property "dxfeed.qd.subscribe.ticker" can be directly placed into the
             * "dxfeed.properties" file and no custom DXEndpoint instance will be needed. Here it is explicitly
             * specified using a DXFeedEndpoint.Builder class. Note, that we don't use "connect" method on DXEndpoint.
             * It is assumed by this sample that "dxfeed.address" property is specified in "dxfeed.properties" and
             * connection is automatically established to that address. To run this sample application without
             * "dxfeed.properties" file, specify connection address using JVM option. For example, use
             * "-Ddxfeed.address=demo.dxfeed.com:7300" to connect to the demo feed.
             */
            //TODO: endpoint
            //DXEndpoint endpoint = DXEndpoint.newBuilder()
            //    .withProperty("dxfeed.qd.subscribe.ticker", records + " " + symbols)
            //    .build();

            /*
             * The actual client code does not need a reference to DXEndpoint, which only contains lifecycle
             * methods like "connect" and "close". The client code needs a reference to DXFeed.
             */
            //TODO: getting feed via endpoint
            //DXFeed feed = endpoint.getFeed();
            DXFeed feed = DXFeed.GetInstance();

            /*
             * Print a short help.
             */
            Console.WriteLine("Type symbols to get their quote, trade, summary, and profile event snapshots");

            TimeSpan taskTimeout = TimeSpan.FromSeconds(1);

            /*
             * The main loop of this sample loops forever reading symbols from console and printing events.
             */
            while (true)
            {
                /*
                 * User of this sample application can type symbols on the console. Symbol like "IBM" corresponds
                 * to the stock. Symbol like "IBM&N" corresponds to the information from a specific exchange.
                 * See the dxFeed Symbol guide at http://www.dxfeed.com/downloads/documentation/dxFeed_Symbol_Guide.pdf
                 */
                string symbol = Console.ReadLine();

                /*
                 * The first step is to extract promises for all events that we are interested in. This way we
                 * can get an event even if we have not previously subscribed for it.
                 */
                Task<LastingEvent> quoteTask = feed.GetLastEventPromise<IDxQuote>(symbol, new CancellationTokenSource(taskTimeout).Token);
                Task<LastingEvent> tradeTask = feed.GetLastEventPromise<IDxTrade>(symbol, new CancellationTokenSource(taskTimeout).Token);
                Task<LastingEvent> summaryTask = feed.GetLastEventPromise<IDxSummary>(symbol, new CancellationTokenSource(taskTimeout).Token);

                /*
                 * All promises are put into a list for convenience.
                 */
                List<Task<LastingEvent>> tasks = new List<Task<LastingEvent>>();
                tasks.Add(quoteTask);
                tasks.Add(tradeTask);
                tasks.Add(summaryTask);

                /*
                 * Profile events are composite-only. They are not available for regional symbols like
                 * "IBM&N" and the attempt to retrieve never completes (will timeout), so we don't event try.
                 */
                if (!MarketEventSymbols.HasExchangeCode(symbol))
                {
                    Task<LastingEvent> profileTask = feed.GetLastEventPromise<IDxProfile>(symbol, new CancellationTokenSource(taskTimeout).Token);
                    tasks.Add(profileTask);
                }

                /*
                 * If the events are available in the in-memory cache, then the promises will be completed immediately.
                 * Otherwise, a request to the upstream data provider is sent. Below we combine promises using
                 * Promises utility class from DXFeed API in order to wait for at most 1 second for all of the
                 * promises to complete. The last event promise never completes exceptionally and we don't
                 * have to specially process a case of timeout, so "awaitWithoutException" is used to continue
                 * normal execution even on timeout. This sample prints a special message in the case of timeout.
                 */
                try
                {
                    Task.WaitAll(tasks.ToArray());
                }
                catch (AggregateException)
                {
                    Console.WriteLine("Request timed out");
                }

                /*
                 * The combination above is used only to ensure a common wait of 1 second. Promises to individual events
                 * are completed independently and the corresponding events can be accessed even if some events were not
                 * available for any reason and the wait above had timed out. This sample just prints all results.
                 * "null" is printed when the event is not available.
                 */
                foreach (Task<LastingEvent> task in tasks)
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                        Console.WriteLine(task.Result);
                }
            }

        }
    }
}
