
namespace Srk.BetaseriesApi.Clients
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// This is a base partial Betaseries client implementation fitted for HTTP data transfert.
    /// This is to be subclassed and overriden.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public partial class BetaseriesBaseHttpClient : IBetaseriesApi, IDisposable
    {
        /// <summary>
        /// Base HTTP url for queries. 
        /// This will permit to use a different base adresse (for HTTPS, different port or domain name...).
        /// Default is http://api.betaseries.com/.
        /// </summary>
        /// <remarks>
        /// Value must be setted from .ctor.
        /// </remarks>
        protected readonly string BaseUrl;

        /// <summary>
        /// Formating string for query string.
        /// Must be set from sub-class.
        /// </summary>
        protected readonly string UrlFormat;

        private string userAgent;
        private string key;
        private string sessionToken;
        private string sessionUsername;

        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        public string Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public string SessionToken
        {
            get { return this.sessionToken; }
        }

        public string SessionUsername
        {
            get { return this.sessionUsername; }
        }

        public void SetSessionTokens(string newSessionToken, string newSessionUsername)
        {
            this.sessionToken = newSessionToken;
            this.sessionUsername = newSessionUsername;

            if (factory != null)
            {
                factory.RaiseSessionTokenChanged(this, this.sessionToken, this.sessionUsername);
            }
        }

        protected BetaseriesBaseHttpClient(string urlFormat, string apiKey)
            : this(urlFormat, apiKey, null, null)
        {
        }

        protected BetaseriesBaseHttpClient(string urlFormat, string apiKey, string userAgent)
            : this(urlFormat, apiKey, userAgent, null)
        {
        }

        /// <summary>
        /// Mandatory .ctor
        /// </summary>
        /// <param name="apiKey">Mandatory field. Ask for your key on the website.</param>
        /// <param name="userAgent"></param>
        /// <param name="baseUrl">If you want to override urls</param>
        /// <param name="urlFormat"></param>
        protected BetaseriesBaseHttpClient(string urlFormat, string apiKey, string userAgent, string baseUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Missing API Key", "apiKey");

            this.Key = apiKey;
            this.UserAgent = userAgent ?? "unknown-client";
            this.BaseUrl = baseUrl ?? "http://api.betaseries.com/";
            this.UrlFormat = urlFormat;
        }

        #region HTTP Query

        /// <summary>
        /// Execute an HTTP GET request through an HTTP wrapper.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="keyValues">Pairs of query string parameters (key1, value1, key2, value2...)</param>
        /// <returns>HTTP response body as a string.</returns>
        protected virtual string ExecuteQuery(string action, params string[] keyValues)
        {
            return this.ExecuteQuery(action, null, keyValues);
        }

        /// <summary>
        /// Execute an HTTP GET request through an HTTP wrapper.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        /// <returns>HTTP response body as a string.</returns>
        protected virtual string ExecuteQuery(string action, Dictionary<string, string> parameters)
        {
            return this.ExecuteQuery(action, null, parameters);
        }

        /// <summary>
        /// Execute an HTTP GET/POST request through an HTTP wrapper.
        /// Request will use a POST method if postParameters contains stuff.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="postParameters">POST data parameters (pass null for GET method)</param>
        /// <param name="keyValues">Pairs of query string parameters (key1, value1, key2, value2...)</param>
        /// <returns>HTTP response body as a string.</returns>
        protected virtual string ExecuteQuery(string action, Dictionary<string, string> postParameters, params string[] keyValues)
        {
            if (keyValues.Length % 2 != 0)
                throw new ArgumentException("Invalid parameters count", "keyvalues");

            Dictionary<string, string> parameters = null;
            if (keyValues.Length > 0)
            {
                parameters = new Dictionary<string, string>();
                bool isKey = true;
                string key = null;
                foreach (var item in keyValues)
                {
                    if (isKey)
                    {
                        key = item;
                    }
                    else
                    {
                        parameters.Add(key, item);
                    }

                    isKey = !isKey;
                }
            }

            return ExecuteQuery(action, postParameters, parameters);
        }

        /// <summary>
        /// Execute an HTTP GET/POST request through an HTTP wrapper.
        /// Request will use a POST method if postParameters contains stuff.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        /// <param name="postParameters">POST data parameters (pass null for GET method)</param>
        /// <returns>HTTP response body as a string.</returns>
        protected virtual string ExecuteQuery(string action, Dictionary<string, string> postParameters, Dictionary<string, string> parameters)
        {
            parameters = parameters ?? new Dictionary<string, string>();
            parameters["key"] = Key;
            if (SessionToken != null)
            {
                parameters["token"] = SessionToken;
            }

            if (postParameters != null && postParameters.Count > 0)
                return this.Http.ExecuteQuery(action, parameters, postParameters);
            else
                return this.Http.ExecuteQuery(action, parameters);
        }

        #endregion

#pragma warning disable 1591
        #region IBetaseriesApi Members

        public virtual IList<Show> SearchShows(string title)
        {
            throw new NotSupportedException();
        }

        public virtual Show GetShow(string url)
        {
            throw new NotSupportedException();
        }

        ////public virtual IList<Show> GetShows()
        ////{
        ////    throw new NotSupportedException();
        ////}

        public virtual IList<Episode> GetEpisodes(string showUrl)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Episode> GetEpisodes(string showUrl, uint season)
        {
            throw new NotSupportedException();
        }

        public virtual Episode GetEpisode(string showUrl, uint season, uint episode)
        {
            throw new NotSupportedException();
        }

        public virtual void AddShow(string showUrl)
        {
            throw new NotSupportedException();
        }

        public virtual void RemoveShow(string showUrl)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Subtitle> GetShowSubtitles(string showUrl, string language, uint? season, uint? episode)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Subtitle> GetLatestSubtitles()
        {
            throw new NotSupportedException();
        }

        public virtual IList<Subtitle> GetLatestSubtitles(string language)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Subtitle> GetLatestSubtitles(string language, uint number)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Subtitle> GetLatestShowSubtitles(string showUrl)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Subtitle> GetLatestShowSubtitles(string showUrl, string language)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Subtitle> GetLatestShowSubtitles(string showUrl, string language, uint number)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Episode> GetPlanning()
        {
            throw new NotSupportedException();
        }

        public virtual IList<Episode> GetMembersPlanning(string username)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Episode> GetMembersPlanning(string username, bool unseenOnly)
        {
            throw new NotSupportedException();
        }

        public virtual string Authenticate(string username, string password)
        {
            throw new NotSupportedException();
        }

        public virtual bool GetIsSessionActive()
        {
            throw new NotSupportedException();
        }

        public virtual void Logoff()
        {
            throw new NotSupportedException();
        }

        public virtual Member GetMember(string username)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Episode> GetMembersNextEpisodes(bool onlyNextEpisode)
        {
            throw new NotSupportedException();
        }

        public virtual Episode GetMembersNextShowEpisode(bool onlyNextEpisode, string showUrl)
        {
            throw new NotSupportedException();
        }

        public virtual void SetEpisodeAsSeen(string showUrl, uint season, uint episode, ushort? mark)
        {
            throw new NotSupportedException();
        }

        public virtual void SetEpisodeAsDownloaded(string showUrl, uint season, uint episode)
        {
            throw new NotSupportedException();
        }

        public virtual void SetEpisodeMark(string showUrl, uint season, uint episode, ushort mark)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Notification> GetNotifications(bool? seen, uint? count, uint? fromNotificationId)
        {
            throw new NotSupportedException();
        }

        public virtual void Signup(string username, string password, string email)
        {
            throw new NotSupportedException();
        }

        public virtual IList<string> GetFriends(string username)
        {
            throw new NotSupportedException();
        }

        public virtual string[] GetBadges(string username)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Comment> GetCommentsForShow(string showUrl)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Comment> GetCommentsForEpisode(string showUrl, uint season, uint episode)
        {
            throw new NotSupportedException();
        }

        public virtual IList<Comment> GetCommentsForMember(string username)
        {
            throw new NotSupportedException();
        }

        public virtual void CommentShow(string showUrl, string comment, uint? inReplyTo)
        {
            throw new NotSupportedException();
        }

        public virtual void CommentEpisode(string showUrl, uint season, uint episode, string comment, uint? inReplyTo)
        {
            throw new NotSupportedException();
        }

        public virtual void CommentMember(string username, string comment, uint? inReplyTo)
        {
            throw new NotSupportedException();
        }

        public virtual IList<TimelineItem> GetMainTimeline(uint? count)
        {
            throw new NotSupportedException();
        }

        public virtual IList<TimelineItem> GetFriendsTimeline(uint? count)
        {
            throw new NotSupportedException();
        }

        public virtual IList<TimelineItem> GetMemberTimeline(string username, uint? count)
        {
            throw new NotSupportedException();
        }

        public virtual ApiStatus GetStatus()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IBetaseriesAsyncApi Members

        public virtual void SearchShowsAsync(string title, AsyncResponseHandler<IList<Show>> callback)
        {
            this.ExecuteAsync(() => this.SearchShows(title), callback);
        }

        public virtual void GetShowAsync(string showUrl, AsyncResponseHandler<Show> callback)
        {
            this.ExecuteAsync(() => this.GetShow(showUrl), callback);
        }

        ////public virtual void GetShowsAsync(AsyncResponseHandler<IList<Show>> callback)
        ////{
        ////    this.ExecuteAsync(() => this.GetShows(), callback);
        ////}

        public virtual void GetEpisodesAsync(string showUrl, AsyncResponseHandler<IList<Episode>> callback)
        {
            this.ExecuteAsync(() => this.GetEpisodes(showUrl), callback);
        }

        public virtual void GetEpisodesAsync(string showUrl, uint season, AsyncResponseHandler<IList<Episode>> callback)
        {
            this.ExecuteAsync(() => this.GetEpisodes(showUrl, season), callback);
        }

        public virtual void GetEpisodeAsync(string showUrl, uint season, uint episode, AsyncResponseHandler<Episode> callback)
        {
            this.ExecuteAsync(() => this.GetEpisode(showUrl, season, episode), callback);
        }

        public virtual void AddShowAsync(string showUrl, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.AddShow(showUrl), callback);
        }

        public virtual void RemoveShowAsync(string showUrl, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.RemoveShow(showUrl), callback);
        }

        public virtual void GetShowSubtitlesAsync(string showUrl, string language, uint? season, uint? episode, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteAsync(() => this.GetShowSubtitles(showUrl, language, season, episode), callback);
        }

        public virtual void GetLatestSubtitlesAsync(AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteAsync(() => this.GetLatestSubtitles(), callback);
        }

        public virtual void GetLatestSubtitlesAsync(string language, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteAsync(() => this.GetLatestSubtitles(), callback);
        }

        public virtual void GetLatestSubtitlesAsync(string language, uint number, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteAsync(() => this.GetLatestSubtitles(language, number), callback);
        }

        public virtual void GetLatestShowSubtitlesAsync(string showUrl, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteAsync(() => this.GetLatestShowSubtitles(showUrl), callback);
        }

        public virtual void GetLatestShowSubtitlesAsync(string showUrl, string language, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteAsync(() => this.GetLatestShowSubtitles(showUrl, language), callback);
        }

        public virtual void GetLatestShowSubtitlesAsync(string showUrl, string language, uint number, AsyncResponseHandler<IList<Subtitle>> callback)
        {
            this.ExecuteAsync(() => this.GetLatestShowSubtitles(showUrl, language, number), callback);
        }

        public virtual void GetPlanningAsync(AsyncResponseHandler<IList<Episode>> callback)
        {
            this.ExecuteAsync(() => this.GetPlanning(), callback);
        }

        public virtual void GetMembersPlanningAsync(string username, AsyncResponseHandler<IList<Episode>> callback)
        {
            this.ExecuteAsync(() => this.GetMembersPlanning(username), callback);
        }

        public virtual void GetMembersPlanningAsync(string username, bool unseenOnly, AsyncResponseHandler<IList<Episode>> callback)
        {
            this.ExecuteAsync(() => this.GetMembersPlanning(username, unseenOnly), callback);
        }

        public virtual void AuthenticateAsync(string username, string password, AsyncResponseHandler<string> callback)
        {
            this.ExecuteAsync(() => this.Authenticate(username, password), callback);
        }

        public virtual void GetIsSessionActiveAsync(AsyncResponseHandler<bool> callback)
        {
            this.ExecuteAsync(() => this.GetIsSessionActive(), callback);
        }

        public virtual void LogoffAsync(AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.Logoff(), callback);
        }

        public virtual void GetMemberAsync(string username, AsyncResponseHandler<Member> callback)
        {
            this.ExecuteAsync(() => this.GetMember(username), callback);
        }

        public virtual void GetMembersNextEpisodesAsync(bool onlyNextEpisode, AsyncResponseHandler<IList<Episode>> callback)
        {
            this.ExecuteAsync(() => this.GetMembersNextEpisodes(onlyNextEpisode), callback);
        }

        public virtual void GetMembersNextShowEpisodeAsync(bool onlyNextEpisode, string showUrl, AsyncResponseHandler<Episode> callback)
        {
            this.ExecuteAsync(() => this.GetMembersNextShowEpisode(onlyNextEpisode, showUrl), callback);
        }

        public virtual void SetEpisodeAsSeenAsync(string showUrl, uint season, uint episode, ushort? mark, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.SetEpisodeAsSeen(showUrl, season, episode, mark), callback);
        }

        public virtual void SetEpisodeAsDownloadedAsync(string showUrl, uint season, uint episode, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.SetEpisodeAsDownloaded(showUrl, season, episode), callback);
        }

        public virtual void SetEpisodeMarkAsync(string showUrl, uint season, uint episode, ushort mark, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.SetEpisodeMark(showUrl, season, episode, mark), callback);
        }

        public virtual void GetNotificationsAsync(bool? seen, uint? count, uint? fromNotificationId, AsyncResponseHandler<IList<Notification>> callback)
        {
            this.ExecuteAsync(() => this.GetNotifications(seen, count, fromNotificationId), callback);
        }

        public virtual void SignupAsync(string username, string password, string email, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.Signup(username, password, email), callback);
        }

        public virtual void GetFriendsAsync(string username, AsyncResponseHandler<IList<string>> callback)
        {
            this.ExecuteAsync(() => this.GetFriends(username), callback);
        }

        public virtual void GetBadgesAsync(string username, AsyncResponseHandler<string[]> callback)
        {
            this.ExecuteAsync(() => this.GetBadges(username), callback);
        }

        public virtual void GetCommentsForShowAsync(string showUrl, AsyncResponseHandler<IList<Comment>> callback)
        {
            this.ExecuteAsync(() => this.GetCommentsForShow(showUrl), callback);
        }

        public virtual void GetCommentsForEpisodeAsync(string showUrl, uint season, uint episode, AsyncResponseHandler<IList<Comment>> callback)
        {
            this.ExecuteAsync(() => this.GetCommentsForEpisode(showUrl, season, episode), callback);
        }

        public virtual void GetCommentsForMemberAsync(string username, AsyncResponseHandler<IList<Comment>> callback)
        {
            this.ExecuteAsync(() => this.GetCommentsForMember(username), callback);
        }

        public virtual void CommentShowAsync(string showUrl, string comment, uint? inReplyTo, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.CommentShow(showUrl, comment, inReplyTo), callback);
        }

        public virtual void CommentEpisodeAsync(string showUrl, uint season, uint episode, string comment, uint? inReplyTo, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.CommentEpisode(showUrl, season, episode, comment, inReplyTo), callback);
        }

        public virtual void CommentMemberAsync(string username, string comment, uint? inReplyTo, AsyncResponseHandler callback)
        {
            this.ExecuteAsync(() => this.CommentMember(username, comment, inReplyTo), callback);
        }

        public virtual void GetMainTimelineAsync(uint? count, AsyncResponseHandler<IList<TimelineItem>> callback)
        {
            this.ExecuteAsync(() => this.GetMainTimeline(count), callback);
        }

        public virtual void GetFriendsTimelineAsync(uint? count, AsyncResponseHandler<IList<TimelineItem>> callback)
        {
            this.ExecuteAsync(() => this.GetFriendsTimeline(count), callback);
        }

        public virtual void GetMemberTimelineAsync(string username, uint? count, AsyncResponseHandler<IList<TimelineItem>> callback)
        {
            this.ExecuteAsync(() => this.GetMemberTimeline(username, count), callback);
        }

        public virtual void GetStatusAsync(AsyncResponseHandler<ApiStatus> callback)
        {
            this.ExecuteAsync(() => this.GetStatus(), callback);
        }

        #endregion
