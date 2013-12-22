using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace Srk.BetaseriesApiApp.ViewModels {
    public class QueryArgument : NotifyPropertyChanged {

        public string Key {
            get { return _key; }
            set { SetValue(ref _key, value, "Key"); }
        }
        private string _key;

        public string Value {
            get { return _value; }
            set { SetValue(ref _value, value, "Value"); }
        }
        private string _value;

        public bool IsEnabled {
            get { return _isEnabled; }
            set { SetValue(ref _isEnabled, value, "IsEnabled"); }
        }
        private bool _isEnabled;

        public string Mode {
            get { return _mode; }
            set { SetValue(ref _mode, value, "Mode"); }
        }
        private string _mode = "GET";
        
        public QueryArgument() {
        }

    }
}
