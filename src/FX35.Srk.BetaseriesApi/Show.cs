
namespace Srk.BetaseriesApi
{
    using System.Collections.Generic;
    using System.ComponentModel;


    /// <summary>
    /// Represent a TV show.
    /// This implement INotifyPropertyChanged for lazy loading.
    /// </summary>
    public class Show : INotifyPropertyChanged
    {
        private string description;
        private string status;
        private bool? archived;
        private string pictureUrl;
        private List<string> genres;
        private string tvDbId;
        private IList<Episode> episodes;
        private IList<SeasonCount> seasons;
        private bool? isInProfile;

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
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Expected values: Continuing, Ended, On Hiatus, Other
        /// </summary>
        //TODO: put this in an enum.
        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// If the member archived the show.
        /// </summary>
        public bool? Archived
        {
            get { return archived; }
            set
            {
                if (archived != value)
                {
                    archived = value;
                    RaisePropertyChanged("Archived");
                }
            }
        }

        /// <summary>
        /// Web URL of a picture representing the show.
        /// </summary>
        public string PictureUrl
        {
            get { return pictureUrl; }
            set
            {
                if (pictureUrl != value)
                {
                    pictureUrl = value;
                    RaisePropertyChanged("PictureUrl");
                }
            }
        }

        /// <summary>
        /// Genres.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<string> Genres
        {
            get { return genres; }
            set
            {
                if (genres != value)
                {
                    genres = value;
                    RaisePropertyChanged("Genres");
                }
            }
        }

        /// <summary>
        /// ID for another service.
        /// </summary>
        public string TVDBId
        {
            get { return tvDbId; }
            set
            {
                if (tvDbId != value)
                {
                    tvDbId = value;
                    RaisePropertyChanged("TvDbId");
                }
            }
        }

        /// <summary>
        /// Can contain an episode list.
        /// You have to populate it yourself [from the service].
        /// </summary>
        //TODO: lazy loading of this?
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<Episode> Episodes
        {
            get { return episodes; }
            set
            {
                if (episodes != value)
                {
                    episodes = value;
                    RaisePropertyChanged("Episodes");
                }
            }
        }

        /// <summary>
        /// Small list of seasons.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<SeasonCount> Seasons
        {
            get { return seasons; }
            set
            {
                if (seasons != value)
                {
                    seasons = value;
                    RaisePropertyChanged("Seasons");
                }
            }
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

        /// <summary>
        /// Stub field to indicate the show is in the current user profile.
        /// Manually populated!
        /// </summary>
        public bool? IsInProfile
        {
            get { return isInProfile; }
            set
            {
                if (isInProfile != value)
                {
                    isInProfile = value;
                    RaisePropertyChanged("IsInProfile");
                }
            }
        }

        /// <summary>
        /// When loading more data for a show, this methods merges data.
        /// </summary>
        /// <param name="show">to import data from (reference is not held)</param>
        public void Merge(Show show)
        {
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
        public override string ToString()
        {
            return Url ?? Title ?? GetType().FullName;
        }

        public class SeasonCount
        {
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
