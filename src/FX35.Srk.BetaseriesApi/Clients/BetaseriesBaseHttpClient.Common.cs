
namespace Srk.BetaseriesApi.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    partial class BetaseriesBaseHttpClient
    {
        private BetaseriesClientFactory factory;
        private bool isCacheEnabled = true;

        /// <summary>
        /// This is a http query wrapper. Use for unit testing only.
        /// </summary>
        protected IHttpRequestWrapper http
        {
            get { return this.http ?? (this.http = new HttpRequestWrapper(this.UrlFormat, this.BaseUrl, this.RealUserAgent)); }
            set { http = value; }
        }

        private string RealUserAgent
        {
            get { return string.Format("{0} {1}", Srk.BetaseriesApi.Version.LibraryUserAgent, this.UserAgent); }
        }

        #region Error handling

        /// <summary>
        /// Handle a custom error via <see cref="HandleCustomError"/>.
        /// The throws an exception if the parameter is not null.
        /// </summary>
        /// <param name="exception"></param>
        protected void HandleError(Exception exception)
        {
            HandleCustomError(exception);
            if (exception is WebException)
            {
                throw new ServiceException("The service does not seem available. Check your Internet connection and the service status. ");
            }

            throw exception;
        }

        /// <summary>
        /// Empty overridable method to handle custom errors.
        /// </summary>
        /// <param name="exception"></param>
        protected virtual void HandleCustomError(Exception exception)
        {
        }

        /// <summary>
        /// Encapsulates a <see cref="BetaError"/> into an exception.
        /// Do nothing if error is null.
        /// </summary>
        /// <param name="error"></param>
        protected static void HandleError(BetaError error)
        {
            if (error == null)
                return;

            throw new BetaException(error);
        }

        /// <summary>
        /// Encapsulates 1 or many <see cref="BetaError"/>s into an exception.
        /// Do nothing if errors is null.
        /// </summary>
        /// <param name="errors"></param>
        protected static void HandleError(IEnumerable<BetaError> errors)
        {
            if (errors == null)
                return;

            //TODO: this will save only 1 error if there are many
            HandleError(errors.FirstOrDefault());
        }

        #endregion

        #region Factory and session sharing


        internal void RegisterFactory(BetaseriesClientFactory factory, string sessionToken, string sessionUsername)
        {
            this.factory = factory;

            this.factory.SessionTokenChangedEvent += this.factory_SessionTokenChangedEvent;

            this.sessionToken = sessionToken;
            this.sessionUsername = sessionUsername;
        }

        private void factory_SessionTokenChangedEvent(object sender, SessionTokenEventArgs e)
        {
            if (e.Sender != this)
            {
                this.sessionToken = e.NewSessionToken;
                this.sessionUsername = e.NewSessionUsername;
            }
        }

        #endregion

        #region Cache

        /// <summary>
        /// Let's you enable a static memory cache.
        /// Very simple implementation, not recommanded for a heavy scenario.
        /// </summary>
        public bool IsCacheEnabled
        {
            get { return this.isCacheEnabled; }
            set { this.isCacheEnabled = value; }
        }

        /// <summary>
        /// Clears the static memory cache.
        /// This will not disable caching.
        /// </summary>
        public static void ClearCache()
        {
            showsCache.Clear();
            showSearchesCache.Clear();
        }

        #region Show searches

        private static readonly Dictionary<string, IList<Show>> showSearchesCache = new Dictionary<string, IList<Show>>();

        protected IList<Show> SearchShowsFromCache(string title)
        {
            if (showSearchesCache == null || !this.IsCacheEnabled)
                return null;
            return showSearchesCache.FirstOrDefault(i => i.Key == title).Value;
        }

        protected static void SearchShowsToCache(string title, IList<Show> shows)
        {
            showSearchesCache[title] = shows;
        }

        #endregion

        #region Shows

        private static readonly List<Show> showsCache = new List<Show>();

        /// <summary>
        /// Returns a show from the cache.
        /// Returns null is cache is disabled.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected Show GetShowFromCache(string url)
        {
            if (showsCache == null || !this.IsCacheEnabled)
                return null;
            return showsCache.FirstOrDefault(s => s.Url == url);
        }

        /// <summary>
        /// Remove a show from the cache.
        /// </summary>
        /// <param name="url"></param>
        protected void RemoveShowFromCache(string url)
        {
            if (showsCache == null || showsCache.Count == 0)
                return;

            var existing = showsCache.FirstOrDefault(s => s.Url == url);

            if (existing != null)
                showsCache.Remove(existing);

            if (episodeListsCache == null || episodeListsCache.Count == 0)
                return;

            if (episodeListsCache.ContainsKey(url))
                episodeListsCache.Remove(url);
        }

        /// <summary>
        /// Add a show to cache and remove existing one.
        /// Returns null is cache is disabled.
        /// </summary>
        /// <param name="show"></param>
        protected void GetShowToCache(Show show)
        {
            var existing = GetShowFromCache(show.Url);
            if (existing != null)
                showsCache.Remove(existing);
            showsCache.Add(show);
        }

        protected void ClearShowsCache()
        {
            showsCache.Clear();
        }

        #endregion

        #region Episode lists

        private readonly static Dictionary<string, IList<Episode>> episodeListsCache = new Dictionary<string, IList<Episode>>();

        protected IList<Episode> GetEpisodesFromCache(string showUrl)
        {
            if (episodeListsCache == null || !this.IsCacheEnabled)
                return null;
            return episodeListsCache.FirstOrDefault(i => i.Key == showUrl).Value;
        }

        protected static void GetEpisodesToCache(string showUrl, IList<Episode> episodes)
        {
            episodeListsCache[showUrl] = episodes;
        }

        #endregion

        #region Episodes

        private readonly static Dictionary<string, Episode> episodesCache = new Dictionary<string, Episode>();

        protected Episode GetEpisodeFromCache(string showUrl, string number)
        {
            string key = string.Concat(showUrl, "/", number);

            if (episodesCache == null || !this.IsCacheEnabled || !episodesCache.ContainsKey(key))
                return null;
            return episodesCache[key];
        }

        protected static void GetEpisodeToCache(string showUrl, string number, Episode episode)
        {
            string key = string.Concat(showUrl, "/", number);

            episodesCache[key] = episode;
        }

        protected void ClearEpisodeFromCache(string showUrl, string number)
        {
            string key = string.Concat(showUrl, "/", number);

            if (episodesCache.ContainsKey(key))
                episodesCache.Remove(key);
        }

        #endregion

        #endregion
    }
}
