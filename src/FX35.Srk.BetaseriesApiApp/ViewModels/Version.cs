using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Srk.BetaseriesApi;
using Srk.BetaseriesApi.Clients;
using Srk.BetaseriesApiApp.Properties;

namespace Srk.BetaseriesApiApp.ViewModels
{
    public class Version : CommonViewModel
    {

        #region Properties

        public ObservableCollection<MethodVM> Methods
        {
            get { return _methods; }
        }
        private readonly ObservableCollection<MethodVM> _methods =
          new ObservableCollection<MethodVM>();

        public ICollectionView MethodsCV
        {
            get
            {
                if (_methodsCV == null)
                {
                    //_methodsCV = new CollectionView(Methods);
                    _methodsCV = CollectionViewSource.GetDefaultView(Methods);
                    _methodsCV.Filter = methodsCvFilter;
                    _methodsCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                }
                return _methodsCV;
            }
        }
        private ICollectionView _methodsCV;
        private bool methodsCvFilter(object o)
        {
            var vm = o as MethodVM;
            if (vm == null)
                return true;

            bool show = true;
            if (FilterImplemented.HasValue)
            {
                if (FilterImplemented.Value)
                {
                    show &= vm.IsImplemented;
                }
                else
                {
                    show &= !vm.IsImplemented;
                }
            }
            if (FilterUptodate.HasValue)
            {
                if (FilterUptodate.Value)
                {
                    show &= vm.IsImplemented && vm.IsUpToDate.Value;
                }
                else
                {
                    show &= vm.IsImplemented && !vm.IsUpToDate.Value;
                }
            }
            return show;
        }

        public bool? FilterImplemented
        {
            get { return _filterImplemented; }
            set
            {
                if (_filterImplemented != value)
                {
                    _filterImplemented = value;
                    RaisePropertyChanged("FilterImplemented");
                    MethodsCV.Refresh();
                }
            }
        }
        private bool? _filterImplemented;

        public bool? FilterUptodate
        {
            get { return _filterUptodate; }
            set
            {
                if (_filterUptodate != value)
                {
                    _filterUptodate = value;
                    RaisePropertyChanged("FilterUptodate");
                    MethodsCV.Refresh();
                }
            }
        }
        private bool? _filterUptodate;

        #endregion


        public Version(Main main)
            : base(main)
        {
        }

        #region Async responses

