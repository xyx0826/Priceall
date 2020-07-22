using Priceall.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Priceall.Hotkey.NonHook
{
    /// <summary>
    /// A manager for hotkeys implemented through RegisterHotkey API.
    /// </summary>
    class ApiHotkeyManager : IHotkeyManager
    {
        private const uint WM_HOTKEY = 0x0312;

        private static IntPtr _windowHandle;

        private static HwndSourceHook _sourceHook;

        // List of KeyCombos stored in settings to be activated.
        private static List<KeyCombo> _keyCombos = new List<KeyCombo>();

        // Dictionary of all active hotkeys indexed by their IDs.
        private static readonly Dictionary<int, Hotkey> _hotkeys
            = new Dictionary<int, Hotkey>();

        #region Initialization/uninitialization
        public ApiHotkeyManager()
        {
            LoadKeyCombosFromSettings();
        }

        public bool InitializeHook()
        {
            var helper = new WindowInteropHelper(Application.Current.MainWindow);
            _windowHandle = helper.EnsureHandle();
            _sourceHook = new HwndSourceHook(HwndSourceHook);
            HwndSource.FromHwnd(_windowHandle).AddHook(_sourceHook);
            return true;
        }

        public bool UninitializeHook()
        {
            SaveActiveHotkeys();
            foreach (var hotkey in _hotkeys.Values)
            {
                if (!UnregisterHotkey(hotkey))
                {
                    Debug.WriteLine($"Error unregistering hotkey {hotkey}.");
                    return false;
                }
            }
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
                    .Get<StringCollection>("Hotkeys");
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
                    // Register the hotkey
                    var hotkey = new Hotkey(name, keyCombo, action);
                    if (RegisterHotkey(hotkey))
                    {
                        // Registration successful, add to list
                        _hotkeys.Add(hotkey.Id, hotkey);
                        _keyCombos.Remove(keyCombo);
                        return true;
                    }
                    break;
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
            for (int i = _hotkeys.Count - 1; i >= 0; i--)
            {
                var hotkey = _hotkeys.ElementAt(i);
                if (hotkey.Value.KeyCombo.Name == name)
                {
                    hotkey.Value.Dispose();
                    _hotkeys.Remove(hotkey.Key);
                }
            }

            var newHotkey = new Hotkey(keyCombo, action);
            if (RegisterHotkey(newHotkey))
            {
                _hotkeys.Add(newHotkey.Id, newHotkey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to get an active hotkey by its name.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <returns>The key combo of the first found hotkey.</returns>
        public KeyCombo GetHotkeyCombo(string name)
        {
            foreach (var hotkey in _keyCombos)
            {
                if (hotkey.Name == name)
                    return hotkey;
            }
            return KeyCombo.Empty;
        }

        public bool RemoveHotkey(string name)
        {
            foreach (var hotkey in _hotkeys.Values)
            {
                if (hotkey.KeyCombo.Name == name)
                {
                    UnregisterHotkey(hotkey);
                    _hotkeys.Remove(hotkey.Id);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Registers a hotkey to Windows hotkey table.
        /// </summary>
        /// <param name="hk">The hotkey to register.</param>
        /// <returns>Whether the registration is successful.</returns>
        private static bool RegisterHotkey(Hotkey hk)
        {
            if (_windowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The window handle is not initialized.");
            }

            return HotkeyInterop.RegisterHotKey(_windowHandle, hk.Id, hk.KeyCombo.ModifierKeys,
                KeyInterop.VirtualKeyFromKey(hk.KeyCombo.Key));
        }

        /// <summary>
        /// Removes a hotkey from Windows hotkey table.
        /// </summary>
        /// <param name="hk">The hotkey to unregister.</param>
        /// <returns>Whether the unregistration is successful.</returns>
        private static bool UnregisterHotkey(Hotkey hk)
        {
            return HotkeyInterop.UnregisterHotKey(_windowHandle, hk.Id);
        }

        /// <summary>
        /// Converts every active hotkey definition 
        /// into its string form and stores to settings.
        /// </summary>
        public static void SaveActiveHotkeys()
        {
            var store = new StringCollection();
            foreach (var hotkey in _hotkeys.Values)
            {
                if (hotkey.KeyCombo.AllKeys.Count != 0)
                    store.Add(KeyComboUtils.ConvertToSettingValue(hotkey.KeyCombo));
            }
            SettingsService.Set("Hotkeys", store);
        }
    }
}
