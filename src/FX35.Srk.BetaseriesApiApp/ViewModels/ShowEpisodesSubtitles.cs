using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Srk.BetaseriesApi;

namespace Srk.BetaseriesApiApp.ViewModels
{
    public class ShowEpisodesSubtitles : CommonViewModel
    {

        public ShowEpisodesSubtitles(Main main)
            : base(main) 
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif
        }

        #region Property

        #region Show

        /// <summary>
        /// The <see cref="MyShow" /> property's name.
        /// </summary>
        public const string MyPropertyShow = "Show";

        private String _myPropertyShow = "";

        /// <summary>
        /// Gets the MyProperty property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public String MyShow
        {
            get
            {
                return _myPropertyShow;
            }

            set
            {
                if (_myPropertyShow == value)
                {
                    return;
                }

                var oldValue = _myPropertyShow;
                _myPropertyShow = value;

                // Update bindings, no broadcast
                RaisePropertyChanged("Show");
            }
        }

        #endregion

        #region Language

        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>
        public const string MyPropertyLanguage = "Language";

        private String _myPropertyLanguage = "";

        /// <summary>
        /// Gets the MyProperty property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public String MyLanguage
        {
            get
            {
                return _myPropertyLanguage;
            }

            set
            {
                if (_myPropertyLanguage == value)
                {
                    return;
                }

                var oldValue = _myPropertyLanguage;
                _myPropertyLanguage = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MyPropertyLanguage);
            }
        }

        #endregion

        #region Number

        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>
        public const string MyPropertyNumber = "Number";

        private string _myPropertyNumber = "";

        /// <summary>
        /// Gets the MyProperty property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string MyNumber
        {
            get
            {
                return _myPropertyNumber;
            }

            set
            {
                if (_myPropertyNumber == value)
                {
                    return;
                }

                var oldValue = _myPropertyNumber;
                _myPropertyNumber = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MyPropertyNumber);
            }
        }

        #endregion

        #region Subtitles

        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>
        public const string MyPropertySubtitles = "MySubtitles";

        private readonly ObservableCollection<Subtitle> _subtitles = new ObservableCollection<Subtitle>();

        /// <summary>
        /// Gets the MyProperty property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public ObservableCollection<Subtitle> MySubtitles
        {
            get
            {
                return _subtitles;
            }
        }       

        #endregion

        #endregion

        #region Commands

        #region Search Subtitles command

        /// <summary>
        /// Command description.
        /// To be bound in the view.
        /// </summary>
        public ICommand SearchCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _PrivNameCommand ?? (_PrivNameCommand = new RelayCommand(OnSearch)); }
        }
        private ICommand _PrivNameCommand;

        private void OnSearch()
        {
            if (String.IsNullOrEmpty(MyShow) && String.IsNullOrEmpty(MyLanguage) && String.IsNullOrEmpty(MyNumber))
            {
                client.GetLatestShowSubtitlesAsync(MyShow, MyLanguage, uint.Parse(MyNumber), this.OnGetLatestShowSubtitlesEnded);
            }
        }

        private void OnGetLatestShowSubtitlesEnded(object sender, AsyncResponseArgs<IList<Subtitle>> e)
        {

        }
        
        #endregion

        #endregion
    }
}
