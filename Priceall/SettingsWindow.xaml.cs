using Priceall.Binding;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using static Priceall.Events.UiEvents;

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

        static SettingsBinding _settings;

        public SettingsWindow()
        {
            InitializeComponent();

            _settings = new SettingsBinding();
        }

        /// <summary>
        /// Binds data contexts as soon as window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _settings;
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
            Instance.ResetSettings();
            OnPropertyChanged(null);
        }

        private void EditHotkey(object sender, KeyEventArgs e)
        {
            if (!_keyRecording)
            {
                _settings.KeyCombo = "";
                _modifierKey = 0;
                _virtualKey = 0;
                _keyRecording = true;
            }

            Debug.WriteLine($"Key state: {e.Key}");

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                _modifierKey = _modifierKey | ModifierKeys.Shift;
                _settings.KeyCombo += "Shift ";
            }

            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                _modifierKey = _modifierKey | ModifierKeys.Control;
                _settings.KeyCombo += "Ctrl ";
            }

            else if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            {
                _modifierKey = _modifierKey | ModifierKeys.Alt;
                _settings.KeyCombo += "Alt ";
            }

            else
            {
                // key recording complete
                _virtualKey = e.Key;
                _settings.KeyCombo += e.Key;
                _keyRecording = false;
                // create event args and fire update event
                var keyArgs = new QueryHotkeyUpdatedEventArgs
                {
                    ModKeys = _modifierKey,
                    VirtKey = _virtualKey
                };
                Instance.UpdateQueryHotkey(keyArgs);
            }

            Debug.WriteLine($"Modkeys is now {(uint)_modifierKey}; Virtkey is now {(uint)_virtualKey}");
        }

        private void BlockFilter(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
