
namespace Srk.BetaseriesApiApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GalaSoft.MvvmLight;

    public class QueryArgument : NotifyPropertyChanged
    {
        private string _key;
        private string _value;
        private bool _isEnabled;
        private string _mode = "GET";

        public QueryArgument()
        {
        }

        public string Key
        {
            get { return _key; }
            set { SetValue(ref _key, value, "Key"); }
        }

        public string Value
        {
            get { return _value; }
            set { SetValue(ref _value, value, "Value"); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetValue(ref _isEnabled, value, "IsEnabled"); }
        }

        public string Mode
        {
            get { return _mode; }
            set { SetValue(ref _mode, value, "Mode"); }
        }
    }
}
