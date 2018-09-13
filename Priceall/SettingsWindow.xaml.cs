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
    /// SettingsWindow.xaml 的交互逻辑
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

        public string PriceColor
        {
            get { return Settings.Default.PriceColor; }
            set
            {
                Settings.Default.PriceColor = value;
                ((MainWindow)Owner).RefreshPriceColor();
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
