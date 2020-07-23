using Priceall.Binding;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Priceall.Services;
using Priceall.Hotkey.Controls;
using Priceall.Hotkey;
using static Priceall.Hotkey.Controls.HotkeyEditor;

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

        static SettingsBinding _settings;

        public delegate void HotkeyCallback(KeyCombo keyCombo);
        static HotkeyCallback _hotkeyCallback;

        public SettingsWindow(HotkeyCallback hotkeyCallback)
        {
            InitializeComponent();
            _settings = new SettingsBinding();
            _hotkeyCallback = hotkeyCallback;
            QueryKeyEditor.SetHotkeyManagerSource(MainWindow.HotkeyManager);
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

        private void OpenLink(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void ResetSettings(object sender, RoutedEventArgs e)
        {
            // MainWindow has issue updating width and height together, 
            // so let's just refresh twice...
            SettingsService.Reset();
            SettingsService.Reset();
        }
        
        public void HotkeyEditor_NewKeyCombo(object sender, RoutedEventArgs e)
        {
            var keyArgs = e as NewKeyComboEventArgs;
            var newKeyCombo = keyArgs.KeyCombo;
            // Set appropriate name for this hotkey
            newKeyCombo.Name = (string)(sender as HotkeyEditor).Tag;
            _hotkeyCallback(newKeyCombo);
        }
    }
}
