using Priceall.Hotkey.NonHook;
using System;

namespace Priceall.Hotkey
{
    /// <summary>
    /// A hotkey combo and its corresponding action.
    /// </summary>
    public class Hotkey : IDisposable
    {
        /// <summary>
        /// The random unique ID of the hotkey.
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// The key combo of the hotkey.
        /// </summary>
        public KeyCombo KeyCombo;

        /// <summary>
        /// The action fired by this hotkey.
        /// </summary>
        private Action _action;

        private bool _disposed;

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
        public Hotkey(string name, KeyCombo keyCombo, Action action)
            : this (keyCombo, action)
        {
            KeyCombo.Name = name;
        }

        public void SetAction(Action action) => _action = action;

        public void Invoke() => _action.Invoke();

        public override string ToString()
        {
            return $"<{KeyCombo.Name} ({KeyCombo})>";
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                HotkeyInterop.DeleteAtom((uint)Id);
            }

            _disposed = true;
        }
        #endregion
    }
}
