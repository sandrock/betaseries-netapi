using System;
using Srk.BetaseriesApi.Clients;

namespace Srk.BetaseriesApi {

    partial class BetaseriesClientFactory {

        #region Client creation
        /*
        // <summary>
        // Create a factory from configuration.
        // </summary>
        // <param name="shareSessionToken">true will activate session token sharing for all clients created from this factory</param>
        // <returns></returns>
        public static BetaseriesClientFactory CreateFromConfiguration(bool shareSessionToken) {
            return new BetaseriesClientFactory(
                ConfigurationManager.AppSettings["BetaseriesApiKey"],
                ConfigurationManager.AppSettings["BetaseriesApiUserAgent"],
                shareSessionToken);
        }
        */
        /// <summary>
        /// Create a new client with the factory's configuration.
        /// </summary>
        /// <typeparam name="T">The client type you want.</typeparam>
        /// <returns></returns>
        public IBetaseriesApi CreateClient<T>() where T : BetaseriesBaseHttpClient {
            var o = Activator.CreateInstance(typeof(T), ApiKey, ApiUserAgent) as IBetaseriesApi;
            var b = o as BetaseriesBaseHttpClient;
            b.RegisterFactory(this, SessionToken, SessionUsername);
            return o;
        }

        /// <summary>
        /// Create a new client with the factory's configuration.
        /// Object type is <see cref="BetaseriesXmlClient"/>.
        /// </summary>
        /// <returns></returns>
        public IBetaseriesApi CreateDefaultClient() {
            var o = new BetaseriesXmlClient(ApiKey, ApiUserAgent);
            o.RegisterFactory(this, SessionToken, SessionUsername);
            return o;
        }

        #endregion

    }
}
