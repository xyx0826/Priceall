using Priceall.Properties;
using System.ComponentModel;

namespace Priceall.Binding
{
    /// <summary>
    /// Handles window opacity.
    /// </summary>
    class UiOpacityBinding : INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public UiOpacityBinding()
        {
            WndOpacity = Settings.Default.WindowOpacity;
            OnPropertyChanged(null);
        }

        double _wndOpacity;

        /// <summary>
        /// Sets the window opacity between 100% and 20%.
        /// </summary>
        public double WndOpacity
        {
            get { return _wndOpacity; }
            set
            {
                _wndOpacity = value;
                if (_wndOpacity < 0.2) _wndOpacity = 0.2;
                else if (_wndOpacity > 1.0) _wndOpacity = 1.0;
                OnPropertyChanged("WndOpacity");
            }
        }
    }
}
