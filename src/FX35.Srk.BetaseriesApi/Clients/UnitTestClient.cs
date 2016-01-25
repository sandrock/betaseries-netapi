
namespace Srk.BetaseriesApi.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public class UnitTestClient : IBetaseriesBaseApi, IBetaseriesSyncAsyncApi {

        #region IBetaseriesBaseApi Members

        public string UserAgent { get; set; }

        public string Key { get; set; }

        public string SessionUsername { get; set; }

        public string SessionToken { get; set; }

        public void SetSessionTokens(string newSessionToken, string newSessionUsername) {
            SessionToken = newSessionToken;
            SessionUsername = newSessionUsername;
        }

        #endregion

        #region IBetaseriesSyncApi Members

        public IList<Show> SearchShows(string title) {
            throw new NotImplementedException();
        }

        public Show GetShow(string url) {
            throw new NotImplementedException();
        }

        public IList<Show> GetShows() {
            throw new NotImplementedException();
        }

        public IList<Episode> GetEpisodes(string showUrl) {
            throw new NotImplementedException();
        }

        public IList<Episode> GetEpisodes(string showUrl, int season) {
            throw new NotImplementedException();
        }

        public Episode GetEpisode(string showUrl, int season, int episode) {
            throw new NotImplementedException();
        }

        public void AddShow(string showUrl) {
            throw new NotImplementedException();
        }

        public void RemoveShow(string showUrl) {
            throw new NotImplementedException();
        }

        public IList<Subtitle> GetShowSubtitles(string showUrl, string language, uint? season, uint? episode) {
            throw new NotImplementedException();
        }

        public IList<Subtitle> GetLatestSubtitles() {
            throw new NotImplementedException();
        }

        public IList<Subtitle> GetLatestSubtitles(string language) {
            throw new NotImplementedException();
        }

        public IList<Subtitle> GetLatestSubtitles(string language, uint number) {
            throw new NotImplementedException();
        }

        public IList<Subtitle> GetLatestShowSubtitles(string showUrl) {
            throw new NotImplementedException();
        }

        public IList<Subtitle> GetLatestShowSubtitles(string showUrl, string language) {
            throw new NotImplementedException();
        }

        public IList<Subtitle> GetLatestShowSubtitles(string showUrl, string language, uint number) {
            throw new NotImplementedException();
        }

        public IList<Episode> GetPlanning() {
            throw new NotImplementedException();
        }

        public IList<Episode> GetMemberPlanning(string username) {
            throw new NotImplementedException();
        }

        public IList<Episode> GetMemberPlanning(string username, bool unseenOnly) {
            throw new NotImplementedException();
        }

        public string Authenticate(string username, string password) {
            throw new NotImplementedException();
        }

        public bool GetIsSessionActive() {
            throw new NotImplementedException();
        }

        public void Logoff() {
            throw new NotImplementedException();
        }

        public Member GetMember(string username) {
            throw new NotImplementedException();
        }

        public IList<Episode> GetMembersNextEpisodes(string username, bool onlyNextEpisode) {
            throw new NotImplementedException();
        }

        public void SetEpisodeAsSeen(string showUrl, uint season, uint episode, ushort? mark) {
            throw new NotImplementedException();
        }

        public void SetEpisodeAsDownloaded(string showUrl, uint season, uint episode) {
            throw new NotImplementedException();
        }

        public void SetEpisodeMark(string showUrl, uint season, uint episode, ushort mark) {
            throw new NotImplementedException();
        }

        public IList<Notification> GetNotifications(bool? seen, uint? count, uint? fromNotificationId) {
            throw new NotImplementedException();
        }

        public void Signup(string username, string password, string email) {
            throw new NotImplementedException();
        }

        public IList<string> GetFriends(string username) {
            throw new NotImplementedException();
        }

        public IList<Comment> GetCommentsForShow(string showUrl) {
            throw new NotImplementedException();
        }

        public IList<Comment> GetCommentsForEpisode(string showUrl, uint season, uint episode) {
            throw new NotImplementedException();
        }

        public IList<Comment> GetCommentsForMember(string username) {
            throw new NotImplementedException();
        }

        public IList<Comment> CommentShow(string showUrl, string comment, uint? inReplyTo) {
            throw new NotImplementedException();
        }

        public IList<Comment> CommentEpisode(string showUrl, uint season, uint episode, string comment, uint? inReplyTo) {
            throw new NotImplementedException();
        }

        public IList<Comment> CommentMember(string username, string comment, uint? inReplyTo) {
            throw new NotImplementedException();
        }

        public IList<TimelineItem> GetMainTimeline(uint? count) {
            throw new NotImplementedException();
        }

        public IList<TimelineItem> GetFriendsTimeline(uint? count) {
            throw new NotImplementedException();
        }

