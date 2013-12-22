using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace Srk.BetaseriesApi.Clients {
    partial class BetaseriesXmlClient {

        private static readonly string ParseErrorMessage = "Could not parse service response";

        /// <summary>
        /// Returns a <see cref="XElement"/> from a HTTP body.
        /// </summary>
        /// <param name="content">the HTTP response content</param>
        /// <returns>the root XML element</returns>
        private static XElement ParseResponse(string content) {
            try {
                return XDocument.Parse(content).Root;
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        /// <summary>
        /// Parse a root element to find server/query errors.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private static BetaError[] GetErrors(XElement root) {
            BetaError[] errors = null;
            var errorsNode = root.Element("errors");
            if (errorsNode.HasElements) {
                try {
                    //errors = errorsNode
                    //    .Elements("error")
                    //    .Select(e => new BetaError(e.Attribute("code").Value, e.Value))
                    //    .ToArray();
                    errors = errorsNode
                        .Elements("error")
                        .Select(e => new BetaError(e.Element("code").Value, e.Element("content").Value))
                        .ToArray();
                } catch (XmlException ex) {
                    throw new ClientException(ParseErrorMessage, ex);
                }
            }
            return errors;
        }

        /// <summary>
        /// Parser for a list of shows.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static IList<Show> ParseShows(XElement xml) {
            var showsNode = xml.Element("shows");
            if (showsNode == null || !showsNode.HasElements)
                return null;
            try {
                return showsNode
                    .Elements("show")
                    .Select(e => new Show {
                        Title = e.Element("title").Value,
                        Url = e.Element("url").Value,
                        IsInProfile = e.ElementValueNbool("is_in_account")
                    })
                    .ToArray();
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        /// <summary>
        /// Parser for a show details.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static Show ParseShow(XElement xml) {
            try {
                return xml
                       .Elements("show")
                       .Select(e => new Show {
                           Title = e.Element("title").Value,
                           Url = e.Element("url").Value,
                           Description = e.Element("description").Value,
                           Status = e.Element("status").Value,
                           PictureUrl = e.ElementValueString("banner"),
                           Genres = new List<string>(
                               e.Elements("genres")
                                .Elements("genre")
                                .Select(f => f.Value)),
                           Seasons = new List<Show.SeasonCount>(
                               e.Element("seasons")
                               .Elements("season")
                               .Select(e2 => new Show.SeasonCount {
                                   Season = e2.Element("number").Value,
                                   Episodes = e2.Element("episodes").Value
                               })
                               .ToList()),
                           TVDBId = e.Element("id_thetvdb").Value,
                           IsInProfile = e.ElementValueNbool("is_in_account"),
                       })
                       .FirstOrDefault();
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        /// <summary>
        /// Parser for an episode list.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static IList<Episode> ParseEpisodesWithSeasons(XElement xml) {
            try {
                return xml
                       .Descendants("episode")
                       .Where(e => e.HasElements)

                       // instanciation is now using a full ctor
                       // leave comments for reference
                       .Select(e => new Episode(
                           //Season = 
                           e.Parent.Parent.Element("number").Value,
                           //SeasonOrder = int.Parse(e.Parent.Parent.Element("number").Value),
                           //Title = 
                           e.Element("title").Value,
                           //Description = 
                           e.Element("description").Value,
                           //PictureUrl = 
                           e.ElementValueString("screen"),
                           //Number = 
                           e.Element("number").Value,
                           //EpisodeNumber = 
                           e.Element("episode").Value,
                           //Order = int.Parse(e.Element("episode").Value),
                           //Date = 
                           e.ElementValueTimestampToDatetime("date"),
                           //Ratings = 
                           e.Element("note").ElementValue("members", 0),
                           //Rating = 
                           e.Element("note").ElementValueNfloat("mean"),
                           //UserRating = 
                           e.Element("note").ElementValueNint("self"),
                           //IsDownloaded = 
                           e.ElementValueNbool("downloaded"),
                           //IsSeen = 
                           e.ElementValueNbool("has_seen")
                       ))
                       .ToList();
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        /// <summary>
        /// Parser for an episode list.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static IList<Episode> ParseEpisodes(XElement xml) {
            try {
                return xml
                    .Descendants("episode")
                    .Where(e => e.HasElements)

                    // instanciation is now using a full ctor
                    // leave comments for reference
                    .Select(e => new Episode(
                        //ShowName = 
                        e.ElementValue("show", string.Empty),
                        //ShowUrl = 
                        e.ElementValue("url", string.Empty),
                        //Title = 
                        e.Element("title").Value,
                        //Number = 
                        e.Element("number").Value,
                        e.ElementValue("global", default(uint)),
                        //Date = 
                        e.ElementValueTimestampToDatetime("date"),
                        //IsDownloaded = 
                        e.ElementValueNbool("downloaded"),
                        //IsSeen = 
                        false
                    ))
                    .ToList();
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        private static string ParseSessionToken(XElement xml) {
            try {
                return xml
                    .Element("member")
                    .Element("token")
                    .Value;
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        private static Member ParseMember(XElement xml) {
            var memberNode = xml.Element("member");
            var stats = memberNode.Element("stats");

            try {
                var member = new Member {
                    Username =
                        memberNode.Element("login").Value,
                    EpisodeCount =
                        stats.ElementValue("episodes", 0u),
                    EpisodesToWatchCount =
                        stats.ElementValue("episodes_to_watch", 0u),
                    PictureUrl =
                        memberNode.ElementValueString("avatar"),
                    Progress =
                        stats.ElementValueString("progress"),
                    SeasonCount =
                        stats.ElementValue("seasons", 0u),
                    ShowCount =
                        stats.ElementValue("shows", 0u),
                    TimeRemaining =
                        stats.ElementValueTimespanFromMinutes("time_to_spend"),
                    TimeSpent =
                        stats.ElementValueTimespanFromMinutes("time_on_tv")
                };

                var showsNode = memberNode.Element("shows");
                if (showsNode != null && showsNode.HasElements) {
                    member.Shows = showsNode
                        .Elements("show")
                        .Select(e => new Show {
                            Url = e.ElementValueString("url"),
                            Title = e.ElementValueString("title"),
                        })
                        .ToList();
                }

                return member;
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        private static IList<Notification> ParseNotifications(XElement xml) {
            try {
                return xml
                    .Element("notifications")
                    .Elements("notification")
                    .Select(e => new Notification {
                        NotificationId = e.ElementValue("id", (ulong)0),
                        Date = e.ElementValue("date", DateTime.MinValue),
                        Content = e.Element("text").Value,
                        Seen = e.ElementValue("seen", false)
                    })
                    .ToList();
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        private static IList<TimelineItem> ParseTimeline(XElement xml) {
            try {
                return xml
                    .Element("timeline")
                    .Elements("item")
                    .Select(e => new TimelineItem(
                        e.Element("ref").Value, e.Element("login").Value,
                        e.Element("type").Value, e.ElementValueTimestampToDatetime("date"),
                        e.Element("html").Value, ParseTimelineEpisode(e)))
                    .ToList();
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        private static Episode ParseTimelineEpisode(XElement timelineItemNode) {
            if (timelineItemNode == null || !timelineItemNode.HasElements)
                return null;

            var dataNode = timelineItemNode.Element("data");
            if (dataNode == null || !dataNode.HasElements)
                return null;

            // instanciation is now using a full ctor
            // leave comments for reference
            return new Episode(
                //Number = 
                dataNode.ElementValueString("number"),
                //ShowUrl = 
                dataNode.ElementValueString("url"),
                //ShowName = 
                timelineItemNode.ElementValueString("ref"),
                //Title = 
                dataNode.ElementValueString("title")
            );
        }

        private static IList<string> ParseFriends(XElement xml) {
            try {
                return xml
                    .Element("friends")
                    .Elements("friend")
                    .Select(e => e.Value)
                    .ToList();
            } catch (XmlException ex) {
                throw new ClientException(ParseErrorMessage, ex);
            }
        }

        private static string[] ParseBadges(XElement xml) {
            return xml
                .Element("badges")
                .Elements("badge")
                .Select(e => e.ElementValueString("code"))
                .ToArray();
        }

        private static readonly ApiChange[] otherChanges = new ApiChange[] {
            new ApiChange { Action = "show/search", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "show/add", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "show/remove", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "planning/general", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "planning/member", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "members/auth", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "members/destroy", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "members/signup", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "comments/show", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "comments/episode", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "comments/member", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "comments/post/show", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "comments/post/episode", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "comments/post/member", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "timeline/home", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "timeline/friends", Date = 20101014, Type = ApiChangeType.New },
            new ApiChange { Action = "timeline/member", Date = 20101014, Type = ApiChangeType.New }
        };

        private static ApiStatus ParseStatus(XElement xml) {
            var status = new ApiStatus();

            var websiteNode = xml.Element("website");
            var apiNode = xml.Element("api");
            var versionsNode = apiNode != null ? apiNode.Element("versions") : null;
            var filesNode = apiNode != null ? apiNode.Element("files") : null;
            var methodsNode = apiNode != null ? apiNode.Element("methods") : null;

            if (websiteNode != null && websiteNode.HasElements) {

                // website/status
                status.WebsiteStatus = websiteNode.Element("status").Value;

                // website/database
                status.WebsiteDatabase = websiteNode.Element("database").Value;
            }

            if (apiNode != null && apiNode.HasElements) {

                // api/version
                status.ApiVersion = apiNode.Element("version").Value;

                // api/changes
                if (versionsNode != null) {
                    var changes = new List<ApiChange>();
                    changes.AddRange(versionsNode
                        .Descendants("change")
                        .Select(e => new ApiChange() {
                            Action = e.Element("value").Value,
                            Date = e.Parent.Parent.ElementValue("date", 0),
                            Type = GetApiChangeType(e.Element("type").Value)
                        })
                        .ToArray());

                    foreach (var item in otherChanges) {
                        if (!changes.Any(c => c.Action == item.Action))
                            changes.Add(item);
                    }

                    status.Changes = changes.ToArray();
                }

                // api/files
                if (filesNode != null) {
                    status.Files = filesNode
                        .Elements("file")
                        .Select(e => new ApiFile() {
                            Name = e.Element("name").Value,
                            Date = e.ElementValue("last_change", 0L)
                        })
                        .ToArray();
                }

                // api/methods
                if (methodsNode != null) {
                    status.Methods = methodsNode
                        .Elements("method")
                        .Select(e => new ApiMethod() {
                            Name = e.Element("name").Value,
                            DateCreated = e.ElementValue("created", 0),
                            DateUpdated = e.ElementValue("updated", 0)
                        })
                        .ToArray();
                }
            }
            return status;
        }

        private static ApiChangeType GetApiChangeType(string value) {
            switch (value.ToLower()) {
                case "updated":
                    return ApiChangeType.Update;
                case "new":
                    return ApiChangeType.New;
                default:
                    return ApiChangeType.Unknown;
            }
        }

        //added By Crevette
        /// <summary>
        /// Parser for comment List
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static IList<Comment> ParseComment(XElement xml) {
            return xml
                .Element("comments")
                .Elements("comment")
                .Select(e => new Comment(
                    e.Element("login").Value,
                    new DateTime(1970,1,1).AddSeconds(int.Parse(e.Element("date").Value)),
                    e.Element("text").Value,
                    int.Parse(e.Element("inner_id").Value),
                    int.Parse(e.Element("in_reply_to").Value)
                    ))
                .ToList();
        }

        private static IList<Subtitle> ParseSubtitle(XElement xml)  {
            return xml
                .Element("subtitles")
                .Elements("subtitle")
                .Select(e => new Subtitle() {
                    Title = e.Element("title").Value,
                    Season = int.Parse(e.Element("season").Value),
                    Episode = int.Parse(e.Element("episode").Value),
                    Language = e.Element("language").Value,
                    Url = e.Element("url").Value,
                    Quality = int.Parse(e.Element("quality").Value),
                    FileName = e.Element("file").Value,
                    Source = e.Element("source").Value
                })
                .ToList();
        }

        private static IList<Episode> ParsePlanning(XElement xml) {
            uint duint = 0;
            return xml
                .Element("planning")
                .Elements("item")
                .Select(e => new Episode(
                      e.ElementValueString("url"), e.ElementValueString("show"),
                      e.ElementValueString("title"),  e.ElementValueString("number"),
                      e.ElementValue("episode", duint), e.ElementValue("season", duint), 
                      e.ElementValueTimestampToDatetime("date")
                ))
                .ToList();
        }

    }
}
