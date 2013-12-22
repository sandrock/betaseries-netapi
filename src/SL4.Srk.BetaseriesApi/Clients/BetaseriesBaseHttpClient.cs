using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Srk.BetaseriesApi.Clients {

    /// <summary>
    /// This is a base partial Betaseries client implementation fitted for HTTP data transfert.
    /// This is to be subclassed and overriden.
    /// </summary>
    public abstract partial class BetaseriesBaseHttpClient : IBetaseriesApi, IDisposable {

        #region Properties

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

#pragma warning disable 1591
        #region IBetaseriesBaseApi Members

        public string UserAgent {
            get { return _userAgent; }
            set { _userAgent = value; }
        }
        private string _userAgent;

        public string Key {
            get { return _key; }
            set { _key = value; }
        }
        private string _key;

        public string SessionToken {
            get { return _sessionToken; }
        }
        private string _sessionToken;

        public string SessionUsername {
            get { return _sessionUsername; }
        }
        private string _sessionUsername;

        public void SetSessionTokens(string newSessionToken, string newSessionUsername) {
            _sessionToken = newSessionToken;
            _sessionUsername = newSessionUsername;

            if (factory != null) {
                factory.RaiseSessionTokenChanged(this, _sessionToken, _sessionUsername);
            }
        }

        #endregion
#pragma warning restore 1591

        #endregion

        #region .ctor

        /// <summary>
        /// Protected class .ctor.
        /// </summary>
        /// <param name="urlFormat">is something like "{0}{1}.xml?{2}" (0 is the BaseUrl, 1 is the action, 2 is for query parameters)</param>
        /// <param name="apiKey">your API key (ask it on the website, don't use someone else's)</param>
        protected BetaseriesBaseHttpClient(string urlFormat, string apiKey) : this(urlFormat, apiKey, null, null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlFormat">is something like "{0}{1}.xml?{2}" (0 is the BaseUrl, 1 is the action, 2 is for query parameters)</param>
        /// <param name="apiKey">your API key (ask it on the website, don't use someone else's)</param>
        /// <param name="userAgent">anything like MyBetaseriesApp/1.0.0.0 (name/version)</param>
        protected BetaseriesBaseHttpClient(string urlFormat, string apiKey, string userAgent) : this(urlFormat, apiKey, userAgent, null) { }

        /// <summary>
        /// Mandatory .ctor
        /// </summary>
        /// <param name="urlFormat">is something like "{0}{1}.xml?{2}" (0 is the BaseUrl, 1 is the action, 2 is for query parameters)</param>
        /// <param name="apiKey">your API key (ask it on the website, don't use someone else's)</param>
        /// <param name="userAgent">anything like MyBetaseriesApp/1.0.0.0 (name/version)</param>
        /// <param name="baseUrl">If you want to override urls (nice for https)</param>
        protected BetaseriesBaseHttpClient(string urlFormat, string apiKey, string userAgent, string baseUrl) {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Missing API Key", "apiKey");
            Key = apiKey;
            UserAgent = userAgent ?? "unknown-client";
            BaseUrl = baseUrl ?? "http://api.betaseries.com/";
            UrlFormat = urlFormat;
        }

        #endregion

        #region HTTP Query

        /// <summary>
        /// Execute an HTTP GET request through an HTTP wrapper.
        /// </summary>
        /// <param name="callback">delegate called when the query is succesful</param>
        /// <param name="errorCallback">delegate called when the query encoutered an error</param>
        /// <param name="action">Service action</param>
        /// <param name="keyValues">Pairs of query string parameters (key1, value1, key2, value2...)</param>
        /// <returns>HTTP response body as a string.</returns>
        protected virtual void ExecuteQuery(AsyncCallback callback, Action<Exception> errorCallback, string action, params string[] keyValues) {
            ExecuteQuery(callback, errorCallback, action, null, keyValues);
        }

        /// <summary>
        /// Execute an HTTP GET request through an HTTP wrapper.
        /// </summary>
        /// <param name="callback">delegate called when the query is succesful</param>
        /// <param name="errorCallback">delegate called when the query encoutered an error</param>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        protected virtual void ExecuteQuery(AsyncCallback callback, Action<Exception> errorCallback, string action, Dictionary<string, string> parameters) {
            ExecuteQuery(callback, errorCallback, action, null, parameters);
        }

        /// <summary>
        /// Execute an HTTP GET/POST request through an HTTP wrapper.
        /// Request will use a POST method if postParameters contains stuff.
        /// </summary>
        /// <param name="callback">delegate called when the query is succesful</param>
        /// <param name="errorCallback">delegate called when the query encoutered an error</param>
        /// <param name="action">Service action</param>
        /// <param name="postParameters">POST data parameters (pass null for GET method)</param>
        /// <param name="keyValues">Pairs of query string parameters (key1, value1, key2, value2...)</param>
        /// <returns>HTTP response body as a string.</returns>
        protected virtual void ExecuteQuery(AsyncCallback callback, Action<Exception> errorCallback, string action, Dictionary<string, string> postParameters, params string[] keyValues) {
            if (keyValues.Length % 2 != 0)
                throw new ArgumentException("Invalid parameters count", "keyvalues");
            Dictionary<string, string> parameters = null;
            if (keyValues.Length > 0) {
                parameters = new Dictionary<string, string>();
                bool isKey = true;
                string key = null;
                foreach (var item in keyValues) {
                    if (isKey) {
                        key = item;
                    } else {
                        parameters.Add(key, item);
                    }
                    isKey = !isKey;
                }
            }
            ExecuteQuery(callback, errorCallback, action, postParameters, parameters);
        }

        /// <summary>
        /// Execute an HTTP GET/POST request through an HTTP wrapper.
        /// Request will use a POST method if postParameters contains stuff.
        /// </summary>
        /// <param name="callback">delegate called when the query is succesful</param>
        /// <param name="errorCallback">delegate called when the query encoutered an error</param>
        /// <param name="action">Service action</param>
        /// <param name="postParameters">POST data parameters (pass null for GET method)</param>
        /// <param name="parameters">Query string parameters</param>
        protected virtual void ExecuteQuery(AsyncCallback callback, Action<Exception> errorCallback, string action, Dictionary<string, string> postParameters, Dictionary<string, string> parameters) {
            parameters = parameters ?? new Dictionary<string, string>();
            parameters["key"] = Key;
            if (SessionToken != null) {
                parameters["token"] = SessionToken;
            }

            if (postParameters != null && postParameters.Count > 0)
                http.ExecuteQuery(callback, errorCallback, action, parameters, postParameters);
            else
                http.ExecuteQuery(callback, errorCallback, action, parameters);
        }

        #endregion

#pragma warning disable 1591
        #region IBetaseriesApi Members

        public abstract void SearchShowsAsync(string title, AsyncResponseHandler<IList<Show>> callback);

        public abstract void GetShowAsync(string showUrl, AsyncResponseHandler<Show> callback);

        ////public abstract void GetShowsAsync(AsyncResponseHandler<IList<Show>> callback);

        public abstract void GetEpisodesAsync(string showUrl, AsyncResponseHandler<IList<Episode>> callback);

        public abstract void GetEpisodesAsync(string showUrl, uint season, AsyncResponseHandler<IList<Episode>> callback);

        public abstract void GetEpisodeAsync(string showUrl, uint season, uint episode, AsyncResponseHandler<Episode> callback);

        public abstract void AddShowAsync(string showUrl, AsyncResponseHandler callback);

        public abstract void RemoveShowAsync(string showUrl, AsyncResponseHandler callback);

        public abstract void GetShowSubtitlesAsync(string showUrl, string language, uint? season, uint? episode, AsyncResponseHandler<IList<Subtitle>> callback);

        public abstract void GetLatestSubtitlesAsync(AsyncResponseHandler<IList<Subtitle>> callback);

        public abstract void GetLatestSubtitlesAsync(string language, AsyncResponseHandler<IList<Subtitle>> callback);

        public abstract void GetLatestSubtitlesAsync(string language, uint number, AsyncResponseHandler<IList<Subtitle>> callback);

        public abstract void GetLatestShowSubtitlesAsync(string showUrl, AsyncResponseHandler<IList<Subtitle>> callback);

        public abstract void GetLatestShowSubtitlesAsync(string showUrl, string language, AsyncResponseHandler<IList<Subtitle>> callback);

        public abstract void GetLatestShowSubtitlesAsync(string showUrl, string language, uint number, AsyncResponseHandler<IList<Subtitle>> callback);

        public abstract void GetPlanningAsync(AsyncResponseHandler<IList<Episode>> callback);

        public abstract void GetMembersPlanningAsync(string username, AsyncResponseHandler<IList<Episode>> callback);

        public abstract void GetMembersPlanningAsync(string username, bool unseenOnly, AsyncResponseHandler<IList<Episode>> callback);

        public abstract void AuthenticateAsync(string username, string password, AsyncResponseHandler<string> callback);

        public abstract void GetIsSessionActiveAsync(AsyncResponseHandler<bool> callback);

        public abstract void LogoffAsync(AsyncResponseHandler callback);

        public abstract void GetMemberAsync(string username, AsyncResponseHandler<Member> callback);

        public abstract void GetMembersNextEpisodesAsync(bool onlyNextEpisode, AsyncResponseHandler<IList<Episode>> callback);

        public abstract void GetMembersNextShowEpisodeAsync(bool onlyNextEpisode, string showUrl, AsyncResponseHandler<Episode> callback);

        public abstract void SetEpisodeAsSeenAsync(string showUrl, uint season, uint episode, ushort? mark, AsyncResponseHandler callback);

        public abstract void SetEpisodeAsDownloadedAsync(string showUrl, uint season, uint episode, AsyncResponseHandler callback);

        public abstract void SetEpisodeMarkAsync(string showUrl, uint season, uint episode, ushort mark, AsyncResponseHandler callback);

        public abstract void GetNotificationsAsync(bool? seen, uint? count, uint? fromNotificationId, AsyncResponseHandler<IList<Notification>> callback);

        public abstract void SignupAsync(string username, string password, string email, AsyncResponseHandler callback);

        public abstract void GetFriendsAsync(string username, AsyncResponseHandler<IList<string>> callback);

        public abstract void GetBadgesAsync(string username, AsyncResponseHandler<string[]> callback);

        public abstract void GetCommentsForShowAsync(string showUrl, AsyncResponseHandler<IList<Comment>> callback);

        public abstract void GetCommentsForEpisodeAsync(string showUrl, uint season, uint episode, AsyncResponseHandler<IList<Comment>> callback);

        public abstract void GetCommentsForMemberAsync(string username, AsyncResponseHandler<IList<Comment>> callback);

        public abstract void CommentShowAsync(string showUrl, string comment, uint? inReplyTo, AsyncResponseHandler callback);

        public abstract void CommentEpisodeAsync(string showUrl, uint season, uint episode, string comment, uint? inReplyTo, AsyncResponseHandler callback);

        public abstract void CommentMemberAsync(string username, string comment, uint? inReplyTo, AsyncResponseHandler callback);

        public abstract void GetMainTimelineAsync(uint? count, AsyncResponseHandler<IList<TimelineItem>> callback);

        public abstract void GetFriendsTimelineAsync(uint? count, AsyncResponseHandler<IList<TimelineItem>> callback);

        public abstract void GetMemberTimelineAsync(string username, uint? count, AsyncResponseHandler<IList<TimelineItem>> callback);

        public abstract void GetStatusAsync(AsyncResponseHandler<ApiStatus> callback);

        #endregion
#pragma warning restore 1591

        #region Async tools

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposing this object when no more needed will prevent memory leaks.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing this object when no more needed will prevent memory leaks.
        /// </summary>
        /// <remarks>
        /// When overriding, don't forget to call the base method and to clean only if disposing is true.
        /// </remarks>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (factory != null) {
                    factory.SessionTokenChangedEvent -= factory_SessionTokenChangedEvent;
                    factory = null;
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
        private void ClearEventHandlers() {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            EventInfo[] events = this.GetType().GetEvents(flags);
            if (events == null)
                return;
            if (events.Length < 1)
                return;

            var ht = new Dictionary<string, FieldInfo>();

            for (int i = 0; i < events.Length; i++) {
                FieldInfo[] fields = events[i].DeclaringType.GetFields(flags);
                foreach (FieldInfo fi in fields) {
                    if (events[i].Name.Equals(fi.Name) && !ht.ContainsKey(fi.Name))
                        ht.Add(fi.Name, fi);
                }
            }

            //TODO: finish that
        }

        #endregion

        /// <summary>
        /// For the authentication method, user passwords must be hashed with MD5.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static string HashString(string value) {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(value);
            byte[] hash = null;
            using (MD5Managed md5 = new MD5Managed()) {
                hash = md5.ComputeHash(bs);
            }

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash) {
                sb.Append(b.ToString("x2").ToLower());
            }

            return sb.ToString();
        }
    }
}
