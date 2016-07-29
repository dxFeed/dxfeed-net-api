using System;
using System.Collections.Generic;
using System.IO;
using com.dxfeed.ipf;

namespace dxf_instrument_profile_sample {
    class Program {
        static void Main(string[] args) {

            const string dxfToolsUser = "demo";
            const string dxfToolsPassword = "demo";
            const string dxfToolsHost = "https://tools.dxfeed.com/ipf";
            const string TXT_FILE_PATH = "profiles.txt";
            const string ZIP_FILE_PATH = "profiles.zip";
            const string GZ_FILE_PATH = "profiles.gz";

            InstrumentProfileReader reader = new InstrumentProfileReader();
            InstrumentProfileWriter writer = new InstrumentProfileWriter();
            try {
                IList<InstrumentProfile> profiles = reader.ReadFromFile(dxfToolsHost, dxfToolsUser, dxfToolsPassword);
                Console.WriteLine("Profiles count: " + profiles.Count);
                writer.WriteToFile(TXT_FILE_PATH, profiles);
                writer.WriteToFile(ZIP_FILE_PATH, profiles);
                writer.WriteToFile(GZ_FILE_PATH, profiles);

                using (FileStream inputStream = new FileStream(TXT_FILE_PATH, FileMode.Open)) {
                    IList<InstrumentProfile> txtProfiles = reader.Read(inputStream, TXT_FILE_PATH);
                    Console.WriteLine("Profiles from TXT count: " + txtProfiles.Count);
                }
                using (FileStream inputStream = new FileStream(ZIP_FILE_PATH, FileMode.Open)) {
                    IList<InstrumentProfile> txtProfiles = reader.Read(inputStream, ZIP_FILE_PATH);
                    Console.WriteLine("Profiles from ZIP count: " + txtProfiles.Count);
                }
                using (FileStream inputStream = new FileStream(GZ_FILE_PATH, FileMode.Open)) {
                    IList<InstrumentProfile> txtProfiles = reader.Read(inputStream, GZ_FILE_PATH);
                    Console.WriteLine("Profiles from GZIP count: " + txtProfiles.Count);
                }
            } catch (Exception exc) {
                Console.WriteLine("Exception occured: " + exc.ToString());
            }

        }
    }
}
