using Priceall.Helpers;
using Priceall.Properties;
using System.ComponentModel;
using System.Windows.Media;

namespace Priceall.Bindings
{
    /// <summary>
    /// Handles window opacity and other style properties.
    /// </summary>
    internal class UiStyleBinding : INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public UiStyleBinding()
        {
            Settings.Default.PropertyChanged += (sender, e) =>
            {
                OnPropertyChanged(e.PropertyName);
            };
        }

        /// <summary>
        /// Sets the window opacity between 100% and 0%.
        /// </summary>
        public double WindowOpacity
        {
            get => Settings.Default.WindowOpacity;
            set
            {
                // brilliant idea from karl-kaefer: 0.0 makes the window click-through
                if (value < 0.0) value = 0.0;
                else if (value > 1.0) value = 1.0;
                Settings.Default.WindowOpacity = value;
            }
        }

        public int WindowWidth
        {
            get => Settings.Default.WindowWidth;
            set
            {
                if (value < 60) value = 60;
                Settings.Default.WindowWidth = value;
                OnPropertyChanged("WndWidth");
            }
        }

        public int WindowHeight
        {
            get => Settings.Default.WindowHeight;
            set
            {
                if (value < 20) value = 20;
                Settings.Default.WindowHeight = value;
                OnPropertyChanged("WndHeight");
            }
        }

        public double WindowTopPos
        {
            get => Settings.Default.WindowTopPos;
            set
            {
                if (value < 0) value = 0;
                Settings.Default.WindowTopPos = value;
                OnPropertyChanged("WndTopPos");
            }
        }

        public double WindowLeftPos
        {
            get => Settings.Default.WindowLeftPos;
            set
            {
                if (value < 0) value = 0;
                Settings.Default.WindowLeftPos = value;
                OnPropertyChanged("WndLeftPos");
            }
        }

        public SolidColorBrush BackgroundBrush
            => ColorHelper.ConvertSettingToColorBrush("BackgroundColor");
    }
}
