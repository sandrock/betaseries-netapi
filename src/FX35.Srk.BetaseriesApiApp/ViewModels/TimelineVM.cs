using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Srk.BetaseriesApi;

namespace Srk.BetaseriesApiApp.ViewModels
{

    public enum TimelineKind
    {
        Main, Own, Friends
    }

    public class TimelineVM : CommonViewModel
    {

        private readonly TimelineKind kind;

        #region Properties

        public ObservableCollection<TimelineItem> Items
        {
            get { return _items; }
        }
        private readonly ObservableCollection<TimelineItem> _items = new ObservableCollection<TimelineItem>();

        public ICollectionView ItemsCV
        {
            get
            {
                if (_itemsCV == null)
                {
                    _itemsCV = CollectionViewSource.GetDefaultView(_items);
                    _itemsCV.Filter = methodsCvFilter;
                    _itemsCV.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
                }
                return _itemsCV;
            }
        }
        private ICollectionView _itemsCV;
        private bool methodsCvFilter(object o)
        {
            var vm = o as TimelineItem;
            if (vm == null)
                return true;

            bool show = true;

            if (SelectedType != null && SelectedType != "All")
            {
                show &= vm.Type.ToString() == SelectedType;
            }

            return show;
        }

        public bool IsAutoRefreshing
        {
            get { return _isAutoRefreshing; }
            set
            {
                if (_isAutoRefreshing != value)
                {
                    if (timer != null)
                        timer.Dispose();
                    _isAutoRefreshing = value;
                    RaisePropertyChanged("IsAutoRefreshing");
                    if (value)
                        timer = new System.Threading.Timer(timer_Tick, this, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(3));
                    else
                        timer = null;
                }
            }
        }
        private bool _isAutoRefreshing;

        private System.Threading.Timer timer;

        public string[] Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new string[] {
                        "All",
                        TimelineItemType.BadgeEarned.ToString(),
                        TimelineItemType.Comment.ToString(),
                        TimelineItemType.CommentOnEpisode.ToString(),
                        TimelineItemType.CommentOnMember.ToString(),
                        TimelineItemType.CommentOnShow.ToString(),
                        TimelineItemType.FriendAdded.ToString(),
                        TimelineItemType.FriendRemoved.ToString(),
                        TimelineItemType.MarkedAs.ToString(),
                        TimelineItemType.Recommendation.ToString(),
                        TimelineItemType.RecommendationAccepted.ToString(),
                        TimelineItemType.RecommendationDenied.ToString(),
                        TimelineItemType.ShowAdded.ToString(),
                        TimelineItemType.ShowArchived.ToString(),
                        TimelineItemType.ShowRemoved.ToString(),
                        TimelineItemType.ShowUnarchived.ToString(),
                        TimelineItemType.Subtitles.ToString(),
                        TimelineItemType.Unknown.ToString(),
                        TimelineItemType.Update.ToString(),
                        TimelineItemType.UserRegistered.ToString()
                    };
                }
                return _types;
            }
        }
        private string[] _types;

        public string SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (SetValue(ref _selectedType, value, "SelectedType"))
                {
                    _itemsCV.Refresh();
                }
            }
        }
        private string _selectedType;


        #endregion

        #region .ctor

        public TimelineVM(Main main, TimelineKind kind)
            : base(main)
        {
            this.kind = kind;
        }

        #endregion

        #region Commands

        #region Refresh Command

        /// <summary>
        /// Refresh
        /// To be bound in the view.
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(OnRefresh, CanRefresh);
                }
                return _refreshCommand;
            }
        }
        private ICommand _refreshCommand;

        private void OnRefresh()
        {
            UpdateStatus("Updating... ", true);
            switch (kind)
            {
                case TimelineKind.Main:
                    client.GetMainTimelineAsync(35, this.client_GetTimelineEnded);
                    break;
                case TimelineKind.Own:
                    client.GetMemberTimelineAsync(null, 25, this.client_GetTimelineEnded);
                    break;
                case TimelineKind.Friends:
                    client.GetFriendsTimelineAsync(25, this.client_GetTimelineEnded);
                    break;
                default:
                    break;
            }
        }

        private bool CanRefresh()
        {
            if (kind == TimelineKind.Main)
                return IsNotBusy;
            else
                return IsNotBusy && main.Login.IsLoggedIn;
        }

        #endregion

        #region Clear Command

        /// <summary>
        /// Clear
        /// To be bound in the view.
        /// </summary>
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand(OnClear, CanClear);
                }
                return _clearCommand;
            }
        }
        private ICommand _clearCommand;

        private void OnClear()
        {
            Items.Clear();
        }

        private bool CanClear()
        {
            return true;
        }

        #endregion

        #endregion

        #region Async responses

        void client_GetTimelineEnded(object sender, AsyncResponseArgs<IList<TimelineItem>> e)
        {
            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Succeed)
                {
                    UpdateStatus("Sucess. ", false);
                    int n = 0;
                    foreach (var item in e.Data)
                    {
                        if (Items.Any(i => i.Date == item.Date && i.Username == item.Username))
                            continue;

                        Items.Add(item);
                        n++;
                    }
                    UpdateStatus("Sucess (" + n + " new items). ", false);
                }
                else
                {
                    UpdateStatus("Failed: " + e.Error.Message, e.Error, false);
                }
            }));
        }

        static void timer_Tick(object state)
        {
            var vm = state as TimelineVM;

            if (vm.IsAutoRefreshing)
                vm.RefreshCommand.Execute(null);
        }

        #endregion

        #region Internal stuff

        #endregion

        public override void Cleanup()
        {
            base.Cleanup();
        }

    }
}
