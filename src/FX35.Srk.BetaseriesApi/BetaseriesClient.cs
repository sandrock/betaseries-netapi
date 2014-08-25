
namespace Srk.BetaseriesApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    partial class BetaseriesClient
    {
        /// <summary>
        /// Execute an HTTP GET request through an HTTP wrapper.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="keyValues">Pairs of query string parameters (key1, value1, key2, value2...)</param>
        /// <returns>HTTP response body as a string.</returns>
        internal virtual string ExecuteQuery(string action, params string[] keyValues)
        {
            if (keyValues.Length % 2 != 0)
                throw new ArgumentException("Invalid parameters count", "keyvalues");

            var parameters = new List<KVP<string, string>>(keyValues.Length / 2);

            if (keyValues.Length > 0)
            {
                bool isKey = true;
                string key = null;
                foreach (var item in keyValues)
                {
                    if (isKey)
                    {
                        key = item;
                    }
                    else
                    {
                        parameters.Add(new KVP<string, string>(key, item));
                    }

                    isKey = !isKey;
                }

                var multiKeys = parameters.GroupBy(p => p.Key).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
                foreach (var item in parameters.Where(p => multiKeys.Contains(p.Key)))
                {
                    item.Key += "[]";
                }
            }

            return ExecuteQuery(action, parameters);
        }

        /// <summary>
        /// Execute an HTTP GET request through an HTTP wrapper.
        /// </summary>
        /// <param name="action">Service action</param>
        /// <param name="parameters">Query string parameters</param>
        /// <returns>HTTP response body as a string.</returns>
        internal virtual string ExecuteQuery(string action, List<KVP<string, string>> parameters)
        {
            parameters = parameters ?? new List<KVP<string, string>>();
            ////parameters["key"] = Key;
            ////if (SessionToken != null)
            ////{
            ////    parameters["token"] = SessionToken;
            ////}

            return this.http.ExecuteQuery(action, parameters);
        }

        internal void HandleErrors<T>(BaseResponse<T> result)
        {
            throw new NotImplementedException();
        }

        internal void HandleErrors(BaseResponse result)
        {
            throw new NotImplementedException();
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
