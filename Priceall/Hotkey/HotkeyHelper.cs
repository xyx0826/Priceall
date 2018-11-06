using Priceall.Properties;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Priceall.Hotkey
{
    /// <summary>
    /// Class for managing multiple hotkeys, or record new hotkeys.
    /// </summary>
    class HotkeyHelper
    {
        // Runtime hotkeys
        public Dictionary<string, string> SavedHotkeys { get; set; }
        public List<Hotkey> ActiveHotkeys { get; set; }

        // Keys tracking
        List<Key> _pressedKeys = new List<Key>();

        // Hotkey hooking
        const int WH_KEYBOARD_LL = 13;
        IntPtr _currentHook = IntPtr.Zero;
        KeyboardHookProc _callback;

        public HotkeyHelper()
        {
            // Initialize low-level hook
            Initialize();
            // Restore hotkeys
            SavedHotkeys = new Dictionary<string, string>();
            ActiveHotkeys = new List<Hotkey>();
            LoadHotkeys();
        }

        /// <summary>
        /// Attempts to register a new hotkey. If same-name hotkey exists, they will be overwritten.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="keys">Key combo of this hotkey. Includes virtual keys and modifier keys.</param>
        /// <param name="action">Action for this hotkey.</param>
        /// <returns></returns>
        public bool RegisterNewHotkey(string name, Key[] keys, Action action)
        {
            // remove same-name hotkeys (if any) and overwrite them later
            var hotkeyDupes = ActiveHotkeys.Where(hotkey => hotkey.Name == name);
            for (int i = 0; i < hotkeyDupes.Count(); i ++)
            {
                ActiveHotkeys.Remove(hotkeyDupes.ElementAt(i));
            }

            // attempt to create a new hotkey
            try
            {
                ActiveHotkeys.Add(new Hotkey(name, keys, action));
                return true;
            }
            catch (InvalidOperationException) { return false; }
        }

        /// <summary>
         /// Attempts to load a hotkey with the given name from settings.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="action">Action for this hotkey.</param>
        /// <returns></returns>
        public bool LoadHotkeyFromSettings(string name, Action action)
        {
            // see if the key already exists
            if (SavedHotkeys.TryGetValue(name, out string keyInfo) == true)
            {
                try
                {
                    ActiveHotkeys.Add(new Hotkey(name, keyInfo, action));
                    return true;
                }
                catch (InvalidOperationException) { return false; }
            }
            else return false;
        }

        #region Hotkey saving & loading
        /// <summary>
        /// Loads all saved hotkeys from settings.
        /// </summary>
        public void LoadHotkeys()
        {
            if (Settings.Default.Hotkeys != null)
            {
                foreach (var hotkey in Settings.Default.Hotkeys)
                {
                    var keyInfo = hotkey.Split(',');
                    SavedHotkeys.Add(hotkey.Split(',')[0], 
                        hotkey.Substring(hotkey.IndexOf(',') + 1));
                }
            }
        }

        /// <summary>
        /// Converts every active hotkey definition into its string form and stores to settings.
        /// </summary>
        public void SaveHotkeys()
        {
            var hotkeys = new StringCollection();
            foreach (var hotkey in ActiveHotkeys)
            {
                hotkeys.Add(hotkey.ToString());
            }
            Settings.Default.Hotkeys = hotkeys;
        }
        #endregion

        #region P/Invoke & hooking
        // Initializes a global windows hook.
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc lpfn, IntPtr hmod, uint dwThreadId);

        // Passes hook event to the next listener.
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, ref KeyboardHook lParam);

        // Uninitializes the global windows hook.
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // Delegate (callback) required for installing the hook.
        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHook lParam);

        // Delegate (callback) required for returning the new hotkey.
        public delegate void HotkeyRecorded(Key[] keyCombo);

        /// <summary>
        /// Starts global hook to listen for keypresses.
        /// </summary>
        /// <returns>Whether the registration is successful.</returns>
        public bool Initialize()
        {
            _callback = OnHookCall;
            _currentHook = SetWindowsHookEx(WH_KEYBOARD_LL, _callback, IntPtr.Zero, 0);
            return _currentHook != IntPtr.Zero;
        }

        /// <summary>
        /// Uninstalls the global windows hook.
        /// </summary>
        /// <returns>Whether the uninitialization is successful.</returns>
        public bool Uninitialize()
        {
            return UnhookWindowsHookEx(_currentHook);
        }

        /// <summary>
        /// Callback fired every time a key is pressed.
        /// </summary>
        public int OnHookCall(int code, int wParam, ref KeyboardHook lParam)
        {
            if (code >= 0)
            {
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
                }

                foreach (var activeHotkey in ActiveHotkeys)
                {
                    if (activeHotkey.IsKeyHit(_pressedKeys.ToArray()))
                    {
                        activeHotkey.Invoke();
                    }
                }
            }
            return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
        }
        #endregion
    }
}
