/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using com.dxfeed.ipf;

namespace com.dxfeed.api
{
    [TestFixture]
    public class InstrumentProfileTest
    {
        const string dxfToolsUser = "demo";
        const string dxfToolsPassword = "demo";
        const string dxfToolsHost = "https://tools.dxfeed.com/ipf";
        const string DATA_PATH = "src/data/instrument_profile_data";
        const string TXT_FILE_NAME = "profiles.txt";
        const string ZIP_FILE_NAME = "profiles.zip";
        const string GZ_FILE_NAME = "profiles.gz";
        const string MANY_ZIP_BY_DIR_FILE_NAME = "many_zip_by_dir.zip";
        const string MANY_PROFILES_FILE_NAME = "many_profiles.zip";
        const int IPF_COUNT = 25380;

        [Test]
        public void ReadFromHttpTest()
        {
            InstrumentProfileReader reader = new InstrumentProfileReader();
            IList<InstrumentProfile> profiles = reader.ReadFromFile(dxfToolsHost, dxfToolsUser, dxfToolsPassword);
            Assert.Greater(profiles.Count, 0);
        }

        private void ReadFromFileHelper(string filePath, int profilesCountExpected)
        {
            InstrumentProfileReader reader = new InstrumentProfileReader();
            using (FileStream inputStream = new FileStream(filePath, FileMode.Open))
            {
                IList<InstrumentProfile> profiles = reader.Read(inputStream, Path.GetFileName(filePath));
                Assert.AreEqual(profilesCountExpected, profiles.Count);
            }
        }

        [Test]
        public void ReadFromTxtTest()
        {
            string filePath = Path.Combine(DATA_PATH, TXT_FILE_NAME);
            ReadFromFileHelper(filePath, IPF_COUNT);
        }

        [Test]
        public void ReadFromZipTest()
        {
            string filePath = Path.Combine(DATA_PATH, ZIP_FILE_NAME);
            ReadFromFileHelper(filePath, IPF_COUNT);
        }

        [Test]
        public void ReadFromGzTest()
        {
            string filePath = Path.Combine(DATA_PATH, GZ_FILE_NAME);
            ReadFromFileHelper(filePath, IPF_COUNT);
        }

        [Test]
        public void ReadManyByDirFromZipTest()
        {
            string filePath = Path.Combine(DATA_PATH, MANY_ZIP_BY_DIR_FILE_NAME);
            ReadFromFileHelper(filePath, IPF_COUNT * 4);
        }

        [Test]
        public void ReadManyProfilesFromZipTest()
        {
            string filePath = Path.Combine(DATA_PATH, MANY_PROFILES_FILE_NAME);
            ReadFromFileHelper(filePath, IPF_COUNT * 4);
        }

        private void WriteToFileHelper(string filePath)
        {
            InstrumentProfileReader reader = new InstrumentProfileReader();
            InstrumentProfileWriter writer = new InstrumentProfileWriter();
            IList<InstrumentProfile> profilesFromHttp = reader.ReadFromFile(dxfToolsHost, dxfToolsUser, dxfToolsPassword);
            Assert.Greater(profilesFromHttp.Count, 0);
            writer.WriteToFile(filePath, profilesFromHttp);

            IList<InstrumentProfile> profilesFromFile;
            using (FileStream inputStream = new FileStream(filePath, FileMode.Open))
            {
                profilesFromFile = reader.Read(inputStream, filePath);
            }

            Assert.AreEqual(profilesFromHttp.Count, profilesFromFile.Count);
            /* NOTE: Next commented code may not performed if current instrument
               format was extended with new. */
            //for (int i = 0; i < profilesFromHttp.Count; i++) {
            //    Assert.AreEqual(profilesFromHttp[i], profilesFromFile[i]);
            //}
        }

        [Test]
        public void WriteToTxtTest()
        {
            WriteToFileHelper(TXT_FILE_NAME);
        }

        [Test]
        public void WriteToZipTest()
        {
            WriteToFileHelper(ZIP_FILE_NAME);
        }

        [Test]
        public void WriteToGzTest()
        {
            WriteToFileHelper(GZ_FILE_NAME);
        }
    }
}
