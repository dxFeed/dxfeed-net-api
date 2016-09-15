using System;
using System.Collections.Generic;
using System.IO;
using com.dxfeed.ipf;

namespace dxf_instrument_profile_sample
{
    class Program
    {
        static bool IsFilePath(string path)
        {
            try
            {
                return new Uri(path).IsFile;
            }
            catch (UriFormatException)
            {
                //This exception used for determine that path is not valid uri.
                return true;
            }
        }

        static int MAX_PRINT_COUNT = 7;

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Usage: dxf_instrument_profile_sample <host> <user> <password>\n" +
                    "or:    dxf_instrument_profile_sample <file>\n" +
                    "where\n" +
                    "    host      - valid host to download instruments (https://tools.dxfeed.com/ipf)\n" +
                    "    user      - user name to host access\n" +
                    "    password  - user password to host access\n" +
                    "    file      - name of file or archive (.gz or .zip) contains instrument profiles\n" +
                    "example: dxf_instrument_profile_sample https://tools.dxfeed.com/ipf demo demo\n" +
                    "or:      dxf_instrument_profile_sample profiles.zip\n"
                );
                return;
            }

            string path = args[0];
            string user = string.Empty;
            string password = string.Empty;
            IList<InstrumentProfile> profiles = null;
            const string ZIP_FILE_PATH = "profiles.zip";
            try
            {
                InstrumentProfileReader reader = new InstrumentProfileReader();
                if (IsFilePath(path))
                {
                    //Read profiles from local file system
                    using (FileStream inputStream = new FileStream(path, FileMode.Open))
                    {
                        profiles = reader.Read(inputStream, path);
                    }
                }
                else
                {
                    if (args.Length >= 2)
                        user = args[1];
                    if (args.Length >= 3)
                        password = args[2];
                    //Read profiles from server
                    profiles = reader.ReadFromFile(path, user, password);
                }

                //Iterate through received profiles
                Console.WriteLine(string.Format("Profiles from '{0}' count: {1}", path, profiles.Count));
                Console.WriteLine(string.Format("Print first {0} instruments:", MAX_PRINT_COUNT));
                for (int i = 0; i < Math.Min(profiles.Count, MAX_PRINT_COUNT); i++)
                    Console.WriteLine(string.Format("#{0}:{1}", i, profiles[i].ToString()));
                if (profiles.Count > MAX_PRINT_COUNT)
                    Console.WriteLine(string.Format("   {0} instruments left...", profiles.Count - MAX_PRINT_COUNT));

                //Write profiles to local file system
                InstrumentProfileWriter writer = new InstrumentProfileWriter();
                writer.WriteToFile(ZIP_FILE_PATH, profiles);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception occured: " + exc.ToString());
            }
        }
    }
}
