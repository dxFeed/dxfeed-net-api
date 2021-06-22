#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using com.dxfeed.ipf;
using com.dxfeed.ipf.option;

namespace dxf_option_chain_sample {
    internal class Program {
        private static void Main(string[] args) {
            if (args.Length < 7) {
                Console.WriteLine(
                    "Usage: dxf_option_chain_sample <host> <user> <password> <symbol> <nStrikes> <nMonths> <price>\n" +
                    "where\n" +
                    "    host     - valid host to download instruments (https://tools.dxfeed.com/ipf)\n" +
                    "    user     - user name to host access\n" +
                    "    password - user password to host access\n" +
                    "    symbol   - is the product or underlying symbol (GOOG, AAPL, IBM etc)" +
                    "    nStrikes - number of strikes to print for each series" +
                    "    nMonths  - number of months to print" +
                    "    price    - price of the last trade" +
                    "example: dxf_option_chain_sample https://tools.dxfeed.com/ipf demo demo AAPL 500 5 90.5\n"
                );
                return;
            }

            var path = args[0];
            var user = args[1];
            var password = args[2];
            var symbol = args[3];

            try {
                var nStrikes = int.Parse(args[4]);
                var nMonths = int.Parse(args[5]);
                var price = double.Parse(args[6], CultureInfo.InvariantCulture);

                var reader = new InstrumentProfileReader();
                //Read profiles from server
                var profiles = reader.ReadFromFile(path, user, password);

                Console.WriteLine("Profiles from '{0}' count: {1}", path, profiles.Count);

                Console.WriteLine("Building option chains ...");
                IDictionary<string, OptionChain> chains = OptionChainsBuilder.Build(profiles).Chains;
                var chain = chains[symbol];
                nMonths = Math.Min(nMonths, chain.GetSeries().Count);
                var seriesList
                    = new List<OptionSeries>(chain.GetSeries()).GetRange(0, nMonths);

                Console.WriteLine("Printing option series ...");
                foreach (var series in seriesList) {
                    Console.WriteLine("Option series {0}", series);
                    var strikes = series.GetNStrikesAround(nStrikes, price);
                    Console.WriteLine("Strikes:");
                    foreach (var strike in strikes) Console.Write("{0} ", strike);
                    Console.WriteLine();
                }
            } catch (Exception exc) {
                Console.WriteLine($"Exception occurred: {exc}");
            }
        }
    }
}