using System;
using System.Text.RegularExpressions;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Utility class for TV shows episode numbers.
    /// </summary>
    public static class EpisodeNumbers {

        /// <summary>
        /// Returns a standardized episode number like "S03E14".
        /// </summary>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        /// <returns></returns>
        public static string GetNumberAsString(uint season, uint episode) {
            return string.Format("S{0,2:00}E{1,2:00}", season, episode);
        }

        /// <summary>
        /// Returns season and episode number from an episode number like "S03E14".
        /// </summary>
        /// <param name="number"></param>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        /// <returns></returns>
        public static bool GetNumbers(string number, out int season, out int episode) {
            int s = 0, e = 0;
            bool ok = false;

            var match = EpisodeNumberRegex.Match(number);
            if (match.Success && match.Groups[1] != null && match.Groups[2] != null) {
#if PocketPC
                try {
                    s = int.Parse(match.Groups[1].Value);
                    e = int.Parse(match.Groups[2].Value);
                    ok = true;
                } catch { }
#else
                ok = int.TryParse(match.Groups[1].Value, out s) && int.TryParse(match.Groups[2].Value, out e);
#endif
            }

            season = s;
            episode = e;

            return ok;
        }

        /// <summary>
        /// Returns season and episode number from an episode number like "S03E14".
        /// </summary>
        /// <param name="number"></param>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        /// <returns></returns>
        public static bool GetNumbers(string number, out uint season, out uint episode) {
            uint s = 0, e = 0;
            bool ok = false;

            var match = EpisodeNumberRegex.Match(number);
            if (match.Success && match.Groups[1] != null && match.Groups[2] != null) {
#if PocketPC
                try {
                    s = uint.Parse(match.Groups[1].Value);
                    e = uint.Parse(match.Groups[2].Value);
                    ok = true;
                } catch { }
#else
                ok = uint.TryParse(match.Groups[1].Value, out s) && uint.TryParse(match.Groups[2].Value, out e);
#endif
            }

            season = s;
            episode = e;

            return ok;
        }

        /// <summary>
        /// Regex to match an episode number like "S03E14".
        /// </summary>
        public static readonly Regex EpisodeNumberRegex = new Regex("^S(\\d+)E(\\d+)$", RegexOptions.IgnoreCase);

    }
}
