using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Srk.BetaseriesApi;
using System;
using Srk.BetaseriesApiApp.Properties;
using System.Threading.Tasks;

namespace Srk.BetaseriesApiApp.ViewModels {
    public class Login : CommonViewModel {

        public event EventHandler StatusChangedEvent;

        protected IBetaseriesTaskAsyncApi TaskAsyncClient {
            get { return this.client; }
        }

        public Login(Main main) : base(main) {
            if (!string.IsNullOrEmpty(Settings.Default.Username)) {
                _username = Settings.Default.Username;
                _saveUsername = true;
            }

            if (!string.IsNullOrEmpty(Settings.Default.Password)) {

            }
        }

        #region View properties

        public Member CurrentMember {
            get { return _currentMember; }
            set {
                if (_currentMember != value) {
                    _currentMember = value;
                    RaisePropertyChanged("CurrentMember");
                    RaisePropertyChanged("IsLoggedIn");
                    RaisePropertyChanged("CanLoginProp");
                }
            }
        }
        private Member _currentMember;

        public string Username {
            get { return _username; }
            set {
                if (_username != value) {
                    _username = value;
                    RaisePropertyChanged("Username");
                    RaisePropertyChanged("CanLoginProp");
                }
            }
        }
        private string _username;

        public string SessionToken {
            get { return client.SessionToken; }
            set {
                RaisePropertyChanged("SessionToken");
            }
        }

        public bool IsLoggedIn {
            get { return CurrentMember != null; }
        }

        public bool SaveUsername {
            get { return _saveUsername; }
            set {
                if (_saveUsername != value) {
                    _saveUsername = value;
                    RaisePropertyChanged("SaveUsername");
                    if (!value)
                        SavePassword = false;
                }
            }
        }
        private bool _saveUsername;

        public bool SavePassword {
            get { return _savePassword; }
            set {
                if (_savePassword != value && SaveUsername) {
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
        public ICommand LoginCommand {
            get {
                if (_loginCommand == null) {
                    _loginCommand = new RelayCommand<string>(OnLogin, CanLogin);
                }
                return _loginCommand;
            }
        }
        private ICommand _loginCommand;

        private void OnLogin(string param) {
            DoLogin(Username, param);
        }

        private bool CanLogin(string param) {
            return !string.IsNullOrEmpty(Username) && CurrentMember == null && IsNotBusy;
        }

        public bool CanLoginProp {
            get { return CanLogin(null); }
        }

        /// <summary>
        /// Logoff
        /// To be bound in the view.
        /// </summary>
        public ICommand LogoffCommand {
            get {
                if (_logoffCommand == null) {
                    _logoffCommand = new RelayCommand(
                        OnLogoff, CanLogoff);
                }
                return _logoffCommand;
            }
        }
        private ICommand _logoffCommand;

        private void OnLogoff() {
            DoLogoff();
        }

        private bool CanLogoff() {
            return CurrentMember != null && IsNotBusy;
        }

        #endregion

        private async Task DoLogin(string username, string password) {
            UpdateStatus("Authenticating...", true);
            try {
                var result = await client.AuthenticateTaskAsync(username, password);
                //var result = await Task.ConfigureAwait(client.AuthenticateTaskAsync(username, password), true);
                //var result = await client.AuthenticateTaskAsync(username, password).ConfigureAwait(true);
                SessionToken = result;
                UpdateStatus("Authenticated. Fetching User informations...", true);
                DoFetchUserInfo();

                if (SaveUsername) {
                    try {
                        Settings.Default.Username = client.SessionUsername;
                        Settings.Default.Save();
                    } catch { }
                }
            } catch (Exception ex) {
                UpdateStatus("Authentication error.", ex, false);
            }

            var handler = this.StatusChangedEvent;
            if (handler != null) {
                handler(this, new EventArgs());
            }
        }

        private async Task DoLogoff() {
            UpdateStatus("Logging-off...", true);
            try {
                await client.LogoffTaskAsync();
                SessionToken = client.SessionToken;
                CurrentMember = null;
                UpdateStatus("Logged-off.", false);
            } catch (Exception ex) {
                UpdateStatus("Failed to log-off.", ex, false);
            }

            var handler = this.StatusChangedEvent;
            if (handler != null) {
                handler(this, new EventArgs());
            }
        }

        private async Task DoFetchUserInfo() {
            UpdateStatus("Fetching User informations...", true);
            try {
                var member = await client.GetMemberTaskAsync(null);
                CurrentMember = member;
#if DEBUG
                UpdateStatus("Authenticated as " + client.SessionUsername + " (" + client.SessionToken + "). ", false);
#else
                UpdateStatus("Authenticated as " + client.SessionUsername + ". ", false);
#endif
            } catch (Exception ex) {
#if DEBUG
                UpdateStatus("Authenticated as " + client.SessionUsername + " (" + client.SessionToken + "). An error occured while fetching user information. ", ex, false);
#else
                UpdateStatus("Authenticated as " + client.SessionUsername + ". An error occured while fetching user information. ", ex, false);
#endif
            }
        }

        protected override void OnBusyStateChanged() {
            RaisePropertyChanged("CanLoginProp");
        }
    }
}
