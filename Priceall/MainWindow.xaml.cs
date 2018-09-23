using Priceall.Binding;
using Priceall.Helper;
using Priceall.Hotkey;
using Priceall.Properties;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Priceall
{
    /// <summary>
    /// Priceall widget window.
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly AppraisalInfoBinding _infoBinding = new AppraisalInfoBinding();
        static readonly AppraisalControlsBinding _controlsBinding = new AppraisalControlsBinding();
        static readonly UiStyleBinding _styleBinding = new UiStyleBinding();
        static readonly AppraisalHelper _appraisal = new AppraisalHelper();
        static readonly ClipboardHelper _clipboard = new ClipboardHelper();
        static readonly HotkeyHelper _hotkey = new HotkeyHelper();

        static Window _settingsWindow = new SettingsWindow();
        
        DateTime _lastQueryTime;

        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        /// Binds data contexts ASAP to prevent flickering.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            UpdateSettings();   // migrate settings over from older Priceall version
            Task.Run(async () => { await FlagsHelper.CheckAllFlags(); });   // update flag values in settings

            DataContext = _styleBinding;
            AppraisalInfo.DataContext = _infoBinding;
            AppraisalControls.DataContext = _controlsBinding;
        }

        #region Window loading and terminating
        /// <summary>
        /// Initializes hotkeys, checks updates.
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _settingsWindow.Owner = this;
            _hotkey.RegisterHotkeyFromSettings("QueryKey", OnHotKeyHandler);

            InitializeClipboard();
            CheckForUpdates();
        }

        private void InitializeClipboard()
        {
            var helper = new WindowInteropHelper(this);
            _clipboard.InitializeListener(HwndSource.FromHwnd(helper.Handle), OnHotKeyHandler);
        }

        public void ToggleAutoRefresh()
        {
            if (Settings.Default.IsUsingAutomaticRefresh)
                _clipboard.StartListener();
            else _clipboard.StopListener();
        }

        /// <summary>
        /// Checks for older Priceall settings and migrate them over.
        /// </summary>
        private void UpdateSettings()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// Checks for update from AppVeyor in the background.
        /// If an update is found, the setting icon will become orange.
        /// </summary>
        private void CheckForUpdates()
        {
            var helper = new UpdateHelper();
            Task.Run(async () =>
            {
                IsUpdateAvailable = await helper.CheckForUpdates();
                _controlsBinding.IsUpdateAvail = IsUpdateAvailable;
            });
        }

        /// <summary>
        /// Checks for query cooldown and calls for appraisal query.
        /// </summary>
        private async void OnHotKeyHandler()
        {
            await QueryAppraisal();
        }

        /// <summary>
        /// Saves config and shuts down the app.
        /// </summary>
        private void AppShutdown(object sender, RoutedEventArgs e)
        {
            _hotkey.SaveHotkeys();
            _hotkey.UnregisterAllHotkeys();
            Settings.Default.Save();
            Application.Current.Shutdown();
        }
        #endregion

        #region Appraisal query
        /// <summary>
        /// Queries appraisal, triggered by hotkey.
        /// </summary>
        /// <returns></returns>
        private async Task QueryAppraisal()
        {
            var clipboardContent = _clipboard.ReadClipboardText();
            await Task.Run(() => { QueryAppraisal(clipboardContent); });
        }

        /// <summary>
        /// Queries appraisal, triggered by button.
        /// </summary>
        private async void QueryAppraisal(object sender, RoutedEventArgs e)
        {
            var clipboardContent = _clipboard.ReadClipboardText();
            await Task.Run(() => { QueryAppraisal(clipboardContent); });
        }

        private async void QueryAppraisal(string clipboardContent)
        {
            var queryElapsed = (DateTime.Now - _lastQueryTime).TotalMilliseconds;
            if (queryElapsed >= Settings.Default.QueryCooldown)
            {
                _lastQueryTime = DateTime.Now;
                _infoBinding.Price = "Hold on...";
                _infoBinding.SetTypeIcon("searchmarket");

                var appraisal = await _appraisal.QueryAppraisal(clipboardContent);
                var json = new Json(appraisal);

                if (!String.IsNullOrEmpty(json.ErrorMessage))
                {
                    _infoBinding.SetTypeIcon("heuristic");
                    _infoBinding.PriceLowerOrHigher = null;
                    RefreshPriceColor();
                    _infoBinding.Price = json.ErrorMessage;
                }
                else
                {
                    _infoBinding.SetTypeIcon(json.Kind);

                    var price = json.SellValue;
                    if (json.SellValue < Settings.Default.LowerPrice)
                    {
                        _infoBinding.PriceLowerOrHigher = true;
                    }
                    else if (json.SellValue > Settings.Default.UpperPrice)
                    {
                        _infoBinding.PriceLowerOrHigher = false;
                    }
                    else _infoBinding.PriceLowerOrHigher = null;

                    RefreshPriceColor();

                    if (Settings.Default.IsUsingPrettyPrint)
                        _infoBinding.Price = json.PrettyPrintValue();
                    else _infoBinding.Price = String.Format("{0:N}", json.SellValue);
                }
            }
        }

        /// <summary>
        /// Reads clipboard on main thread for COM compliance.
        /// </summary>
        /// <returns>Clipboard content, or empty string if clipboard does not have text.</returns>
        private string ReadClipboardOnMainThread()
        {
            return _clipboard.ReadClipboardText();
        }
        #endregion

        #region Control utilities
        /// <summary>
        /// Sets the window to be topmost (overlay).
        /// </summary>
        private void SetWindowOnTop(object sender, EventArgs e)
        {
            Topmost = true;
        }

        /// <summary>
        /// Changes window background transparency when scrolls on it.
        /// </summary>
        private void TweakWindowTransparency(object sender, MouseWheelEventArgs e)
        {
            bool isZooming = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (isZooming)
            {
                if (e.Delta > 0)
                {
                    _styleBinding.WndWidth += 12;
                    _styleBinding.WndHeight += 4;
                }
                else
                {
                    _styleBinding.WndWidth -= 12;
                    _styleBinding.WndHeight -= 4;
                }
            }
            else
            {
                if (e.Delta > 0) _styleBinding.WndOpacity += 0.05;
                else _styleBinding.WndOpacity -= 0.05;
            }
        }

        /// <summary>
        /// Starts window drag move when button is held.
        /// </summary>
        private void StartDragMoving(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.IsDragEnabled &&
                e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        /// <summary>
        /// Toggles response of drag drop button and save to settings.
        /// </summary>
        private void TogglePin(object sender, MouseButtonEventArgs e)
        {
            var currentState = Settings.Default.IsDragEnabled;
            Settings.Default.IsDragEnabled = !currentState;
            _controlsBinding.SetRectOpacityStyle(!currentState);
        }

        public void RefreshPriceColor()
        {
            _infoBinding.RefreshPriceColor();
        }

        public void ResetSettings()
        {
            Settings.Default.Reset();
            _infoBinding.Refresh();
            _controlsBinding.Refresh();
            _styleBinding.Refresh();
        }

        /// <summary>
        /// Shows the settings window.
        /// </summary>
        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            _settingsWindow.ShowDialog();
        }
        #endregion
    }
}
