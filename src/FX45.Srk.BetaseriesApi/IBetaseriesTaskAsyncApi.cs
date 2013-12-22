using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srk.BetaseriesApi {
    /// <summary>
    /// Represents the betaseries.com API.
    /// This interface requires asynchronous implementation via the new .NET 4 Tasks pattern TaskAsync(for async/await usage).
    /// </summary>
    public interface IBetaseriesTaskAsyncApi : IBetaseriesBaseApi, IDisposable {
        #region Shows

        /// <summary>
        /// Search for shows.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        Task<IList<Show>> SearchShowsTaskAsync(string title);

        /// <summary>
        /// Get a show's information.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<Show> GetShowTaskAsync(string url);

        /////// <summary>
        /////// Get a list of all shows.
        /////// </summary>
        /////// <returns></returns>
        ////Task<IList<Show>> GetShowsTaskAsync();

        /// <summary>
        /// Get all episodes from a show.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <returns></returns>
        Task<IList<Episode>> GetEpisodesTaskAsync(string showUrl);

        /// <summary>
        /// Get all episodes from a show and season.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <param name="season"></param>
        /// <returns></returns>
        Task<IList<Episode>> GetEpisodesTaskAsync(string showUrl, uint season);

        /// <summary>
        /// Get 1 episode from a show.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        /// <returns></returns>
        Task<Episode> GetEpisodeTaskAsync(string showUrl, uint season, uint episode);

        #region For the current user

        /// <summary>
        /// Add a show to your account.
        /// You must have a valid session <see cref="IBetaseriesBaseApi.SessionToken"/>.
        /// </summary>
        /// <param name="showUrl"></param>
        Task AddShowTaskAsync(string showUrl);

        /// <summary>
        /// Remove a show from your account.
        /// You must have a valid session <see cref="IBetaseriesBaseApi.SessionToken"/>.
        /// </summary>
        /// <param name="showUrl"></param>
        Task RemoveShowTaskAsync(string showUrl);

        #endregion

        #endregion

        #region Subtitles
        /*
        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <returns></returns>
        IList<Subtitle> GetShowSubtitlesTaskAsync(string showUrl);

        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">"VO" / "VF" / null TaskAsync(for all languages)</param>
        /// <returns></returns>
        IList<Subtitle> GetShowSubtitlesTaskAsync(string showUrl, string language);

        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">"VO" / "VF" / null TaskAsync(for all languages)</param>
        /// <param name="number"></param>
        /// <returns></returns>
        IList<Subtitle> GetShowSubtitlesTaskAsync(string showUrl, string language, uint season);
        */
        /// <summary>
        /// Get the  subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url TaskAsync(required)</param>
        /// <param name="language">"VO" or "VF" or null for all languages</param>
        /// <param name="season">a season number or null for all seasons</param>
        /// <param name="episode">an episode number or null for all episodes</param>
        /// <returns></returns>
        Task<IList<Subtitle>> GetShowSubtitlesTaskAsync(string showUrl, string language, uint? season, uint? episode);

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <returns></returns>
        Task<IList<Subtitle>> GetLatestSubtitlesTaskAsync();

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <param name="language">VO / VF</param>
        /// <returns></returns>
        Task<IList<Subtitle>> GetLatestSubtitlesTaskAsync(string language);

        /// <summary>
        /// Get the latest subtitles.
        /// </summary>
        /// <param name="language">VO / VF</param>
        /// <param name="number"></param>
        /// <returns></returns>
        Task<IList<Subtitle>> GetLatestSubtitlesTaskAsync(string language, uint number);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <returns></returns>
        Task<IList<Subtitle>> GetLatestShowSubtitlesTaskAsync(string showUrl);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">VO / VF</param>
        /// <returns></returns>
        Task<IList<Subtitle>> GetLatestShowSubtitlesTaskAsync(string showUrl, string language);

        /// <summary>
        /// Get the latest subtitles for a show.
        /// </summary>
        /// <param name="showUrl">Show's url</param>
        /// <param name="language">VO / VF</param>
        /// <param name="number"></param>
        /// <returns></returns>
        Task<IList<Subtitle>> GetLatestShowSubtitlesTaskAsync(string showUrl, string language, uint number);

        #endregion

        #region Planning

        /// <summary>
        /// Fetch all episodes from the last 8 days to the next 8 days.
        /// </summary>
        /// <returns></returns>
        Task<IList<Episode>> GetPlanningTaskAsync();

        /// <summary>
        /// Fetch episodes from a member's agenda and from the last 8 days to the next 8 days.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<IList<Episode>> GetMembersPlanningTaskAsync(string username);

        /// <summary>
        /// Fetch episodes from a member's agenda and from the last 8 days to the next 8 days.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="unseenOnly">if true, only fetch unseem episodes</param>
        /// <returns></returns>
        Task<IList<Episode>> GetMembersPlanningTaskAsync(string username, bool unseenOnly);

        #endregion

        #region Members

        /// <summary>
        /// Authenticates a user.
        /// This will set the received token in the <see cref="IBetaseriesBaseApi.SessionToken"/> property.
        /// This will set the username in the <see cref="IBetaseriesBaseApi.SessionUsername"/> property.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<string> AuthenticateTaskAsync(string username, string password);

        /// <summary>
        /// Verifies the session is still active.
        /// </summary>
        Task<bool> GetIsSessionActiveTaskAsync();

        /// <summary>
        /// Logs off. 
        /// This will unset the <see cref="IBetaseriesBaseApi.SessionToken"/> property.
        /// This will unset the <see cref="IBetaseriesBaseApi.SessionUsername"/> property.
        /// </summary>
        Task LogoffTaskAsync();

        /// <summary>
        /// Gets a member's informations.
        /// </summary>
        /// <param name="username">if null, it will use the <see cref="IBetaseriesBaseApi.SessionUsername"/> property's value</param>
        /// <returns></returns>
        Task<Member> GetMemberTaskAsync(string username);

        #endregion

        #region Comments


        Task<IList<Comment>> GetCommentsForShowTaskAsync(string showUrl);


        Task<IList<Comment>> GetCommentsForEpisodeTaskAsync(string showUrl, uint season, uint episode);


        Task<IList<Comment>> GetCommentsForMemberTaskAsync(string username);


        Task CommentShowTaskAsync(string showUrl, string comment, uint? inReplyTo);


        Task CommentEpisodeTaskAsync(string showUrl, uint season, uint episode, string comment, uint? inReplyTo);


        Task CommentMemberTaskAsync(string username, string comment, uint? inReplyTo);

        #endregion

        #region Timeline


        Task<IList<TimelineItem>> GetMainTimelineTaskAsync(uint? count);


        Task<IList<TimelineItem>> GetFriendsTimelineTaskAsync(uint? count);


        Task<IList<TimelineItem>> GetMemberTimelineTaskAsync(string username, uint? count);

        #endregion

        #region Status

        Task<ApiStatus> GetStatusTaskAsync();

        #endregion
    }
}
