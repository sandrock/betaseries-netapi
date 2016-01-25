
namespace Srk.BetaseriesApi
{
    using System;
    using Srk.BetaseriesApi.Resources;

    /// <summary>
    /// Timeline item.
    /// </summary>
    public class TimelineItem
    {

        /// <summary>
        /// Default .ctor.
        /// </summary>
        public TimelineItem()
        {
        }

        /// <summary>
        /// Specific .ctor you shoudn't use.
        /// It is used from parsing methods.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="username"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="content"></param>
        /// <param name="episode"></param>
        public TimelineItem(string reference, string username, string type, DateTime? date, string content, Episode episode)
        {
            Reference = reference;
            Username = username;
            Content = content;
            Date = date;
            ServiceType = type;
            ReferenceEpisode = episode;

            FindTimelineItemType();
        }

        /// <summary>
        /// The type as identified by the service.
        /// </summary>
        public string ServiceType { get; private set; }

        #region Parsing

        public static TimelineItemType ParseTimelineItemType(string value)
        {
            TimelineItemType type = TimelineItemType.Unknown;
            switch (value)
            {
                case "friend_add":
                    type = TimelineItemType.FriendAdded;
                    break;
                case "friend_delete":
                    type = TimelineItemType.FriendRemoved;
                    break;
                case "markas":
                    type = TimelineItemType.MarkedAs;
                    break;
                case "add_serie":
                    type = TimelineItemType.ShowAdded;
                    break;
                case "del_serie":
                    type = TimelineItemType.ShowRemoved;
                    break;
                case "archive":
                    type = TimelineItemType.ShowArchived;
                    break;
                case "unarchive":
                    type = TimelineItemType.ShowUnarchived;
                    break;
                case "recommandation":
                    type = TimelineItemType.Recommendation;
                    break;
                case "recommandation_decline":
                    type = TimelineItemType.RecommendationDenied;
                    break;
                case "recommandation_accept":
                    type = TimelineItemType.RecommendationAccepted;
                    break;
                case "inscription":
                    type = TimelineItemType.UserRegistered;
                    break;
                case "update":
                    type = TimelineItemType.Update;
                    break;
                case "subtitles":
                    type = TimelineItemType.Subtitles;
                    break;
                case "comment":
                    type = TimelineItemType.Comment;
                    break;
                case "badge":
                    type = TimelineItemType.BadgeEarned;
                    break;
                case "wiki":
                    type = TimelineItemType.WikiUpdate;
                    break;
                default:
                    type = TimelineItemType.Unknown;
                    break;
            }
            return type;
        }

        private void FindTimelineItemType()
        {
            TimelineItemType type = ParseTimelineItemType(ServiceType);
            if (type == TimelineItemType.Comment && Reference != null)
            {
                if (Reference.StartsWith("membre."))
                {
                    // "membre.albator"
                    type = TimelineItemType.CommentOnMember;
                    Reference = Reference.Substring(7);
                }
                else if (Reference.StartsWith("serie."))
                {
                    // "serie.dexter"
                    type = TimelineItemType.CommentOnShow;
                    Reference = Reference.Substring(6);
                }
                else if (Reference.StartsWith("episode."))
                {
                    type = TimelineItemType.CommentOnEpisode;
                    // "episode.dexter.s02e04"
                    Reference = Reference.Substring(8);
                    string[] refSplit = Reference.Split('.');
                    ReferenceEpisode = new Episode(refSplit[1], refSplit[0], null, null);
                }
            }
            if (type == TimelineItemType.BadgeEarned)
            {
                Badges badge;
                BadgesUtil.TryParseBadge(Reference, out badge);
                ReferenceBadge = badge;
            }
            Type = type;
        }

        #endregion

        /// <summary>
        /// Item type.
        /// </summary>
        public TimelineItemType Type { get; set; }

