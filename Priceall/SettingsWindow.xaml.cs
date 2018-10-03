using Priceall.Properties;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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

        static ModifierKeys _modifierKey;
        static Key _virtualKey;

        static bool _keyRecording = false;

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

        public bool IsUsingAutomaticRefresh
        {
            get
            {
                return Settings.Default.IsUsingAutomaticRefresh;
            }
            set
            {
                Settings.Default.IsUsingAutomaticRefresh = value;
                ((MainWindow)Owner).ToggleAutoRefresh();
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
        
        public string KeyCombo
        {
            get { return Settings.Default.KeyCombo; }
            set
            {
                Settings.Default.KeyCombo = value;
                OnPropertyChanged("KeyCombo");
            }
        }

        private void EditHotkey(object sender, KeyEventArgs e)
        {
            if (!_keyRecording)
            {
                KeyCombo = "";
                _modifierKey = 0;
                _virtualKey = 0;
                _keyRecording = true;
            }

            Debug.WriteLine($"Key state: {e.Key}");

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                _modifierKey = _modifierKey | ModifierKeys.Shift;
                KeyCombo += "Shift ";
            }

            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                _modifierKey = _modifierKey | ModifierKeys.Control;
                KeyCombo += "Ctrl ";
            }

            else if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            {
                _modifierKey = _modifierKey | ModifierKeys.Alt;
                KeyCombo += "Alt ";
            }

            else
            {
                // key recording complete
                _virtualKey = e.Key;
                KeyCombo += e.Key;
                _keyRecording = false;
                ((MainWindow)Owner).UpdateQueryHotkey(_modifierKey, _virtualKey);
            }

            Debug.WriteLine($"Modkeys is now {(uint)_modifierKey}; Virtkey is now {(uint)_virtualKey}");
        }

        private void BlockFilter(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
