

using Srk.BetaseriesApi;
namespace Srk.BetaseriesApiApp.ViewModels {
    public class Main : CommonViewModel {

        public string ApiKey
        {
            get { return this.apiKey; }
            set
            {
                if (this.apiKey != value)
                {
                    this.apiKey = value;
                    this.RaisePropertyChanged("ApiKey");
                    BetaseriesClientFactory.Default = new BetaseriesClientFactory(
                        ApiKey, AppVersion.ApplicationUserAgent, true);
                }
            }
        }
        private string apiKey;
        

        public ShowInfo ShowInfo {
            get { return _showInfo ?? (_showInfo = new ShowInfo(this)); }
        }
        private ShowInfo _showInfo;

        public ShowsSearch ShowsSearch {
            get { return _showsSearch ?? (_showsSearch = new ShowsSearch(this)); }
        }
        private ShowsSearch _showsSearch;

        public Login Login {
            get { return _login ?? (_login = new Login(this)); }
        }
        private Login _login;

        public Version Version {
            get { return _version ?? (_version = new Version(this)); }
        }
        private Version _version;

        public Badges Badges {
            get { return _badges ?? (_badges = new Badges(this)); }
        }
        private Badges _badges;
        
        public TimelineVM MainTimeline {
            get { return _mainTimeline ?? (_mainTimeline = new TimelineVM(this, TimelineKind.Main)); }
        }
        private TimelineVM _mainTimeline;

        public TimelineVM OwnTimeline {
            get { return _ownTimeline ?? (_ownTimeline = new TimelineVM(this, TimelineKind.Own)); }
        }
        private TimelineVM _ownTimeline;

        public TimelineVM FriendsTimeline {
            get { return _friendsTimeline ?? (_friendsTimeline = new TimelineVM(this, TimelineKind.Friends)); }
        }
        private TimelineVM _friendsTimeline;

        public DebugVM Debug {
            get { return _debug ?? (_debug = new DebugVM()); }
        }
        private DebugVM _debug;

        public ShowEpisodesComments EpisodesComments {
            get { return _episodesComments ?? (_episodesComments = new ShowEpisodesComments(this)); }
        }
        private ShowEpisodesComments _episodesComments;

        public ShowEpisodesSubtitles EpisodesSubtitles
        {
            get { return _episodesSubtitles ?? (_episodesSubtitles = new ShowEpisodesSubtitles(this)); }
        }
        private ShowEpisodesSubtitles _episodesSubtitles;

        public UserBadges UserBadges {
            get { return _userBadges ?? (_userBadges = new UserBadges(this)); }
        }
        private UserBadges _userBadges;

        public FriendsVM Friends {
            get { return _friends ?? (_friends = new FriendsVM(this)); }
        }
        private FriendsVM _friends;

        public QueryVM Query {
            get { return _query ?? (_query = new QueryVM(this)); }
        }
        private QueryVM _query;
        
        public Main() {
        }

    }
}
