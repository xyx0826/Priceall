using Priceall.Properties;
using System.ComponentModel;

namespace Priceall.Binding
{
    /// <summary>
    /// Handles window opacity and other style properties.
    /// </summary>
    class UiStyleBinding : INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Refresh()
        {
            OnPropertyChanged(null);
        }
        #endregion

        public UiStyleBinding()
        {
            OnPropertyChanged(null);
        }

        /// <summary>
        /// Sets the window opacity between 100% and 20%.
        /// </summary>
        public double WndOpacity
        {
            get { return Settings.Default.WindowOpacity; }
            set
            {
                // brilliant idea from karl-kaefer: 0.0 makes the window click-through
                if (value < 0.0) value = 0.0;
                else if (value > 1.0) value = 1.0;
                Settings.Default.WindowOpacity = value;
                OnPropertyChanged("WndOpacity");
            }
        }

        public int WndWidth
        {
            get { return Settings.Default.WindowWidth; }
            set
            {
                if (value < 60) value = 60;
                Settings.Default.WindowWidth = value;
                OnPropertyChanged("WndWidth");
            }
        }
        
        public int WndHeight
        {
            get { return Settings.Default.WindowHeight; }
            set
            {
                if (value < 20) value = 20;
                Settings.Default.WindowHeight = value;
                OnPropertyChanged("WndHeight");
            }
        }

        public double WndTopPos
        {
            get { return Settings.Default.WindowTopPos; }
            set
            {
                if (value < 0) value = 0;
                Settings.Default.WindowTopPos = value;
                OnPropertyChanged("WndTopPos");
            }
        }

        public double WndLeftPos
        {
            get { return Settings.Default.WindowLeftPos; }
            set
            {
                if (value < 0) value = 0;
                Settings.Default.WindowLeftPos = value;
                OnPropertyChanged("WndLeftPos");
            }
        }
    }
}
