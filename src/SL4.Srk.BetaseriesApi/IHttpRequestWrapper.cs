using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Interface to represent HTTP requests.
    /// </summary>
    public interface IHttpRequestWrapper {

        /// <summary>
        /// Download a response string.
        /// </summary>
        /// <param name="callback">delegate called when the query is succesful</param>
        /// <param name="errorCallback">delegate called when the query encoutered an error</param>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        void ExecuteQuery(AsyncCallback callback, Action<Exception> errorCallback, string action, Dictionary<string, string> parameters);

        /// <summary>
        /// Download a response string.
        /// </summary>
        /// <param name="callback">delegate called when the query is succesful</param>
        /// <param name="errorCallback">delegate called when the query encoutered an error</param>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        /// <param name="postParameters">POST data parameters</param>
        void ExecuteQuery(AsyncCallback callback, Action<Exception> errorCallback, string action, Dictionary<string, string> parameters, Dictionary<string, string> postParameters);

    }
}
