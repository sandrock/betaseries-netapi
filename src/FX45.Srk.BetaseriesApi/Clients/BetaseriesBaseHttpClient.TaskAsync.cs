using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Srk.BetaseriesApi.Clients {
    partial class BetaseriesBaseHttpClient : IBetaseriesApi {
        #region Shows

        public Task<IList<Show>> SearchShowsTaskAsync(string title) {
            return Task.Run(() => this.SearchShows(title));
        }

        public Task<Show> GetShowTaskAsync(string url) {
            return Task.Run(() => this.GetShow(url));
        }

        ////public Task<IList<Show>> GetShowsTaskAsync() {
        ////    return Task.Run(() => this.GetShows());
        ////}

        public Task<IList<Episode>> GetEpisodesTaskAsync(string showUrl) {
            return Task.Run(() => this.GetEpisodes(showUrl));
        }

        public Task<IList<Episode>> GetEpisodesTaskAsync(string showUrl, uint season) {
            return Task.Run(() => this.GetEpisodes(showUrl, season));
        }

        public Task<Episode> GetEpisodeTaskAsync(string showUrl, uint season, uint episode) {
            return Task.Run(() => this.GetEpisode(showUrl, season, episode));
        }

        #region For the current user

        public Task AddShowTaskAsync(string showUrl) {
            return Task.Run(() => this.AddShow(showUrl));
        }

        public Task RemoveShowTaskAsync(string showUrl) {
            return Task.Run(() => this.RemoveShow(showUrl));
        }

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
        
        public Task<IList<Subtitle>> GetShowSubtitlesTaskAsync(string showUrl, string language, uint? season, uint? episode) {
            return Task.Run(() => this.GetShowSubtitles(showUrl, language, season, episode));
        }

        public Task<IList<Subtitle>> GetLatestSubtitlesTaskAsync() {
            return Task.Run(() => this.GetLatestSubtitles());
        }
        
        public Task<IList<Subtitle>> GetLatestSubtitlesTaskAsync(string language) {
            return Task.Run(() => this.GetLatestSubtitles(language));
        }

        public Task<IList<Subtitle>> GetLatestSubtitlesTaskAsync(string language, uint number) {
            return Task.Run(() => this.GetLatestSubtitles(language, number));
        }

        public Task<IList<Subtitle>> GetLatestShowSubtitlesTaskAsync(string showUrl) {
            return Task.Run(() => this.GetLatestShowSubtitles(showUrl));
        }

        public Task<IList<Subtitle>> GetLatestShowSubtitlesTaskAsync(string showUrl, string language) {
            return Task.Run(() => this.GetLatestShowSubtitles(showUrl, language));
        }

        public Task<IList<Subtitle>> GetLatestShowSubtitlesTaskAsync(string showUrl, string language, uint number) {
            return Task.Run(() => this.GetLatestShowSubtitles(showUrl, language, number));
        }

        #endregion

        #region Planning

        public Task<IList<Episode>> GetPlanningTaskAsync() {
            return Task.Run(() => this.GetPlanning());
        }

        public Task<IList<Episode>> GetMembersPlanningTaskAsync(string username) {
            return Task.Run(() => this.GetMembersPlanning(username));
        }

        public Task<IList<Episode>> GetMembersPlanningTaskAsync(string username, bool unseenOnly) {
            return Task.Run(() => this.GetMembersPlanning(username, unseenOnly));
        }

        #endregion

        #region Members

        public Task<string> AuthenticateTaskAsync(string username, string password) {
            return Task.Run(() => this.Authenticate(username, password));
        }

        public Task<bool> GetIsSessionActiveTaskAsync() {
            return Task.Run(() => this.GetIsSessionActive());
        }

        public Task LogoffTaskAsync() {
            return Task.Run(new Action(this.Logoff));
        }

        public Task<Member> GetMemberTaskAsync(string username) {
            return Task.Run(() => this.GetMember(username));
        }

        #endregion

        #region Comments

        public Task<IList<Comment>> GetCommentsForShowTaskAsync(string showUrl) {
            return Task.Run(() => this.GetCommentsForShow(showUrl));
        }

        public Task<IList<Comment>> GetCommentsForEpisodeTaskAsync(string showUrl, uint season, uint episode) {
            return Task.Run(() => this.GetCommentsForEpisode(showUrl, season, episode));
        }

        public Task<IList<Comment>> GetCommentsForMemberTaskAsync(string username) {
            return Task.Run(() => this.GetCommentsForMember(username));
        }

        public Task CommentShowTaskAsync(string showUrl, string comment, uint? inReplyTo) {
            return Task.Run(() => this.CommentShow(showUrl, comment, inReplyTo));
        }

        public Task CommentEpisodeTaskAsync(string showUrl, uint season, uint episode, string comment, uint? inReplyTo) {
            return Task.Run(() => this.CommentEpisode(showUrl, season, episode, comment, inReplyTo));
        }

        public Task CommentMemberTaskAsync(string username, string comment, uint? inReplyTo) {
            return Task.Run(() => this.CommentMember(username, comment, inReplyTo));
        }

        #endregion

        #region Timeline

        public Task<IList<TimelineItem>> GetMainTimelineTaskAsync(uint? count) {
            return Task.Run(() => this.GetMainTimeline(count));
        }

        public Task<IList<TimelineItem>> GetFriendsTimelineTaskAsync(uint? count) {
            return Task.Run(() => this.GetFriendsTimeline(count));
        }

        public Task<IList<TimelineItem>> GetMemberTimelineTaskAsync(string username, uint? count) {
            return Task.Run(() => this.GetMemberTimeline(username, count));
        }

        #endregion

        #region Status

        public Task<ApiStatus> GetStatusTaskAsync() {
            return Task.Run(() => this.GetStatus());
        }

        #endregion
    }
}
