using Priceall.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Priceall.Hotkey.Hook
{
    /// <summary>
    /// Class for managing multiple hotkeys, or record new hotkeys.
    /// </summary>
    internal class HotkeyManager : IHotkeyManager
    {
        #region Constant (variables)
        private const int WH_KEYBOARD_LL = 13;

        private static readonly Key[] _modifierKeys = {
            Key.LeftCtrl, Key.RightCtrl,
            Key.LeftShift, Key.RightShift,
            Key.LeftAlt, Key.RightAlt,
            Key.LWin, Key.RWin,
            Key.System
        };
        #endregion

        #region Runtime hotkey and key lists
        // List of KeyCombos stored in settings to be activated.
        private static List<KeyCombo> _keyCombos = new List<KeyCombo>();
        // List of all active hotkeys.
        private static List<Hotkey> _hotkeys = new List<Hotkey>();
        // List of currently held keys.
        private static List<Key> _pressedKeys = new List<Key>();
        #endregion

        private static IntPtr _currentHook = IntPtr.Zero;

        #region P/Invoke functions and delegate methods
        // Initializes a global windows hook.
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc lpfn, IntPtr hmod, uint dwThreadId);

        // Passes hook event to the next listener.
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, ref KeyboardHook lParam);

        // Uninitializes the global windows hook.
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // Delegate (callback) required for installing the hook.
        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHook lParam);

        // Instance of keyboard hook delegate to prevent it from being GC'd.
        private static KeyboardHookProc _callback;
        #endregion
        
        #region Initialization/uninitialization

        public HotkeyManager()
        {
            LoadKeyCombosFromSettings();
        }

        /// <summary>
        /// Installs the global window hook.
        /// </summary>
        /// <returns>Whether the initialization is successful.</returns>
        public bool InitializeHook()
        {
            _callback = OnHookCall;
            _currentHook = SetWindowsHookEx(WH_KEYBOARD_LL, _callback, IntPtr.Zero, 0);
            return _currentHook != IntPtr.Zero;
        }

        /// <summary>
        /// Uninstalls the global windows hook.
        /// </summary>
        /// <returns>Whether the uninitialization is successful.</returns>
        public bool UninitializeHook()
        {
            SaveActiveHotkeys();
            return UnhookWindowsHookEx(_currentHook);
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

        #region Hotkey management
        /// <summary>
        /// Activates a hotkey using settings-stored KeyCombo.
        /// </summary>
        public bool ActivateHotkey(string name, Action action)
        {
            foreach (KeyCombo keyCombo in _keyCombos)
            {
                if (keyCombo.Name == name)
                {
                    _hotkeys.Add(new Hotkey(keyCombo, action));
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
            for (int i = _hotkeys.Count - 1; i >= 0; i --)
            {
                if (_hotkeys[i].KeyCombo.Name == name)
                {
                    _hotkeys[i].Dispose();
                    _hotkeys.RemoveAt(i);
                }
            }
            
            _hotkeys.Add(new Hotkey(keyCombo, action));
            return true;
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
            foreach (var hotkey in _hotkeys)
            {
                if (hotkey.KeyCombo.Name == name)
                {
                    _hotkeys.Remove(hotkey);
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
            foreach (var hotkey in _hotkeys)
            {
                if (hotkey.KeyCombo.AllKeys.Count != 0)
                    hotkeys.Add(KeyComboUtils
                        .ConvertToSettingValue(hotkey.KeyCombo));
            }
            SettingsService.SetSetting("Hotkeys", hotkeys);
        }
        #endregion
        
        /// <summary>
        /// Callback fired every time a key is pressed.
        /// </summary>
        public static int OnHookCall(int code, int wParam, ref KeyboardHook lParam)
        {
            if (code >= 0)
            {
                // Parse all parameters
                var msg = (KeyboardMessages)wParam;
                var key = KeyInterop.KeyFromVirtualKey(lParam.vkCode);
                // Interop doesn't catch alt keys correctly
                switch (lParam.vkCode)
                {
                    case 0xa4:
                        key = Key.LeftAlt;
                        break;
                    case 0xa5:
                        key = Key.RightAlt;
                        break;
                }

                if (msg == KeyboardMessages.KeyDown || msg == KeyboardMessages.SysKeyDown)
                {
                    // A key is pressed
                    if (_pressedKeys.Contains(key))
                        // But the key is duplicate, so throw it out
                        return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
                    else if (!_modifierKeys.Contains(key))
                    {
                        // But the key isn't duplicate, it is not a modifier key
                        // The key combo is finished
                        _pressedKeys.Add(key);
                        Debug.WriteLine($"New key added; now {String.Join(" + ", _pressedKeys)}");
                            // Not recording a combo, check for key hits
                            foreach (var activeHotkey in _hotkeys)
                            {
                                if (activeHotkey.KeysEqual(_pressedKeys))
                                {
                                    activeHotkey.Invoke();
                                }
                            }
                        // Wipe the now processed key combo
                        // _pressedKeys.Clear();
                    }
                    else
                    {
                        // But the key is a modifier key, combo is not done
                        _pressedKeys.Add(key);
                    }
                }
                // A key is released, try to remove it
                else _pressedKeys.Remove(key);
            }

            // Pass messages to next hook listener.
            return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
        }
    }
}
