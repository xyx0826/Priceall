using Priceall.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Priceall
{
    /// <summary>
    /// Priceall settings window.
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        static readonly Regex _numberRegex = new Regex("[^0-9]+");
        static readonly Regex _hexRegex = new Regex("[^0-9A-Fa-f]+");

        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Binds data contexts as soon as window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        /// <summary>
        /// Cancels window closing, reflect latest settings and hide.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            OnPropertyChanged(null);
            Hide();
        }

        #region Settings values
        public int MaxStringLength
        {
            get { return Settings.Default.MaxStringLength; }
            set
            {
                if (value != 0)
                {
                    Settings.Default.MaxStringLength = value;
                    Settings.Default.Save();
                }
            }
        }

        public int QueryCooldown
        {
            get { return Settings.Default.QueryCooldown; }
            set
            {
                if (value != 0)
                {
                    Settings.Default.QueryCooldown = value;
                    Settings.Default.Save();
                }
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
                ((MainWindow)Owner).RefreshPriceColor();
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

        public Visibility UpdateTagVisibility
        {
            get
            {
                return ((Owner as MainWindow).IsUpdateAvailable ?
                    Visibility.Visible : Visibility.Collapsed);
            }
        }

        public string AppVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();
        #endregion

        private void NumberFilter(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberRegex.IsMatch(e.Text);
        }

        private void ColorFilter(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _hexRegex.IsMatch(e.Text);
        }

        private void OpenGithubPage(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void ResetSettings(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Owner).ResetSettings();
            OnPropertyChanged(null);
        }
    }
}
