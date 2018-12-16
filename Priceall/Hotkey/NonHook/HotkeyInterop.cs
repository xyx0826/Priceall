using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Priceall.Hotkey.NonHook
{
    static class HotkeyInterop
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32.dll")]
        public static extern uint AddAtom(string lpString);

        [DllImport("kernel32.dll")]
        public static extern uint DeleteAtom(uint nAtom);
    }
}
