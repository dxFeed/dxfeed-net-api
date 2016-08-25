using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.dxfeed.ipf;
using com.dxfeed.ipf.live;

namespace dxf_instrument_profile_live_sample {

    class UpdateListener : InstrumentProfileUpdateListener {
        public void InstrumentProfilesUpdated(ICollection<InstrumentProfile> instruments) {
            Console.WriteLine(string.Format("Update received @{0}:", DateTime.Now.ToString()));
            foreach (InstrumentProfile ip in instruments) {
                Console.WriteLine("   " + ip);
            }
        }
    }

    class Program {
        static void Main(string[] args) {

            if (args.Length == 0) {
                //TODO: update time period if needed
                Console.WriteLine(
                    "Usage: dxf_instrument_profile_live_sample <host:port>[update=<time-period>]\n" +
                    "where\n" +
                    "    host:port   - valid host and port to download instruments (https://tools.dxfeed.com/ipf)\n" +
                    "    time-period - update period in ISO8601 duration format (optional)\n" +
                    "example: dxf_instrument_profile_live_sample https://tools.dxfeed.com/ipf[update=P30S]\n"
                );
                return;
            }

            string path = args[0];
            try {
                InstrumentProfileConnection connection = new InstrumentProfileConnection(path);
                UpdateListener updateListener = new UpdateListener();
                connection.AddUpdateListener(updateListener);
                connection.Start();

                Console.WriteLine("Press enter to stop");
                Console.ReadLine();

            } catch (Exception exc) {
                Console.WriteLine("Exception occured: " + exc.ToString());
            }

        }
    }
}
