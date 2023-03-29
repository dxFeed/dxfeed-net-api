#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

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

namespace dxf_ipf_connect_sample
{
    internal class Program
    {
        private static void ConnectionStatusChangeHandler(IDxConnection connection, ConnectionStatus oldStatus,
            ConnectionStatus newStatus)
        {
            switch (newStatus)
            {
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

        private static void DisconnectHandler(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        private static bool IsFilePath(string path)
        {
            try
            {
                return new Uri(path).IsFile;
            }
            catch (UriFormatException)
            {
                return true;
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Usage: \n" +
                    "dxf_ipf_connect_sample <ipf_host> <ipf_user> <ipf_password> [<token>] <host:port> <events>\n" +
                    "or:    \n" +
                    "dxf_ipf_connect_sample <ipf_host|ipf_file> [<token>] <host:port> <events>\n" +
                    "where:\n" +
                    "    ipf_host     - The valid ipf host (with optional base credentials) to \n" +
                    "                   download instruments (https://tools.dxfeed.com/ipf or \n" +
                    "                   https://demo:demo@tools.dxfeed.com/ipf)\n" +
                    "    ipf_user     - The user name to ipf host access\n" +
                    "    ipf_password - The user password to ipf host access\n" +
                    "    host:port    - The address of dxFeed server (demo.dxfeed.com:7300)\n" +
                    "    token        - The authorization token (also could be used by IPF host)\n" +
                    "    events       - Any of the {Profile,Quote,Trade,TimeAndSale,Summary,\n" +
                    "                   TradeETH,Candle,Greeks,TheoPrice,Underlying,Series,\n" +
                    "                   Configuration}\n" +
                    "    ipf_file  - The name of file or archive (.gz or .zip) contains instrument profiles\n\n" +
                    "examples:\n" +
                    "dxf_ipf_connect_sample https://tools.dxfeed.com/ipf?TYPE=STOCK demo demo demo.dxfeed.com:7300 Quote,Trade\n" +
                    "dxf_ipf_connect_sample https://tools.dxfeed.com/ipf?TYPE=STOCK demo demo Z2V0LmR4ZmVlZCxhbW... demo.dxfeed.com:7300 Quote,Trade\n" +
                    "dxf_ipf_connect_sample https://tools.dxfeed.com/ipf?TYPE=STOCK Z2V0LmR4ZmVlZCxhbW... demo.dxfeed.com:7300 Quote,Trade\n" +
                    "dxf_ipf_connect_sample https://demo:demo@tools.dxfeed.com/ipf?TYPE=STOCK demo.dxfeed.com:7300 Quote,Trade\n" +
                    "dxf_ipf_connect_sample https://demo:demo@tools.dxfeed.com/ipf?TYPE=STOCK Z2V0LmR4ZmVlZCxhbW... demo.dxfeed.com:7300 Quote,Trade\n" +
                    "dxf_ipf_connect_sample profiles.zip demo.dxfeed.com:7300 Quote,Trade\n" +
                    "dxf_ipf_connect_sample profiles.zip Z2V0LmR4ZmVlZCxhbW... demo.dxfeed.com:7300 Quote,Trade\n"
                );
                return;
            }

            var path = args[0];
            var user = string.Empty;
            var password = string.Empty;
            var token = string.Empty;
            string dxFeedAddress;
            EventType events;
            List<string> symbols;

            try
            {
                var reader = new InstrumentProfileReader();
                IList<InstrumentProfile> profiles;
                var dxFeedAddressParamIndex = 1;

                if (IsFilePath(path))
                {
                    //Read profiles from local file system
                    using (var inputStream = new FileStream(path, FileMode.Open))
                    {
                        profiles = reader.Read(inputStream, path);
                    }
                }
                else
                {
                    if (args.Length >= 5)
                    {
                        user = args[1];
                        password = args[2];
                        dxFeedAddressParamIndex += 2;

                        if (args.Length == 6)
                        {
                            token = args[dxFeedAddressParamIndex];
                            dxFeedAddressParamIndex++;
                        }
                    }
                    else if (args.Length == 4)
                    {
                        token = args[dxFeedAddressParamIndex];
                        dxFeedAddressParamIndex++;
                    }

                    var urlContainsLoginAndPassword = path.Contains("@");

                    //Read profiles from server

                    if (urlContainsLoginAndPassword)
                        profiles = reader.ReadFromFile(path);
                    else
                        profiles = string.IsNullOrEmpty(token)
                            ? reader.ReadFromFile(path, user, password)
                            : reader.ReadFromFile(path, token);
                }

                dxFeedAddress = args[dxFeedAddressParamIndex];
                var eventsString = args[dxFeedAddressParamIndex + 1];

                if (!Enum.TryParse(eventsString, true, out events))
                {
                    Console.WriteLine($"Unsupported event type: {eventsString}");
                    return;
                }

                if (profiles.Count == 0)
                {
                    Console.WriteLine("There are no profiles");

                    return;
                }

                Console.WriteLine("Profiles from '{0}' count: {1}", path, profiles.Count);

                symbols = profiles.Select(profile => profile.GetSymbol()).ToList();

                Console.WriteLine($"Symbols: {string.Join(", ", symbols.Take(42).ToArray())}...");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exception occurred: {exc}");

                return;
            }

            Console.WriteLine(
                $"Connecting to {dxFeedAddress} for [{events} on [{string.Join(", ", symbols.Take(42).ToArray())}...] ...");

            NativeTools.InitializeLogging("dxf_ipf_connect_sample.log", true, true);

            using (var connection = string.IsNullOrEmpty(token)
                ? new NativeConnection(dxFeedAddress, DisconnectHandler, ConnectionStatusChangeHandler)
                : new NativeConnection(dxFeedAddress, token, DisconnectHandler, ConnectionStatusChangeHandler))
            {
                IDxSubscription subscription = null;
                try
                {
                    subscription = connection.CreateSubscription(events, new EventPrinter());

                    if (events == EventType.Candle)
                        subscription.AddSymbols(symbols.ConvertAll(CandleSymbol.ValueOf).ToArray());
                    else
                        subscription.AddSymbols(symbols.ToArray());

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                }
                catch (DxException dxException)
                {
                    Console.WriteLine($"Native exception occurred: {dxException.Message}");
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Exception occurred: {exc.Message}");
                }
                finally
                {
                    subscription?.Dispose();
                }
            }
        }
    }
}