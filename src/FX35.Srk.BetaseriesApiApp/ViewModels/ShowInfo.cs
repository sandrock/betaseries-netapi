using System;
using System.Collections.ObjectModel;
using System.Threading;
using GalaSoft.MvvmLight.Threading;
using Srk.BetaseriesApi;

namespace Srk.BetaseriesApiApp.ViewModels {
    public class ShowInfo : CommonViewModel {

        public Show Show {
            get { return _show; }
            set {
                if (_show != value) {
                    _show = value;
                    RaisePropertyChanged("Show");

                    if (!value.IsLoaded) {
                        LoadShowAsync(value);
                    }

                    if (value.Episodes == null) {
                        LoadEpisodesAsync(value);
                    }
                }
            }
        }
        private Show _show;

        public ObservableCollection<Episode> Episodes {
            get { return _episodes; }
        }
        private readonly ObservableCollection<Episode> _episodes =
          new ObservableCollection<Episode>();

        public ShowInfo(Main main) : base(main) {
            
        }

        private void LoadShowAsync(Show value) {
            UpdateStatus("Loading show information...", true);

            ThreadPool.QueueUserWorkItem(param => {
                var result = client.GetShow(value.Url);

                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
                    UpdateStatus("Done.", false);
                    value.Merge(result);
                }));
            });
        }

        private void LoadEpisodesAsync(Show value) {
            UpdateStatus("Loading episodes...", true);

            ThreadPool.QueueUserWorkItem(param => {
                var result = client.GetEpisodes(value.Url);

                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
                    UpdateStatus("Done.", false);
                    Show.Episodes = result;
                }));
            });
        }

    }
}
