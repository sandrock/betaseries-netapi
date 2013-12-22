
namespace Srk.BetaseriesApi {

    /// <summary>
    /// API status class.
    /// </summary>
    public class ApiStatus {

        /// <summary>
        /// Expected values: ok
        /// </summary>
        public string WebsiteStatus { get; set; }

        /// <summary>
        /// Expected values: ok
        /// </summary>
        public string WebsiteDatabase { get; set; }

        /// <summary>
        /// Expected value: "N.n"
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// List of changes on the API.
        /// </summary>
        public ApiChange[] Changes { get; set; }

        /// <summary>
        /// List of files ????????
        /// </summary>
        public ApiFile[] Files { get; set; }

        /// <summary>
        /// API methods with dates.
        /// </summary>
        public ApiMethod[] Methods { get; set; }

    }

    /// <summary>
    /// To be used with <see cref="ApiChange"/>.
    /// </summary>
    public enum ApiChangeType {

        /// <summary>
        /// Default value if parsing fails.
        /// </summary>
        Unknown,
        
        /// <summary>
        /// This is a new thing.
        /// </summary>
        New,
        
        /// <summary>
        /// Something gets an update!
        /// </summary>
        Update
    }

    /// <summary>
    /// API change.
    /// </summary>
    public struct ApiChange {

        /// <summary>
        /// A date formated like "20100130".
        /// </summary>
        public int Date;

        /// <summary>
        /// Type of change.
        /// </summary>
        public ApiChangeType Type;

        /// <summary>
        /// API method name (like "shows/details").
        /// </summary>
        public string Action;
    }

    /// <summary>
    /// API Method.
    /// </summary>
    public struct ApiMethod {

        /// <summary>
        /// API method name (like "shows/details").
        /// </summary>
        public string Name;

        /// <summary>
        /// A date formated like "20100130".
        /// </summary>
        public int DateCreated;

        /// <summary>
        /// A date formated like "20100130".
        /// </summary>
        public int DateUpdated;
    }

    /// <summary>
    /// API file ???
    /// </summary>
    public struct ApiFile {

        /// <summary>
        /// File name.
        /// </summary>
        public string Name;

        /// <summary>
        /// UNIX timestamp.
        /// </summary>
        public long Date;
    }

}
