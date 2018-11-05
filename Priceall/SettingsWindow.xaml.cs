using Priceall.Binding;
using System.Collections.Generic;
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

        static List<Key> _keys = new List<Key>();
        static int _keysHeld = 0;

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

        private void EditingKeyDown(object sender, KeyEventArgs e)
        {
            // clear key list and begin new recording
            if (_keysHeld == 0)
            {
                Debug.WriteLine("Recording a new keycombo.");
                _keys.Clear();
            }

            // prevent duplicates
            if (!e.IsRepeat && !_keys.Contains(e.Key))
            {
                // annoying windows alt codes
                var altDown = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

                Debug.WriteLine($"Adding {(altDown ? e.SystemKey : e.Key)}");
                _keysHeld++;
                _keys.Add(altDown ? e.SystemKey : e.Key);
            }
        }

        private void EditingKeyUp(object sender, KeyEventArgs e)
        {
            var altDown = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
            _keysHeld--;

            Debug.WriteLine($"Releasing {(altDown ? e.SystemKey : e.Key)}");

            // no more keys down, recording complete
            if (_keysHeld == 0)
            {
                Debug.WriteLine($"Recording complete. New keycombo with {_keys.Count} keys.");
                return;

                // key recording complete
                // create event args and fire update event
                var keyArgs = new QueryHotkeyUpdatedEventArgs
                {
                    KeyCombo = _keys.ToArray()
                };
                Instance.UpdateQueryHotkey(keyArgs);
            }
        }

        private void BlockFilter(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
