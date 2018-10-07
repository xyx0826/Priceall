using System;
using System.Windows.Input;

namespace Priceall.Events
{
    public class UiEvents
    {
        #region Singleton impl
        private static UiEvents _singleInstance;

        private UiEvents() { }

        public static UiEvents Instance
        {
            get
            {
                if (_singleInstance == null) _singleInstance = new UiEvents();
                return _singleInstance;
            }
        }
        #endregion

        public event EventHandler AutoRefreshToggled;

        protected virtual void OnAutoRefreshToggled(EventArgs e)
        {
            AutoRefreshToggled?.Invoke(this, e);
        }

        public void ToggleAutoRefresh()
        {
            OnAutoRefreshToggled(EventArgs.Empty);
        }

        public event EventHandler<QueryHotkeyUpdatedEventArgs> QueryHotkeyUpdated;

        protected virtual void OnQueryHotkeyUpdated(QueryHotkeyUpdatedEventArgs e)
        {
            QueryHotkeyUpdated?.Invoke(this, e);
        }

        public void UpdateQueryHotkey(QueryHotkeyUpdatedEventArgs e)
        {
            OnQueryHotkeyUpdated(e);
        }

        public class QueryHotkeyUpdatedEventArgs : EventArgs
        {
            public ModifierKeys ModKeys { get; set; }
            public Key VirtKey { get; set; }
        }

        public event EventHandler PriceColorChanged;

        protected virtual void OnPriceColorChanged(EventArgs e)
        {
            PriceColorChanged?.Invoke(this, e);
        }

        public void ChangePriceColor()
        {
            OnPriceColorChanged(EventArgs.Empty);
        }
    }
}
