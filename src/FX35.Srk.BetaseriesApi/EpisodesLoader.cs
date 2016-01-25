
namespace Srk.BetaseriesApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Helper to browse episodes within a tv show.
    /// This object operates asynchronously.
    /// __Supposed__ to be thread-safe.
    /// </summary>
    /// <remarks>
    /// Original ticket: http://projects.sandrock.fr/trac/betaseries/ticket/64
    /// Will not dispose the API client.
    /// </remarks>
    public class EpisodesLoader : IDisposable
    {
        /// <summary>
        /// Betaseries API client.
        /// Will not be disposed avec a call to <see cref="Dispose"/>.
        /// </summary>
        private readonly IBetaseriesApi client;

        /// <summary>
        /// Show URL.
        /// </summary>
        private readonly string showUrl;

        /// <summary>
        /// Show being navigated.
        /// </summary>
        private readonly Show show;

        /// <summary>
        /// Lock object for internal collections.
        /// </summary>
        private object instanceLock = new object();

        /// <summary>
        /// Lock object for <see cref="queue" />.
        /// </summary>
        private object queueLock = new object();

        /// <summary>
        /// Lock object for <see cref="queue" />.
        /// </summary>
        private object queueTimerLock = new object();

        /// <summary>
        /// List of operations to execute.
        /// </summary>
        private readonly Queue<KeyValuePair<string, string>> queue = new Queue<KeyValuePair<string, string>>();

        /// <summary>
        /// Contains a list of fully loaded seasons
        /// </summary>
        private readonly List<uint> seasons = new List<uint>();

        private const uint loopLimit = 3;
        private volatile uint loopCount;

        private Timer queueTimer;
        private volatile bool queuedTimerDisposition;

        /// <summary>
        /// Indicates an internal process is fetching information from the service.
        /// </summary>
        public bool IsProcessing
        {
            get { return _isProcessing; }
            set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    RaiseProcessingEvent();
                }
            }
        }
        private volatile bool _isProcessing;

        /// <summary>
        /// Raised when value of property IsProcessing changes.
        /// </summary>
        public event EventHandler IsProcessingChangedEvent;

        /// <summary>
        /// This is the to-be-loaded episode accessor.
        /// </summary>
        public Episode Episode
        {
            get { lock (instanceLock) { return _episode; } }
            protected set
            {
                bool changed = false;
                //lock (instanceLock) {
                if (_episode != value)
                {
                    _episode = value;
                    changed = true;
                }
                //}
                if (changed)
                    RaiseEpisodeChangedEvent();
            }
        }
        private Episode _episode;

        /// <summary>
        /// Raised when the required episode is available.
        /// </summary>
        public event EventHandler EpisodeChangedEvent;

        /// <summary>
        /// Raised when the an error occurs.
        /// </summary>
        public event ErrorEventHandler ErrorOccuredEvent;

        protected IEnumerable<Episode> Episodes
        {
            get { return _episodes; }
        }
        private readonly List<Episode> _episodes = new List<Episode>();
        protected IEnumerable<Episode> EpisodesSorted
        {
            get
            {
                return _episodes
                    .OrderBy(e => e.Order)
                    .OrderBy(e => e.SeasonOrder);
            }
        }

        private Episode episodeBeingRefreshed;

        private bool hasNextEpisode;
        private Episode nextEpisode;

        private bool? episodeLoaded;

        /// <summary>
        /// Default class .ctor.
        /// </summary>
        /// <param name="betaseriesClient"></param>
        /// <param name="show"></param>
        public EpisodesLoader(IBetaseriesApi betaseriesClient, Show show)
        {
            this.client = betaseriesClient;
            this.show = show;
            this.showUrl = show.Url;
        }

        /// <summary>
        /// Get the first episode.
        /// </summary>
        public void GetFirst()
        {
            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("first", null));
            }
            Run();
        }

        /// <summary>
        /// Get the previous episode.
        /// </summary>
        public void GetPrevious()
        {
            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("prev", null));
            }
            Run();
        }

        /// <summary>
        /// Get the next episode.
        /// </summary>
        public void GetNext()
        {
            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("next", null));
            }
            Run();
        }

        /// <summary>
        /// Get the last episode.
        /// </summary>
        public void GetLast()
        {
            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("last", null));
            }
            Run();
        }

        /// <summary>
        /// Get the last episode seen.
        /// </summary>
        public void GetLastSeen()
        {
            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("lastseen", null));
            }
            Run();
        }

        /// <summary>
        /// Get the next episode to see.
        /// </summary>
        public void GetNextToSee()
        {
            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("nexttosee", null));
            }
            Run();
        }

        /// <summary>
        /// Get an episode by number.
        /// </summary>
        /// <param name="number"></param>
        public void Get(string number)
        {
            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("number", number));
            }
            Run();
        }

        /// <summary>
        /// Get an episode by number.
        /// </summary>
        /// <param name="season"></param>
        /// <param name="episode"></param>
        public void Get(uint season, uint episode)
        {
            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("number", EpisodeNumbers.GetNumberAsString(season, episode)));
            }
            Run();
        }

        /// <summary>
        /// Refresh current episode.
        /// </summary>
        public void RefreshCurrent()
        {
            if (_episode == null)
                return;

            lock (queueLock)
            {
                queue.Enqueue(new KeyValuePair<string, string>("refresh", _episode.Number));
            }
            Run();
        }

        public void SetLastSeen(uint season, uint episode)
        {
            foreach (var ep in EpisodesSorted)
            {
                if (ep.SeasonOrder < season)
                {
                    ep.IsSeen = true;
                }
                else if (ep.SeasonOrder == season)
                {
                    if (ep.Order <= episode)
                    {
                        ep.IsSeen = true;
                    }
                    else
                    {
                        ep.IsSeen = false;
                    }
                }
                else
                {
                    ep.IsSeen = false;
                }
            }
        }

        private void StartQueueTimer()
        {
            lock (queueTimerLock)
            {
                if (queueTimer == null)
                    queueTimer = new Timer(queueTimer_Tick, this, 500, 1000);
            }
        }

        private void StopQueueTimer()
        {
            lock (queueTimerLock)
            {
                if (queueTimer != null)
                {
                    queueTimer.Dispose();
                    queueTimer = null;
                }
            }
        }

        private void queueTimer_Tick(object state)
        {
            if (queuedTimerDisposition)
            {
                lock (queueTimerLock)
                {
                    if (queueTimer != null)
                    {
                        queueTimer.Dispose();
                        queueTimer = null;
                    }
                }
            }
            Run();
        }

        private void Run()
        {
            if (_isProcessing)
                return;
            if (queueLock == null || instanceLock == null)
                throw new ObjectDisposedException("EpisodesLoader");

            // infinite loop detection
            if (loopCount >= loopLimit)
            {
                KeyValuePair<string, string>? value = null;
                lock (queueLock)
                {
                    if (queue.Count > 0)
                    {
                        value = queue.Dequeue();
                    }
                    else
                    {
                        value = new KeyValuePair<string, string>("-", "-");
                    }
                }
                string msg = "The EpisodeLoader went wrong (loop/" + value + "). The user request was aborted. A developer will be whipped. ";
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
#endif
                loopCount = 0;
                try
                {
                    throw new InvalidOperationException(msg);
                }
                catch (InvalidOperationException ex)
                {
                    ReportError(ex);
                }
            }

            // unqueue next element
            KeyValuePair<string, string>? unqueued = null;
            lock (queueLock)
            {
                if (queue.Count > 0)
                    unqueued = queue.Peek();
            }

            if (!unqueued.HasValue)
            {
                loopCount = 0;
                return;
            }
            loopCount++;

            // setting this flag to false means
            // another thread will call Run(); when
            // it has processed the task
            // setting to true will remove the task
            // at the end of this method
            bool accepted = true;

            Episode ep = null;
            bool found = false;

            lock (instanceLock)
            {
                uint currentSeasonNbr = Episode != null ? Episode.SeasonOrder : uint.Parse(show.Seasons.FirstOrDefault().Season);
                var nextSeason = show.Seasons
                    .SkipWhile(s => uint.Parse(s.Season) != currentSeasonNbr)
                    .Skip(1)
                    .FirstOrDefault();
                var prevSeason = show.Seasons
                    .TakeWhile(s => uint.Parse(s.Season) != currentSeasonNbr)
                    .LastOrDefault();
                bool isLastSeason = nextSeason == null;
                bool isFirstSeason = prevSeason == null;
                uint? nextSeasonNbr = nextSeason != null ? uint.Parse(nextSeason.Season) : default(uint?);
                uint? prevSeasonNbr = prevSeason != null ? uint.Parse(prevSeason.Season) : default(uint?);

                string command = unqueued.Value.Key;

            bigswitch:
                #region Stupid switch
                switch (command)
                {
                    case "number":
                        ep = _episodes.SingleOrDefault(e => e.Number == unqueued.Value.Value);
                        if (episodeLoaded.HasValue)
                        {
                            episodeLoaded = null;
                            accepted = true;
                            if (ep != null)
                            {
                                Episode = ep;
                            }
                        }
                        else
                        {
                            accepted = false;
                            uint season = 0, number = 0;
                            EpisodeNumbers.GetNumbers(unqueued.Value.Value, out season, out number);
                            FetchEpisode(season, number);
                        }
                        break;
                    case "refresh":
                        if (episodeBeingRefreshed == null)
                        {
                            episodeBeingRefreshed = Episode;
                            accepted = false;
                            uint season = 0, number = 0;
                            EpisodeNumbers.GetNumbers(unqueued.Value.Value, out season, out number);
                            FetchEpisode(season, number);
                        }
                        else
                        {
                            accepted = true;
                            episodeBeingRefreshed = null;
                            ep = _episodes.SingleOrDefault(e => e.Number == unqueued.Value.Value);
                            if (ep != null)
                            {
                                Episode = ep;
                            }
                            else
                            {
                                //TODO: unfinished code?
                            }
                        }
                        break;
                    case "first":
                        // get the first season number
                        var firstSeason = uint.Parse(show.Seasons.First().Season);

                        // check if first season is loaded
                        if (seasons.Contains(firstSeason))
                        {
                            accepted = true;

                            // then return first element
                            Episode = EpisodesSorted.First();
                        }
                        else
                        {
                            // season not loaded, load it
                            accepted = false;
                            FetchSeason(firstSeason);
                        }
                        break;

                    case "last":
                        // get the last season number
                        var lastSeason = uint.Parse(show.Seasons.Last().Season);

                        // check if last season is loaded
                        if (seasons.Contains(lastSeason))
                        {
                            accepted = true;

                            // then return last element
                            Episode = EpisodesSorted.Last();
                        }
                        else
                        {
                            // season not loaded, load it
                            accepted = false;
                            FetchSeason(lastSeason);
                        }
                        break;

                    case "next":
                        // check current season is loaded
                        if (!seasons.Contains(currentSeasonNbr))
                        {
                            accepted = false;
                            FetchSeason(currentSeasonNbr);
                            break;
                        }

                        // check next season is loaded
                        //TODO: check that the next episode is not in the current season
                        if (!isLastSeason && !seasons.Contains(nextSeasonNbr.Value))
                        {
                            accepted = false;
                            FetchSeason(nextSeasonNbr.Value);
                            break;
                        }

                        // current and next seasons are available, continue
                        accepted = true;
                        foreach (var item in EpisodesSorted)
                        {
                            if (found)
                            {
                                ep = item;
                                break;
                            }
                            if (item.Number == Episode.Number)
                            {
                                found = true;
                            }
                        }

                        if (ep != null)
                            Episode = ep;
                        break;

                    case "prev":
                        // check current season is loaded
                        if (!seasons.Contains(currentSeasonNbr))
                        {
                            accepted = false;
                            FetchSeason(currentSeasonNbr);
                            break;
                        }

                        // check previous season is loaded
                        //TODO: check that the previous episode is not in the current season
                        if (!isFirstSeason && !seasons.Contains(prevSeasonNbr.Value))
                        {
                            accepted = false;
                            FetchSeason(prevSeasonNbr.Value);
                            break;
                        }

                        // current and previous seasons are available, continue
                        accepted = true;
                        foreach (var item in EpisodesSorted)
                        {
                            if (item.Number == Episode.Number)
                            {
                                found = true;
                                break;
                            }
                            else
                            {
                                ep = item;
                            }
                        }

                        if (ep != null)
                            Episode = ep;
                        break;

                    case "lastseen":
                    case "nexttosee":
                        // fetch the next episode
                        if (!hasNextEpisode)
                        {
                            accepted = false;
                            FetchNextEpisode();
                        }
                        // when fetched, show it
                        else
                        {
                            accepted = true;

                            // if all episodes are seen, go to last
                            if (nextEpisode == null)
                            {
                                command = "last";
                                goto bigswitch;
                            }
                            // if there is a next episode
                            else
                            {
                                if (command == "lastseen")
                                {
                                    command = "prev";
                                    goto bigswitch;
                                }
                                else
                                {
                                    Episode = nextEpisode;
                                }
                            }

                            nextEpisode = null;
                            hasNextEpisode = false;
                        }
                        break;

                    default:
                        throw new NotSupportedException("Command " + unqueued.Value.Key + " is unknown");
                }
                #endregion

            }

            bool @continue = false;
            if (accepted)
            {
                lock (queueLock)
                {
                    if (queue.Count > 0)
                        queue.Dequeue();
                    @continue = queue.Count > 0;
                }
                loopCount = 0;
                if (@continue)
                {
                    // deadlock :(
                    //Run();
                    StartQueueTimer();
                }
                else
                {
                    StopQueueTimer();
                }
            }

        }

        private void FetchSeason(uint season)
        {
            IsProcessing = true;
            try
            {
                client.GetEpisodesAsync(show.Url, season, this.client_GetEpisodesEnded);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void FetchEpisode(uint season, uint number)
        {
            IsProcessing = true;
            episodeLoaded = false;
            try
            {
                client.GetEpisodeAsync(show.Url, season, number, this.client_GetEpisodeEnded);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void FetchNextEpisode()
        {
            IsProcessing = true;
            try
            {
                client.GetMembersNextShowEpisodeAsync(true, show.Url, this.client_GetMembersNextShowEpisodeEnded);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        void client_GetEpisodesEnded(object sender, AsyncResponseArgs<IList<Episode>> e)
        {
            if (e.Succeed)
            {
                if (e.Data.Count > 0)
                {
                    var season = e.Data.FirstOrDefault().SeasonOrder;

                    lock (instanceLock)
                    {
                        // remove volatile episodes from this season
                        var toRemove = _episodes.Where(ep => ep.SeasonOrder == season).ToArray();
                        foreach (var ex in toRemove)
                        {
                            _episodes.Remove(ex);
                        }

                        // add all episodes
                        _episodes.AddRange(e.Data);

                        // register season as loaded
                        if (!seasons.Contains(season))
                            seasons.Add(season);
                    }
                }
                else
                {
                    ReportError(new Exception("No episode and no error were returned from service"));
                }
            }
            else
            {
                ReportError(e.Error);
            }
            IsProcessing = false;
            Run();
        }

        void client_GetEpisodeEnded(object sender, AsyncResponseArgs<Episode> e)
        {
            episodeLoaded = true;
            if (e.Succeed)
            {
                if (e.Data != null)
                {
                    lock (instanceLock)
                    {
                        // remove existing episode
                        var existing = _episodes.SingleOrDefault(ep => ep.Number == e.Data.Number);
                        if (existing != null)
                            _episodes.Remove(existing);

                        // add episode
                        _episodes.Add(e.Data);
                    }
                }
                else
                {
                    ReportError(new Exception("No episode and no error were returned from service"));
                }
            }
            else
            {
                ReportError(e.Error);
            }
            IsProcessing = false;
            Run();
        }

        void client_GetMembersNextShowEpisodeEnded(object sender, AsyncResponseArgs<Episode> e)
        {
            lock (instanceLock)
            {
                hasNextEpisode = true;
                if (e.Succeed)
                {
                    nextEpisode = e.Data;
                }
                else
                {
                    lock (queueLock)
                    {
                        queue.Dequeue();
                    }
                    ReportError(e.Error);
                }
            }
            IsProcessing = false;
            Run();
        }

        private void ReportError(Exception ex)
        {
            ErrorEventHandler handler = ErrorOccuredEvent;
            if (handler != null)
            {
                handler.Invoke(this, new AsyncResponseArgs(ex));
            }
        }

        private void RaiseProcessingEvent()
        {
            if (IsProcessingChangedEvent != null)
                IsProcessingChangedEvent(this, EventArgs.Empty);
        }

        private void RaiseEpisodeChangedEvent()
        {
            if (EpisodeChangedEvent != null)
                EpisodeChangedEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Clear event handlers.
        /// This will not dispose the client.
        /// </summary>
        public void Dispose()
        {
            lock (instanceLock)
            {
                IsProcessingChangedEvent = null;
                EpisodeChangedEvent = null;
            }

            _episodes.Clear();
            _episode = null;

            if (queueTimer != null)
            {
                queueTimer.Dispose();
                queueTimer = null;
            }

            instanceLock = null;
            queueLock = null;
            queueTimerLock = null;
        }
    }

    public delegate void ErrorEventHandler(object sender, AsyncResponseArgs args);
}
