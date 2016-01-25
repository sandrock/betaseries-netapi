

namespace Srk.BetaseriesApi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    /// <summary>
    /// POCO representing an episode from a TV Show.
    /// </summary>
    public class Episode : IComparable
    {
        private List<Comment> _comments;

        /// <summary>
        /// Default .ctor.
        /// </summary>
        public Episode()
        {
        }

        /// <summary>
        /// .ctor used for episode list parsing
        /// </summary>
        internal Episode(
            string season, string title, string description, string screen, string number, string episode,
            DateTime? date, int ratings, float? rating, int? userRating, bool? downloaded, bool? seen)
        {
            Season = season;
            Title = title;
            Description = description;
            PictureUrl = screen;
            Number = number;
            EpisodeNumber = episode;
            Date = date;
            Ratings = ratings;
            Rating = rating;
            UserRating = userRating;
            IsDownloaded = downloaded;
            IsSeen = seen;

            FillFields();
        }

        /// <summary>
        /// .ctor used for episode list parsing
        /// </summary>
        internal Episode(string show, string showUrl, string title, string number, uint globalNumber, DateTime? date, bool? downloaded, bool seen)
        {
            ShowName = show;
            ShowUrl = showUrl;
            Title = title;
            Number = number;
            Date = date;
            IsDownloaded = downloaded;
            IsSeen = seen;

            FillFields();
        }

        /// <summary>
        /// .ctor used for timeline parsing
        /// </summary>
        internal Episode(string number, string showUrl, string show, string title)
        {
            Number = number;
            ShowName = show;
            ShowUrl = showUrl;
            Title = title;

            FillFields();
        }

        /// <summary>
        /// .ctor used for planning parsing
        /// </summary>
        internal Episode(string showUrl, string showTitle, string title, string number, uint episodeOrder, uint seasonOrder, DateTime? date)
        {
            ShowUrl = showUrl;
            ShowName = showTitle;
            Title = title;

            Number = number;
            Order = episodeOrder;
            SeasonOrder = seasonOrder;
            EpisodeNumber = number.ToString();
            Season = SeasonOrder.ToString();

            Date = date;

            FillFields();
        }

        /// <summary>
        /// This is a string like "2". 
        /// Its value come from seasons/season/number.
        /// </summary>
        public string Season { get; set; }

        /// <summary>
        /// Integer to sort seasons.
        /// This is the int value of <see cref="Season"/>.
        /// </summary>
        public uint SeasonOrder { get; set; }

        /// <summary>
        /// Episode number like "2".
        /// </summary>
        public string EpisodeNumber { get; set; }

        /// <summary>
        /// Integer to sort episodes.
        /// Its value come from seasons/season/episodes/episode/episode.
        /// </summary>
        public uint Order { get; set; }

        /// <summary>
        /// This is a string like "S01E01".
        /// Its value come from seasons/season/episodes/episode/number.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Episode's title.
        /// Its value come from seasons/season/episodes/episode/title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Global episode number (getting rid of seasons).
        /// </summary>
        public uint GlobalNumber { get; set; }

        /// <summary>
        /// Description.
        /// Its value come from seasons/season/episodes/episode/description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Picture for this episode.
        /// Its value come from seasons/season/episodes/episode/screen.
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// Release date.
        /// Its value come from episodes/episode/date or seasons/season/episodes/episode/date.
        /// Value is not always available.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Inforn if the user watched this episode.
        /// Value is not always available.
        /// </summary>
        public bool? IsSeen { get; set; }

        /// <summary>
        /// Inforn if the user downloaded this episode.
        /// Value is not always available.
        /// </summary>
        public bool? IsDownloaded { get; set; }

        /// <summary>
        /// Name of the TV show.
        /// Its value come from episodes/episode/show.
        /// Value is not always available.
        /// </summary>
        public string ShowName { get; set; }

        /// <summary>
        /// URL of the TV show.
        /// Its value come from episodes/episode/url.
        /// Value is not always available.
        /// </summary>
        public string ShowUrl { get; set; }

        /// <summary>
        /// Number of ratings.
        /// </summary>
        public int Ratings { get; set; }

        /// <summary>
        /// Average rating. 1.0-5.0.
        /// </summary>
        public float? Rating { get; set; }

        /// <summary>
        /// Rating of the current user.
        /// </summary>
        public int? UserRating { get; set; }

        public int InternalGlobalNumber
        {
            get
            {
                return (int)this.SeasonOrder * 1000 + (int)this.Order;
            }
        }

        /// <summary>
        /// Comments of this episode
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<Comment> Comments
        {
            get { return _comments; }
            set
            {
                if (_comments != value)
                {
                    _comments = value;
                    RaisePropertyChanged("Comments");
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void FillFields()
        {
            uint s = 0, e = 0;
            if (Number != null)
            {
                EpisodeNumbers.GetNumbers(Number, out s, out e);
                if (Order == 0)
                    Order = e;
                if (SeasonOrder == 0)
                    SeasonOrder = s;
                if (EpisodeNumber == null)
                    EpisodeNumber = e.ToString();
                if (Season == null)
                    Season = s.ToString();
            }
            else
            {
                if (EpisodeNumber != null && Season != null)
                {
                    if (uint.TryParse(Season, out s) && uint.TryParse(EpisodeNumber, out e))
                    {
                        Number = EpisodeNumbers.GetNumberAsString(s, e);
                        if (Order == 0)
                            Order = e;
                        if (SeasonOrder == 0)
                            SeasonOrder = s;
                    }
                }


            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", ShowUrl ?? ShowName, Number);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Equals(object obj)
        {
            var other = obj as Episode;
            if (other == null || other.Number == null)
                return false;

            if (Episode.ReferenceEquals(this, other))
                return true;

            if (ShowUrl != null && other.ShowUrl != null)
            {
                return this.ShowUrl == other.ShowUrl && this.Number == other.Number;
            }

            if (ShowName != null && other.ShowName != null)
            {
                return this.ShowName == other.ShowName && this.Number == other.Number;
            }

            return false;
        }

        public bool Equals(Episode other)
        {
            if (Episode.ReferenceEquals(this, other))
                return true;

            if (ShowUrl != null && other.ShowUrl != null)
            {
                return this.ShowUrl == other.ShowUrl && this.Number == other.Number;
            }

            if (ShowName != null && other.ShowName != null)
            {
                return this.ShowName == other.ShowName && this.Number == other.Number;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var show = ShowUrl ?? ShowName;
            return (show != null ? show.GetHashCode() : -1) ^ (Number != null ? Number.GetHashCode() : -1);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="obj"/>.
        /// Zero
        /// This instance is equal to <paramref name="obj"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="obj"/> is not the same type as this instance.
        ///   </exception>
        public int CompareTo(object obj)
        {
            var comp = (Episode)obj;

            int me = (int)this.SeasonOrder * 1000 + (int)this.Order;
            int ot = (int)comp.SeasonOrder * 1000 + (int)comp.Order;

            return ot - me;
        }
    }
}
