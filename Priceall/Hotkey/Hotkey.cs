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
    public class Hotkey : IDisposable
    {
        #region P/Invoke
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        const int WM_HOTKEY = 0x0312;

        int _keyRegId;
        static Action _hotkeyAction;
        bool _disposed = false;
        
        public Hotkey(ModifierKeys modifierKey, Key virtualkey, Action action)
        {
            Register(modifierKey, virtualkey, action);
        }
        
        public bool Register(ModifierKeys modifierKey, Key virtualKey, Action action)
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(virtualKey);
            _keyRegId = virtualKeyCode + ((int)modifierKey * 0x10000);
            bool result = RegisterHotKey(IntPtr.Zero, _keyRegId, (uint)modifierKey, (uint)virtualKeyCode);

            _hotkeyAction = action;

            ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(OnTfm);
            
            Debug.Print(result.ToString() + ", " + _keyRegId + ", " + virtualKeyCode);

            return result;
        }
        
        public void Unregister()
        {
            UnregisterHotKey(IntPtr.Zero, _keyRegId);
        }

        private static void OnTfm(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY && !handled)
            {
                _hotkeyAction.Invoke();
                handled = true;
            }
        }

        public void Dispose()
        {
            if (!_disposed) Unregister();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
