using System;
using System.Collections.Generic;
using System.Linq;

namespace Srk.BetaseriesApi.Clients
{

    /// <summary>
    /// This is the default Betaseries client implementation using XML data type transfert.
    /// This is the recommended client to use.
    /// </summary>
    public partial class BetaseriesXmlClient : BetaseriesBaseHttpClient, IMethodVersionReport
    {

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

        public override void SearchShowsAsync(string title, AsyncResponseHandler<IList<Show>> callback)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Search parameter must be non-empty", "title");

            if (IsCacheEnabled)
            {
                var fromCache = SearchShowsFromCache(title);
                if (fromCache != null)
                {
                    callback(this, new AsyncResponseArgs<IList<Show>>(fromCache));
                    return;
                }
            }

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseShows(xml);
                    SearchShowsToCache(title, result);

                    callback(this, new AsyncResponseArgs<IList<Show>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<Show>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<Show>>(ex)),
            "shows/search", "title", title);
        }

        public override void GetShowAsync(string showUrl, AsyncResponseHandler<Show> callback)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "url");

            if (IsCacheEnabled)
            {
                var fromCache = GetShowFromCache(showUrl);
                if (fromCache != null)
                {
                    //RaiseGetShowEnded(new AsyncResponseArgs<Show>(fromCache));
                    callback(this, new AsyncResponseArgs<Show>(fromCache));
                    return;
                }
            }

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseShow(xml);

                    callback(this, new AsyncResponseArgs<Show>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<Show>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<Show>(ex)),
            "shows/display/" + showUrl);
        }

        public override void GetEpisodesAsync(string showUrl, AsyncResponseHandler<IList<Episode>> callback)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "showUrl");

            if (IsCacheEnabled)
            {
                var fromCache = GetEpisodesFromCache(showUrl);
                if (fromCache != null)
                {
                    callback(this, new AsyncResponseArgs<IList<Episode>>(fromCache));
                    return;
                }
            }

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseEpisodesWithSeasons(xml);

                    GetEpisodesToCache(showUrl, result);

                    callback(this, new AsyncResponseArgs<IList<Episode>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<Episode>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<Episode>>(ex)),
            "shows/episodes/" + showUrl);
        }

        public override void GetEpisodesAsync(string showUrl, uint season, AsyncResponseHandler<IList<Episode>> callback)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "showUrl");

            ////if (IsCacheEnabled) {
            ////    var fromCache = GetEpisodesFromCache(showUrl);
            ////    if (fromCache != null) {
            ////        var filtered = fromCache
            ////            .Where(e => e.Season == oups)
            ////            .Tolist();
            ////        callback(_getEpisodesEnded, new AsyncResponseArgs<IList<Episode>>(fromCache));
            ////        return;
            ////    }
            ////}

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseEpisodesWithSeasons(xml);

                    callback(this, new AsyncResponseArgs<IList<Episode>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<Episode>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<Episode>>(ex)),
            "shows/episodes/" + showUrl, "season", season.ToString());
        }

        public override void GetEpisodeAsync(string showUrl, uint season, uint episode, AsyncResponseHandler<Episode> callback)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "showUrl");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseEpisodesWithSeasons(xml).FirstOrDefault();

                    callback(this, new AsyncResponseArgs<Episode>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<Episode>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<Episode>(ex)),
            "shows/episodes/" + showUrl, "season", season.ToString(), "episode", episode.ToString());
        }

        public override void AddShowAsync(string showUrl, AsyncResponseHandler callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "shows/add/" + showUrl);
        }

        public override void RemoveShowAsync(string showUrl, AsyncResponseHandler callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "shows/remove/" + showUrl);
        }

        #endregion

        #region Subtitles

        public override void GetShowSubtitlesAsync(string showUrl, string language, uint? season, uint? episode, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            callback(this, new AsyncResponseArgs<IList<Subtitle>>(new NotImplementedException()));
        }

        public override void GetLatestSubtitlesAsync(AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));
                        var result = ParseSubtitle(xml);
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex)),
                "subtitles/last");
        }

        public override void GetLatestSubtitlesAsync(string language, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(language))
                            throw new ArgumentException("Search parameter mus be not empty", "language");

                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));
                        var result = ParseSubtitle(xml);
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex)),
                "subtitles/last", "language", language);
        }

        public override void GetLatestSubtitlesAsync(string language, uint number, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(language))
                            throw new ArgumentException("Search parameter mus be not empty", "language");

                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));
                        var result = ParseSubtitle(xml);
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex)),
                "subtitles/last", "number", number.ToString(), "language", language);
        }

        public override void GetLatestShowSubtitlesAsync(string showUrl, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(showUrl))
                            throw new ArgumentException("Search parameter must be not empty", "showUrl");

                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));
                        var result = ParseSubtitle(xml);
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex)),
                "/subtitles/show/" + showUrl);
        }

        public override void GetLatestShowSubtitlesAsync(string showUrl, string language, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(showUrl))
                            throw new ArgumentException("Search parameter must be not empty", "showUrl");

                        if (string.IsNullOrEmpty(language))
                            throw new ArgumentException("Search parameter mus be not empty", "language");

                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));
                        var result = ParseSubtitle(xml);
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex)),
                "/subtitles/show/" + showUrl, "language", language);
        }

        public override void GetLatestShowSubtitlesAsync(string showUrl, string language, uint number, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(showUrl))
                            throw new ArgumentException("Search parameter must be not empty", "showUrl");

                        if (string.IsNullOrEmpty(language))
                            throw new ArgumentException("Search parameter mus be not empty", "language");

                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));
                        var result = ParseSubtitle(xml);
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<IList<Subtitle>>(ex)),
                "/subtitles/show/" + showUrl, "language", language);
        }

        #endregion

        #region Members

        public override void AuthenticateAsync(string username, string password, AsyncResponseHandler<string> callback)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username is empty", "username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password is empty", "password");

            var hash = HashString(password);

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var sessionToken = ParseSessionToken(xml);
                    SetSessionTokens(sessionToken, username);

                    callback(this, new AsyncResponseArgs<string>(sessionToken));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<string>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<string>(ex)),
            "members/auth", "login", username, "password", hash);
        }

        public override void GetIsSessionActiveAsync(AsyncResponseHandler<bool> callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    var errors = GetErrors(xml);

                    if (errors != null && errors.Length > 0)
                    {
                        var err2001 = errors.FirstOrDefault(e => e.IntCode == 2001);
                        if (errors.Length == 1 && err2001 != null)
                        {
                            callback(this, new AsyncResponseArgs<bool>(false));
                            return;
                        }
                    }

                    HandleError(errors);

                    callback(this, new AsyncResponseArgs<bool>(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<bool>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<bool>(ex)),
            "members/is_active");
        }

        public override void LogoffAsync(AsyncResponseHandler callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    SetSessionTokens(null, null);

                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "members/destroy");
        }

        public override void GetMemberAsync(string username, AsyncResponseHandler<Member> callback)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(SessionToken))
                throw new ArgumentException("username and SessionToken are empty", "username");

            string q;
            if (username == null)
            {
                q = "members/infos";
            }
            else
            {
                q = "members/infos/" + username;
            }

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var member = ParseMember(xml);

                    callback(this, new AsyncResponseArgs<Member>(member));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<Member>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<Member>(ex)),
            q);
        }

        public override void SignupAsync(string username, string password, string email, AsyncResponseHandler callback)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username must not be empty", "username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password must not be empty", "password");
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("email must not be empty", "email");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "members/signup", "login", username, "password", password, "mail", email);
        }

        public override void GetMembersNextEpisodesAsync(bool onlyNextEpisode, AsyncResponseHandler<IList<Episode>> callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var dic = new Dictionary<string, string>();
            if (onlyNextEpisode)
                dic["view"] = "next";

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseEpisodes(xml);

                    callback(this, new AsyncResponseArgs<IList<Episode>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<Episode>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<Episode>>(ex)),
            "members/episodes/all", dic);
        }

        public override void GetMembersNextShowEpisodeAsync(bool onlyNextEpisode, string showUrl, AsyncResponseHandler<Episode> callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");

            var dic = new Dictionary<string, string>();
            dic["show"] = showUrl;
            if (onlyNextEpisode)
                dic["view"] = "next";

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseEpisodes(xml)
                        .SingleOrDefault();

                    callback(this, new AsyncResponseArgs<Episode>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<Episode>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<Episode>(ex)),
            "members/episodes/all", dic);
        }

        public override void SetEpisodeAsSeenAsync(string showUrl, uint season, uint episode, ushort? mark, AsyncResponseHandler callback)
        {
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

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    RemoveShowFromCache(showUrl);

                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "members/watched/" + showUrl, dic);
        }

        public override void SetEpisodeAsDownloadedAsync(string showUrl, uint season, uint episode, AsyncResponseHandler callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("showUrl is null or empty", "showUrl");

            var dic = new Dictionary<string, string>();
            dic["season"] = season.ToString();
            dic["episode"] = episode.ToString();

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    RemoveShowFromCache(showUrl);

                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "members/downloaded/" + showUrl, dic);
        }

        public override void SetEpisodeMarkAsync(string showUrl, uint season, uint episode, ushort mark, AsyncResponseHandler callback)
        {
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

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    RemoveShowFromCache(showUrl);

                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "members/note/" + showUrl, dic);
        }

        public override void GetNotificationsAsync(bool? seen, uint? count, uint? fromNotificationId, AsyncResponseHandler<IList<Notification>> callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var dic = new Dictionary<string, string>();
            if (seen.HasValue)
                dic["seen"] = seen.Value ? "yes" : "no";
            if (count.HasValue)
                dic["number"] = count.Value.ToString();
            if (fromNotificationId.HasValue)
                dic["last_id"] = fromNotificationId.Value.ToString();

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseNotifications(xml);

                    callback(this, new AsyncResponseArgs<IList<Notification>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<Notification>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<Notification>>(ex)),
            "members/notifications", dic);
        }

        public override void GetFriendsAsync(string username, AsyncResponseHandler<IList<string>> callback)
        {
            if (username == null && SessionUsername == null)
                throw new ArgumentException("username and SessionUsername are empty", "username");

            string query = username == null ? "members/friends" : ("members/friends/" + username);

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseFriends(xml);

                    callback(this, new AsyncResponseArgs<IList<string>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<string>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<string>>(ex)),
            query);
        }

        public override void GetBadgesAsync(string username, AsyncResponseHandler<string[]> callback)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username is empty", "username");

            string query = username == null ? "members/friends" : ("members/friends/" + username);

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseBadges(xml);

                    callback(this, new AsyncResponseArgs<string[]>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<string[]>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<string[]>(ex)),
            "members/badges/" + username);
        }

        #endregion

        #region Planning

        public override void GetMembersPlanningAsync(string username, AsyncResponseHandler<IList<Episode>> callback)
        {
            this.GetMembersPlanningAsync(username, null, callback);
        }

        public override void GetMembersPlanningAsync(string username, bool unseenOnly, AsyncResponseHandler<IList<Episode>> callback)
        {
            this.GetMembersPlanningAsync(username, unseenOnly, callback);
        }

        ////public override void GetPlanningAsync(AsyncResponseHandler<IList<Episode>> callback)
        ////{
        ////    this.GetMembersPlanningAsync(null, null);
        ////}

        void GetMembersPlanningAsync(string username, bool? unseenOnly, AsyncResponseHandler<IList<Episode>> callback)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(SessionUsername))
                throw new ArgumentException("username and SessionToken are empty", "username");


            var action = username != null ? "planning/member/" + username : "planning/member";
            var dic = new Dictionary<string, string>();
            if (unseenOnly.HasValue && unseenOnly.Value)
                dic["view"] = "unseen";

            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));

                        var result = ParsePlanning(xml);

                        callback(this, new AsyncResponseArgs<IList<Episode>>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<IList<Episode>>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<IList<Episode>>(ex)),
                action, dic);
        }

        public override void GetPlanningAsync(AsyncResponseHandler<IList<Episode>> callback)
        {
            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));

                        var result = ParsePlanning(xml);

                        callback(this, new AsyncResponseArgs<IList<Episode>>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<IList<Episode>>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<IList<Episode>>(ex)),
                "planning/general");
        }

        #endregion

        #region Timeline

        public override void GetFriendsTimelineAsync(uint? count, AsyncResponseHandler<IList<TimelineItem>> callback)
        {
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var dic = new Dictionary<string, string>();
            if (count.HasValue)
                dic["number"] = count.Value.ToString();

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseTimeline(xml);

                    callback(this, new AsyncResponseArgs<IList<TimelineItem>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<TimelineItem>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<TimelineItem>>(ex)),
            "timeline/friends", dic);
        }

        public override void GetMainTimelineAsync(uint? count, AsyncResponseHandler<IList<TimelineItem>> callback)
        {
            var dic = new Dictionary<string, string>();
            if (count.HasValue)
                dic["number"] = count.Value.ToString();

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseTimeline(xml);

                    callback(this, new AsyncResponseArgs<IList<TimelineItem>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<TimelineItem>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<TimelineItem>>(ex)),
            "timeline/home", dic);
        }

        public override void GetMemberTimelineAsync(string username, uint? count, AsyncResponseHandler<IList<TimelineItem>> callback)
        {
            if (username == null && SessionUsername == null)
                throw new ArgumentException("username and SessionUsername are empty", "username");
            username = username ?? SessionUsername;

            var dic = new Dictionary<string, string>();
            if (count.HasValue)
                dic["number"] = count.Value.ToString();

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseTimeline(xml);

                    callback(this, new AsyncResponseArgs<IList<TimelineItem>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<TimelineItem>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<TimelineItem>>(ex)),
            "timeline/member/" + username, dic);
        }

        #endregion

        #region Comments

        public override void GetCommentsForEpisodeAsync(string showUrl, uint season, uint episode, AsyncResponseHandler<IList<Comment>> callback)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "showUrl");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseComment(xml);

                    callback(this, new AsyncResponseArgs<IList<Comment>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<Comment>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<Comment>>(ex)),
            "comments/episode/" + showUrl, "season", season.ToString(), "episode", episode.ToString());
        }

        public override void GetCommentsForShowAsync(string showUrl, AsyncResponseHandler<IList<Comment>> callback)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "url");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseComment(xml);

                    callback(this, new AsyncResponseArgs<IList<Comment>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<Comment>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<Comment>>(ex)),
            "comments/show/" + showUrl);
        }

        public override void GetCommentsForMemberAsync(string username, AsyncResponseHandler<IList<Comment>> callback)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Empty username", "username");

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));

                    var result = ParseComment(xml);

                    callback(this, new AsyncResponseArgs<IList<Comment>>(result));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs<IList<Comment>>(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs<IList<Comment>>(ex)),
            "comments/member/" + username);
        }

        public override void CommentShowAsync(string showUrl, string comment, uint? inReplyTo, AsyncResponseHandler callback)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "url");
            if (string.IsNullOrEmpty(comment))
                throw new ArgumentException("A comment must be provided", "comment");
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var post = new Dictionary<string, string>();
            post["text"] = comment;

            var get = new Dictionary<string, string>();
            get["show"] = showUrl;
            if (inReplyTo.HasValue)
                get["in_reply_to"] = inReplyTo.Value.ToString();

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));
                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "comments/post/show", post, get);
        }

        public override void CommentEpisodeAsync(string showUrl, uint season, uint episode, string comment, uint? inReplyTo, AsyncResponseHandler callback)
        {
            if (string.IsNullOrEmpty(showUrl))
                throw new ArgumentException("Empty show identifier", "url");
            if (string.IsNullOrEmpty(comment))
                throw new ArgumentException("A comment must be provided", "comment");
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var post = new Dictionary<string, string>();
            post["text"] = comment;

            var get = new Dictionary<string, string>();
            get["show"] = showUrl;
            get["season"] = season.ToString();
            get["episode"] = episode.ToString();
            if (inReplyTo.HasValue)
                get["in_reply_to"] = inReplyTo.Value.ToString();

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));
                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "comments/post/episode", post, get);
        }

        public override void CommentMemberAsync(string username, string comment, uint? inReplyTo, AsyncResponseHandler callback)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Empty username", "usrname");
            if (string.IsNullOrEmpty(comment))
                throw new ArgumentException("A comment must be provided", "comment");
            if (SessionToken == null)
                throw new InvalidOperationException("Not logged-in");

            var post = new Dictionary<string, string>();
            post["text"] = comment;

            var get = new Dictionary<string, string>();
            get["member"] = username;
            if (inReplyTo.HasValue)
                get["in_reply_to"] = inReplyTo.Value.ToString();

            this.ExecuteQuery(new AsyncCallback((response) =>
            {
                try
                {
                    var xml = ParseResponse(response);
                    HandleError(GetErrors(xml));
                    callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    callback(this, new AsyncResponseArgs(ex));
                }
            }),
            ex => callback(this, new AsyncResponseArgs(ex)),
            "comments/post/member", post, get);
        }

        #endregion

        public override void GetStatusAsync(AsyncResponseHandler<ApiStatus> callback)
        {
            this.ExecuteQuery(
                new AsyncCallback((response) =>
                {
                    try
                    {
                        var xml = ParseResponse(response);
                        HandleError(GetErrors(xml));
                        var result = ParseStatus(xml);
                        callback(this, new AsyncResponseArgs<ApiStatus>(result));
                    }
                    catch (Exception ex)
                    {
                        callback(this, new AsyncResponseArgs<ApiStatus>(ex));
                    }
                }),
                ex => callback(this, new AsyncResponseArgs<ApiStatus>(ex)),
                "status");
        }

        #endregion
#pragma warning restore 1591
    }
}
