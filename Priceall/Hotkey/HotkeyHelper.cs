using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Priceall.Hotkey
{
    /// <summary>
    /// Class for managing multiple hotkeys. Not in use yet.
    /// </summary>
    class HotkeyHelper
    {
        List<Hotkey> _hotkeys = new List<Hotkey>();

        public void CreateNewHotkey(ModifierKeys[] modifierKeys, Key virtualKey, Action action)
        {
            ModifierKeys keyCombo = ModifierKeys.None;

            switch (modifierKeys.Length)
            {
                case 0:
                    return;
                case 1:
                    keyCombo = modifierKeys[0];
                    break;
                case 2:
                    keyCombo = modifierKeys[0] | modifierKeys[1];
                    break;
            }

            _hotkeys.Add(new Hotkey(keyCombo, virtualKey, action));
        }

        public void UnregisterHotkey(int keyId)
        {
            _hotkeys[keyId].Unregister();
        }
    }
}
