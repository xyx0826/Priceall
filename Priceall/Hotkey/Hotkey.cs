using System;
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
        #region P/Invoke
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        const int WM_HOTKEY = 0x0312;

        public string Name;
        public int KeyId;
        public ModifierKeys ModifierKeyCombo;
        public Key VirtualKey;

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
            var virtualKeyCode = KeyInterop.VirtualKeyFromKey(VirtualKey);
            KeyId = virtualKeyCode + ((int)ModifierKeyCombo * 0x10000);
            
            ComponentDispatcher.ThreadFilterMessage += (ref MSG msg, ref bool handled) =>
            {
                if (msg.message == WM_HOTKEY && !handled)
                {
                    _keyAction.Invoke();
                    handled = true;
                }
            };

            // key params to pass: hotkey combo and its unique keyId
            return RegisterHotKey(IntPtr.Zero, KeyId, 
                (uint)ModifierKeyCombo, (uint)virtualKeyCode);
        }

        /// <summary>
        /// Unregisters the hotkey.
        /// </summary>
        public void Unregister()
        {
            UnregisterHotKey(IntPtr.Zero, KeyId);
            GC.SuppressFinalize(this);
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
