using System;
using System.Collections.Generic;
using com.dxfeed.ipf;
using com.dxfeed.ipf.option;
using System.Globalization;

namespace dxf_option_chain_sample {
    class Program {
        static void Main(string[] args) {
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

            IList<InstrumentProfile> profiles = null;
            IDictionary<string, OptionChain> chains = null;

            string path = args[0];
            string user = args[1];
            string password = args[2];
            string symbol = args[3];

            try {
                int nStrikes = int.Parse(args[4]);
                int nMonths = int.Parse(args[5]);
                double price = double.Parse(args[6], CultureInfo.InvariantCulture);

                InstrumentProfileReader reader = new InstrumentProfileReader();
                //Read profiles from server
                profiles = reader.ReadFromFile(path, user, password);

                Console.WriteLine(string.Format("Profiles from '{0}' count: {1}", path, profiles.Count));

                Console.WriteLine("Building option chains ...");
                chains = OptionChainsBuilder.Build(profiles).Chains;
                OptionChain chain = chains[symbol];
                nMonths = Math.Min(nMonths, chain.GetSeries().Count);
                List<OptionSeries> seriesList 
                    = new List<OptionSeries>(chain.GetSeries()).GetRange(0, nMonths);


                Console.WriteLine("Printing option series ...");
                foreach (OptionSeries series in seriesList) {
                    Console.WriteLine("Option series {0}", series);
                    List<double> strikes = series.GetNStrikesAround(nStrikes, price);
                    Console.WriteLine("Strikes:");
                    foreach (double strike in strikes) {
                        Console.Write("{0} ", strike);
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception exc) {
                Console.WriteLine("Exception occured: " + exc.ToString());
            }
        }
    }
}
