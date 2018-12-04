using Priceall.Helpers;
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
                    OnPropertyChanged("RectOpacity");

                if (e.PropertyName == "UpdateAvailable")
                    OnPropertyChanged("RectBackgroundBrush");
            };
        }

        public double RectOpacity
        {
            get
            {
                if (SettingsService
                    .GetSetting<bool>("IsDragEnabled")) return 1.0;
                else return 0.4;
            }
        }

        public SolidColorBrush RectBackgroundBrush
        {
            get
            {
                if (SettingsService.GetSetting<bool>("UpdateAvailable") || Settings.Default.FLAG_AUTO_REFRESH_OFF)
                {
                    return ColorHelper.ConvertHexToColorBrush("FFA500");
                }
                else return null;
            }
        }
    }
}
