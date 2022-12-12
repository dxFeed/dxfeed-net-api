#region License

/*
Copyright (c) 2010-2022 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using com.dxfeed.ipf;
using com.dxfeed.ipf.live;

namespace com.dxfeed.api
{
    [TestFixture]
    class InstrumentProfileLiveTest
    {
        const string DATA_PATH = "src\\data\\instrument_profile_data";
        const string ZIP_FILE_NAME = "profiles.zip";
        const string TEST_FILE_NAME = "instrument_profile_live_data.zip";
        //default update period id 3 seconds
        const string UPDATE_PERIOD_STR = "[update=P3S]";
        const int IPF_COUNT = 25380;

        class UpdateListener : InstrumentProfileUpdateListener
        {
            List<InstrumentProfile> buffer = new List<InstrumentProfile>();
            bool isUpdatedNonSync = false;
            object isUpdatedLocker = new object();

            public UpdateListener()
            {
                IsUpdated = false;
            }

            public void InstrumentProfilesUpdated(ICollection<InstrumentProfile> instruments)
            {
                if (IsUpdated)
                    return;
                buffer.AddRange(instruments);
                IsUpdated = true;
            }

            public ICollection<InstrumentProfile> LastUpdate
            {
                get
                {
                    return buffer;
                }
            }

            public bool IsUpdated
            {
                get
                {
                    bool value = false;
                    lock (isUpdatedLocker)
                    {
                        value = isUpdatedNonSync;
                    }
                    return value;
                }
                private set
                {
                    lock (isUpdatedLocker)
                    {
                        isUpdatedNonSync = value;
                    }
                }
            }

            public void DropState()
            {
                buffer.Clear();
                IsUpdated = false;
            }
        }

        [Test]
        public void UpdateChangeTest()
        {
            const int UPDATE_CHANGE_1_COUNT = 1;
            const int UPDATE_CHANGE_2_COUNT = 3;
            const string UPDATE_CHANGE_1_FILE_NAME = "update_change_field_profiles.zip";
            const string UPDATE_CHANGE_2_FILE_NAME = "update_change_ins_profiles.zip";
            const string CUSTOM_FIELD_ONE_NAME = "CUSTOM_ONE";
            const string CUSTOM_FIELD_TWO_NAME = "CUSTOM_TWO";
            const string CUSTOM_FIELD_ONE_VALUE = "filled_custom_one";
            const string CUSTOM_FIELD_TWO_VALUE = "filled_custom_two";
            const string UPDATED_FIELD_NAME = "DESCRIPTION";
            const string UPDATED_FIELD_VALUE = "Updated Description";
            const string PRODUCT_SYMBOL = "/6E";
            string sourceFile = Path.GetFullPath(Path.Combine(DATA_PATH, ZIP_FILE_NAME));
            string updateFile = Path.GetFullPath(Path.Combine(DATA_PATH, UPDATE_CHANGE_1_FILE_NAME));
            string targetFile = Path.GetFullPath(Path.Combine(DATA_PATH, TEST_FILE_NAME));
            Assert.True(File.Exists(sourceFile));
            Assert.True(File.Exists(updateFile));
            Uri uri = new Uri(targetFile);
            File.Copy(sourceFile, targetFile, true);
            InstrumentProfileConnection connection = new InstrumentProfileConnection(uri.AbsoluteUri + UPDATE_PERIOD_STR);
            UpdateListener updateListener = new UpdateListener();
            connection.AddUpdateListener(updateListener);
            connection.Start();

            while (!updateListener.IsUpdated) { }
            Assert.AreEqual(IPF_COUNT, updateListener.LastUpdate.Count);

            //update#1 - updated one of field
            updateListener.DropState();
            File.Copy(updateFile, targetFile, true);
            File.SetLastWriteTime(targetFile, DateTime.Now);
            while (!updateListener.IsUpdated) { }
            Assert.AreEqual(UPDATE_CHANGE_1_COUNT, updateListener.LastUpdate.Count);
            foreach (InstrumentProfile ip in updateListener.LastUpdate)
            {
                if (ip.GetTypeName().Equals(InstrumentProfileType.PRODUCT.Name) && ip.GetSymbol().Equals(PRODUCT_SYMBOL))
                {
                    Assert.AreEqual(ip.GetField(UPDATED_FIELD_NAME), UPDATED_FIELD_VALUE);
                    continue;
                }
            }

            //update #2 - new fields inserted
            updateFile = Path.GetFullPath(Path.Combine(DATA_PATH, UPDATE_CHANGE_2_FILE_NAME));
            updateListener.DropState();
            File.Copy(updateFile, targetFile, true);
            File.SetLastWriteTime(targetFile, DateTime.Now);
            while (!updateListener.IsUpdated) { }
            Assert.AreEqual(UPDATE_CHANGE_2_COUNT, updateListener.LastUpdate.Count);
            foreach (InstrumentProfile ip in updateListener.LastUpdate)
            {
                if (ip.GetTypeName().Equals(InstrumentProfileType.PRODUCT.Name) && ip.GetSymbol().Equals(PRODUCT_SYMBOL))
                {
                    Assert.AreEqual(ip.GetField(CUSTOM_FIELD_ONE_NAME), CUSTOM_FIELD_ONE_VALUE);
                    Assert.AreEqual(ip.GetField(CUSTOM_FIELD_TWO_NAME), CUSTOM_FIELD_TWO_VALUE);
                    continue;
                }
            }

            connection.Close();
        }

        [Test]
        public void UpdateAddTest()
        {
            const int UPDATE_ADD_COUNT = 2;
            const string UPDATE_ADD_FILE_NAME = "update_add_profiles.zip";
            const string PRODUCT_SYMBOL_1 = "/EX";
            const string PRODUCT_SYMBOL_2 = "/EW";
            string sourceFile = Path.GetFullPath(Path.Combine(DATA_PATH, ZIP_FILE_NAME));
            string updateFile = Path.GetFullPath(Path.Combine(DATA_PATH, UPDATE_ADD_FILE_NAME));
            string targetFile = Path.GetFullPath(Path.Combine(DATA_PATH, TEST_FILE_NAME));
            Assert.True(File.Exists(sourceFile));
            Assert.True(File.Exists(updateFile));
            Uri uri = new Uri(targetFile);
            File.Copy(sourceFile, targetFile, true);
            InstrumentProfileConnection connection = new InstrumentProfileConnection(uri.AbsoluteUri + UPDATE_PERIOD_STR);
            UpdateListener updateListener = new UpdateListener();
            connection.AddUpdateListener(updateListener);
            connection.Start();

            while (!updateListener.IsUpdated) { }
            Assert.AreEqual(IPF_COUNT, updateListener.LastUpdate.Count);

            updateListener.DropState();
            File.Copy(updateFile, targetFile, true);
            File.SetLastWriteTime(targetFile, DateTime.Now);
            while (!updateListener.IsUpdated) { }
            Assert.AreEqual(UPDATE_ADD_COUNT, updateListener.LastUpdate.Count);
            foreach (InstrumentProfile ip in updateListener.LastUpdate)
            {
                if (ip.GetTypeName().Equals(InstrumentProfileType.PRODUCT.Name) &&
                    (ip.GetSymbol().Equals(PRODUCT_SYMBOL_1) || ip.GetSymbol().Equals(PRODUCT_SYMBOL_2)))
                {

                    continue;
                }
                Assert.Fail("Unexpected instrument profiles here!");
            }

            connection.Close();
        }

        [Test]
        public void UpdateRemoveTest()
        {
            const int UPDATE_ADD_COUNT = 2;
            const string UPDATE_REMOVE_FILE_NAME = "update_remove_profiles.zip";
            const string PRODUCT_SYMBOL_1 = "/CL";
            const string PRODUCT_SYMBOL_2 = "/TF";
            string sourceFile = Path.GetFullPath(Path.Combine(DATA_PATH, ZIP_FILE_NAME));
            string updateFile = Path.GetFullPath(Path.Combine(DATA_PATH, UPDATE_REMOVE_FILE_NAME));
            string targetFile = Path.GetFullPath(Path.Combine(DATA_PATH, TEST_FILE_NAME));
            Assert.True(File.Exists(sourceFile));
            Assert.True(File.Exists(updateFile));
            Uri uri = new Uri(targetFile);
            File.Copy(sourceFile, targetFile, true);
            InstrumentProfileConnection connection = new InstrumentProfileConnection(uri.AbsoluteUri + UPDATE_PERIOD_STR);
            UpdateListener updateListener = new UpdateListener();
            connection.AddUpdateListener(updateListener);
            connection.Start();

            while (!updateListener.IsUpdated) { }
            Assert.AreEqual(IPF_COUNT, updateListener.LastUpdate.Count);

            updateListener.DropState();
            File.Copy(updateFile, targetFile, true);
            File.SetLastWriteTime(targetFile, DateTime.Now);
            while (!updateListener.IsUpdated) { }
            Assert.AreEqual(UPDATE_ADD_COUNT, updateListener.LastUpdate.Count);
            foreach (InstrumentProfile ip in updateListener.LastUpdate)
            {
                if (ip.GetTypeName().Equals(InstrumentProfileType.REMOVED.Name) &&
                    (ip.GetSymbol().Equals(PRODUCT_SYMBOL_1) || ip.GetSymbol().Equals(PRODUCT_SYMBOL_2)))
                {
                    continue;
                }
                Assert.Fail("Unexpected instrument profiles here!");
            }

            connection.Close();
        }

        [Test]
        public void SetPeriodTest()
        {
            const int TEST_TIMES = 4;
            const long PERIOD_NEW = 5000;
            const double PERIOD_DELTA = 1800;
            const string UPDATE_CHANGE_FILE_NAME = "update_change_field_profiles.zip";
            string sourceFile = Path.GetFullPath(Path.Combine(DATA_PATH, ZIP_FILE_NAME));
            string updateFile = Path.GetFullPath(Path.Combine(DATA_PATH, UPDATE_CHANGE_FILE_NAME));
            string targetFile = Path.GetFullPath(Path.Combine(DATA_PATH, TEST_FILE_NAME));
            Assert.True(File.Exists(sourceFile));
            Assert.True(File.Exists(updateFile));
            Uri uri = new Uri(targetFile);
            File.Copy(sourceFile, targetFile, true);
            InstrumentProfileConnection connection = new InstrumentProfileConnection(uri.AbsoluteUri + UPDATE_PERIOD_STR);
            UpdateListener updateListener = new UpdateListener();
            connection.AddUpdateListener(updateListener);
            connection.Start();

            DateTime time = DateTime.Now;
            while (!updateListener.IsUpdated) { }
            for (int i = 0; i < TEST_TIMES * 2; i++)
            {
                if (i == TEST_TIMES)
                {
                    connection.UpdatePeriod = PERIOD_NEW;
                }
                updateListener.DropState();
                File.Copy(updateFile, targetFile, true);
                File.SetLastWriteTime(targetFile, DateTime.Now);
                while (!updateListener.IsUpdated) { }
                Assert.AreEqual(connection.UpdatePeriod, DateTime.Now.Subtract(time).TotalMilliseconds, PERIOD_DELTA);
                time = DateTime.Now;
                //swap source/update files
                string temp = updateFile;
                updateFile = sourceFile;
                sourceFile = temp;
            }

            connection.Close();
        }

        [Test]
        public void AddListenerTest()
        {
            const string UPDATE_CHANGE_FILE_NAME = "update_change_field_profiles.zip";
            string sourceFile = Path.GetFullPath(Path.Combine(DATA_PATH, ZIP_FILE_NAME));
            string updateFile = Path.GetFullPath(Path.Combine(DATA_PATH, UPDATE_CHANGE_FILE_NAME));
            string targetFile = Path.GetFullPath(Path.Combine(DATA_PATH, TEST_FILE_NAME));
            Assert.True(File.Exists(sourceFile));
            Assert.True(File.Exists(updateFile));
            Uri uri = new Uri(targetFile);
            File.Copy(sourceFile, targetFile, true);
            InstrumentProfileConnection connection = new InstrumentProfileConnection(uri.AbsoluteUri + UPDATE_PERIOD_STR);
            UpdateListener updateListener = new UpdateListener();
            connection.AddUpdateListener(updateListener);
            connection.Start();

            while (!updateListener.IsUpdated) { }
            Assert.AreEqual(IPF_COUNT, updateListener.LastUpdate.Count);

            updateListener.DropState();
            File.Copy(updateFile, targetFile, true);
            File.SetLastWriteTime(targetFile, DateTime.Now);
            while (!updateListener.IsUpdated) { }

            UpdateListener newListener = new UpdateListener();
            connection.AddUpdateListener(newListener);
            while (!newListener.IsUpdated) { }
            Assert.AreEqual(IPF_COUNT, newListener.LastUpdate.Count);

            connection.Close();
        }
    }
}
