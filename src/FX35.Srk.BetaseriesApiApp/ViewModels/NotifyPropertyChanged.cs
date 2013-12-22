

namespace Srk.BetaseriesApiApp.ViewModels {

    using System;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Base ViewModel implementing <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public bool IsInDesignMode {
            get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyPropertyChanged"/> class.
        /// </summary>
        protected NotifyPropertyChanged()
        {
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property changed.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Changes a property's value and notifies the view.
        /// </summary>
        /// <typeparam name="T">the property type</typeparam>
        /// <param name="property">a reference to a field</param>
        /// <param name="value">the new value</param>
        /// <param name="propertyName">the public property name for change notification</param>
        /// <returns>
        /// Returns <b>true</b> if the new value is different from the old one; otherwise <b>false</b>.
        /// </returns>
        protected bool SetValue<T>(ref T property, T value, string propertyName)
        {
            if (Object.Equals(property, value))
            {
                return false;
            }

            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
