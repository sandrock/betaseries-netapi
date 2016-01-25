
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
        private IBetaseriesApi _client;
        private Srk.BetaseriesApi2.BetaseriesClient _client2;
        private bool isBusy;
        private string errorMessage;
        private string statusMessage = "Ready.";

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

        public bool IsBusy
        {
            get { return this.isBusy; }
            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    RaisePropertyChanged("IsBusy");
                    RaisePropertyChanged("IsNotBusy");
                    OnBusyStateChanged();
                }
            }
        }
        public bool IsNotBusy
        {
            get { return !this.isBusy; }
        }

        public string ErrorMessage
        {
            get { return this.errorMessage; }
            set
            {
                if (this.errorMessage != value)
                {
                    this.errorMessage = value;
                    RaisePropertyChanged("ErrorMessage");
                }
            }
        }

        public string StatusMessage
        {
            get { return this.statusMessage; }
            set
            {
                if (this.statusMessage != value)
                {
                    this.statusMessage = value;
                    RaisePropertyChanged("StatusMessage");
                }
            }
        }

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
