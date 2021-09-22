using System;
using Priceall.Appraisal;
using Priceall.Bindings;
using Priceall.Hotkey;
using Priceall.Hotkey.Controls;
using Priceall.Properties;
using Priceall.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Priceall
{
    /// <summary>
    /// Priceall settings window.
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Regex for validating a number.
        /// </summary>
        private static readonly Regex NumberRegex = new Regex("[^0-9]+");

        /// <summary>
        /// Regex for validating a hexadecimal value.
        /// </summary>
        private static readonly Regex HexRegex = new Regex("[^0-9A-Fa-f]+");

        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
        
        private static SettingsBinding _settings;

        public delegate void HotkeyCallback(KeyCombo keyCombo);
        static HotkeyCallback _hotkeyCallback;

        public SettingsWindow(HotkeyCallback hotkeyCallback)
        {
            InitializeComponent();
            _settings = new SettingsBinding();
            _hotkeyCallback = hotkeyCallback;
            QueryKeyEditor.SetHotkeyManagerSource(MainWindow.HotkeyManager);
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            _settings.MarketFlags = MainWindow.AppraisalService.GetAvailableMarkets();
            MainWindow.AppraisalService.SetCurrentMarket(_settings.SelectedMarket);
        }

        /// <summary>
        /// Helper method for creating a WPF binding from an <see cref="AppraisalSetting{T}"/>
        /// of any type.
        /// </summary>
        /// <typeparam name="T">Any setting type.</typeparam>
        /// <param name="setting">The setting to bind to.</param>
        /// <returns>The WPF binding.</returns>
        private Binding CreateSettingBinding<T>(AppraisalSetting<T> setting) where T : struct
        {
            return new Binding("Value") { Source = setting };
        }

        /// <summary>
        /// Creates controls for the data source currently in use
        /// and removes controls for old data sources.
        /// </summary>
        private void UpdateDataSourceSettings()
        {
            var svc = MainWindow.AppraisalService;

            // Register available markets and (try to) apply currently selected market
            _settings.MarketFlags = svc.GetAvailableMarkets();
            svc.SetCurrentMarket(_settings.SelectedMarket);

            // Clear old controls and make new ones
            AppraisalServiceSettingsPanel.Children.Clear();
            foreach (var setting in svc.GetCustomSettings())
            {
                var row = new DockPanel { LastChildFill = true };
                var name = new Label { Content = setting.Name };
                ContentControl control;
                switch (setting)
                {
                    case AppraisalSetting<bool> sb:
                        // Checkbox for boolean
                        control = new CheckBox();
                        control.SetBinding(CheckBox.IsCheckedProperty, CreateSettingBinding(sb));
                        break;
                    default:
                        // Unrecognized type
                        throw new Exception(
                            "Encountered a custom appraisal setting with an unsupported type.");
                }

                row.Children.Add(name);
                row.Children.Add(control);
                DockPanel.SetDock(name, Dock.Left);
                DockPanel.SetDock(control, Dock.Right);
                AppraisalServiceSettingsPanel.Children.Add(row);
            }

            // Hide the whole group box if there are no settings
            AppraisalServiceSettingsGroupBox.Visibility
                = AppraisalServiceSettingsPanel.Children.Count > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DataSource":
                    // Data source changed, update available settings
                    UpdateDataSourceSettings();
                    break;
                case "SelectedMarket":
                    MainWindow.AppraisalService.SetCurrentMarket(_settings.SelectedMarket);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Binds data contexts as soon as window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _settings;
            UpdateDataSourceSettings();
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

        /// <summary>
        /// Filters entered number digit.
        /// </summary>
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = NumberRegex.IsMatch(e.Text);
        }

        /// <summary>
        /// Filters entered color value.
        /// </summary>
        private void ColorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = HexRegex.IsMatch(e.Text);
        }

        /// <summary>
        /// Navigates to Google color picker.
        /// </summary>
        private void ColorPickerHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        ///// <summary>
        ///// Resets all settings.
        ///// </summary>
        //private void ResetSettingsButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // MainWindow has issue updating width and height together, 
        //    // so let's just refresh twice...
        //    SettingsService.Reset();
        //    SettingsService.Reset();
        //}
        
        public void HotkeyEditor_NewKeyCombo(object sender, RoutedEventArgs e)
        {
            var keyArgs = e as HotkeyEditor.NewKeyComboEventArgs;
            var newKeyCombo = keyArgs.KeyCombo;
            // Set appropriate name for this hotkey
            newKeyCombo.Name = (string)(sender as HotkeyEditor).Tag;
            _hotkeyCallback(newKeyCombo);
        }
    }
}
