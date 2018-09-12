using Priceall.Binding;
using Priceall.Helper;
using Priceall.Hotkey;
using Priceall.Properties;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

[assembly: AssemblyVersion("1.0.1")]

namespace Priceall
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly AppraisalInfoBinding _infoBinding = new AppraisalInfoBinding();
        static readonly AppraisalControlsBinding _controlsBinding = new AppraisalControlsBinding();
        static readonly UiOpacityBinding _opacityBinding = new UiOpacityBinding();
        static readonly AppraisalHelper _appraisal = new AppraisalHelper();
        static readonly HotkeyHelper _hotkey = new HotkeyHelper();

        static Window _settingsWindow = new SettingsWindow();
        
        DateTime _lastQueryTime;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Window loading and terminating
        /// <summary>
        /// Registers hotkeys as soon as window handle is fully initialized.
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hotkey = new Hotkey.Hotkey(ModifierKeys.Control | ModifierKeys.Shift, Key.C, OnHotKeyHandler);
        }

        /// <summary>
        /// Checks for query cooldown and calls for appraisal query.
        /// </summary>
        private async void OnHotKeyHandler()
        {
            var queryElapsed = (DateTime.Now - _lastQueryTime).TotalMilliseconds;
            if (queryElapsed >= Settings.Default.QueryCooldown)
            {
                _lastQueryTime = DateTime.Now;
                await QueryAppraisal();
            }
        }

        /// <summary>
        /// Binds data contexts as soon as window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Top = Settings.Default.WindowTopPos;
            Left = Settings.Default.WindowLeftPos;

            _settingsWindow.Owner = this;

            DataContext = _opacityBinding;
            AppraisalInfo.DataContext = _infoBinding;
            AppraisalControls.DataContext = _controlsBinding;
        }

        /// <summary>
        /// Saves config and shuts down the app.
        /// </summary>
        private void AppShutdown(object sender, RoutedEventArgs e)
        {
            Settings.Default.WindowTopPos = Top;
            Settings.Default.WindowLeftPos = Left;
            Settings.Default.WindowOpacity = _opacityBinding.WndOpacity;

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
            var clipboardContent = ClipboardHelper.ReadClipboardText();
            await Task.Run(() => { QueryAppraisal(clipboardContent); });
        }

        /// <summary>
        /// Queries appraisal, triggered by button.
        /// </summary>
        private async void QueryAppraisal(object sender, RoutedEventArgs e)
        {
            var clipboardContent = ClipboardHelper.ReadClipboardText();
            await Task.Run(() => { QueryAppraisal(clipboardContent); });
        }

        private async void QueryAppraisal(string clipboardContent)
        {
            _infoBinding.Price = "Hold on...";
            _infoBinding.SetTypeIcon("searchmarket");

            var appraisal = await _appraisal.QueryAppraisal(clipboardContent);
            var json = new Json(appraisal);

            if (!String.IsNullOrEmpty(json.ErrorMessage))
            {
                _infoBinding.SetTypeIcon("heuristic");
                _infoBinding.Price = json.ErrorMessage;
            }
            else
            {
                _infoBinding.SetTypeIcon(json.Kind);
                _infoBinding.Price = json.SellValue.ToString("C2");
            }
        }

        /// <summary>
        /// Reads clipboard on main thread for COM compliance.
        /// </summary>
        /// <returns>Clipboard content, or empty string if clipboard does not have text.</returns>
        private string ReadClipboardOnMainThread()
        {
            return ClipboardHelper.ReadClipboardText();
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
            if (e.Delta > 0) _opacityBinding.WndOpacity += 0.05;
            else _opacityBinding.WndOpacity -= 0.05;
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
        /// Shows the settings window.
        /// </summary>
        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            _settingsWindow.ShowDialog();
        }
        #endregion
    }
}
