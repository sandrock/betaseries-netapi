
namespace Srk.BetaseriesApiApp {

    /// <summary>
    /// Basic class to keep track of the current version.
    /// </summary>
    public static class AppVersion {

        /// <summary>
        /// The application internal name.
        /// </summary>
        internal const string ApplicationName = "Srk.BetaseriesApiApp";

        /// <summary>
        /// Current version of this application.
        /// </summary>
        //VERSION: version number is hard-coded
        public const string ApplicationVersion = "0.5.0.0";

        /// <summary>
        /// Computed user-agent for this app.
        /// </summary>
        public static readonly string ApplicationUserAgent = ApplicationName + "/" + ApplicationVersion;

    }
}
