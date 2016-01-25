
namespace Srk.BetaseriesApi.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    partial class BetaseriesXmlClient
    {
        /// <summary>
        /// Returns a dictionary containing the implementation date of each API method.
        /// </summary>
        public IDictionary<string, int> MethodsVersion
        {
            get
            {
                var dic = new Dictionary<string, int>();
                dic.Add("subtitles/last", 20110305);
                dic.Add("subtitles/show", 20110305);
                dic.Add("comments/post", 20101209);
                dic.Add("comments/member", 20101014);
                dic.Add("comments/show", 20101014);
                dic.Add("comments/episode", 20101222);
                dic.Add("members/auth", 20101014);
                dic.Add("members/badges", 20101014);
                dic.Add("members/destroy", 20101014);
                dic.Add("members/downloaded", 20101222);
                dic.Add("members/episodes", 20101209);
                dic.Add("members/friends", 20101227);
                dic.Add("members/infos", 20110118);
                dic.Add("members/is_active", 20101031);
                dic.Add("members/note", 20110201);
                dic.Add("members/notifications", 20110206);
                dic.Add("members/signup", 20101014);
                //dic.Add("members/search", );
                dic.Add("members/watched", 20101104);
                dic.Add("planning/general", 20110316);
                dic.Add("planning/member", 20110316);
                dic.Add("shows/add", 20101014);
                dic.Add("shows/display", 20110201);
                dic.Add("shows/episodes", 20101222);
                dic.Add("shows/remove", 20101014);
                dic.Add("shows/search", 20110316);
                dic.Add("status", 20101222);
                dic.Add("timeline/friends", 20110118);
                dic.Add("timeline/home", 20110118);
                dic.Add("timeline/member", 20110118);
                return dic;
            }
        }
    }
}
