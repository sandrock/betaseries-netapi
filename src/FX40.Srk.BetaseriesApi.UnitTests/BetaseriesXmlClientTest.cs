using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Srk.BetaseriesApi;
using Srk.BetaseriesApi.Clients;

namespace Srk.BetaseriesApi.UnitTests {

    [TestClass]
    public class BetaseriesXmlClientTest {

        private static string GetSampleXml(string name) {
            return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, name));
        }

        #region .ctor

        [TestMethod]
        public void Ctor0_Success() {
            string key = "aaaaa";
            string agent = "bbbbb";

            var target = new BetaseriesXmlClient(key, agent);

            Assert.AreEqual(key, target.Key);
            Assert.AreEqual(agent, target.UserAgent);
        }

        [TestMethod]
        public void Ctor1_Success() {
            string key = "aaaaa";
            string agent = "bbbbb";
            string url = "ccccc";

            var target = new TestBetaseriesXmlClient(key, agent, url);

            Assert.AreEqual(key, target.Key);
            Assert.AreEqual(agent, target.UserAgent);
            Assert.AreEqual(url, target.BaseUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor0_NullKey_Failure() {
            string key = null;
            string agent = "bbbbb";

            var target = new BetaseriesXmlClient(key, agent);
        }

        [TestMethod]
        public void Ctor0_NullAgent_Failure() {
            string key = "aaaaa";
            string agent = null;

            var target = new BetaseriesXmlClient(key, agent);

            Assert.AreEqual(key, target.Key);
            Assert.IsNotNull(target.UserAgent);
            Assert.AreNotEqual(string.Empty, target.UserAgent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor1_NullKey_Failure() {
            string key = null;
            string agent = "bbbbb";
            string url = "ccccc";

            var target = new TestBetaseriesXmlClient(key, agent, url);
        }

        [TestMethod]
        public void Ctor1_NullAgent_Failure() {
            string key = "aaaaa";
            string agent = null;
            string url = "ccccc";

            var target = new TestBetaseriesXmlClient(key, agent, url);

            Assert.AreEqual(key, target.Key);
            Assert.IsNotNull(target.UserAgent);
            Assert.AreNotEqual(string.Empty, target.UserAgent);
            Assert.AreEqual(url, target.BaseUrl);
        }

        [TestMethod]
        public void Ctor1_NullBase_Failure() {
            string key = "aaaaa";
            string agent = "bbbbb";
            string url = null;

            var target = new TestBetaseriesXmlClient(key, agent, url);

            Assert.AreEqual(key, target.Key);
            Assert.AreEqual(agent, target.UserAgent);
            Assert.IsNotNull(target.BaseUrl);
            Assert.AreNotEqual(string.Empty, target.BaseUrl);
        }

        #endregion

        #region Session tokens

        [TestMethod]
        public void SetSessionTokens_Success() {
            var target = new BetaseriesXmlClient("aaa", null);

            Assert.IsNull(target.SessionToken);
            Assert.IsNull(target.SessionUsername);

            var sessionToken = "iiiiiiii";
            var sessionUsername = "jjjjjjjjjjjj";

            target.SetSessionTokens(sessionToken, sessionUsername);

            Assert.AreEqual(sessionToken, target.SessionToken);
            Assert.AreEqual(sessionUsername, target.SessionUsername);

            sessionToken = sessionUsername = null;

            target.SetSessionTokens(sessionToken, sessionUsername);

            Assert.IsNull(target.SessionToken);
            Assert.IsNull(target.SessionUsername);
        }

        #endregion

        #region HTTP Query

        [TestMethod]
        public void ExecuteQuery_TranslatesParameters_Success() {
            var target = new TestBetaseriesXmlClient();

            string action = "someAction";
            string key0 = "key0", key1 = "key1", val0 = "val0", val1 = "val1";
            var dic = new Dictionary<string, string>();
            dic.Add("key0", "val0");
            dic.Add("key1", "val1");

            string result = target.ExecuteQuery(action, key0, val0, key1, val1);
            string expected = TestBetaseriesXmlClient.GetRequestString(action, dic, true);

            Assert.AreEqual(expected, result);
        }


        #endregion

        #region IBetaseriesApi Members

        #region Shows

        #region SearchShows

        [TestMethod]
        public void SearchShows_Success() {
            string search = "star";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/search", action);
                Assert.IsTrue(dic.ContainsKey("title"));
                Assert.AreEqual(search, dic["title"]);
                return GetSampleXml("search-success.xml");
            });

            var result = target.SearchShows(search);
            target.VerifyHttpRequest();

            Assert.AreEqual(6, result.Count);
            var verifItem = result.First(i => i.Url == "stargatesg1");
            Assert.AreEqual("Stargate SG1", verifItem.Title);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchShows_EmptyParam_Fails() {
            var target = new TestBetaseriesXmlClient();
            target.SearchShows(string.Empty);
        }

        [TestMethod]
        public void SearchShowsAsync_BadApiKey_Fails() {
            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                return GetSampleXml("error-badapikey.xml");
            });

            try {
                target.SearchShows("aaaa");
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("1001", ex.BetaError.Code);
                Assert.AreEqual(1001, ex.BetaError.IntCode);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        #endregion

        #region GetShow

        [TestMethod]
        public void GetShow_Success() {
            string search = "dexter";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/display/" + search, action);
                return GetSampleXml("getshow-success.xml");
            });

            Show result = target.GetShow(search);
            target.VerifyHttpRequest();

            Assert.AreEqual("Dexter", result.Title);
            Assert.AreEqual("dexter", result.Url);
            Assert.AreEqual("Continuing", result.Status);
            Assert.AreEqual("http://www.betaseries.com/images/fonds/dexter.png", result.PictureUrl);
            Assert.IsTrue(result.Genres.Any(g => g == "Drama"));
            Assert.IsTrue(result.Genres.Any(g => g == "Action and Adventure"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetShow_EmptyParam_Fails() {
            var target = new TestBetaseriesXmlClient();
            target.GetShow(string.Empty);
        }

        [TestMethod]
        public void GetShow_NotFound_Fails() {

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                return GetSampleXml("getshow-notfound.xml");
            });

            try {
                target.GetShow("aaaa");
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("4001", ex.BetaError.Code);
                Assert.AreEqual(4001, ex.BetaError.IntCode);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        #endregion

        #region GetEpisodes

        [TestMethod]
        public void GetEpisodes0_Success() {
            string search = "dexter";
            string doc = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "getepisodes-success.xml"));

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/episodes/" + search, action);
                return doc;
            });

            var result = target.GetEpisodes(search);
            target.VerifyHttpRequest();

            Assert.AreEqual(4, result.Count);

            var first = result.FirstOrDefault();
            Assert.AreEqual(1U, first.Order, "ep number as int");
            Assert.AreEqual("1", first.EpisodeNumber, "ep number as string");
            Assert.AreEqual("S01E01", first.Number, "full ep number as string");
            Assert.AreEqual("1", first.Season, "season number as int");
            Assert.AreEqual(1U, first.SeasonOrder, "season number as string");
            Assert.AreEqual("Dexter", first.Title, "title");
            Assert.AreEqual("test", first.Description, "desc");
            Assert.AreEqual("http://cdn.betacie.com/betaseries/data/banners/episodes/79349/307473.jpg", first.PictureUrl);
            Assert.AreEqual(183, first.Ratings);
            Assert.AreEqual(4.5F, first.Rating);
            Assert.AreEqual(5, first.UserRating);
        }

        [TestMethod]
        public void GetEpisodes1_Success() {
            string search = "dexter";
            uint season = 1U;
            string doc = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "getepisodes-success.xml"));

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/episodes/" + search, action);
                Assert.AreEqual(season.ToString(), dic["season"]);
                return doc;
            });

            var result = target.GetEpisodes(search, season);
            target.VerifyHttpRequest();

            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetEpisodes0_EmptyParam_Fails() {
            var target = new TestBetaseriesXmlClient();
            target.GetEpisodes(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetEpisodes1_EmptyParam_Fails() {
            var target = new TestBetaseriesXmlClient();
            target.GetEpisodes(string.Empty, 1U);
        }

        #endregion

        #region GetEpisode

        [TestMethod]
        public void GetEpisode_Success() {
            string search = "dexter";
            uint season = 1U, episode = 1U;

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/episodes/" + search, action);
                Assert.AreEqual(season.ToString(), dic["season"]);
                Assert.AreEqual(episode.ToString(), dic["episode"]);
                return GetSampleXml("getepisodes-success.xml");
            });

            var result = target.GetEpisode(search, season, episode);
            target.VerifyHttpRequest();

            //TODO: finish this unit test
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetEpisode_EmptyParam_Fails() {
            var target = new TestBetaseriesXmlClient();
            target.GetEpisode(string.Empty, 1U, 1U);
        }

        #endregion

        #region AddShow

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddShow_NotLoggedIn_Failure() {
            string show = "dexter";

            var target = new TestBetaseriesXmlClient();
            target.AddShow(show);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddShow_EmptyParam_Failure() {
            string show = string.Empty;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens("aaaaa", "aaaaa");

            target.AddShow(show);
        }

        [TestMethod]
        public void AddShow_Success() {
            string show = "dexter";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/add/" + show, action);
                return GetSampleXml("success.xml");
            });
            target.SetSessionTokens("aaaaa", "aaaaa");

            target.AddShow(show);
            target.VerifyHttpRequest();
        }

        [TestMethod]
        public void AddShow_AlreadyInProfile_Failure() {
            string show = "dexter";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/add/" + show, action);
                return GetSampleXml("error-showAlreadyInProfile.xml");
            });
            target.SetSessionTokens("aaaaa", "aaaaa");

            try {
                target.AddShow(show);
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("2003", ex.BetaError.Code);
                Assert.AreEqual(2003, ex.BetaError.IntCode);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        #endregion

        #region RemoveShow

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveShow_NotLoggedIn_Failure() {
            string show = "dexter";

            var target = new TestBetaseriesXmlClient();
            target.RemoveShow(show);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveShow_EmptyParam_Failure() {
            string show = string.Empty;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens("aaaaa", "aaaaa");

            target.RemoveShow(show);
        }

        [TestMethod]
        public void RemoveShow_Success() {
            string show = "dexter";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/remove/" + show, action);
                return GetSampleXml("success.xml");
            });
            target.SetSessionTokens("aaaaa", "aaaaa");

            target.RemoveShow(show);
            target.VerifyHttpRequest();
        }

        [TestMethod]
        public void RemoveShow_NotInProfile_Failure() {
            string show = "dexter";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("shows/remove/" + show, action);
                return GetSampleXml("error-showNotInProfile.xml");
            });
            target.SetSessionTokens("aaaaa", "aaaaa");

            try {
                target.RemoveShow(show);
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("2004", ex.BetaError.Code);
                Assert.AreEqual(2004, ex.BetaError.IntCode);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        #endregion

        #endregion

        #region Members

        #region Authenticate

        [TestMethod]
        public void Authenticate_Success() {
            string user = "user", pass = "pass";
            string hash = "1a1dc91c907325c69271ddf0c944bc72";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/auth", action);
                Assert.AreEqual(user, dic["login"]);
                Assert.AreEqual(hash, dic["password"]);
                return GetSampleXml("authenticate-success.xml");
            });

            var result = target.Authenticate(user, pass);
            target.VerifyHttpRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Authenticate_EmptyUsername_Failure() {
            string show = string.Empty;

            var target = new TestBetaseriesXmlClient();

            target.Authenticate(string.Empty, "nnnn");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Authenticate_EmptyPassword_Failure() {
            string show = string.Empty;

            var target = new TestBetaseriesXmlClient();

            target.Authenticate("jhjkhkhk", string.Empty);
        }

        [TestMethod]
        public void Authenticate_InvalidPassword() {
            string user = "user", pass = "pass";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                return GetSampleXml("error-invalidPassword.xml");
            });


            try {
                var result = target.Authenticate(user, pass);
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("4003", ex.BetaError.Code);
                Assert.AreEqual(4003, ex.BetaError.IntCode);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        [TestMethod]
        public void Authenticate_InvalidUsername() {
            string user = "user", pass = "pass";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                return GetSampleXml("error-invalidUsername.xml");
            });


            try {
                var result = target.Authenticate(user, pass);
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("4002", ex.BetaError.Code);
                Assert.AreEqual(4002, ex.BetaError.IntCode);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        #endregion

        #region GetIsSessionActive

        [TestMethod]
        public void GetIsSessionActive_Active_Success() {
            string user = "user", token = "xxxxxxx";

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, user);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/is_active", action);
                Assert.AreEqual(token, dic["token"]);
                return GetSampleXml("success.xml");
            });

            var result = target.GetIsSessionActive();
            target.VerifyHttpRequest();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetIsSessionActive_Inactive_Success() {
            string user = "user", token = "xxxxxxx";

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, user);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/is_active", action);
                Assert.AreEqual(token, dic["token"]);
                return GetSampleXml("error-inactiveSession.xml");
            });

            //TODO: verify this test, there should be no exception
            bool result = false;
            try {
                result = target.GetIsSessionActive();
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("2001", ex.BetaError.Code);
                Assert.AreEqual(2001, ex.BetaError.IntCode);
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetIsSessionActive_EmptyToken_Failure() {
            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.Fail();
                return null;
            });

            target.GetIsSessionActive();
        }

        #endregion

        #region Logoff

        [TestMethod]
        public void Logoff_Active_Success() {
            string user = "user", token = "xxxxxxx";

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, user);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/destroy", action);
                Assert.AreEqual(token, dic["token"]);
                return GetSampleXml("success.xml");
            });

            target.Logoff();
            target.VerifyHttpRequest();

            Assert.IsNull(target.SessionToken);
            Assert.IsNull(target.SessionUsername);
        }

        [TestMethod]
        public void Logoff_Inactive_Success() {
            string user = "user", token = "xxxxxxx";

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, user);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/destroy", action);
                Assert.AreEqual(token, dic["token"]);
                return GetSampleXml("error-inactiveSession.xml");
            });

            target.Logoff();
            target.VerifyHttpRequest();

            Assert.IsNull(target.SessionToken);
            Assert.IsNull(target.SessionUsername);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Logoff_EmptyToken_Failure() {
            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.Fail();
                return null;
            });

            target.Logoff();
        }

        #endregion

        #region GetMember

        [TestMethod]
        public void GetMember0_Sucess() {
            string username = "srktest";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/infos/" + username, action);
                return GetSampleXml("getmember-success.xml");
            });

            var result = target.GetMember(username);
            target.VerifyHttpRequest();

            Assert.AreEqual(username, result.Username);
            Assert.AreEqual("aaaa", result.PictureUrl);
            Assert.AreEqual(7U, result.ShowCount);
            Assert.AreEqual(18U, result.SeasonCount);
            Assert.AreEqual(236U, result.EpisodeCount);
            Assert.AreEqual("45,38 %", result.Progress);
            Assert.AreEqual(284U, result.EpisodesToWatchCount);
            Assert.AreEqual(11, result.TimeRemaining.Value.Days);
            Assert.AreEqual(14, result.TimeRemaining.Value.Hours);
            Assert.AreEqual(5, result.TimeSpent.Value.Days);
            Assert.AreEqual(11, result.TimeSpent.Value.Hours);
            Assert.AreEqual(7, result.Shows.Count);
        }

        [TestMethod]
        public void GetMember0_HiddenOrNonExistingUser_Failure() {
            string username = "srktest";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                return GetSampleXml("error-2002-privacy.xml");
            });

            try {
                var result = target.GetMember(username);
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("2002", ex.BetaError.Code);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        [TestMethod]
        public void GetMember1_Sucess() {
            string username = "srktest";
            string token = "nkjlkjklj";

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/infos", action);
                Assert.AreEqual(token, dic["token"]);
                return GetSampleXml("getmember-success.xml");
            });

            var result = target.GetMember(null);
            target.VerifyHttpRequest();

            Assert.AreEqual(username, result.Username);
            Assert.AreEqual("aaaa", result.PictureUrl);
            Assert.AreEqual(7U, result.ShowCount);
            Assert.AreEqual(18U, result.SeasonCount);
            Assert.AreEqual(236U, result.EpisodeCount);
            Assert.AreEqual("45,38 %", result.Progress);
            Assert.AreEqual(284U, result.EpisodesToWatchCount);
            Assert.AreEqual(11, result.TimeRemaining.Value.Days);
            Assert.AreEqual(14, result.TimeRemaining.Value.Hours);
            Assert.AreEqual(5, result.TimeSpent.Value.Days);
            Assert.AreEqual(11, result.TimeSpent.Value.Hours);
            Assert.AreEqual(7, result.Shows.Count);
        }

        [TestMethod]
        public void GetMember1_SessionExpired_Failure() {
            string username = "srktest";
            string token = "nkjlkjklj";

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/infos", action);
                Assert.AreEqual(token, dic["token"]);
                return GetSampleXml("error-inactiveSession.xml");
            });

            try {
                var result = target.GetMember(null);
                Assert.Fail("An error was expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("2001", ex.BetaError.Code);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMember_EmptyUsername_Failure() {
            string show = string.Empty;

            var target = new TestBetaseriesXmlClient();

            target.GetMember(string.Empty);
        }

        #endregion

        #region Signup

        [TestMethod]
        public void Signup_Success() {
            string user = "user", pass = "SuperPassW0rd", email = "someEmail@company.com";

            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/signup", action);
                Assert.AreEqual(user, dic["login"]);
                Assert.AreEqual(pass, dic["password"]);
                Assert.AreEqual(email, dic["mail"]);
                return GetSampleXml("authenticate-success.xml");
            });

            target.Signup(user, pass, email);
            target.VerifyHttpRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Signup_EmptyUsername_Failure() {
            string user = string.Empty, pass = "SuperPassW0rd", email = "someEmail@company.com";

            var target = new TestBetaseriesXmlClient();

            target.Signup(user, pass, email);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Signup_EmptyPassword_Failure() {
            string user = "user", pass = string.Empty, email = "someEmail@company.com";

            var target = new TestBetaseriesXmlClient();

            target.Signup(user, pass, email);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Signup_EmptyEmail_Failure() {
            string user = "user", pass = "SuperPassW0rd", email = string.Empty;

            var target = new TestBetaseriesXmlClient();

            target.Signup(user, pass, email);
        }

        #endregion

        #region GetMembersNextEpisodes

        [TestMethod]
        public void GetMembersNextEpisodes_Success() {
            string username = "srktest";
            string token = "nkjlkjklj";

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/episodes/all", action);
                Assert.AreEqual("next", dic["view"]);
                return GetSampleXml("nextepisode-success.xml");
            });

            var result = target.GetMembersNextEpisodes(true);
            target.VerifyHttpRequest();

            Assert.AreEqual(11, result.Count);
            var first = result.First();
            Assert.AreEqual("24", first.ShowUrl);
            Assert.AreEqual("S01E13", first.Number);
            Assert.AreEqual("24", first.ShowName);
            Assert.AreEqual("Day 1 - 12:00 P.M.-1:00 P.M.", first.Title);
            Assert.AreEqual(new DateTime(2002, 2, 25), first.Date.Value.Date);
            Assert.AreEqual(false, first.IsDownloaded);
        }

        [TestMethod]
        public void GetMembersNextEpisodes1_Success() {
            string username = "srktest";
            string token = "nkjlkjklj";

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/episodes/all", action);
                Assert.IsFalse(dic.ContainsKey("view"));
                return GetSampleXml("nextepisode-success.xml");
            });

            var result = target.GetMembersNextEpisodes(false);
            target.VerifyHttpRequest();

            Assert.AreEqual(11, result.Count);
            var first = result.First();
            Assert.AreEqual("24", first.ShowUrl);
            Assert.AreEqual("S01E13", first.Number);
            Assert.AreEqual("24", first.ShowName);
            Assert.AreEqual("Day 1 - 12:00 P.M.-1:00 P.M.", first.Title);
            Assert.AreEqual(new DateTime(2002, 2, 25), first.Date.Value.Date);
            Assert.AreEqual(false, first.IsDownloaded);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetMembersNextEpisodes_NotLoggedIn_Failure() {
            var target = new TestBetaseriesXmlClient();
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/episodes/all", action);
                return GetSampleXml("nextepisode-success.xml");
            });

            var result = target.GetMembersNextEpisodes(true);
        }

        #endregion

        #region SetEpisodeAsSeen

        [TestMethod]
        public void SetEpisodeAsSeen_WithMark_Success() {
            string username = "srktest", token = "nkjlkjklj";
            string show = "dexter";
            uint season = 2U, episode = 12U;
            ushort? mark = 3;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/watched/" + show, action);
                Assert.AreEqual(mark.ToString(), dic["note"]);
                Assert.AreEqual(season.ToString(), dic["season"]);
                Assert.AreEqual(episode.ToString(), dic["episode"]);
                return GetSampleXml("success.xml");
            });

            target.SetEpisodeAsSeen(show, season, episode, mark);
            target.VerifyHttpRequest();
        }

        [TestMethod]
        public void SetEpisodeAsSeen_WithoutMark_Success() {
            string username = "srktest", token = "nkjlkjklj";
            string show = "dexter";
            uint season = 2U, episode = 12U;
            ushort? mark = null;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/watched/" + show, action);
                Assert.IsFalse(dic.ContainsKey("note"));
                Assert.AreEqual(season.ToString(), dic["season"]);
                Assert.AreEqual(episode.ToString(), dic["episode"]);
                return GetSampleXml("success.xml");
            });

            target.SetEpisodeAsSeen(show, season, episode, mark);
            target.VerifyHttpRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetEpisodeAsSeen_NotLoggedIn_Failure() {
            string show = "dexter";
            uint season = 2U, episode = 12U;
            ushort? mark = 3;

            var target = new TestBetaseriesXmlClient();

            target.SetEpisodeAsSeen(show, season, episode, mark);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetEpisodeAsSeen_ShowArgEmpty_Failure() {
            string username = "srktest", token = "nkjlkjklj";
            string show = string.Empty;
            uint season = 2U, episode = 12U;
            ushort? mark = 3;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);

            target.SetEpisodeAsSeen(show, season, episode, mark);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetEpisodeAsSeen_MarkArgInvalid_Failure() {
            string username = "srktest", token = "nkjlkjklj";
            string show = string.Empty;
            uint season = 2U, episode = 12U;
            ushort? mark = 6;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);

            target.SetEpisodeAsSeen(show, season, episode, mark);
        }

        #endregion

        #region SetEpisodeAsDownloaded

        [TestMethod]
        public void SetEpisodeAsSeen_Success() {
            string username = "srktest", token = "nkjlkjklj";
            string show = "dexter";
            uint season = 2U, episode = 12U;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/downloaded/" + show, action);
                Assert.AreEqual(season.ToString(), dic["season"]);
                Assert.AreEqual(episode.ToString(), dic["episode"]);
                return GetSampleXml("success.xml");
            });

            target.SetEpisodeAsDownloaded(show, season, episode);
            target.VerifyHttpRequest();
        }

        [TestMethod]
        public void SetEpisodeAsSeen_NoSuchEpisode_Failure() {
            string username = "srktest", token = "nkjlkjklj";
            string show = "dexter";
            uint season = 2U, episode = 124U;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/downloaded/" + show, action);
                Assert.AreEqual(season.ToString(), dic["season"]);
                Assert.AreEqual(episode.ToString(), dic["episode"]);
                return GetSampleXml("error-3004-noSuchEpisode.xml");
            });

            try {
                target.SetEpisodeAsDownloaded(show, season, episode);
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("3004", ex.BetaError.Code);
                Assert.AreEqual(3004, ex.BetaError.IntCode);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        [TestMethod]
        public void SetEpisodeAsSeen_NotEnabledFeature_Failure() {
            string username = "srktest", token = "nkjlkjklj";
            string show = "dexter";
            uint season = 2U, episode = 124U;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/downloaded/" + show, action);
                Assert.AreEqual(season.ToString(), dic["season"]);
                Assert.AreEqual(episode.ToString(), dic["episode"]);
                return GetSampleXml("error-2007-downloadedNotEnabled.xml");
            });

            try {
                target.SetEpisodeAsDownloaded(show, season, episode);
                Assert.Fail("Error expected");
            } catch (BetaException ex) {
                target.VerifyHttpRequest();
                Assert.AreEqual("2007", ex.BetaError.Code);
                Assert.AreEqual(2007, ex.BetaError.IntCode);
            } catch {
                Assert.Fail("Different error expected");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetEpisodeAsDownloaded_NotLoggedIn_Failure() {
            string show = "dexter";
            uint season = 2U, episode = 12U;

            var target = new TestBetaseriesXmlClient();

            target.SetEpisodeAsDownloaded(show, season, episode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetEpisodeAsDownloaded_ShowArgEmpty_Failure() {
            string username = "srktest", token = "nkjlkjklj";
            string show = string.Empty;
            uint season = 2U, episode = 12U;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);

            target.SetEpisodeAsDownloaded(show, season, episode);
        }

        #endregion

        #region SetEpisodeMark

        [TestMethod]
        public void SetEpisodeMark_Success() {
            string username = "srktest", token = "nkjlkjklj";
            string show = "dexter";
            uint season = 2U, episode = 12U;
            ushort mark = 3;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);
            target.SetHttpFunc((action, dic) => {
                Assert.AreEqual("members/note/" + show, action);
                Assert.AreEqual(mark.ToString(), dic["note"]);
                Assert.AreEqual(season.ToString(), dic["season"]);
                Assert.AreEqual(episode.ToString(), dic["episode"]);
                return GetSampleXml("success.xml");
            });

            target.SetEpisodeMark(show, season, episode, mark);
            target.VerifyHttpRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetEpisodeMark_NotLoggedIn_Failure() {
            string show = "dexter";
            uint season = 2U, episode = 12U;
            ushort mark = 3;

            var target = new TestBetaseriesXmlClient();

            target.SetEpisodeMark(show, season, episode, mark);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetEpisodeMark_ShowArgEmpty_Failure() {
            string username = "srktest", token = "nkjlkjklj";
            string show = string.Empty;
            uint season = 2U, episode = 12U;
            ushort mark = 3;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);

            target.SetEpisodeMark(show, season, episode, mark);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetEpisodeMark_MarkArgInvalid_Failure() {
            string username = "srktest", token = "nkjlkjklj";
            string show = string.Empty;
            uint season = 2U, episode = 12U;
            ushort mark = 6;

            var target = new TestBetaseriesXmlClient();
            target.SetSessionTokens(token, username);

            target.SetEpisodeMark(show, season, episode, mark);
        }

        #endregion

        #region GetNotifications

        //TODO: Write tests for GetNotifications

        #endregion

        #region GetFriends

        //TODO: Write tests for GetFriends

        #endregion

        #region GetBadges

        //TODO: Write tests for GetBadges

        #endregion

        #endregion

        #endregion

        #region Timeline

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #endregion

        #region Comments

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #region xxxxxxxxxxxxx

        #endregion

        #endregion

    }
}
