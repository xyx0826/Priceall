using Priceall.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Priceall.Hotkey.NonHook
{
    class HotkeyManager : IHotkeyManager
    {
        private const uint WM_HOTKEY = 0x0312;

        private static IntPtr _windowHandle;

        private static HwndSourceHook _sourceHook;

        private static Dispatcher _delegateDispatcher;

        // List of KeyCombos stored in settings to be activated.
        private static List<KeyCombo> _keyCombos = new List<KeyCombo>();

        // Dictionary of all active hotkeys indexed by their IDs.
        private static readonly Dictionary<int, Hotkey> _hotkeys
            = new Dictionary<int, Hotkey>();

        #region Initialization/uninitialization
        public HotkeyManager()
        {
            LoadKeyCombosFromSettings();
        }

        public bool InitializeHook()
        {
            var helper = new WindowInteropHelper(Application.Current.MainWindow);
            _windowHandle = helper.EnsureHandle();
            _sourceHook = new HwndSourceHook(HwndSourceHook);
            _delegateDispatcher = Dispatcher.CurrentDispatcher;
            HwndSource.FromHwnd(_windowHandle).AddHook(_sourceHook);
            return true;
        }

        public bool UninitializeHook()
        {
            HwndSource.FromHwnd(_windowHandle).RemoveHook(_sourceHook);
            return true;
        }

        private static IntPtr HwndSourceHook(
            IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                handled = true;
                _hotkeys[wParam.ToInt32()].Invoke();
            }
            return IntPtr.Zero;
        }

        public void LoadKeyCombosFromSettings()
        {
            var savedHotkeys = SettingsService
                    .GetSetting<StringCollection>("Hotkeys");
            if (savedHotkeys != null)
            {
                foreach (var hotkey in savedHotkeys)
                {
                    var keyCombo = KeyCombo.Empty;
                    try
                    {
                        keyCombo = KeyComboUtils.ConvertFromSettingValue(hotkey);
                    }
                    finally
                    {
                        _keyCombos.Add(keyCombo);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Activates a hotkey using settings-stored KeyCombo.
        /// </summary>
        public bool ActivateHotkey(string name, Action action)
        {
            foreach (KeyCombo keyCombo in _keyCombos)
            {
                if (keyCombo.Name == name)
                {
                    var hotkey = new Hotkey(name, keyCombo, action, _windowHandle);
                    _hotkeys.Add(hotkey.Id, hotkey);
                    _keyCombos.Remove(keyCombo);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Activates or replaces a hotkey using a new KeyCombo.
        /// </summary>
        public bool ActivateHotkey(string name, KeyCombo keyCombo, Action action)
        {
            // Remove all same-name hotkeys
            var hotkeyDupes = _hotkeys.Values.Where(hotkey => hotkey.KeyCombo.Name == name);
            foreach (var hotkeyDupe in hotkeyDupes)
                _hotkeys.Remove(hotkeyDupe.Id);

            var newHotkey = new Hotkey(keyCombo, action);
            _hotkeys.Add(newHotkey.Id, newHotkey);
            return true;
        }

        /// <summary>
        /// Attempts to get an active hotkey by its name.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <returns>The key combo of the first found hotkey.</returns>
        public string GetHotkeyCombo(string name)
        {
            foreach (var hotkey in _hotkeys.Values)
            {
                if (hotkey.KeyCombo.Name == name)
                    return hotkey.KeyCombo.ToString();
            }
            return "N/A";
        }

        public bool RemoveHotkey(string name)
        {
            foreach (var hotkey in _hotkeys.Values)
            {
                if (hotkey.KeyCombo.Name == name)
                {
                    _hotkeys.Remove(hotkey.Id);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts every active hotkey definition 
        /// into its string form and stores to settings.
        /// </summary>
        public static void SaveActiveHotkeys()
        {
            var hotkeys = new StringCollection();
            foreach (var hotkey in _hotkeys.Values)
            {
                if (hotkey.KeyCombo.AllKeys.Count != 0)
                    hotkeys.Add(KeyComboUtils
                        .ConvertToSettingValue(hotkey.KeyCombo));
            }
            SettingsService.SetSetting("Hotkeys", hotkeys);
        }
    }
}
