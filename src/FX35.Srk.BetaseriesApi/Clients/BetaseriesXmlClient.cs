using System;
using System.Collections.Generic;
using System.Linq;

namespace Srk.BetaseriesApi.Clients {

    /// <summary>
    /// This is the default Betaseries client implementation using XML data type transfert.
    /// This is the recommended client to use.
    /// </summary>
    /// <remarks>
    /// The async pattern is implemented in the base class.
    /// </remarks>
    public partial class BetaseriesXmlClient : BetaseriesBaseHttpClient, IMethodVersionReport {

        #region .ctor

        /// <summary>
        /// Simpliest class .ctor.
        /// </summary>
        /// <param name="apiKey">your API key (ask it on the website, don't use someone else's)</param>
        /// <param name="userAgent">anything like MyBetaseriesApp/1.0.0.0 (name/version)</param>
        public BetaseriesXmlClient(string apiKey, string userAgent) : this(apiKey, userAgent, null) { }

        /// <summary>
        /// Other class .ctor to override the BaseUrl.
        /// </summary>
        /// <param name="apiKey">your API key (ask it on the website, don't use someone else's)</param>
        /// <param name="userAgent">anything like MyBetaseriesApp/1.0.0.0 (name/version)</param>
        /// <param name="baseUrl">If you want to override urls (nice for https)</param>
        public BetaseriesXmlClient(string apiKey, string userAgent, string baseUrl) : base("{0}{1}.xml?{2}", apiKey, userAgent, baseUrl) { }

        #endregion

#pragma warning disable 1591
        #region IBetaseriesApi Members

        #region Shows

        public override IList<Show> SearchShows(string title) {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Search parameter must be non-empty", "title");

            if (IsCacheEnabled) {
                var fromCache = SearchShowsFromCache(title);
                if (fromCache != null)
                    return fromCache;
            }

            var response = ExecuteQuery("shows/search", "title", title);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            var result = ParseShows(xml);
            SearchShowsToCache(title, result);

            return result;
        }

        public override Show GetShow(string url) {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Empty show identifier", "url");

            if (IsCacheEnabled) {
                var fromCache = GetShowFromCache(url);
                if (fromCache != null)
                    return fromCache;
            }

            var response = ExecuteQuery("shows/display/" + url);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseShow(xml);
        }

        public override IList<Episode> GetEpisodes(string showUrl) {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "showUrl");

            var response = ExecuteQuery("shows/episodes/" + showUrl);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseEpisodesWithSeasons(xml);
        }

        public override IList<Episode> GetEpisodes(string showUrl, uint season) {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "showUrl");

