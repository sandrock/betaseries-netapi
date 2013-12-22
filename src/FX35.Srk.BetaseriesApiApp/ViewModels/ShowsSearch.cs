using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using Srk.BetaseriesApi;

namespace Srk.BetaseriesApiApp.ViewModels {
    public class ShowsSearch : CommonViewModel {

        public string SearchString {
            get { return _searchString; }
            set {
                if (_searchString != value) {
                    _searchString = value;
                    RaisePropertyChanged("SearchString");
                }
            }
        }
        private string _searchString;

        public ObservableCollection<Show> Shows {
            get { return _shows; }
        }
        private readonly ObservableCollection<Show> _shows =
          new ObservableCollection<Show>();

        public Show SelectedShow {
            get { return _selectedShow; }
            set {
                if (_selectedShow != value) {
                    _selectedShow = value;
                    RaisePropertyChanged("SelectedShow");

                    if (!value.IsLoaded) {
                        LoadShowAsync(value);
                    }
                }
            }
        }
        private Show _selectedShow;

        public ShowsSearch(Main main) : base(main) {

        }

        #region Commands

        /// <summary>
        /// Search
        /// To be bound in the view.
        /// </summary>
        public ICommand SearchCommand {
            get {
                if (_searchCommand == null) {
                    _searchCommand = new RelayCommand(
                        OnSearch, CanSearch);
                }
                return _searchCommand;
            }
        }
        private ICommand _searchCommand;

        private void OnSearch() {
            DoSearchAsync(SearchString);
        }

        private bool CanSearch() {
            return !string.IsNullOrEmpty(SearchString) && IsNotBusy;
        }


        #endregion

        #region Search

        private void DoSearchAsync(string searchString) {
            UpdateStatus("Searching...", true);

            ThreadPool.QueueUserWorkItem(param => {
                IList<Show> result = null;
                
                try {
                    result = client.SearchShows(searchString);
                } catch (Exception ex) {
                    CurrentDispatcher.BeginInvoke(new Action(() => {
                        UpdateStatus(ex.Message, false);
                    }));
                    return;
                }

                CurrentDispatcher.BeginInvoke(new Action(() => {
                    UpdateStatus("Done.", false);
                    Shows.Clear();
                    if (result != null) {
                        foreach (var item in result) {
                            Shows.Add(item);
                        }
                    }
                }));
            });
        }

        #endregion

        private void LoadShowAsync(Show value) {
            ThreadPool.QueueUserWorkItem(param => {
                var result = client.GetShow(value.Url);

                CurrentDispatcher.BeginInvoke(new Action(() => {
                    value.Merge(result);
                }));
            });
        }

    }
}
