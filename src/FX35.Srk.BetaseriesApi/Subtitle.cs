
namespace Srk.BetaseriesApi
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;

    /// <summary>
    /// Represent a subtitle file.
    /// 
    /// </summary>
    public class Subtitle
    {
        private string _status;

        public Subtitle()
        {
        }

        public string Title { get; set; }

        public int Season { get; set; }

        public int Episode { get; set; }

        public string Language { get; set; }

        public string Url { get; set; }

        public int Quality { get; set; }

        public string FileName { get; set; }

        public string Source { get; set; }

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// Because this object is not always filled.
        /// </summary>
        public bool IsLoaded
        {
            get { return Status != null; }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
