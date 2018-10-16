using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace Priceall.Hotkey
{
    /// <summary>
    /// Class representing a hotkey registration.
    /// </summary>
    public class Hotkey
    {
        public struct KeyboardHook
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public delegate int keyboardHookProc(int code, int wParam, ref KeyboardHook lParam);

        #region P/Invoke
        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="idHook">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">A pointer to the hook procedure.</param>
        /// <param name="hmod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter.</param>
        /// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc lpfn, IntPtr hmod, uint dwThreadId);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, ref KeyboardHook lParam);

        #endregion

        const int WH_KEYBOARD_LL = 13;

        public string Name;
        public int KeyId;
        public ModifierKeys ModifierKeyCombo;
        public Key VirtualKey;

        public IntPtr currentHook = IntPtr.Zero;

        static Action _keyAction;
        
        /// <summary>
        /// Creates and registers a hotkey of the given keypresses.
        /// </summary>
        /// <param name="modifierKey">A combination of modifier keys.</param>
        /// <param name="virtualkey">A single virtual key.</param>
        /// <param name="action">The action to be executed on hotkey press.</param>
        public Hotkey(string name, ModifierKeys modifierKey, Key virtualkey, Action action)
        {
            Name = name;
            ModifierKeyCombo = modifierKey;
            VirtualKey = virtualkey;
            _keyAction = action;

            if (Register() != true)
            {
                throw new InvalidOperationException("Hotkey registration failed.");
            }
        }
        
        /// <summary>
        /// Registers the hotkey.
        /// </summary>
        /// <returns>Whether the registration is successful.</returns>
        public bool Register()
        {
            currentHook = SetWindowsHookEx(WH_KEYBOARD_LL, OnHookCall, IntPtr.Zero, 0);

            return true;

            // key params to pass: hotkey combo and its unique keyId
            // return RegisterHotKey(IntPtr.Zero, KeyId, 
            //     (uint)ModifierKeyCombo, (uint)virtualKeyCode);
        }

        /// <summary>
        /// Unregisters the hotkey.
        /// </summary>
        public void Unregister()
        {
            // UnregisterHotKey(IntPtr.Zero, KeyId);
            // GC.SuppressFinalize(this);
        }

        public int OnHookCall(int code, int wParam, ref KeyboardHook lParam)
        {
            if (code >= 0)
            {
                if (((KeyboardMessages)wParam) == KeyboardMessages.KeyDown)
                {
                    Debug.WriteLine($"Key pressed: {KeyInfo.GetKey(lParam.vkCode)}");
                }
            }
            return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
        }

        /// <summary>
        /// Converts the key definition of this hotkey to comma-separated form.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name},{(int)ModifierKeyCombo},{(int)VirtualKey}";
        }
    }
}
