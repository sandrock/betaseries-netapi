using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Srk.BetaseriesApi.Clients {
    partial class BetaseriesBaseHttpClient {

        /// <summary>
        /// This is a http query wrapper. Use for unit testing only.
        /// </summary>
        protected IHttpRequestWrapper http {
            get { return _http ?? (_http = new HttpRequestWrapper(UrlFormat, BaseUrl, RealUserAgent)); }
            set { _http = value; }
        }
        private IHttpRequestWrapper _http;

        private string RealUserAgent {
            get { return string.Format("{0} {1}", Version.LibraryUserAgent, UserAgent); }
        }

        #region Error handling

        /// <summary>
        /// Handle a custom error via <see cref="HandleCustomError"/>.
        /// The throws an exception if the parameter is not null.
        /// </summary>
        /// <param name="exception"></param>
        protected void HandleError(Exception exception) {
            HandleCustomError(exception);
            if (exception is WebException) {
                throw new ServiceException("The service does not seem available. Check your Internet connection and the service status. ");
            }
            throw exception;
        }

        /// <summary>
        /// Empty overridable method to handle custom errors.
        /// </summary>
        /// <param name="exception"></param>
        protected virtual void HandleCustomError(Exception exception) {

        }

        /// <summary>
        /// Encapsulates a <see cref="BetaError"/> into an exception.
        /// Do nothing if error is null.
        /// </summary>
        /// <param name="error"></param>
        protected static void HandleError(BetaError error) {
            if (error == null)
                return;

            throw new BetaException(error);
        }

        /// <summary>
        /// Encapsulates 1 or many <see cref="BetaError"/>s into an exception.
        /// Do nothing if errors is null.
        /// </summary>
        /// <param name="errors"></param>
        protected static void HandleError(IEnumerable<BetaError> errors) {
            if (errors == null)
                return;

            //TODO: this will save only 1 error if there are many
            HandleError(errors.FirstOrDefault());
        }

        #endregion

        #region Factory and session sharing

        private BetaseriesClientFactory factory;

        internal void RegisterFactory(BetaseriesClientFactory factory, string sessionToken, string sessionUsername) {
            this.factory = factory;

            factory.SessionTokenChangedEvent += factory_SessionTokenChangedEvent;

            _sessionToken = sessionToken;
            _sessionUsername = sessionUsername;
        }

        private void factory_SessionTokenChangedEvent(object sender, SessionTokenEventArgs e) {
            if (e.Sender != this) {
                _sessionToken = e.NewSessionToken;
                _sessionUsername = e.NewSessionUsername;
            }
        }

        #endregion

        #region Cache

        /// <summary>
        /// Let's you enable a static memory cache.
        /// Very simple implementation, not recommanded for a heavy scenario.
        /// </summary>
        public bool IsCacheEnabled {
            get { return _isCacheEnabled; }
            set { _isCacheEnabled = value; }
        }
        private bool _isCacheEnabled = true;
        
        /// <summary>
        /// Clears the static memory cache.
        /// This will not disable caching.
        /// </summary>
        public static void ClearCache() {
            _showsCache.Clear();
            _showSearchesCache.Clear();
        }

        #region Show searches

        private static readonly Dictionary<string, IList<Show>> _showSearchesCache = new Dictionary<string, IList<Show>>();

        protected IList<Show> SearchShowsFromCache(string title) {
            if (_showSearchesCache == null || !IsCacheEnabled)
                return null;
            return _showSearchesCache.FirstOrDefault(i => i.Key == title).Value;
        }

        protected static void SearchShowsToCache(string title, IList<Show> shows) {
            _showSearchesCache[title] = shows;
        }

        #endregion

        #region Shows

        private static readonly List<Show> _showsCache = new List<Show>();

        /// <summary>
        /// Returns a show from the cache.
        /// Returns null is cache is disabled.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected Show GetShowFromCache(string url) {
            if (_showsCache == null || !IsCacheEnabled)
                return null;
            return _showsCache.FirstOrDefault(s => s.Url == url);
        }

        /// <summary>
        /// Remove a show from the cache.
        /// </summary>
        /// <param name="url"></param>
        protected void RemoveShowFromCache(string url) {
            if (_showsCache == null || _showsCache.Count == 0)
                return;

            var existing = _showsCache.FirstOrDefault(s => s.Url == url);

            if (existing != null)
                _showsCache.Remove(existing);

            if (_episodeListsCache == null || _episodeListsCache.Count == 0)
                return;

            if (_episodeListsCache.ContainsKey(url))
                _episodeListsCache.Remove(url);
        }

        /// <summary>
        /// Add a show to cache and remove existing one.
        /// Returns null is cache is disabled.
        /// </summary>
        /// <param name="show"></param>
        protected void GetShowToCache(Show show) {
            var existing = GetShowFromCache(show.Url);
            if (existing != null)
                _showsCache.Remove(existing);
            _showsCache.Add(show);
        }

        protected void ClearShowsCache() {
            _showsCache.Clear();
        }

        #endregion

        #region Episode lists

        private readonly static Dictionary<string, IList<Episode>> _episodeListsCache = new Dictionary<string, IList<Episode>>();

        protected IList<Episode> GetEpisodesFromCache(string showUrl) {
            if (_episodeListsCache == null || !IsCacheEnabled)
                return null;
            return _episodeListsCache.FirstOrDefault(i => i.Key == showUrl).Value;
        }

        protected static void GetEpisodesToCache(string showUrl, IList<Episode> episodes) {
            _episodeListsCache[showUrl] = episodes;
        }

        #endregion

        #region Episodes

        private readonly static Dictionary<string, Episode> _episodesCache = new Dictionary<string, Episode>();

        protected Episode GetEpisodeFromCache(string showUrl, string number) {
            string key = string.Concat(showUrl, "/", number);

            if (_episodesCache == null || !IsCacheEnabled || !_episodesCache.ContainsKey(key))
                return null;
            return _episodesCache[key];
        }

        protected static void GetEpisodeToCache(string showUrl, string number, Episode episode) {
            string key = string.Concat(showUrl, "/", number);

            _episodesCache[key] = episode;
        }

        protected void ClearEpisodeFromCache(string showUrl, string number) {
            string key = string.Concat(showUrl, "/", number);

            if (_episodesCache.ContainsKey(key))
                _episodesCache.Remove(key);
        }

        #endregion

        #endregion


    }
}
