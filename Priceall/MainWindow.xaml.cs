using Priceall.Binding;
using Priceall.Services;
using Priceall.Hotkey;
using Priceall.Properties;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static Priceall.Events.UiEvents;

namespace Priceall
{
    /// <summary>
    /// Priceall widget window.
    /// </summary>
    public partial class MainWindow : Window
    {
        // initialize binding sources
        static readonly AppraisalInfoBinding _infoBinding = new AppraisalInfoBinding();
        static readonly AppraisalControlsBinding _controlsBinding = new AppraisalControlsBinding();
        static readonly UiStyleBinding _styleBinding = new UiStyleBinding();

        // initialize helpers
        static readonly ClipboardService _clipboard = new ClipboardService();

        static Window _settingsWindow = new SettingsWindow();
        
        DateTime _lastQueryTime;

        /// <summary>
        /// Binds data contexts ASAP to prevent flickering.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SettingsService.UpdateSettings();   // migrate settings over from older Priceall version
            AppraisalService.Initialize();
            HotkeyHelper.Initialize();
            Task.Run(async () => { await FlagService.CheckAllFlags(); });   // update flag values in settings

            DataContext = _styleBinding;
            AppraisalInfo.DataContext = _infoBinding;
            AppraisalControls.DataContext = _controlsBinding;

            // subscribe settings events
            Instance.AutoRefreshToggled += ToggleAutoRefresh;
            Instance.PriceColorChanged += RefreshPriceColor;
        }

        #region Window loading and terminating
        /// <summary>
        /// Initializes hotkeys, checks updates.
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _settingsWindow.Owner = this;

            if (!HotkeyHelper.ActivateHotkeyFromSettings("QueryKey", OnHotKeyHandler))
            {
                HotkeyHelper.CreateNewHotkey("QueryKey", 
                    new Key[] { Key.LeftCtrl, Key.LeftShift, Key.C }, 
                    OnHotKeyHandler);
                _infoBinding.Price = "Query hotkey: LCtrl + LShft + C.";
            }

            SetWindowOnTopDelegate();
            InitializeClipboard();
            ToggleAutoRefresh();
            CheckForUpdates();
        }

        /// <summary>
        /// Passes MainWindow handle to clipboard helper for setting up later.
        /// </summary>
        private void InitializeClipboard()
        {
            var helper = new WindowInteropHelper(this);
            _clipboard.InitializeListener(HwndSource.FromHwnd(helper.Handle));
            _clipboard.ClipboardChanged += QueryAppraisalAsync;
        }

        /// <summary>
        /// Toggles auto refresh based on setting value.
        /// </summary>
        public void ToggleAutoRefresh(object sender = null, EventArgs e = null)
        {
            if (Settings.Default.IsUsingAutomaticRefresh)
                _clipboard.StartListener();
            else _clipboard.StopListener();
        }

        /// <summary>
        /// Overwrites the current query hotkey.
        /// </summary>
        /// <param name="modKeys">New modifier key combo.</param>
        /// <param name="virtKey">New virtual key.</param>
        public void UpdateQueryHotkey(object sender, QueryHotkeyUpdatedEventArgs e)
        {
            HotkeyHelper.CreateNewHotkey("QueryKey", e.KeyCombo, OnHotKeyHandler);
        }

        /// <summary>
        /// Checks for update from AppVeyor in the background.
        /// If an update is found, the setting icon will become orange.
        /// </summary>
        private void CheckForUpdates()
        {
            Task.Run(async () =>
            {
                Settings.Default.UpdateAvailable = await UpdateService.CheckForUpdates();
                _controlsBinding.IsUpdateAvail = Settings.Default.UpdateAvailable;
            });
        }

        /// <summary>
        /// Checks for query cooldown and calls for appraisal query.
        /// </summary>
        private async void OnHotKeyHandler()
        {
            await QueryAppraisalAsync();
        }

        /// <summary>
        /// Saves config and shuts down the app.
        /// </summary>
        private void AppShutdown(object sender, RoutedEventArgs e)
        {
            HotkeyHelper.Uninitialize();
            Settings.Default.Save();
            Application.Current.Shutdown();
        }
        #endregion

        #region Appraisal query
        /// <summary>
        /// Queries appraisal, triggered by hotkey.
        /// </summary>
        /// <returns></returns>
        private async Task QueryAppraisalAsync()
        {
            var clipboardContent = _clipboard.ReadClipboardText();
            await Task.Run(() => { QueryAppraisalAsync(clipboardContent); });
        }

        /// <summary>
        /// Queries appraisal, triggered by clipboard event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void QueryAppraisalAsync(object sender, EventArgs e)
        {
            var clipboardContent = _clipboard.ReadClipboardText();
            await Task.Run(() => { QueryAppraisalAsync(clipboardContent); });
        }

        /// <summary>
        /// Universal entry for triggering a price check.
        /// </summary>
        /// <param name="clipboardContent">Content in clipboard to be price checked.</param>
        private async void QueryAppraisalAsync(string clipboardContent)
        {
            SetWindowOnTopDelegate();

            var queryElapsed = (DateTime.Now - _lastQueryTime).TotalMilliseconds;
            if (queryElapsed >= Settings.Default.QueryCooldown)
            {
                _lastQueryTime = DateTime.Now;
                _infoBinding.Price = "Hold on...";
                _infoBinding.SetTypeIcon("searchmarket");

                var appraisal = await AppraisalService.QueryAppraisal(clipboardContent);
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
                    if (json.SellValue <= Settings.Default.LowerPrice)
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
        private void SetWindowOnTop(object sender = null, EventArgs e = null)
        {
            Topmost = true;
        }

        /// <summary>
        /// Sets the window to be topmost (overlay).
        /// Executed on MainWindow's delegate (UI thread).
        /// </summary>
        private void SetWindowOnTopDelegate()
        {
            Dispatcher.BeginInvoke(
                new Action(() =>  
                {
                    SetWindowOnTop();
                }));
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
                    _styleBinding.WindowWidth += 12;
                    _styleBinding.WindowHeight += 4;
                }
                else
                {
                    _styleBinding.WindowWidth -= 12;
                    _styleBinding.WindowHeight -= 4;
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

        /// <summary>
        /// Triggers a price tag color refresh based on setting value.
        /// </summary>
        public void RefreshPriceColor(object sender = null, EventArgs e = null)
        {
            _infoBinding.RefreshPriceColor();
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
