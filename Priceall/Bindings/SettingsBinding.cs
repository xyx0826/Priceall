using Priceall.Appraisal;
using Priceall.Properties;
using Priceall.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Priceall.Bindings
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

        /// <summary>
        /// Available appraisal service names and their implementations.
        /// </summary>
        static readonly Dictionary<string, Type> _appraisalServices
            = new Dictionary<string, Type>()
            {
                { "Evepraisal", typeof(EvepraisalAppraisalService) },
                { "Janice", typeof(JaniceAppraisalService) },
                // { "ceve-market", typeof(CeveMarketAppraisalService) }
            };

        #region Appraisal settings
        public Dictionary<string, Type> DataSources => _appraisalServices;

        public Type SelectedDataSource
        {
            get
            {
                foreach (var service in _appraisalServices.Values)
                {
                    if (service.Name == Settings.Default.DataSource)
                    {
                        return service;
                    }
                }
                return _appraisalServices.Values.FirstOrDefault();
            }
            set
            {
                if (value != null)
                {
                    Settings.Default.DataSource = value.Name;
                    // Force update selected market
                    OnPropertyChanged(nameof(SelectedMarket));
                }
            }
        }

        private AppraisalMarket _marketFlags;

        public AppraisalMarket MarketFlags
        {
            set
            {
                _marketFlags = value;
                OnPropertyChanged(nameof(Markets));
            }
        }

        public List<AppraisalMarket> Markets
        {
            get
            {
                var list = new List<AppraisalMarket>();
                foreach (var m in Enum.GetValues(typeof(AppraisalMarket)))
                {
                    if (_marketFlags.HasFlag((AppraisalMarket)m))
                    {
                        list.Add((AppraisalMarket)m);
                    }
                }
                return list;
            }
        }

        public AppraisalMarket SelectedMarket
        {
            get
            {
                var mkt = (AppraisalMarket)SettingsService.Get<int>("SelectedMarket");
                if (_marketFlags != 0 && !_marketFlags.HasFlag(mkt))
                {
                    // Currently selected market is illegal, reset to Jita
                    mkt = AppraisalMarket.Jita;
                    SettingsService.Set("SelectedMarket", (int)mkt);
                }

                return mkt;
            }
            set
            {
                SettingsService.Set("SelectedMarket", (int)value);
                OnPropertyChanged(nameof(SelectedMarket));
            }
        }

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
