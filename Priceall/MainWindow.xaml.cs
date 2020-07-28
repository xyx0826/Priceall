using Priceall.Appraisal;
using Priceall.Bindings;
using Priceall.Hotkey;
using Priceall.Hotkey.Hook;
using Priceall.Hotkey.NonHook;
using Priceall.Properties;
using Priceall.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Priceall
{
    /// <summary>
    /// Priceall widget window.
    /// </summary>
    partial class MainWindow : Window
    {
        // constants
        static readonly IAppraisalService[] _appraisalServices = new IAppraisalService[]
        {
            new EvepraisalAppraisalService(),
            new JaniceAppraisalService(),
            new CeveMarketAppraisalService()
        };

        // initialize binding sources
        static readonly AppraisalInfoBinding _infoBinding = new AppraisalInfoBinding();
        static readonly AppraisalControlsBinding _controlsBinding = new AppraisalControlsBinding();
        static readonly UiStyleBinding _styleBinding = new UiStyleBinding();

        // initialize helpers
        static readonly ClipboardService _clipboard = new ClipboardService();

        internal static IAppraisalService AppraisalService;

        internal static IHotkeyManager HotkeyManager;

        private static Window _settingsWindow;
        
        private DateTime _lastQueryTime;

        /// <summary>
        /// Binds data contexts ASAP to prevent flickering.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            SettingsService.Upgrade();

            // Network tasks
            Task.Run(async () => { await FlagService.CheckAllFlags(); });
            Task.Run(async () => { await UpdateService.CheckForUpdates(); });

            // Bind data contexts
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

            UpdateAppraisalService(SettingsService.Get<string>("DataSource"));
            UpdateHotkeyService(SettingsService.Get<bool>("UseLowLevelHotkey"));

            if (!HotkeyManager.InitializeHook())
            {
                _infoBinding.Price = "Hotkey service error.";
            }
            else
            {
                if (HotkeyManager.ActivateHotkey("QueryKey", OnHotKeyHandler))
                {
                    _infoBinding.Price = $"Press {HotkeyManager.GetHotkeyCombo("QueryKey")} to query.";
                }
                else
                {
                    _infoBinding.Price = "Priceall";
                }
            }
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            _settingsWindow = new SettingsWindow(OnHotkeyUpdated);
            InitializeClipboard();
        }

        /// <summary>
        /// Passes MainWindow handle to clipboard helper for setting up later.
        /// </summary>
        private void InitializeClipboard()
        {
            var helper = new WindowInteropHelper(this);
            _clipboard.InitializeListener(HwndSource.FromHwnd(helper.EnsureHandle()));
            _clipboard.ClipboardChanged += QueryAppraisalAsync;
        }

        /// <summary>
        /// Checks for query cooldown and calls for appraisal query.
        /// </summary>
        private async void OnHotKeyHandler()
        {
            await QueryAppraisalAsync();
        }
        #endregion

        #region Settings listener
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DataSource":
                    UpdateAppraisalService(SettingsService.Get<string>(e.PropertyName));
                    break;
                case "UseLowLevelHotkey":
                    UpdateHotkeyService(SettingsService.Get<bool>(e.PropertyName));
                    break;
            }
        }

        private void UpdateAppraisalService(string dataSource)
        {
            foreach (var service in _appraisalServices)
            {
                if (service.GetType().Name == dataSource)
                {
                    AppraisalService = service;
                    return;
                }
            }
        }

        private void UpdateHotkeyService(bool useLowLevelHotkey)
        {
            HotkeyManager = useLowLevelHotkey
                ? new LowLevelHotkeyManager()
                : (IHotkeyManager)new ApiHotkeyManager();
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
            // Check cooldown
            var queryElapsed = (DateTime.Now - _lastQueryTime).TotalMilliseconds;
            if (queryElapsed < SettingsService.Get<int>("QueryCooldown"))
            {
                // Still in cooldown
                return;
            }

            // Do it
            _lastQueryTime = DateTime.Now;
            _infoBinding.Price = "Hold on...";
            _infoBinding.SetTypeIcon("searchmarket");

            var appraisal = await AppraisalService.AppraiseAsync(clipboardContent);
            if (appraisal.Status == AppraisalStatus.Successful)
            {
                // Return OK
                // Color
                _infoBinding.SetTypeIcon(appraisal.Kind);
                if (appraisal.SellValue <= Settings.Default.LowerPrice)
                {
                    _infoBinding.PriceLowerOrHigher = true;
                }
                else if (appraisal.SellValue > Settings.Default.UpperPrice)
                {
                    _infoBinding.PriceLowerOrHigher = false;
                }
                else _infoBinding.PriceLowerOrHigher = null;
                // Prettyprint
                if (SettingsService.Get<bool>("IsUsingPrettyPrint"))
                {
                    _infoBinding.Price = appraisal.FormattedSellValue;
                }
                else
                {
                    _infoBinding.Price = String.Format("{0:N}", appraisal.SellValue);
                }
            }
            else
            {
                // Return error
                _infoBinding.SetTypeIcon("heuristic");
                _infoBinding.PriceLowerOrHigher = null;
                _infoBinding.Price = appraisal.Message;
            }
        }
        #endregion

        #region UI handlers
        /// <summary>
        /// Shuts down the app.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            QuickButton_Click(sender, e);
        }

        /// <summary>
        /// Sets the window to be topmost (overlay).
        /// </summary>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            Topmost = true;
        }

        /// <summary>
        /// Saves config and shuts down the app.
        /// </summary>
        private void QuickButton_Click(object sender, EventArgs e)
        {
            HotkeyManager.UninitializeHook();
            Settings.Default.Save();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Changes window background transparency when scrolls on it.
        /// </summary>
        private void AppraisalInfoGrid_MouseWheel(object sender, MouseWheelEventArgs e)
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
                if (e.Delta > 0) _styleBinding.WindowOpacity += 0.05;
                else _styleBinding.WindowOpacity -= 0.05;
            }
        }

        /// <summary>
        /// Starts window drag move when button is held.
        /// </summary>
        private void DragButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.IsDragEnabled &&
                e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        /// <summary>
        /// Toggles response of drag drop button and save to settings.
        /// </summary>
        private void DragButton_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var currentState = SettingsService.Get<bool>("IsDragEnabled");
            SettingsService.Set("IsDragEnabled", !currentState);
        }

        /// <summary>
        /// Shows the settings window.
        /// </summary>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow.ShowDialog();
        }
        #endregion

        #region Hotkey
        public void OnHotkeyUpdated(KeyCombo keyCombo)
        {
            switch (keyCombo.Name)
            {
                case "QueryKey":
                    {
                        HotkeyManager.ActivateHotkey(keyCombo.Name, keyCombo, OnHotKeyHandler);
                        break;
                    }
            }
        }
        #endregion
    }
}
