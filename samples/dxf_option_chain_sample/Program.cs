using System;
using System.Collections.Generic;
using com.dxfeed.ipf;
using com.dxfeed.ipf.option;

namespace dxf_option_chain_sample {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 6)
            {
                Console.WriteLine(
                    "Usage: dxf_option_chain_sample <host> <user> <password> <symbol> <nStrikes> <nMonths>\n" +
                    "where\n" +
                    "    host     - valid host to download instruments (https://tools.dxfeed.com/ipf)\n" +
                    "    user     - user name to host access\n" +
                    "    password - user password to host access\n" +
                    "    symbol   - is the product or underlying symbol" +
                    "    nStrikes - number of strikes to print for each series" +
                    "    nMonths  - number of months to print" +
                    "example: dxf_option_chain_sample https://tools.dxfeed.com/ipf demo demo\n"
                );
                return;
            }

            string path = args[0];
            string user = args[1];
            string password = args[2];
            IList<InstrumentProfile> profiles = null;
            IDictionary<string, OptionChain<InstrumentProfile>> chains = null;

            try {
                InstrumentProfileReader reader = new InstrumentProfileReader();
                //Read profiles from server
                profiles = reader.ReadFromFile(path, user, password);

                //Iterate through received profiles
                Console.WriteLine(string.Format("Profiles from '{0}' count: {1}", path, profiles.Count));

            } catch (Exception exc) {
                Console.WriteLine("Exception occured: " + exc.ToString());
            }

        }
    }
}
