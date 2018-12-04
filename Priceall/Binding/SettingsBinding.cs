using Priceall.Events;
using Priceall.Properties;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace Priceall.Binding
{
    class SettingsBinding : INotifyPropertyChanged
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

        public SettingsBinding()
        {
            Settings.Default.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                OnPropertyChanged(e.PropertyName);
            };
        }

        #region Appraisal settings
        public int MaxStringLength
        {
            get { return Settings.Default.MaxStringLength; }
            set
            {
                if (value != 0)
                {
                    Settings.Default.MaxStringLength = value;
                }
            }
        }

        public int QueryCooldown
        {
            get { return Settings.Default.QueryCooldown; }
            set
            {
                if (value > 0)
                {
                    Settings.Default.QueryCooldown = value;
                }
            }
        }

        public bool IsUsingAutomaticRefresh
        {
            get
            {
                return Settings.Default.IsUsingAutomaticRefresh;
            }
            set
            {
                Settings.Default.IsUsingAutomaticRefresh = value;
                UiEvents.Instance.ToggleAutoRefresh();
            }
        }

        public Visibility UpdateTagVisibility
        {
            get
            {
                return (Settings.Default.UpdateAvailable
                    ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public Visibility AutoRefreshTagVisibility
        {
            get
            {
                return (Settings.Default.FLAG_AUTO_REFRESH_OFF
                    ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public bool IsUsingPrettyPrint
        {
            get
            {
                return Settings.Default.IsUsingPrettyPrint;
            }
            set
            {
                Settings.Default.IsUsingPrettyPrint = value;
            }
        }

        public bool IsUsingConditionalColors
        {
            get
            {
                return Settings.Default.IsUsingConditionalColors;
            }
            set
            {
                Settings.Default.IsUsingConditionalColors = value;
                OnPropertyChanged("IsUsingConditionalColors");
            }
        }

        public string PriceColor
        {
            get { return Settings.Default.PriceColor; }
            set
            {
                Settings.Default.PriceColor = value;
                UiEvents.Instance.ChangePriceColor();
            }
        }

        public int LowerPrice
        {
            get { return Settings.Default.LowerPrice; }
            set
            {
                Settings.Default.LowerPrice = value;
            }
        }

        public int UpperPrice
        {
            get { return Settings.Default.UpperPrice; }
            set
            {
                Settings.Default.UpperPrice = value;
            }
        }

        public string LowerColor
        {
            get { return Settings.Default.LowerColor; }
            set
            {
                Settings.Default.LowerColor = value;
            }
        }

        public string UpperColor
        {
            get { return Settings.Default.UpperColor; }
            set
            {
                Settings.Default.UpperColor = value;
            }
        }

        public string AppVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();
        #endregion
    }
}
