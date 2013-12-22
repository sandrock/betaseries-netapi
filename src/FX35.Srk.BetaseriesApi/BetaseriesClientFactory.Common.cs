using System;
using Srk.BetaseriesApi.Clients;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// This class helps instanciating client classes.
    /// It also permits to share session tokens between clients.
    /// </summary>
    public partial class BetaseriesClientFactory {

        #region Properties

        /// <summary>
        /// This is the main factory instance.
        /// It's recommended to fill this property on application startup.
        /// </summary>
        public static BetaseriesClientFactory Default { get; set; }

        private string ApiKey;
        private string ApiUserAgent;
        private bool ShareSessionToken;

        internal event EventHandler<SessionTokenEventArgs> SessionTokenChangedEvent;

        #endregion

        #region .ctor

        /// <summary>
        /// Class .ctor to create a factory.
        /// </summary>
        /// <param name="apiKey">your API key (ask it on the website, don't use someone else's)</param>
        /// <param name="apiUserAgent">anything like MyBetaseriesApp/1.0.0.0 (name/version)</param>
        /// <param name="shareSessionToken">true will activate session token sharing for all clients created from this factory</param>
        public BetaseriesClientFactory(string apiKey, string apiUserAgent, bool shareSessionToken) {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Missing API key", "apiKey");
            if (string.IsNullOrEmpty(apiUserAgent))
                throw new ArgumentException("Missing UserAgent", "apiUserAgent");

            this.ApiKey = apiKey;
            this.ApiUserAgent = apiUserAgent;
            this.ShareSessionToken = shareSessionToken;
        }

        #endregion

        #region Session management

        /// <summary>
        /// Change session token for all clients created from this factory.
        /// If session token is share, you can call the same method on any client created from this factory.
        /// </summary>
        /// <param name="sessionToken"></param>
        /// <param name="sessionUsername"></param>
        public void SetSessionToken(string sessionToken, string sessionUsername) {
            if (!ShareSessionToken)
                return;

            RaiseSessionTokenChanged(null, sessionToken, sessionUsername);
        }

        internal void RaiseSessionTokenChanged(object sender, string sessionToken, string sessionUsername) {
            if (!ShareSessionToken)
                return;

            this.SessionToken = sessionToken;
            this.SessionUsername = sessionUsername;

            if (SessionTokenChangedEvent != null) {
                SessionTokenChangedEvent(this, new SessionTokenEventArgs(sender, sessionToken, sessionUsername));
            }
        }

        private string SessionToken { get; set; }
        private string SessionUsername { get; set; }

        #endregion

    }

    internal class SessionTokenEventArgs : EventArgs {

        public string NewSessionToken { get; private set; }
        public string NewSessionUsername { get; private set; }
        public object Sender { get; private set; }

        public SessionTokenEventArgs(object sender, string newSessionToken, string newSessionUsername)
            : base() {
            this.NewSessionToken = newSessionToken;
            this.NewSessionUsername = newSessionUsername;
            this.Sender = sender;
        }

    }
}
