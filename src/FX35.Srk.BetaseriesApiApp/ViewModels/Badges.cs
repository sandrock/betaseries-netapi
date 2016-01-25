
namespace Srk.BetaseriesApiApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Data;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using Srk.BetaseriesApi;
    using Srk.BetaseriesApiApp.Properties;

    public class BadgeVM
    {
        public string ServiceName { get; set; }
        public Srk.BetaseriesApi.Badges InternalType { get; set; }
        public bool IsImplemented { get; set; }
        public bool IsVerified { get; set; }
        public string TranslatedName { get; set; }
        public string TranslatedDescription { get; set; }
    }

    public class Badges : CommonViewModel
    {
        private readonly ObservableCollection<BadgeVM> _badges = new ObservableCollection<BadgeVM>();
        private ICollectionView _methodsCV;

        #region Properties

        public ObservableCollection<BadgeVM> BadgesCollection
        {
            get { return _badges; }
        }

        public ICollectionView BadgesCV
        {
            get
            {
                if (_methodsCV == null)
                {
                    //_methodsCV = new CollectionView(Methods);
                    _methodsCV = CollectionViewSource.GetDefaultView(BadgesCollection);
                    _methodsCV.Filter = badgesCvFilter;
                    _methodsCV.SortDescriptions.Add(new SortDescription("ServiceName", ListSortDirection.Ascending));
                }
                return _methodsCV;
            }
        }

        private bool badgesCvFilter(object o)
        {
            var vm = o as BadgeVM;
            if (vm == null)
                return true;

            bool show = true;
            if (FilterImplemented.HasValue)
            {
                if (FilterImplemented.Value)
                {
                    show &= vm.InternalType != Srk.BetaseriesApi.Badges.Unknown;
                }
                else
                {
                    show &= vm.InternalType == Srk.BetaseriesApi.Badges.Unknown;
                }
            }
            if (FilterNameTranslated.HasValue)
            {
                if (FilterNameTranslated.Value)
                {
                    show &= vm.TranslatedName != null;
                }
                else
                {
                    show &= vm.TranslatedName == null;
                }
            }
            if (FilterDescriptionTranslated.HasValue)
            {
                if (FilterDescriptionTranslated.Value)
                {
                    show &= vm.TranslatedDescription != null;
                }
                else
                {
                    show &= vm.TranslatedDescription == null;
                }
            }
            if (FilterVerified.HasValue)
            {
                if (FilterVerified.Value)
                {
                    show &= vm.IsVerified;
                }
                else
                {
                    show &= !vm.IsVerified;
                }
            }
            if (FilterRequireAttention.HasValue)
            {
                bool re = !vm.IsVerified || !vm.IsImplemented || string.IsNullOrEmpty(vm.TranslatedName) || string.IsNullOrEmpty(vm.TranslatedDescription);
                if (FilterRequireAttention.Value)
                {
                    show &= re;
                }
                else
                {
                    show &= !re;
                }
            }
            return show;
        }

        public bool? FilterImplemented
        {
            get { return _filterImplemented; }
            set
            {
                if (_filterImplemented != value)
                {
                    _filterImplemented = value;
                    RaisePropertyChanged("FilterImplemented");
                    BadgesCV.Refresh();
                }
            }
        }
        private bool? _filterImplemented;

        public bool? FilterNameTranslated
        {
            get { return _filterNameTranslated; }
            set
            {
                if (_filterNameTranslated != value)
                {
                    _filterNameTranslated = value;
                    RaisePropertyChanged("FilterNameTranslated");
                    BadgesCV.Refresh();
                }
            }
        }
        private bool? _filterNameTranslated;

        public bool? FilterDescriptionTranslated
        {
            get { return _filterDescriptionTranslated; }
            set
            {
                if (_filterDescriptionTranslated != value)
                {
                    _filterDescriptionTranslated = value;
                    RaisePropertyChanged("FilterDescriptionTranslated");
                    BadgesCV.Refresh();
                }
            }
        }
        private bool? _filterDescriptionTranslated;

        public bool? FilterVerified
        {
            get { return _filterVerified; }
            set
            {
                if (_filterVerified != value)
                {
                    _filterVerified = value;
                    RaisePropertyChanged("FilterVerified");
                    BadgesCV.Refresh();
                }
            }
        }
        private bool? _filterVerified;

        public bool? FilterRequireAttention
        {
            get { return _filterRequireAttention; }
            set
            {
                if (_filterRequireAttention != value)
                {
                    _filterRequireAttention = value;
                    RaisePropertyChanged("FilterRequireAttention");
                    BadgesCV.Refresh();
                }
            }
        }
        private bool? _filterRequireAttention = true;

        public string[] Stats
        {
            get { return _stats; }
            set
            {
                if (_stats != value)
                {
                    _stats = value;
                    RaisePropertyChanged("Stats");
                }
            }
        }
        private string[] _stats;

        public bool IsAutoUpdateEnabled
        {
            get { return _isAutoUpdateEnabled; }
            set
            {
                if (_isAutoUpdateEnabled != value)
                {
                    if (timer != null)
                        timer.Dispose();
                    _isAutoUpdateEnabled = value;
                    RaisePropertyChanged("IsAutoUpdateEnabled");
                    if (value)
                        timer = new System.Threading.Timer(timer_Tick, this, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(10));
                    else
                        timer = null;
                }
            }
        }
        private bool _isAutoUpdateEnabled;

        private System.Threading.Timer timer;

        public ObservableCollection<BadgesLog> Log
        {
            get { return _log; }
        }
        private readonly ObservableCollection<BadgesLog> _log = new ObservableCollection<BadgesLog>();

        public ICollectionView LogCV
        {
            get
            {
                if (_logCV == null)
                {
                    _logCV = CollectionViewSource.GetDefaultView(Log);
                    _logCV.Filter = logCvFilter;
                    _logCV.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
                }
                return _logCV;
            }
        }
        private ICollectionView _logCV;
        private bool logCvFilter(object o)
        {
            var vm = o as BadgesLog;
            if (vm == null)
                return true;

            bool show = true;
            if (!string.IsNullOrEmpty(LogSearch))
            {
                show = false;
                foreach (var item in vm.Entries)
                {
                    if (item.Value.Any(i => i.Contains(_logSearch)))
                        show = true;
                }
            }
            return show;
        }

        public string LogSearch
        {
            get { return _logSearch; }
            set
            {
                if (_logSearch != value)
                {
                    _logSearch = value;
                    RaisePropertyChanged("LogSearch");
                    _logCV.Refresh();
                }
            }
        }
        private string _logSearch;

        public string Username
        {
            get { return _username; }
            set { SetValue(ref _username, value, "Username"); }
        }
        private string _username;

        private uint successCounter;
        private uint failureCounter;

        #endregion

        public Badges(Main main)
            : base(main)
        {
            ComputeBadges(null);
        }

        #region Commands

        #region Update Command

        /// <summary>
        /// Update
        /// To be bound in the view.
        /// </summary>
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new RelayCommand(OnUpdate, CanUpdate);
                }
                return _updateCommand;
            }
        }
        private ICommand _updateCommand;

        private void OnUpdate()
        {
            UpdateStatus("Searching timeline...", true);
            client.GetMainTimelineAsync(100, this.client_GetMainTimelineAsyncEnded);
        }

        private bool CanUpdate()
        {
            return IsNotBusy;
        }

        #endregion

        #region Update from self

        /// <summary>
        /// UpdateSelfCommand
        /// To be bound in the view.
        /// </summary>
        public ICommand UpdateSelfCommand
        {
            get
            {
                if (_updateSelfCommand == null)
                {
                    _updateSelfCommand = new RelayCommand(OnUpdateSelf, CanUpdateSelf);
                }
                return _updateSelfCommand;
            }
        }
        private ICommand _updateSelfCommand;

        private void OnUpdateSelf()
        {
            UpdateStatus("Searching in your timeline...", true);
            client.GetMemberTimelineAsync(null, 100, this.client_GetMainTimelineAsyncEnded);
        }

        private bool CanUpdateSelf()
        {
            return main.Login.IsLoggedIn && IsNotBusy;
        }

        #endregion

        #region Update from friends

        /// <summary>
        /// UpdateFriends
        /// To be bound in the view.
        /// </summary>
        public ICommand UpdateFriendsCommand
        {
            get
            {
                if (_updateFriendsCommand == null)
                {
                    _updateFriendsCommand = new RelayCommand(OnUpdateFriends, CanUpdateFriends);
                }
                return _updateFriendsCommand;
            }
        }
        private ICommand _updateFriendsCommand;

        private void OnUpdateFriends()
        {
            UpdateStatus("Searching in your friends timeline...", true);
            client.GetFriendsTimelineAsync(100, this.client_GetMainTimelineAsyncEnded);
        }

        private bool CanUpdateFriends()
        {
            return main.Login.IsLoggedIn && IsNotBusy;
        }

        #endregion

        #region Update from user

        /// <summary>
        /// UpdateFriends
        /// To be bound in the view.
        /// </summary>
        public ICommand UpdateUserCommand
        {
            get
            {
                if (_updateUserCommand == null)
                {
                    _updateUserCommand = new RelayCommand(OnUpdateUser, CanUpdateUser);
                }
                return _updateUserCommand;
            }
        }
        private ICommand _updateUserCommand;

        private void OnUpdateUser()
        {
            if (_username == null || _username.Length < 2)
            {
                StatusMessage = "Please enter a valid username!";
                return;
            }

            UpdateStatus("Searching from user '" + _username + "'...", true);
            client.GetBadgesAsync(_username, this.client_GetBadgesEnded);
        }

        private bool CanUpdateUser()
        {
            return !IsBusy;
        }

        #endregion

        #region Clear log command

        /// <summary>
        /// ClearLog
        /// To be bound in the view.
        /// </summary>
        public ICommand ClearLogCommand
        {
            get
            {
                if (_clearLogCommand == null)
                {
                    _clearLogCommand = new RelayCommand(OnClearLog);
                }
                return _clearLogCommand;
            }
        }
        private ICommand _clearLogCommand;

        private void OnClearLog()
        {
            Log.Clear();
        }

        #endregion

        #endregion

        #region Async responses

        void client_GetMainTimelineAsyncEnded(object sender, AsyncResponseArgs<IList<TimelineItem>> e)
        {
            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Succeed)
                {
                    UpdateStatus("Succeed. ", false);
                    ComputeBadgesFromTimeline(e);
                }
                else
                {
                    UpdateStatus("Failed  [" + ++failureCounter + "/" + (successCounter + failureCounter) + "]. " + e.Message, false);
                }
            }));
        }

        private void ComputeBadgesFromTimeline(AsyncResponseArgs<IList<TimelineItem>> e)
        {
            var apiBadges = e.Data
                .Where(i => i.Type == TimelineItemType.BadgeEarned)
                .GroupBy(i => i.Reference)
                .Select(s => s.Key);
            var currentBadges = BadgesCollection.Select(y => y.ServiceName.ToString());

            int n = 0;
            foreach (var item in apiBadges)
            {
                if (!currentBadges.Any(i => i == item))
                    n++;
            }
            UpdateStatus("Succeed [" + ++successCounter + "/" + (successCounter + failureCounter) + "] (" + n + " new badges). ", false);

            ComputeBadges(e.Data);
        }

        void client_GetBadgesEnded(object sender, AsyncResponseArgs<string[]> e)
        {
            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Succeed)
                {
                    UpdateStatus("Succeed. ", false);
                    ComputeBadgesFromUser(e);
                }
                else
                {
                    UpdateStatus("Failed. " + e.Message, false);
                }
            }));
        }

        private void ComputeBadgesFromUser(AsyncResponseArgs<string[]> e)
        {
            var currentBadges = BadgesCollection.Select(y => y.ServiceName.ToString());

            int n = 0;
            foreach (var item in e.Data)
            {
                if (!currentBadges.Any(i => i == item))
                    n++;
            }
            UpdateStatus("Succeed (" + n + " new badges). ", false);

            var timelineItems = e.Data
                .Select(i => new TimelineItem(i, _username, "badge", DateTime.Now, null, null));

            ComputeBadges(timelineItems);
        }

        static void timer_Tick(object state)
        {
            var vm = state as Badges;

            if (vm.IsAutoUpdateEnabled)
                vm.UpdateCommand.Execute(null);
        }

        #endregion

        protected DateTime? last;

        private void ComputeBadges(IEnumerable<TimelineItem> e)
        {
            BadgesCollection.Clear();

            List<string> badgesSource = null;
            var logging = new Dictionary<string, List<string>>();

            // get already fetched badges from user settings
            string badgesStr = Settings.Default.Badges;
            if (badgesStr != null)
            {
                badgesSource = badgesStr
                    .Split(',')
                    .Where(s => s.Length > 0)
                    .Where(s => s != "lol" && s != "kikoo")
                    .ToList();
            }
            else
            {
                badgesSource = new List<string>();
            }

            // get more from the timeline
            if (e != null)
            {
                foreach (var item in e)
                {
                    if (last.HasValue && item.Date.HasValue)
                    {
                        if (last >= item.Date)
                            continue;
                    }
                    if (item.Type == TimelineItemType.BadgeEarned)
                    {
                        if (!badgesSource.Any(s => s == item.Reference))
                        {
                            badgesSource.Add(item.Reference);
                        }
                        if (logging.ContainsKey(item.Username))
                        {
                            logging[item.Username].Add(item.Reference);
                        }
                        else
                        {
                            logging[item.Username] = new List<string>() { item.Reference };
                        }
                    }
                }
            }
            if (e != null)
                last = e.Max(i => i.Date);

            // look what translations we have
            foreach (var b in badgesSource)
            {
                Srk.BetaseriesApi.Badges badge = Srk.BetaseriesApi.Badges.Unknown;
                string name = null;
                string desc = null;

                if (BadgesUtil.TryParseBadge(b, out badge))
                {
                    name = BadgesUtil.GetTranslatedName(badge);
                    desc = BadgesUtil.GetDescription(badge);
                }

                var vm = new BadgeVM
                {
                    ServiceName = b,
                    InternalType = badge,
                    TranslatedName = name,
                    TranslatedDescription = desc,
                    IsImplemented = badge != Srk.BetaseriesApi.Badges.Unknown,
                    IsVerified = true
                };

                BadgesCollection.Add(vm);
            }

            // add a log entry
            if (logging.Count > 0)
            {
                var log = new BadgesLog();
                log.Entries = logging;
                CurrentDispatcher.BeginInvoke(new Action(() =>
                    Log.Add(log)
                ));
            }

            // add unverified stuff we have
            foreach (var b in BadgesUtil.ServiceKeys)
            {
                if (badgesSource.Contains(b))
                    continue;

                Srk.BetaseriesApi.Badges ba = BadgesUtil.Parse(b);
                var vm = new BadgeVM
                {
                    ServiceName = b,
                    InternalType = ba,
                    TranslatedName = BadgesUtil.GetTranslatedName(ba),
                    TranslatedDescription = BadgesUtil.GetDescription(ba),
                    IsImplemented = true,
                    IsVerified = false
                };

                BadgesCollection.Add(vm);
            }

            // compute statistics
            Stats = new string[] {
                (BadgesCollection.Count + " total"),
                (BadgesCollection.Count(i => !i.IsVerified) + " unverified"),
                (BadgesCollection.Count(i => !i.IsImplemented) + " to implement"),
                (BadgesCollection.Count(i => i.TranslatedName != null && i.TranslatedDescription != null) + " fully translated"),
                (BadgesCollection.Count(i => i.TranslatedName == null) + " names to translate"),
                (BadgesCollection.Count(i => i.TranslatedDescription == null) + " descriptions to translate")
            };

            // save for next time
            if (e != null)
            {
                var sb = new StringBuilder();
                string sep = string.Empty;
                foreach (var item in BadgesCollection)
                {
                    if (item.ServiceName.StartsWith("test") || !item.IsVerified)
                        continue;
                    sb.Append(sep);
                    sb.Append(item.ServiceName);
                    sep = ",";
                }
                var result = sb.ToString();
                if (result != badgesStr)
                {
                    Settings.Default.Badges = result;
                    Settings.Default.Save();
                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public class BadgesLog : ViewModelBase
        {
            private readonly DateTime _date = DateTime.Now;
            public DateTime Date { get { return _date; } }
            public Dictionary<string, List<string>> Entries { get; set; }
        }
    }
}
