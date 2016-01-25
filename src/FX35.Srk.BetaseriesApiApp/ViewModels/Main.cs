

namespace Srk.BetaseriesApiApp.ViewModels
{
    using Srk.BetaseriesApi;
    public class Main : CommonViewModel
    {
        private string apiKey;
        private ShowInfo showInfo;
        private ShowsSearch showsSearch;
        private Login login;
        private Version version;
        private Badges badges;
        private TimelineVM mainTimeline;
        private TimelineVM ownTimeline;
        private TimelineVM friendsTimeline;
        private DebugVM debug;
        private ShowEpisodesComments episodesComments;
        private ShowEpisodesSubtitles episodesSubtitles;
        private UserBadges userBadges;
        private FriendsVM friends;
        private QueryVM query;

        public Main()
        {
        }

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


        public ShowInfo ShowInfo
        {
            get { return this.showInfo ?? (this.showInfo = new ShowInfo(this)); }
        }

        public ShowsSearch ShowsSearch
        {
            get { return this.showsSearch ?? (this.showsSearch = new ShowsSearch(this)); }
        }

        public Login Login
        {
            get { return this.login ?? (this.login = new Login(this)); }
        }

        public Version Version
        {
            get { return this.version ?? (this.version = new Version(this)); }
        }

        public Badges Badges
        {
            get { return this.badges ?? (this.badges = new Badges(this)); }
        }

        public TimelineVM MainTimeline
        {
            get { return this.mainTimeline ?? (this.mainTimeline = new TimelineVM(this, TimelineKind.Main)); }
        }

        public TimelineVM OwnTimeline
        {
            get { return this.ownTimeline ?? (this.ownTimeline = new TimelineVM(this, TimelineKind.Own)); }
        }

        public TimelineVM FriendsTimeline
        {
            get { return this.friendsTimeline ?? (this.friendsTimeline = new TimelineVM(this, TimelineKind.Friends)); }
        }

        public DebugVM Debug
        {
            get { return this.debug ?? (this.debug = new DebugVM()); }
        }

        public ShowEpisodesComments EpisodesComments
        {
            get { return this.episodesComments ?? (this.episodesComments = new ShowEpisodesComments(this)); }
        }

        public ShowEpisodesSubtitles EpisodesSubtitles
        {
            get { return this.episodesSubtitles ?? (this.episodesSubtitles = new ShowEpisodesSubtitles(this)); }
        }

        public UserBadges UserBadges
        {
            get { return this.userBadges ?? (this.userBadges = new UserBadges(this)); }
        }

        public FriendsVM Friends
        {
            get { return this.friends ?? (this.friends = new FriendsVM(this)); }
        }

        public QueryVM Query
        {
            get { return this.query ?? (this.query = new QueryVM(this)); }
        }
    }
}