        public IList<TimelineItem> GetMemberTimeline(string username, uint? count) {
            throw new NotImplementedException();
        }

        public ApiStatus GetStatus() {
            throw new NotImplementedException();
        }

        #endregion

        #region IBetaseriesAsyncApi Members

        public void SearchShowsAsync(string title) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Show>> SearchShowsEnded;

        public void GetShowAsync(string url) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<Show> GetShowEnded;

        public void GetShowsAsync() {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Show>> GetShowsEnded;

        public void GetEpisodesAsync(string showUrl) {
            throw new NotImplementedException();
        }

        public void GetEpisodesAsync(string showUrl, int season) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Episode>> GetEpisodesEnded;

        public void GetEpisodeAsync(string showUrl, int season, int episode) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<Episode> GetEpisodeEnded;

        public void AddShowAsync(string showUrl) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler AddShowEnded;

        public void RemoveShowAsync(string showUrl) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler RemoveShowEnded;

        public void GetShowSubtitlesAsync(string showUrl, string language, uint? season, uint? episode) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Subtitle>> GetShowSubtitlesEnded;

        public void GetLatestSubtitlesAsync() {
            throw new NotImplementedException();
        }

        public void GetLatestSubtitlesAsync(string language) {
            throw new NotImplementedException();
        }

        public void GetLatestSubtitlesAsync(string language, uint number) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Subtitle>> GetLatestSubtitlesEnded;

        public void GetLatestShowSubtitlesAsync(string showUrl) {
            throw new NotImplementedException();
        }

        public void GetLatestShowSubtitlesAsync(string showUrl, string language) {
            throw new NotImplementedException();
        }

        public void GetLatestShowSubtitlesAsync(string showUrl, string language, uint number) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Subtitle>> GetLatestShowSubtitlesEnded;

        public void GetPlanningAsync() {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Episode>> GetPlanningEnded;

        public void GetMemberPlanningAsync(string username) {
            throw new NotImplementedException();
        }

        public void GetMemberPlanningAsync(string username, bool unseenOnly) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Episode>> GetMemberPlanningEnded;

        public void AuthenticateAsync(string username, string password) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<string> AuthenticateEnded;

        public void GetIsSessionActiveAsync() {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<bool> GetIsSessionActiveEnded;

        public void LogoffAsync() {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler LogoffEnded;

        public void GetMemberAsync(string username) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<Member> GetMemberEnded;

        public void GetMembersNextEpisodesAsync(string username, bool onlyNextEpisode) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Episode>> GetMembersNextEpisodesEnded;

        public void SetEpisodeAsSeenAsync(string showUrl, uint season, uint episode, ushort? mark) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler SetEpisodeAsSeenEnded;

        public void SetEpisodeAsDownloadedAsync(string showUrl, uint season, uint episode) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler SetEpisodeAsDownloadedEnded;

        public void SetEpisodeMarkAsync(string showUrl, uint season, uint episode, ushort mark) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler SetEpisodeMarkEnded;

        public void GetNotificationsAsync(bool? seen, uint? count, uint? fromNotificationId) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Notification>> GetNotificationsEnded;

        public void SignupAsync(string username, string password, string email) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler SignupEnded;

        public void GetFriendsAsync(string username) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<string>> GetFriendsEnded;

        public void GetCommentsForShowAsync(string showUrl) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Comment>> GetCommentsForShowEnded;

        public void GetCommentsForEpisodeAsync(string showUrl, uint season, uint episode) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Comment>> GetCommentsForEpisodeEnded;

        public void GetCommentsForMemberAsync(string username) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<Comment>> GetCommentsForMemberEnded;

        public void CommentShowAsync(string showUrl, string comment, uint? inReplyTo) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler CommentShowEnded;

        public void CommentEpisodeAsync(string showUrl, uint season, uint episode, string comment, uint? inReplyTo) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler CommentEpisodeEnded;

        public void CommentMemberAsync(string username, string comment, uint? inReplyTo) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler CommentMemberEnded;

        public void GetMainTimelineAsync(uint? count) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<TimelineItem>> GetMainTimelineAsyncEnded;

        public void GetFriendsTimelineAsync(uint? count) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<TimelineItem>> GetFriendsTimelineAsyncEnded;

        public void GetMemberTimelineAsync(string username, uint? count) {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<IList<TimelineItem>> GetMemberTimelineAsyncEnded;

        public void GetStatusAsync() {
            throw new NotImplementedException();
        }

        public event AsyncResponseHandler<ApiStatus> GetStatusEnded;

        #endregion

        #region IDisposable Members

        public void Dispose() {
            
        }

        #endregion

    }
}
