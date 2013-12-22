using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Simple struct to easy episode numbers manipulations
    /// </summary>
    public struct EpisodeNumber {

        /// <summary>
        /// Season number.
        /// </summary>
        public uint Season;

        /// <summary>
        /// Episode number.
        /// </summary>
        public uint Episode;

        /// <summary>
        /// Returns a full number like "S02E08".
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return EpisodeNumbers.GetNumberAsString(Season, Episode);
        }

    }
}
