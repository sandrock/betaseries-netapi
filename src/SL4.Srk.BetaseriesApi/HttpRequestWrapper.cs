using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace Srk.BetaseriesApi {
    public partial class HttpRequestWrapper : IHttpRequestWrapper {

#pragma warning disable 1591
        public void ExecuteQuery(AsyncCallback callback, Action<Exception> errorCallback, string action, Dictionary<string, string> parameters) {
            string queryString = GetQueryString(action, parameters);

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(queryString);

            request.AllowAutoRedirect = false;
            request.UserAgent = UserAgent;

            // execute the request
            var r = request.BeginGetResponse(delegate(IAsyncResult @async) {

                try {
                    StringBuilder sb = new StringBuilder();
                    byte[] buf = new byte[8192];
                    HttpWebRequest request2 = (HttpWebRequest)@async.AsyncState;
                    HttpWebResponse response = null;

                    response = (HttpWebResponse)request2.EndGetResponse(@async);
                    HandleHttpCodes(response.StatusCode);

                    string stringResponse;

                    using (Stream resStream = response.GetResponseStream()) {
                        string tempString = null;
                        int count = 0;

                        do {
                            // fill the buffer with data
                            count = resStream.Read(buf, 0, buf.Length);

                            // make sure we read some data
                            if (count != 0) {
                                // translate from bytes to ASCII text
                                tempString = Encoding.UTF8.GetString(buf, 0, count);

                                // continue building the string
                                sb.Append(tempString);
                            }
                        }
                        while (count > 0); // any more data to read?

                        stringResponse = sb.ToString();
                    }

                    callback(stringResponse);
                } catch (Exception ex) {
                    errorCallback(ex);
                }

            }, request);
        }

        public void ExecuteQuery(AsyncCallback callback, Action<Exception> errorCallback, string action, Dictionary<string, string> parameters, Dictionary<string, string> postParameters) {
            string queryString = GetQueryString(action, parameters);

            // prepare POST data in memory
            var postDataStream = new MemoryStream();
            string sep = string.Empty;
            foreach (var item in postParameters) {
                var bytes = UTF8Encoding.UTF8.GetBytes(string.Concat(
                    item.Key, '=', PostEncode(item.Value)));
                postDataStream.Write(bytes, 0, bytes.Length);
            }
            postDataStream.Seek(0L, SeekOrigin.Begin);

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
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(queryString);

            request.AllowAutoRedirect = false;
            request.UserAgent = UserAgent;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // get request stream
            request.BeginGetRequestStream(getRequestResult => {
                try {
                    using (var requestStream = request.EndGetRequestStream(getRequestResult)) {
                        byte[] buffer = new byte[4096];
                        int bytesRead = 0;
                        while ((bytesRead = postDataStream.Read(buffer, 0, buffer.Length)) != 0) {
                            requestStream.Write(buffer, 0, bytesRead);
                            requestStream.Flush();
                        }
                        requestStream.Close();
                        postDataStream.Close();
                    }
                } catch (Exception ex) {
                    errorCallback(ex);
                    return;
                } finally {
                    postDataStream.Dispose();
                }

                // execute the request
                var r = request.BeginGetResponse(delegate(IAsyncResult @async) {
                    try {
                        StringBuilder sb = new StringBuilder();
                        byte[] buf = new byte[8192];
                        HttpWebRequest request2 = (HttpWebRequest)@async.AsyncState;
                        HttpWebResponse response = null;

                        response = (HttpWebResponse)request2.EndGetResponse(@async);
                        HandleHttpCodes(response.StatusCode);

                        string stringResponse;

                        using (Stream resStream = response.GetResponseStream()) {
                            string tempString = null;
                            int count = 0;

                            do {
                                // fill the buffer with data
                                count = resStream.Read(buf, 0, buf.Length);

                                // make sure we read some data
                                if (count != 0) {
                                    // translate from bytes to ASCII text
                                    tempString = Encoding.UTF8.GetString(buf, 0, count);

                                    // continue building the string
                                    sb.Append(tempString);
                                }
                            }
                            while (count > 0); // any more data to read?

                            stringResponse = sb.ToString();
                        }

                        callback(stringResponse);
                    } catch (Exception ex) {
                        errorCallback(ex);
                    }

                }, request);
            }, null);
        }
#pragma warning restore 1591

    }
}
