
namespace Srk.BetaseriesApiApp.CoreExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using Srk.BetaseriesApi;

    internal class ExtendedHttpRequestWrapper : HttpRequestWrapper, IHttpRequestWrapper
    {
        /// <summary>
        /// Default .ctor.
        /// </summary>
        /// <param name="urlFormat"></param>
        /// <param name="baseUrl"></param>
        /// <param name="userAgent"></param>
        public ExtendedHttpRequestWrapper(string urlFormat, string baseUrl, string userAgent)
            : base(urlFormat, baseUrl, userAgent)
        {
        }

        public string QueryString { get; private set; }

        public new string ExecuteQuery(string action, Dictionary<string, string> parameters)
        {
            string queryString = GetQueryString(action, parameters);
            this.QueryString = queryString;

            // prepare the web page we will be asking for
            HttpWebRequest request = PrepareRequest(queryString);

            // execute the request
            var stringResponse = GetStringResponse(request);

            return stringResponse;
        }

        public new string ExecuteQuery(string action, Dictionary<string, string> parameters, Dictionary<string, string> postParameters)
        {
            string queryString = GetQueryString(action, parameters);
            this.QueryString = queryString;

            // prepare POST data in memory
            var postDataStream = CreatePostStream(postParameters);

            // prepare the web page we will be asking for
            HttpWebRequest request = PrepareRequest(queryString);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // get request stream
            SendPostData(postDataStream, request);

            // execute the request
            var stringResponse = GetStringResponse(request);

            return stringResponse;
        }
    }
}
