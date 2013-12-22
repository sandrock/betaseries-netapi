
namespace Srk.BetaseriesApi
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the betaseries.com API.
    /// This interface requires asynchronous implementation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IBetaseriesAsyncApi : IBetaseriesBaseApi, IDisposable
    {
        #region Shows

        /// <summary>
        /// Search for shows.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="callback">The callback.</param>
        void SearchShowsAsync(string title, AsyncResponseHandler<IList<Show>> callback);

        /// <summary>
        /// Get a show's information.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="callback">The callback.</param>
        void GetShowAsync(string showUrl, AsyncResponseHandler<Show> callback);

        //// <summary>
        //// Get a list of all shows.
        //// </summary>
        //// <param name="callback">The callback.</param>
        ////void GetShowsAsync(AsyncResponseHandler<IList<Show>> callback);

        /// <summary>
        /// Get all episodes from a show.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="callback">The callback.</param>
        void GetEpisodesAsync(string showUrl, AsyncResponseHandler<IList<Episode>> callback);

        /// <summary>
        /// Get all episodes from a show and season.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="season">The season.</param>
        /// <param name="callback">The callback.</param>
        void GetEpisodesAsync(string showUrl, uint season, AsyncResponseHandler<IList<Episode>> callback);

        /// <summary>
        /// Get 1 episode from a show.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="season">The season.</param>
        /// <param name="episode">The episode.</param>
        /// <param name="callback">The callback.</param>
        void GetEpisodeAsync(string showUrl, uint season, uint episode, AsyncResponseHandler<Episode> callback);

        #region For the current user

        /// <summary>
        /// Add a show to your account.
        /// You must have a valid session <see cref="IBetaseriesBaseApi.SessionToken" />.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="callback">The callback.</param>
        void AddShowAsync(string showUrl, AsyncResponseHandler callback);

        /// <summary>
        /// Remove a show from your account.
        /// You must have a valid session <see cref="IBetaseriesBaseApi.SessionToken" />.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="callback">The callback.</param>
        void RemoveShowAsync(string showUrl, AsyncResponseHandler callback);

        #endregion

        #endregion

        #region Subtitles

        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url (required)</param>
        /// <param name="language">"VO" or "VF" or null for all languages</param>
        /// <param name="season">a season number or null for all seasons</param>
        /// <param name="episode">an episode number or null for all episodes</param>
        /// <param name="callback">The callback.</param>
        void GetShowSubtitlesAsync(string showUrl, string language, uint? season, uint? episode, AsyncResponseHandler<IList<Subtitle>> callback);

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void GetLatestSubtitlesAsync(AsyncResponseHandler<IList<Subtitle>> callback);

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <param name="language">VO / VF</param>
        /// <param name="callback">The callback.</param>
        void GetLatestSubtitlesAsync(string language, AsyncResponseHandler<IList<Subtitle>> callback);

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <param name="language">VO / VF</param>
        /// <param name="number">The number.</param>
        /// <param name="callback">The callback.</param>
        void GetLatestSubtitlesAsync(string language, uint number, AsyncResponseHandler<IList<Subtitle>> callback);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="callback">The callback.</param>
        void GetLatestShowSubtitlesAsync(string showUrl, AsyncResponseHandler<IList<Subtitle>> callback);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">VO / VF</param>
        /// <param name="callback">The callback.</param>
        void GetLatestShowSubtitlesAsync(string showUrl, string language, AsyncResponseHandler<IList<Subtitle>> callback);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">VO / VF</param>
        /// <param name="number">The number.</param>
        /// <param name="callback">The callback.</param>
        void GetLatestShowSubtitlesAsync(string showUrl, string language, uint number, AsyncResponseHandler<IList<Subtitle>> callback);

        #endregion

        #region Planning

        /// <summary>
        /// Fetch all episodes from the last 8 days to the next 8 days.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void GetPlanningAsync(AsyncResponseHandler<IList<Episode>> callback);

        /// <summary>
        /// Fetch episodes from a member's agenda and from the last 8 days to the next 8 days.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="callback">The callback.</param>
        void GetMembersPlanningAsync(string username, AsyncResponseHandler<IList<Episode>> callback);

        /// <summary>
        /// Fetch episodes from a member's agenda and from the last 8 days to the next 8 days.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="unseenOnly">if true, only fetch unseem episodes</param>
        /// <param name="callback">The callback.</param>
        void GetMembersPlanningAsync(string username, bool unseenOnly, AsyncResponseHandler<IList<Episode>> callback);

        #endregion

        #region Members

        /// <summary>
        /// Authenticate a user.
        /// This will set the received token in the <see cref="IBetaseriesBaseApi.SessionToken" /> property.
        /// This will set the username in the <see cref="IBetaseriesBaseApi.SessionUsername" /> property.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="callback">The callback.</param>
        void AuthenticateAsync(string username, string password, AsyncResponseHandler<string> callback);

        /// <summary>
        /// Verify the session is still active.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void GetIsSessionActiveAsync(AsyncResponseHandler<bool> callback);

        /// <summary>
        /// Log-off.
        /// This will unset the <see cref="IBetaseriesBaseApi.SessionToken" /> property.
        /// This will unset the <see cref="IBetaseriesBaseApi.SessionUsername" /> property.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void LogoffAsync(AsyncResponseHandler callback);

        /// <summary>
        /// Get a member's informations.
        /// </summary>
        /// <param name="username">if null, it will use the <see cref="IBetaseriesBaseApi.SessionUsername" /> property's value</param>
        /// <param name="callback">The callback.</param>
        void GetMemberAsync(string username, AsyncResponseHandler<Member> callback);

        /// <summary>
        /// Get member's next episodes to watch.
        /// </summary>
        /// <param name="onlyNextEpisode">if true, only next 1 episode per show is fetched.
        /// if false, all next episodes are fetched.</param>
        /// <param name="callback">The callback.</param>
        void GetMembersNextEpisodesAsync(bool onlyNextEpisode, AsyncResponseHandler<IList<Episode>> callback);

        /// <summary>
        /// Get member's next episodes to watch for a specific show.
        /// </summary>
        /// <param name="onlyNextEpisode">if true, only next 1 episode per show is fetched.
        /// if false, all next episodes are fetched.</param>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="callback">The callback.</param>
        void GetMembersNextShowEpisodeAsync(bool onlyNextEpisode, string showUrl, AsyncResponseHandler<Episode> callback);

        /// <summary>
        /// Set the latest seen episode of a show.
        /// All previous shows will be marked as seen because the website does not support per-episode seen-flag.
        /// If you pass season=0 and episode=0, the show will be marked as unseen.
        /// Only works if the <see cref="IBetaseriesBaseApi.SessionToken" /> is set and valid.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="season">The season.</param>
        /// <param name="episode">The episode.</param>
        /// <param name="mark">null or a number from 1 to 5</param>
        /// <param name="callback">The callback.</param>
        void SetEpisodeAsSeenAsync(string showUrl, uint season, uint episode, ushort? mark, AsyncResponseHandler callback);

        /// <summary>
        /// Set the latest downloaded episode of a show.
        /// Only works if the <see cref="IBetaseriesBaseApi.SessionToken" /> is set and valid.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="season">The season.</param>
        /// <param name="episode">The episode.</param>
        /// <param name="callback">The callback.</param>
        void SetEpisodeAsDownloadedAsync(string showUrl, uint season, uint episode, AsyncResponseHandler callback);

        /// <summary>
        /// Set a mark for an episode.
        /// Only works if the <see cref="IBetaseriesBaseApi.SessionToken" /> is set and valid.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="season">The season.</param>
        /// <param name="episode">The episode.</param>
        /// <param name="mark">null or a number from 1 to 5</param>
        /// <param name="callback">The callback.</param>
        void SetEpisodeMarkAsync(string showUrl, uint season, uint episode, ushort mark, AsyncResponseHandler callback);


        /// <summary>
        /// Gets the notifications async.
        /// </summary>
        /// <param name="seen">The seen.</param>
        /// <param name="count">The count.</param>
        /// <param name="fromNotificationId">From notification id.</param>
        /// <param name="callback">The callback.</param>
        void GetNotificationsAsync(bool? seen, uint? count, uint? fromNotificationId, AsyncResponseHandler<IList<Notification>> callback);

        /// <summary>
        /// Create a new account.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="callback">The callback.</param>
        void SignupAsync(string username, string password, string email, AsyncResponseHandler callback);

        /// <summary>
        /// Returns a user's friends.
        /// </summary>
        /// <param name="username">current member if null</param>
        /// <param name="callback">The callback.</param>
        void GetFriendsAsync(string username, AsyncResponseHandler<IList<string>> callback);

        /// <summary>
        /// Returns user's badges.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="callback">The callback.</param>
        void GetBadgesAsync(string username, AsyncResponseHandler<string[]> callback);

        #endregion

        #region Comments


        /// <summary>
        /// Gets the comments for show.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="callback">The callback.</param>
        void GetCommentsForShowAsync(string showUrl, AsyncResponseHandler<IList<Comment>> callback);

        /// <summary>
        /// Gets the comments for episode.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="season">The season.</param>
        /// <param name="episode">The episode.</param>
        /// <param name="callback">The callback.</param>
        void GetCommentsForEpisodeAsync(string showUrl, uint season, uint episode, AsyncResponseHandler<IList<Comment>> callback);

        /// <summary>
        /// Gets the comments for member.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="callback">The callback.</param>
        void GetCommentsForMemberAsync(string username, AsyncResponseHandler<IList<Comment>> callback);

        /// <summary>
        /// Comments the show.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="inReplyTo">The in reply to.</param>
        /// <param name="callback">The callback.</param>
        void CommentShowAsync(string showUrl, string comment, uint? inReplyTo, AsyncResponseHandler callback);

        /// <summary>
        /// Comments the episode.
        /// </summary>
        /// <param name="showUrl">The show URL.</param>
        /// <param name="season">The season.</param>
        /// <param name="episode">The episode.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="inReplyTo">The in reply to.</param>
        /// <param name="callback">The callback.</param>
        void CommentEpisodeAsync(string showUrl, uint season, uint episode, string comment, uint? inReplyTo, AsyncResponseHandler callback);

        /// <summary>
        /// Comments the member.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="inReplyTo">The in reply to.</param>
        /// <param name="callback">The callback.</param>
        void CommentMemberAsync(string username, string comment, uint? inReplyTo, AsyncResponseHandler callback);

        #endregion

        #region Timeline

        /// <summary>
        /// Gets the main timeline.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="callback">The callback.</param>
        void GetMainTimelineAsync(uint? count, AsyncResponseHandler<IList<TimelineItem>> callback);

        /// <summary>
        /// Gets the friends timeline.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="callback">The callback.</param>
        void GetFriendsTimelineAsync(uint? count, AsyncResponseHandler<IList<TimelineItem>> callback);

        /// <summary>
        /// Gets the member timeline.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="count">The count.</param>
        /// <param name="callback">The callback.</param>
        void GetMemberTimelineAsync(string username, uint? count, AsyncResponseHandler<IList<TimelineItem>> callback);

        #endregion

        #region Status

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void GetStatusAsync(AsyncResponseHandler<ApiStatus> callback);

        #endregion
    }
}
