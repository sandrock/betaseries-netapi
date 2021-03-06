﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Srk.BetaseriesApi {
    public interface IHttpRequestWrapper {

        /// <summary>
        /// Download a response string.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        /// <returns>HTTP response body as a string.</returns>
        string ExecuteQuery(string action, Dictionary<string, string> parameters);

        /// <summary>
        /// Download a response string with POST data.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        /// <param name="postParameters">POST data parameters</param>
        /// <returns>HTTP response body as a string.</returns>
        string ExecuteQuery(string action, Dictionary<string, string> parameters, Dictionary<string, string> postParameters);

    }
}
