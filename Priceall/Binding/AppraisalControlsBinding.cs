﻿using Priceall.Properties;
using System.ComponentModel;
using System.Windows.Media;

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

        public void Refresh()
        {
            SetRectOpacityStyle(Settings.Default.IsDragEnabled);
            OnPropertyChanged(null);
        }
        #endregion

        public AppraisalControlsBinding()
        {
            SetRectOpacityStyle(Settings.Default.IsDragEnabled);
            IsUpdateAvail = false;
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

        private bool _isUpdateAvail;

        public bool IsUpdateAvail
        {
            get { return _isUpdateAvail; }
            set
            {
                _isUpdateAvail = value;
                OnPropertyChanged("RectBackgroundBrush");
            }
        }

        public SolidColorBrush RectBackgroundBrush
        {
            get
            {
                if (IsUpdateAvail)
                {
                    return (SolidColorBrush)
                        (new BrushConverter()
                        .ConvertFrom("#FFA500"));
                }
                else return null;
            }
        }
    }
}
