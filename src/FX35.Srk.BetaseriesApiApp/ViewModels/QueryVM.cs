using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using Srk.BetaseriesApiApp.CoreExtensions;

namespace Srk.BetaseriesApiApp.ViewModels {
    public class QueryVM : CommonViewModel {

        #region View properties

        public string Key {
            get { return _key; }
            set { SetValue(ref _key, value, "Key"); }
        }
        private string _key;

        public bool IsKeyEnabled {
            get { return _isKeyEnabled; }
            set { SetValue(ref _isKeyEnabled, value, "IsKeyEnabled"); }
        }
        private bool _isKeyEnabled = true;
        
        public string Token {
            get { return _token; }
            set { SetValue(ref _token, value, "Token"); }
        }
        private string _token;

        public bool IsTokenEnabled {
            get { return _isTokenEnabled; }
            set { SetValue(ref _isTokenEnabled, value, "IsTokenEnabled"); }
        }
        private bool _isTokenEnabled = true;

        public string Action {
            get { return _action; }
            set { SetValue(ref _action, value, "Action"); }
        }
        private string _action;

        public string QueryString {
            get { return _queryString; }
            set { SetValue(ref _queryString, value, "QueryString"); }
        }
        private string _queryString;
        
        public ObservableCollection<QueryArgument> Arguments { get { return _arguments; } }
        private readonly ObservableCollection<QueryArgument> _arguments = new ObservableCollection<QueryArgument>();

        public string Response {
            get { return _response; }
            set { SetValue(ref _response, value, "Response"); }
        }
        private string _response;
        
        #endregion

        public QueryVM(Main main) : base(main) {
            this.Key = this.client.Key;
            this.Token = this.client.SessionToken;

            this.Action = "status";
            this.Arguments.Add(new QueryArgument { IsEnabled = false, Key = "login", Value = "alba" });
            this.Arguments.Add(new QueryArgument { IsEnabled = false, Key = "title", Value = "starg" });
            this.Arguments.Add(new QueryArgument { IsEnabled = false, Key = "season", Value = "1" });
            this.Arguments.Add(new QueryArgument { IsEnabled = false, Key = "episode", Value = "1" });
            this.Arguments.Add(new QueryArgument { IsEnabled = false, Key = "number", Value = "100" });
        }

        #region LoadedCommand

        /// <summary>
        /// Loaded
        /// To be bound in the view.
        /// </summary>
        public ICommand LoadedCommand {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded)); }
        }
        private ICommand _loadedCommand;

        private bool initialized;

        private void OnLoaded() {
            if (this.initialized)
                return;

            this.initialized = true;

            base.main.Login.StatusChangedEvent += this.Login_StatusChangedEvent;
        }

        void Login_StatusChangedEvent(object sender, EventArgs e) {
            this.Token = this.client.SessionToken;
        }

        #endregion

        #region NewArgCommand

        /// <summary>
        /// NewArg
        /// To be bound in the view.
        /// </summary>
        public ICommand NewArgCommand {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _newArgCommand ?? (_newArgCommand = new RelayCommand(OnNewArg)); }
        }
        private ICommand _newArgCommand;

        private void OnNewArg() {
            this.Arguments.Add(new QueryArgument());
        }

        #endregion

        #region ExecuteCommand

        /// <summary>
        /// Execute.
        /// To be bound in the view.
        /// </summary>
        public ICommand ExecuteCommand {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _executeCommand ?? (_executeCommand = new RelayCommand(OnExecute, CanExecute)); }
        }
        private ICommand _executeCommand;

        private bool CanExecute() {
            return base.IsNotBusy;
        }

        private void OnExecute() {
            base.UpdateStatus("Querying...", true);

            var get = new Dictionary<string, string>();
            var post = new Dictionary<string, string>();

            if (this.IsKeyEnabled)
                get.Add("key", this.Key);
            if (this.IsTokenEnabled && !string.IsNullOrEmpty(this.Token))
                get.Add("token", this.Token);

            foreach (var arg in this.Arguments)
	        {
                if (arg.IsEnabled) {
                    if (arg.Mode == "GET")
                        get.Add(arg.Key, arg.Value);
                    if (arg.Mode == "POST")
                        post.Add(arg.Key, arg.Value);
                }
	        }

            var httpWrapper = new ExtendedHttpRequestWrapper("{0}{1}.xml?{2}", "http://api.betaseries.com/", AppVersion.ApplicationUserAgent);

            ThreadPool.QueueUserWorkItem(o => {
                string response, qs = null;
                try {
                    if (post.Count > 0)
                        response = httpWrapper.ExecuteQuery(this.Action, get, post);
                    else
                        response = httpWrapper.ExecuteQuery(this.Action, get);
                    qs = httpWrapper.QueryString;
                } catch (Exception ex) {
                    response = ex.ToString();
                }

                CurrentDispatcher.BeginInvoke(new Action(() => {
                    this.QueryString = qs;
                    this.Response = response;
                    UpdateStatus("Done", false);
                }));
            });
        }

        #endregion

        #region SaveCommand

        /// <summary>
        /// Save
        /// To be bound in the view.
        /// </summary>
        public ICommand SaveCommand {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(OnSave, CanSave)); }
        }
        private ICommand _saveCommand;

        private void OnSave() {
        }

        private bool CanSave() {
            return false;
        }

        #endregion
    }
}
