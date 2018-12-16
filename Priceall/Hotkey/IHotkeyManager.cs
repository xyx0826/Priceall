using System;

namespace Priceall.Hotkey
{
    public interface IHotkeyManager
    {
        bool ActivateHotkey(string name, Action action);

        bool ActivateHotkey(string name, KeyCombo keyCombo, Action action);

        string GetHotkeyCombo(string name);

        bool RemoveHotkey(string name);

        bool InitializeHook();

        bool UninitializeHook();
    }
}
