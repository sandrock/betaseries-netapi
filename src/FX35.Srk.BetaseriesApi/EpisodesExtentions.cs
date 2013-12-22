using System;
using System.Collections.Generic;
using System.Linq;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Extension methods for lists of <see cref="Episode"/>s.
    /// </summary>
    public static class EpisodesExtentions {

        /// <summary>
        /// Returns distinct days from an list of episodes.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> SelectDays(IEnumerable<Episode> list) {
            return list
                .Where(e => e.Date.HasValue)
                .Select(e => e.Date.Value.Date)
                .Distinct();
        }
        
    }
}
