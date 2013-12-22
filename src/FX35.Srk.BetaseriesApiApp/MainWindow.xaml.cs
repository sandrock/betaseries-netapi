using System;
using System.Configuration;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Srk.BetaseriesApiApp.ViewModels;

namespace Srk.BetaseriesApiApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            DispatcherHelper.Initialize();

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var key = ConfigurationManager.AppSettings["ApiKey"];
                if (string.IsNullOrEmpty(key) || key == "API KEY")
                    MessageBox.Show("Please write your API KEY in the config file!");
            }));
        }

        public Main Model {
            get { return DataContext as Main; }
        }
    }
}
