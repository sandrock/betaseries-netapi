using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Srk.BetaseriesApi;
using Srk.BetaseriesApi.Clients;

namespace Srk.BetaseriesApi.IntegrationTests
{
    
    
    /// <summary>
    /// This is a test class for BetaseriesXmlClientTest and is intended
    /// to contain all BetaseriesXmlClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BetaseriesXmlClientTest {

        #region Additional test attributes

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }
        
        [TestInitialize()]
        public void MyTestInitialize()
        {
            if (BetaseriesClientFactory.Default == null)
                BetaseriesClientFactory.Default = new BetaseriesClientFactory(TestApp.ApiKey, TestApp.ApiUserAgent, false);
        }

        private string TestUsername = "Dev012";
        private string TestPassword = "developer";

        private BetaseriesXmlClient GetClient() {
            return BetaseriesClientFactory.Default.CreateClient<BetaseriesXmlClient>() as BetaseriesXmlClient;
        }
        
        #endregion

        /// <summary>
        /// A test for Authenticate
        /// </summary>
        [TestMethod()]
        public void AuthenticateTest() {
            BetaseriesXmlClient target = GetClient();
            string username = TestUsername;
            string password = TestPassword;

            string sessiontoken = target.Authenticate(username, password);

            Assert.IsFalse(string.IsNullOrEmpty(sessiontoken), "a session token must be provided");
            Assert.AreEqual(sessiontoken, target.SessionToken, "the session token must be in the client object");
            Assert.IsNotNull(target.SessionToken, "the session token must be in the client object");
            Assert.AreEqual(username, target.SessionUsername, "the session username must be in the client object");

            SilentLogoff(target);
            target.Dispose();
        }

        private static void SilentLogoff(BetaseriesXmlClient target) {
            try {
                target.Logoff();
            } catch { }
        }

        /// <summary>
        /// A test for GetEpisodes
        /// </summary>
        [TestMethod()]
        public void GetEpisodesTest() {
            BetaseriesXmlClient target = GetClient();
            string showUrl = "dexter";

            IList<Episode> result = target.GetEpisodes(showUrl);
            target.Dispose();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            foreach (var item in result) {
                Assert.IsFalse(string.IsNullOrEmpty(item.Season));
                Assert.IsFalse(string.IsNullOrEmpty(item.Number));
            }
        }

        /////// <summary>
        /////// A test for GetFriendsTimeline
        /////// </summary>
        ////[TestMethod()]
        ////public void GetFriendsTimelineTest() {
        ////    Assert.Inconclusive("Verify the correctness of this test method.");
        ////}

        /// <summary>
        /// A test for GetIsSessionActive
        /// </summary>
        [TestMethod()]
        public void GetIsSessionActiveTest() {
            BetaseriesXmlClient target = GetClient();
            try {
                target.Authenticate(TestUsername, TestPassword);
            } catch {
                target.Dispose();
                Assert.Inconclusive();
            }

            bool expected = true;
            var result = target.GetIsSessionActive();

            Assert.AreEqual(result, expected);

            SilentLogoff(target);
            target.Dispose();
        }

        /// <summary>
        /// A test for GetMember
        /// </summary>
        [TestMethod()]
        public void GetMemberTest() {
            BetaseriesXmlClient target = GetClient();

            var result = target.GetMember(TestUsername);
            target.Dispose();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Username));

        }

        /////// <summary>
        /////// A test for GetMembersNextEpisodes
        /////// </summary>
        ////[TestMethod()]
        ////public void GetMembersNextEpisodesTest() {
        ////    Assert.Inconclusive("Verify the correctness of this test method.");
        ////}
        ////
        /////// <summary>
        /////// A test for GetNotifications
        /////// </summary>
        ////[TestMethod()]
        ////public void GetNotificationsTest() {
        ////    Assert.Inconclusive("Verify the correctness of this test method.");
        ////}

        /// <summary>
        /// A test for GetShow
        /// </summary>
        [TestMethod()]
        public void GetShowTest() {
            BetaseriesXmlClient target = GetClient();
            string url = "dexter";

            var result = target.GetShow(url);
            target.Dispose();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Title));

        }

        /// <summary>
        /// A test for GetStatus
        /// </summary>
        [TestMethod()]
        public void GetStatusTest() {
            BetaseriesXmlClient target = GetClient();

            var result = target.GetStatus();
            target.Dispose();

            Assert.IsNotNull(result);

            Assert.IsFalse(string.IsNullOrEmpty(result.WebsiteStatus));
            Assert.IsFalse(string.IsNullOrEmpty(result.WebsiteDatabase));
            Assert.IsFalse(string.IsNullOrEmpty(result.ApiVersion));

            Assert.IsNotNull(result.Changes);
            Assert.IsTrue(result.Changes.Length > 0);

            // obsolete
            ////Assert.IsNotNull(result.Files);
            ////Assert.IsTrue(result.Files.Length > 0);
        }

        /// <summary>
        /// A test for Logoff
        /// </summary>
        [TestMethod()]
        public void LogoffTest() {
            BetaseriesXmlClient target = GetClient();
            try {
                target.Authenticate(TestUsername, TestPassword);
            } catch {
                target.Dispose();
                Assert.Inconclusive();
            }

            target.Logoff();

            Assert.IsNull(target.SessionToken, "the session token must be in the client object");
            Assert.IsNull(target.SessionUsername, "the session username must be in the client object");

            target.Dispose();
        }

        /// <summary>
        /// A test for SearchShows
        /// </summary>
        [TestMethod()]
        public void SearchShowsTest() {
            BetaseriesXmlClient target = GetClient();
            string title = "dex";

            var result = target.SearchShows(title);
            target.Dispose();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            foreach (var item in result) {
                Assert.IsFalse(string.IsNullOrEmpty(item.Url));
                Assert.IsFalse(string.IsNullOrEmpty(item.Title));
            }
        }

        /////// <summary>
        /////// A test for SetEpisodeAsDownloaded
        /////// </summary>
        ////[TestMethod()]
        ////public void SetEpisodeAsDownloadedTest() {
        ////    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        ////}
        ////
        /////// <summary>
        /////// A test for SetEpisodeAsSeen
        /////// </summary>
        ////[TestMethod()]
        ////public void SetEpisodeAsSeenTest() {
        ////    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        ////}
        ////
        /////// <summary>
        /////// A test for Signup
        /////// </summary>
        ////[TestMethod()]
        ////public void SignupTest() {
        ////    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        ////}

    }
}
