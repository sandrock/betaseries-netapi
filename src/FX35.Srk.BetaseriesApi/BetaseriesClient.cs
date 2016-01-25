
namespace Srk.BetaseriesApi2
{
    using Srk.BetaseriesApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    partial class BetaseriesClient
    {
        private string apiKey;
        private IHttpRequestWrapper _http;

        private string UrlFormat = "{0}{1}?{2}";
        private string BaseUrl = "https://api.betaseries.com/";

        public BetaseriesClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public string UserAgent { get; set; }

        private string RealUserAgent
        {
            get { return string.Format("{0} {1}", Srk.BetaseriesApi.Version.LibraryUserAgent, UserAgent ?? "unknown app"); }
        }

        /// <summary>
        /// This is a http query wrapper. Use for unit testing only.
        /// </summary>
        protected IHttpRequestWrapper http
        {
            get { return _http ?? (_http = new HttpRequestWrapper(UrlFormat, BaseUrl, RealUserAgent)); }
            set { _http = value; }
        }

        internal virtual string ExecuteQuery(RequestContext request)
        {
            request.AddUrlArgumentToUrlQueryString("key", this.apiKey);

            string result;
            if (request.PostQueryStrings != null && request.PostQueryStrings.Count > 0)
            {
                result = this.http.ExecuteQuery(
                    request.UrlPath,
                    request.QueryStrings,
                    request.PostQueryStrings,
                    method: request.Method);
            }
            else
            {
                result = this.http.ExecuteQuery(
                    request.UrlPath,
                    request.QueryStrings,
                    method: request.Method);
            }

            return result;
        }

        internal void HandleErrors<T>(BaseResponse<T> result)
        {
            if (result.Errors == null || result.Errors.Length == 0)
                return;

            var first = result.Errors[0];
            throw new Exception(first.Code + " " + first.Content);
        }

        internal void HandleErrors(BaseResponse result)
        {
            if (result.Errors == null || result.Errors.Length == 0)
                return;

            var first = result.Errors[0];
            throw new Exception(first.Code + " " + first.Content);
        }

        internal string ApplyMD5(string value)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(value);

            using (var x = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                data = x.ComputeHash(data);
            }

            string ret = "";
            for (int i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2").ToLower();
            return ret;
        }
    }

    internal class KVP<TKey, TValue>
    {
        public KVP()
        {
        }

        public KVP(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}