#pragma warning restore 1591

        #region Async tools

        private void ExecuteAsync<T>(Func<T> action, AsyncResponseHandler<T> callback)
        {
            ThreadPool.QueueUserWorkItem(param =>
            {
                try
                {
                    var result = action();
                    if (callback != null)
                        callback(this, new AsyncResponseArgs<T>(result));
                }
                catch (Exception ex)
                {
                    if (callback != null)
                        callback(this, new AsyncResponseArgs<T>(ex));
                }
            });
        }

        private void ExecuteAsync(Action action, AsyncResponseHandler callback)
        {
            ThreadPool.QueueUserWorkItem(param =>
            {
                try
                {
                    action();
                    if (callback != null)
                        callback(this, new AsyncResponseArgs(true));
                }
                catch (Exception ex)
                {
                    if (callback != null)
                        callback(this, new AsyncResponseArgs(ex));
                }
            });
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposing this object when no more needed will prevent memory leaks.
        /// </summary>
        /// <remarks>
        /// When overriding, don't forget to call the base method.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.factory != null)
                {
                    this.factory.SessionTokenChangedEvent -= this.factory_SessionTokenChangedEvent;
                    this.factory = null;
                }

                ClearEventHandlers();
            }
        }

        /// <summary>
        /// Set all event handlers to null. 
        /// </summary>
        /// <remarks>
        /// Using reflection is simplier because there are too much event handlers. 
        /// T4 would help.
        /// </remarks>
        private void ClearEventHandlers()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            EventInfo[] events = this.GetType().GetEvents(flags);
            if (events == null)
                return;
            if (events.Length < 1)
                return;

            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            for (int i = 0; i < events.Length; i++)
            {
                FieldInfo[] fields = events[i].DeclaringType.GetFields(flags);
                foreach (FieldInfo fi in fields)
                {
                    if (events[i].Name.Equals(fi.Name) && !ht.Contains(fi.Name))
                        ht.Add(fi.Name, fi);
                }
            }

            //TODO: finish that
        }

        #endregion

        protected static string HashString(string value)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(value);

            using (var x = new MD5CryptoServiceProvider())
            {
                data = x.ComputeHash(data);
            }

            string ret = "";
            for (int i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2").ToLower();
            return ret;
        }
    }
}
