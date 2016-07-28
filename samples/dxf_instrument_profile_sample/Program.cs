using System;
using System.Collections.Generic;
using com.dxfeed.ipf;

namespace dxf_instrument_profile_sample {
    class Program {
        static void Main(string[] args) {

            const string dxfToolsUser = "demo";
            const string dxfToolsPassword = "demo";
            const string dxfToolsHost = "https://tools.dxfeed.com/ipf";
            InstrumentProfileReader reader = new InstrumentProfileReader();
            try {
                IList<InstrumentProfile> profiles = reader.ReadFromFile(dxfToolsHost, dxfToolsUser, dxfToolsPassword);
                Console.WriteLine("Profiles count: " + profiles.Count);
            } catch (Exception exc) {
                Console.WriteLine("Exception occured: " + exc.ToString());
            }

        }
    }
}
