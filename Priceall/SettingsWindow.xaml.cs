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
using Priceall.Properties;
using System.Windows.Controls;
using Priceall.Appraisal;
using System.Windows.Data;
using Priceall.Bindings;

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
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            _settings.MarketFlags = MainWindow.AppraisalService.GetAvailableMarkets();
            MainWindow.AppraisalService.SetCurrentMarket(_settings.SelectedMarket);
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataSource")
            {
                var serv = MainWindow.AppraisalService;
                _settings.MarketFlags = serv.GetAvailableMarkets();
                serv.SetCurrentMarket(_settings.SelectedMarket);
                var ui = AppraisalServiceSettingsPanel.Children;
                ui.Clear();

                foreach (var settings in serv.GetCustomSettings())
                {
                    var settingsPanel = new DockPanel();
                    settingsPanel.LastChildFill = true;

                    var nameLabel = new Label();
                    nameLabel.Content = settings.Name;
                    Binding binding;

                    ContentControl input;
                    if (settings is AppraisalSettings<bool> sb)
                    {
                        input = new CheckBox();
                        binding = new Binding("Value");
                        binding.Source = sb;
                        input.SetBinding(CheckBox.IsCheckedProperty, binding);
                    }
                    else
                    {
                        // TODO: support for other kinds of settings
                        break;
                    }

                    settingsPanel.Children.Add(nameLabel);
                    settingsPanel.Children.Add(input);
                    DockPanel.SetDock(nameLabel, Dock.Left);
                    DockPanel.SetDock(input, Dock.Right);
                    ui.Add(settingsPanel);
                }

                AppraisalServiceSettingsGroupBox.Visibility = ui.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (e.PropertyName == "SelectedMarket")
            {
                MainWindow.AppraisalService.SetCurrentMarket(_settings.SelectedMarket);
            }
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

        /// <summary>
        /// Filters entered number digit.
        /// </summary>
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberRegex.IsMatch(e.Text);
        }

        /// <summary>
        /// Filters entered color value.
        /// </summary>
        private void ColorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _hexRegex.IsMatch(e.Text);
        }

        /// <summary>
        /// Navigates to Google color picker.
        /// </summary>
        private void ColorPickerHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        /// <summary>
        /// Resets all settings.
        /// </summary>
        private void ResetSettingsButton_Click(object sender, RoutedEventArgs e)
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