            var response = ExecuteQuery(
                "shows/episodes/" + showUrl, 
                "season", season.ToString());
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseEpisodesWithSeasons(xml);
        }

        public override Episode GetEpisode(string showUrl, uint season, uint episode) {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "showUrl");

            var response = ExecuteQuery(
                "shows/episodes/" + showUrl, 
                "season", season.ToString(), 
                "episode", episode.ToString());
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseEpisodesWithSeasons(xml).FirstOrDefault();
        }

        public override void AddShow(string showUrl) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");

            var response = ExecuteQuery("shows/add/" + showUrl);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
        }

        public override void RemoveShow(string showUrl) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");

            var response = ExecuteQuery("shows/remove/" + showUrl);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
        }

        #endregion

        #region Members

        public override string Authenticate(string username, string password) {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username is empty", "username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password is empty", "password");

            var hash = HashString(password);
            var response = ExecuteQuery("members/auth", "login", username, "password", hash);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            var sessionToken = ParseSessionToken(xml);
            SetSessionTokens(sessionToken, username);

            return SessionToken;
        }

        public override bool GetIsSessionActive() {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var response = ExecuteQuery("members/is_active");
            var xml = ParseResponse(response);
            var errors = GetErrors(xml);

            if (errors != null && errors.Length > 0) {
                var err2001 = errors.FirstOrDefault(e => e.IntCode == 2001);
                if (errors.Length == 1 && err2001 != null) {
                    return false;
                }
            }

            HandleError(errors);

            return true;
        }

        public override void Logoff() {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var response = ExecuteQuery("members/destroy");
            var xml = ParseResponse(response);
            var errors = GetErrors(xml);

            if (errors != null && errors.Length > 0) {
                var err2001 = errors.FirstOrDefault(e => e.IntCode == 2001);
                if (errors.Length == 1 && err2001 != null) {
                    // session has expired
                    SetSessionTokens(null, null);
                    return;
                }
            }
            
            HandleError(errors);

            SetSessionTokens(null, null);
        }

        public override Member GetMember(string username) {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(SessionToken))
                throw new ArgumentException("username and SessionToken are empty", "username");

            string response;
            if (username == null) {
                response = ExecuteQuery("members/infos");
            } else {
                response = ExecuteQuery("members/infos/" + username);
            }
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseMember(xml);
        }

        public override void Signup(string username, string password, string email) {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username must not be empty", "username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password must not be empty", "password");
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("email must not be empty", "email");

            var response = ExecuteQuery("members/signup", "login", username, "password", password, "mail", email);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
        }

        public override IList<Episode> GetMembersNextEpisodes(bool onlyNextEpisode) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var dic = new Dictionary<string, string>();
            if (onlyNextEpisode)
                dic["view"] = "next";

            var response = ExecuteQuery("members/episodes/all", dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseEpisodes(xml);
        }

        public override Episode GetMembersNextShowEpisode(bool onlyNextEpisode, string showUrl) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");

            var dic = new Dictionary<string, string>();
            dic["show"] = showUrl;
            if (onlyNextEpisode)
                dic["view"] = "next";

            var response = ExecuteQuery("members/episodes/all", dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseEpisodes(xml).SingleOrDefault();
        }

        public override void SetEpisodeAsSeen(string showUrl, uint season, uint episode, ushort? mark) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");
            if (mark.HasValue && mark.Value > 5)
                throw new ArgumentException("mark must be between 1 and 5", "mark");

            var dic = new Dictionary<string, string>();
            dic["season"] = season.ToString();
            dic["episode"] = episode.ToString();
            if (mark.HasValue && mark.Value != 0)
                dic["note"] = mark.Value.ToString();

            var response = ExecuteQuery("members/watched/" + showUrl, dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            RemoveShowFromCache(showUrl);
        }

        public override void SetEpisodeAsDownloaded(string showUrl, uint season, uint episode) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");

            var dic = new Dictionary<string, string>();
            dic["season"] = season.ToString();
            dic["episode"] = episode.ToString();

            var response = ExecuteQuery("members/downloaded/" + showUrl, dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            RemoveShowFromCache(showUrl);
        }

        public override void SetEpisodeMark(string showUrl, uint season, uint episode, ushort mark) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");
            if (mark > 5)
                throw new ArgumentException("mark must be between 1 and 5", "mark");

            var dic = new Dictionary<string, string>();
            dic["season"] = season.ToString();
            dic["episode"] = episode.ToString();
            dic["note"] = mark.ToString();

            var response = ExecuteQuery("members/note/" + showUrl, dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            RemoveShowFromCache(showUrl);
        }

        public override IList<Notification> GetNotifications(bool? seen, uint? count, uint? fromNotificationId) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var dic = new Dictionary<string, string>();
            if (seen.HasValue)
                dic["seen"] = seen.Value ? "yes" : "no";
            if (count.HasValue)
                dic["number"] = count.Value.ToString();
            if (fromNotificationId.HasValue)
                dic["last_id"] = fromNotificationId.Value.ToString();

            var response = ExecuteQuery("members/notifications", dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseNotifications(xml);
        }

        public override IList<string> GetFriends(string username) {
            if (username == null && SessionToken == null)
                throw new ArgumentException("username and SessionToken are empty", "username");

            string query = username == null ? "members/friends" : ("members/friends/" + username);

            string response;
            if (username == null)
                response = ExecuteQuery("members/friends");
            else
                response = ExecuteQuery("members/friends/" + username);

            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseFriends(xml);
        }

        public override string[] GetBadges(string username) {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username is empty", "username");

            var response = ExecuteQuery("members/badges/" + username);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseBadges(xml);
        }

        #endregion

        #region Planning

        public override IList<Episode> GetMembersPlanning(string username) {
            return GetMembersPlanning(username, null);
        }

        public override IList<Episode> GetMembersPlanning(string username, bool unseenOnly) {
            return GetMembersPlanning(username, unseenOnly);
        }

        IList<Episode> GetMembersPlanning(string username, bool? unseenOnly) {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(SessionUsername))
                throw new ArgumentException("username and SessionUsername are empty", "username");


            var action = username != null ? "planning/member/" + username : "planning/member";
            var dic = new Dictionary<string, string>();
            if (unseenOnly.HasValue && unseenOnly.Value)
                dic["view"] = "unseen";

            var response = ExecuteQuery(action, dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParsePlanning(xml);
        }

        public override IList<Episode> GetPlanning() {
            var response = ExecuteQuery("planning/general");
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParsePlanning(xml);
        }

        #endregion

        #region Timeline

        public override IList<TimelineItem> GetFriendsTimeline(uint? count) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var dic = new Dictionary<string, string>();
            if (count.HasValue)
                dic["number"] = count.Value.ToString();

            var response = ExecuteQuery("timeline/friends", dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseTimeline(xml);
        }

        public override IList<TimelineItem> GetMainTimeline(uint? count) {
            var dic = new Dictionary<string, string>();
            if (count.HasValue)
                dic["number"] = count.Value.ToString();

            var response = ExecuteQuery("timeline/home", dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseTimeline(xml);
        }

        public override IList<TimelineItem> GetMemberTimeline(string username, uint? count) {
            if (username == null && SessionUsername == null)
                throw new ArgumentException("username and SessionToken are empty", "username");
            username = username ?? SessionUsername;

            var dic = new Dictionary<string, string>();
            if (count.HasValue)
                dic["number"] = count.Value.ToString();

            var response = ExecuteQuery("timeline/member/" + username, dic);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseTimeline(xml);
        }

        #endregion

        #region Comments

        public override IList<Comment> GetCommentsForEpisode(string showUrl, uint season, uint episode)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "showUrl");

            var response = ExecuteQuery("comments/episode/" + showUrl, "season", season.ToString(), "episode", episode.ToString());
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
            return ParseComment(xml);
        }

        public override IList<Comment> GetCommentsForShow(string showUrl)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "url");

            var response = ExecuteQuery("comments/show/" + showUrl);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
            return ParseComment(xml);
        }

        public override IList<Comment> GetCommentsForMember(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Empty username identifier", "username");

            var response = ExecuteQuery("comments/member/" + username);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
            return ParseComment(xml);
        }

        public override void CommentShow(string showUrl, string comment, uint? inReplyTo) {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "url");
            if (string.IsNullOrEmpty(comment))
                throw new ArgumentException("A comment must be provided", "comment");

            var post = new Dictionary<string, string>();
            post["text"] = comment;

            var get = new Dictionary<string, string>();
            get["show"] = showUrl;
            if (inReplyTo.HasValue)
                get["in_reply_to"] = inReplyTo.Value.ToString();

            var response = ExecuteQuery("comments/post/show", post, get);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
        }

        public override void CommentEpisode(string showUrl, uint season, uint episode, string comment, uint? inReplyTo)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not Logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier","url");
            if (string.IsNullOrEmpty(comment))
                throw new ArgumentException("A comment must be provided", "comment");

            var post = new Dictionary<string, string>();
            post["text"] = comment;

            var get = new Dictionary<string, string>();
            get["show"] = showUrl;
            get["season"] = season.ToString();
            get["episode"] = episode.ToString();
            if (inReplyTo.HasValue)
                get["in_reply_to"] = inReplyTo.Value.ToString();

            var response = ExecuteQuery("comments/post/episode", post, get);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
        }

        public override void CommentMember(string username, string comment, uint? inReplyTo)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not Logged-in");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Empty username", "usrname");
            if (string.IsNullOrEmpty(comment))
                throw new ArgumentException("A comment must be provided", "comment");

            var post = new Dictionary<string, string>();
            post["text"] = comment;

            var get = new Dictionary<string, string>();
            get["member"] = username;
            if (inReplyTo.HasValue)
                get["in_reply_to"] = inReplyTo.Value.ToString();

            var response = ExecuteQuery("comments/post/member", post, get);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
        }

        #endregion

        #region Subtitle

        public override IList<Subtitle> GetLatestShowSubtitles(string showUrl)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Search parameter must be not empty", "showUrl");

            var response = ExecuteQuery("/subtitles/show/" + showUrl);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
            return ParseSubtitle(xml);
        }

        public override IList<Subtitle> GetLatestShowSubtitles(string showUrl, string language)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Search parameter must be not empty", "showUrl");
            
            if (string.IsNullOrEmpty(language))
                throw new ArgumentException("Search parameter mus be not empty", "language");

            var response = ExecuteQuery("/subtitles/show/" + showUrl, "language", language);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
            return ParseSubtitle(xml);
        }

        public override IList<Subtitle> GetLatestSubtitles(string language, uint number)
        {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentException("Search parameter mus be not empty", "language");

            var response = ExecuteQuery("subtitles/last","number",number.ToString(),"language",language);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
            return ParseSubtitle(xml);
        }

        public override IList<Subtitle> GetLatestSubtitles(string language)
        {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentException("Search parameter mus be not empty", "language");

            var response = ExecuteQuery("subtitles/last","language", language);
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
            return ParseSubtitle(xml);
        }

        public override IList<Subtitle> GetLatestSubtitles()
        {
            var response = ExecuteQuery("subtitles/last");
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));
            return ParseSubtitle(xml);
        }

        #endregion

        public override ApiStatus GetStatus() {
            string response = ExecuteQuery("status");
            var xml = ParseResponse(response);
            HandleError(GetErrors(xml));

            return ParseStatus(xml);
        }

        #endregion
#pragma warning restore 1591

    }
}
