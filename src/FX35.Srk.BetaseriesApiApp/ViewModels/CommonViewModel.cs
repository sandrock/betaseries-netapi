
namespace Srk.BetaseriesApiApp.ViewModels
{
    using System;
    using GalaSoft.MvvmLight;
    using Srk.BetaseriesApi;
    using Srk.BetaseriesApi.Clients;
    using System.Windows.Threading;
    using System.Configuration;

    public class CommonViewModel : NotifyPropertyChanged
    {
        protected readonly Main main;

        protected Dispatcher CurrentDispatcher = Dispatcher.CurrentDispatcher;

        public CommonViewModel(Main main)
        {
            this.main = main;
        }

        public CommonViewModel()
        {
        }

        protected IBetaseriesApi client
        {
            get
            {
                if (BetaseriesClientFactory.Default == null)
                    return null;
                if (_client == null)
                    _client = BetaseriesClientFactory.Default.CreateDefaultClient();
                return _client;
            }
        }
        private IBetaseriesApi _client;

        protected Srk.BetaseriesApi2.BetaseriesClient Client2
        {
            get
            {
                if (_client2 == null)
                {
                    var key = ConfigurationManager.AppSettings["ApiKey"];
                    _client2 = new Srk.BetaseriesApi2.BetaseriesClient(key);
                }

                return _client2;
            }
        }
        private Srk.BetaseriesApi2.BetaseriesClient _client2;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged("IsBusy");
                    RaisePropertyChanged("IsNotBusy");
                    OnBusyStateChanged();
                }
            }
        }
        public bool IsNotBusy
        {
            get { return !_isBusy; }
        }
        private bool _isBusy;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    RaisePropertyChanged("ErrorMessage");
                }
            }
        }
        private string _errorMessage;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    RaisePropertyChanged("StatusMessage");
                }
            }
        }
        private string _statusMessage = "Ready.";

        protected void UpdateStatus(string status, Exception error, bool isBusy)
        {
            if (error is BetaException)
            {
                var beta = (BetaException)error;
                if (beta.BetaError != null)
                    this.UpdateStatus(status, isBusy, beta.BetaError.DisplayMessage);
                else
                    this.UpdateStatus(status, isBusy, beta.Message);
            }
            else
            {
                this.UpdateStatus(status, isBusy, error.Message);
            }
        }

        protected void UpdateStatus(string status, bool isBusy)
        {
            this.UpdateStatus(status, isBusy, null);
        }

        protected void UpdateStatus(string status, bool isBusy, string error)
        {
            this.StatusMessage = status;
            this.ErrorMessage = error;
            this.IsBusy = isBusy;
        }

        protected virtual void OnBusyStateChanged()
        {

        }

        public virtual void Cleanup()
        {
            _client.Dispose();
            _client = null;
        }

    }
}
