using System.Windows;
using System.Windows.Controls;

namespace Srk.BetaseriesApiApp.Views {
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl {
        public Login() {
            InitializeComponent();
        }

        public ViewModels.Login Model {
            get { return DataContext as ViewModels.Login; }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            Model.LoginCommand.Execute(pwLogin.Password);
        }

    }
}
