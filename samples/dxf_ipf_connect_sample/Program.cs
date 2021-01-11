#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.dxfeed.api;
using com.dxfeed.api.candle;
using com.dxfeed.api.connection;
using com.dxfeed.api.data;
using com.dxfeed.api.extras;
using com.dxfeed.ipf;
using com.dxfeed.native;

namespace dxf_ipf_connect_sample {
    internal class Program {
        private static void ConnectionStatusChangeHandler(IDxConnection connection, ConnectionStatus oldStatus, ConnectionStatus newStatus) {
            switch (newStatus) {
                case ConnectionStatus.Connected:
                    Console.WriteLine("Connected!");
                    break;
                case ConnectionStatus.Authorized:
                    Console.WriteLine("Authorized!");
                    break;
                case ConnectionStatus.NotConnected:
                    break;
                case ConnectionStatus.LoginRequired:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus), newStatus, null);
            }
        }

        private static void DisconnectHandler(IDxConnection con) {
            Console.WriteLine("Disconnected");
        }

        private static bool IsFilePath(string path) {
            try {
                return new Uri(path).IsFile;
            } catch (UriFormatException) {
                return true;
            }
        }

        private static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine(
                    "Usage: dxf_ipf_connect_sample <ipf_host> <user> <password> <host:port> <events>\n" +
                    "or:    dxf_ipf_connect_sample <file> <host:port> <event>\n" +
                    "where\n" +
                    "    ipf_host  - The valid ipf host to download instruments (https://tools.dxfeed.com/ipf)\n" +
                    "    user      - The user name to host access\n" +
                    "    password  - The user password to host access\n" +
                    "    host:port - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    events    - Any of the {Profile,Quote,Trade,TimeAndSale,Summary,\n" +
                    "                TradeETH,Candle,Greeks,TheoPrice,Underlying,Series,\n" +
                    "                Configuration}\n" +
                    "    file      - The name of file or archive (.gz or .zip) contains instrument profiles\n\n" +
                    "example: dxf_ipf_connect_sample https://tools.dxfeed.com/ipf?TYPE=STOCK demo demo demo.dxfeed.com:7300 Quote,Trade\n" +
                    "or:      dxf_ipf_connect_sample https://demo:demo@tools.dxfeed.com/ipf?TYPE=STOCK demo.dxfeed.com:7300 Quote,Trade\n" +
                    "or:      dxf_ipf_connect_sample profiles.zip demo.dxfeed.com:7300 Quote,Trade\n"
                );
                return;
            }

            var path = args[0];
            var user = string.Empty;
            var password = string.Empty;
            string dxFeedAddress;
            EventType events;
            List<string> symbols;

            try {
                var reader = new InstrumentProfileReader();
                IList<InstrumentProfile> profiles;
                var dxFeedAddressParamIndex = 1;

                if (IsFilePath(path)) {
                    //Read profiles from local file system
                    using (var inputStream = new FileStream(path, FileMode.Open)) {
                        profiles = reader.Read(inputStream, path);
                    }
                } else {
                    if (args.Length == 5) {
                        user = args[1];
                        password = args[2];
                        dxFeedAddressParamIndex += 2;
                    }

                    //Read profiles from server
                    profiles = reader.ReadFromFile(path, user, password);
                }

                dxFeedAddress = args[dxFeedAddressParamIndex];
                var eventsString = args[dxFeedAddressParamIndex + 1];

                if (!Enum.TryParse(eventsString, true, out events)) {
                    Console.WriteLine($"Unsupported event type: {eventsString}");
                    return;
                }


                if (profiles.Count == 0) {
                    Console.WriteLine("There are no profiles");

                    return;
                }

                Console.WriteLine("Profiles from '{0}' count: {1}", path, profiles.Count);

                symbols = profiles.Select(profile => profile.GetSymbol()).ToList();

                Console.WriteLine(value: $"Symbols: {string.Join(", ", symbols.Take(42).ToArray())}...");
            } catch (Exception exc) {
                Console.WriteLine($"Exception occured: {exc}");

                return;
            }

            Console.WriteLine($"Connecting to {dxFeedAddress} for [{events} on [{string.Join(", ", symbols.Take(42).ToArray())}...] ...");

            NativeTools.InitializeLogging("dxf_ipf_connect_sample.log", true, true);

            using (var connection = new NativeConnection(dxFeedAddress, DisconnectHandler, ConnectionStatusChangeHandler)) {
                IDxSubscription subscription = null;
                try {
                    subscription = connection.CreateSubscription(events, new EventPrinter());

                    if (events == EventType.Candle) {
                        subscription.AddSymbols(symbols.ConvertAll(CandleSymbol.ValueOf).ToArray());
                    } else {
                        subscription.AddSymbols(symbols.ToArray());
                    }

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                } catch (DxException dxException) {
                    Console.WriteLine($"Native exception occured: {dxException.Message}");
                } catch (Exception exc) {
                    Console.WriteLine($"Exception occured: {exc.Message}");
                } finally {
                    subscription?.Dispose();
                }
            }
        }
    }
}
