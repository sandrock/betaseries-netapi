using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace Srk.BetaseriesApi {
    public partial class HttpRequestWrapper : IHttpRequestWrapper {

        /// <summary>
        /// Download a response string.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        /// <returns>HTTP response body as a string.</returns>
        public string ExecuteQuery(string action, Dictionary<string, string> parameters) {
            string queryString = GetQueryString(action, parameters);

            // prepare the web page we will be asking for
            HttpWebRequest request = PrepareRequest(queryString);

            // execute the request
            var stringResponse = GetStringResponse(request);

            return stringResponse;
        }

        /// <summary>
        /// Download a response string with POST data.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        /// <param name="postParameters">POST data parameters</param>
        /// <returns>HTTP response body as a string.</returns>
        public string ExecuteQuery(string action, Dictionary<string, string> parameters, Dictionary<string, string> postParameters) {
            string queryString = GetQueryString(action, parameters);

            // prepare POST data in memory
            var postDataStream = CreatePostStream(postParameters);

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached && false) {
                string postString = null;
                using (var sr = new StreamReader(postDataStream)) {
                    postString = sr.ReadToEnd();
                }
                postDataStream.Seek(0L, SeekOrigin.Begin);
                System.Diagnostics.Debugger.Break();
            }
#endif

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

        protected static string GetStringResponse(HttpWebRequest request) {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            HandleHttpCodes(response.StatusCode);

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            int count = 0;

            do {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0) {
                    // translate from bytes to ASCII text
                    sb.Append(Encoding.UTF8.GetString(buf, 0, count));
                }
            }
            while (count > 0); // any more data to read?

            var stringResponse = sb.ToString();
            return stringResponse;
        }

        protected static void SendPostData(MemoryStream postDataStream, HttpWebRequest request) {
            using (var requestStream = request.GetRequestStream()) {
                try {
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    while ((bytesRead = postDataStream.Read(buffer, 0, buffer.Length)) != 0) {
                        requestStream.Write(buffer, 0, bytesRead);
                        requestStream.Flush();
                    }
                    requestStream.Close();
                    postDataStream.Close();
                } catch {
                    throw;
                } finally {
                    postDataStream.Dispose();
                }
            }
        }

        protected HttpWebRequest PrepareRequest(string queryString) {
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(new Uri(queryString, UriKind.Absolute));
            request.AllowAutoRedirect = false;
            request.UserAgent = UserAgent;
            return request;
        }

        protected static MemoryStream CreatePostStream(Dictionary<string, string> postParameters) {
            var postDataStream = new MemoryStream();
            string sep = string.Empty;
            foreach (var item in postParameters) {
                var bytes = UTF8Encoding.UTF8.GetBytes(string.Concat(
                    item.Key, '=', PostEncode(item.Value)));
                postDataStream.Write(bytes, 0, bytes.Length);
            }
            postDataStream.Seek(0L, SeekOrigin.Begin);
            return postDataStream;
        }

    }
}
