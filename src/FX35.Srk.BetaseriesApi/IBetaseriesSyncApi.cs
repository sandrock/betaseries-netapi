using System.Collections.Generic;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Represents the betaseries.com API.
    /// This interface requires synchronous implementation.
    /// </summary>
    public interface IBetaseriesSyncApi : IBetaseriesBaseApi {

        #region Shows

        /// <summary>
        /// Search for shows.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        IList<Show> SearchShows(string title);

        /// <summary>
        /// Get a show's information.
        /// This method fetches a big volume of data!
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Show GetShow(string url);

        //// <summary>
        //// Get a list of all shows.
        //// </summary>
        //// <returns></returns>
        ////IList<Show> GetShows();

        /// <summary>
        /// Get all episodes from a show.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <returns></returns>
        IList<Episode> GetEpisodes(string showUrl);

        /// <summary>
        /// Get all episodes from a show and season.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <param name="season"></param>
        /// <returns></returns>
        IList<Episode> GetEpisodes(string showUrl, uint season);

        /// <summary>
        /// Get 1 episode from a show.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        /// <returns></returns>
        Episode GetEpisode(string showUrl, uint season, uint episode);

        #region For the current user

        /// <summary>
        /// Add a show to your account.
        /// You must have a valid session <see cref="IBetaseriesBaseApi.SessionToken"/>.
        /// </summary>
        /// <param name="showUrl"></param>
        void AddShow(string showUrl);

        /// <summary>
        /// Remove a show from your account.
        /// You must have a valid session <see cref="IBetaseriesBaseApi.SessionToken"/>.
        /// </summary>
        /// <param name="showUrl"></param>
        void RemoveShow(string showUrl);

        #endregion

        #endregion

        #region Subtitles
        /*
        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <returns></returns>
        IList<Subtitle> GetShowSubtitles(string showUrl);

        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">"VO" / "VF" / null (for all languages)</param>
        /// <returns></returns>
        IList<Subtitle> GetShowSubtitles(string showUrl, string language);

        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">"VO" / "VF" / null (for all languages)</param>
        /// <param name="number"></param>
        /// <returns></returns>
        IList<Subtitle> GetShowSubtitles(string showUrl, string language, uint season);
        */
        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url (required)</param>
        /// <param name="language">"VO" or "VF" or null for all languages</param>
        /// <param name="season">a season number or null for all seasons</param>
        /// <param name="episode">an episode number or null for all episodes</param>
        /// <returns></returns>
        IList<Subtitle> GetShowSubtitles(string showUrl, string language, uint? season, uint? episode);

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <returns></returns>
        IList<Subtitle> GetLatestSubtitles();

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <param name="language">VO / VF</param>
        /// <returns></returns>
        IList<Subtitle> GetLatestSubtitles(string language);

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <param name="language">VO / VF</param>
        /// <param name="number"></param>
        /// <returns></returns>
        IList<Subtitle> GetLatestSubtitles(string language, uint number);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <returns></returns>
        IList<Subtitle> GetLatestShowSubtitles(string showUrl);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">VO / VF</param>
        /// <returns></returns>
        IList<Subtitle> GetLatestShowSubtitles(string showUrl, string language);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">VO / VF</param>
        /// <param name="number"></param>
        /// <returns></returns>
        IList<Subtitle> GetLatestShowSubtitles(string showUrl, string language, uint number);

        #endregion

        #region Planning

        /// <summary>
        /// Fetch all episodes from the last 8 days to the next 8 days.
        /// </summary>
        /// <returns></returns>
        IList<Episode> GetPlanning();

        /// <summary>
        /// Fetch episodes from a member's agenda and from the last 8 days to the next 8 days.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        IList<Episode> GetMembersPlanning(string username);

        /// <summary>
        /// Fetch episodes from a member's agenda and from the last 8 days to the next 8 days.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="unseenOnly">if true, only fetch unseem episodes</param>
        /// <returns></returns>
        IList<Episode> GetMembersPlanning(string username, bool unseenOnly);

        #endregion

        #region Members

        /// <summary>
        /// Authenticate a user.
        /// This will set the received token in the <see cref="IBetaseriesBaseApi.SessionToken"/> property.
        /// This will set the username in the <see cref="IBetaseriesBaseApi.SessionUsername"/> property.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string Authenticate(string username, string password);

        /// <summary>
        /// Verify the session is still active.
        /// </summary>
        bool GetIsSessionActive();

        /// <summary>
        /// Log-off. 
        /// This will unset the <see cref="IBetaseriesBaseApi.SessionToken"/> property.
        /// This will unset the <see cref="IBetaseriesBaseApi.SessionUsername"/> property.
        /// </summary>
        void Logoff();

        /// <summary>
        /// Get a member's informations.
        /// </summary>
        /// <param name="username">if null, it will use the <see cref="IBetaseriesBaseApi.SessionUsername"/> property's value</param>
        /// <returns></returns>
        Member GetMember(string username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onlyNextEpisode">
        /// if true, only next 1 episode per show is fetched.
        /// if false, all next episodes are fetched.
        /// </param>
        /// <returns></returns>
        IList<Episode> GetMembersNextEpisodes(bool onlyNextEpisode);

        /// <summary>
        /// Get member's next episodes to watch for a specific show.
        /// </summary>
        /// <param name="onlyNextEpisode">
        /// if true, only next 1 episode per show is fetched.
        /// if false, all next episodes are fetched.
        /// </param>
        /// <param name="showUrl"></param>
        /// <returns></returns>
        Episode GetMembersNextShowEpisode(bool onlyNextEpisode, string showUrl);

        /// <summary>
        /// Set the latest seen episode of a show.
        /// All previous shows will be marked as seen because the website does not support per-episode seen-flag.
        /// If you pass season=0 and episode=0, the show will be marked as unseen.
        /// Only works if the <see cref="IBetaseriesBaseApi.SessionToken"/> is set and valid.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        /// <param name="mark">null or a number from 1 to 5</param>
        /// <returns></returns>
        void SetEpisodeAsSeen(string showUrl, uint season, uint episode, ushort? mark);

        /// <summary>
        /// Set the latest downloaded episode of a show.
        /// Only works if the <see cref="IBetaseriesBaseApi.SessionToken"/> is set and valid.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        void SetEpisodeAsDownloaded(string showUrl, uint season, uint episode);

        /// <summary>
        /// Set a mark for an episode.
        /// Only works if the <see cref="IBetaseriesBaseApi.SessionToken"/> is set and valid.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        /// <param name="mark">null or a number from 1 to 5</param>
        void SetEpisodeMark(string showUrl, uint season, uint episode, ushort mark);


        IList<Notification> GetNotifications(bool? seen, uint? count, uint? fromNotificationId);

        /// <summary>
        /// Create a new account.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        void Signup(string username, string password, string email);

        /// <summary>
        /// Returns a user's friends.
        /// </summary>
        /// <param name="username">current member if null</param>
        /// <returns></returns>
        IList<string> GetFriends(string username);

        string[] GetBadges(string username);

        #endregion

        #region Comments


        IList<Comment> GetCommentsForShow(string showUrl);


        IList<Comment> GetCommentsForEpisode(string showUrl, uint season, uint episode);


        IList<Comment> GetCommentsForMember(string username);


        void CommentShow(string showUrl, string comment, uint? inReplyTo);


        void CommentEpisode(string showUrl, uint season, uint episode, string comment, uint? inReplyTo);


        void CommentMember(string username, string comment, uint? inReplyTo);

        #endregion

        #region Timeline


        IList<TimelineItem> GetMainTimeline(uint? count);


        IList<TimelineItem> GetFriendsTimeline(uint? count);


        IList<TimelineItem> GetMemberTimeline(string username, uint? count);

        #endregion

        #region Status 

        ApiStatus GetStatus();

        #endregion

    }
}
