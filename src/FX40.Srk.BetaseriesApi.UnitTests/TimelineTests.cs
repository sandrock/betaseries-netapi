using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Srk.BetaseriesApi;
using Srk.BetaseriesApi.Resources;

namespace Srk.BetaseriesApi.UnitTests {
    [TestClass]
    public class TimelineTests {

        [ClassInitialize]
        public static void Initialize(TestContext testContext) {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }

        #region ParseTimelineItemType

        [TestMethod]
        public void ParseTimelineItemType_Existing() {
            // prepare
            var input = "friend_add";
            var expected = TimelineItemType.FriendAdded;

            // texecute
            var actual = TimelineItem.ParseTimelineItemType(input);

            // verify
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParseTimelineItemType_Unknown() {
            // prepare
            var input = "jdcvzehgfez";
            var expected = TimelineItemType.Unknown;

            // texecute
            var actual = TimelineItem.ParseTimelineItemType(input);

            // verify
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetFormatableTranslation

        [TestMethod]
        public void GetFormatableTranslation_WikiUpdate() {
            // prepare
            var input = TimelineItemType.WikiUpdate;
            var expected = GeneralStrings.TimelineItemType_WikiUpdate_;

            // texecute
            var actual = TimelineItem.GetFormatableTranslation(input);

            // verify
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetFormatableTranslation_Comment() {
            // prepare
            var input = TimelineItemType.CommentOnShow;
            var expected = GeneralStrings.TimelineItemType_CommentOnShow_;

            // texecute
            var actual = TimelineItem.GetFormatableTranslation(input);

            // verify
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetFormatableTranslation_Unknown() {
            // prepare
            var input = TimelineItemType.Unknown;
            var expected = GeneralStrings.TimelineItemType_Unknown_;

            // texecute
            var actual = TimelineItem.GetFormatableTranslation(input);

            // verify
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region TimelineItem .ctor

        [TestMethod]
        public void TimelineItemCtor_FriendAdded() {
            // prepare
            string reference = "friender", username = "friendee", type = "friend_add";
            DateTime? date = DateTime.Now;
            var itype = TimelineItemType.FriendAdded;
            var expectedMsg = string.Format(
                GeneralStrings.TimelineItemType_FriendAdded_,
                username, reference);

            // execute
            var item = new TimelineItem(reference, username, type, date, null, null);

            //verify
            Assert.AreEqual(date, item.Date);
            Assert.AreEqual(reference, item.Reference);
            Assert.IsNull(item.ReferenceBadge);
            Assert.IsNull(item.ReferenceEpisode);
            Assert.AreEqual(type, item.ServiceType);
            Assert.AreEqual(itype, item.Type);
            Assert.AreEqual(username, item.Username);
            Assert.AreEqual(expectedMsg, item.Message);
        }

        [TestMethod]
        public void TimelineItemCtor_Unknown() {
            // prepare
            string reference = "some.reference", username = "some-username", type = "bidou";
            DateTime? date = DateTime.Now;
            var itype = TimelineItemType.Unknown;
            var expectedMsg = string.Format(
                GeneralStrings.TimelineItemType_Unknown_,
                username, type, reference);

            // execute
            var item = new TimelineItem(reference, username, type, date, null, null);

            //verify
            Assert.AreEqual(date, item.Date);
            Assert.AreEqual(reference, item.Reference);
            Assert.IsNull(item.ReferenceBadge);
            Assert.IsNull(item.ReferenceEpisode);
            Assert.AreEqual(type, item.ServiceType);
            Assert.AreEqual(itype, item.Type);
            Assert.AreEqual(username, item.Username);
            Assert.AreEqual(expectedMsg, item.Message);
        }

        [TestMethod]
        public void TimelineItemCtor_Badge_Confirme() {
            // prepare
            string reference = "confirme", username = "some-username", type = "badge";
            DateTime? date = DateTime.Now;
            var itype = TimelineItemType.BadgeEarned;
            var iBadge = Badges.Confirme;
            var expectedMsg = string.Format(
                GeneralStrings.TimelineItemType_BadgeEarned_,
                username, GeneralStrings.Badges_Confirme);

            // execute
            var item = new TimelineItem(reference, username, type, date, null, null);

            //verify
            Assert.AreEqual(date, item.Date);
            Assert.AreEqual(reference, item.Reference);
            Assert.AreEqual(iBadge, item.ReferenceBadge);
            Assert.IsNull(item.ReferenceEpisode);
            Assert.AreEqual(type, item.ServiceType);
            Assert.AreEqual(itype, item.Type);
            Assert.AreEqual(username, item.Username);
            Assert.AreEqual(expectedMsg, item.Message);
        }

        [TestMethod]
        public void TimelineItemCtor_Badge_Unknown() {
            // prepare
            string reference = "bidabidou", username = "some-username", type = "badge";
            DateTime? date = DateTime.Now;
            var itype = TimelineItemType.BadgeEarned;
            var iBadge = Badges.Unknown;
            var expectedMsg = string.Format(
                GeneralStrings.TimelineItemType_BadgeEarned_,
                username, "bidabidou");

            // execute
            var item = new TimelineItem(reference, username, type, date, null, null);

            //verify
            Assert.AreEqual(date, item.Date);
            Assert.AreEqual(reference, item.Reference);
            Assert.AreEqual(iBadge, item.ReferenceBadge);
            Assert.IsNull(item.ReferenceEpisode);
            Assert.AreEqual(type, item.ServiceType);
            Assert.AreEqual(itype, item.Type);
            Assert.AreEqual(username, item.Username);
            Assert.AreEqual(expectedMsg, item.Message);
        }

        #endregion


    }
}
