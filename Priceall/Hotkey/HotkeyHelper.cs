using Priceall.Properties;
using System;
using System.Linq;
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
        /// Attempts to register a new hotkey. If same-name hotkey exists, they will be overwritten.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="modifierKeys">Modifier key combo of this hotkey.</param>
        /// <param name="virtualKey">Virtual key of this hotkey.</param>
        /// <param name="action">Action for this hotkey.</param>
        /// <returns></returns>
        public bool RegisterNewHotkey(string name, ModifierKeys modifierKeys, Key virtualKey, Action action)
        {
            // remove same-name hotkeys (if any) and overwrite them later
            var hotkeyDupes = ActiveHotkeys.Where(hotkey => hotkey.Name == name);
            foreach (var hotkey in hotkeyDupes)
            {
                hotkey.Unregister();
                ActiveHotkeys.Remove(hotkey);
            }

            // attempt to create a new hotkey
            try
            {
                ActiveHotkeys.Add(new Hotkey(name, modifierKeys, virtualKey, action));
                return true;
            }
            catch (InvalidOperationException) { return false; }
        }

        /// <summary>
        /// Attempts to create a hotkey with the given name from settings.
        /// </summary>
        /// <param name="name">Name (identifier) of the hotkey.</param>
        /// <param name="action">Action for this hotkey.</param>
        /// <returns></returns>
        public bool RegisterHotkeyFromSettings(string name, Action action)
        {
            // see if the key already exists
            if (SavedHotkeys.TryGetValue(name, out string keyInfo) == true)
            {
                // deserialize key info stored as csv
                var keys = keyInfo.Split(',');
                Int32.TryParse(keys[0], out int modKey);
                Int32.TryParse(keys[1], out int virtKey);
                try
                {
                    ActiveHotkeys.Add(new Hotkey(name, (ModifierKeys)modKey, (Key)virtKey, action));
                    return true;
                }
                catch (InvalidOperationException) { return false; }
            }
            else return false;
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
    }
}
