using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Srk.BetaseriesApiApp.Views {
    /// <summary>
    /// Interaction logic for ShowSearch.xaml
    /// </summary>
    public partial class ShowSearch : UserControl {
        public ShowSearch() {
            InitializeComponent();
        }

        protected ViewModels.ShowInfo Model {
            get { return DataContext as ViewModels.ShowInfo; }
        }

        private void shows_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (lvSearchShows.SelectedItem != null) {
                //tcMain.SelectedItem = tabInfo;
                //Model.Show = Model.ShowsSearch.SelectedShow;
            }
        }

        private void showMenu_Episodes_Click(object sender, RoutedEventArgs e) {
            if (lvSearchShows.SelectedItem != null) {
                //tcMain.SelectedItem = tabInfo;
                //Model.Show = Model.ShowsSearch.SelectedShow;
            }
        }

        private void showMenu_Comments_Click(object sender, RoutedEventArgs e) {
            if (lvSearchShows.SelectedItem != null) {
                //tcMain.SelectedItem = tabComments;
                //EpisodesComments.MyShow = Model.ShowsSearch.SelectedShow.Url;
            }
        }
    }
}
