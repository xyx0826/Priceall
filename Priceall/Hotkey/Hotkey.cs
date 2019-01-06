using Priceall.Hotkey.NonHook;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Priceall.Hotkey
{
    public class Hotkey : IDisposable
    {
        public readonly int Id;

        public KeyCombo KeyCombo;

        private Action _action;
        
        private static IntPtr _windowHandle = IntPtr.Zero;

        public void SetAction(Action action) => _action = action;

        public void Invoke() => _action.Invoke();

        #region Constructors
        /// <summary>
        /// Creates hotkey for hooked mode.
        /// </summary>
        public Hotkey(KeyCombo keyCombo, Action action)
        {
            KeyCombo = keyCombo;
            _action = action;
            Id = (int)HotkeyInterop.AddAtom(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Creates hotkey for non-hooked mode.
        /// </summary>
        public Hotkey(string name, KeyCombo keyCombo, Action action, IntPtr windowHandle)
            : this (keyCombo, action)
        {
            KeyCombo.Name = name;
            _windowHandle = windowHandle;
            RegisterNonHook();
        }
        #endregion

        #region Hook mode
        public bool KeysEqual(List<Key> pressedKeys) => KeyCombo.KeysEqual(pressedKeys);
        #endregion

        #region Non-hook (RegisterHotkey) mode
        /// <summary>
        /// In non-keyboard hook mode, register this hotkey.
        /// </summary>
        /// <returns>Whether the registration is successful.</returns>
        public bool RegisterNonHook()
        {
            return HotkeyInterop.RegisterHotKey(_windowHandle, Id,
                KeyCombo.ModifierKeys, KeyInterop.VirtualKeyFromKey(KeyCombo.Key));
        }

        /// <summary>
        /// In non-keyboard hook mode, unregister this hotkey.
        /// </summary>
        /// <returns>Whether the unregistration is successful.</returns>
        public bool UnregisterNonHook()
        {
            return HotkeyInterop.UnregisterHotKey(_windowHandle, Id);
        }
        #endregion

        #region Overrides and impls
        public override string ToString()
        {
            return $"<{KeyCombo.Name} ({KeyCombo.ToString()})>";
        }

        public void Dispose()
        {
            UnregisterNonHook();
            HotkeyInterop.DeleteAtom((uint)Id);
        }
        #endregion
    }
}
