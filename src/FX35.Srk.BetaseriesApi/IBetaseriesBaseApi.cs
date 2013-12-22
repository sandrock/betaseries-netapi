
namespace Srk.BetaseriesApi
{
    /// <summary>
    /// Internal interface for the betaseries.com API.
    /// This is supposed to be a base for multiple full implementations.
    /// </summary>
    public interface IBetaseriesBaseApi
    {

        /// <summary>
        /// Recommended field for application tracking.
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// API Key required for all queries.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Session username when using authenticated queries.
        /// </summary>
        string SessionUsername { get; }

        /// <summary>
        /// Session token when using authenticated queries.
        /// </summary>
        string SessionToken { get; }

        void SetSessionTokens(string newSessionToken, string newSessionUsername);
    }
}
