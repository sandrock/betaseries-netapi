using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace Srk.BetaseriesApiApp.ViewModels {
    public class DebugVM : CommonViewModel {

        /// <summary>
        /// Break
        /// To be bound in the view.
        /// </summary>
        public ICommand BreakCommand {
            get {
                if (_BreakCommand == null) {
                    _BreakCommand = new RelayCommand(OnBreak);
                }
                return _BreakCommand;
            }
        }
        private ICommand _BreakCommand;

        private void OnBreak() {
            if (Debugger.IsAttached) {
                Debugger.Break();
            }
        }


    }
}
