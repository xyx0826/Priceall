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

        #region Event handlers
        // Toggle auto refresh
        public event EventHandler AutoRefreshToggled;

        protected virtual void OnAutoRefreshToggled(EventArgs e)
        {
            AutoRefreshToggled?.Invoke(this, e);
        }

        public void ToggleAutoRefresh()
        {
            OnAutoRefreshToggled(EventArgs.Empty);
        }
        #endregion
    }
}
