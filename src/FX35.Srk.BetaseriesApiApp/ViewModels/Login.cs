
namespace Srk.BetaseriesApiApp.ViewModels
{
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Srk.BetaseriesApi;
    using System;
    using Srk.BetaseriesApiApp.Properties;
    using System.Threading;
    using System.Windows.Threading;

    public class Login : CommonViewModel
    {
        public event EventHandler StatusChangedEvent;

        public Login(Main main)
            : base(main)
        {
            if (!string.IsNullOrEmpty(Settings.Default.Username))
            {
                _username = Settings.Default.Username;
                _saveUsername = true;
            }

            if (!string.IsNullOrEmpty(Settings.Default.Password))
            {

            }
        }

        #region View properties

        public Member CurrentMember
        {
            get { return _currentMember; }
            set
            {
                if (_currentMember != value)
                {
                    _currentMember = value;
                    RaisePropertyChanged("CurrentMember");
                    RaisePropertyChanged("IsLoggedIn");
                    RaisePropertyChanged("CanLoginProp");
                }
            }
        }
        private Member _currentMember;

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged("Username");
                    RaisePropertyChanged("CanLoginProp");
                }
            }
        }
        private string _username;

        public string SessionToken
        {
            get { return client.SessionToken; }
            set
            {
                RaisePropertyChanged("SessionToken");
            }
        }

        public bool IsLoggedIn
        {
            get { return CurrentMember != null; }
        }

        public bool SaveUsername
        {
            get { return _saveUsername; }
            set
            {
                if (_saveUsername != value)
                {
                    _saveUsername = value;
                    RaisePropertyChanged("SaveUsername");
                    if (!value)
                        SavePassword = false;
                }
            }
        }
        private bool _saveUsername;

        public bool SavePassword
        {
            get { return _savePassword; }
            set
            {
                if (_savePassword != value && SaveUsername)
                {
                    _savePassword = value;
                    RaisePropertyChanged("SavePassword");
                }
            }
        }
        private bool _savePassword;

        #endregion

        #region Commands

        /// <summary>
        /// Login
        /// To be bound in the view.
        /// </summary>
        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand<string>(OnLogin, CanLogin);
                }
                return _loginCommand;
            }
        }
        private ICommand _loginCommand;

        private void OnLogin(string param)
        {
            DoLogin(Username, param);
        }

        private bool CanLogin(string param)
        {
            return !string.IsNullOrEmpty(Username) && CurrentMember == null && IsNotBusy;
        }

        public bool CanLoginProp
        {
            get { return CanLogin(null); }
        }

        /// <summary>
        /// Logoff
        /// To be bound in the view.
        /// </summary>
        public ICommand LogoffCommand
        {
            get
            {
                if (_logoffCommand == null)
                {
                    _logoffCommand = new RelayCommand(
                        OnLogoff, CanLogoff);
                }
                return _logoffCommand;
            }
        }
        private ICommand _logoffCommand;

        private void OnLogoff()
        {
            DoLogoff();
        }

        private bool CanLogoff()
        {
            return CurrentMember != null && IsNotBusy;
        }

        #endregion

        private void DoLogin(string username, string password)
        {
            UpdateStatus("Authenticating...", true);
            var dsp = Dispatcher.CurrentDispatcher;
            ////client.AuthenticateAsync(username, password, client_AuthenticateEnded);
            
            ThreadPool.QueueUserWorkItem(_ => 
            {
                string message = null,  status=null;
                bool isBusy = false;
                try
                {
                    var e = this.Client2.Members.ClassicAuthenticate(username, password);
                    message = e.ToString();
                    status = "Authenticated. Fetching User informations...";
                }
                catch (Exception ex)
                {
                    message = ex.ToString();
                }
                /*
                if (e.Succeed)
                {
                    SessionToken = e.Data;
                    UpdateStatus("Authenticated. Fetching User informations...", true);
                    DoFetchUserInfo();

                    if (SaveUsername)
                    {
                        try
                        {
                            Settings.Default.Username = client.SessionUsername;
                            Settings.Default.Save();
                        }
                        catch { }
                    }
                }
                else
                {
                    UpdateStatus("Authentication error.", e.Error, false);
                }

                var handler = this.StatusChangedEvent;
                if (handler != null)
                {
                    handler(this, new EventArgs());
                }*/

                dsp.BeginInvoke(new Action(() =>
                {
                    UpdateStatus(status, isBusy);
                    System.Windows.MessageBox.Show(message);
                }));
            });
        }

        private void DoLogoff()
        {
            UpdateStatus("Logging-off...", true);
            client.LogoffAsync(client_LogoffEnded);
        }

        private void DoFetchUserInfo()
        {
            UpdateStatus("Fetching User informations...", true);
            client.GetMemberAsync(null, client_GetMemberEnded);
        }

        #region Async responses

        void client_AuthenticateEnded(object sender, AsyncResponseArgs<string> e)
        {
            if (e.Succeed)
            {
                SessionToken = e.Data;
                UpdateStatus("Authenticated. Fetching User informations...", true);
                DoFetchUserInfo();

                if (SaveUsername)
                {
                    try
                    {
                        Settings.Default.Username = client.SessionUsername;
                        Settings.Default.Save();
                    }
                    catch { }
                }
            }
            else
            {
                UpdateStatus("Authentication error.", e.Error, false);
            }

            var handler = this.StatusChangedEvent;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        void client_LogoffEnded(object sender, AsyncResponseArgs e)
        {
            if (e.Succeed)
            {
                SessionToken = client.SessionToken;
                CurrentMember = null;
                UpdateStatus("Logged-off.", false);
            }
            else
            {
                UpdateStatus("Failed to log-off.", e.Error, false);
            }

            var handler = this.StatusChangedEvent;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        void client_GetMemberEnded(object sender, AsyncResponseArgs<Member> e)
        {
            if (e.Succeed)
            {
                CurrentMember = e.Data;
#if DEBUG
                UpdateStatus("Authenticated as " + client.SessionUsername + " (" + client.SessionToken + "). ", false);
#else
                UpdateStatus("Authenticated as " + client.SessionUsername + ". ", false);
#endif
            }
            else
            {
#if DEBUG
                UpdateStatus("Authenticated as " + client.SessionUsername + " (" + client.SessionToken + "). An error occured while fetching user information. ", e.Error, false);
#else
                UpdateStatus("Authenticated as " + client.SessionUsername + ". An error occured while fetching user information. ", e.Error, false);
#endif
            }
        }

        #endregion

        //protected override void Dispose(bool disposing) {
        //    client.AuthenticateEnded -= client_AuthenticateEnded;
        //    client.LogoffEnded -= client_LogoffEnded;
        //    client.GetMemberEnded -= client_GetMemberEnded;

        //    base.Dispose(disposing);
        //}

        protected override void OnBusyStateChanged()
        {
            RaisePropertyChanged("CanLoginProp");
        }

    }
}
