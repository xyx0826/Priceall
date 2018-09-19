using Priceall.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Priceall.Hotkey
{
    /// <summary>
    /// Class for managing multiple hotkeys. Not in use yet.
    /// </summary>
    class HotkeyHelper
    {
        public Dictionary<string, string> SavedHotkeys { get; set; }
        public List<Hotkey> ActiveHotkeys { get; set; }

        public HotkeyHelper()
        {
            SavedHotkeys = new Dictionary<string, string>();
            ActiveHotkeys = new List<Hotkey>();
            LoadHotkeys();
        }

        /// <summary>
        /// Creates a hotkey with the given name from settings. If not found, a new one will be created.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="modifierKeyCombo">Combo keypress of modifier keys.</param>
        /// <param name="virtualKey">Keypress of virtual key.</param>
        /// <param name="action">Action for this hotkey.</param>
        /// <returns></returns>
        public bool TryCreateHotkey(string name, ModifierKeys modifierKeyCombo, Key virtualKey, Action action)
        {
            // first, assuse it is a new one
            var modKey = (int)modifierKeyCombo;
            var virtKey = (int)virtualKey;
            // see if the key already exists
            if (SavedHotkeys.TryGetValue(name, out string keyInfo) == true)
            {
                // deserialize key info stored as csv
                var keys = keyInfo.Split(',');
                Int32.TryParse(keys[0], out modKey);
                Int32.TryParse(keys[1], out virtKey);
            }

            ActiveHotkeys.Add(new Hotkey(name, (ModifierKeys)modKey, (Key)virtKey, action));
            return true;
        }

        /// <summary>
        /// Unregisters all currently active hotkeys.
        /// </summary>
        public void UnregisterAllHotkeys()
        {
            foreach (var hotkey in ActiveHotkeys) hotkey.Unregister();
        }

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
                    SavedHotkeys.Add(keyInfo[0], $"{keyInfo[1]},{keyInfo[2]}");
                }
            }
        }

        /// <summary>
        /// Converts every valid hotkey definition into its string form and stores to settings.
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
    }
}
