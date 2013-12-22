using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Srk.BetaseriesApi {

    //addede by Crevette
    /// <summary>
    /// POCO representing a comment.
    /// </summary>
    public class Comment {

        /// <summary>
        /// Name of the TV show.
        /// Its value come from episodes/episode/show.
        /// Value is not always available.
        /// </summary>
        public string ShowUrl { get; set; }

        /// <summary>
        /// This is a string like "2". 
        /// Its value come from seasons/season/number.
        /// </summary>
        public uint Season { get; set; }

        /// <summary>
        /// Episode number like "2".
        /// </summary>
        public uint EpisodeNumber { get; set; }

        /// <summary>
        /// Username of the comment owner
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// When the comment was posted
        /// </summary>
        public DateTime Postdate { get; set; }

        /// <summary>
        /// Text of the comment
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Inner Id
        /// </summary>
        public int InnerId { get; set; }

        /// <summary>
        /// Id in reply to
        /// </summary>
        public int InreplyTo { get; set; }

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
        private string _status;

        /// <summary>
        /// 
        /// </summary>
        public Comment() {

        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// Because this object is not always filled.
        /// </summary>
        public bool IsLoaded
        {
            get { return Status != null; }
        }

        internal Comment(string username, DateTime postdate, string text, int innerid, int inreplyto)
        {
            Username = username;
            Postdate = postdate;
            Text = text;
            InnerId = innerid;
            InreplyTo = inreplyto;
        }

#pragma warning disable 1591
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
#pragma warning restore 1591

    }
}
