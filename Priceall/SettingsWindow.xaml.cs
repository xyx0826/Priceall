using Priceall.Binding;
using System;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Priceall.Services;
using System.Windows.Controls;
using Priceall.Hotkey;

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

        static bool _editingHotkey = false;
        static Button _editingHotkeyButton;
        static Key[] _oldKeyCombo;

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

        private void OpenLink(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void ResetSettings(object sender, RoutedEventArgs e)
        {
            // MainWindow has issue updating width and height together, 
            // so let's just refresh twice...
            SettingsService.ResetSettings();
            SettingsService.ResetSettings();
        }

        private void RecordHotkey(object sender, RoutedEventArgs e)
        {
            if (!_editingHotkey)
            {
                // No already editing hotkey
                _editingHotkey = true;
                _editingHotkeyButton = sender as Button;
                Debug.WriteLine((sender as Button).Name + " - new hotkey change.");

                _oldKeyCombo = HotkeyHelper.GetHotkeyByName(_editingHotkeyButton.Name);
                HotkeyHelper.StartRecording(ReceiveHotkeyFinished, UpdateKeyComboString);
            }
            else
            {
                HotkeyHelper.StopRecording();
                UpdateKeyComboString(_oldKeyCombo);
                Debug.WriteLine((sender as Button).Name + " - conflicting hotkey change.");
            }
        }

        public void UpdateKeyComboString(Key[] keys)
        {
            _editingHotkeyButton.Content = String.Join(" + ", keys);
        }

        public void ReceiveHotkeyFinished(Key[] keys)
        {
            _editingHotkey = false;
            HotkeyHelper.UpdateHotkey(_editingHotkeyButton.Name, keys);
            UpdateKeyComboString(keys);
            Debug.WriteLine(_editingHotkeyButton.Name + " - hotkey changed.");
        }
    }
}
