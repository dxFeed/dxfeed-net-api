/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.ipf;
using com.dxfeed.ipf.live;
using System;
using System.Collections.Generic;

namespace dxf_instrument_profile_live_sample
{
    class UpdateListener : InstrumentProfileUpdateListener
    {
        public void InstrumentProfilesUpdated(ICollection<InstrumentProfile> instruments)
        {
            Console.WriteLine(string.Format("Update received @{0}:", DateTime.Now.ToString()));
            foreach (InstrumentProfile ip in instruments)
            {
                Console.WriteLine("   " + ip);
            }
        }
    }

    class Program
    {
        static void OnErrorHandler(object sender, System.IO.ErrorEventArgs e)
        {
            Console.WriteLine("Error occured: " + e.GetException().Message);
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
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
            try
            {
                InstrumentProfileConnection connection = new InstrumentProfileConnection(path);
                connection.OnError += OnErrorHandler;
                UpdateListener updateListener = new UpdateListener();
                connection.AddUpdateListener(updateListener);
                connection.Start();

                Console.WriteLine("Press enter to stop");
                Console.ReadLine();

                connection.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception occured: " + exc.ToString());
            }
        }
    }
}
