using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Srk.BetaseriesApi;

namespace Srk.BetaseriesApiApp.ViewModels
{
    public class UserBadges : CommonViewModel
    {
        #region View properties

        public string Username
        {
            get { return _username; }
            set { SetValue(ref _username, value, "Username"); }
        }
        private string _username;

        public ObservableCollection<BadgeVM> Badges { get { return _badges; } }
        private readonly ObservableCollection<BadgeVM> _badges = new ObservableCollection<BadgeVM>();

        #endregion

        #region .ctor

        public UserBadges(Main main)
            : base(main)
        {
        }

        #endregion

        #region Commands

        #region GetBadges command

        /// <summary>
        /// Fetch badges
        /// To be bound in the view.
        /// </summary>
        public ICommand GetBadgesCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return _getBadgesCommand ?? (_getBadgesCommand = new RelayCommand(OnGetBadges, CanGetBadges));
            }
        }
        private ICommand _getBadgesCommand;

        private void OnGetBadges()
        {
            FetchBadges(_username);
        }

        private bool CanGetBadges()
        {
            return !string.IsNullOrEmpty(_username);
        }

        #endregion

        #endregion

        #region Internal stuff

        private void FetchBadges(string username)
        {
            UpdateStatus("Fetching badges from user '" + username + "'...", true);
            client.GetBadgesAsync(username, client_GetBadgesEnded);
        }

        #endregion

        #region Async Responses

        void client_GetBadgesEnded(object sender, AsyncResponseArgs<string[]> e)
        {
            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Succeed)
                {
                    UpdateStatus("Done.", false);
                    _badges.Clear();

                    foreach (var item in e.Data)
                    {
                        _badges.Add(new BadgeVM
                        {
                            InternalType = BadgesUtil.Parse(item),
                            ServiceName = item,
                            TranslatedName = BadgesUtil.GetName(item),
                            TranslatedDescription = BadgesUtil.GetDescription(item)
                        });
                    }
                }
                else
                {
                    UpdateStatus(e.Message, false);
                }
            }));
        }

        #endregion

        #region Helpers

        #endregion

        #region Cleanup

        public override void Cleanup()
        {
            base.Cleanup();
        }

        #endregion

    }
}
