#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

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

namespace dxf_instrument_profile_live_sample
{
    internal class UpdateListener : InstrumentProfileUpdateListener
    {
        public void InstrumentProfilesUpdated(ICollection<InstrumentProfile> instruments)
        {
            Console.WriteLine("Update received @{0}:", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            foreach (var ip in instruments) Console.WriteLine($"   {ip}");
        }
    }

    internal class Program
    {
        private static void OnErrorHandler(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"Error occurred: {e.GetException().Message}");
        }

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Usage: " +
                    "    dxf_instrument_profile_live_sample <ipf-url>[update=<time-period>] [<token>]\n" +
                    "where\n" +
                    "    ipf-url     - The valid url to download instruments (https://tools.dxfeed.com/ipf)\n" +
                    "    time-period - The update period in ISO8601 duration format (optional, non-live connection)\n" +
                    "    token       - The bearer token\n" +
                    "\n" +
                    "Non-live connection (periodical GET requests) examples: " +
                    "    dxf_instrument_profile_live_sample https://tools.dxfeed.com/ipf[update=P30S]\n" +
                    "    dxf_instrument_profile_live_sample https://tools.dxfeed.com/ipf[update=P30S] Z2V0LmR4ZmVlZCxhbW...\n" +
                    "    dxf_instrument_profile_live_sample https://user:password@tools.dxfeed.com/ipf[update=P30S]\n" +
                    "Live connection examples (with 'live=true' query parameter): " +
                    "    dxf_instrument_profile_live_sample https://tools.dxfeed.com/ipf?TYPE=STOCK\n" +
                    "    dxf_instrument_profile_live_sample https://tools.dxfeed.com/ipf?TYPE=OPTION&UNDERLYING=AAPL,IBM Z2V0LmR4ZmVlZCxhbW...\n" +
                    "    dxf_instrument_profile_live_sample https://user:password@tools.dxfeed.com/ipf\n\n"
                );
                return;
            }

            var path = args[0];
            string token = null;

            if (args.Length > 1) token = args[1];

            try
            {
                var connection = string.IsNullOrEmpty(token)
                    ? new InstrumentProfileConnection(path)
                    : new InstrumentProfileConnection(path, token);
                connection.OnError += OnErrorHandler;
                var updateListener = new UpdateListener();
                connection.AddUpdateListener(updateListener);
                connection.Start();

                Console.WriteLine("Press enter to stop");
                Console.ReadLine();

                connection.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exception occurred: {exc}");
            }
        }
    }
}