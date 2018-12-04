using Priceall.Properties;
using Priceall.Services;
using System.ComponentModel;
using System.Windows.Media;

namespace Priceall.Binding
{
    class AppraisalControlsBinding : INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public AppraisalControlsBinding()
        {
            Settings.Default.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                OnPropertyChanged(e.PropertyName);
                if (e.PropertyName == "IsDragEnabled")
                    SetRectOpacityStyle(SettingsService
                        .GetSetting<bool>("IsDragEnabled"));
            };

            SetRectOpacityStyle(Settings.Default.IsDragEnabled);
            IsUpdateAvail = false;
        }

        public double RectOpacity { get; set; }

        /// <summary>
        /// Sets drag drop button opacity based on its state.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void SetRectOpacityStyle(bool enabled)
        {
            if (enabled) RectOpacity = 1.0;
            else RectOpacity = 0.4;
            OnPropertyChanged("RectOpacity");
        }

        private bool _isUpdateAvail;

        public bool IsUpdateAvail
        {
            get { return _isUpdateAvail; }
            set
            {
                _isUpdateAvail = value;
                OnPropertyChanged("RectBackgroundBrush");
            }
        }

        public SolidColorBrush RectBackgroundBrush
        {
            get
            {
                if (IsUpdateAvail || Settings.Default.FLAG_AUTO_REFRESH_OFF)
                {
                    return (SolidColorBrush)
                        (new BrushConverter()
                        .ConvertFrom("#FFA500"));
                }
                else return null;
            }
        }
    }
}
