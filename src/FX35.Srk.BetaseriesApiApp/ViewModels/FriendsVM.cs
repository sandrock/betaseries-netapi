using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Srk.BetaseriesApi;

namespace Srk.BetaseriesApiApp.ViewModels
{
    public class FriendsVM : CommonViewModel
    {

        #region Composition parts

        #endregion

        #region View properties

        public string Username
        {
            get { return _username; }
            set { SetValue(ref _username, value, "Username"); }
        }
        private string _username;

        public ObservableCollection<Member> Members { get { return _members; } }
        private readonly ObservableCollection<Member> _members = new ObservableCollection<Member>();

        #endregion

        #region .ctor

        public FriendsVM(Main main)
            : base(main)
        {
        }

        #endregion

        #region Commands

        #region GetFriends command

        /// <summary>
        /// GetFriends
        /// To be bound in the view.
        /// </summary>
        public ICommand GetFriendsCommand
        {
            get
            {
                return _getFriendsCommand ?? (_getFriendsCommand = new RelayCommand(OnGetFriends, CanGetFriends));
            }
        }
        private ICommand _getFriendsCommand;

        private void OnGetFriends()
        {
            LoadFriends(Username);
        }

        private bool CanGetFriends()
        {
            return !IsBusy;
        }

        #endregion

        #region Stop command

        /// <summary>
        /// Stop
        /// To be bound in the view.
        /// </summary>
        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ?? (_stopCommand = new RelayCommand(OnStop, CanStop));
            }
        }
        private ICommand _stopCommand;

        private void OnStop()
        {
            membersToFetch.Clear();
        }

        private bool CanStop()
        {
            return IsBusy;
        }

        #endregion

        #endregion

        #region ShowErrors command

        /// <summary>
        /// ShowErrors
        /// To be bound in the view.
        /// </summary>
        public ICommand ShowErrorsCommand
        {
            get
            {
                return _showErrorsCommand ?? (_showErrorsCommand = new RelayCommand(OnShowErrors, CanShowErrors));
            }
        }
        private ICommand _showErrorsCommand;

        private void OnShowErrors()
        {
            if (errors == null || errors.Count == 0)
            {
                MessageBox.Show("No errors. ");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Error list");
            sb.AppendLine("==========");
            sb.AppendLine();

            foreach (var e in errors)
            {
                sb.AppendLine(e.GetType().FullName);
                sb.AppendLine(e.Message);
                sb.AppendLine();
            }
            MessageBox.Show(sb.ToString());
        }

        private bool CanShowErrors()
        {
            return true;
        }

        #endregion

        #region Async Responses

        void client_GetFriendsEnded(object sender, AsyncResponseArgs<IList<string>> e)
        {
            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Succeed)
                {
                    UpdateStatus("Done.", true);
                    _members.Clear();

                    foreach (var item in e.Data)
                    {
                        _members.Add(new Member
                        {
                            Username = item
                        });
                        membersToFetch.Enqueue(item);
                    }
                    StartFetchNextMember();
                }
                else
                {
                    UpdateStatus(e.Message, false);
                }
            }));
        }

        void client_GetMemberEnded(object sender, AsyncResponseArgs<Member> e)
        {
            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Succeed)
                {
                    var member = Members.SingleOrDefault(m => m.Username == e.Data.Username);
                    if (member == null)
                    {
                        errors.Add(new NullReferenceException("No member in query result. "));
                    }
                    else
                    {
                        Members.Remove(member);
                        Members.Add(e.Data);
                    }
                }
                else
                {
                    errors.Add(e.Error);
                }
                FetchNextMember();
            }));
        }

        #endregion

        #region Internal stuff

        private void LoadFriends(string username)
        {
            client.GetFriendsAsync(username, client_GetFriendsEnded);
            UpdateStatus("Fetching friends...", true);
        }

        private readonly Queue<string> membersToFetch = new Queue<string>();
        private int membersCountToFetch = 0;
        private int membersCountFetched = 0;
        private readonly List<Exception> errors = new List<Exception>();

        private void StartFetchNextMember()
        {
            membersCountToFetch = membersToFetch.Count;
            membersCountFetched = 0;

            FetchNextMember();
        }

        private void FetchNextMember()
        {
            string nextUsername = membersToFetch.Count > 0 ? membersToFetch.Dequeue() : null;

            if (nextUsername == null)
            {
                if (errors.Count > 0)
                    UpdateStatus("Done (" + errors.Count + " errors).", false);
                else
                    UpdateStatus("Done.", false);
            }
            else
            {
                UpdateStatus("Fetching member details (" + ++membersCountFetched + " /" + membersCountToFetch + ")...", true);
                try
                {
                    client.GetMemberAsync(nextUsername, client_GetMemberEnded);
                }
                catch { }
            }
        }

        #endregion
    }
}
