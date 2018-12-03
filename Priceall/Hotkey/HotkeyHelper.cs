using Priceall.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Priceall.Hotkey
{
    /// <summary>
    /// Class for managing multiple hotkeys, or record new hotkeys.
    /// </summary>
    internal static class HotkeyHelper
    {
        #region Constant (variables)
        private const int WH_KEYBOARD_LL = 13;

        private static readonly Key[] _modifierKeys = {
            Key.LeftCtrl, Key.RightCtrl,
            Key.LeftShift, Key.RightShift,
            Key.LeftAlt, Key.RightAlt,
            Key.System
        };
        #endregion

        #region Runtime hotkey and key lists
        // List of all active hotkeys.
        private static List<Hotkey> _hotkeys = new List<Hotkey>();
        // List of currently held keys.
        private static List<Key> _pressedKeys = new List<Key>();
        #endregion

        private static IntPtr _currentHook = IntPtr.Zero;
        private static bool _isRecording = false;
        private static HotkeyRecorded _recordingCallback;

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

        // Delegate (callback) required for returning the new hotkey.
        public delegate void HotkeyRecorded(Key[] keyCombo);

        // Instance of keyboard hook delegate to prevent it from being GC'd.
        private static KeyboardHookProc _callback;
        #endregion

        #region Initialization/uninitialization
        /// <summary>
        /// Starts global hook to listen for keypresses.
        /// </summary>
        /// <returns>Whether the registration is successful.</returns>
        public static bool Initialize()
        {
            _callback = OnHookCall;
            _currentHook = SetWindowsHookEx(WH_KEYBOARD_LL, _callback, IntPtr.Zero, 0);
            return _currentHook == IntPtr.Zero;
        }

        /// <summary>
        /// Uninstalls the global windows hook.
        /// </summary>
        /// <returns>Whether the uninitialization is successful.</returns>
        public static bool Uninitialize()
        {
            return UnhookWindowsHookEx(_currentHook);
        }
        #endregion
        
        #region Hotkey management
        /// <summary>
        /// Attempts to create a new hotkey. If same-name hotkey exists, they will be overwritten.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="keys">Key combo of this hotkey. Includes virtual keys and modifier keys.</param>
        /// <param name="action">Action for this hotkey.</param>
        public static void CreateNewHotkey(string name, Key[] keys, Action action)
        {
            // remove same-name hotkeys (if any) and overwrite them later
            var hotkeyDupes = _hotkeys.Where(hotkey => hotkey.Name == name);
            for (int i = 0; i < hotkeyDupes.Count(); i++)
            {
                _hotkeys.Remove(hotkeyDupes.ElementAt(i));
            }

            // attempt to create a new hotkey
            _hotkeys.Add(new Hotkey(name, keys, action));
        }

        /// <summary>
        /// Attempts to activate a hotkey with the given name from settings.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="action">Action for this hotkey.</param>
        /// <returns></returns>
        public static bool ActivateHotkeyFromSettings(string name, Action action)
        {
            var savedHotkeys = SettingsService
                    .GetSetting<StringCollection>("Hotkeys");
            if (savedHotkeys != null)
            {
                // Saved hotkey list is not empty
                foreach (var hotkey in savedHotkeys)
                {
                    var keyInfo = hotkey.Split(',');
                    if (keyInfo[0] == name)
                    {
                        // Found the hotkey by name in settings
                        _hotkeys.Add(new Hotkey(keyInfo[0], keyInfo[1], action));
                        return true;
                    }
                }
            }
            // Hotkey list is empty or hotkey not found
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
                hotkeys.Add(hotkey.ToString());
            }
            SettingsService.SetSetting("Hotkeys", hotkeys);
        }

        /// <summary>
        /// Puts hotkey service in recording mode to capture a new hotkey combo.
        /// </summary>
        /// <param name="callback">Callback method to be called 
        /// when the recording finishes.</param>
        public static void StartRecording(HotkeyRecorded callback)
        {
            _isRecording = true;
            _recordingCallback = callback;
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

                if (msg != KeyboardMessages.KeyDown
                    && msg != KeyboardMessages.SysKeyDown)
                {
                    // Key released event
                    _pressedKeys.Remove(key);
                }
                else if (_pressedKeys.Contains(key))
                {
                    // Key repeated event
                    return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
                }
                else
                {
                    // Key pressed event (fresh)
                    _pressedKeys.Add(key);

                    if (!_isRecording)
                    {
                        // Not recording, check for hotkey hits
                        foreach (var activeHotkey in _hotkeys)
                        {
                            if (activeHotkey.IsKeyHit(_pressedKeys.ToArray()))
                            {
                                activeHotkey.Invoke();
                            }
                        }
                    }
                    else if (!_modifierKeys.Contains(key))
                    {
                        // Recording, check for key combo completion
                        _isRecording = false;
                        _recordingCallback(_pressedKeys.ToArray());
                    }
                }
            }

            // Pass messages to next hook listener.
            return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
        }
    }
}
