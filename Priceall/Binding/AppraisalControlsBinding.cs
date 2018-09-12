using Priceall.Properties;
using System.ComponentModel;

namespace Priceall.Binding
{
    class AppraisalControlsBinding : INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public AppraisalControlsBinding()
        {
            SetRectOpacityStyle(Settings.Default.IsDragEnabled);
        }

        public double RectOpacity { get; set; }

        /// <summary>
        /// Sets drag drop button opacity based on its state.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void SetRectOpacityStyle(bool enabled)
        {
            if (enabled) RectOpacity = 1.0;
            else RectOpacity = 0.4;
            OnPropertyChanged("RectOpacity");
        }
    }
}
