
namespace Srk.BetaseriesApiApp
{
    using System;
    using System.Configuration;
    using System.Windows;
    using GalaSoft.MvvmLight.Threading;
    using Srk.BetaseriesApiApp.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            DispatcherHelper.Initialize();

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var key = ConfigurationManager.AppSettings["ApiKey"];
                if (string.IsNullOrEmpty(key) || key == "API KEY")
                    MessageBox.Show("Please write your API KEY in the config file!");
            }));
        }

        public Main Model
        {
            get { return DataContext as Main; }
        }
    }
}
