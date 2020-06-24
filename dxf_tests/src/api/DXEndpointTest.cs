#region License

/*
Copyright (c) 2010-2020 dxFeed Solutions DE GmbH

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using NUnit.Framework;
using System;

namespace com.dxfeed.api
{
    [TestFixture]
    public class DXEndpointTest
    {
        [Test]
        public void StateTest()
        {
            IDXEndpoint endpoint = DXEndpoint.Create();
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);

            endpoint.Disconnect();
            Assert.AreEqual(endpoint.State, DXEndpointState.NotConnected);

            endpoint.Connect(demoServerAddress);
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);

            endpoint.Close();
            Assert.AreEqual(endpoint.State, DXEndpointState.Closed);
            endpoint.Connect(demoServerAddress);
            Assert.AreEqual(endpoint.State, DXEndpointState.Closed);
        }

        [Test]
        public void UserTest()
        {
            IDXEndpoint endpoint = DXEndpoint.Create();
            Assert.Catch(typeof(ArgumentNullException), () => {
                endpoint.User(null);
            });
            Assert.Catch(typeof(ArgumentNullException), () => {
                endpoint.User(string.Empty);
            });
            Assert.AreEqual(endpoint, endpoint.User("test"));
        }

        [Test]
        public void PasswordTest()
        {
            IDXEndpoint endpoint = DXEndpoint.Create();
            Assert.Catch(typeof(ArgumentNullException), () => {
                endpoint.Password(null);
            });
            Assert.Catch(typeof(ArgumentNullException), () => {
                endpoint.Password(string.Empty);
            });
            Assert.AreEqual(endpoint, endpoint.Password("test"));
        }

        [Test]
        public void ConnectTest()
        {
            IDXEndpoint endpoint = DXEndpoint.Create();
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);

            Assert.Catch(typeof(ArgumentNullException), () => {
                endpoint.Connect(null);
            });
            Assert.Catch(typeof(ArgumentNullException), () => {
                endpoint.Connect(string.Empty);
            });
            Assert.Catch(typeof(ArgumentNullException), () => {
                endpoint.Connect("   ");
            });
            Assert.Catch(() => {
                endpoint.Connect("not-valid-address");
            });
            endpoint.Connect(testServerAddress);
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);
            endpoint.Connect(demoServerAddress);
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);

            //try to connect after disconnect
            endpoint.Disconnect();
            Assert.AreEqual(endpoint.State, DXEndpointState.NotConnected);
            endpoint.Connect(demoServerAddress);
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);
        }

        [Test]
        public void DisconnectTest()
        {
            IDXEndpoint endpoint = DXEndpoint.Create();
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);

            endpoint.Disconnect();
            Assert.AreEqual(endpoint.State, DXEndpointState.NotConnected);

            endpoint.Connect(demoServerAddress);
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);

            endpoint.Disconnect();
            Assert.AreEqual(endpoint.State, DXEndpointState.NotConnected);

            endpoint.Close();
            Assert.AreEqual(endpoint.State, DXEndpointState.Closed);

            endpoint.Disconnect();
            Assert.AreEqual(endpoint.State, DXEndpointState.Closed);
        }

        [Test]
        public void CloseTest()
        {
            isFiresOnClosing = false;
            IDXEndpoint endpoint = DXEndpoint.Create();
            endpoint.OnClosing += Endpoint_OnClosing;
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);

            endpoint.Close();
            Assert.AreEqual(endpoint.State, DXEndpointState.Closed);
            Assert.True(isFiresOnClosing);

            endpoint.Connect(demoServerAddress);
            Assert.AreEqual(endpoint.State, DXEndpointState.Closed);

            endpoint.Disconnect();
            Assert.AreEqual(endpoint.State, DXEndpointState.Closed);
        }

        [Test]
        public void FeedTest()
        {
            IDXEndpoint endpoint = DXEndpoint.Create();
            Assert.True(endpoint.State == DXEndpointState.Connected || endpoint.State == DXEndpointState.Connecting);

            Assert.IsNotNull(endpoint.Feed);
        }

        #region Private fields and methods

        private static readonly string demoServerAddress = "demo.dxfeed.com:7300";
        private static readonly string testServerAddress = "mddqa.in.devexperts.com:7400";
        private bool isFiresOnClosing = false;

        private void Endpoint_OnClosing(object sender, EventArgs e)
        {
            isFiresOnClosing = true;
        }

        #endregion
    }
}
