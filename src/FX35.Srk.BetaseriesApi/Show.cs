using System.Collections.Generic;
using System.ComponentModel;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Represent a TV show.
    /// This implement INotifyPropertyChanged for lazy loading.
    /// </summary>
    public class Show : INotifyPropertyChanged {

        /// <summary>
        /// Identifier for service calls.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description {
            get { return _description; }
            set {
                if (_description != value) {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }
        private string _description;

        /// <summary>
        /// Expected values: Continuing, Ended, On Hiatus, Other
        /// </summary>
        //TODO: put this in an enum.
        public string Status {
            get { return _status; }
            set {
                if (_status != value) {
                    _status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }
        private string _status;

        /// <summary>
        /// If the member archived the show.
        /// </summary>
        public bool? Archived {
            get { return _archived; }
            set {
                if (_archived != value) {
                    _archived = value;
                    RaisePropertyChanged("Archived");
                }
            }
        }
        private bool? _archived;

        /// <summary>
        /// Web URL of a picture representing the show.
        /// </summary>
        public string PictureUrl {
            get { return _pictureUrl; }
            set {
                if (_pictureUrl != value) {
                    _pictureUrl = value;
                    RaisePropertyChanged("PictureUrl");
                }
            }
        }
        private string _pictureUrl;
        
        /// <summary>
        /// Genres.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<string> Genres {
            get { return _genres; }
            set {
                if (_genres != value) {
                    _genres = value;
                    RaisePropertyChanged("Genres");
                }
            }
        }
        private List<string> _genres;

        /// <summary>
        /// ID for another service.
        /// </summary>
        public string TVDBId {
            get { return _tvDbId; }
            set {
                if (_tvDbId != value) {
                    _tvDbId = value;
                    RaisePropertyChanged("TvDbId");
                }
            }
        }
        private string _tvDbId;

        /// <summary>
        /// Can contain an episode list.
        /// You have to populate it yourself [from the service].
        /// </summary>
        //TODO: lazy loading of this?
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<Episode> Episodes {
            get { return _episodes; }
            set {
                if (_episodes != value) {
                    _episodes = value;
                    RaisePropertyChanged("Episodes");
                }
            }
        }
        private IList<Episode> _episodes;

        /// <summary>
        /// Small list of seasons.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<SeasonCount> Seasons {
            get { return _seasons; }
            set {
                if (_seasons != value) {
                    _seasons = value;
                    RaisePropertyChanged("Seasons");
                }
            }
        }
        private IList<SeasonCount> _seasons;
        
        private void RaisePropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Because this object is not always filled.
        /// </summary>
        public bool IsLoaded {
            get { return Status != null; }
        }

        /// <summary>
        /// Stub field to indicate the show is in the current user profile.
        /// Manually populated!
        /// </summary>
        public bool? IsInProfile {
            get { return _isInProfile; }
            set {
                if (_isInProfile != value) {
                    _isInProfile = value;
                    RaisePropertyChanged("IsInProfile");
                }
            }
        }
        private bool? _isInProfile;
        
        /// <summary>
        /// When loading more data for a show, this methods merges data.
        /// </summary>
        /// <param name="show">to import data from (reference is not held)</param>
        public void Merge(Show show) {
            Description = show.Description;
            Status = show.Status;
            Genres = show.Genres;
            TVDBId = show.TVDBId;
            PictureUrl = show.PictureUrl;
            Seasons = show.Seasons;
            RaisePropertyChanged("IsLoaded");
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Show"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Url ?? Title ?? GetType().FullName;
        }

        public class SeasonCount {
            public string Season { get; set; }
            public string Episodes { get; set; }
        }

#pragma warning disable 1591
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
#pragma warning restore 1591

    }
}
