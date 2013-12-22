using System;
using System.Collections.Generic;
using System.Web;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Extension methods for dictionaries.
    /// </summary>
    public static class DictionaryExtensions {

        /// <summary>
        /// Constructs a QueryString (string).
        /// Consider this method to be the opposite of "System.Web.HttpUtility.ParseQueryString"
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns>String</returns>
        public static string GetQueryString(this Dictionary<string, string> dictionary) {
            List<String> items = new List<String>();

            foreach (var item in dictionary) {
                items.Add(String.Concat(item.Key, "=", HttpUtility.UrlEncode(item.Value)));
            }

            return String.Join("&", items.ToArray());
        }
    }
}
