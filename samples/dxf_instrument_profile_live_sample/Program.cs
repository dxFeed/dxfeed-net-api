#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using com.dxfeed.ipf;
using com.dxfeed.ipf.live;

namespace dxf_instrument_profile_live_sample {
    internal class UpdateListener : InstrumentProfileUpdateListener {
        public void InstrumentProfilesUpdated(ICollection<InstrumentProfile> instruments) {
            Console.WriteLine("Update received @{0}:", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            foreach (var ip in instruments) Console.WriteLine($"   {ip}");
        }
    }

    internal class Program {
        private static void OnErrorHandler(object sender, ErrorEventArgs e) {
            Console.WriteLine($"Error occured: {e.GetException().Message}");
        }

        private static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine(
                    "Usage: dxf_instrument_profile_live_sample <host:port>[update=<time-period>]\n" +
                    "where\n" +
                    "    host:port   - The valid host and port to download instruments (https://tools.dxfeed.com/ipf)\n" +
                    "    time-period - The update period in ISO8601 duration format (optional)\n\n" +
                    "examples: " +
                    "    dxf_instrument_profile_live_sample https://tools.dxfeed.com/ipf[update=P30S]\n" +
                    "    dxf_instrument_profile_live_sample https://user:password@tools.dxfeed.com/ipf[update=P30S]\n"
                );
                return;
            }

            var path = args[0];
            try {
                var connection = new InstrumentProfileConnection(path);
                connection.OnError += OnErrorHandler;
                var updateListener = new UpdateListener();
                connection.AddUpdateListener(updateListener);
                connection.Start();

                Console.WriteLine("Press enter to stop");
                Console.ReadLine();

                connection.Close();
            } catch (Exception exc) {
                Console.WriteLine($"Exception occured: {exc}");
            }
        }
    }
}