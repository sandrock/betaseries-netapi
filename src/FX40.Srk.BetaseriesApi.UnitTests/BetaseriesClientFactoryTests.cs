using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Srk.BetaseriesApi;

namespace Srk.BetaseriesApi.UnitTests {

    [TestClass]
    public class BetaseriesClientFactoryTests {

        [TestMethod]
        public void Ctor_Success() {
            var apikey = "aaaaaaaa";
            var useragent = "hegjreghkjerhkgjher/1234";
            var sharesession = false;

            var target = new BetaseriesClientFactory(apikey, useragent, sharesession);
            var client = target.CreateDefaultClient();

            Assert.IsNotNull(client);
            Assert.AreEqual(apikey, client.Key);
            Assert.AreEqual(useragent, client.UserAgent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NullKey_Failure() {
            string apikey = null;
            var useragent = "hegjreghkjerhkgjher/1234";
            var sharesession = false;

            var target = new BetaseriesClientFactory(apikey, useragent, sharesession);
            var client = target.CreateDefaultClient();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NullAgent_Failure() {
            var apikey = "aaaaaaaa";
            string useragent = null;
            var sharesession = false;

            var target = new BetaseriesClientFactory(apikey, useragent, sharesession);
            var client = target.CreateDefaultClient();
        }

        [TestMethod]
        public void SessionSharingEnabled_ExistingClients() {
            var apikey = "aaaaaaaa";
            var useragent = "hegjreghkjerhkgjher/1234";
            var sharesession = true;
            string sessionToken = null;
            string sessionUsername = null;

            var target = new BetaseriesClientFactory(apikey, useragent, sharesession);
            var client0 = target.CreateDefaultClient();
            var client1 = target.CreateDefaultClient();
            
            Assert.AreEqual(sessionToken, client0.SessionToken);
            Assert.AreEqual(sessionToken, client1.SessionToken);
            Assert.AreEqual(sessionUsername, client0.SessionUsername);
            Assert.AreEqual(sessionUsername, client1.SessionUsername);
            
            sessionToken = "aaaaaaaaaaaaaa";
            sessionUsername = "aaaaaavaaaaaaa";
            client0.SetSessionTokens(sessionToken, sessionUsername);

            Assert.AreEqual(sessionToken, client0.SessionToken);
            Assert.AreEqual(sessionToken, client1.SessionToken);
            Assert.AreEqual(sessionUsername, client0.SessionUsername);
            Assert.AreEqual(sessionUsername, client1.SessionUsername);

            sessionToken = null;
            sessionUsername = null;
            client0.SetSessionTokens(sessionToken, sessionUsername);

            Assert.AreEqual(sessionToken, client0.SessionToken);
            Assert.AreEqual(sessionToken, client1.SessionToken);
            Assert.AreEqual(sessionUsername, client0.SessionUsername);
            Assert.AreEqual(sessionUsername, client1.SessionUsername);

            sessionToken = "aaaaaaaaaaaaaa";
            sessionUsername = "aaaaaavaaaaaaa";
            target.SetSessionToken(sessionToken, sessionUsername);

            Assert.AreEqual(sessionToken, client0.SessionToken);
            Assert.AreEqual(sessionToken, client1.SessionToken);
            Assert.AreEqual(sessionUsername, client0.SessionUsername);
            Assert.AreEqual(sessionUsername, client1.SessionUsername);

            sessionToken = null;
            sessionUsername = null;
            target.SetSessionToken(sessionToken, sessionUsername);

            Assert.AreEqual(sessionToken, client0.SessionToken);
            Assert.AreEqual(sessionToken, client1.SessionToken);
            Assert.AreEqual(sessionUsername, client0.SessionUsername);
            Assert.AreEqual(sessionUsername, client1.SessionUsername);
        }

        [TestMethod]
        public void SessionSharingEnabled_NewClients() {
            var apikey = "aaaaaaaa";
            var useragent = "hegjreghkjerhkgjher/1234";
            var sharesession = true;
            string sessionToken = null;
            string sessionUsername = null;

            var target = new BetaseriesClientFactory(apikey, useragent, sharesession);
            var client0 = target.CreateDefaultClient();
            
            sessionToken = "aaaaaaaaaaaaaa";
            sessionUsername = "aaaaaavaaaaaaa";

            client0.SetSessionTokens(sessionToken, sessionUsername);

            var client1 = target.CreateDefaultClient();

            Assert.AreEqual(sessionToken, client1.SessionToken);
            Assert.AreEqual(sessionUsername, client1.SessionUsername);

            sessionToken = null;
            sessionUsername = null;

            client0.SetSessionTokens(sessionToken, sessionUsername);

            var client2 = target.CreateDefaultClient();

            Assert.AreEqual(sessionToken, client2.SessionToken);
            Assert.AreEqual(sessionUsername, client2.SessionUsername);

            sessionToken = "aaaaaaaaaaaaaa";
            sessionUsername = "aaaaaavaaaaaaa";

            target.SetSessionToken(sessionToken, sessionUsername);

            var client3 = target.CreateDefaultClient();

            Assert.AreEqual(sessionToken, client3.SessionToken);
            Assert.AreEqual(sessionUsername, client3.SessionUsername);

            sessionToken = null;
            sessionUsername = null;

            target.SetSessionToken(sessionToken, sessionUsername);

            var client4 = target.CreateDefaultClient();

            Assert.AreEqual(sessionToken, client4.SessionToken);
            Assert.AreEqual(sessionUsername, client4.SessionUsername);
        }

        [TestMethod]
        public void SessionSharingDisabled_ExistingClients() {
            var apikey = "aaaaaaaa";
            var useragent = "hegjreghkjerhkgjher/1234";
            var sharesession = false;
            string sessionToken = null;
            string sessionUsername = null;

            var target = new BetaseriesClientFactory(apikey, useragent, sharesession);
            var client0 = target.CreateDefaultClient();
            var client1 = target.CreateDefaultClient();

            Assert.IsNull(sessionToken, client0.SessionToken);
            Assert.IsNull(sessionToken, client1.SessionToken);
            Assert.IsNull(sessionUsername, client0.SessionUsername);
            Assert.IsNull(sessionUsername, client1.SessionUsername);

            sessionToken = "aaaaaaaaaaaaaa";
            sessionUsername = "aaaaaavaaaaaaa";
            client0.SetSessionTokens(sessionToken, sessionUsername);

            Assert.AreEqual(sessionToken, client0.SessionToken);
            Assert.IsNull(client1.SessionToken);
            Assert.AreEqual(sessionUsername, client0.SessionUsername);
            Assert.IsNull(client1.SessionUsername);

            sessionToken = null;
            sessionUsername = null;
            client0.SetSessionTokens(sessionToken, sessionUsername);

            Assert.IsNull(client0.SessionToken);
            Assert.IsNull(client1.SessionToken);
            Assert.IsNull(client0.SessionUsername);
            Assert.IsNull(client1.SessionUsername);

            sessionToken = "aaaaaaaaaaaaaa";
            sessionUsername = "aaaaaavaaaaaaa";
            target.SetSessionToken(sessionToken, sessionUsername);

            Assert.IsNull(client0.SessionToken);
            Assert.IsNull(client1.SessionToken);
            Assert.IsNull(client0.SessionUsername);
            Assert.IsNull(client1.SessionUsername);

            sessionToken = null;
            sessionUsername = null;
            target.SetSessionToken(sessionToken, sessionUsername);

            Assert.IsNull(client0.SessionToken);
            Assert.IsNull(client1.SessionToken);
            Assert.IsNull(client0.SessionUsername);
            Assert.IsNull(client1.SessionUsername); 
        }

        [TestMethod]
        public void SessionSharingDisabled_NewClients() {
            var apikey = "aaaaaaaa";
            var useragent = "hegjreghkjerhkgjher/1234";
            var sharesession = false;
            string sessionToken = null;
            string sessionUsername = null;

            var target = new BetaseriesClientFactory(apikey, useragent, sharesession);
            var client0 = target.CreateDefaultClient();

            sessionToken = null;
            sessionUsername = null;

            client0.SetSessionTokens(sessionToken, sessionUsername);

            var client2 = target.CreateDefaultClient();

            Assert.AreEqual(sessionToken, client0.SessionToken);
            Assert.AreEqual(sessionUsername, client0.SessionUsername);
            Assert.IsNull(client2.SessionToken);
            Assert.IsNull(client2.SessionUsername);

            sessionToken = "aaaaaaaaaaaaaa";
            sessionUsername = "aaaaaavaaaaaaa";

            client0.SetSessionTokens(sessionToken, sessionUsername);

            var client1 = target.CreateDefaultClient();

            Assert.AreEqual(sessionToken, client0.SessionToken);
            Assert.AreEqual(sessionUsername, client0.SessionUsername);
            Assert.IsNull(client1.SessionToken);
            Assert.IsNull(client1.SessionUsername);

            sessionToken = "aaaa";
            sessionUsername = "rrrr";

            target.SetSessionToken(sessionToken, sessionUsername);

            var client3 = target.CreateDefaultClient();

            Assert.AreNotSame(sessionToken, client0.SessionToken);
            Assert.AreNotSame(sessionUsername, client0.SessionUsername);
            Assert.IsNull(client3.SessionToken);
            Assert.IsNull(client3.SessionUsername);

            sessionToken = null;
            sessionUsername = null;

            target.SetSessionToken(sessionToken, sessionUsername);

            var client4 = target.CreateDefaultClient();

            Assert.AreNotSame(sessionToken, client0.SessionToken);
            Assert.AreNotSame(sessionUsername, client0.SessionUsername);
            Assert.IsNull(client4.SessionToken);
            Assert.IsNull(client4.SessionUsername);
        }

    }
}