        void client_GetStatusEnded(object sender, AsyncResponseArgs<ApiStatus> e)
        {
            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Succeed)
                {
                    var status = new ApiVersionReport(client);
                    var report = status.GetReport(client as BetaseriesXmlClient);

                    if (e.Data != null && e.Data.Methods != null)
                    {
                        UpdateStatus("Fetched information. Processing... ", false);
                        var methods = new List<MethodVM>();

                        foreach (var method in e.Data.Methods)
                        {
                            var existing = methods.FirstOrDefault(m => m.Name == method.Name);
                            int? val = null;
                            if (report.ContainsKey(method.Name))
                                val = report[method.Name];

                            methods.Add(new MethodVM
                            {
                                Name = method.Name,
                                Current = val,
                                Last = method.DateUpdated,
                                IsImplemented = report.ContainsKey(method.Name)
                            });
                        }
                        foreach (var action in report)
                        {
                            if (!methods.Any(m => m.Name == action.Key))
                            {
                                methods.Add(new MethodVM
                                {
                                    Name = action.Key,
                                    Current = action.Value,
                                    Last = 0,
                                    IsImplemented = true
                                });
                            }
                        }

                        Methods.Clear();
                        /*using (MethodsCV.DeferRefresh())*/
                        {
                            foreach (var m in methods)
                            {
                                Methods.Add(m);
                            }
                        }

                        UpdateStatus("Done. ", false);
                    }
                    else
                    {
                        UpdateStatus("Failed: no data from API. ", false);
                    }
                }
                else
                {
                    UpdateStatus("Failed: " + e.Error.Message, e.Error, false);
                }
            }));
        }

        #endregion

        #region Commands

        #region Update

        /// <summary>
        /// Methods
        /// To be bound in the view.
        /// </summary>
        public ICommand MethodsCommand
        {
            get
            {
                if (_methodsCommand == null)
                {
                    _methodsCommand = new RelayCommand(
                        OnMethods, CanMethods);
                }
                return _methodsCommand;
            }
        }
        private ICommand _methodsCommand;

        private void OnMethods()
        {
            client.GetStatusAsync(this.client_GetStatusEnded);
            UpdateStatus("Getting API information... ", true);
        }

        private bool CanMethods()
        {
            return IsNotBusy;
        }

        #endregion

        #region Update

        /// <summary>
        /// CopyTrac
        /// To be bound in the view.
        /// </summary>
        public ICommand CopyTracCommand
        {
            get
            {
                if (_copyTracCommand == null)
                {
                    _copyTracCommand = new RelayCommand(
                        OnCopyTrac, CanCopyTrac);
                }
                return _copyTracCommand;
            }
        }
        private ICommand _copyTracCommand;

        private void OnCopyTrac()
        {
            // we are generating a document, so a StringBuilder is recommended
            StringBuilder sb = new StringBuilder();

            // formating strings
            string headerFormat = "={0} =||";
            string valueFormat = "{0} ||";

            // write the headers
            sb.Append("||");
            foreach (var name in new string[] { "Method", "Status", "Last", "Current"/*, "Comment"*/ })
            {
                sb.Append(string.Format(headerFormat, name));
            }
            sb.AppendLine();

            // enumerate methods and write 
            MethodVM vm = null;
            foreach (var item in MethodsCV)
            {
                vm = item as MethodVM;

                sb.Append("||");
                sb.Append(string.Format(valueFormat, vm.Name));
                if (vm.IsImplemented)
                    sb.Append(string.Format(valueFormat, vm.IsUpToDate.Value ? "OK" : "ok (outdated)"));
                else
                    sb.Append(string.Format(valueFormat, "Not implemented"));
                sb.Append(string.Format(valueFormat, vm.Last));
                sb.Append(string.Format(valueFormat, vm.Current.HasValue ? vm.Current.Value.ToString() : "0"));
                //sb.Append(string.Format(valueFormat, string.Empty));
                sb.AppendLine();
            }

            // get the final string from the stringbuilder
            string final = sb.ToString();

            // send document to the clipboard
            try
            {
                Clipboard.SetData(DataFormats.Text, final);
            }
            catch { }
        }

        private bool CanCopyTrac()
        {
            return CanMethods();
        }

        #endregion

        #region Save as CSV

        /// <summary>
        /// Copy
        /// To be bound in the view.
        /// </summary>
        public ICommand SaveCsvCommand
        {
            get
            {
                if (_copyCommand == null)
                {
                    _copyCommand = new RelayCommand(
                        OnCopy, CanCopy);
                }
                return _copyCommand;
            }
        }
        private ICommand _copyCommand;

        private void OnCopy()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var name in new string[] { "Method", "Status", "Last", "Current"/*, "Comment"*/ })
            {
                sb.Append(name);
                sb.Append(',');
            }
            sb.AppendLine();

            MethodVM vm = null;
            foreach (var item in MethodsCV)
            {
                vm = item as MethodVM;

                sb.Append(vm.Name);
                sb.Append(',');
                if (vm.IsImplemented)
                    sb.Append(vm.IsUpToDate.Value ? "OK" : "ok (outdated)");
                else
                    sb.Append("Not implemented");
                sb.Append(',');
                sb.Append(vm.Last);
                sb.Append(',');
                sb.Append(vm.Current.HasValue ? vm.Current.Value.ToString() : "0");
                //sb.Append(string.Format(valueFormat, string.Empty));
                sb.AppendLine();
            }

            string final = sb.ToString();
            sb = null;

            try
            {
                Clipboard.SetData(DataFormats.Text, final);
            }
            catch { }

            try
            {
                var diag = new SaveFileDialog();
                diag.InitialDirectory = Settings.Default.SavePath;

                var result = diag.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    var filepath = diag.FileName;
                    Settings.Default.SavePath = Path.GetDirectoryName(filepath);
                    Settings.Default.Save();
                    try
                    {
                        File.WriteAllText(filepath, final);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

            }
            catch { }
        }

        private bool CanCopy()
        {
            return CanMethods();
        }

        #endregion

        #endregion

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }

    [Serializable]
    public class MethodVM : GalaSoft.MvvmLight.ViewModelBase
    {

        public string Name { get; set; }
        public bool IsImplemented { get; set; }
        public int Last { get; set; }
        public int? Current { get; set; }

        public bool? IsUpToDate
        {
            get
            {
                bool? n = null;
                if (IsImplemented)
                    return Last == 0 ? Current > 0 : Current == Last;
                else
                    return n;
            }
        }
    }
}