        /// <summary>
        /// Translated message.
        /// </summary>
        public string Message
        {
            get
            {
                try
                {
                    string reference = null;
                    switch (Type)
                    {
                        case TimelineItemType.UserRegistered:
                            return string.Format(GetFormatableTranslation(Type), Username);
                        case TimelineItemType.MarkedAs:
                            reference = ReferenceEpisode != null ?
                                string.Concat(Reference, " ", ReferenceEpisode.Number) :
                                Reference;
                            return string.Format(GetFormatableTranslation(Type), Username, reference);
                        case TimelineItemType.CommentOnEpisode:
                            reference = ReferenceEpisode != null ?
                                string.Concat(ReferenceEpisode.ShowUrl, " ", ReferenceEpisode.Number) :
                                Reference;
                            return string.Format(GetFormatableTranslation(Type), Username, reference);
                        case TimelineItemType.BadgeEarned:
                            string badge = (ReferenceBadge.HasValue && ReferenceBadge.Value != Badges.Unknown) ? ReferenceBadge.Value.ToString() : Reference;
                            return string.Format(GetFormatableTranslation(Type), Username, BadgesUtil.GetName(badge));
                        case TimelineItemType.Unknown:
                            return string.Format(GetFormatableTranslation(Type), Username, ServiceType, Reference);
                        default:
                            return string.Format(GetFormatableTranslation(Type), Username, Reference);
                    }
                }
                catch
                {
                    return string.Format("{0}, {1} ({2}) {3}.", Username, Type.ToString(), ServiceType, Reference);
                }
            }
        }

        /// <summary>
        /// The generic subject.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The subject if <see cref="Type"/> is <see cref="TimelineItemType.BadgeEarned"/>.
        /// </summary>
        public Badges? ReferenceBadge { get; set; }

        /// <summary>
        /// The subject if <see cref="Type"/> is <see cref="TimelineItemType.MarkedAs"/>.
        /// </summary>
        public Episode ReferenceEpisode { get; set; }

        /// <summary>
        /// User involved.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// French message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Event's date.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Stub field you can use for display purpose.
        /// </summary>
        public bool IsNew { get; set; }

        #region Translation methods

        /// <summary>
        /// Returns a formatable string. 
        /// {0} is supposed to be the current user's name
        /// {1} is supposed the be a <see cref="TimelineItem.Reference"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Something like "{0} did something to {1}."</returns>
        public static string GetFormatableTranslation(string type)
        {
            return GeneralStrings.ResourceManager.GetString(
                string.Concat("TimelineItemType_", type, "_")
            ) ?? string.Concat("{0} ", type, " {1}");
        }

        /// <summary>
        /// Returns a formatable string. 
        /// {0} is supposed to be the current user's name
        /// {1} is supposed the be a <see cref="TimelineItem.Reference"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Something like "{0} did something to {1}."</returns>
        public static string GetFormatableTranslation(TimelineItemType type)
        {
            return GetFormatableTranslation(type.ToString());
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} by {1} on {2}", Type.ToString(), Username, Reference);
        }
    }

    /// <summary>
    /// To be used with <see cref="TimelineItem"/>.
    /// </summary>
    public enum TimelineItemType
    {

        /// <summary>
        /// Default value if parsing fails.
        /// </summary>
        Unknown,

        /// <summary>
        /// 
        /// </summary>
        FriendAdded,

        /// <summary>
        /// 
        /// </summary>
        FriendRemoved,

        /// <summary>
        /// Episode marked as seen.
        /// </summary>
        MarkedAs,

        /// <summary>
        /// Show added to profile.
        /// </summary>
        ShowAdded,

        /// <summary>
        /// Show removed from profile.
        /// </summary>
        ShowRemoved,

        /// <summary>
        /// 
        /// </summary>
        ShowArchived,

        /// <summary>
        /// 
        /// </summary>
        ShowUnarchived,

        /// <summary>
        /// 
        /// </summary>
        Recommendation,

        /// <summary>
        /// 
        /// </summary>
        RecommendationAccepted,

        /// <summary>
        /// 
        /// </summary>
        RecommendationDenied,

        /// <summary>
        /// 
        /// </summary>
        UserRegistered,

        /// <summary>
        /// 
        /// </summary>
        Update,

        /// <summary>
        /// 
        /// </summary>
        Subtitles,

        /// <summary>
        /// 
        /// </summary>
        //TODO: remove this safely
        Comment,

        /// <summary>
        /// 
        /// </summary>
        CommentOnShow,

        /// <summary>
        /// 
        /// </summary>
        CommentOnEpisode,

        /// <summary>
        /// 
        /// </summary>
        CommentOnMember,

        /// <summary>
        /// 
        /// </summary>
        BadgeEarned,

        /// <summary>
        /// 
        /// </summary>
        WikiUpdate,
    }
}
