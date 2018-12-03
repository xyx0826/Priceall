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
        // A list of modifier keys.
        static readonly Key[] _modifierKeys = {
            Key.LeftCtrl, Key.RightCtrl,
            Key.LeftShift, Key.RightShift,
            Key.LeftAlt, Key.RightAlt,
            Key.System
        };

        // Runtime hotkeys
        /// <summary>
        /// Stores all hotkeys defined in settings.
        /// These hotkeys are inactive until being assigned actions.
        /// </summary>
        public Dictionary<string, string> SavedHotkeys { get; set; }
        /// <summary>
        /// Stores all valid hotkeys.
        /// These hotkeys have actions assigned and will respond correctly.
        /// </summary>
        public List<Hotkey> ActiveHotkeys { get; set; }

        // Keys tracking
        List<Key> _pressedKeys = new List<Key>();
        List<Key> _recordedKeys = new List<Key>();

        // Hotkey hooking
        const int WH_KEYBOARD_LL = 13;
        IntPtr _currentHook = IntPtr.Zero;
        KeyboardHookProc _callback;

        // Hotkey recording
        private bool _isRecording = false;
        private HotkeyRecorded _recordingCallback;

        public HotkeyHelper()
        {
            // Initialize low-level hook
            if (!Initialize())
                Debug.WriteLine("For some reason hooking failed.");
            // Restore hotkeys
            SavedHotkeys = new Dictionary<string, string>();
            ActiveHotkeys = new List<Hotkey>();
            LoadAllHotkeys();
        }

        #region Hotkey management
        /// <summary>
        /// Attempts to create a new hotkey. If same-name hotkey exists, they will be overwritten.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="keys">Key combo of this hotkey. Includes virtual keys and modifier keys.</param>
        /// <param name="action">Action for this hotkey.</param>
        public void CreateNewHotkey(string name, Key[] keys, Action action)
        {
            // remove same-name hotkeys (if any) and overwrite them later
            var hotkeyDupes = ActiveHotkeys.Where(hotkey => hotkey.Name == name);
            for (int i = 0; i < hotkeyDupes.Count(); i++)
            {
                ActiveHotkeys.Remove(hotkeyDupes.ElementAt(i));
            }

            // attempt to create a new hotkey
            ActiveHotkeys.Add(new Hotkey(name, keys, action));
        }

        /// <summary>
        /// Loads all saved hotkeys from settings.
        /// </summary>
        public void LoadAllHotkeys()
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
        /// Attempts to load a hotkey with the given name from settings.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="action">Action for this hotkey.</param>
        /// <returns></returns>
        public bool TryLoadHotkey(string name, Action action)
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

        /// <summary>
        /// Attempts to find an active hotkey with the specified name.
        /// </summary>
        /// <param name="name">The name of hotkey to be searched for.</param>
        /// <returns>A Hotkey if found, or null.</returns>
        public Hotkey FindHotkeyByName(string name)
        {
            foreach (var hotkey in ActiveHotkeys)
            {
                if (hotkey.Name == name) return hotkey;
            }
            return null;
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
        #endregion

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

        public void StartRecording(HotkeyRecorded callback)
        {
            _isRecording = true;
            _recordingCallback = callback;
        }

        /// <summary>
        /// Callback fired every time a key is pressed.
        /// </summary>
        public int OnHookCall(int code, int wParam, ref KeyboardHook lParam)
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

                // Not recording, use _pressedKeys list to check for hotkey hits
                if (!_isRecording)
                {
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

                    // check whether current key combo hits any hotkey
                    foreach (var activeHotkey in ActiveHotkeys)
                    {
                        if (activeHotkey.IsKeyHit(_pressedKeys.ToArray()))
                        {
                            activeHotkey.Invoke();
                        }
                    }
                }
                // Is recording, use _recordedKeys list to save all keys
                else
                {
                    if (msg != KeyboardMessages.KeyDown
                        && msg != KeyboardMessages.SysKeyDown)
                    {
                        // Key is released, remove it from list
                        _recordedKeys.Remove(key);
                    }
                    else if (_pressedKeys.Contains(key))
                    {
                        // Key repeated event
                        return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
                    }
                    else
                    {
                        // Key pressed event (fresh)
                        _recordedKeys.Add(key);
                        if (!_modifierKeys.Contains(key))
                        {
                            // We've got a non-modifier key, stop recording
                            _recordingCallback(_pressedKeys.ToArray());
                            _isRecording = false;
                        }
                    }
                }
            }

            // Pass messages to next hook listener.
            return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
        }
    }
}
