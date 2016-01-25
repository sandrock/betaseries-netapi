
namespace Srk.BetaseriesApi
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represent a betaseries member.
    /// </summary>
    public class Member
    {

        /// <summary>
        /// This is the unique ID.
        /// The username can be used for API methods.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// URL for a profile picture.
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// [Statistics] Number of shows added on the profile.
        /// </summary>
        public uint ShowCount { get; set; }

        /// <summary>
        /// [Statistics] Numbers of seasons from the shows added on the profile.
        /// </summary>
        public uint SeasonCount { get; set; }

        /// <summary>
        /// [Statistics] Numbers of episodes from the shows added on the profile.
        /// </summary>
        public uint EpisodeCount { get; set; }

        /// <summary>
        /// Percentage of watched episodes. It's a string like "99.23 %".
        /// </summary>
        public string Progress { get; set; }

        /// <summary>
        /// [Statistics] Numbers of episodes from the shows added on the profile not marked as seen.
        /// </summary>
        public uint EpisodesToWatchCount { get; set; }

        /// <summary>
        /// Time spent watching TV shows.
        /// </summary>
        public TimeSpan? TimeSpent { get; set; }

        /// <summary>
        /// Time remaining to watching TV shows for unseen episodes.
        /// </summary>
        public TimeSpan? TimeRemaining { get; set; }

        /// <summary>
        /// List of TV shows on the profile.
        /// </summary>
        public List<Show> Shows { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the member is a friend.
        /// </summary>
        public bool? IsFriend { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Member"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Username;
        }
    }
}
